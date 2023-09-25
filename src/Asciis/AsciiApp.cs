using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using No8.Ascii.Controls;
using No8.Ascii.DependencyInjection;
using No8.Ascii.ElementLayout;
using No8.Ascii.VirtualTerminal;

namespace No8.Ascii;

public interface IApp
{
    bool NeedsLayout { get; set; }
    bool NeedsPainting { get; set; }

}

public class AsciiApp : IApp
{
    public DependencyInjectionContainer Services { get; } = new();
    public ConfigurationManager Configuration { get; } = new();
    public ConsoleDriver ConsoleDriver { get; }
    public ExConsole ExCon { get; }
    public Canvas Canvas { get; }

    public bool NeedsLayout { get; set; }
    public bool NeedsPainting { get; set; }


    private readonly List<Window> _windows = new();
    private readonly List<Func<bool>> _idleHandlers = new();
    private readonly SortedList<long, TimeoutFunc> _timeouts = new();
    private bool _invalidateConsole;
    private bool _active;
    private DateTime _screenTick = DateTime.Now;
    private WindowSize? _newWindowSize = null;

    private static AsciiApp? _current;

    public AsciiApp(string[]? args = null)
    {
        _current = this;

        Services.Register(Configuration);

        InitConfig(args);

        ConsoleDriver = ConsoleDriver.Create(Services);
        ExCon = ExConsole.Create(options => 
        { 
            options.ConsoleDriver = ConsoleDriver;
            options.StartFullScreen = true;
            options.StopOnControlC = true;
        });
        
        InitServices();
        System.Console.OutputEncoding = Encoding.UTF8;

        ExCon.WindowResized += (_, size) => _newWindowSize = size;

        Canvas = new Canvas(System.Console.WindowWidth, System.Console.WindowHeight);

        SynchronizationContext.SetSynchronizationContext(new AsciiSlimAppSyncContext(this));
    }
    
