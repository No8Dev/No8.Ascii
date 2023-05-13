using System.Collections;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using Asciis.UI.Engine;

namespace Asciis.UI.Controls;

/// <summary>
/// Base control containing all basic properties used to define the control
/// </summary>
public abstract class Control : IEnumerable<Control>, INode
{
    private          Style?          _style;
    private readonly List<Control>   _children = new();
    private          bool            _hasFocus;
    private          bool            _isEnabled     = true;
    private          bool            _needsLayout   = true;
    private          bool            _needsPainting = true;
    protected        bool            _isMouseOver   = false;
    private          float?          _minWidth;
    private          float?          _maxWidth;
    private          float?          _minHeight;
    private          float?          _maxHeight;
    private          Edges?          _padding;
    private          LayoutPosition? _horzPosition;
    private          LayoutPosition? _vertPosition;
    private          Color?          _foreground;
    private          Color?          _background;
    private          string?         _styleKey;
    private          Screen?         _host;
    private          bool            _isDirty;


    public Control?     Parent      { get; internal set; }
    public object?      Context     { get; set; }
    public MeasureFunc? MeasureNode { get; set; }

    public Screen? Host
    {
        get => _host;
        internal set => SetHost(value);
    }

    public string? Name { get; set; }

    public float MinWidth
    {
        get => _minWidth ?? float.NaN;
        set => ChangeDirtiesLayout(ref _minWidth, value);
    }

    public float MaxWidth
    {
        get => _maxWidth ?? float.NaN;
        set => ChangeDirtiesLayout(ref _maxWidth, value);
    }

    public float MinHeight
    {
        get => _minHeight ?? float.NaN;
        set => ChangeDirtiesLayout(ref _minHeight, value);
    }

    public float MaxHeight
    {
        get => _maxHeight ?? float.NaN;
        set => ChangeDirtiesLayout(ref _maxHeight, value);
    }

    public float Width
    {
        set => MinWidth = MaxWidth = value;
    }

    public float Height
    {
        set => MinHeight = MaxHeight = value;
    }

    public Edges? Padding
    {
        get => _padding;
        set => ChangeDirtiesLayout(ref _padding, value);
    }

    public LayoutPosition? HorzPosition
    {
        get => _horzPosition;
        set => ChangeDirtiesLayout(ref _horzPosition, value);
    }

    public LayoutPosition? VertPosition
    {
        get => _vertPosition;
        set => ChangeDirtiesLayout(ref _vertPosition, value);
    }

    public Color? Foreground
    {
        get => _foreground;
        set => ChangeDirtiesPainting(ref _foreground, value);
    }

    public Color? Background
    {
        get => _background;
        set => ChangeDirtiesPainting(ref _background, value);
    }

    public bool CanFocus { get; protected set; }

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

    private LayoutPlan?   _plan   = new();
    private LayoutActual? _layout = new();

    public LayoutPlan Plan
    {
        get => _plan ??= new LayoutPlan().OnChanged(PlanOnChanged);
        init
        {
            if (_plan != null)
            {
                _plan.CopyFrom(value);
            }
            else
            {
                _plan         =  new LayoutPlan(value);
                _plan.Changed += PlanOnChanged;
            }
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

    protected void ChangeDirtiesLayout<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            RaisePropertyChanging(name);
            var old = field;
            field = value;
            SetNeedsLayout();
            SetNeedsPainting();
            RaisePropertyChanged(name, old, value);
        }
    }

