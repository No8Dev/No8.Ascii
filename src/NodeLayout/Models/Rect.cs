using System;
using System.Collections;
using System.Collections.Generic;

namespace NodeLayout;

/// <summary>
/// A two-dimensional immutable rectangle with integer coordinates.
///
/// Many operations treat a [Rect] as a collection of discrete points. In those
/// cases, the boundaries of the rect are two half-open intervals when
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
public class Rect
{
    public static readonly Rect Empty = new Rect(VecInt.Zero, VecInt.Zero);

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
    public static Rect Intersect(Rect a, Rect b)
    {
        var left = Math.Max(a.Left, b.Left);
        var right = Math.Min(a.Right, b.Right);
        var top = Math.Max(a.Top, b.Top);
        var bottom = Math.Min(a.Bottom, b.Bottom);

        var width = Math.Max(0, right - left);
        var height = Math.Max(0, bottom - top);

        return new Rect(left, top, width, height);
    }

    public static Rect CenterIn(Rect toCenter, Rect main)
    {
        var pos = main.Pos + (main.Size - toCenter.Size) / 2;
        return Rect.PosAndSize(pos, toCenter.Size);
    }

    public VecInt Pos { get; }
    public VecInt Size { get; }

    private Rect(VecInt pos, VecInt size)
    {
        Pos = pos;
        Size = size;
    }

    public Rect(int x, int y, int width, int height)
    {
        Pos = new VecInt(x, y);
        Size = new VecInt(width, height);
    }

    public int X => Pos.X;
    public int Y => Pos.Y;
    public int Width => Size.X;
    public int Height => Size.Y;

    // Use min and max to handle negative sizes.

    public int Left => Math.Min(X, X + Width);
    public int Top => Math.Min(Y, Y + Height);
    public int Right => Math.Max(X, X + Width);
    public int Bottom => Math.Max(Y, Y + Height);

    public VecInt TopLeft => new VecInt(Left, Top);
    public VecInt TopRight => new VecInt(Right, Top);
    public VecInt BottomLeft => new VecInt(Left, Bottom);
    public VecInt BottomRight => new VecInt(Right, Bottom);

    public VecInt Center => new VecInt((Left + Right) / 2, (Top + Bottom) / 2);

    public int Area => Size.Area;

    public static Rect PosAndSize(VecInt pos, VecInt size) { return new Rect(pos, size); }

    public static Rect LeftTopRightBottom(int left, int top, int right, int bottom)
    {
        return new Rect(new VecInt(left, top),
                        new VecInt(right - left, bottom - top));
    }

    /// Creates a new rectangle a single row in height, as wide as [size],
    /// with its top left corner at [pos].
    public static Rect Row(int x, int y, int size)
    {
        return new Rect(x, y, size, 1);
    }

    /// Creates a new rectangle a single column in width, as tall as [size],
    /// with its top left corner at [pos].
    public static Rect Column(int x, int y, int size) { return new Rect(x, y, 1, size); }

    //public IEnumerator<VecInt> GetEnumerator() => new RectIterator(this);
    //IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => $"({Pos})-({Size})";

    public Rect Inflate(int distance)
    {
        return new Rect(X - distance, Y - distance, Width + (distance * 2),
                        Height + (distance * 2));
    }

    public Rect Offset(int x, int y) => new Rect(this.X + x, this.Y + y, Width, Height);

    public bool Contains(object obj)
    {
        if (obj is VecInt point)
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

    public VecInt Clamp(VecInt vec)
    {
        var x = vec.X.Clamp(Left, Right);
        var y = vec.Y.Clamp(Top, Bottom);
        return new VecInt(x, y);
    }

    /// Returns the distance between this Rect and [other]. This is minimum
    /// length that a corridor would have to be to go from one Rect to the other.
    /// If the two Rects are adjacent, returns zero. If they overlap, returns -1.
    public int DistanceTo(Rect other)
    {
        int vertical;
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

        int horizontal;
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

        if (vertical == -1 && horizontal == -1) return -1;
        if (vertical == -1) return horizontal;
        if (horizontal == -1) return vertical;
        return horizontal + vertical;
    }

    /// Iterates over the points along the edge of the Rect.
    public IEnumerable<VecInt> Trace()
    {
        if ((Width > 1) && (Height > 1))
        {
            for (var x = Left; x < Right; x++)
            {
                yield return new VecInt(x, Top);
                yield return new VecInt(x, Bottom - 1);
            }

            for (var y = Top + 1; y < Bottom - 1; y++)
            {
                yield return new VecInt(Left, y);
                yield return new VecInt(Right - 1, y);
            }

            yield break;
        }
        if ((Width > 1) && (Height == 1))
        {
            var rect = Row(Left, Top, Width);

            yield return rect.TopLeft;
            yield return rect.BottomRight;
        }
        if ((Height >= 1) && (Width == 1))
        {
            var rect = Column(Left, Top, Height);

            yield return rect.TopLeft;
            yield return rect.BottomRight;
        }
    }

    class RectIterator : IEnumerator<VecInt>
    {
        private readonly Rect _rect;
        private int _x;
        private int _y;

        public RectIterator(Rect rect)
        {
            _rect = rect;
            _x = _rect.X - 1;
            _y = _rect.Y;
        }

        public bool MoveNext()
        {
            _x++;
            if (_x >= _rect.Right)
            {
                _x = _rect.X;
                _y++;
            }

            return _y < _rect.Bottom;
        }

        public void Reset()
        {
            _x = _rect.X - 1;
            _y = _rect.Y;
        }

        public VecInt Current => new VecInt(_x, _y);
        object IEnumerator.Current => Current;

        public void Dispose() { }
    }

    protected bool Equals(Rect other) =>
        Equals(Pos, other.Pos)
     && Equals(Size, other.Size);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;

        return Equals((Rect)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Pos != null ? Pos.GetHashCode() : 0) * 397)
                 ^ (Size != null ? Size.GetHashCode() : 0);
        }
    }
}
