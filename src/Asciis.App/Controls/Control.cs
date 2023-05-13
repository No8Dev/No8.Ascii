using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using Asciis.App.Controls.Animation;
using Asciis.App.ElementLayout;

namespace Asciis.App.Controls;


public interface IHasStyle<out T>
    where T : Style
{
    T Style { get; }
}

public interface IHasLayoutPlan<out T>
    where T : LayoutPlan
{
    T Plan { get; }
}


/// <summary>
///     Base control containing all basic properties used to define the control
/// </summary>
[DebuggerDisplay("{ShortString}")]
public abstract class Control : IEnumerable<Control>, IElement, IHasStyle<Style>, IHasLayoutPlan<LayoutPlan>, IAnimatable
{
    public delegate void PropertyChangedDelegate<in T>(T oldValue, T newValue);

    public delegate void PropertyChangingDelegate();

    private readonly List<Control>                _children               = new();
    private readonly Dictionary<string, Delegate> _propertyChangedActions = new();

    private readonly Dictionary<string, PropertyChangingDelegate> _propertyChangingActions = new();

    private   bool          _hasFocus;
    private   IApp?         _app;
    private   bool          _isDirty;
    private   bool          _isEnabled = true;
    protected bool          _isMouseOver;
    private   LayoutActual? _layout        = new();
    private   bool          _needsLayout   = true;
    private   bool          _needsPainting = true;

    protected LayoutPlan? _plan;
    protected Style?      _style;


    public event EventHandler?         Dirtied;
    public event EventHandler<float>?  Update;
    public event EventHandler<Canvas>? Draw;

    //**********************************************

    protected Control(LayoutPlan? plan = null, Style? style = null)
    {
        if (plan != null)
        {
            Plan.MergeFrom(plan);
            _isDirty = Plan.IsDirty;
        }

        SetupPlanChanged();

        _style = DefaultStyle(this);
        if (style != null)
        {
            Style    =  style;
            _isDirty |= style.IsDirty;
        }
        else
            Style = DefaultStyle(this);

        SetupStyleChanged();
    }

    protected Control(
        out Control control,
        LayoutPlan? plan  = null,
        Style?      style = null)
        : this(plan, style)
    {
        control = this;
    }

    //--

    public IApp? App
    {
        get => GetApp();
        internal set => SetHost(value);
    }

    public Brush? ForegroundBrush
    {
        get => Style.ForegroundBrush;
        set => Style.ForegroundBrush = value;
    }

    /// <summary>
    ///     The boundtry of the control. Includes Transformations
    /// </summary>
    public RectF Bounds
    {
        get
        {
            return Layout.Bounds
                .Offset(Style.TranslationX ?? 0, Style.TranslationY ?? 0)
                .Scale(Style.ScaleX, Style.ScaleY);
        }
    }

    public RectF ContentBounds
    {
        get
        {
            var bounds = Bounds;
            return new(
                bounds.X + Layout.Padding.Left,
                bounds.Y + Layout.Padding.Top,
                bounds.Width - (Layout.Padding.Left + Layout.Padding.Right),
                bounds.Height - (Layout.Padding.Top + Layout.Padding.Bottom)
                );
        }
    }

    public Brush? BackgroundBrush
    {
        get => Style.BackgroundBrush;
        set => Style.BackgroundBrush = value;
    }

    public float TranslationX
    {
        get => Style.TranslationX ?? 0;
        set => Style.TranslationX = value;
    }
    public float TranslationY
    {
        get => Style.TranslationY ?? 0;
        set => Style.TranslationY = value;
    }

    public float Scale
    {
        get => Style.ScaleX ?? 1;
        set
        {
            Style.ScaleX = value;
            Style.ScaleY = value;
        }
    }
    public float ScaleX
    {
        get => Style.ScaleX ?? 1;
        set => Style.ScaleX = value;
    }
    public float ScaleY
    {
        get => Style.ScaleY ?? 1;
        set => Style.ScaleY = value;
    }