    protected void ChangeDirtiesPainting<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            RaisePropertyChanging(name);
            var old = field;
            field = value;
            SetNeedsPainting();
            RaisePropertyChanged(name, old, value);
        }
    }

    public delegate void PropertyChangingDelegate();

    public delegate void PropertyChangedDelegate<in T>(T oldValue, T newValue);

    private readonly Dictionary<string, PropertyChangingDelegate> _propertyChangingActions = new();
    private readonly Dictionary<string, Delegate>                 _propertyChangedActions  = new();

    /// <summary>
    /// Register action to be called when property about to change
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
    /// Register action to be called when property has changed
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

    public void SetNeedsLayout()
    {
        MarkDirty();
        if (_needsLayout)
            return;

        _needsLayout = true;
        if (Host != null)
            Host.NeedsLayout = true;
    }

    // Painting bubbles down
    public void SetNeedsPainting()
    {
        if (_needsPainting)
            return;

        _needsPainting = true;
        if (Host != null)
            Host.NeedsPainting = true;
    }

    public void Clean()
    {
        _needsLayout   = false;
        _needsPainting = false;
        foreach (var child in Children)
            child.Clean();
    }
    
    //**********************************************

    public event EventHandler? Dirtied;
    
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
                    Plan.IsDirty = false;
            }
        }
    }

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
    /// Propagate clearing of the flag to all child elements
    /// </summary>
    public void ClearDirtyFlag()
    {
        _isDirty      = false;
        Plan.IsDirty = false;
        foreach (var child in Children)
            child.ClearDirtyFlag();
    }
    
    protected virtual void RaiseDirtied() => Dirtied?.Invoke(this, EventArgs.Empty);

    //**********************************************
    

    /// <summary>
    ///     How the control will be styled and drawn
    /// </summary>
    public Style Style
    {
        get => _style ??= DefaultStyle(this);
        set => ChangeDirtiesLayout(ref _style, value);
    }

    public string? StyleKey
    {
        get => _styleKey;
        set => ChangeDirtiesLayout(ref _styleKey, value);
    }

    /// <summary>
    /// Check to see if the control has a default style attribute and create that type
    /// </summary>
    public static Style DefaultStyle(Control control)
    {
        Type type      = control.GetType();
        var  className = type.FullName + "Style";
        var  handle    = Activator.CreateInstance(type.Assembly.FullName ?? "", className);
        var  style     = (Style?)handle?.Unwrap();
        return style ?? new Style();
    }

    private void SetHost(Screen? value)
    {
        _host = value;
        foreach (var child in Children)
            child.SetHost(value);
    }

    //**********************************************
    protected Control(string? name = null)
    {
        Name = name;
    }
    
    protected Control(Control other)
    {
        Plan = new LayoutPlan();
        Plan.CopyFrom(other.Plan);

        Parent = other.Parent;
        Context   = other.Context;

        foreach (var element in other.Children.ToArray()) 
            Add(element);

        _isDirty = Plan.IsDirty;
        SetupPlanChanged();
    }

    protected Control(LayoutPlan plan)
    {
        Plan.CopyFrom(plan);
        _isDirty = Plan.IsDirty;
        SetupPlanChanged();
    }
    protected Control(string? name = null, LayoutPlan? plan = null)
    {
        Plan.CopyFrom(plan);
        Name = name;
        _isDirty = Plan.IsDirty;
        SetupPlanChanged();
    }
    protected Control(out Control node,
                LayoutPlan?       plan = null,
                string?           name = null)
        : this(name, plan)
    {
        node = this;
    }

    protected Control(out Control control, string? name = null)
        : this(name)
    {
        control = this;
    }
    
    //**********************************************

    private void SetupPlanChanged()                          { Plan.Changed += PlanOnChanged; }
    private void PlanOnChanged(object? sender, LayoutPlan e) { MarkDirty(); }

    
    public Control Add(Style style)
    {
        Style = style;
        return this;
    }

    public Control Add(Control control)
    {
        control.Parent?.Remove(control);
        control.Parent = this;
        control.Host   = Host;
        _children.Add(control);
        SetNeedsLayout();
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
            control.Host   = null;
            control.Layout = new();
            _children.Remove(control);
            SetNeedsLayout();
        }

        return this;
    }

    public Control Clear()
    {
        if (_children.Count <= 0)
            return this;

        foreach (var control in _children.ToArray())
            Remove(control);
        SetNeedsLayout();

        return this;
    }

    public void CopyPlan(Control control)
    {
        Plan.CopyFrom(control.Plan);
        MarkDirty();
    }

    public Control SetChildren(IEnumerable<Control> controls)
    {
        Clear();
        Add(controls);
        return this;
    }

    public IReadOnlyList<Control> Children => _children.AsReadOnly();

    public IEnumerable<T> ChildrenOf<T>() where T : Control => Enumerable.OfType<T>(_children.AsReadOnly());

    public IEnumerator<Control> GetEnumerator() => _children.AsReadOnly().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //**********************************************

    public void Traverse(Action<Control> action)
    {
        action(this);
        foreach (var element in _children.ToArray())
            element.Traverse(action);
    }

    //**********************************************

    public virtual bool HandleKeyEvent(AKeyEventArgs keyArgs) => keyArgs.Handled;

    /// <summary>
    /// Override this method to handle accelerator type keys. e.g. Alt-A
    /// Return true to stop processing key event
    /// </summary>
    public virtual bool PreProcessKey(AKeyEventArgs keyArgs) => keyArgs.Handled;

    /// <summary>
    /// Override this method to handle keys not handled by other controls.e.g. Enter, Tab
    /// Return true to stop processing key event
    /// </summary>
    public virtual bool PostProcessKey(AKeyEventArgs keyArgs) => keyArgs.Handled;

    public virtual bool HandlePointerEvent(PointerEventArgs pointerEvent) => pointerEvent.Handled;

    public override string ToString()
    {
        var sb = new StringBuilder();
        AppendProperties(sb);
        if (_children.Count > 0)
        {
            sb.AppendLine();
            AppendChildren(sb);
        }

        return sb.ToString();
    }

    public virtual string ShortString =>
        Name != null
            ? $"{GetType().Name} [{Name}]>"
            : $"{GetType().Name}>";

    protected virtual void AppendProperties(StringBuilder sb)
    {
        sb.Append($"{GetType().Name}");
        if (Name != null) sb.Append($" Name={Name}");
        if (!MinWidth.IsUndefined()) sb.Append($" MinWidth={MinWidth}");
        if (!MaxWidth.IsUndefined()) sb.Append($" MaxWidth={MaxWidth}");
        if (!MinHeight.IsUndefined()) sb.Append($" MinWidth={MinHeight}");
        if (!MaxHeight.IsUndefined()) sb.Append($" MaxWidth={MaxHeight}");
        if (Padding != null) sb.Append($" Padding={Padding}");
        if (HorzPosition != null) sb.Append($" HorzPosition={HorzPosition}");
        if (VertPosition != null) sb.Append($" VertPosition={VertPosition}");
        if (Foreground != null) sb.Append($" Foreground={Foreground}");
        if (Background != null) sb.Append($" Background={Background}");
    }

    protected void AppendChildren(StringBuilder sb)
    {
        foreach (var child in Children)
        {
            child.AppendProperties(sb);
            sb.AppendLine();
            child.AppendChildren(sb);
        }
    }
}