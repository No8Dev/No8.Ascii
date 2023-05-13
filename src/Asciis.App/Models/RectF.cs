namespace Asciis.App;

/// <summary>
/// A two-dimensional immutable rectangle with float coordinates.
///
/// The boundaries of the rect are two half-open intervals when
/// determining which points are inside the rect. For example, consider the
/// rect whose coordinates are (-1, 1)-(3, 4):
///
///      -2 -1  0  1  2  3  4
///       |  |  |  |  |  |  |
///     0-
///     1-   +-----------+
///     2-   |           |
///     3-   |           |
///     4-   +-----------+
///     5-
///
/// It contains all points within that region except for the ones that lie
/// directly on the right and bottom edges. (It's always right and bottom,
/// even if the rectangle has negative coordinates.) In the above examples,
/// that's these points:
///
///      -2 -1  0  1  2  3  4
///       |  |  |  |  |  |  |
///     0-
///     1-   *--*--*--*--+
///     2-   *  *  *  *  |
///     3-   *  *  *  *  |
///     4-   +-----------+
///     5-
///
/// This seems a bit odd, but does what you want in almost all respects. For
/// example, the width of this rect, determined by subtracting the left
/// coordinate (-1) from the right (3) is 4 and indeed it contains four columns
/// of points.
/// </summary>
public class RectF
{
    public static readonly RectF Empty = new RectF(VecF.Zero, VecF.Zero);

    /// Creates a new rectangle that is the intersection of [a] and [b].
    ///
    ///     .----------.
    ///     | a        |
    ///     | .--------+----.
    ///     | | result |  b |
    ///     | |        |    |
    ///     '-+--------'    |
    ///       |             |
    ///       '-------------'
    public static RectF Intersect(RectF a, RectF b)
    {
        var left   = Math.Max(a.Left, b.Left);
        var right  = Math.Min(a.Right, b.Right);
        var top    = Math.Max(a.Top, b.Top);
        var bottom = Math.Min(a.Bottom, b.Bottom);

        var width  = Math.Max(0, right - left);
        var height = Math.Max(0, bottom - top);

        return new RectF(left, top, width, height);
    }

    public static RectF CenterIn(RectF toCenter, RectF main)
    {
        var pos = main.Pos + (main.Size - toCenter.Size) / 2;
        return RectF.PosAndSize(pos, toCenter.Size);
    }

    public VecF Pos  { get; }
    public VecF Size { get; }

    private RectF(VecF pos, VecF size)
    {
        Pos  = pos;
        Size = size;
    }

    public RectF(float x, float y, float width, float height)
    {
        Pos  = new VecF(x, y);
        Size = new VecF(width, height);
    }

    public static RectF Create(float x,   float y, float width, float height) => new(x, y, width, height);
    public static RectF Create(VecF  pos, VecF  size) => new(pos, size);

    public float X      => Pos.X;
    public float Y      => Pos.Y;
    public float Width  => Size.X;
    public float Height => Size.Y;

    // Use min and max to handle negative sizes.

    public float Left   => Math.Min(X, X + Width);
    public float Top    => Math.Min(Y, Y + Height);
    public float Right  => Math.Max(X, X + Width);
    public float Bottom => Math.Max(Y, Y + Height);

    public VecF TopLeft     => new VecF(Left, Top);
    public VecF TopRight    => new VecF(Right, Top);
    public VecF BottomLeft  => new VecF(Left, Bottom);
    public VecF BottomRight => new VecF(Right, Bottom);

    public VecF Center => new VecF((Left + Right) / 2f, (Top + Bottom) / 2f);

    public float Area => Size.Area;

    public static RectF PosAndSize(VecF pos, VecF size) { return new RectF(pos, size); }

    public static RectF LeftTopRightBottom(float left, float top, float right, float bottom)
    {
        return new RectF(new VecF(left, top),
            new VecF(right - left, bottom - top));
    }

    /// Creates a new rectangle a single row in height, as wide as [size],
    /// with its top left corner at [pos].
    public static RectF Row(float x, float y, float size)
    {
        return new RectF(x, y, size, 1);
    }

    /// Creates a new rectangle a single column in width, as tall as [size],
    /// with its top left corner at [pos].
    public static RectF Column(float x, float y, float size) { return new RectF(x, y, 1, size); }

    public override string ToString() => $"({Pos})-({Size})";

    public RectF Inflate(float distance)
    {
        return new RectF(
            X - distance, 
            Y - distance, 
            Width + (distance * 2),
            Height + (distance * 2));
    }

