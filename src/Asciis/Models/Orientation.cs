namespace No8.Ascii;

public readonly struct Orientation : IVec
{
    public bool Equals(Orientation other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Orientation other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Orientation left, Orientation right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Orientation left, Orientation right)
    {
        return !left.Equals(right);
    }

    public static readonly Orientation None = new(0, 0);
    public static readonly Orientation N    = new(0, -1);
    public static readonly Orientation Ne   = new(1, -1);
    public static readonly Orientation E    = new(1, 0);
    public static readonly Orientation Se   = new(1, 1);
    public static readonly Orientation S    = new(0, 1);
    public static readonly Orientation Sw   = new(-1, 1);
    public static readonly Orientation W    = new(-1, 0);
    public static readonly Orientation Nw   = new(-1, -1);

    /// The eight main orientations.
    public static readonly Orientation[] All = { N, Ne, E, Se, S, Sw, W, Nw };

    /// The four main orientations: north, south, east, and west.
    public static readonly Orientation[] Cardinal = { N, E, S, W };

    /// The four intermediate orientations between the main ones: northwest, northeast, southwest and southeast.
    public static readonly Orientation[] InterCardinal = { Ne, Se, Sw, Nw };

    private Orientation(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }

    public Orientation RotateLeft45
    {
        get
        {
            if (Equals(this, None)) return None;
            if (Equals(this, N)) return Nw;
            if (Equals(this, Ne)) return N;
            if (Equals(this, E)) return Ne;
            if (Equals(this, Se)) return E;
            if (Equals(this, S)) return Se;
            if (Equals(this, Sw)) return S;
            if (Equals(this, W)) return Sw;
            if (Equals(this, Nw)) return W;

            throw new NotSupportedException();
        }
    }

    public Orientation RotateRight45
    {
        get
        {
            if (Equals(this, None)) return None;
            if (Equals(this, N)) return Ne;
            if (Equals(this, Ne)) return E;
            if (Equals(this, E)) return Se;
            if (Equals(this, Se)) return S;
            if (Equals(this, S)) return Sw;
            if (Equals(this, Sw)) return W;
            if (Equals(this, W)) return Nw;
            if (Equals(this, Nw)) return N;

            throw new NotSupportedException();

        }
    }

    public Orientation RotateLeft90
    {
        get
        {
            if (Equals(this, None)) return None;
            if (Equals(this, N)) return W;
            if (Equals(this, Ne)) return Nw;
            if (Equals(this, E)) return N;
            if (Equals(this, Se)) return Ne;
            if (Equals(this, S)) return E;
            if (Equals(this, Sw)) return Se;
            if (Equals(this, W)) return S;
            if (Equals(this, Nw)) return Sw;

            throw new NotSupportedException();
        }
    }

    public Orientation RotateRight90
    {
        get
        {
            if (Equals(this, None)) return None;
            if (Equals(this, N)) return E;
            if (Equals(this, Ne)) return Se;
            if (Equals(this, E)) return S;
            if (Equals(this, Se)) return Sw;
            if (Equals(this, S)) return W;
            if (Equals(this, Sw)) return Nw;
            if (Equals(this, W)) return N;
            if (Equals(this, Nw)) return Ne;

            throw new NotSupportedException();
        }
    }

    public Orientation Rotate180
    {
        get
        {
            if (Equals(this, None)) return None;
            if (Equals(this, N)) return S;
            if (Equals(this, Ne)) return Sw;
            if (Equals(this, E)) return W;
            if (Equals(this, Se)) return Nw;
            if (Equals(this, S)) return N;
            if (Equals(this, Sw)) return Ne;
            if (Equals(this, W)) return E;
            if (Equals(this, Nw)) return Se;

            throw new NotSupportedException();
        }
    }

    public override string ToString()
    {
        if (Equals(this, None)) return "none";
        if (Equals(this, N)) return "n";
        if (Equals(this, Ne)) return "ne";
        if (Equals(this, E)) return "e";
        if (Equals(this, Se)) return "se";
        if (Equals(this, S)) return "s";
        if (Equals(this, Sw)) return "sw";
        if (Equals(this, W)) return "w";
        if (Equals(this, Nw)) return "nw";

        return $"{X}, {Y}";
    }
}