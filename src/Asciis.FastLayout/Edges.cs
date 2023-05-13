namespace Asciis.FastLayout;

using static NumberExtensions;

public class Edges
{
    public static readonly Edges Zero = new(0, 0, 0, 0);

    public float Left   { get; set; }
    public float Top    { get; set; }
    public float Right  { get; set; } 
    public float Bottom { get; set; }

    public Edges() { }

    public Edges(float value)
    {
        Left = Top = Right = Bottom = value;
    }

    public Edges(float left, float top, float right, float bottom)
    {
        Left   = left;
        Top    = top;
        Right  = right;
        Bottom = bottom;
    }
    
    public static Edges Create(float defaultValue = default)
    {
        return new(defaultValue, defaultValue, defaultValue, defaultValue);
    }

    public static Edges Create(Edges other)
    {
        return new(other.Left, other.Top, other.Right, other.Bottom);
    }

    public bool IsZero =>
        Left.IsZero()
        && Top.IsZero()
        && Right.IsZero()
        && Bottom.IsZero();

    public bool IsNotZero => !IsZero;
    
    public float this[Side edge]
    {
        get
        {
            switch (edge)
            {
            case Side.Left:   return Left;
            case Side.Top:    return Top;
            case Side.Right:  return Right;
            case Side.Bottom: return Bottom;
            default:
                throw new ArgumentException("Unknown edge", nameof(edge));
            }
        }
        set
        {
            switch (edge)
            {
            case Side.Left:
                Left = value;
                break;
            case Side.Top:
                Top = value;
                break;
            case Side.Right:
                Right = value;
                break;
            case Side.Bottom:
                Bottom = value;
                break;
            default:
                throw new ArgumentException("Unknown edge", nameof(edge));
            }
        }

    }

    public static implicit operator Edges(float value) => Create(value);

    public static Edges operator +(Edges? a, Edges? b)
    {
        a ??= Zero;
        b ??= Zero;
        return new Edges(
            a.Left + b.Left,
            a.Top + b.Top,
            a.Right + b.Right,
            a.Bottom + b.Bottom);
    }
    
    public virtual bool Equals(Edges? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return FloatsEqual(Left, other.Left) && 
               FloatsEqual( Top, other.Top) && 
               FloatsEqual( Right, other.Right) && 
               FloatsEqual( Bottom, other.Bottom);
    }

    public override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

    public override string ToString() => $"({Left}, {Top}, {Right}, {Bottom})";
}