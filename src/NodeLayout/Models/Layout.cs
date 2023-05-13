using System;

using static NodeLayout.NumberExtensions;

namespace NodeLayout;

/// <summary>
///     Calculated Layout
/// </summary>
public class Layout
{
    public float Width { get; set; } = float.NaN;
    public float Height { get; set; } = float.NaN;

    private VecF _displayLocation = null;
    public VecF DisplayLocation
    {
        get => _displayLocation;
        internal set => _displayLocation = value;
    }

    public RectF DisplayRect => new RectF(_displayLocation?.X ?? 0f, _displayLocation?.Y ?? 0f, Width, Height);

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

    // The margin is outside the control
    public readonly LTRBEdge Margin = new LTRBEdge(0f);
    public readonly LTRBEdge Border = new LTRBEdge(0f);
    public readonly LTRBEdge Padding = new LTRBEdge(0f);

    public float this[Side side]
    {
        get => Position[side];
        set => Position[side] = value;
    }

    public float ComputedMainLength { get; set; } = float.NaN;

    public UIDirection LastContainerUIDirection { get; set; } = UIDirection.Unknown;

    public UIDirection UIDirection { get; internal set; }

    public bool HadOverflow { get; internal set; }

    public int LineIndex { get; set; }

    // INTERNAL VARIABLES **************************************

    // Instead of recomputing the entire layout every single time, we cache some
    // information to break early when nothing changed
    internal int GenerationCount { get; set; }

    public readonly LTRBEdge Position = new LTRBEdge(0f);
    internal int ComputedMainLengthGeneration { get; set; }
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

    public Layout() { CachedMeasurements.Fill(() => new CachedMeasurement()); }

    public Layout(Layout other)
    {
        Position = new LTRBEdge(other.Position);
        Width = other.Width;
        Height = other.Height;
        Border = new LTRBEdge(other.Border);
        Padding = new LTRBEdge(other.Padding);

        ComputedMainLengthGeneration = other.ComputedMainLengthGeneration;
        ComputedMainLength = other.ComputedMainLength;

        GenerationCount = other.GenerationCount;

        LastContainerUIDirection = other.LastContainerUIDirection;

        NextCachedMeasurementsIndex = other.NextCachedMeasurementsIndex;
        Array.Copy(other.CachedMeasurements, CachedMeasurements, CachedMeasurements.Length);

        MeasuredDimensions = new DimensionsFloat(other.MeasuredDimensions);
        CachedLayout = new CachedMeasurement(other.CachedLayout);
        UIDirection = other.UIDirection;
        HadOverflow = other.HadOverflow;
        ResolvedWidth = other.ResolvedWidth;
        ResolvedHeight = other.ResolvedHeight;
    }

    protected bool Equals(Layout other)
    {
        var isEqual = Position == other.Position
                   && FloatsEqual(Width, other.Width)
                   && FloatsEqual(Height, other.Height)
                   && Border == other.Border
                   && Padding == other.Padding
                   && UIDirection == other.UIDirection
                   && HadOverflow == other.HadOverflow
                   && LastContainerUIDirection == other.LastContainerUIDirection
                   && NextCachedMeasurementsIndex == other.NextCachedMeasurementsIndex
                   && CachedLayout == other.CachedLayout
                   && ComputedMainLength == other.ComputedMainLength
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
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((Layout)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Position != null
                               ? Position.GetHashCode()
                               : 0;
            hashCode = (hashCode * 397) ^ Width.GetHashCode();
            hashCode = (hashCode * 397) ^ Height.GetHashCode();
            hashCode = (hashCode * 397) ^ (Border != null ? Border.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Padding != null ? Padding.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ ComputedMainLengthGeneration;
            hashCode = (hashCode * 397) ^ ComputedMainLength.GetHashCode();
            hashCode = (hashCode * 397) ^ GenerationCount;
            hashCode = (hashCode * 397) ^ (int)LastContainerUIDirection;
            hashCode = (hashCode * 397) ^ NextCachedMeasurementsIndex;
            hashCode = (hashCode * 397) ^ (CachedMeasurements != null ? CachedMeasurements.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (MeasuredDimensions != null ? MeasuredDimensions.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (CachedLayout != null ? CachedLayout.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (int)UIDirection;
            hashCode = (hashCode * 397) ^ HadOverflow.GetHashCode();

            return hashCode;
        }
    }

    public static bool operator ==(Layout left, Layout right) =>
        Equals(left, right);

    public static bool operator !=(Layout left, Layout right) =>
        !Equals(left, right);
}
