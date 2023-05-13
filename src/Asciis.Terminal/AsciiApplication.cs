using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Asciis.Terminal.ConsoleDrivers.NetDriver;
using System.ComponentModel;
using Asciis.Terminal.Core;
using Asciis.Terminal.Views;

namespace Asciis.Terminal;

public class AsciiApplication
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsciiAppBuilder"/> class with preconfigured defaults.
    /// </summary>
    /// <returns>The <see cref="AsciiAppBuilder"/>.</returns>
    public static AsciiAppBuilder CreateBuilder() => new(null);

    /// <summary>
    /// Initializes a new instance of the <see cref="AsciiAppBuilder"/> class with optional defaults.
    /// </summary>
    /// <param name="useDefaults">Whether to create the <see cref="AsciiAppBuilder"/> with common defaults.</param>
    /// <returns>The <see cref="AsciiAppBuilder"/>.</returns>
    public static AsciiAppBuilder CreateBuilder(string[]? args = null) => new(args);


    protected Stack<Window> windows = new();

    public IHost Host { get; }
    public IServiceProvider Services => Host.Services;

    public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

    public event EventHandler<ModalPoppedEventArgs>? ModalPopped;
    public event EventHandler<ModalPoppingEventArgs>? ModalPopping;
    public event EventHandler<ModalPushedEventArgs>? ModalPushed;
    public event EventHandler<ModalPushingEventArgs>? ModalPushing;
    public event EventHandler<Window>? WindowAppearing;
    public event EventHandler<Window>? WindowDisappearing;
    public event EventHandler? Iteration;
    public event EventHandler<ResizedEventArgs> Resized;

    public IReadOnlyCollection<Window> Windows => windows;
    public Window? CurrentWindow { get; set; }
    public AsciiTheme? Theme { get; set; }

    public ILogger<AsciiApplication>? Logger => _logger ??= Services.GetService<ILogger<AsciiApplication>>();
    public ConsoleDriver Driver
    {
        get => driver ?? throw new ArgumentNullException("The driver must be initialized first.");
        private set => driver = value;
    }
    public MainLoop MainLoop
    {
        get => mainLoop ?? throw new ArgumentNullException("The main loop must be initialised first");
        private set => mainLoop = value;
    }

    public bool HeightAsBuffer
    {
        get => Driver.HeightAsBuffer;
        set => Driver.HeightAsBuffer = value;
    }
    public bool AlwaysSetPosition
    {
        get
        {
            if (Driver is SystemConsoleDriver netDriver)
                return netDriver.AlwaysSetPosition;
            return false;
        }
        set
        {
            if (Driver is SystemConsoleDriver netDriver)
            {
                netDriver.AlwaysSetPosition = value;
                Driver.Refresh();
            }
        }
    }
    /// <summary>
    /// Alternative key to navigate forwards through all views. Ctrl+Tab is always used.
    /// </summary>
    public static Key AlternateForwardKey { get; set; } = Key.PageDown | Key.CtrlMask;

    /// <summary>
    /// Alternative key to navigate backwards through all views. Shift+Ctrl+Tab is always used.
    /// </summary>
    public static Key AlternateBackwardKey { get; set; } = Key.PageUp | Key.CtrlMask;

    public static Key QuitKey { get; set; } = Key.Q | Key.CtrlMask;

    /// <summary>
    /// Gets all the Mdi childes which represent all the not modal <see cref="Window"/> from the <see cref="MdiTop"/>.
    /// </summary>
    public List<Window>? MdiChildes
    {
        get
        {
            if (MdiTop != null)
            {
                List<Window> mdiChildes = new();
                foreach (var top in windows)
                {
                    if (top != MdiTop && !top.Modal)
                        mdiChildes.Add(top);
                }
                return mdiChildes;
            }

            return null;
        }
    }

    /// <summary>
    /// The <see cref="Window"/> object used for the application on startup which <see cref="Window.IsMdiContainer"/> is true.
    /// </summary>
    public Window? MdiTop
    {
        get
        {
            if (Top?.IsMdiContainer == true)
                return Top;
            return null;
        }
    }

    /// <summary>
    /// The <see cref="Window"/> object used for the application on startup (<seealso cref="Application.Top"/>)
    /// </summary>
    /// <value>The top.</value>
    public Window? Top { get; set; }

    public AsciiApplication(IHost host, ConsoleDriver consoleDriver, IMainLoopDriver mainLoopDriver)
    {
        Host = host;
        Driver = consoleDriver;
        Driver.Application = this;
        MainLoop = new MainLoop(mainLoopDriver);
        SynchronizationContext.SetSynchronizationContext(new MainLoopSyncContext(MainLoop));

        Driver.Init(TerminalResized);
    }

    public AsciiApplication Init(Window? top = null)
    {
        CurrentWindow = Top = top ?? new Window(new Rectangle(0,0,Driver.Cols,Driver.Rows));
        Top.Application = this;
        return this;
    }

    private void ProcessKeyEvent(KeyEvent ke)
    {
        var chain = windows.ToList();
        foreach (var topLevel in chain)
        {
            if (topLevel.ProcessHotKey(ke))
                return;
            if (topLevel.Modal)
                break;
        }

        foreach (var topLevel in chain)
        {
            if (topLevel.ProcessKey(ke))
                return;
            if (topLevel.Modal)
                break;
        }

        foreach (var topLevel in chain)
        {
            // Process the key normally
            if (topLevel.ProcessColdKey(ke))
                return;
            if (topLevel.Modal)
                break;
        }
    }

    private void ProcessKeyDownEvent(KeyEvent ke)
    {
        var chain = windows.ToList();
        foreach (var topLevel in chain)
        {
            if (topLevel.OnKeyDown(ke))
                return;
            if (topLevel.Modal)
                break;
        }
    }


    private void ProcessKeyUpEvent(KeyEvent ke)
    {
        var chain = windows.ToList();
        foreach (var topLevel in chain)
        {
            if (topLevel.OnKeyUp(ke))
                return;
            if (topLevel.Modal)
                break;
        }
    }

    private View? FindDeepestTop(Window start, int x, int y, out int resx, out int resy)
    {
        var startFrame = start.Frame;

        if (!startFrame.Contains(x, y))
        {
            resx = 0;
            resy = 0;
            return null;
        }

        if (windows != null)
        {
            var count = windows.Count;
            if (count > 0)
            {
                var rx = x - startFrame.X;
                var ry = y - startFrame.Y;
                foreach (var t in windows)
                    if (t != CurrentWindow)
                        if (t != start && t.Visible && t.Frame.Contains(rx, ry))
                        {
                            start = t;
                            break;
                        }
            }
        }

        resx = x - startFrame.X;
        resy = y - startFrame.Y;
        return start;
    }

    private View? FindDeepestMdiView(View start, int x, int y, out int resx, out int resy)
    {
        if (start.GetType().BaseType != typeof(Window)
            && !((Window)start).IsMdiContainer)
        {
            resx = 0;
            resy = 0;
            return null;
        }

        var startFrame = start.Frame;

        if (!startFrame.Contains(x, y))
        {
            resx = 0;
            resy = 0;
            return null;
        }

        var count = windows.Count;
        for (var i = count - 1; i >= 0; i--)
            foreach (var top in windows)
            {
                var rx = x - startFrame.X;
                var ry = y - startFrame.Y;
                if (top.Visible && top.Frame.Contains(rx, ry))
                {
                    var deep = FindDeepestView(top, rx, ry, out resx, out resy);
                    if (deep == null)
                        return FindDeepestMdiView(top, rx, ry, out resx, out resy);
                    if (deep != MdiTop)
                        return deep;
                }
            }

        resx = x - startFrame.X;
        resy = y - startFrame.Y;
        return start;
    }

    private View? FindDeepestView(View start, int x, int y, out int resx, out int resy)
    {
        var startFrame = start.Frame;

        if (!startFrame.Contains(x, y))
        {
            resx = 0;
            resy = 0;
            return null;
        }

        if (start.InternalSubviews != null)
        {
            var count = start.InternalSubviews.Count;
            if (count > 0)
            {
                var rx = x - startFrame.X;
                var ry = y - startFrame.Y;
                for (var i = count - 1; i >= 0; i--)
                {
                    View v = start.InternalSubviews[i];
                    if (v.Visible && v.Frame.Contains(rx, ry))
                    {
                        var deep = FindDeepestView(v, rx, ry, out resx, out resy);
                        if (deep == null)
                            return v;
                        return deep;
                    }
                }
            }
        }

        resx = x - startFrame.X;
        resy = y - startFrame.Y;
        return start;
    }

    private View FindTopFromView(View view)
    {
        View? top = view?.SuperView != null && view?.SuperView != Top
            ? view?.SuperView
            : view;

        while (top?.SuperView != null && top?.SuperView != Top)
            top = top?.SuperView;

        return top!;
    }

    internal View? mouseGrabView;

    /// <summary>
    /// Grabs the mouse, forcing all mouse events to be routed to the specified view until UngrabMouse is called.
    /// </summary>
    /// <returns>The grab.</returns>
    /// <param name="view">View that will receive all mouse events until UngrabMouse is invoked.</param>
    public void GrabMouse(View? view)
    {
        if (view == null)
            return;
        mouseGrabView = view;
        Driver.UncookMouse();
    }

    /// <summary>
    /// Releases the mouse grab, so mouse events will be routed to the view on which the mouse is.
    /// </summary>
    public void UngrabMouse()
    {
        mouseGrabView = null;
        Driver.CookMouse();
    }

    /// <summary>
    /// Merely a debugging aid to see the raw mouse events
    /// </summary>
    public Action<MouseEvent> RootMouseEvent;

    internal View? wantContinuousButtonPressedView;
    private View? lastMouseOwnerView;

    private void ProcessMouseEvent(MouseEvent me)
    {
        var view = FindDeepestView(CurrentWindow, me.X, me.Y, out var rx, out var ry);

        if (view != null && view.WantContinuousButtonPressed)
            wantContinuousButtonPressedView = view;
        else
            wantContinuousButtonPressedView = null;
        if (view != null) me.View = view;
        RootMouseEvent?.Invoke(me);
        if (mouseGrabView != null)
        {
            var newxy = mouseGrabView.ScreenToView(me.X, me.Y);
            var nme = new MouseEvent()
            {
                X = newxy.X,
                Y = newxy.Y,
                Flags = me.Flags,
                OfX = me.X - newxy.X,
                OfY = me.Y - newxy.Y,
                View = view
            };
            if (OutsideFrame(new Point(nme.X, nme.Y), mouseGrabView.Frame)) lastMouseOwnerView?.OnMouseLeave(me);
            // System.Diagnostics.Debug.WriteLine ($"{nme.Flags};{nme.X};{nme.Y};{mouseGrabView}");
            if (mouseGrabView != null)
            {
                mouseGrabView.OnMouseEvent(nme);
                return;
            }
        }

        if ((view == null || view == MdiTop) && !CurrentWindow.Modal && MdiTop != null
            && me.Flags != MouseFlags.ReportMousePosition && me.Flags != 0)
        {
            var top = FindDeepestTop(Top, me.X, me.Y, out _, out _);
            view = FindDeepestView(top, me.X, me.Y, out rx, out ry);

            if (view != null && view != MdiTop && top != CurrentWindow) MoveCurrent((Window)top);
        }

        if (view != null)
        {
            var nme = new MouseEvent()
            {
                X = rx,
                Y = ry,
                Flags = me.Flags,
                OfX = 0,
                OfY = 0,
                View = view
            };

            if (lastMouseOwnerView == null)
            {
                lastMouseOwnerView = view;
                view.OnMouseEnter(nme);
            }
            else if (lastMouseOwnerView != view)
            {
                lastMouseOwnerView.OnMouseLeave(nme);
                view.OnMouseEnter(nme);
                lastMouseOwnerView = view;
            }

            if (!view.WantMousePositionReports && me.Flags == MouseFlags.ReportMousePosition)
                return;

            if (view.WantContinuousButtonPressed)
                wantContinuousButtonPressedView = view;
            else
                wantContinuousButtonPressedView = null;

            // Should we bubbled up the event, if it is not handled?
            view.OnMouseEvent(nme);

            EnsuresTopOnFront();
        }
    }

    // Only return true if the Current has changed.
    private bool MoveCurrent(Window top)
    {
        // The Current is modal and the top is not modal toplevel then
        // the Current must be moved above the first not modal toplevel.
        if (MdiTop != null && top != MdiTop && top != CurrentWindow && CurrentWindow?.Modal == true && !windows.Peek().Modal)
        {
            lock (windows)
            {
                windows.MoveTo(CurrentWindow, 0, new ToplevelEqualityComparer());
            }

            var index = 0;
            var savedToplevels = windows.ToArray();
            foreach (var t in savedToplevels)
            {
                if (!t.Modal && t != CurrentWindow && t != top && t != savedToplevels[index])
                    lock (windows)
                    {
                        windows.MoveTo(top, index, new ToplevelEqualityComparer());
                    }

                index++;
            }

            return false;
        }

        // The Current and the top are both not running toplevel then
        // the top must be moved above the first not running toplevel.
        if (MdiTop != null && top != MdiTop && top != CurrentWindow && CurrentWindow?.Running == false && !top.Running)
        {
            lock (windows)
            {
                windows.MoveTo(CurrentWindow, 0, new ToplevelEqualityComparer());
            }

            var index = 0;
            foreach (var t in windows.ToArray())
            {
                if (!t.Running && t != CurrentWindow && index > 0)
                    lock (windows)
                    {
                        windows.MoveTo(top, index - 1, new ToplevelEqualityComparer());
                    }

                index++;
            }

            return false;
        }

        if (MdiTop != null && top?.Modal == true && windows.Peek() != top
            || MdiTop != null && CurrentWindow != MdiTop && CurrentWindow?.Modal == false && top == MdiTop
            || MdiTop != null && CurrentWindow?.Modal == false && top != CurrentWindow
            || MdiTop != null && CurrentWindow?.Modal == true && top == MdiTop)
            lock (windows)
            {
                windows.MoveTo(top, 0, new ToplevelEqualityComparer());
                CurrentWindow = top;
            }

        return true;
    }

    private bool OutsideFrame(Point p, Rectangle r)
    {
        return p.X < 0 || p.X > r.Width - 1 || p.Y < 0 || p.Y > r.Height - 1;
    }

    /// <summary>
    /// Building block API: Prepares the provided <see cref="Window"/>  for execution.
    /// </summary>
    /// <returns>The runstate handle that needs to be passed to the <see cref="End(RunState)"/> method upon completion.</returns>
    /// <param name="window">Toplevel to prepare execution for.</param>
    /// <remarks>
    ///  This method prepares the provided toplevel for running with the focus,
    ///  it adds this to the list of toplevels, sets up the mainloop to process the
    ///  event, lays out the subviews, focuses the first element, and draws the
    ///  toplevel in the screen. This is usually followed by executing
    ///  the <see cref="RunLoop"/> method, and then the <see cref="End(RunState)"/> method upon termination which will
    ///   undo these changes.
    /// </remarks>
    public RunState Begin(Window window)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));
        else if (window.IsMdiContainer && MdiTop != null)
            throw new InvalidOperationException("Only one Mdi Container is allowed.");

        window.Application = this;
        var rs = new RunState(this, window);

        if (window is ISupportInitializeNotification initializableNotification &&
            !initializableNotification.IsInitialized)
        {
            initializableNotification.BeginInit();
            initializableNotification.EndInit();
        }
        else if (window is ISupportInitialize initializable)
        {
            initializable.BeginInit();
            initializable.EndInit();
        }

        lock (windows)
        {
            if (string.IsNullOrEmpty(window.Id.ToString()))
            {
                var count = 1;
                var id = (windows.Count + count).ToString();
                while (windows.Count > 0 && windows.FirstOrDefault(x => x.Id.ToString() == id) != null)
                {
                    count++;
                    id = (windows.Count + count).ToString();
                }

                window.Id = (windows.Count + count).ToString();

                windows.Push(window);
            }
            else
            {
                var dup = windows.FirstOrDefault(x => x.Id.ToString() == window.Id);
                if (dup == null) windows.Push(window);
            }

            if (windows.FindDuplicates(new ToplevelEqualityComparer()).Count > 0)
                throw new ArgumentException("There are duplicates toplevels Id's");
        }

        if (window.IsMdiContainer)
            Top = window;

        var refreshDriver = true;
        if (MdiTop == null || window.IsMdiContainer || CurrentWindow?.Modal == false && window.Modal
            || CurrentWindow?.Modal == false && !window.Modal || CurrentWindow?.Modal == true && window.Modal)
        {
            if (window.Visible)
            {
                CurrentWindow = window;
                SetCurrentAsTop();
            }
            else
            {
                refreshDriver = false;
            }
        }
        else if (MdiTop != null && window != MdiTop && CurrentWindow?.Modal == true && !windows.Peek().Modal
                 || MdiTop != null && window != MdiTop && CurrentWindow?.Running == false)
        {
            refreshDriver = false;
            MoveCurrent(window);
        }
        else
        {
            refreshDriver = false;
            MoveCurrent(CurrentWindow);
        }

        Driver.PrepareToRun(MainLoop, ProcessKeyEvent, ProcessKeyDownEvent, ProcessKeyUpEvent, ProcessMouseEvent);
        if (window.LayoutStyle == LayoutStyle.Computed)
            window.SetRelativeLayout(new Rectangle(0, 0, Driver.Cols, Driver.Rows));
        window.LayoutSubviews();
        window.PositionToplevels();
        window.WillPresent();
        if (refreshDriver)
        {
            if (MdiTop != null) MdiTop.OnChildLoaded(window);
            window.OnLoaded();
            Redraw(window);
            window.PositionCursor();
            Driver.Refresh();
        }

        return rs;
    }

    /// <summary>
    /// Building block API: completes the execution of a <see cref="Window"/>  that was started with <see cref="Begin(Window)"/> .
    /// </summary>
    /// <param name="runState">The runstate returned by the <see cref="Begin(Window)"/> method.</param>
    public void End(RunState runState)
    {
        if (runState == null)
            throw new ArgumentNullException(nameof(runState));

        if (MdiTop != null)
            MdiTop.OnChildUnloaded(runState.Window);
        else
            runState.Window?.OnUnloaded();
        runState.Dispose();
    }

    /// <summary>
    /// Shutdown an application initialized with <see cref="Init(ConsoleDriver, IMainLoopDriver)"/>
    /// </summary>
    public void Shutdown() { ResetState(); }

    // Encapsulate all setting of initial state for Application; Having
    // this in a function like this ensures we don't make mistakes in
    // guaranteeing that the state of this singleton is deterministic when Init
    // starts running and after Shutdown returns.
    private void ResetState()
    {
        // Shutdown is the bookend for Init. As such it needs to clean up all resources
        // Init created. Apps that do any threading will need to code defensively for this.
        // e.g. see Issue #537
        // TODO: Some of this state is actually related to Begin/End (not Init/Shutdown) and should be moved to `RunState` (#520)
        foreach (var window in windows)
        {
            window.Running = false;
            window.Dispose();
        }

        windows.Clear();
        CurrentWindow = null;
        Top = null;

        MainLoop = null;
        if (driver != null)
            driver.End();
        driver = null;
        Iteration = null;
        RootMouseEvent = null;
        Resized = null;
        mouseGrabView = null;

        // Reset synchronization context to allow the user to run async/await,
        // as the main loop has been ended, the synchronization context from 
        // gui.cs does no longer process any callbacks. See #1084 for more details:
        // (https://github.com/migueldeicaza/gui.cs/issues/1084).
        SynchronizationContext.SetSynchronizationContext(null);
    }


    private void Redraw(View view)
    {
        view.Redraw(view.Bounds);
        Driver.Refresh();
    }

    private void Refresh(View view)
    {
        view.Redraw(view.Bounds);
        Driver.Refresh();
    }

    /// <summary>
    /// Triggers a refresh of the entire display.
    /// </summary>
    public void Refresh()
    {
        Driver.UpdateScreen();
        View? last = null;
        foreach (var v in windows.Reverse())
        {
            if (v.Visible)
            {
                v.SetNeedsDisplay();
                v.Redraw(v.Bounds);
            }

            last = v;
        }

        last?.PositionCursor();
        Driver.Refresh();
    }

    internal void End(View view)
    {
        if (windows.Peek() != view)
            throw new ArgumentException("The view that you end with must be balanced");
        windows.Pop();

        (view as Window)?.OnClosed((Window)view);

        if (MdiTop != null && !((Window)view).Modal && view != MdiTop) MdiTop.OnChildClosed(view as Window);

        if (windows.Count == 0)
        {
            CurrentWindow = null;
        }
        else
        {
            CurrentWindow = windows.Peek();
            if (windows.Count == 1 && CurrentWindow == MdiTop)
                MdiTop.OnAllChildClosed();
            else
                SetCurrentAsTop();
            Refresh();
        }
    }

    /// <summary>
    ///   Building block API: Runs the main loop for the created dialog
    /// </summary>
    /// <remarks>
    ///   Use the wait parameter to control whether this is a
    ///   blocking or non-blocking call.
    /// </remarks>
    /// <param name="state">The state returned by the Begin method.</param>
    /// <param name="wait">By default this is true which will execute the runloop waiting for events, if you pass false, you can use this method to run a single iteration of the events.</param>
    public void RunLoop(RunState state, bool wait = true)
    {
        if (state == null)
            throw new ArgumentNullException(nameof(state));
        if (state.Window == null)
            throw new ObjectDisposedException("state");

        var firstIteration = true;
        for (state.Window.Running = true; state.Window.Running;)
        {
            if (MainLoop.EventsPending(wait))
            {
                // Notify Toplevel it's ready
                if (firstIteration)
                    state.Window.OnReady();
                firstIteration = false;

                MainLoop.MainIteration();
                RaiseItteration();

                EnsureModalAlwaysOnTop(state.Window);
                if (state.Window != CurrentWindow && CurrentWindow?.Modal == true
                    || state.Window != CurrentWindow && CurrentWindow?.Modal == false)
                {
                    MdiTop?.OnDeactivate(state.Window);
                    state.Window = CurrentWindow;
                    MdiTop?.OnActivate(state.Window);
                    Top.SetChildNeedsDisplay();
                    Refresh();
                }

                if (Driver.EnsureCursorVisibility())
                    state.Window.SetNeedsDisplay();
            }
            else if (!wait)
            {
                return;
            }

            if (state.Window != Top
                && (!Top.NeedDisplay.IsEmpty || Top.ChildNeedsDisplay || Top.LayoutNeeded))
            {
                Top.Redraw(Top.Bounds);
                state.Window.SetNeedsDisplay(state.Window.Bounds);
            }

            if (!state.Window.NeedDisplay.IsEmpty || state.Window.ChildNeedsDisplay ||
                state.Window.LayoutNeeded
                || MdiChildNeedsDisplay())
            {
                state.Window.Redraw(state.Window.Bounds);
                if (DebugDrawBounds)
                    DrawBounds(state.Window);
                state.Window.PositionCursor();
                Driver.Refresh();
            }
            else
            {
                Driver.UpdateCursor();
            }

            if (state.Window != Top &&
                !state.Window.Modal &&
                (!Top.NeedDisplay.IsEmpty || Top.ChildNeedsDisplay || Top.LayoutNeeded))
            {
                Top.Redraw(Top.Bounds);
            }
        }
    }

    private void EnsureModalAlwaysOnTop(Window window)
    {
        if (!window.Running || window == CurrentWindow || MdiTop == null || windows.Peek().Modal)
            return;

        foreach (var top in windows.Reverse())
        {
            if (top.Modal && top != CurrentWindow)
            {
                MoveCurrent(top);
                return;
            }
        }
    }

    private bool MdiChildNeedsDisplay()
    {
        if (MdiTop == null) return false;

        foreach (var top in windows)
        {
            if (top != CurrentWindow && top.Visible &&
                (!top.NeedDisplay.IsEmpty || top.ChildNeedsDisplay || top.LayoutNeeded))
            {
                MdiTop.SetChildNeedsDisplay();
                return true;
            }
        }

        return false;
    }

    internal bool DebugDrawBounds = false;

    // Need to look into why this does not work properly.
    private void DrawBounds(View v)
    {
        v.DrawFrame(v.Frame, 0, false);
        if (v.InternalSubviews != null && v.InternalSubviews.Count > 0)
        {
            foreach (var sub in v.InternalSubviews)
                DrawBounds(sub);
        }
    }

    /// <summary>
    /// Runs the application by calling <see cref="Run(Window, Func{Exception, bool})"/> with the value of <see cref="Top"/>
    /// </summary>
    public void Run(Func<Exception, bool> errorHandler = null)
    {
        Run(Top, errorHandler);
    }

    /// <summary>
    /// Runs the application by calling <see cref="Run(Window, Func{Exception, bool})"/> with a new instance of the specified <see cref="Window"/>-derived class
    /// </summary>
    public void Run<T>(Func<Exception, bool> errorHandler = null) where T : Window, new()
    {
        var window = new T();
        Run(window, errorHandler);
    }

    /// <summary>
    ///   Runs the main loop on the given <see cref="Window"/> container.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This method is used to start processing events
    ///     for the main application, but it is also used to
    ///     run other modal <see cref="View"/>s such as <see cref="Dialog"/> boxes.
    ///   </para>
    ///   <para>
    ///     To make a <see cref="Run(Window, Func{Exception, bool})"/> stop execution, call <see cref="Application.RequestStop"/>.
    ///   </para>
    ///   <para>
    ///     Calling <see cref="Run(Window, Func{Exception, bool})"/> is equivalent to calling <see cref="Begin(Window)"/>, followed by <see cref="RunLoop(RunState, bool)"/>,
    ///     and then calling <see cref="End(RunState)"/>.
    ///   </para>
    ///   <para>
    ///     Alternatively, to have a program control the main loop and 
    ///     process events manually, call <see cref="Begin(Window)"/> to set things up manually and then
    ///     repeatedly call <see cref="RunLoop(RunState, bool)"/> with the wait parameter set to false.   By doing this
    ///     the <see cref="RunLoop(RunState, bool)"/> method will only process any pending events, timers, idle handlers and
    ///     then return control immediately.
    ///   </para>
    ///   <para>
    ///     When <paramref name="errorHandler"/> is null the exception is rethrown, when it returns true the application is resumed and when false method exits gracefully.
    ///   </para>
    /// </remarks>
    /// <param name="window">The <see cref="Window"/> to run modally.</param>
    /// <param name="errorHandler">Handler for any unhandled exceptions (resumes when returns true, rethrows when null).</param>
    public void Run(Window window, Func<Exception, bool>? errorHandler = null)
    {
        //CurrentWindow ??= window;
        //Top ??= window;
        window.Application = this;

        var resume = true;
        while (resume)
        {
#if !DEBUG
				try {
#endif
            resume = false;
            var runToken = Begin(window);
            RunLoop(runToken);
            End(runToken);
#if !DEBUG
				}
				catch (Exception error)
				{
					if (errorHandler == null)
					{
						throw;
					}
					resume = errorHandler(error);
				}
#endif
        }
    }

    /// <summary>
    /// Stops running the most recent <see cref="Window"/> or the <paramref name="window"/> if provided.
    /// </summary>
    /// <param name="window">The toplevel to request stop.</param>
    /// <remarks>
    ///   <para>
    ///   This will cause <see cref="Application.Run(Func{Exception, bool})"/> to return.
    ///   </para>
    ///   <para>
    ///     Calling <see cref="Application.RequestStop"/> is equivalent to setting the <see cref="Window.Running"/> property on the currently running <see cref="Window"/> to false.
    ///   </para>
    /// </remarks>
    public void RequestStop(Window? window = null)
    {
        if (MdiTop == null || window == null || MdiTop == null && window != null)
            window = CurrentWindow;

        if (window == null)
            return;

        if (MdiTop != null && window.IsMdiContainer && window.Running == true
            && (CurrentWindow?.Modal == false || CurrentWindow?.Modal == true && CurrentWindow?.Running == false))
        {
            MdiTop.RequestStop();
        }
        else if (MdiTop != null && window != CurrentWindow && CurrentWindow?.Running == true && CurrentWindow?.Modal == true
                 && window.Modal && window.Running)
        {
            var ev = new ToplevelClosingEventArgs(CurrentWindow);
            CurrentWindow.OnClosing(ev);
            if (ev.Cancel) return;
            ev = new ToplevelClosingEventArgs(window);
            window.OnClosing(ev);
            if (ev.Cancel) return;
            CurrentWindow.Running = false;
            window.Running = false;
        }
        else if (MdiTop != null && window != MdiTop && window != CurrentWindow && CurrentWindow?.Modal == false
                 && CurrentWindow?.Running == true && !window.Running
                 || MdiTop != null && window != MdiTop && window != CurrentWindow && CurrentWindow?.Modal == false
                 && CurrentWindow?.Running == false && !window.Running && windows.ToArray()[1].Running)
        {
            MoveCurrent(window);
        }
        else if (MdiTop != null && CurrentWindow != window && CurrentWindow?.Running == true && !window.Running
                 && CurrentWindow?.Modal == true && window.Modal)
        {
            // The Current and the top are both modal so needed to set the Current.Running to false too.
            CurrentWindow.Running = false;
        }
        else if (MdiTop != null && CurrentWindow == window && MdiTop?.Running == true && CurrentWindow?.Running == true &&
                 window.Running
                 && CurrentWindow?.Modal == true && window.Modal)
        {
            // The MdiTop was requested to stop inside a modal toplevel which is the Current and top,
            // both are the same, so needed to set the Current.Running to false too.
            CurrentWindow.Running = false;
        }
        else
        {
            Window currentTop;
            if (window == CurrentWindow || CurrentWindow?.Modal == true && !window.Modal)
                currentTop = CurrentWindow;
            else
                currentTop = window;
            if (!currentTop.Running) return;
            var ev = new ToplevelClosingEventArgs(currentTop);
            currentTop.OnClosing(ev);
            if (ev.Cancel) return;
            currentTop.Running = false;
        }
    }

    private void TerminalResized()
    {
        var full = new Rectangle(0, 0, Driver.Cols, Driver.Rows);
        SetToplevelsSize(full);
        RaiseResized(new ResizedEventArgs() { Cols = full.Width, Rows = full.Height });
        Driver.Clip = full;
        foreach (var t in windows)
        {
            t.SetRelativeLayout(full);
            t.LayoutSubviews();
            t.PositionToplevels();
        }

        Refresh();
    }

    private void SetToplevelsSize(Rectangle full)
    {
        if (MdiTop == null)
        {
            foreach (var t in windows)
                if (t?.SuperView == null && !t.Modal)
                {
                    t.Frame = full;
                    t.Width = full.Width;
                    t.Height = full.Height;
                }
        }
        else
        {
            Top.Frame = full;
            Top.Width = full.Width;
            Top.Height = full.Height;
        }
    }

    private bool SetCurrentAsTop()
    {
        if (MdiTop == null && CurrentWindow != Top && CurrentWindow?.SuperView == null && CurrentWindow?.Modal == false)
        {
            if (CurrentWindow.Frame != new Rectangle(0, 0, Driver.Cols, Driver.Rows))
                CurrentWindow.Frame = new Rectangle(0, 0, Driver.Cols, Driver.Rows);
            Top = CurrentWindow;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Move to the next Mdi child from the <see cref="MdiTop"/>.
    /// </summary>
    public void MoveNext()
    {
        if (MdiTop != null && !CurrentWindow.Modal)
            lock (windows)
            {
                windows.MoveNext();
                while (windows.Peek() == MdiTop || !windows.Peek().Visible) windows.MoveNext();
                CurrentWindow = windows.Peek();
            }
    }

    /// <summary>
    /// Move to the previous Mdi child from the <see cref="MdiTop"/>.
    /// </summary>
    public void MovePrevious()
    {
        if (MdiTop != null && !CurrentWindow.Modal)
            lock (windows)
            {
                windows.MovePrevious();
                while (windows.Peek() == MdiTop || !windows.Peek().Visible)
                    lock (windows)
                    {
                        windows.MovePrevious();
                    }

                CurrentWindow = windows.Peek();
            }
    }

    internal bool ShowChild(Window top)
    {
        if (top.Visible && MdiTop != null && CurrentWindow?.Modal == false)
        {
            lock (windows)
            {
                windows.MoveTo(top, 0, new ToplevelEqualityComparer());
                CurrentWindow = top;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Wakes up the mainloop that might be waiting on input, must be thread safe.
    /// </summary>
    public void DoEvents() { MainLoop.Driver.Wakeup(); }

    /// <summary>
    /// Ensures that the superview of the most focused view is on front.
    /// </summary>
    public void EnsuresTopOnFront()
    {
        if (MdiTop != null) return;
        var top = FindTopFromView(Top?.MostFocused);
        if (top != null && Top.Subviews.Count > 1 && Top.Subviews[Top.Subviews.Count - 1] != top)
            Top.BringSubviewToFront(top);
    }

    internal void NotifyOfWindowModalEvent(System.EventArgs eventArgs)
    {
        switch (eventArgs)
        {
            case ModalPoppedEventArgs poppedEvents:
                ModalPopped?.Invoke(this, poppedEvents);
                break;
            case ModalPoppingEventArgs poppingEvents:
                ModalPopping?.Invoke(this, poppingEvents);
                break;
            case ModalPushedEventArgs pushedEvents:
                ModalPushed?.Invoke(this, pushedEvents);
                break;
            case ModalPushingEventArgs pushingEvents:
                ModalPushing?.Invoke(this, pushingEvents);
                break;
            default:
                break;
        }
    }

    public void RaiseWindowAppearing(Window window) => WindowAppearing?.Invoke(this, window);
    public void RaiseWindowDisappearing(Window window) => WindowDisappearing?.Invoke(this, window);
    public void RaiseItteration() => Iteration?.Invoke(this, System.EventArgs.Empty);
    public void RaiseResized(ResizedEventArgs args) => Resized?.Invoke(this, args);


    private ILogger<AsciiApplication>? _logger;
    private ConsoleDriver? driver;
    private MainLoop? mainLoop;
    private Window top;
    private Window? current;

    /// <summary>
    /// provides the sync context set while executing code, to let users use async/await on their code
    /// </summary>
    private class MainLoopSyncContext : SynchronizationContext
    {
        private MainLoop mainLoop;
        public MainLoopSyncContext(MainLoop mainLoop) { this.mainLoop = mainLoop; }
        public override SynchronizationContext CreateCopy() { return new MainLoopSyncContext(mainLoop); }
        public override void Post(SendOrPostCallback d, object? state)
        {
            mainLoop.AddIdle(() =>
            {
                d(state);
                return false;
            });
            //mainLoop.Driver.Wakeup ();
        }
        public override void Send(SendOrPostCallback d, object? state) { mainLoop.Invoke(() => { d(state); }); }
    }

    /// <summary>
    /// Captures the execution state for the provided <see cref="Window"/>  view.
    /// </summary>
    public class RunState : IDisposable
    {
        /// <summary>
        /// Initializes a new <see cref="RunState"/> class.
        /// </summary>
        /// <param name="window"></param>
        public RunState(AsciiApplication asciiApplication, Window window)
        {
            Application = asciiApplication;
            Window = window;
        }

        internal Window? Window;
        internal AsciiApplication Application;

        /// <summary>
        /// Releases alTop = l resource used by the <see cref="Application.RunState"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose()"/> when you are finished using the <see cref="Application.RunState"/>. The
        /// <see cref="Dispose()"/> method leaves the <see cref="Application.RunState"/> in an unusable state. After
        /// calling <see cref="Dispose()"/>, you must release all references to the
        /// <see cref="Application.RunState"/> so the garbage collector can reclaim the memory that the
        /// <see cref="Application.RunState"/> was occupying.</remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the specified disposing.
        /// </summary>
        /// <returns>The dispose.</returns>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Window != null && disposing)
            {
                Application.End(Window);
                Window.Dispose();
                Window = null;
            }
        }
    }
}
