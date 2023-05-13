using System.Diagnostics;

namespace Asciis.UI;

using static NumberExtensions;


/// <summary>
///     LayoutPlan object hold the attributes you would like the element to have after the layout arrangement process.
///     The Layout object holds the results
/// </summary>
public class LayoutPlan
{
    //********************************************************************

    /// <summary>
    ///     Called when the Plan first changes
    /// </summary>
    public event EventHandler<LayoutPlan>? Changed;

    //********************************************************************

    /// <summary>
    /// Direction that elements are added to the container.
    /// This defines the Main-Direction as Column or Row.
    /// The Cross-Direction would be the other.  Row or Column
    /// Column Column-Reverse Row    Row-Reverse
    ///   1       3           1 2 3    3 2 1
    ///   2       2
    ///   3       1
    /// </summary>
    /// <remarks>Default value is: Column</remarks>
    public LayoutDirection LayoutDirection
    {
        get => _layoutDirection ??= LayoutDirection.Column;
        set => CheckChanged(ref _layoutDirection, value);
    }

    /// <summary>
    /// Do you want the elements to wrap when the direction (row/col) is full
    /// NoWrap:      12345678
    /// Wrap:        1 2 3 4 5
    ///              6 7 8
    /// WrapReverse: 6 7 8
    ///              1 2 3 4 5
    /// </summary>
    /// <remarks>Default value is: NoWrap</remarks>
    public LayoutWrap LayoutWrap
    {
        get => _layoutWrap ??= LayoutWrap.NoWrap;
        set => CheckChanged(ref _layoutWrap, value);
    }

    /// <summary>
    /// Determines how the content is laid out in the Main-Direction
    /// Flex Start:    [1 2 3          ]
    /// Flex End:      [          1 2 3]
    /// Centre:        [     1 2 3     ]
    /// Space Between: [1      2      3]
    /// Space Around:  [  1    2    3  ]
    /// Space Evenly:  [   1   2   3   ]
    /// </summary>
    /// <remarks>Default value is: Start</remarks>
    /// <remarks>A good mental modal for this how a single line of elements are aligned and spaced</remarks>
    public AlignmentMain AlignContentMain
    {
        get => _alignContentMain ??= AlignmentMain.Start;
        set => CheckChanged(ref _alignContentMain, value);
    }

    /// <summary>
    /// Align Content for the Cross-Direction
    /// </summary>
    /// <remarks>Default value is: Start</remarks>
    public AlignmentCross AlignContentCross
    {
        get => _alignContentCross ??= AlignmentCross.Start;
        set => CheckChanged(ref _alignContentCross, value);
    }

    /// <summary>
    /// Determines how the elements are laid out in the Cross-Direction
    /// [A     D ]  A = Flex-Start
    /// [A   C D ]  B = Flex-End
    /// [A B C D ]  C = Centre
    /// [  B C D ]  D = Stretch
    /// [  B   D ]
    /// </summary>
    /// <remarks>Default value is: Stretch</remarks>
    public AlignmentCross AlignElements
    {
        get => _alignElements ??= AlignmentCross.Stretch;
        set => CheckChanged(ref _alignElements, value);
    }

    /// <summary>
    /// Alignment is normally assigned by the container.  AlignSelf will overwrite the parents wishes.
    /// </summary>
    public AlignmentCross AlignSelf
    {
        get => _alignSelf ??= AlignmentCross.Auto;
        set => CheckChanged(ref _alignSelf, value);
    }

    /// <summary>
    /// When content is too big.  Visible, Hidden or Scroll
    /// </summary>
    /// <remarks>Default value is: Visible</remarks>
    public Overflow Overflow
    {
        get => _overflow ??= Overflow.Visible;
        set => CheckChanged(ref _overflow, value);
    }

    //********************************************************************

    /// <summary>
    ///     Node Margin
    /// </summary>
    public Sides Margin
    {
        get => _margin ??= new Sides(Number.Undefined).SetChanged(OnSideChanged);
        set
        {
            if (value != _margin)
            {
                if (_margin != null)
                    _margin.Changed -= OnSideChanged;
                CheckChanged(ref _margin, value);
                if (_margin != null)
                    _margin.Changed += OnSideChanged;
            }
        }
    }

