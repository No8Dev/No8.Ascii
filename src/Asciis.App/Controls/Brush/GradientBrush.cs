namespace Asciis.App.Controls;

public sealed class GradientBrush : Brush
{
    public GradientStopList GradientStops { get; init; } = new();

    public GradientDirection Direction { get; init; }

    public GradientBrush() { }

    public GradientBrush(GradientStopList list, GradientDirection direction = GradientDirection.Horizontal)
    {
        Direction     = direction;
        GradientStops = list;
    }

    public GradientBrush(Color start, Color end, GradientDirection direction = GradientDirection.Horizontal)
    {
        Direction = direction;
        GradientStops.Add(new(start, 0f));
        GradientStops.Add(new(end, 1f));
    }

    /// <summary>
    ///     Color blended between gradient stops at the specified offset.
    ///     Offset must be between 0.0 and 1.0
    /// </summary>
    public override Color GetColorAt(float offset)
    {
        offset = Math.Clamp(offset, 0.0f, 1.0f);

        Color? beforeColor = null;
        Color? afterColor  = null;
        float  before      = 0f, after = 1f;

        foreach (var stop in GradientStops.OrderBy(s => s.Offset))
        {
            if (stop.Offset.Is(offset))
                return stop.Color;

            if (stop.Offset > offset)
            {
                afterColor = stop.Color;
                after      = stop.Offset;
                break;
            }
             
            beforeColor = stop.Color;
            before      = stop.Offset;
        }

        if (beforeColor == null)
        {
            beforeColor = Color.Transparent;
            before      = 0f;
        }

        if (afterColor == null)
        {
            afterColor = Color.Transparent;
            after      = 1f;
        }

        if (beforeColor == afterColor ||
            before.Is(after))
            return afterColor.Value;

        var range = after - before;
        offset -= before;
        var percentage = range * offset;
        percentage = Math.Clamp(percentage, 0f, 1f);

        return beforeColor.Value.BlendRGB(afterColor.Value, percentage);
    }
}

public enum GradientDirection
{
    Horizontal,
    Vertical
}
