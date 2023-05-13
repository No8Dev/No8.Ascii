using System.ComponentModel;
using Asciis.Terminal.Windows;

namespace Asciis.Terminal.Core;

/// <summary>
/// Toplevel views can be modally executed.
/// </summary>
/// <remarks>
///   <para>
///     Toplevels can be modally executing views, started by calling <see cref="Application.Run(Window, Func{Exception, bool})"/>. 
///     They return control to the caller when <see cref="Application.RequestStop(Window)"/> has 
///     been called (which sets the <see cref="Window.Running"/> property to false). 
///   </para>
///   <para>
///     A Toplevel is created when an application initializes Terminal.Gui by calling <see cref="Application.Init(ConsoleDriver, IMainLoopDriver)"/>.
///     The application Toplevel can be accessed via <see cref="Application.Top"/>. Additional Toplevels can be created 
///     and run (e.g. <see cref="Dialog"/>s. To run a Toplevel, create the <see cref="Core.Window"/> and 
///     call <see cref="Application.Run(Window, Func{Exception, bool})"/>.
///   </para>
///   <para>
///     Toplevels can also opt-in to more sophisticated initialization
///     by implementing <see cref="ISupportInitialize"/>. When they do
///     so, the <see cref="ISupportInitialize.BeginInit"/> and
///     <see cref="ISupportInitialize.EndInit"/> methods will be called
///     before running the view.
///     If first-run-only initialization is preferred, the <see cref="ISupportInitializeNotification"/>
///     can be implemented too, in which case the <see cref="ISupportInitialize"/>
///     methods will only be called if <see cref="ISupportInitializeNotification.IsInitialized"/>
///     is <see langword="false"/>. This allows proper <see cref="View"/> inheritance hierarchies
///     to override base class layout code optimally by doing so only on first run,
///     instead of on every run.
///   </para>
/// </remarks>
public class Window : View
{
    /// <summary>
    /// Gets or sets whether the <see cref="MainLoop"/> for this <see cref="Core.Window"/> is running or not. 
    /// </summary>
    /// <remarks>
    ///    Setting this property directly is discouraged. Use <see cref="Application.RequestStop"/> instead. 
    /// </remarks>
    public bool Running { get; set; }

    /// <summary>
    /// Fired once the Toplevel's <see cref="Application.RunState"/> has begin loaded.
    /// A Loaded event handler is a good place to finalize initialization before calling `<see cref="Application.RunLoop(Application.RunState, bool)"/>.
    /// </summary>
    public event Action? Loaded;

    /// <summary>
    /// Fired once the Toplevel's <see cref="MainLoop"/> has started it's first iteration.
    /// Subscribe to this event to perform tasks when the <see cref="Core.Window"/> has been laid out and focus has been set.
    /// changes. A Ready event handler is a good place to finalize initialization after calling `<see cref="Application.Run(Func{Exception, bool})"/>(topLevel)`.
    /// </summary>
    public event Action? Ready;

    /// <summary>
    /// Fired once the Toplevel's <see cref="Application.RunState"/> has begin unloaded.
    /// A Unloaded event handler is a good place to disposing after calling `<see cref="Application.End(Application.RunState)"/>.
    /// </summary>
    public event Action? Unloaded;

    /// <summary>
    /// Invoked once the Toplevel's <see cref="Application.RunState"/> becomes the <see cref="Application.Current"/>.
    /// </summary>
    public event Action<Window>? Activate;

    /// <summary>
    /// Invoked once the Toplevel's <see cref="Application.RunState"/> ceases to be the <see cref="Application.Current"/>.
    /// </summary>
    public event Action<Window>? Deactivate;

    /// <summary>
    /// Invoked once the child Toplevel's <see cref="Application.RunState"/> is closed from the <see cref="Application.End(View)"/>
    /// </summary>
    public event Action<Window>? ChildClosed;

    /// <summary>
    /// Invoked once the last child Toplevel's <see cref="Application.RunState"/> is closed from the <see cref="Application.End(View)"/>
    /// </summary>
    public event Action? AllChildClosed;

    /// <summary>
    /// Invoked once the Toplevel's <see cref="Application.RunState"/> is being closing from the <see cref="Application.RequestStop(Window)"/>
    /// </summary>
    public event Action<ToplevelClosingEventArgs>? Closing;

    /// <summary>
    /// Invoked once the Toplevel's <see cref="Application.RunState"/> is closed from the <see cref="Application.End(View)"/>
    /// </summary>
    public event Action<Window>? Closed;

    /// <summary>
    /// Invoked once the child Toplevel's <see cref="Application.RunState"/> has begin loaded.
    /// </summary>
    public event Action<Window>? ChildLoaded;

    /// <summary>
    /// Invoked once the child Toplevel's <see cref="Application.RunState"/> has begin unloaded.
    /// </summary>
    public event Action<Window>? ChildUnloaded;

    internal virtual void OnChildUnloaded(Window? top) 
    { 
        if (top != null)
            ChildUnloaded?.Invoke(top); 
    }

