#if TRACE_POINTER
using System.Diagnostics;
#endif
using No8.Ascii.ElementLayout;

namespace No8.Ascii.Controls;

public class Window : Control
{
    public Window(ControlPlan? plan = null, Style? style = null)
        : base(plan ?? new WindowLayoutPlan(), style)
    {
    }

    public Window(out Window node, ControlPlan? plan = null, Style? style = null)
        : this(plan, style)
    {
        node = this;
    }

    //--

    internal bool Active  { get; set; }
    public   bool IsModal { get; set; }

    public Vec Pointer { get; private set; } = Vec.Unknown;

    public new WindowLayoutPlan ControlPlan => (WindowLayoutPlan)_controlPlan!;
    

    //--
    public event EventHandler?                         Loaded;
    public event EventHandler?                         Ready;
    public event EventHandler?                         Unloaded;
    public event EventHandler<Window>?                 Activate;
    public event EventHandler<Window>?                 Deactivate;
    public event EventHandler<Window>?                 ChildOpened;
    public event EventHandler<Window>?                 ChildClosed;
    public event EventHandler?                         AllChildrenClosed;
    public event EventHandler<WindowClosingEventArgs>? WindowClosing;
    public event EventHandler<Window>?                 WindowClosed;
    public event EventHandler<PointerEvent>?           PointerEvent;

    //--

    /// <summary>
    /// Calculate layout.  *Not recommended* as this is automatically done by the app
    /// </summary>
    public void DoLayout(int width, int height)
    {
        ElementArrange.Calculate(this, width, height);
        IsDirty = true;    // Force recalculate properly
    }

    //--
    protected void RaiseLoaded()   => Loaded?.Invoke(this, EventArgs.Empty);
    protected void RaiseReady()    => Ready?.Invoke(this, EventArgs.Empty);
    protected void RaiseUnloaded() => Unloaded?.Invoke(this, EventArgs.Empty);

    protected void RaiseActivate(Window    window) => Activate?.Invoke(this, window);
    protected void RaiseDeactivate(Window  window) => Deactivate?.Invoke(this, window);
    protected void RaiseChildClosed(Window window) => ChildClosed?.Invoke(this, window);
    protected void RaiseChildOpened(Window window) => ChildOpened?.Invoke(this, window);

    protected void        RaiseAllChildrenClosed()                          => AllChildrenClosed?.Invoke(this, EventArgs.Empty);
    protected void        RaiseWindowClosing(WindowClosingEventArgs e)      => WindowClosing?.Invoke(this, e);
    protected void        RaiseWindowClosed(Window                  window) => WindowClosed?.Invoke(this, window);

    internal bool HandlePointerEvent(PointerEvent pointerEvent)
    {
        PointerEvent?.Invoke(this, pointerEvent);

#if TRACE_POINTER
        var sb          = new StringBuilder();
        sb.Append(pointerEvent);
#endif

        var lastPointer = Pointer;
        Pointer = new Vec(pointerEvent.X, pointerEvent.Y);

        var lastControls         = FindControlsAt(lastPointer.X, lastPointer.Y);
        var controlsUnderPointer = FindControlsAt(pointerEvent.X, pointerEvent.Y).ToArray();

        var controlsLeave       = lastControls.Where(c => !controlsUnderPointer.Contains(c));
        var controlsEnter       = controlsUnderPointer.Where(c => !lastControls.Contains(c));

        foreach (var control in controlsLeave)
        {
#if TRACE_POINTER
            sb.Append($" Leave: {control.ShortString}");
#endif
            control.OnPointerLeave(
                new PointerEvent(pointerEvent.TimeStamp, PointerEventType.Leave, pointerEvent.X, pointerEvent.Y));
        }
        foreach (var control in controlsEnter)
        {
#if TRACE_POINTER
            sb.Append($" Enter: {control.ShortString}");
#endif
            control.OnPointerEnter(
                new PointerEvent(pointerEvent.TimeStamp, PointerEventType.Enter, pointerEvent.X, pointerEvent.Y));
        }

        foreach (var control in controlsUnderPointer.Reverse()) // Closest control to Window
        {
#if TRACE_POINTER
            sb.Append(", ");
            sb.Append(control.ShortString);
#endif

            switch (pointerEvent.PointerEventType)
            {
            case PointerEventType.Move:
                control.OnPointerMove(pointerEvent);
                break;
            case PointerEventType.Pressed:
                control.OnPointerPressed(pointerEvent);
                break;
            case PointerEventType.Released:
                control.OnPointerReleased(pointerEvent);
                break;
            case PointerEventType.Click:
                control.OnPointerClick(pointerEvent);
                if (control.CanFocus && !control.HasFocus)
                {
                    control.HasFocus      = true;
                    control.NeedsPainting = true;
                }
                break;
            case PointerEventType.DoubleClick:
                control.OnPointerDoubleClick(pointerEvent);
                break;
            case PointerEventType.Wheel:
                control.OnPointerWheel(pointerEvent);
                break;
            case PointerEventType.HorizontalWheel:
                control.OnPointerHorizontalWheel(pointerEvent);
                break;


            case PointerEventType.Enter: break;
            case PointerEventType.Leave: break;
            default:                     break;
            }
        }

        NeedsPainting = true;   // TODO: I don't think we need to do this

        if (pointerEvent.PointerEventType == PointerEventType.Leave && lastPointer != Vec.Unknown)
        {
            Pointer = Vec.Unknown;
            foreach (var control in lastControls)
            {
#if TRACE_POINTER
                sb.Append($" Leave Screen: {control.ShortString}");
#endif
                control.OnPointerLeave(
                    new PointerEvent(pointerEvent.TimeStamp, PointerEventType.Leave, pointerEvent.X, pointerEvent.Y));
            }
        }

#if TRACE_POINTER
        Trace.WriteLine(sb.ToString());
#endif

        return false;
    }

    public List<Control> FindControlsAt(int x, int y)
    {
        var list = new List<Control>();

        FindControlsAt(list, this, x, y);

        return list;
    }

    private void FindControlsAt(List<Control> list, Control control, int x, int y)
    {
        if (control.Layout.Bounds.Contains(x, y))
            list.Add(control);

        foreach (var child in control.Children)
            FindControlsAt(list, child, x, y);
    }


}