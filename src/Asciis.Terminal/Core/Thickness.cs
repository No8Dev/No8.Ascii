namespace Asciis.Terminal.Core;

/// <summary>
/// Describes the thickness of a frame around a rectangle. Four <see cref="int"/> values describe
///  the <see cref="Left"/>, <see cref="Top"/>, <see cref="Right"/>, and <see cref="Bottom"/> sides
///  of the rectangle, respectively.
/// </summary>
public struct Thickness
{
    /// <summary>
    /// Gets or sets the width, in integers, of the left side of the bounding rectangle.
    /// </summary>
    public int Left;

    /// <summary>
    /// Gets or sets the width, in integers, of the upper side of the bounding rectangle.
    /// </summary>
    public int Top;

    /// <summary>
    /// Gets or sets the width, in integers, of the right side of the bounding rectangle.
    /// </summary>
    public int Right;

    /// <summary>
    /// Gets or sets the width, in integers, of the lower side of the bounding rectangle.
    /// </summary>
    public int Bottom;

    /// <summary>
    /// Initializes a new instance of the <see cref="Thickness"/> structure that has the
    ///  specified uniform length on each side.
    /// </summary>
    /// <param name="length"></param>
    public Thickness(int length)
    {
        if (length < 0) throw new ArgumentException("Invalid value for this property.");

        Left = Top = Right = Bottom = length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Thickness"/> structure that has specific
    ///  lengths (supplied as a <see cref="int"/>) applied to each side of the rectangle.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <param name="right"></param>
    /// <param name="bottom"></param>
    public Thickness(int left, int top, int right, int bottom)
    {
        if (left < 0 || top < 0 || right < 0 || bottom < 0)
            throw new ArgumentException("Invalid value for this property.");

        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
}
