using No8.Ascii.ElementLayout;

namespace No8.Ascii;

internal static class ElementArrangeHelpers
{
    public static Dimension ToDimension(this LayoutDirection direction)
    {
        switch (direction)
        {
        case LayoutDirection.Vert:        return Dimension.Height;
        case LayoutDirection.Horz:           return Dimension.Width;
        default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public static Side ToLeadingSide(this LayoutDirection direction)
    {
        switch (direction)
        {
        case LayoutDirection.Vert:        return Side.Top;
        case LayoutDirection.Horz:           return Side.Left;
        default:                            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public static Side ToTrailingSide(this LayoutDirection direction)
    {
        switch (direction)
        {
        case LayoutDirection.Vert:        return Side.Bottom;
        case LayoutDirection.Horz:           return Side.Right;
        default:                            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public static Side ToPosition(this LayoutDirection direction)
    {
        switch (direction)
        {
        case LayoutDirection.Vert:        return Side.Top;
        case LayoutDirection.Horz:           return Side.Left;
        default:                            throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

}