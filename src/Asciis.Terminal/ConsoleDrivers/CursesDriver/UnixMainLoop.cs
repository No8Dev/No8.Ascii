using System.Runtime.InteropServices;
using Asciis.Terminal.Core;

namespace Asciis.Terminal.ConsoleDrivers.CursesDriver
{
    /// <summary>
    /// Unix main loop, suitable for using on Posix systems
    /// </summary>
    /// <remarks>
    /// In addition to the general functions of the mainloop, the Unix version
    /// can watch file descriptors using the AddWatch methods.
    /// </remarks>
    internal class UnixMainLoop : IMainLoopDriver
    {
        public const int KEY_RESIZE = unchecked((int)0xffffffffffffffff);

        [StructLayout(LayoutKind.Sequential)]
        private struct Pollfd
        {
            public int fd;
            public short events, revents;
        }

        /// <summary>
        ///   Condition on which to wake up from file descriptor activity.  These match the Linux/BSD poll definitions.
        /// </summary>
        [Flags]
        public enum Condition : short
        {
            /// <summary>
            /// There is data to read
            /// </summary>
            PollIn = 1,

            /// <summary>
            /// Writing to the specified descriptor will not block
            /// </summary>
            PollOut = 4,

            /// <summary>
            /// There is urgent data to read
            /// </summary>
            PollPri = 2,

            /// <summary>
            ///  Error condition on output
            /// </summary>
            PollErr = 8,

            /// <summary>
            /// Hang-up on output
            /// </summary>
            PollHup = 16,

            /// <summary>
            /// File descriptor is not open.
            /// </summary>
            PollNval = 32
        }

        private class Watch
        {
            public int File;
            public Condition Condition;
            public Func<MainLoop, bool> Callback;
        }

        private Dictionary<int, Watch> descriptorWatchers = new();

        [DllImport("libc")]
        private static extern int poll([In][Out] Pollfd[] ufds, uint nfds, int timeout);

        [DllImport("libc")]
        private static extern int pipe([In][Out] int[] pipes);

        [DllImport("libc")]
        private static extern int read(int fd, IntPtr buf, IntPtr n);

        [DllImport("libc")]
        private static extern int write(int fd, IntPtr buf, IntPtr n);

        private Pollfd[] pollmap;
        private bool poll_dirty = true;
        private int[] wakeupPipes = new int[2];
        private static IntPtr ignore = Marshal.AllocHGlobal(1);
        private MainLoop mainLoop;
        private bool winChanged;

        public Action WinChanged;

        void IMainLoopDriver.Wakeup() { write(wakeupPipes[1], ignore, (IntPtr)1); }

        void IMainLoopDriver.Setup(MainLoop mainLoop)
        {
            this.mainLoop = mainLoop;
            pipe(wakeupPipes);
            AddWatch(
                wakeupPipes[0],
                Condition.PollIn,
                ml =>
                {
                    read(wakeupPipes[0], ignore, (IntPtr)1);
                    return true;
                });
        }

        /// <summary>
        ///   Removes an active watch from the mainloop.
        /// </summary>
        /// <remarks>
        ///   The token parameter is the value returned from AddWatch
        /// </remarks>
        public void RemoveWatch(object token)
        {
            var watch = token as Watch;
            if (watch == null)
                return;
            descriptorWatchers.Remove(watch.File);
        }

        /// <summary>
        ///  Watches a file descriptor for activity.
        /// </summary>
        /// <remarks>
        ///  When the condition is met, the provided callback
        ///  is invoked.  If the callback returns false, the
        ///  watch is automatically removed.
        ///
        ///  The return value is a token that represents this watch, you can
        ///  use this token to remove the watch by calling RemoveWatch.
        /// </remarks>
        public object AddWatch(int fileDescriptor, Condition condition, Func<MainLoop, bool> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            var watch = new Watch() { Condition = condition, Callback = callback, File = fileDescriptor };
            descriptorWatchers[fileDescriptor] = watch;
            poll_dirty = true;
            return watch;
        }

        private void UpdatePollMap()
        {
            if (!poll_dirty)
                return;
            poll_dirty = false;

            pollmap = new Pollfd[descriptorWatchers.Count];
            var i = 0;
            foreach (var fd in descriptorWatchers.Keys)
            {
                pollmap[i].fd = fd;
                pollmap[i].events = (short)descriptorWatchers[fd].Condition;
                i++;
            }
        }

        bool IMainLoopDriver.EventsPending(bool wait)
        {
            if (CheckTimers(wait, out var pollTimeout)) return true;

            UpdatePollMap();

            var n = poll(pollmap, (uint)pollmap.Length, pollTimeout);

            if (n == KEY_RESIZE) winChanged = true;
            return n >= KEY_RESIZE || CheckTimers(wait, out pollTimeout);
        }

        private bool CheckTimers(bool wait, out int pollTimeout)
        {
            var now = DateTime.UtcNow.Ticks;

            var firstTimeout = mainLoop.FirstTimeout();
            if (firstTimeout != long.MaxValue)
            {
                pollTimeout = (int)((firstTimeout - now) / TimeSpan.TicksPerMillisecond);
                if (pollTimeout < 0) 
                    return true;
            }
            else
            {
                pollTimeout = -1;
            }

            if (!wait)
                pollTimeout = 0;

            int ic = mainLoop.IdleCount();
            return ic > 0;
        }

        void IMainLoopDriver.MainIteration()
        {
            if (winChanged)
            {
                winChanged = false;
                WinChanged?.Invoke();
            }

            if (pollmap != null)
                foreach (var p in pollmap)
                {
                    if (p.revents == 0)
                        continue;

                    if (!descriptorWatchers.TryGetValue(p.fd, out var watch))
                        continue;
                    if (!watch.Callback(mainLoop))
                        descriptorWatchers.Remove(p.fd);
                }
        }
    }
}