    /// <summary>
    ///     Node border
    /// </summary>
    public Sides Border
    {
        get => _border ??= new Sides(Number.Undefined).SetChanged(OnSideChanged);
        set
        {
            if (value != _border)
            {
                if (_border != null)
                    _border.Changed -= OnSideChanged;
                CheckChanged(ref _border, value);
                if (_border != null)
                    _border.Changed += OnSideChanged;
            }
        }
    }

    /// <summary>
    ///     Node Padding
    /// </summary>
    public Sides Padding
    {
        get => _padding ??= new Sides(Number.Undefined).SetChanged(OnSideChanged);
        set
        {
            if (value != _padding)
            {
                if (_padding != null)
                    _padding.Changed -= OnSideChanged;
                CheckChanged(ref _padding, value);
                if (_padding != null)
                    _padding.Changed += OnSideChanged;
            }
        }
    }

    /// <summary>
    ///     Node Position
    /// </summary>
    public Sides Position
    {
        get => _position ??= new Sides(Number.Undefined).SetChanged(OnSideChanged);
        set
        {
            if (value != _position)
            {
                if (_position != null)
                    _position.Changed -= OnSideChanged;
                CheckChanged(ref _position, value);
                if (_position != null)
                    _position.Changed += OnSideChanged;
            }
        }
    }

    //********************************************************************

    /// <summary>
    ///     Planned Left
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Left
    {
        get => Position[Side.Left];
        set => Position[Side.Left] = value;
    }

    /// <summary>
    ///     Planned Top
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Top
    {
        get => Position[Side.Top];
        set => Position[Side.Top] = value;
    }

    /// <summary>
    ///     Planned Right
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Right
    {
        get => Position[Side.Right];
        set => Position[Side.Right] = value;
    }

    /// <summary>
    ///     Planned Bottom
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Bottom
    {
        get => Position[Side.Bottom];
        set => Position[Side.Bottom] = value;
    }

    /// <summary>
    ///     Planned Start
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Start
    {
        get => Position[Side.Start];
        set => Position[Side.Start] = value;
    }

    /// <summary>
    ///     Planned End
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number End
    {
        get => Position[Side.End];
        set => Position[Side.End] = value;
    }

    /// <summary>
    ///     Planned Horizontal
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Horizontal
    {
        get => Position[Side.Horizontal];
        set => Position[Side.Horizontal] = value;
    }

    /// <summary>
    ///     Planned Vertical
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Vertical
    {
        get => Position[Side.Vertical];
        set => Position[Side.Vertical] = value;
    }

    //********************************************************************

    /// <summary>
    ///     Combination of FlexGrow and FlexShrink
    /// </summary>
    public float Flex
    {
        get => _flex ??= Number.ValueUndefined;
        set => CheckChanged(ref _flex, value);
    }

    /// <summary>
    /// Grow item to fill the Main-Direction
    /// </summary>
    public float FlexGrow
    {
        get
        {
            if (_flexGrow == null || float.IsNaN(_flexGrow.Value))
                return DefaultFlexGrow;

            return _flexGrow.Value;
        }
        set => CheckChanged(ref _flexGrow, value);
    }

    /// <summary>
    /// Shrink item to fit Main-Direction
    /// </summary>
    public float FlexShrink
    {
        get
        {
            if (_flexShrink == null || float.IsNaN(_flexShrink.Value))
                return DefaultFlexShrink;

            return _flexShrink.Value;
        }
        set => CheckChanged(ref _flexShrink, value);
    }

    //********************************************************************

    /// <summary>
    /// The default size of the element in the Main-Direction, prior to remaining space being distributed
    /// </summary>
    public Number MainLength
    {
        get => _mainLength ??= Number.Auto;
        set => CheckChanged(ref _mainLength, value);
    }

    /// <summary>
    ///     Requested Width
    /// </summary>
    public Number Width
    {
        get => _width ??= Number.Auto;
        set => CheckChanged(ref _width, value);
    }

    /// <summary>
    ///     Requested Height
    /// </summary>
    public Number Height
    {
        get => _height ??= Number.Auto;
        set => CheckChanged(ref _height, value);
    }

    /// <summary>
    ///     Minimum Width for node
    /// </summary>
    public Number MinWidth
    {
        get => _minWidth ??= Number.Undefined;
        set => CheckChanged(ref _minWidth, value);
    }

