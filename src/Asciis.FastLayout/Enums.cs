namespace Asciis.FastLayout;

public enum Side
{
    Top,
    Bottom,
    Start,
    End,
    Horizontal,
    Vertical,
    All
}

public enum Align
{
    Start, End, Center, Stretch
}

public enum ContentsPlan
{
    FreeForm,
    Horizontal,
    Vertical,
}

/// <summary>
///     Applies to Contents of Horz and Vert children
/// </summary>
public enum DirectionLayout
{
    Flex,
    Scroll,
    Truncate,
    Wrap,
}

public enum Space
{
    None,
    Around,
    Between
}

/// <summary>
///     Used in custom measurement to indicate the measurement mode.
/// </summary>
public enum MeasureMode
{
    Undefined = -1,
    Exactly   = 0,
    AtMost    = 1
}

public enum PositionType
{
    Relative,
    Absolute
}

public enum LayoutDirection
{
    Col,
    Row
}

public enum Dimension
{
    Width,
    Height
}



internal static class NodeArrangeHelpers
{
    public static Dimension ToDimension(this LayoutDirection direction)
    {
        switch (direction)
        {
            case LayoutDirection.Col: return Dimension.Height;
            case LayoutDirection.Row: return Dimension.Width;
            default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public static Side ToLeadingSide(this LayoutDirection direction)
    {
        switch (direction)
        {
            case LayoutDirection.Col: return Side.Top;
            case LayoutDirection.Row: return Side.Start;
            default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public static Side ToTrailingSide(this LayoutDirection direction)
    {
        switch (direction)
        {
            case LayoutDirection.Col: return Side.Bottom;
            case LayoutDirection.Row: return Side.End;
            default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public static Side ToPosition(this LayoutDirection direction)
    {
        switch (direction)
        {
            case LayoutDirection.Col: return Side.Top;
            case LayoutDirection.Row: return Side.Start;
            default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public static LayoutDirection ToDirection(this ContentsPlan contentsPlan)
    {
        switch (contentsPlan)
        {
        case ContentsPlan.Vertical: 
            return LayoutDirection.Col;
        case ContentsPlan.FreeForm:   
        case ContentsPlan.Horizontal:
        default:
            return LayoutDirection.Row;
        }
    }

}
