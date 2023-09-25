namespace No8.Ascii.GridLayout;

public class LTRB<T>
{
    public T Left { get; set; }
    public T Top { get; set; }
    public T Right { get; set; }
    public T Bottom { get; set; }

    public LTRB(T left, T top, T right, T bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public LTRB(T horz, T vert)
    {
        Left = Right = horz;
        Top = Bottom = vert;
    }
    
    public LTRB(T all)
    {
        Left = Top = Right = Bottom = all;
    }

    public static LTRB<int> XY_Width_Height(int X, int Y, int width, int height) 
        => new(X, Y, X + width, Y + height);
    public static LTRB<int> XY(int X, int Y) 
        => new(X, Y, X + 1, Y + 1);

    protected bool Equals(LTRB<T> other)
    {
        return EqualityComparer<T>.Default.Equals(Left, other.Left) && 
               EqualityComparer<T>.Default.Equals(Top, other.Top) && 
               EqualityComparer<T>.Default.Equals(Right, other.Right) && 
               EqualityComparer<T>.Default.Equals(Bottom, other.Bottom);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != this.GetType())
            return false;

        return Equals((LTRB<T>)obj);
    }

    public override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

    public static bool operator ==(LTRB<T>? left, LTRB<T>? right) => Equals(left, right);
    public static bool operator !=(LTRB<T>? left, LTRB<T>? right) => !Equals(left, right);
}