namespace Asciis.App;

using static NumberExtensions;

internal class CachedMeasurement
{
    // This value was chosen based on empirical data:
    // 98% of analyzed layouts require less than 8 entries.
    public const int MaxCachedResults = 8;

    public float AvailableWidth { get; set; }
    public float AvailableHeight { get; set; }
    public MeasureMode WidthMeasureMode { get; set; }
    public MeasureMode HeightMeasureMode { get; set; }
    public float ComputedWidth { get; set; }
    public float ComputedHeight { get; set; }

    public CachedMeasurement()
    {
        AvailableWidth = 0;
        AvailableHeight = 0;
        WidthMeasureMode = MeasureMode.Undefined;
        HeightMeasureMode = MeasureMode.Undefined;
        ComputedWidth = -1;
        ComputedHeight = -1;
    }

    public CachedMeasurement(CachedMeasurement other)
    {
        AvailableWidth = other.AvailableWidth;
        AvailableHeight = other.AvailableHeight;
        WidthMeasureMode = other.WidthMeasureMode;
        HeightMeasureMode = other.HeightMeasureMode;
        ComputedWidth = other.ComputedWidth;
        ComputedHeight = other.ComputedHeight;
    }

    protected bool Equals(CachedMeasurement other)
    {
        bool isEqual = WidthMeasureMode == other.WidthMeasureMode &&
            HeightMeasureMode == other.HeightMeasureMode;

        if (AvailableWidth.HasValue() || other.AvailableWidth.HasValue())
        {
            isEqual = isEqual && FloatsEqual(AvailableWidth, other.AvailableWidth);
        }

        if (AvailableHeight.HasValue() || other.AvailableHeight.HasValue())
        {
            isEqual = isEqual && FloatsEqual(AvailableHeight, other.AvailableHeight);
        }

        if (ComputedWidth.HasValue() || other.ComputedWidth.HasValue())
        {
            isEqual = isEqual && FloatsEqual(ComputedWidth, other.ComputedWidth);
        }

        if (ComputedHeight.HasValue() || other.ComputedHeight.HasValue())
        {
            isEqual = isEqual && FloatsEqual(ComputedHeight, other.ComputedHeight);
        }

        return isEqual;
    }
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CachedMeasurement)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(AvailableWidth, AvailableHeight, (int)WidthMeasureMode, (int)HeightMeasureMode, ComputedWidth, ComputedHeight);
    }

    public static bool operator ==(CachedMeasurement? left, CachedMeasurement? right) { return Equals(left, right); }
    public static bool operator !=(CachedMeasurement? left, CachedMeasurement? right) { return !Equals(left, right); }

    
}
