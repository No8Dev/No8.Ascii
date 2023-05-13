namespace Asciis.UI;

public class Vec
{
    public static readonly Vec Zero = new Vec(0, 0);
    public static readonly Vec Unknown = new Vec(int.MinValue, int.MinValue);

    public int X { get; }
    public int Y { get; }

    public Vec(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int Area => X * Y;

    /// <summary>
    /// Number of squares a rook on a chessboard would need to move from (0, 0) to reach the endpoint of the Vec.
    /// Also known as Manhattan or taxicab distance
    /// </summary>
    public int RookLength => Math.Abs(X) + Math.Abs(Y);

    /// <summary>
    /// Length of the Vec, which is the number of squares a king on a chessboard would need to move from (0, 0) to reach the endpoint of the Vec.
    /// Also known as Chebyshev distance
    /// </summary>
    public int KingLength => Math.Max(Math.Abs(X), Math.Abs(Y));

    public int LengthSquared => X * X + Y * Y;

    /// <summary>
    /// Cartesian length of the Vec
    /// If you just need to compare the magnitude of two vectors, prefer using the comparison operators or [LengthSquared],
    /// both of which are faster than this
    /// </summary>
    public double Length => Math.Sqrt(LengthSquared);

    /// <Summary>
    /// The [Orientation] that most closely approximates the angle of this Vec.
    ///
    /// In cases where two orientations are equally close, chooses the one that is
    /// clockwise from this Vec's angle.
    ///
    /// In other words, it figures out which octant the vector's angle lies in
    /// (the dotted lines) and chooses the corresponding orientation:
    ///
    ///               n
    ///      nw   2.0  -2.0  ne
    ///         \  '  |  '  /
    ///          \    |    /
    ///      0.5  \ ' | ' /   -0.5
    ///         '  \  |  /  '
    ///           ' \'|'/ '
    ///             '\|/'
    ///       w ------0------ e
    ///             '/|\'
    ///           ' /'|'\ '
    ///         '  /  |  \  '
    ///     -0.5  / ' | ' \   0.5
    ///          /    |    \
    ///         /  '  |  '  \
    ///       sw -2.0   2.0  se
    ///               s
    /// </Summary>
    public Orientation NearestOrientation
    {
        get
        {
            if (X == 0)
            {
                if (Y < 0) return Orientation.N;
                if (Y == 0) return Orientation.None;

                return Orientation.S;
            }

            var slope = Y / X;

            if (X < 0)
            {
                if (slope >= 2.0) return Orientation.N;
                if (slope >= 0.5) return Orientation.Nw;
                if (slope >= -0.5) return Orientation.W;
                if (slope >= -2.0) return Orientation.Sw;

                return Orientation.S;
            }

            if (slope >= 2.0) return Orientation.S;
            if (slope >= 0.5) return Orientation.Se;
            if (slope >= -0.5) return Orientation.E;
            if (slope >= -2.0) return Orientation.Ne;

            return Orientation.N;
        }
    }

    /// <summary>
    /// The eight Vecs surrounding this one to the north, south, east, and west and points in between.
    /// </summary>
    public List<Vec> Neighbours
    {
        get
        {
            var result = new List<Vec>();
            foreach (var orientation in Orientation.All) 
                result.Add(this + orientation);
            return result;
        }
    }

    /// <summary>
    /// The four Vecs surrounding this one to the north, south, east, and west. 
    /// </summary>
    public List<Vec> CardinalNeighbours
    {
        get
        {
            var result = new List<Vec>();
            foreach (var orientation in Orientation.Cardinal) result.Add(this + orientation);
            return result;
        }
    }

    /// <summary>
    /// The four Vecs surrounding this one to the northeast, southeast, southwest, and northwest.
    /// </summary>
    public List<Vec> InterCardinalNeighbours
    {
        get
        {
            var result = new List<Vec>();
            foreach (var orientation in Orientation.InterCardinal)
                result.Add(this + orientation);
            return result;
        }
    }

    public static Vec operator *(Vec left, int other) { return new Vec(left.X * other, left.Y * other); }
    public static Vec operator /(Vec left, int other) { return new Vec(left.X / other, left.Y / other); }

    public static Vec operator +(Vec left, int other) { return new Vec(left.X + other, left.Y + other); }
    public static Vec operator +(Vec left, Vec other) { return new Vec(left.X + other.X, left.Y + other.Y); }

    public static Vec operator -(Vec left, int other) { return new Vec(left.X - other, left.Y - other); }
    public static Vec operator -(Vec left, Vec other) { return new Vec(left.X - other.X, left.Y - other.Y); }

    public static bool operator >(Vec left, int other) { return left.LengthSquared > (other * other); }
    public static bool operator >(Vec left, Vec other) { return left.LengthSquared > other.LengthSquared; }
    public static bool operator <(Vec left, int other) { return left.LengthSquared < (other * other); }
    public static bool operator <(Vec left, Vec other) { return left.LengthSquared < other.LengthSquared; }

    public static bool operator >=(Vec left, int other) { return left.LengthSquared >= (other * other); }
    public static bool operator >=(Vec left, Vec other) { return left.LengthSquared >= other.LengthSquared; }
    public static bool operator <=(Vec left, int other) { return left.LengthSquared <= (other * other); }
    public static bool operator <=(Vec left, Vec other) { return left.LengthSquared <= other.LengthSquared; }

    /// <summary>
    /// Gets whether the given vector is within a rectangle from (0,0) to this vector (half-inclusive).
    /// </summary>
    public bool Contains(Vec pos)
    {
        var left = Math.Min(0, X);
        if (pos.X < left) return false;

        var right = Math.Max(0, X);
        if (pos.X >= right) return false;

        var top = Math.Min(0, Y);
        if (pos.Y < top) return false;

        var bottom = Math.Max(0, Y);
        
        return pos.Y < bottom;
    }

    public Vec Abs() => new Vec(Math.Abs(X), Math.Abs(Y));

    public Vec Offset(int x, int y) => new Vec(X + x, Y + y);
    public Vec OffsetX(int x) => new Vec(X + x, Y);
    public Vec OffsetY(int y) => new Vec(X, Y + y);

    private bool Equals(Vec other) =>
        X == other.X
     && Y == other.Y;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        
        return obj.GetType() == GetType() && Equals((Vec)obj);
    }

    public override int GetHashCode()
    {
        unchecked { return (X * 397) ^ Y; }
    }

    public override string ToString() => $"{X}, {Y}";
}