    internal virtual void OnChildLoaded(Window? top) 
    { 
        if(top != null)
            ChildLoaded?.Invoke(top); 
    }

    internal virtual void OnClosed(Window? top) 
    { 
        if (top != null)
            Closed?.Invoke(top); 
    }

    internal virtual bool OnClosing(ToplevelClosingEventArgs ev)
    {
        Closing?.Invoke(ev);
        return ev.Cancel;
    }

    internal virtual void OnAllChildClosed() { AllChildClosed?.Invoke(); }

    internal virtual void OnChildClosed(Window top)
    {
        if (IsMdiContainer) SetChildNeedsDisplay();
        ChildClosed?.Invoke(top);
    }

    internal virtual void OnDeactivate(Window activated) { Deactivate?.Invoke(activated); }

    internal virtual void OnActivate(Window deactivated) { Activate?.Invoke(deactivated); }

    /// <summary>
    /// Called from <see cref="Application.Begin(Window)"/> before the <see cref="Core.Window"/> is redraws for the first time.
    /// </summary>
    internal virtual void OnLoaded() { Loaded?.Invoke(); }

    /// <summary>
    /// Called from <see cref="Application.RunLoop"/> after the <see cref="Core.Window"/> has entered it's first iteration of the loop.
    /// </summary>
    internal virtual void OnReady() { Ready?.Invoke(); }

    /// <summary>
    /// Called from <see cref="Application.End(Application.RunState)"/> before the <see cref="Core.Window"/> is disposed.
    /// </summary>
    internal virtual void OnUnloaded() { Unloaded?.Invoke(); }