    /// <summary>
    ///     Minimum Height for node
    /// </summary>
    public Number MinHeight
    {
        get => _minHeight ??= Number.Undefined;
        set => CheckChanged(ref _minHeight, value);
    }

    /// <summary>
    ///     Maximum Width for node
    /// </summary>
    public Number MaxWidth
    {
        get => _maxWidth ??= Number.Undefined;
        set => CheckChanged(ref _maxWidth, value);
    }

    /// <summary>
    ///     Maximum Height for node
    /// </summary>
    public Number MaxHeight
    {
        get => _maxHeight ??= Number.Undefined;
        set => CheckChanged(ref _maxHeight, value);
    }

    /// <summary>
    ///     Aspect ratio for node
    /// </summary>
    public float AspectRatio
    {
        get => _aspectRatio ??= float.NaN;
        set => CheckChanged(ref _aspectRatio, value);
    }

    //********************************************************************

    /// <summary>
    ///     Get the Planned width or height
    /// </summary>
    public Number Dimension(Dimension dim)
    {
        switch (dim)
        {
            case UI.Dimension.Width:
                return Width;
            case UI.Dimension.Height:
                return Height;
            default:
                throw new ArgumentException("Unknown dimension", nameof(dim));
        }
    }

    /// <summary>
    ///     Get the minimum Planned width or height
    /// </summary>
    public Number MinDimension(Dimension dim)
    {
        switch (dim)
        {
            case UI.Dimension.Width:
                return MinWidth;
            case UI.Dimension.Height:
                return MinHeight;
            default:
                throw new ArgumentException("Unknown dimension", nameof(dim));
        }
    }

    /// <summary>
    ///     Get the maximum Planned width or height
    /// </summary>
    public Number MaxDimension(Dimension dim)
    {
        switch (dim)
        {
            case UI.Dimension.Width:
                return MaxWidth;
            case UI.Dimension.Height:
                return MaxHeight;
            default:
                throw new ArgumentException("Unknown dimension", nameof(dim));
        }
    }

    //********************************************************************

    /// <summary>
    /// This node is used as the reference baseline
    /// </summary>
    public bool IsReferenceBaseline
    {
        get => _isReferenceBaseline;
        set => CheckChanged(ref _isReferenceBaseline, value);
    }

    /// <summary>
    /// Is the Node a Text element. Used to calculate baseline
    /// </summary>
    public bool IsText
    {
        get => _isText;
        set => CheckChanged(ref _isText, value);
    }

    /// <summary>
    ///     Used to define the baseline for Text
    /// </summary>
    public BaselineFunc? BaselineFunc { get; set; }

    /*
    /// <summary>
    ///     Custom function to measure a node
    /// </summary>
    public MeasureFunc MeasureFunc
    {
        get => _measureFunc;
        set
        {
            _measureFunc = value;
            IsText       = _measureFunc != null;
        }
    }
    */

    /// <summary>
    /// Left to Right or Right to Left. Defaults to inherit so can be taken from container
    /// </summary>
    public UIDirection UIDirection
    {
        get => _uiDirection ??= UIDirection.Inherit;
        set => CheckChanged(ref _uiDirection, value);
    }

    /// <summary>
    /// Position is set to either Relative, or absolute
    /// </summary>
    public PositionType PositionType
    {
        get => _positionType ??= PositionType.Relative;
        set => CheckChanged(ref _positionType, value);
    }

    /// <summary>
    ///     A element of a container without sub-elements (equivalent to Display: none)
    ///     Does not calculate elements as part of layout arrangement
    /// </summary>
    public bool Atomic
    {
        get => _atomic;
        set => CheckChanged(ref _atomic, value);
    }

    //********************************************************************

