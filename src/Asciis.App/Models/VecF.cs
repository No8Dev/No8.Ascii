using System.Diagnostics;

namespace Asciis.App;

[DebuggerDisplay("({X},{Y})")]
public class VecF
{
    public static readonly VecF Zero    = new VecF(0, 0);
    public static readonly VecF Unknown = new VecF(float.MinValue, float.MinValue);

    public float X { get; }
    public float Y { get; }

    public VecF(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float Area => X * Y;

    /// <summary>
    /// Number of squares a rook on a chessboard would need to move from (0, 0) to reach the endpoint of the VecF.
    /// Also known as Manhattan or taxicab distance
    /// </summary>
    public float RookLength => Math.Abs(X) + Math.Abs(Y);

    /// <summary>
    /// Length of the VecF, which is the number of squares a king on a chessboard would need to move from (0, 0) to reach the endpoint of the VecF.
    /// Also known as Chebyshev distance
    /// </summary>
    public float KingLength => Math.Max(Math.Abs(X), Math.Abs(Y));

    public float LengthSquared => X * X + Y * Y;

    // The tor.
    //
    // 


    /// <summary>
    /// Cartesian length of the VecF
    /// If you just need to compare the magnitude of two VecFtors, prefer using the comparison operators or [LengthSquared],
    /// both of which are faster than this
    /// </summary>
    public double Length => Math.Sqrt(LengthSquared);

    /// <Summary>
    /// The [Orientation] that most closely approximates the angle of this VecF.
    ///
    /// In cases where two orientations are equally close, chooses the one that is
    /// clockwise from this VecF's angle.
    ///
    /// In other words, it figures out which octant the VecFtor's angle lies in
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
            if (X.IsZero())
            {
                if (Y < 0) return Orientation.N;
                if (Y.IsZero()) return Orientation.None;

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
    /// The eight VecFs surrounding this one to the north, south, east, and west and points in between.
    /// </summary>
    public List<VecF> Neighbours
    {
        get
        {
            var result = new List<VecF>();
            foreach (var orientation in Orientation.All) result.Add(this + orientation);
            return result;
        }
    }

    /// <summary>
    /// The four VecFs surrounding this one to the north, south, east, and west. 
    /// </summary>
    public List<VecF> CardinalNeighbours
    {
        get
        {
            var result = new List<VecF>();
            foreach (var orientation in Orientation.Cardinal) result.Add(this + orientation);
            return result;
        }
    }

    /// <summary>
    /// The four VecFs surrounding this one to the northeast, southeast, southwest, and northwest.
    /// </summary>
    public List<VecF> InterCardinalNeighbours
    {
        get
        {
            var result = new List<VecF>();
            foreach (var orientation in Orientation.InterCardinal) result.Add(this + orientation);
            return result;
        }
    }

    public static VecF operator *(VecF left, float other) { return new VecF(left.X * other, left.Y * other); }
    public static VecF operator /(VecF left, float other) { return new VecF(left.X / other, left.Y / other); }

    public static VecF operator +(VecF left, float  other) { return new VecF(left.X + other, left.Y + other); }
    public static VecF operator +(VecF left, Vec other) { return new VecF(left.X + other.X, left.Y + other.Y); }
    public static VecF operator +(VecF left, VecF   other) { return new VecF(left.X + other.X, left.Y + other.Y); }

    public static VecF operator -(VecF left, float  other) { return new VecF(left.X - other, left.Y - other); }
    public static VecF operator -(VecF left, Vec other) { return new VecF(left.X - other.X, left.Y - other.Y); }
    public static VecF operator -(VecF left, VecF   other) { return new VecF(left.X - other.X, left.Y - other.Y); }

    public static bool operator >(VecF left, float  other) { return left.LengthSquared > (other * other); }
    public static bool operator >(VecF left, Vec other) { return left.LengthSquared > other.LengthSquared; }
    public static bool operator >(VecF left, VecF   other) { return left.LengthSquared > other.LengthSquared; }
    public static bool operator <(VecF left, float  other) { return left.LengthSquared < (other * other); }
    public static bool operator <(VecF left, Vec other) { return left.LengthSquared < other.LengthSquared; }
    public static bool operator <(VecF left, VecF   other) { return left.LengthSquared < other.LengthSquared; }

    public static bool operator >=(VecF left, float  other) { return left.LengthSquared >= (other * other); }
    public static bool operator >=(VecF left, Vec other) { return left.LengthSquared >= other.LengthSquared; }
    public static bool operator >=(VecF left, VecF   other) { return left.LengthSquared >= other.LengthSquared; }
    public static bool operator <=(VecF left, float  other) { return left.LengthSquared <= (other * other); }
    public static bool operator <=(VecF left, Vec other) { return left.LengthSquared <= other.LengthSquared; }
    public static bool operator <=(VecF left, VecF   other) { return left.LengthSquared <= other.LengthSquared; }

    /// <summary>
    /// Gets whether the given Vector is within a rectangle from (0,0) to this Vector (half-inclusive).
    /// </summary>
    public bool Contains(VecF pos)
    {
        var left = Math.Min(0, X);
        if (pos.X < left) return false;

        var right = Math.Max(0, X);
        if (pos.X >= right) return false;

        var top = Math.Min(0, Y);
        if (pos.Y < top) return false;

        var bottom = Math.Max(0, Y);
        if (pos.Y >= bottom) return false;

        return true;
    }

    public VecF Abs() => new VecF(Math.Abs(X), Math.Abs(Y));

    public VecF Offset(float  x, float y) => new VecF(X + x, Y + y);
    public VecF OffsetX(float x) => new VecF(X + x, Y);
    public VecF OffsetY(float y) => new VecF(X, Y + y);

    protected bool Equals(VecF other) =>
        X.Equals(other.X)
        && Y.Equals(other.Y);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;

        return Equals((VecF)obj);
    }

    public override int GetHashCode()
    {
        unchecked { return (X.GetHashCode() * 397) ^ Y.GetHashCode(); }
    }

    public override string ToString() => $"{X}, {Y}";
}