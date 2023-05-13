using No8.Ascii.Console;

namespace No8.Ascii;

internal class DimensionsFloat
{
    public float Width { get; set; }
    public float Height { get; set; }

    public DimensionsFloat(float width = float.NaN, float height = float.NaN)
    {
        Width = width;
        Height = height;
    }

    public DimensionsFloat(DimensionsFloat other)
    {
        Width = other.Width;
        Height = other.Height;
    }

    public float this[Dimension dim]
    {
        get
        {
            switch (dim)
            {
                case Dimension.Width: return Width;
                case Dimension.Height: return Height;
            }

            throw new ArgumentException("Unknown dimension", nameof(dim));
        }
        set
        {
            switch (dim)
            {
                case Dimension.Width:
                    Width = value;
                    return;
                case Dimension.Height:
                    Height = value;
                    return;
                default:
                    throw new ArgumentException("Unknown dimension", nameof(dim));
            }
        }
    }

    /// <inheritdoc />
    public override string ToString() => $"({Width}, {Height})";
}
