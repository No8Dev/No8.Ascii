using System.Diagnostics;

namespace Asciis.App.Controls;

using static NumberExtensions;


/// <summary>
///     LayoutPlan object hold the attributes you would like the element to have after the layout arrangement process.
///     The Layout object holds the results
/// </summary>
public class LayoutPlan
{
    private const LayoutDirection                   DefaultElementsDirection             = LayoutDirection.Vert;
    private const LayoutWrap                        DefaultElementsWrap                  = LayoutWrap.NoWrap;
    private const AlignmentElements_LayoutDirection DefaultAlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Start;
    private const Alignment_Cross                   DefaultAlignElements_Cross           = Alignment_Cross.Start;
    private const AlignmentLine_Cross        DefaultAlign_Cross                   = AlignmentLine_Cross.Stretch;
    private const AlignmentLine_Cross        DefaultAlignSelf_Cross               = AlignmentLine_Cross.Auto;
    private const Overflow                          DefaultOverflow                      = Overflow.Visible;

    public const float DefaultFlexGrow = 0.0f;
    public const float DefaultFlexShrink = 0.0f;
    public const float DefaultFlex = float.NaN;

    public static readonly Number DefaultLayoutDirectionLength = Number.Auto;
    public static readonly Number DefaultWidth = Number.Auto;
    public static readonly Number DefaultHeight = Number.Auto;

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (IsDirty) sb.Append($" *");

        AppendProperty(sb, nameof(ElementsDirection), _layoutDirection, DefaultElementsDirection);
        AppendProperty(sb, nameof(ElementsWrap), _elementsWrap, DefaultElementsWrap);
        AppendProperty(sb, nameof(AlignElements_LayoutDirection), _alignElements_LayoutDirection, DefaultAlignElements_LayoutDirection);
        AppendProperty(sb, nameof(AlignElements_Cross), _alignElements_Cross, DefaultAlignElements_Cross);
        AppendProperty(sb, nameof(Align_Cross), _align_Cross, DefaultAlign_Cross);
        AppendProperty(sb, nameof(AlignSelf_Cross), _alignSelf_Cross, DefaultAlignSelf_Cross);
        AppendProperty(sb, nameof(Overflow), _overflow, DefaultOverflow);

        if (_margin != null && !_margin.IsUndefined) sb.Append($" Margin={Margin}");
        if (_padding != null && !_padding.IsUndefined) sb.Append($" Padding={Padding}");
        if (_position != null && !_position.IsUndefined) sb.Append($" Position={Position}.{PositionType}");

        AppendProperty(sb, nameof(Flex), _flex, DefaultFlex);
        AppendProperty(sb, nameof(FlexGrow), _flexGrow, DefaultFlexGrow);
        AppendProperty(sb, nameof(FlexShrink), _flexShrink, DefaultFlexShrink);

        AppendProperty(sb, nameof(ChildLayoutDirectionLength), _layoutDirectionLength, DefaultLayoutDirectionLength);
        AppendProperty(sb, nameof(Width), _width, DefaultWidth);
        AppendProperty(sb, nameof(Height), _height, DefaultHeight);

        if (!MinWidth.IsUndefined()) sb.Append($" MinWidth={MinWidth}");
        if (!MaxWidth.IsUndefined()) sb.Append($" MaxWidth={MaxWidth}");
        if (!MinHeight.IsUndefined()) sb.Append($" MinWidth={MinHeight}");
        if (!MaxHeight.IsUndefined()) sb.Append($" MaxWidth={MaxHeight}");

        if (!AspectRatio.IsUndefined()) sb.Append($" AspectRatio={AspectRatio}");

        if (IsReferenceBaseline) sb.Append($" {nameof(IsReferenceBaseline)}={IsReferenceBaseline}");
        if (IsText) sb.Append($" {nameof(IsText)}={IsText}");
        if (Atomic) sb.Append($" {nameof(Atomic)}={Atomic}");

