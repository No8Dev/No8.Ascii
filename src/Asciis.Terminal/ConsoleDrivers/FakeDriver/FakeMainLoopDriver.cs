using Asciis.Terminal.Core;
using System.Threading;

namespace Asciis.Terminal.ConsoleDrivers.FakeDriver;

/// <summary>
/// Mainloop intended to be used with the .NET System.Console API, and can
/// be used on Windows and Unix, it is cross platform but lacks things like
/// file descriptor monitoring.
/// </summary>
/// <remarks>
/// This implementation is used for FakeDriver.
/// </remarks>
public class FakeMainLoopDriver : IMainLoopDriver
{
    private AutoResetEvent keyReady = new(false);
    private AutoResetEvent waitForProbe = new(false);
    private ConsoleKeyInfo? keyResult = null;
    private MainLoop mainLoop;
    private Func<ConsoleKeyInfo> consoleKeyReaderFn;
    private FakeConsoleDriver Driver;

    /// <summary>
    /// Invoked when a Key is pressed.
    /// </summary>
    public Action<ConsoleKeyInfo> KeyPressed;

    /// <summary>
    /// Initializes the class.
    /// </summary>
    /// <remarks>
    ///   Passing a consoleKeyReaderfn is provided to support unit test scenarios.
    /// </remarks>
    /// <param name="consoleKeyReaderFn">The method to be called to get a key from the console.</param>
    public FakeMainLoopDriver(ConsoleDriver driver)
    {
        Driver = (FakeConsoleDriver)driver;
        consoleKeyReaderFn = () => Driver.FakeConsole.ReadKey(true);
    }

    private void WindowsKeyReader()
    {
        while (true)
        {
            waitForProbe.WaitOne();
            keyResult = consoleKeyReaderFn();
            keyReady.Set();
        }
    }

    void IMainLoopDriver.Setup(MainLoop mainLoop)
    {
        this.mainLoop = mainLoop;
        Thread readThread = new(WindowsKeyReader);
        readThread.Start();
    }

    void IMainLoopDriver.Wakeup() { }

    bool IMainLoopDriver.EventsPending(bool wait)
    {
        var now = DateTime.UtcNow.Ticks;

        int waitTimeout;
        var firstTimeout = mainLoop.FirstTimeout();
        if (firstTimeout != long.MaxValue)
        {
            waitTimeout = (int)((firstTimeout - now) / TimeSpan.TicksPerMillisecond);
            if (waitTimeout < 0)
                return true;
        }
        else
            waitTimeout = -1;

        if (!wait)
            waitTimeout = 0;

        keyResult = null;
        waitForProbe.Set();
        keyReady.WaitOne(waitTimeout);
        return keyResult.HasValue;
    }

    void IMainLoopDriver.MainIteration()
    {
        if (keyResult.HasValue)
        {
            KeyPressed?.Invoke(keyResult.Value);
            keyResult = null;
        }
    }
}
