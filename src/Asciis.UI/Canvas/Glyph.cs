using System.Drawing;

namespace Asciis.UI;

public class Glyph
{
    public static readonly Glyph Clear = new(' ');

    public char Chr { get; }
    public Color Fore { get; }
    public Color Back { get; }

    public float OffsetX { get; set; }
    public float OffsetY { get; set; }

    public string ChrStr { get; }

    public Glyph(string str, Color? fore = null, Color? back = null)
    {
        Chr = str[0];
        ChrStr = new string(Chr, 1);
        Fore = fore ?? Color.White;
        Back = back ?? Color.Black;
    }

    public Glyph(char chr, Color? fore = null, Color? back = null)
    {
        Chr = chr;
        ChrStr = new string(Chr, 1);
        Fore = fore ?? Color.White;
        Back = back ?? Color.Black;
    }

    public static bool operator ==(Glyph? left, Glyph? other)
    {
        if (left is null)
            return other is null;

        return left.Equals(other);
    }
    public static bool operator !=(Glyph? left, Glyph? other) { return !(left == other); }

    protected bool Equals(Glyph? other) =>
        !(other is null)
     && Chr == other.Chr
     && Fore.Equals(other.Fore)
     && Back.Equals(other.Back);

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((Glyph)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Chr.GetHashCode();
            hashCode = (hashCode * 397) ^ Fore.GetHashCode();
            hashCode = (hashCode * 397) ^ Back.GetHashCode();

            return hashCode;
        }
    }

    public override string ToString() => $"{ChrStr}";
}