        return sb.ToString();
    }

    protected virtual void AppendProperty<T>(StringBuilder sb, string? name, T? value, T defaultValue)
        where T : struct
    {
        if (value != null && !value.Value.Equals(defaultValue))
            sb.Append($" {name}={value}");
    }
    protected virtual void AppendProperty(StringBuilder sb, string? name, float? value, float defaultValue)
    {
        if (value != null && !value.IsUndefined() && !value.Value.Is(defaultValue))
            sb.Append($" {name}={value}");
    }
    protected virtual void AppendProperty(StringBuilder sb, string? name, Number? value, Number defaultValue)
    {
        if (value != null && !value.IsUndefined() && value.Value != defaultValue)
            sb.Append($" {name}={value}");
    }

    //********************************************************************

    /// <summary>
    ///     Called when the Plan first changes
    /// </summary>
    public event EventHandler<LayoutPlan>? Changed;

    //********************************************************************

    /// <summary>
    /// Direction that elements are added to the container. This defines the Layout-Direction as Vert or Horz.
    /// The Cross-Direction would be the other.  Horz or Vert
    /// Vert   Horz
    ///   1    1 2 3
    ///   2    
    ///   3    
    /// </summary>
    /// <remarks>Default value is: Vert</remarks>
    public LayoutDirection ElementsDirection
    {
        get => _layoutDirection ??= DefaultElementsDirection;
        set => CheckChanged(ref _layoutDirection, value);
    }

    /// <summary>
    /// Do you want the elements to wrap when the layout direction (horz/vert) is full
    /// NoWrap:      12345678
    /// Wrap:        1 2 3 4 5
    ///              6 7 8
    /// </summary>
    /// <remarks>Default value is: NoWrap</remarks>
    public LayoutWrap ElementsWrap
    {
        get => _elementsWrap ??= DefaultElementsWrap;
        set => CheckChanged(ref _elementsWrap, value);
    }

    /// <summary>
    /// Determines how the elements are laid out in the Layout Direction
    /// Flex Start:    [1 2 3          ]
    /// Flex End:      [          1 2 3]
    /// Centre:        [     1 2 3     ]
    /// Space Between: [1      2      3]
    /// Space Around:  [  1    2    3  ]
    /// Space Evenly:  [   1   2   3   ]
    /// </summary>
    /// <remarks>Default value is: Start</remarks>
    /// <remarks>A good mental modal for this how a single line of elements are aligned and spaced</remarks>
    public AlignmentElements_LayoutDirection AlignElements_LayoutDirection
    {
        get => _alignElements_LayoutDirection ??= DefaultAlignElements_LayoutDirection;
        set => CheckChanged(ref _alignElements_LayoutDirection, value);
    }

    /// <summary>
    /// Determines how each element is laid out in the Cross Direction within a single line.
    /// [A     D ]  A = Start
    /// [A   C D ]  B = End
    /// [A B C D ]  C = Centre
    /// [  B C D ]  D = Stretch
    /// [  B   D ]
    /// </summary>
    /// <remarks>Default value is: Stretch</remarks>
    public AlignmentLine_Cross Align_Cross
    {
        get => _align_Cross ??= DefaultAlign_Cross;
        set => CheckChanged(ref _align_Cross, value);
    }

    /// <summary>
    /// Align Element Cross is normally assigned by the container (Align_Cross).  AlignSelf_Cross will overwrite the containers wishes.
    /// e.g. The container want all elements aligned to start, but this one element should align center.
    /// </summary>
    /// <remarks>Default value is: Auto</remarks>
    public AlignmentLine_Cross AlignSelf_Cross
    {
        get => _alignSelf_Cross ??= DefaultAlignSelf_Cross;
        set => CheckChanged(ref _alignSelf_Cross, value);
    }

    /// <summary>
    /// Alignment of lines of elements in the Cross-Direction
    ///            Col
    ///  A B C D E F G
    /// [1     D   1   ]  A = Start
    /// [2     D 1     ]  B = End
    /// [3     D     1 ]  C = Centre
    /// [    1 D       ]  D = Stretch
    /// [    2 D 2 2 2 ]  E = Space Around
    /// [    3 D       ]  F = Space Between
    /// [  1   D     3 ]  G = Space Evenly (not implemented)
    /// [  2   D 3     ]
    /// [  3   D   3   ]
    /// </summary>
    /// <remarks>Default value is: Start</remarks>
    public Alignment_Cross AlignElements_Cross
    {
        get => _alignElements_Cross ??= DefaultAlignElements_Cross;
        set => CheckChanged(ref _alignElements_Cross, value);
    }

    /// <summary>
    /// When elements are too big for the container.  Visible, Hidden or Scroll
    /// </summary>
    /// <remarks>Default value is: Visible</remarks>
    public Overflow Overflow
    {
        get => _overflow ??= DefaultOverflow;
        set => CheckChanged(ref _overflow, value);
    }

    //********************************************************************

    /// <summary>
    ///     Element Margin
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

    //********************************************************************

    /// <summary>
    /// Position is set to either Relative, or absolute
    /// </summary>
    /// <remarks>Default value is: Relative</remarks>
    public PositionType PositionType
    {
        get => _positionType ??= PositionType.Relative;
        set => CheckChanged(ref _positionType, value);
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
    /// Grow item to fill the Layout-Direction
    /// </summary>
    /// <remarks>Default value is: 0.0</remarks>
    public float FlexGrow
    {
        get
        {
            if (_flexGrow is null or float.NaN)
                return DefaultFlexGrow;

            return _flexGrow.Value;
        }
        set => CheckChanged(ref _flexGrow, value);
    }

    /// <summary>
    /// Shrink item to fit Layout-Direction
    /// </summary>
    /// <remarks>Default value is: 0.0</remarks>
    public float FlexShrink
    {
        get
        {
            if (_flexShrink is null or float.NaN)
                return DefaultFlexShrink;

            return _flexShrink.Value;
        }
        set => CheckChanged(ref _flexShrink, value);
    }

    //********************************************************************

    /// <summary>
    /// The default size of the element in the Layout-Direction, prior to remaining space being distributed
    /// </summary>
    /// <remarks>Default value is: auto</remarks>
    public Number ChildLayoutDirectionLength
    {
        get => _layoutDirectionLength ??= DefaultLayoutDirectionLength;
        set => CheckChanged(ref _layoutDirectionLength, value);
    }

    /// <summary>
    ///     Requested Width
    /// </summary>
    /// <remarks>Default value is: auto</remarks>
    public Number Width
    {
        get => _width ??= DefaultWidth;
        set => CheckChanged(ref _width, value);
    }

    /// <summary>
    ///     Requested Height
    /// </summary>
    /// <remarks>Default value is: auto</remarks>
    public Number Height
    {
        get => _height ??= DefaultHeight;
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
            case App.Dimension.Width:
                return Width;
            case App.Dimension.Height:
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
            case App.Dimension.Width:
                return MinWidth;
            case App.Dimension.Height:
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
            case App.Dimension.Width:
                return MaxWidth;
            case App.Dimension.Height:
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
        get => _isReferenceBaseline ?? false;
        set => CheckChanged(ref _isReferenceBaseline, value);
    }

    /// <summary>
    /// Is the Node a Text element. Used to calculate baseline
    /// </summary>
    public bool IsText
    {
        get => _isText ?? false;
        set => CheckChanged(ref _isText, value);
    }

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
    ///     A element of a container without sub-elements (equivalent to Display: none)
    ///     Does not calculate elements as part of layout arrangement
    /// </summary>
    public bool Atomic
    {
        get => _atomic ?? false;
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

    private float? _flex;
    private float? _flexGrow;
    private float? _flexShrink;
    private bool _isDirty;
    private bool? _atomic;
    private bool? _isText;
    private bool? _isReferenceBaseline;
    private LayoutDirection? _layoutDirection;
    private LayoutWrap? _elementsWrap;
    private AlignmentElements_LayoutDirection? _alignElements_LayoutDirection;
    private AlignmentLine_Cross? _align_Cross;
    private AlignmentLine_Cross? _alignSelf_Cross;
    private Alignment_Cross? _alignElements_Cross;
    private Number? _layoutDirectionLength;
    private PositionType? _positionType;
    private Overflow? _overflow;
    private Number? _width;
    private Number? _height;
    private Number? _minWidth;
    private Number? _minHeight;
    private Number? _maxWidth;
    private Number? _maxHeight;
    private float? _aspectRatio;
    private Sides? _margin;
    private Sides? _position;
    private Sides? _padding;

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
        MergeFrom(other);
        IsDirty = false;
    }

    /// <summary>
    ///     Copy plan from another plan
    /// </summary>
    public LayoutPlan MergeFrom(LayoutPlan? other)
    {
        if (other == null) return this;
        if (other._flex.HasValue) CheckChanged(ref _flex, other.Flex);
        if (other._flexGrow.HasValue) CheckChanged(ref _flexGrow, other.FlexGrow);
        if (other._flexShrink.HasValue) CheckChanged(ref _flexShrink, other.FlexShrink);
        if (other._isText.HasValue) CheckChanged(ref _isText, other.IsText);
        if (other._atomic.HasValue) CheckChanged(ref _atomic, other.Atomic);
        if (other._isReferenceBaseline.HasValue) CheckChanged(ref _isReferenceBaseline, other.IsReferenceBaseline);

        if (other._layoutDirection.HasValue) CheckChanged(ref _layoutDirection, other._layoutDirection);
        if (other._elementsWrap.HasValue) CheckChanged(ref _elementsWrap, other._elementsWrap);
        if (other._alignElements_LayoutDirection.HasValue) CheckChanged(ref _alignElements_LayoutDirection, other._alignElements_LayoutDirection);
        if (other._alignElements_Cross.HasValue) CheckChanged(ref _alignElements_Cross, other._alignElements_Cross);
        if (other._align_Cross.HasValue) CheckChanged(ref _align_Cross, other._align_Cross);
        if (other._alignSelf_Cross.HasValue) CheckChanged(ref _alignSelf_Cross, other._alignSelf_Cross);
        if (other._layoutDirectionLength != null) CheckChanged(ref _layoutDirectionLength, other._layoutDirectionLength);
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
        if (other.Changed != null) Changed = other.Changed;

        return this;
    }

    protected bool Equals(LayoutPlan other)
    {
        var areNonFloatValuesEqual = ElementsDirection == other.ElementsDirection
                                  && AlignElements_LayoutDirection == other.AlignElements_LayoutDirection
                                  && AlignElements_Cross == other.AlignElements_Cross
                                  && Align_Cross == other.Align_Cross
                                  && AlignSelf_Cross == other.AlignSelf_Cross;
        areNonFloatValuesEqual = areNonFloatValuesEqual
                              && PositionType == other.PositionType
                              && ElementsWrap == other.ElementsWrap
                              && Overflow == other.Overflow
                              && Atomic == other.Atomic;
        areNonFloatValuesEqual = areNonFloatValuesEqual
                                 && ChildLayoutDirectionLength == other.ChildLayoutDirectionLength
                                 && Margin == other.Margin
                                 && Position == other.Position
                                 && Padding == other.Padding;
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
            var hashCode = (int)ElementsDirection;
            hashCode = (hashCode * 397) ^ (int)AlignElements_LayoutDirection;
            hashCode = (hashCode * 397) ^ (int)AlignElements_Cross;
            hashCode = (hashCode * 397) ^ (int)Align_Cross;
            hashCode = (hashCode * 397) ^ (int)AlignSelf_Cross;
            hashCode = (hashCode * 397) ^ (int)PositionType;
            hashCode = (hashCode * 397) ^ (int)ElementsWrap;
            hashCode = (hashCode * 397) ^ (int)Overflow;
            hashCode = (hashCode * 397) ^ Flex.GetHashCode();
            hashCode = (hashCode * 397) ^ FlexGrow.GetHashCode();
            hashCode = (hashCode * 397) ^ FlexShrink.GetHashCode();
            hashCode = (hashCode * 397) ^ ChildLayoutDirectionLength.GetHashCode();
            hashCode = (hashCode * 397) ^ Margin.GetHashCode();
            hashCode = (hashCode * 397) ^ Position.GetHashCode();
            hashCode = (hashCode * 397) ^ Padding.GetHashCode();
            hashCode = (hashCode * 397) ^ Width.GetHashCode();
            hashCode = (hashCode * 397) ^ Height.GetHashCode();
            hashCode = (hashCode * 397) ^ MinWidth.GetHashCode();
            hashCode = (hashCode * 397) ^ MinHeight.GetHashCode();
            hashCode = (hashCode * 397) ^ MaxWidth.GetHashCode();
            hashCode = (hashCode * 397) ^ MaxHeight.GetHashCode();
            hashCode = (hashCode * 397) ^ AspectRatio.GetHashCode();

            return hashCode;
        }
    }

    public static bool operator ==(LayoutPlan? left, LayoutPlan? right) => Equals(left, right);
    public static bool operator !=(LayoutPlan? left, LayoutPlan? right) => !Equals(left, right);

    protected virtual void RaiseChanged(LayoutPlan e) => Changed?.Invoke(this, e);

    public LayoutPlan OnChanged(EventHandler<LayoutPlan>? planOnChanged)
    {
        if (planOnChanged != null)
            Changed += planOnChanged;
        return this;
    }
}