    public float Opacity
    {
        get => Style.Opacity ?? 1;
        set => Style.Opacity = value;
    }

    public virtual bool CanFocus { get; }

    public bool HasFocus
    {
        get => _hasFocus;
        internal set => ChangeDirtiesPainting(ref _hasFocus, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => ChangeDirtiesPainting(ref _isEnabled, value);
    }

    public bool IsMouseOver
    {
        get => _isMouseOver;
        internal set => ChangeDirtiesPainting(ref _isMouseOver, value);
    }

    public virtual string ShortString =>
        Name != null
            ? $"{GetType().Name} [{Name}]>"
            : $"{GetType().Name}>";

    public IEnumerator<Control> GetEnumerator() { return _children.GetEnumerator(); }

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }


    public Control?     Parent      { get; internal set; }
    public object?      Context     { get; set; }
    public MeasureFunc? MeasureFunc { get; set; }

    public string? Name { get; set; }

    public LayoutPlan Plan
    {
        get =>
            _plan ??= DefaultLayoutPlan(this)
               .OnChanged(PlanOnChanged);
        init
        {
            if (_plan == null)
                _plan = DefaultLayoutPlan(this)
                       .MergeFrom(value)
                       .OnChanged(PlanOnChanged);
            else
                _plan.MergeFrom(value);
        }
    }

    public bool NeedsLayout
    {
        get => _needsLayout;
        set
        {
            if (_needsLayout)
                return;

            MarkDirty();
            _needsLayout = true;
            if (App != null)
                App.NeedsLayout = true;
        }
    }

    public Style Style
    {
        get => _style ??= DefaultStyle(this).OnChanged(StyleOnChanged);
        init
        {
            if (_style == null)
                _style = DefaultStyle(this)
                        .MergeFrom(value)
                        .OnChanged(StyleOnChanged);
            else
                _style.MergeFrom(value);
        }
    }


    public bool NeedsPainting
    {
        get => _needsPainting;
        set
        {
            if (_needsPainting)
                return;

            _needsPainting = true;
            foreach (var child in Children)
                child.NeedsPainting = true;
            if (App != null)
                App.NeedsPainting = true;
        }
    }