    private void InitConfig(string[]? args = null)
    {
        Configuration["contentRoot"] = Directory.GetCurrentDirectory();
        Configuration.AddEnvironmentVariables(prefix: "DOTNET_");
        Configuration.AddEnvironmentVariables();
        if (args?.Length > 0)
            Configuration.AddCommandLine(args);

        var applicationName = Configuration["applicationName"];
        var environmentName = Configuration["environment"] ?? "production";
        bool reloadOnChange = Configuration.GetValue("reloadConfigOnChange", defaultValue: true);

        Configuration
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: reloadOnChange)
           .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: reloadOnChange);

        if (environmentName.Equals("development", StringComparison.OrdinalIgnoreCase) &&
            applicationName?.Length > 0)
        {
            var appAssembly = Assembly.Load(new AssemblyName(applicationName));
            Configuration.AddUserSecrets(appAssembly, optional: true, reloadOnChange: reloadOnChange);
        }

        // Reload again to overwrite anything from appsetting.json
        Configuration.AddEnvironmentVariables();
        if (args?.Length > 0)
            Configuration.AddCommandLine(args);

        Configuration["applicationName"] = applicationName ?? GetDefaultApplicationName();
    }

    private void InitServices()
    {
        Services.Register(ConsoleDriver);
        Services.Register(this);
    }

    internal static string GetDefaultApplicationName()
    {
        var startupAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
        return startupAssemblyName ?? "AsciiApp";
    }

    public void AddWindow(Window window)
    {
        lock (_windows)
        {
            if (_windows.Count == 0)
                window.IsModal = true;
            _windows.Add(window);
            window.App = this;
        }
    }

    public void Invoke(Action action)
    {
        AddIdle(
            () =>
            {
                action();
                return false;
            });
    }

    public Func<bool> AddIdle(Func<bool> idleHandler)
    {
        lock (_idleHandlers)
        {
            _idleHandlers.Add(idleHandler);
        }

        return idleHandler;
    }

    public bool RemoveIdle(Func<bool> func)
    {
        lock (_idleHandlers)
        {
            return _idleHandlers.Remove(func);
        }
    }

    private void AddTimeout(TimeoutFunc timeoutFunc)
    {
        var k = (DateTime.UtcNow + timeoutFunc.Span).Ticks;
        lock (_timeouts)
        {
            while (_timeouts.ContainsKey(k))
                k++;

            _timeouts.Add(k, timeoutFunc);
        }
    }

    public TimeoutFunc AddTimeout(TimeSpan timespan, Func<IApp, bool> callback)
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));
        var timeoutFunc = new TimeoutFunc(
            timespan,
            callback);

        AddTimeout(timeoutFunc);
        return timeoutFunc;
    }

    public bool RemoveTimeout(TimeoutFunc callback)
    {
        lock (_timeouts)
        {
            var idx = _timeouts.IndexOfValue(callback);
            if (idx == -1)
                return false;
            _timeouts.RemoveAt(idx);
            return true;
        }
    }

    private void RunTimers()
    {
        lock (_timeouts)
        {
            if (!_timeouts.Any())
                return;
        }

        var now = DateTime.UtcNow.Ticks;

        ImmutableSortedDictionary<long, TimeoutFunc> copy;
        lock (_timeouts)
        {
            copy = _timeouts.ToImmutableSortedDictionary();
        }

        foreach (var (ticks, timeoutFunc) in copy)
            if (ticks < now)
            {
                RemoveTimeout(timeoutFunc);
                if (timeoutFunc.Callback(this))
                    AddTimeout(timeoutFunc);
            }
    }

    private void RunIdle()
    {
        List<Func<bool>> iterate;
        lock (_idleHandlers)
        {
            if (!_idleHandlers.Any())
                return;

            iterate = _idleHandlers.ToList();
        }

        foreach (var func in iterate)
            if (!func())
                RemoveIdle(func);
    }

    //--

    /// <summary>
    ///     Start the application.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     A <see cref="Task" /> that represents the startup of the <see cref="AsciiHost" />.
    ///     Successful completion indicates the HTTP server is ready to accept new requests.
    /// </returns>
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        // Force arrange for first update
        Canvas.Resize(System.Console.WindowWidth, System.Console.WindowHeight);
        foreach (var window in _windows.ToArray())
            ElementArrange.Calculate(window, System.Console.WindowWidth, System.Console.WindowHeight);

        return Task.Run(Start, cancellationToken);
    }

    /// <summary>
    ///     Runs the MainLoop.
    /// </summary>
    private void Start()
    {
        ExCon.Run();
        
        _active = true;
        _screenTick = DateTime.Now;
        while (_active)
        {
            OneIteration();

            if (!ExCon.Alive)
                break;
        }

        //ExCon.Stop();
        //ExCon.Screen.ScreenNormal();
    }

    /// <summary>
    ///     Runs one iteration of timers and file watches
    /// </summary>
    /// <remarks>
    ///     You use this to process all pending events (timers, idle handlers and file watches).
    ///     You can use it like this:
    ///     while (main.EventsPending ()) OneIteration ();
    /// </remarks>
    public void OneIteration()
    {
        var nextTick = DateTime.Now;
        var elapsedTime = nextTick - _screenTick;
        _screenTick = nextTick;

        try
        {
            RunTimers();

            if (_newWindowSize is not null)
            {
                var size = _newWindowSize;
                _newWindowSize = null;
                
                Canvas.Resize(size.Cols, size.Rows);
                
                foreach (var window in _windows)
                    window.NeedsLayout = true;
            }
            //ConsoleDriver.GatherInputEvents();
            //ProcessInputQueues(ConsoleDriver.KbEvents, ConsoleDriver.PointerEvents);

            ProcessWindows((float)elapsedTime.TotalSeconds);

            RunIdle();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }

    /// <summary>
    ///     Shuts down the application.
    /// </summary>
    public void Stop()
    {
        _active = false;
    }

    private void ProcessWindows(float elapsedSeconds)
    {
        foreach (var window in _windows.ToArray())
        {
            window.OnUpdate(elapsedSeconds);

            if (!_active) break;

            if (window.NeedsLayout || NeedsLayout)
            {
                ElementArrange.Calculate(window, System.Console.WindowWidth, System.Console.WindowHeight);

                window.NeedsLayout = false;
                window.NeedsPainting = true;
                Canvas.Clear();
            }
           
            if (window.NeedsPainting || NeedsLayout || NeedsPainting)
            {
                window.OnDraw(Canvas, window.Layout.Bounds);
                InvalidateConsole();
            }
            else
                window.RaiseDraw(Canvas); // Just for the Window

            window.CleanFlags();
        }

        NeedsPainting = false;
        NeedsLayout   = false;

        if (_invalidateConsole && _active)
        {
            _invalidateConsole = false;
            ConsoleDriver.WriteConsole(Canvas);
            UpdateCursor();
        }
    }

    private void UpdateCursor()
    {
        // Note: XTerm cursor is 1 based.
        if (Canvas.Cursor != Vec.Unknown && Canvas.Cursor != Vec.Zero)
        {
            if (Canvas.Cursor.X < Canvas.Width && Canvas.Cursor.Y < Canvas.Height)
            {
                var (left, top) = System.Console.GetCursorPosition();
                if (left != Canvas.Cursor.X || top != Canvas.Cursor.Y)
                {
                    ExCon.Cursor.Show();
                    ExCon.Cursor.Set(Canvas.Cursor.Y + 1, Canvas.Cursor.X + 1);
                }
            }
        }
        else
        {
            ExCon.Cursor.Set(1, 1);
            ExCon.Cursor.Hide();
        }
    }


    public void InvalidateConsole() { _invalidateConsole = true; }

    /*
    public void ProcessInputQueues(
        ConcurrentQueue<KeyboardEvent> kbEvents,
        ConcurrentQueue<PointerEvent> pointerEvents)
    {
        while (kbEvents.TryDequeue(out var kbEvent))
            switch (kbEvent.EventType)
            {
                case KeyboardEventType.Pressed:
                    ProcessKeyDownEvent(kbEvent);
                    break;
                case KeyboardEventType.Key:
                    HandleKbEvent(kbEvent);
                    break;
                case KeyboardEventType.Released:
                    ProcessKeyUpEvent(kbEvent);
                    break;
            }

        while (pointerEvents.TryDequeue(out var pointerEvent))
            HandlePointerEvent(pointerEvent);
    }
    */

    // public KeyState GetVirtualKey(VirtualKeyCode code) => ConsoleDriver.VirtualKeys[(int)code];

    private void HandleKbEvent(KeyboardEvent kbEvent)
    {
        Trace.WriteLine($"KeyboardEvent: {kbEvent}");

        var chain = _windows.ToArray().Reverse().ToList();   // Top window first
        foreach (var window in chain)
        {
            if (Control.Traverse(window, ctrl => ctrl.OnPreProcessKey(kbEvent)))
                return;
            if (window.IsModal)
                break;
        }

        foreach (var window in chain)
        {
            if (Control.Traverse(window, ctrl => ctrl.OnKeyEvent(kbEvent), TraverseStrategy.ChildrenThenParent))
                return;
            if (window.IsModal)
                break;
        }

        foreach (var window in chain)
        {
            // Process the key normally
            if (Control.Traverse(window, ctrl => ctrl.OnPostProcessKey(kbEvent)))
                return;
            if (window.IsModal)
                break;
        }
    }

    private void ProcessKeyDownEvent(KeyboardEvent kbEvent)
    {
        foreach (var window in _windows.ToArray())
        {
            if (window.OnKeyDown(kbEvent))
                return;
            if (window.IsModal)
                break;
        }
    }

    private void ProcessKeyUpEvent(KeyboardEvent kbEvent)
    {
        foreach (var window in _windows.ToArray())
        {
            if (window.OnKeyUp(kbEvent))
                return;
            if (window.IsModal)
                break;
        }
    }

    private void HandlePointerEvent(PointerEvent pointerEvent)
    {
#if TRACE_POINTER
        Debug.WriteLine("PointerEvent: {pointerEvent}", pointerEvent);
#endif

        var chain = _windows.ToArray().Reverse(); // Top window first
        foreach (var window in chain)
        {
            if (window.HandlePointerEvent(pointerEvent))
                return;
            if (window.IsModal)
                break;
        }
    }
}