    /// <summary>
    ///     Has the planned valued changed since the last layout arrangement
    /// </summary>
    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (_isDirty != value)
            {
                _isDirty = value;

                // set dirty flag bubbles up
                // clear dirty flag propagates down
                if (value)
                    RaiseChanged(this);
                else
                {
                    _margin?.ClearDirtyFlag();
                    _border?.ClearDirtyFlag();
                    _padding?.ClearDirtyFlag();
                    _position?.ClearDirtyFlag();
                }
            }
        }
    }

    /// <summary>
    ///     Be notified when planned values change
    /// </summary>
    private void CheckChanged<T>(ref T field, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            IsDirty = true;
        }
    }

    //********************************************************************

    public const float DefaultFlexGrow = 0.0f;
    public const float DefaultFlexShrink = 0.0f;
    public const float DefaultFlex = float.NaN;

    private float?           _flex;
    private float?           _flexGrow;
    private float?           _flexShrink;
    private bool             _isDirty;
    private bool             _atomic;
    private bool             _isText;
    private bool             _isReferenceBaseline;
    private LayoutDirection? _layoutDirection;
    private LayoutWrap?      _layoutWrap;
    private AlignmentMain?   _alignContentMain;
    private AlignmentCross?  _alignElements;
    private AlignmentCross?  _alignSelf;
    private AlignmentCross?  _alignContentCross;
    private Number?          _mainLength;
    private UIDirection?     _uiDirection;
    private PositionType?    _positionType;
    private Overflow?        _overflow;
    private Number?          _width;
    private Number?          _height;
    private Number?          _minWidth;
    private Number?          _minHeight;
    private Number?          _maxWidth;
    private Number?          _maxHeight;
    private float?           _aspectRatio;
    private Sides?           _margin;
    private Sides?           _position;
    private Sides?           _padding;
    private Sides?           _border;

    //********************************************************************

    /// <summary>
    ///     
    /// </summary>
    public LayoutPlan() { _isDirty = false; }

    private void OnSideChanged(object? sender, EventArgs e) { IsDirty = true; }

    /// <summary>
    ///     Clone plan
    /// </summary>
    public LayoutPlan(LayoutPlan other)
    {
        CopyFrom(other);
        IsDirty = false;
    }

    /// <summary>
    ///     Copy plan from another plan
    /// </summary>
    public void CopyFrom(LayoutPlan? other)
    {
        if (other == null) return;
        if (other._flex != null)
            CheckChanged(ref _flex, other._flex);
        if (other._flexGrow != null)
            CheckChanged(ref _flexGrow, other._flexGrow);
        if (other._flexShrink != null)
            CheckChanged(ref _flexShrink, other._flexShrink);

        Atomic = other.Atomic;
        IsText = other.IsText;
        IsReferenceBaseline = other.IsReferenceBaseline;

        if (other._layoutDirection.HasValue) CheckChanged(ref _layoutDirection, other._layoutDirection);
        if (other._layoutWrap.HasValue) CheckChanged(ref _layoutWrap, other._layoutWrap);
        if (other._alignContentMain.HasValue) CheckChanged(ref _alignContentMain, other._alignContentMain);
        if (other._alignContentCross.HasValue) CheckChanged(ref _alignContentCross, other._alignContentCross);
        if (other._alignElements.HasValue) CheckChanged(ref _alignElements, other._alignElements);
        if (other._alignSelf.HasValue) CheckChanged(ref _alignSelf, other._alignSelf);
        if (other._mainLength != null) CheckChanged(ref _mainLength, other._mainLength);
        if (other._uiDirection != null) CheckChanged(ref _uiDirection, other._uiDirection);
        if (other._positionType != null) CheckChanged(ref _positionType, other._positionType);
        if (other._overflow != null) CheckChanged(ref _overflow, other._overflow);
        if (other._width != null) CheckChanged(ref _width, other._width);
        if (other._height != null) CheckChanged(ref _height, other._height);
        if (other._minWidth != null) CheckChanged(ref _minWidth, other._minWidth);
        if (other._maxWidth != null) CheckChanged(ref _maxWidth, other._maxWidth);
        if (other._minHeight != null) CheckChanged(ref _minHeight, other._minHeight);
        if (other._maxHeight != null) CheckChanged(ref _maxHeight, other._maxHeight);
        if (other._aspectRatio != null) CheckChanged(ref _aspectRatio, other._aspectRatio);
        if (other._margin != null) CheckChanged(ref _margin, new Sides(other._margin));
        if (other._position != null) CheckChanged(ref _position, new Sides(other._position));
        if (other._padding != null) CheckChanged(ref _padding, new Sides(other._padding));
        if (other._border != null) CheckChanged(ref _border, new Sides(other._border));
        if (other.BaselineFunc != null) BaselineFunc = other.BaselineFunc;
    }

    protected bool Equals(LayoutPlan other)
    {
        var areNonFloatValuesEqual = UIDirection == other.UIDirection
                                  && LayoutDirection == other.LayoutDirection
                                  && AlignContentMain == other.AlignContentMain
                                  && AlignContentCross == other.AlignContentCross
                                  && AlignElements == other.AlignElements
                                  && AlignSelf == other.AlignSelf;
        areNonFloatValuesEqual = areNonFloatValuesEqual
                              && PositionType == other.PositionType
                              && LayoutWrap == other.LayoutWrap
                              && Overflow == other.Overflow
                              && Atomic == other.Atomic;
        areNonFloatValuesEqual = areNonFloatValuesEqual
                              && MainLength == other.MainLength
                              && Margin == other.Margin
                              && Position == other.Position
                              && Padding == other.Padding
                              && Border == other.Border;
        areNonFloatValuesEqual = areNonFloatValuesEqual
                              && Width == other.Width
                              && Height == other.Height
                              && MinWidth == other.MinWidth
                              && MinHeight == other.MinHeight
                              && MaxWidth == other.MaxWidth
                              && MaxHeight == other.MaxHeight;

        areNonFloatValuesEqual = areNonFloatValuesEqual && Flex.IsUndefined() == other.Flex.IsUndefined();
        if (areNonFloatValuesEqual && Flex.HasValue() && other.Flex.HasValue())
            areNonFloatValuesEqual = FloatsEqual(Flex, other.Flex);

        areNonFloatValuesEqual = areNonFloatValuesEqual && FlexGrow.IsUndefined() == other.FlexGrow.IsUndefined();
        if (areNonFloatValuesEqual && FlexGrow.HasValue())
            areNonFloatValuesEqual = FloatsEqual(FlexGrow, other.FlexGrow);

        areNonFloatValuesEqual = areNonFloatValuesEqual && FlexShrink.IsUndefined() == other.FlexShrink.IsUndefined();
        if (areNonFloatValuesEqual && other.FlexShrink.HasValue())
            areNonFloatValuesEqual = FloatsEqual(FlexShrink, other.FlexShrink);

        if (!(AspectRatio.IsUndefined() && other.AspectRatio.IsUndefined()))
            areNonFloatValuesEqual = areNonFloatValuesEqual && FloatsEqual(AspectRatio, other.AspectRatio);

        return areNonFloatValuesEqual;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((LayoutPlan)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (int)UIDirection;
            hashCode = (hashCode * 397) ^ (int)LayoutDirection;
            hashCode = (hashCode * 397) ^ (int)AlignContentMain;
            hashCode = (hashCode * 397) ^ (int)AlignContentCross;
            hashCode = (hashCode * 397) ^ (int)AlignElements;
            hashCode = (hashCode * 397) ^ (int)AlignSelf;
            hashCode = (hashCode * 397) ^ (int)PositionType;
            hashCode = (hashCode * 397) ^ (int)LayoutWrap;
            hashCode = (hashCode * 397) ^ (int)Overflow;
            hashCode = (hashCode * 397) ^ Flex.GetHashCode();
            hashCode = (hashCode * 397) ^ FlexGrow.GetHashCode();
            hashCode = (hashCode * 397) ^ FlexShrink.GetHashCode();
            hashCode = (hashCode * 397) ^ MainLength?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ Margin.GetHashCode();
            hashCode = (hashCode * 397) ^ Position.GetHashCode();
            hashCode = (hashCode * 397) ^ Padding.GetHashCode();
            hashCode = (hashCode * 397) ^ Border.GetHashCode();
            hashCode = (hashCode * 397) ^ Width?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ Height?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ MinWidth?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ MinHeight?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ MaxWidth?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ MaxHeight?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ AspectRatio.GetHashCode();

            return hashCode;
        }
    }

    public static bool operator ==(LayoutPlan? left, LayoutPlan? right) => Equals(left, right);
    public static bool operator !=(LayoutPlan? left, LayoutPlan? right) => !Equals(left, right);

    protected virtual void RaiseChanged(LayoutPlan e) { Changed?.Invoke(this, e); }

    public LayoutPlan OnChanged(EventHandler<LayoutPlan>? planOnChanged)
    {
        if (planOnChanged != null)
            Changed += planOnChanged;
        return this;
    }
}