    public LayoutActual Layout
    {
        get => _layout ??= new();
        set
        {
            if (_layout != value)
            {
                _layout = value;
                MarkDirty();
            }
        }
    }

    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (_isDirty != value)
            {
                _isDirty = value;
                if (value)
                    RaiseDirtied();
                else
                {
                    Plan.IsDirty   = false;
                    if (_style != null)
                        _style.IsDirty = false;
                }
            }
        }
    }

    public IReadOnlyList<Control> Children => _children.AsReadOnly();

    //--

    public static bool Traverse(Control control, Func<Control, bool> action, TraverseStrategy strategy = TraverseStrategy.ParentThenChildren)
    {
        if (strategy == TraverseStrategy.ParentThenChildren && action(control))
            return true;

        foreach (var child in control.Children.ToArray())
        {
            if (Traverse(child, action, strategy))
                return true;
        }

        if (strategy == TraverseStrategy.ChildrenThenParent && action(control))
            return true;

        return false;
    }


    //--

    protected void ChangeDirtiesLayout<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            RaisePropertyChanging(name);
            var old = field;
            field         = value;
            NeedsLayout   = true;
            NeedsPainting = true;
            RaisePropertyChanged(name, old, value);
        }
    }

    protected void ChangeDirtiesPainting<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            RaisePropertyChanging(name);
            var old = field;
            field         = value;
            NeedsPainting = true;
            RaisePropertyChanged(name, old, value);
        }
    }

    protected void ChangeDirtiesLayoutIfNotNull<T>(ref T field, T value, [CallerMemberName] string name = "")
        where T : class
    {
        if (value is not null)
            ChangeDirtiesLayout(ref field, value, name);
    }

    protected void ChangeDirtiesLayoutIfNotNull<T>(ref T field, T? value, [CallerMemberName] string name = "")
        where T : struct
    {
        if (value is not null)
            ChangeDirtiesLayout(ref field, value.Value, name);
    }

    protected void ChangeDirtiesPaintingIfNotNull<T>(ref T field, T value, [CallerMemberName] string name = "")
        where T : class
    {
        if (value is not null)
            ChangeDirtiesPainting(ref field, value, name);
    }

    protected void ChangeDirtiesPaintingIfNotNull<T>(ref T field, T? value, [CallerMemberName] string name = "")
        where T : struct
    {
        if (value is not null)
            ChangeDirtiesPainting<T>(ref field, value.Value, name);
    }


    /// <summary>
    ///     Register action to be called when property about to change
    /// </summary>
    public void OnPropertyChanging(string propertyName, PropertyChangingDelegate action)
    {
        if (!_propertyChangingActions.ContainsKey(propertyName))
            _propertyChangingActions[propertyName] = action;
        else
            _propertyChangingActions[propertyName] = (PropertyChangingDelegate)Delegate.Combine(
                _propertyChangingActions[propertyName],
                action);
    }

    /// <summary>
    ///     Register action to be called when property has changed
    /// </summary>
    public void OnPropertyChanged<T>(string propertyName, PropertyChangedDelegate<T> action)
    {
        if (!_propertyChangedActions.ContainsKey(propertyName))
            _propertyChangedActions[propertyName] = action;
        else
            _propertyChangedActions[propertyName] = Delegate.Combine(_propertyChangedActions[propertyName], action);
    }

    protected virtual void RaisePropertyChanging(string name)
    {
        if (_propertyChangingActions.ContainsKey(name))
            _propertyChangingActions[name]?.Invoke();
    }

    protected virtual void RaisePropertyChanged<T>(string name, T oldValue, T newValue)
    {
        if (_propertyChangedActions.ContainsKey(name))
            _propertyChangedActions[name]?.DynamicInvoke(oldValue, newValue);
    }

    public void CleanFlags()
    {
        _needsLayout   = false;
        _needsPainting = false;
        _isDirty       = false;
        Plan.IsDirty   = false;
        Style.IsDirty  = false;
        foreach (var child in Children)
            child.CleanFlags();
    }

    //**********************************************

    public Control MarkDirty()
    {
        if (!IsDirty)
        {
            IsDirty = true;
            Parent?.MarkDirty();
        }

        return this;
    }

    /// <summary>
    ///     Propagate clearing of the flag to all child elements
    /// </summary>
    public void ClearDirtyFlags()
    {
        _isDirty      = false;
        Plan.IsDirty  = false;
        Style.IsDirty = false;

        foreach (var child in Children)
            child.ClearDirtyFlags();
    }

    protected virtual void RaiseDirtied() { Dirtied?.Invoke(this, EventArgs.Empty); }

    //**********************************************

    public void SetStyle(Style style)
    {
        Style.Clear();
        Style.MergeFrom(DefaultStyle(this));
        Style.Combine(Style, style);
    }

    /// <summary>
    ///     Check to see if the control has a default style attribute and create that type
    /// </summary>
    public static Style DefaultStyle(Control control)
    {
        var           type   = control.GetType();
        ObjectHandle? handle = null;
        try
        {
            handle = Activator.CreateInstance(type.Assembly.FullName ?? "", type.FullName + ".Style");
        }
        catch
        {
            // ignored
        }

        if (handle == null)
        {
            try
            {
                handle = Activator.CreateInstance(type.Assembly.FullName ?? "", type.FullName + "Style");
            }
            catch
            {
                // ignored
            }
        }

        var style = (Style?)handle?.Unwrap();
        return style ?? new Style();
    }

    /// <summary>
    ///     Check to see if the control has a default style attribute and create that type
    /// </summary>
    public static LayoutPlan DefaultLayoutPlan(Control control)
    {
        var           type   = control.GetType();
        ObjectHandle? handle = null;
        try
        {
            handle = Activator.CreateInstance(type.Assembly.FullName ?? "", type.FullName + "LayoutPlan");
        }
        catch
        {
            // ignored
        }

        if (handle == null)
        {
            try
            {
                handle = Activator.CreateInstance(type.Assembly.FullName ?? "", type.FullName + ".LayoutPlan");
            }
            catch
            {
                // ignored
            }
        }

        var plan = (LayoutPlan?)handle?.Unwrap();
        return plan ?? new LayoutPlan();
    }


    internal void SetHost(IApp? app)
    {
        _app = app;
        // Lazy set children hosts
        // foreach (var child in Children)
        //     child.SetHost(value);
    }

    private IApp? GetApp()
    {
        if (_app != null)
            return _app;
        return _app = Parent?.GetApp();
    }

    //**********************************************

    private void SetupPlanChanged()                               { Plan.Changed  += PlanOnChanged; }
    private void SetupStyleChanged()                              { Style.Changed += StyleOnChanged; }
    private void PlanOnChanged(object?  sender, LayoutPlan e)     { MarkDirty(); }
    private void StyleOnChanged(object? sender, Style      style) { NeedsPainting = true; }

    public Control Add(LayoutPlan plan)
    {
        Plan.MergeFrom(plan);
        return this;
    }

    public Control Add(Style style)
    {
        SetStyle(style);
        return this;
    }

    public Control Insert(int index, Control control)
    {
        control.Parent?.Remove(control);
        control.Parent = this;
        control._app = _app;
        _children.Insert(index, control);
        NeedsLayout = true;
        return this;

    }

    public Control Add(Control control)
    {
        control.Parent?.Remove(control);
        control.Parent = this;
        control._app  = _app;
        _children.Add(control);
        NeedsLayout = true;
        return this;
    }

    public Control Add(IEnumerable<Control> controls)
    {
        foreach (var control in controls)
            Add(control);
        return this;
    }

    public Control Remove(Control control)
    {
        if (_children.Contains(control))
        {
            control.Parent = null;
            control._app  = null;
            control.Layout = new();
            _children.Remove(control);
            MarkDirty();
            NeedsLayout = true;
        }

        return this;
    }

    public Control Clear()
    {
        if (_children.Count <= 0)
            return this;

        foreach (var control in _children.ToArray())
            Remove(control);
        NeedsLayout = true;

        return this;
    }

    public void CopyPlan(Control control)
    {
        Plan.MergeFrom(control.Plan);
        MarkDirty();
    }

    public Control SetChildren(IEnumerable<Control> controls)
    {
        Clear();
        Add(controls);
        return this;
    }

    public IEnumerable<T> ChildrenOf<T>() where T : Control { return _children.OfType<T>(); }

    //**********************************************


    internal virtual bool OnKeyEvent(KeyboardEvent kbEvent)
    {
        switch (kbEvent.EventType)
        {
        case KeyboardEventType.Pressed:
            if (OnKeyDown(kbEvent))
                return true;
            break;
        case KeyboardEventType.Released:
            if (OnKeyUp(kbEvent))
                return true;
            break;
        case KeyboardEventType.Key:      
            break;
        case KeyboardEventType.Unknown:
            break;
        default:                         
            throw new ArgumentOutOfRangeException();
        }

        return false;
    }

    /// <summary>
    ///     Override this method to handle accelerator type keys. e.g. Alt-A
    ///     Return true to stop processing key event
    /// </summary>
    internal virtual bool OnPreProcessKey(KeyboardEvent kbEvent) => false;

    /// <summary>
    ///     Override this method to handle keys not handled by other controls.e.g. Enter, Tab
    ///     Return true to stop processing key event
    /// </summary>
    internal virtual bool OnPostProcessKey(KeyboardEvent kbEvent) => false;

    public virtual bool OnKeyDown(KeyboardEvent kbEvent) => false;
    public virtual bool OnKeyUp(KeyboardEvent   kbEvent) => false;

    public virtual void OnPointerLeave(PointerEvent           pointerEvent) { PointerLeave?.Invoke(this, pointerEvent); }
    public virtual void OnPointerEnter(PointerEvent           pointerEvent) { PointerEnter?.Invoke(this, pointerEvent); }
    public virtual void OnPointerMove(PointerEvent            pointerEvent) { PointerMove?.Invoke(this, pointerEvent); }
    public virtual void OnPointerPressed(PointerEvent         pointerEvent) { PointerPressed?.Invoke(this, pointerEvent); }
    public virtual void OnPointerReleased(PointerEvent        pointerEvent) { PointerReleased?.Invoke(this, pointerEvent); }
    public virtual void OnPointerClick(PointerEvent           pointerEvent) { Clicked?.Invoke(this, pointerEvent); }
    public virtual void OnPointerDoubleClick(PointerEvent     pointerEvent) { DoubleClicked?.Invoke(this, pointerEvent); }
    public virtual void OnPointerWheel(PointerEvent           pointerEvent) { PointerWheel?.Invoke(this, pointerEvent); }
    public virtual void OnPointerHorizontalWheel(PointerEvent pointerEvent) { PointerHorzWheel?.Invoke(this, pointerEvent); }

    public event EventHandler<PointerEvent>? PointerLeave;
    public event EventHandler<PointerEvent>? PointerEnter;
    public event EventHandler<PointerEvent>? PointerMove;
    public event EventHandler<PointerEvent>? PointerWheel;
    public event EventHandler<PointerEvent>? PointerHorzWheel;
    public event EventHandler<PointerEvent>? PointerPressed;
    public event EventHandler<PointerEvent>? PointerReleased;
    public event EventHandler<PointerEvent>? Clicked;
    public event EventHandler<PointerEvent>? DoubleClicked;

    //**********************************************

    public override string ToString() { return ToString(new StringBuilder(), true, false); }

    public string ToString(StringBuilder sb, bool plan, bool layout)
    {
        sb.Append($"{GetType().Name}{(IsDirty ? " *" : "")}");
        if (!string.IsNullOrEmpty(Name)) sb.Append($" Name[{Name}]");
        if (plan)
            sb.Append($" {nameof(Plan)}[{Plan}]");
        if (layout)
            sb.Append($" {nameof(Layout)}[{Layout}]");
        AppendProperties(sb);

        if (_children.Count > 0)
        {
            sb.AppendLine();
            AppendChildren(sb, plan, layout);
        }

        return sb.ToString();
    }

    protected virtual void AppendProperties(StringBuilder sb)
    {
        if (HasFocus) sb.Append($" {nameof(HasFocus)}={HasFocus}");
        if (!IsEnabled) sb.Append($" {nameof(IsEnabled)}={IsEnabled}");
        if (ForegroundBrush != null) sb.Append($" {nameof(ForegroundBrush)}={ForegroundBrush}");
        if (BackgroundBrush != null) sb.Append($" {nameof(BackgroundBrush)}={BackgroundBrush}");
    }

    protected void AppendChildren(StringBuilder sb, bool plan, bool layout)
    {
        foreach (var child in Children)
        {
            child.ToString(sb, plan, layout);
            sb.AppendLine();
            //child.AppendChildren(sb, plan, layout);
        }
    }

    public virtual void OnUpdate(float elapsedTimeTotalSeconds)
    {
        Update?.Invoke(this, elapsedTimeTotalSeconds);

        foreach (var child in _children.ToArray())
            child.OnUpdate(elapsedTimeTotalSeconds);
    }

    public virtual void OnDraw(Canvas canvas, RectF? bounds)
    {
        foreach (var child in _children.ToArray())
            child.OnDraw(canvas, bounds);

        Draw?.Invoke(this, canvas);
    }

    internal void DrawChildren(Canvas canvas, RectF? bounds)
    {
        foreach (var child in _children.ToArray())
            child.OnDraw(canvas, bounds);
    }

    public void RaiseDraw(Canvas canvas)
    {
        Draw?.Invoke(this, canvas);
    }
}

public enum TraverseStrategy
{
    ParentThenChildren,
    ChildrenThenParent,
    OnlyChildren
}