    /// <summary>
    /// Initializes a new instance of the <see cref="Core.Window"/> class using <see cref="LayoutStyle.Computed"/> positioning.
    /// </summary>
    public static Window CreateWindow(string? id = null)
    {
        var win = new Window(null);
        win.isWindow = true;
        if (id is not null)
            win.Id = id;
        return win;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Core.Window"/> class with <see cref="LayoutStyle.Computed"/> layout, defaulting to full screen.
    /// </summary>
    public Window()
        : base()
    {
        Initialize();
        Width = Dim.Fill();
        Height = Dim.Fill();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Core.Window"/> class with the specified absolute layout.
    /// </summary>
    /// <param name="frame">A superview-relative rectangle specifying the location and size for the new Toplevel</param>
    public Window(Rectangle frame)
        : base(frame)
    {
        Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Core.Window"/> class with an optional title using <see cref="LayoutStyle.Absolute"/> positioning.
    /// </summary>
    /// <param name="frame">Superview-relative rectangle specifying the location and size</param>
    /// <param name="title">Title</param>
    /// <remarks>
    /// This constructor initializes a Window with a <see cref="LayoutStyle"/> of <see cref="LayoutStyle.Absolute"/>. Use constructors
    /// that do not take <c>Rect</c> parameters to initialize a Window with <see cref="LayoutStyle.Computed"/>. 
    /// </remarks>
    public Window(Rectangle frame, string? title = null)
        : this(frame, title, 0, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Core.Window"/> class with an optional title using <see cref="LayoutStyle.Computed"/> positioning.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <remarks>
    ///   This constructor initializes a View with a <see cref="LayoutStyle"/> of <see cref="LayoutStyle.Computed"/>. 
    ///   Use <see cref="View.X"/>, <see cref="View.Y"/>, <see cref="View.Width"/>, and <see cref="View.Height"/> properties to dynamically control the size and location of the view.
    /// </remarks>
    public Window(string? title = null)
        : this(title, 0, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Core.Window"/> class using <see cref="LayoutStyle.Computed"/> positioning.
    /// </summary>
    //public Window()
    //    : this(null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Window"/> using <see cref="LayoutStyle.Absolute"/> positioning with the specified frame for its location, with the specified frame padding,
    /// and an optional title.
    /// </summary>
    /// <param name="frame">Superview-relative rectangle specifying the location and size</param>
    /// <param name="title">Title</param>
    /// <param name="padding">Number of characters to use for padding of the drawn frame.</param>
    /// <param name="border">The <see cref="Border"/>.</param>
    /// <remarks>
    /// This constructor initializes a Window with a <see cref="LayoutStyle"/> of <see cref="LayoutStyle.Absolute"/>. Use constructors
    /// that do not take <c>Rect</c> parameters to initialize a Window with  <see cref="LayoutStyle"/> of <see cref="LayoutStyle.Computed"/> 
    /// </remarks>
    public Window(Rectangle frame, string? title = null, int padding = 0, Border? border = null)
        : this(frame)
    {
        Initialize(title, frame, padding, border);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Core.Window"/> using <see cref="LayoutStyle.Computed"/> positioning,
    /// and an optional title.
    /// </summary>
    /// <param name="title">Title.</param>
    /// <param name="padding">Number of characters to use for padding of the drawn frame.</param>
    /// <param name="border">The <see cref="Border"/>.</param>
    /// <remarks>
    ///   This constructor initializes a View with a <see cref="LayoutStyle"/> of <see cref="LayoutStyle.Computed"/>. 
    ///   Use <see cref="View.X"/>, <see cref="View.Y"/>, <see cref="View.Width"/>, and <see cref="View.Height"/> properties to dynamically control the size and location of the view.
    /// </remarks>
    public Window(string? title = null, int padding = 0, Border? border = null)
        : this()
    {
        Initialize(title, Rectangle.Empty, padding, border);
    }

    private void Initialize() { ColorScheme = Colors.TopLevel; }

    private void Initialize(string? title, Rectangle frame, int padding = 0, Border? border = null)
    {
        isWindow = true;
        CanFocus = true;
        ColorScheme = Colors.Base;
        Title = title;
        Border = border ?? new Border()
        {
            Application = Application,
            BorderStyle = BorderStyle.Single,
            Padding = new Thickness(padding),
            BorderBrush = ColorScheme.Normal.Background
        };
    }

    private void AdjustContentView(Rectangle frame)
    {
        var borderLength = Border.DrawMarginFrame ? 1 : 0;
        var sumPadding = Border.GetSumThickness();
        var wb = new Size();
        if (frame == Rectangle.Empty)
        {
            wb.Width = borderLength + sumPadding.Right;
            wb.Height = borderLength + sumPadding.Bottom;
            if (contentView == null)
            {
                contentView = new ContentView(this)
                {
                    X = borderLength + sumPadding.Left,
                    Y = borderLength + sumPadding.Top,
                    Width = Dim.Fill(wb.Width),
                    Height = Dim.Fill(wb.Height)
                };
            }
            else
            {
                contentView.X = borderLength + sumPadding.Left;
                contentView.Y = borderLength + sumPadding.Top;
                contentView.Width = Dim.Fill(wb.Width);
                contentView.Height = Dim.Fill(wb.Height);
            }
        }
        else
        {
            wb.Width = 2 * borderLength + sumPadding.Right + sumPadding.Left;
            wb.Height = 2 * borderLength + sumPadding.Bottom + sumPadding.Top;
            var cFrame = new Rectangle(
                borderLength + sumPadding.Left,
                borderLength + sumPadding.Top,
                frame.Width - wb.Width,
                frame.Height - wb.Height);
            if (contentView == null)
                contentView = new ContentView(cFrame, this);
            else
                contentView.Frame = cFrame;
        }

        base.Add(contentView);
        Border.Child = contentView;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="Core.Window"/> can focus.
    /// </summary>
    /// <value><c>true</c> if can focus; otherwise, <c>false</c>.</value>
    public override bool CanFocus => SuperView == null ? true : base.CanFocus;

    /// <summary>
    /// Determines whether the <see cref="Core.Window"/> is modal or not.
    /// Causes <see cref="ProcessKey(KeyEvent)"/> to propagate keys upwards
    /// by default unless set to <see langword="true"/>.
    /// </summary>
    public bool Modal { get; set; }

    /// <summary>
    ///   The text displayed by the <see cref="Label"/>.
    /// </summary>
    public override string Text
    {
        get
        {
            if (isWindow)
                return contentView.Text;
            return base.Text;
        }
        set
        {
            base.Text = value;
            if (isWindow)
            {
                if (contentView != null) 
                    contentView.Text = value;
            }
        }
    }

    /// <summary>
    /// Controls the text-alignment property of the label, changing it will redisplay the <see cref="Label"/>.
    /// </summary>
    /// <value>The text alignment.</value>
    public override TextAlignment TextAlignment
    {
        get
        {
            if (isWindow)
                return contentView.TextAlignment;
            else
                return base.TextAlignment;
        }
        set
        {
            base.TextAlignment = value;
            if (isWindow)
                contentView.TextAlignment = value;
        }
    }

    private string? title;

    /// <summary>
    /// The title to be displayed for this window.
    /// </summary>
    /// <value>The title</value>
    public string? Title
    {
        get => title;
        set
        {
            title = value;
            SetNeedsDisplay();
        }
    }

    /// <summary>
    /// Gets or sets the menu for this Toplevel
    /// </summary>
    public virtual MenuBar MenuBar { get; set; }

    /// <summary>
    /// Gets or sets the status bar for this Toplevel
    /// </summary>
    public virtual StatusBar StatusBar { get; set; }

    /// <summary>
    /// Gets or sets if this Toplevel is a Mdi container.
    /// </summary>
    public bool IsMdiContainer { get; set; }

    /// <summary>
    /// Gets or sets if this Toplevel is a Mdi child.
    /// </summary>
    public bool IsMdiChild => Application.MdiTop != null && Application.MdiTop != this && !Modal;

    protected bool isWindow;
    private View contentView;

    /// <inheritdoc/>
    public override Border Border
    {
        get => base.Border;
        set
        {
            if (base.Border != null && base.Border.Child != null && value.Child == null)
                value.Child = base.Border.Child;
            base.Border = value;
            if (value == null) return;
            Rectangle frame;
            if (contentView != null && (contentView.Width is Dim || contentView.Height is Dim))
                frame = Rectangle.Empty;
            else
                frame = Frame;
            AdjustContentView(frame);

            Border.BorderChanged += Border_BorderChanged;
        }
    }

    private void Border_BorderChanged(Border border)
    {
        Rectangle frame;
        if (contentView != null && (contentView.Width is Dim || contentView.Height is Dim))
            frame = Rectangle.Empty;
        else
            frame = Frame;
        AdjustContentView(frame);
    }


    /// <summary>
    /// ContentView is an internal implementation detail of Window. It is used to host Views added with <see cref="Add(View)"/>. 
    /// Its ONLY reason for being is to provide a simple way for Window to expose to those SubViews that the Window's Bounds 
    /// are actually deflated due to the border. 
    /// </summary>
    private class ContentView : View
    {
        private Window instance;

        public ContentView(Rectangle frame, Window instance)
            : base(frame)
        {
            this.instance = instance;
        }

        public ContentView(Window instance)
            : base()
        {
            this.instance = instance;
        }

        public override bool MouseEvent(MouseEvent mouseEvent) { return instance.MouseEvent(mouseEvent); }
    }

    ///<inheritdoc/>
    public override bool OnKeyDown(KeyEvent keyEvent)
    {
        if (base.OnKeyDown(keyEvent)) return true;

        switch (keyEvent.Key)
        {
            case Key.AltMask:
            case Key.AltMask | Key.Space:
            case Key.CtrlMask | Key.Space:
                if (MenuBar != null && MenuBar.OnKeyDown(keyEvent)) return true;
                break;
        }

        return false;
    }

    ///<inheritdoc/>
    public override bool OnKeyUp(KeyEvent keyEvent)
    {
        if (base.OnKeyUp(keyEvent)) return true;

        switch (keyEvent.Key)
        {
            case Key.AltMask:
            case Key.AltMask | Key.Space:
            case Key.CtrlMask | Key.Space:
                if (MenuBar != null && MenuBar.OnKeyUp(keyEvent)) return true;
                break;
        }

        return false;
    }

    ///<inheritdoc/>
    public override bool ProcessKey(KeyEvent keyEvent)
    {
        if (base.ProcessKey(keyEvent))
            return true;

        switch (ShortcutHelper.GetModifiersKey(keyEvent))
        {
            case Key k when k == AsciiApplication.QuitKey:
                // FIXED: stop current execution of this container
                if (Application.MdiTop != null)
                    Application.MdiTop.RequestStop();
                else
                    Application.RequestStop();
                break;
            case Key.Z | Key.CtrlMask:
                Driver.Suspend();
                return true;

#if false
			case Key.F5:
				Application.DebugDrawBounds = !Application.DebugDrawBounds;
				SetNeedsDisplay ();
				return true;
#endif
            case Key.Tab:
            case Key.CursorRight:
            case Key.CursorDown:
            case Key.I | Key.CtrlMask: // Unix
                var old = GetDeepestFocusedSubview(Focused);
                if (!FocusNext())
                    FocusNext();
                if (old != Focused && old != Focused?.Focused)
                {
                    old?.SetNeedsDisplay();
                    Focused?.SetNeedsDisplay();
                }
                else
                {
                    FocusNearestView(SuperView?.TabIndexes, Direction.Forward);
                }

                return true;
            case Key.BackTab | Key.ShiftMask:
            case Key.CursorLeft:
            case Key.CursorUp:
                old = GetDeepestFocusedSubview(Focused);
                if (!FocusPrev())
                    FocusPrev();
                if (old != Focused && old != Focused?.Focused)
                {
                    old?.SetNeedsDisplay();
                    Focused?.SetNeedsDisplay();
                }
                else
                {
                    FocusNearestView(SuperView?.TabIndexes?.Reverse(), Direction.Backward);
                }

                return true;
            case Key.Tab | Key.CtrlMask:
            case Key key when key == AsciiApplication.AlternateForwardKey: // Needed on Unix
                if (Application.MdiTop == null)
                {
                    var top = Modal ? this : Application.Top;
                    top.FocusNext();
                    if (top.Focused == null) top.FocusNext();
                    top.SetNeedsDisplay();
                    Application.EnsuresTopOnFront();
                }
                else
                {
                    MoveNext();
                }

                return true;
            case Key.Tab | Key.ShiftMask | Key.CtrlMask:
            case Key key when key == AsciiApplication.AlternateBackwardKey: // Needed on Unix
                if (Application.MdiTop == null)
                {
                    var top = Modal ? this : Application.Top;
                    top.FocusPrev();
                    if (top.Focused == null) top.FocusPrev();
                    top.SetNeedsDisplay();
                    Application.EnsuresTopOnFront();
                }
                else
                {
                    MovePrevious();
                }

                return true;
            case Key.L | Key.CtrlMask:
                Application.Refresh();
                return true;
        }

        return false;
    }

    ///<inheritdoc/>
    public override bool ProcessColdKey(KeyEvent keyEvent)
    {
        if (base.ProcessColdKey(keyEvent)) return true;

        if (ShortcutHelper.FindAndOpenByShortcut(keyEvent, this)) return true;
        return false;
    }

    private View GetDeepestFocusedSubview(View view)
    {
        if (view == null) return null;

        foreach (var v in view.Subviews)
            if (v.HasFocus)
                return GetDeepestFocusedSubview(v);
        return view;
    }

    private void FocusNearestView(IEnumerable<View> views, Direction direction)
    {
        if (views == null) return;

        var found = false;
        var focusProcessed = false;
        var idx = 0;

        foreach (var v in views)
        {
            if (v == this) found = true;
            if (found && v != this)
            {
                if (direction == Direction.Forward)
                    SuperView?.FocusNext();
                else
                    SuperView?.FocusPrev();
                focusProcessed = true;
                if (SuperView.Focused != null && SuperView.Focused != this) return;
            }
            else if (found && !focusProcessed && idx == views.Count() - 1)
            {
                views.ToList()[0].SetFocus();
            }

            idx++;
        }
    }

    ///<inheritdoc/>
    public override void Add(View view)
    {
        view.Application = Application;
        if (isWindow)
        {
            contentView.Add(view);
            if (view.CanFocus) 
                CanFocus = true;
            AddMenuStatusBar(view);
        }
        else
        {
            AddMenuStatusBar(view);
            base.Add(view);
        }
    }

    internal void AddMenuStatusBar(View view)
    {
        if (view is MenuBar menuBar) 
            MenuBar = menuBar;
        else if (view is StatusBar statusBar) 
            StatusBar = statusBar;
        
        view.Application = Application;
    }

    ///<inheritdoc/>
    public override void Remove(View view)
    {
        if (isWindow)
        {
            if (view == null) 
                return;

            SetNeedsDisplay();
            contentView.Remove(view);

            if (contentView.InternalSubviews.Count < 1) 
                CanFocus = false;
            RemoveMenuStatusBar(view);
            if (view != contentView && Focused == null) 
                FocusFirst();

            view.Application = null;
        }
        else
        {
            if (this is Window toplevel && toplevel.MenuBar != null)
                RemoveMenuStatusBar(view);
            base.Remove(view);
        }
    }

    ///<inheritdoc/>
    public override void RemoveAll()
    {
        if (isWindow)
        {
            contentView.RemoveAll();
        }
        else
        {
            if (this == Application.Top)
            {
                MenuBar?.Dispose();
                MenuBar = null;
                StatusBar?.Dispose();
                StatusBar = null;
            }

            base.RemoveAll();
        }
    }

    internal void RemoveMenuStatusBar(View view)
    {
        if (view is MenuBar)
        {
            MenuBar?.Dispose();
            MenuBar = null;
        }

        if (view is StatusBar)
        {
            StatusBar?.Dispose();
            StatusBar = null;
        }
    }

    internal View EnsureVisibleBounds(
        Window top,
        int x,
        int y,
        out int nx,
        out int ny,
        out View mb,
        out View sb)
    {
        int l;
        View superView;
        if (top?.SuperView == null || top == Application.Top || top?.SuperView == Application.Top)
        {
            l = Driver.Cols;
            superView = Application.Top;
        }
        else
        {
            l = top.SuperView.Frame.Width;
            superView = top.SuperView;
        }

        nx = Math.Max(x, 0);
        nx = nx + top.Frame.Width > l ? Math.Max(l - top.Frame.Width, 0) : nx;
        SetWidth(top.Frame.Width, out var rWidth);
        if (rWidth < 0 && nx >= top.Frame.X) nx = Math.Max(top.Frame.Right - 2, 0);
        //System.Diagnostics.Debug.WriteLine ($"nx:{nx}, rWidth:{rWidth}");
        bool m, s;
        if (top?.SuperView == null || top == Application.Top || top?.SuperView == Application.Top)
        {
            m = Application.Top.MenuBar?.Visible == true;
            mb = Application.Top.MenuBar;
        }
        else
        {
            var t = top.SuperView;
            while (!(t is Window)) t = t.SuperView;
            m = ((Window)t).MenuBar?.Visible == true;
            mb = ((Window)t).MenuBar;
        }

        if (top?.SuperView == null || top == Application.Top || top?.SuperView == Application.Top)
            l = m ? 1 : 0;
        else
            l = 0;
        ny = Math.Max(y, l);
        if (top?.SuperView == null || top == Application.Top || top?.SuperView == Application.Top)
        {
            s = Application.Top.StatusBar?.Visible == true;
            sb = Application.Top.StatusBar;
        }
        else
        {
            var t = top.SuperView;
            while (!(t is Window)) t = t.SuperView;
            s = ((Window)t).StatusBar?.Visible == true;
            sb = ((Window)t).StatusBar;
        }

        if (top?.SuperView == null || top == Application.Top || top?.SuperView == Application.Top)
            l = s ? Driver.Rows - 1 : Driver.Rows;
        else
            l = s ? top.SuperView.Frame.Height - 1 : top.SuperView.Frame.Height;
        ny = Math.Min(ny, l);
        ny = ny + top.Frame.Height >= l ? Math.Max(l - top.Frame.Height, m ? 1 : 0) : ny;
        SetHeight(top.Frame.Height, out var rHeight);
        if (rHeight < 0 && ny >= top.Frame.Y) ny = Math.Max(top.Frame.Bottom - 2, 0);
        //System.Diagnostics.Debug.WriteLine ($"ny:{ny}, rHeight:{rHeight}");

        return superView;
    }

    internal void PositionToplevels()
    {
        PositionToplevel(this);
        foreach (var top in Subviews)
            if (top is Window)
                PositionToplevel((Window)top);
    }

    /// <summary>
    /// Virtual method which allow to be overridden to implement specific positions for inherited <see cref="Core.Window"/>.
    /// </summary>
    /// <param name="top">The toplevel.</param>
    public virtual void PositionToplevel(Window top)
    {
        var superView = EnsureVisibleBounds(
            top,
            top.Frame.X,
            top.Frame.Y,
            out var nx,
            out var ny,
            out _,
            out View sb);
        var layoutSubviews = false;
        if ((top?.SuperView != null || top != Application.Top && top.Modal
                                    || top?.SuperView == null && top.IsMdiChild)
            && (nx > top.Frame.X || ny > top.Frame.Y) && top.LayoutStyle == LayoutStyle.Computed)
        {
            if ((top.X == null || top.X is Pos.PosAbsolute) && top.Bounds.X != nx)
            {
                top.X = nx;
                layoutSubviews = true;
            }

            if ((top.Y == null || top.Y is Pos.PosAbsolute) && top.Bounds.Y != ny)
            {
                top.Y = ny;
                layoutSubviews = true;
            }
        }

        if (sb != null && ny + top.Frame.Height != superView.Frame.Height - (sb.Visible ? 1 : 0)
                       && top.Height is Dim.DimFill)
        {
            top.Height = Dim.Fill(sb.Visible ? 1 : 0);
            layoutSubviews = true;
        }

        if (layoutSubviews) superView.LayoutSubviews();
    }

    /// <inheritdoc/>
    public void WindowRedraw(Rectangle bounds)
    {
        var padding = Border.GetSumThickness();
        var scrRect = ViewToScreen(new Rectangle(0, 0, base.Frame.Width, base.Frame.Height));
        //var borderLength = Border.DrawMarginFrame ? 1 : 0;

        // BUGBUG: Why do we draw the frame twice? This call is here to clear the content area, I think. Why not just clear that area?
        if (!NeedDisplay.IsEmpty)
        {
            Driver.SetAttribute(GetNormalColor());
            //Driver.DrawWindowFrame (scrRect, padding.Left + borderLength, padding.Top + borderLength, padding.Right + borderLength, padding.Bottom + borderLength,
            //	Border.BorderStyle != BorderStyle.None, fill: true, Border);
            Border.DrawContent();
        }

        var savedClip = contentView.ClipToBounds();

        // Redraw our contentView
        // TODO: smartly constrict contentView.Bounds to just be what intersects with the 'bounds' we were passed
        contentView.Redraw(contentView.Bounds);
        Driver.Clip = savedClip;

        ClearLayoutNeeded();
        ClearNeedsDisplay();
        if (Border.BorderStyle != BorderStyle.None)
        {
            Driver.SetAttribute(GetNormalColor());
            //Driver.DrawWindowFrame (scrRect, padding.Left + borderLength, padding.Top + borderLength, padding.Right + borderLength, padding.Bottom + borderLength,
            //	Border.BorderStyle != BorderStyle.None, fill: true, Border.BorderStyle);
            if (HasFocus)
                Driver.SetAttribute(ColorScheme.HotNormal);
            Driver.DrawWindowTitle(scrRect, Title, padding.Left, padding.Top, padding.Right, padding.Bottom);
        }

        Driver.SetAttribute(GetNormalColor());

        // Checks if there are any SuperView view which intersect with this window.
        if (SuperView != null)
        {
            SuperView.SetNeedsLayout();
            SuperView.SetNeedsDisplay();
        }
    }

    ///<inheritdoc/>
    public override void Redraw(Rectangle bounds)
    {
        if (isWindow)
        {
            WindowRedraw(bounds);
            return;
        }

        if (!Visible) return;

        if (!NeedDisplay.IsEmpty || ChildNeedsDisplay || LayoutNeeded)
        {
            Driver.SetAttribute(GetNormalColor());

            // This is the Application.Top. Clear just the region we're being asked to redraw 
            // (the bounds passed to us).
            Clear();
            Driver.SetAttribute(Enabled ? Colors.Base.Normal : Colors.Base.Disabled);

            LayoutSubviews();
            PositionToplevels();

            if (this == Application.MdiTop)
                foreach (var top in Application.MdiChildes.AsEnumerable().Reverse())
                    if (top.Frame.IntersectsWith(bounds))
                        if (top != this && !top.IsCurrentWindowTop && !OutsideTopFrame(top) && top.Visible)
                        {
                            top.SetNeedsLayout();
                            top.SetNeedsDisplay(top.Bounds);
                            top.Redraw(top.Bounds);
                        }

            foreach (var view in Subviews)
                if (view.Frame.IntersectsWith(bounds) && !OutsideTopFrame(this))
                {
                    view.SetNeedsLayout();
                    view.SetNeedsDisplay(view.Bounds);
                    //view.Redraw (view.Bounds);
                }

            ClearLayoutNeeded();
            ClearNeedsDisplay();
        }

        base.Redraw(Bounds);
    }

    private bool OutsideTopFrame(Window top)
    {
        if (top.Frame.X > Driver.Cols || top.Frame.Y > Driver.Rows) return true;
        return false;
    }

    //
    // FIXED:It does not look like the event is raised on clicked-drag
    // need to figure that out.
    //
    internal static Point? dragPosition;
    private Point start;

    ///<inheritdoc/>
    public override bool MouseEvent(MouseEvent mouseEvent)
    {
        // FIXED:The code is currently disabled, because the
        // Driver.UncookMouse does not seem to have an effect if there is
        // a pending mouse event activated.

        if (!CanFocus) return true;

        int nx, ny;
        if (!dragPosition.HasValue && (mouseEvent.Flags == MouseFlags.Button1Pressed
                                       || mouseEvent.Flags == MouseFlags.Button2Pressed
                                       || mouseEvent.Flags == MouseFlags.Button3Pressed))
        {
            SetFocus();
            Application.EnsuresTopOnFront();

            // Only start grabbing if the user clicks on the title bar.
            if (mouseEvent.Y == 0 && mouseEvent.Flags == MouseFlags.Button1Pressed)
            {
                start = new Point(mouseEvent.X, mouseEvent.Y);
                dragPosition = new Point();
                nx = mouseEvent.X - mouseEvent.OfX;
                ny = mouseEvent.Y - mouseEvent.OfY;
                dragPosition = new Point(nx, ny);
                Application.GrabMouse(this);
            }

            //System.Diagnostics.Debug.WriteLine ($"Starting at {dragPosition}");
            return true;
        }
        else if (mouseEvent.Flags == (MouseFlags.Button1Pressed | MouseFlags.ReportMousePosition) ||
                 mouseEvent.Flags == MouseFlags.Button3Pressed)
        {
            if (dragPosition.HasValue)
            {
                if (SuperView == null) // Redraw the entire app window using just our Frame. Since we are 
                                       // Application.Top, and our Frame always == our Bounds (Location is always (0,0))
                                       // our Frame is actually view-relative (which is what Redraw takes).
                                       // We need to pass all the view bounds because since the windows was 
                                       // moved around, we don't know exactly what was the affected region.
                    Application.Top.SetNeedsDisplay();
                else
                    SuperView.SetNeedsDisplay();
                EnsureVisibleBounds(
                    this,
                    mouseEvent.X + (SuperView == null ? mouseEvent.OfX - start.X : Frame.X - start.X),
                    mouseEvent.Y + (SuperView == null ? mouseEvent.OfY : Frame.Y),
                    out nx,
                    out ny,
                    out _,
                    out _);

                dragPosition = new Point(nx, ny);
                LayoutSubviews();
                base.Frame = new Rectangle(nx, ny, base.Frame.Width, base.Frame.Height);
                if (X == null || X is Pos.PosAbsolute) X = nx;
                if (Y == null || Y is Pos.PosAbsolute) Y = ny;
                //System.Diagnostics.Debug.WriteLine ($"nx:{nx},ny:{ny}");

                // FIXED: optimize, only SetNeedsDisplay on the before/after regions.
                SetNeedsDisplay();
                return true;
            }
        }

        if (mouseEvent.Flags == MouseFlags.Button1Released && dragPosition.HasValue)
        {
            Application.UngrabMouse();
            Driver.UncookMouse();
            dragPosition = null;
        }

        //System.Diagnostics.Debug.WriteLine (mouseEvent.ToString ());
        return false;
    }

    /// <summary>
    /// Invoked by <see cref="Application.Begin"/> as part of the <see cref="Application.Run(Window, Func{Exception, bool})"/> after
    /// the views have been laid out, and before the views are drawn for the first time.
    /// </summary>
    public virtual void WillPresent() { FocusFirst(); }

    /// <summary>
    /// Move to the next Mdi child from the <see cref="Application.MdiTop"/>.
    /// </summary>
    public virtual void MoveNext() { Application.MoveNext(); }

    /// <summary>
    /// Move to the previous Mdi child from the <see cref="Application.MdiTop"/>.
    /// </summary>
    public virtual void MovePrevious() { Application.MovePrevious(); }

    /// <summary>
    /// Stops running this <see cref="Core.Window"/>.
    /// </summary>
    public virtual void RequestStop()
    {
        if (IsMdiContainer && Running
                           && (Application.CurrentWindow == this
                               || Application.CurrentWindow?.Modal == false
                               || Application.CurrentWindow?.Modal == true && Application.CurrentWindow?.Running == false))
        {
            foreach (var child in Application.MdiChildes)
            {
                var ev = new ToplevelClosingEventArgs(this);
                if (child.OnClosing(ev)) return;
                child.Running = false;
                Application.RequestStop(child);
            }

            Running = false;
            Application.RequestStop(this);
        }
        else if (IsMdiContainer && Running && Application.CurrentWindow?.Modal == true &&
                 Application.CurrentWindow?.Running == true)
        {
            var ev = new ToplevelClosingEventArgs(Application.CurrentWindow);
            if (OnClosing(ev)) return;
            Application.RequestStop(Application.CurrentWindow);
        }
        else if (!IsMdiContainer && Running && (!Modal || Modal && Application.CurrentWindow != this))
        {
            var ev = new ToplevelClosingEventArgs(this);
            if (OnClosing(ev)) return;
            Running = false;
            Application.RequestStop(this);
        }
        else
        {
            Application.RequestStop(Application.CurrentWindow);
        }
    }

    /// <summary>
    /// Stops running the <paramref name="top"/> <see cref="Core.Window"/>.
    /// </summary>
    /// <param name="top">The toplevel to request stop.</param>
    public virtual void RequestStop(Window top) { top.RequestStop(); }

    ///<inheritdoc/>
    public override void PositionCursor()
    {
        if (!IsMdiContainer)
        {
            base.PositionCursor();
            return;
        }

        if (Focused == null)
            foreach (var top in Application.MdiChildes)
                if (top != this && top.Visible)
                {
                    top.SetFocus();
                    return;
                }

        base.PositionCursor();
    }

    /// <summary>
    /// Gets the current visible toplevel Mdi child that match the arguments pattern.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="exclude">The strings to exclude.</param>
    /// <returns>The matched view.</returns>
    public View GetTopMdiChild(Type type = null, string[] exclude = null)
    {
        if (Application.MdiTop == null) return null;

        foreach (var top in Application.MdiChildes)
        {
            if (type != null && top.GetType() == type
                             && exclude?.Contains(top.Data.ToString()) == false)
                return top;
            else if (type != null && top.GetType() != type
                     || exclude?.Contains(top.Data.ToString()) == true)
                continue;
            return top;
        }

        return null;
    }

    /// <summary>
    /// Shows the Mdi child indicated by the <paramref name="top"/> setting as <see cref="Application.Current"/>.
    /// </summary>
    /// <param name="top">The toplevel.</param>
    /// <returns><see langword="true"/> if the toplevel can be showed.<see langword="false"/> otherwise.</returns>
    public virtual bool ShowChild(Window top = null)
    {
        if (Application.MdiTop != null) return Application.ShowChild(top == null ? this : top);
        return false;
    }
}

/// <summary>
/// Implements the <see cref="IEqualityComparer{T}"/> to comparing two <see cref="Window"/> used by <see cref="StackExtensions"/>.
/// </summary>
public class ToplevelEqualityComparer : IEqualityComparer<Window?>
{
    /// <summary>Determines whether the specified objects are equal.</summary>
    /// <param name="x">The first object of type <see cref="Window" /> to compare.</param>
    /// <param name="y">The second object of type <see cref="Window" /> to compare.</param>
    /// <returns>
    ///     <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.</returns>
    public bool Equals(Window? x, Window? y)
    {
        if (y == null && x == null)
            return true;
        else if (x == null || y == null)
            return false;
        else if (x.Id == y.Id)
            return true;
        else
            return false;
    }

    /// <summary>Returns a hash code for the specified object.</summary>
    /// <param name="obj">The <see cref="Window" /> for which a hash code is to be returned.</param>
    /// <returns>A hash code for the specified object.</returns>
    /// <exception cref="ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is <see langword="null" />.</exception>
    public int GetHashCode(Window? obj)
    {
        if (obj == null)
            throw new ArgumentNullException();

        var hCode = 0;
        if (int.TryParse(obj.Id.ToString(), out var result)) hCode = result;
        return hCode.GetHashCode();
    }
}

/// <summary>
/// Implements the <see cref="IComparer{T}"/> to sort the <see cref="Window"/> from the <see cref="Application.MdiChildes"/> if needed.
/// </summary>
public sealed class ToplevelComparer : IComparer<Window>
{
    /// <summary>Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero
    ///             <paramref name="x" /> is less than <paramref name="y" />.Zero
    ///             <paramref name="x" /> equals <paramref name="y" />.Greater than zero
    ///             <paramref name="x" /> is greater than <paramref name="y" />.</returns>
    public int Compare(Window x, Window y)
    {
        if (ReferenceEquals(x, y))
            return 0;
        else if (x == null)
            return -1;
        else if (y == null)
            return 1;
        else
            return string.Compare(x.Id.ToString(), y.Id.ToString());
    }
}

/// <summary>
/// <see cref="System.EventArgs"/> implementation for the <see cref="Window.Closing"/> event.
/// </summary>
public class ToplevelClosingEventArgs : System.EventArgs
{
    /// <summary>
    /// The toplevel requesting stop.
    /// </summary>
    public View RequestingTop { get; }

    /// <summary>
    /// Provides an event cancellation option.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Initializes the event arguments with the requesting toplevel.
    /// </summary>
    /// <param name="requestingTop">The <see cref="RequestingTop"/>.</param>
    public ToplevelClosingEventArgs(Window requestingTop) { RequestingTop = requestingTop; }
}
