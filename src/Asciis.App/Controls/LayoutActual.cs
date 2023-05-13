
using System.ComponentModel;

namespace Asciis.App.Controls;

using static NumberExtensions;

/// <summary>
///     Calculated Layout
/// </summary>
public class LayoutActual
{
    public float Width  { get; set; } = float.NaN;
    public float Height { get; set; } = float.NaN;

    private RectF? _bounds;

    //public RectF DisplayRect => new RectF(_bounds?.X ?? 0, _bounds?.Y ?? 0, Width, Height);

    public float Left =>
        Position.Left;

    public float Top =>
        Position.Top;

    public float Right
    {
        get
        {
            if (Position.Right.IsZero() && Width.IsNotZero())
                Position.Right = Position.Left + Width - 1;
            return Position.Right;
        }
    }

    public float Bottom
    {
        get
        {
            if (Position.Bottom.IsZero() && Height.IsNotZero())
                Position.Bottom = Position.Top + Height - 1;
            return Position.Bottom;
        }
    }

    public bool HasLayout => _bounds != null;

    /// <summary>
    ///     The location and size of the control.  Does not include any translations
    /// </summary>
    public RectF Bounds
    {
        get => _bounds ?? throw new WarningException("Layout not calculated");
        internal set => _bounds = value;
    }

    /// <summary>
    ///     The location and size of the contents of a control.  Does not include and translations
    /// </summary>
    public virtual RectF ContentBounds =>
        new(
            Bounds.X + Padding.Left,
            Bounds.Y + Padding.Top,
            Width - (Padding.Left + Padding.Right),
            Height - (Padding.Top + Padding.Bottom)
        );


    // The margin is outside the control
    public readonly Edges Margin  = new();
    public readonly Edges Padding = new();

    public float this[Side side]
    {
        get => Position[side];
        set => Position[side] = value;
    }

    public float ComputedLayoutDirectionLength { get; set; } = float.NaN;

    public bool HadOverflow { get; internal set; }

    public int LineIndex { get; set; }

    // INTERNAL VARIABLES **************************************

    // Instead of recomputing the entire layout every single time, we cache some
    // information to break early when nothing changed
    internal int GenerationCount { get; set; }

    public readonly Edges Position = Edges.Create(0);

    internal int ComputedLayoutDirectionLengthGeneration { get; set; }
    internal int NextCachedMeasurementsIndex { get; set; }
    internal readonly CachedMeasurement[] CachedMeasurements = new CachedMeasurement[CachedMeasurement.MaxCachedResults];
    internal CachedMeasurement CachedLayout { get; } = new CachedMeasurement();
    internal readonly DimensionsFloat MeasuredDimensions = new DimensionsFloat(float.NaN);
    internal Number ResolvedWidth { get; set; } = Number.Undefined;
    internal Number ResolvedHeight { get; set; } = Number.Undefined;

    internal Number GetResolvedDimension(Dimension dimension)
    {
        switch (dimension)
        {
            case Dimension.Width: return ResolvedWidth;
            case Dimension.Height: return ResolvedHeight;
            default:
                return Number.Undefined;
        }
    }

    //**********************************************************

    public LayoutActual() { CachedMeasurements.Fill((_) => new CachedMeasurement()); }

    public LayoutActual(LayoutActual other)
    {
        Position = Edges.Create(other.Position);
        Width = other.Width;
        Height = other.Height;
        Padding = Edges.Create(other.Padding);

        ComputedLayoutDirectionLengthGeneration = other.ComputedLayoutDirectionLengthGeneration;
        ComputedLayoutDirectionLength = other.ComputedLayoutDirectionLength;

        GenerationCount = other.GenerationCount;

        NextCachedMeasurementsIndex = other.NextCachedMeasurementsIndex;
        Array.Copy(other.CachedMeasurements, CachedMeasurements, CachedMeasurements.Length);

        MeasuredDimensions = new DimensionsFloat(other.MeasuredDimensions);
        CachedLayout = new CachedMeasurement(other.CachedLayout);
        HadOverflow = other.HadOverflow;
        ResolvedWidth = other.ResolvedWidth;
        ResolvedHeight = other.ResolvedHeight;
    }
    protected bool Equals(LayoutActual other)
    {
        var isEqual = Position == other.Position
                   && FloatsEqual(Width, other.Width)
                   && FloatsEqual(Height, other.Height)
                   && Padding == other.Padding
                   && HadOverflow == other.HadOverflow
                   && NextCachedMeasurementsIndex == other.NextCachedMeasurementsIndex
                   && CachedLayout == other.CachedLayout
                   && FloatsEqual(ComputedLayoutDirectionLength, other.ComputedLayoutDirectionLength)
                   && ResolvedWidth == other.ResolvedWidth
                   && ResolvedHeight == other.ResolvedHeight;

        for (var i = 0; i < CachedMeasurements.Length && isEqual; ++i)
        {
            isEqual = CachedMeasurements[i] == other.CachedMeasurements[i];
        }

        if (MeasuredDimensions.Width.HasValue() || other.MeasuredDimensions.Width.HasValue())
            isEqual = isEqual && FloatsEqual(MeasuredDimensions.Width, other.MeasuredDimensions.Width);

        if (MeasuredDimensions.Height.HasValue() || other.MeasuredDimensions.Height.HasValue())
            isEqual = isEqual && FloatsEqual(MeasuredDimensions.Height, other.MeasuredDimensions.Height);

        return isEqual;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Padding);
        hashCode.Add(Position);
        hashCode.Add(CachedMeasurements);
        hashCode.Add(MeasuredDimensions);
        hashCode.Add(Width);
        hashCode.Add(Height);
        hashCode.Add(ComputedLayoutDirectionLength);
        hashCode.Add(HadOverflow);
        hashCode.Add(GenerationCount);
        hashCode.Add(ComputedLayoutDirectionLengthGeneration);
        hashCode.Add(NextCachedMeasurementsIndex);
        hashCode.Add(CachedLayout);
        return hashCode.ToHashCode();

    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((LayoutActual)obj);
    }

    public static bool operator ==(LayoutActual? left, LayoutActual? right) { return Equals(left, right); }
    public static bool operator !=(LayoutActual? left, LayoutActual? right) { return !Equals(left, right); }

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (_bounds != null) 
            sb.Append($" {nameof(Bounds)}={Bounds}");
        if (_bounds == null || 
            (_bounds != null && !ContentBounds.Equals(_bounds)))
            sb.Append($" {nameof(ContentBounds)}={ContentBounds}");
        if (Margin.IsNotZero) sb.Append($" {nameof(Margin)}={Margin}");
        if (Padding.IsNotZero) sb.Append($" {nameof(Padding)}={Padding}");
        if (HadOverflow) sb.Append($" {nameof(HadOverflow)}={HadOverflow}");
        if (LineIndex != 0) sb.Append($" {nameof(LineIndex)}={LineIndex}");

        return sb.ToString();
    }
}
