namespace Asciis.App;

public class Orientation : Vec
{
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

    private Orientation(int x, int y) : base(x, y) { }

    public Orientation RotateLeft45
    {
        get
        {
            if (ReferenceEquals(this, None)) return None;
            if (ReferenceEquals(this, N)) return Nw;
            if (ReferenceEquals(this, Ne)) return N;
            if (ReferenceEquals(this, E)) return Ne;
            if (ReferenceEquals(this, Se)) return E;
            if (ReferenceEquals(this, S)) return Se;
            if (ReferenceEquals(this, Sw)) return S;
            if (ReferenceEquals(this, W)) return Sw;
            if (ReferenceEquals(this, Nw)) return W;

            throw new NotSupportedException();
        }
    }

    public Orientation RotateRight45
    {
        get
        {
            if (ReferenceEquals(this, None)) return None;
            if (ReferenceEquals(this, N)) return Ne;
            if (ReferenceEquals(this, Ne)) return E;
            if (ReferenceEquals(this, E)) return Se;
            if (ReferenceEquals(this, Se)) return S;
            if (ReferenceEquals(this, S)) return Sw;
            if (ReferenceEquals(this, Sw)) return W;
            if (ReferenceEquals(this, W)) return Nw;
            if (ReferenceEquals(this, Nw)) return N;

            throw new NotSupportedException();

        }
    }

    public Orientation RotateLeft90
    {
        get
        {
            if (ReferenceEquals(this, None)) return None;
            if (ReferenceEquals(this, N)) return W;
            if (ReferenceEquals(this, Ne)) return Nw;
            if (ReferenceEquals(this, E)) return N;
            if (ReferenceEquals(this, Se)) return Ne;
            if (ReferenceEquals(this, S)) return E;
            if (ReferenceEquals(this, Sw)) return Se;
            if (ReferenceEquals(this, W)) return S;
            if (ReferenceEquals(this, Nw)) return Sw;

            throw new NotSupportedException();
        }
    }

    public Orientation RotateRight90
    {
        get
        {
            if (ReferenceEquals(this, None)) return None;
            if (ReferenceEquals(this, N)) return E;
            if (ReferenceEquals(this, Ne)) return Se;
            if (ReferenceEquals(this, E)) return S;
            if (ReferenceEquals(this, Se)) return Sw;
            if (ReferenceEquals(this, S)) return W;
            if (ReferenceEquals(this, Sw)) return Nw;
            if (ReferenceEquals(this, W)) return N;
            if (ReferenceEquals(this, Nw)) return Ne;

            throw new NotSupportedException();
        }
    }

    public Orientation Rotate180
    {
        get
        {
            if (ReferenceEquals(this, None)) return None;
            if (ReferenceEquals(this, N)) return S;
            if (ReferenceEquals(this, Ne)) return Sw;
            if (ReferenceEquals(this, E)) return W;
            if (ReferenceEquals(this, Se)) return Nw;
            if (ReferenceEquals(this, S)) return N;
            if (ReferenceEquals(this, Sw)) return Ne;
            if (ReferenceEquals(this, W)) return E;
            if (ReferenceEquals(this, Nw)) return Se;

            throw new NotSupportedException();
        }
    }

    public override string ToString()
    {
        if (ReferenceEquals(this, None)) return "none";
        if (ReferenceEquals(this, N)) return "n";
        if (ReferenceEquals(this, Ne)) return "ne";
        if (ReferenceEquals(this, E)) return "e";
        if (ReferenceEquals(this, Se)) return "se";
        if (ReferenceEquals(this, S)) return "s";
        if (ReferenceEquals(this, Sw)) return "sw";
        if (ReferenceEquals(this, W)) return "w";
        if (ReferenceEquals(this, Nw)) return "nw";

        return base.ToString();
    }
}