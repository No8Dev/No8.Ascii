
using System.ComponentModel;

using static Asciis.UI.NumberExtensions;

namespace Asciis.UI;

/// <summary>
///     Calculated Layout
/// </summary>
public class LayoutActual
{
    public float Width  { get; set; } = float.NaN;
    public float Height { get; set; } = float.NaN;

    private VecF? _displayLocation;
    public VecF DisplayLocation
    {
        get => _displayLocation ?? throw new WarningException("Layout not calculated");
        internal set => _displayLocation = value;
    }

    public RectF DisplayRect => new RectF(_displayLocation?.X ?? 0, _displayLocation?.Y ?? 0, Width, Height);

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
    public readonly Edges Margin  = new();
    public readonly Edges Border  = new();
    public readonly Edges Padding = new();

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

    public readonly Edges Position = Edges.Create(0);
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

    public LayoutActual() { CachedMeasurements.Fill((_) => new CachedMeasurement()); }

    public LayoutActual(LayoutActual other)
    {
        Position = Edges.Create(other.Position);
        Width = other.Width;
        Height = other.Height;
        Border = Edges.Create(other.Border);
        Padding = Edges.Create(other.Padding);

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
    protected bool Equals(LayoutActual other)
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
                   && FloatsEqual(ComputedMainLength, other.ComputedMainLength)
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
        hashCode.Add(ComputedMainLength);
        hashCode.Add((int)LastContainerUIDirection);
        hashCode.Add((int)UIDirection);
        hashCode.Add(HadOverflow);
        hashCode.Add(GenerationCount);
        hashCode.Add(ComputedMainLengthGeneration);
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
}
