using System.Threading;
using System.Threading.Tasks;

namespace Asciis.Terminal.ConsoleDrivers;

/// <summary>
/// Mainloop intended to be used with the <see cref="WindowsDriver"/>, and can
/// only be used on Windows.
/// </summary>
/// <remarks>
/// This implementation is used for WindowsDriver.
/// </remarks>
internal class WindowsMainLoop : IMainLoopDriver
{
    private ManualResetEventSlim eventReady = new(false);
    private ManualResetEventSlim waitForProbe = new(false);
    private ManualResetEventSlim winChange = new(false);
    private MainLoop mainLoop;
    private ConsoleDriver consoleDriver;
    private WindowsConsole winConsole;
    private bool winChanged;
    private Size windowSize;
    private CancellationTokenSource tokenSource = new();

    // The records that we keep fetching
    private WindowsConsole.InputRecord[] result = new WindowsConsole.InputRecord[1];

    /// <summary>
    /// Invoked when a Key is pressed or released.
    /// </summary>
    public Action<WindowsConsole.InputRecord> ProcessInput;

    /// <summary>
    /// Invoked when the window is changed.
    /// </summary>
    public Action<Size> WinChanged;

    public WindowsMainLoop(ConsoleDriver consoleDriver = null)
    {
        this.consoleDriver = consoleDriver ??
                             throw new ArgumentNullException("Console driver instance must be provided.");
        winConsole = ((WindowsDriver)consoleDriver).WinConsole;
    }

    void IMainLoopDriver.Setup(MainLoop mainLoop)
    {
        this.mainLoop = mainLoop;
        Task.Run(WindowsInputHandler);
        Task.Run(CheckWinChange);
    }

    private void WindowsInputHandler()
    {
        while (true)
        {
            waitForProbe.Wait();
            waitForProbe.Reset();

            result = winConsole.ReadConsoleInput();

            eventReady.Set();
        }
    }

    private void CheckWinChange()
    {
        while (true)
        {
            winChange.Wait();
            winChange.Reset();
            WaitWinChange();
            winChanged = true;
            eventReady.Set();
        }
    }

    private void WaitWinChange()
    {
        while (true)
        {
            Thread.Sleep(100);
            if (!consoleDriver.HeightAsBuffer)
            {
                windowSize = winConsole.GetConsoleBufferWindow(out _);
                //System.Diagnostics.Debug.WriteLine ($"{consoleDriver.HeightAsBuffer},{windowSize.Width},{windowSize.Height}");
                if (windowSize != Size.Empty && windowSize.Width != consoleDriver.Cols
                    || windowSize.Height != consoleDriver.Rows)
                    return;
            }
        }
    }

    void IMainLoopDriver.Wakeup()
    {
        //tokenSource.Cancel ();
        eventReady.Set();
    }

    bool IMainLoopDriver.EventsPending(bool wait)
    {
        if (CheckTimers(wait, out var waitTimeout)) return true;

        result = null;
        waitForProbe.Set();
        winChange.Set();

        try
        {
            if (!tokenSource.IsCancellationRequested) eventReady.Wait(waitTimeout, tokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            return true;
        }
        finally
        {
            eventReady.Reset();
        }

        if (!tokenSource.IsCancellationRequested) return result != null || CheckTimers(wait, out _) || winChanged;

        tokenSource.Dispose();
        tokenSource = new CancellationTokenSource();
        return true;
    }

    private bool CheckTimers(bool wait, out int waitTimeout)
    {
        var now = DateTime.UtcNow.Ticks;

        var firstTimeout = mainLoop.FirstTimeout();
        if (firstTimeout != long.MaxValue)
        {
            waitTimeout = (int)((firstTimeout - now) / TimeSpan.TicksPerMillisecond);
            if (waitTimeout < 0)
                return true;
        }
        else
        {
            waitTimeout = -1;
        }

        if (!wait)
            waitTimeout = 0;

        int ic = mainLoop.IdleCount();
        return ic > 0;
    }

    void IMainLoopDriver.MainIteration()
    {
        if (result != null)
        {
            var inputEvent = result[0];
            result = null;
            ProcessInput?.Invoke(inputEvent);
        }

        if (winChanged)
        {
            winChanged = false;
            WinChanged?.Invoke(windowSize);
        }
    }
}