    public RectF Scale(float? scaleX, float? scaleY)
    {
        if (scaleX == null && scaleY == null)
            return this;
        var width = Width * (scaleX ?? 1);
        var height = Height * (scaleY ?? 1);

        if (MathF.Abs(Width - width) < 1 &&
            MathF.Abs(Height - height) < 1)
            return this;

        return new RectF(
            X + (MathF.Abs(Width - width) / 2f),
            Y + (MathF.Abs(Height - height) / 2f),
            width, height);
    }

    public RectF Offset(float x, float y)
    {
        if (x == 0 && y == 0)
            return this;
        return new(X + x, Y + y, Width, Height);
    }

    public bool Contains(object obj)
    {
        if (float.IsNaN(Size.X) || float.IsNaN(Size.Y))
            return false;

        if (obj is Vec point)
        {
            if (point.X < Pos.X) return false;
            if (point.X >= Pos.X + Size.X) return false;
            if (point.Y < Pos.Y) return false;
            if (point.Y >= Pos.Y + Size.Y) return false;

            return true;
        }
        if (obj is VecF pointF)
        {
            if (pointF.X < Pos.X) return false;
            if (pointF.X >= Pos.X + Size.X) return false;
            if (pointF.Y < Pos.Y) return false;
            if (pointF.Y >= Pos.Y + Size.Y) return false;

            return true;
        }

        return false;
    }

    public bool ContainsRect(Rect rect)
    {
        if (rect.Left < Left) return false;
        if (rect.Right > Right) return false;
        if (rect.Top < Top) return false;
        if (rect.Bottom > Bottom) return false;

        return true;
    }
    
    public bool ContainsAnyPart(RectF rect)
    {
        return (rect.Right >= Left) &&
               (rect.Left <= Right) &&
               (rect.Bottom >= Top) &&
               (rect.Top <= Bottom);
    }

    public bool Contains(int x, int y)
    {
        return x >= Left &&
               x < Right &&
               y >= Top &&
               y < Bottom;
    }



    public VecF Clamp(VecF vec)
    {
        var x = vec.X.Clamp(Left, Right);
        var y = vec.Y.Clamp(Top, Bottom);
        return new VecF(x, y);
    }

    /// Returns the distance between this Rect and [other]. This is minimum
    /// length that a corridor would have to be to go from one Rect to the other.
    /// If the two Rects are adjacent, returns zero. If they overlap, returns -1.
    public float DistanceTo(RectF other)
    {
        float vertical;
        if (Top >= other.Bottom)
        {
            vertical = Top - other.Bottom;
        }
        else if (Bottom <= other.Top)
        {
            vertical = other.Top - Bottom;
        }
        else
        {
            vertical = -1;
        }

        float horizontal;
        if (Left >= other.Right)
        {
            horizontal = Left - other.Right;
        }
        else if (Right <= other.Left)
        {
            horizontal = other.Left - Right;
        }
        else
        {
            horizontal = -1;
        }

        if (vertical.Is(-1f) && horizontal.Is(-1f)) return -1;
        if (vertical.Is(-1f)) return horizontal;
        if (horizontal.Is(-1f)) return vertical;
        return horizontal + vertical;
    }

    /// Iterates over the points along the edge of the Rect.
    public IEnumerable<VecF> Trace()
    {
        if ((Width > 1) && (Height > 1))
        {
            for (var x = Left; x < Right; x++)
            {
                yield return new VecF(x, Top);
                yield return new VecF(x, Bottom - 1);
            }

            for (var y = Top + 1; y < Bottom - 1; y++)
            {
                yield return new VecF(Left, y);
                yield return new VecF(Right - 1, y);
            }

            yield break;
        }
        if ((Width > 1) && (Height.Is(1)))
        {
            var rect = Row(Left, Top, Width);

            yield return rect.TopLeft;
            yield return rect.BottomRight;
        }
        if ((Height >= 1) && (Width.Is(1)))
        {
            var rect = Column(Left, Top, Height);

            yield return rect.TopLeft;
            yield return rect.BottomRight;
        }
    }

    protected bool Equals(RectF other) =>
        Equals(Pos, other.Pos)
        && Equals(Size, other.Size);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;

        return Equals((RectF)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Pos != null ? Pos.GetHashCode() : 0) * 397)
                   ^ (Size != null ? Size.GetHashCode() : 0);
        }
    }

    public static implicit operator Rect(RectF rectF) => new Rect((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
    public static implicit operator RectF(Rect rect) => new RectF(rect.X, rect.Y, rect.Width, rect.Height);
}