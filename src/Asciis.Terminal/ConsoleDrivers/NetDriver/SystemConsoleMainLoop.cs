using System.Threading;
using System.Threading.Tasks;
using Asciis.Terminal.ConsoleDrivers.NetDriver;

namespace Asciis.Terminal.ConsoleDrivers;

/// <summary>
/// Mainloop intended to be used with the .NET System.Console API, and can
/// be used on Windows and Unix, it is cross platform but lacks things like
/// file descriptor monitoring.
/// </summary>
/// <remarks>
/// This implementation is used for NetDriver.
/// </remarks>
internal class SystemConsoleMainLoop : IMainLoopDriver
{
    private ManualResetEventSlim keyReady = new(false);
    private ManualResetEventSlim waitForProbe = new(false);
    private Queue<SystemConsoleEvents.InputResult?> inputResult = new();
    private MainLoop? mainLoop;
    private CancellationTokenSource tokenSource = new();
    private SystemConsoleEvents consoleEvents;

    private AsciiApplication Application { get; }

    /// <summary>
    /// Invoked when a Key is pressed.
    /// </summary>
    public Action<SystemConsoleEvents.InputResult>? ProcessInput;

    /// <summary>
    /// Initializes the class with the console driver.
    /// </summary>
    /// <remarks>
    ///   Passing a consoleDriver is provided to capture windows resizing.
    /// </remarks>
    /// <param name="consoleDriver">The console driver used by this Net main loop.</param>
    public SystemConsoleMainLoop(AsciiApplication application)
    {
        Application = application;
        consoleEvents = new SystemConsoleEvents(application);
    }

    private void NetInputHandler()
    {
        while (true)
        {
            waitForProbe.Wait();
            waitForProbe.Reset();
            if (inputResult.Count == 0) inputResult.Enqueue(consoleEvents.ReadConsoleInput());
            try
            {
                while (inputResult.Peek() == null) inputResult.Dequeue();
                if (inputResult.Count > 0) keyReady.Set();
            }
            catch (InvalidOperationException) { }
        }
    }

    void IMainLoopDriver.Setup(MainLoop mainLoop)
    {
        this.mainLoop = mainLoop;
        Task.Run(NetInputHandler);
    }

    void IMainLoopDriver.Wakeup() { keyReady.Set(); }

    bool IMainLoopDriver.EventsPending(bool wait)
    {
        waitForProbe.Set();

        if (CheckTimers(wait, out var waitTimeout)) return true;

        try
        {
            if (!tokenSource.IsCancellationRequested) keyReady.Wait(waitTimeout, tokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            return true;
        }
        finally
        {
            keyReady.Reset();
        }

        if (!tokenSource.IsCancellationRequested) return inputResult.Count > 0 || CheckTimers(wait, out _);

        tokenSource.Dispose();
        tokenSource = new CancellationTokenSource();
        return true;
    }

    private bool CheckTimers(bool wait, out int waitTimeout)
    {
        var now = DateTime.UtcNow.Ticks;

        var firstTimeout = mainLoop?.FirstTimeout() ?? long.MaxValue;

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

        int ic = mainLoop?.IdleCount() ?? 0;
        return ic > 0;
    }

    void IMainLoopDriver.MainIteration()
    {
        if (inputResult.TryDequeue(out var value))
            ProcessInput?.Invoke(value!.Value);
    }
}
