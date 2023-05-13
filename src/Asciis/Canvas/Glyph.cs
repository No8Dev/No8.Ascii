namespace No8.Ascii;

public class Glyph : ICloneable
{
    public static readonly Glyph Clear = new(' ');

    private Rune   _chr;

    public Rune Chr
    {
        get => _chr;
        set
        {
            if (value.Utf16SequenceLength == 1)
                ChrStr = "" + value;
            else
                ChrStr = "" + value;
            _chr = value;
        }
    }

    public  Color? Fore { get; set; }
    public  Color? Back { get; set; }

    public float OffsetX { get; set; }
    public float OffsetY { get; set; }

    public string ChrStr { get; private set; }

    public Glyph(Rune rune, Color? fore = null, Color? back = null)
    {
        Fore = fore;
        Back = back;
        Chr  = rune;
        if (rune.Utf16SequenceLength == 1)
            ChrStr = "" + Chr;
        else
            ChrStr = "" + Chr;
    }
    
    public Glyph(string str, Color? fore = null, Color? back = null)
    {
        Fore = fore;
        Back = back;
        if (str.Length > 0)
        {
            if (!char.IsSurrogate(str[0]))
            {
                Chr    = (Rune)str[0];
                ChrStr = str.Substring(0, 1);
                return;
            }

            if (str.Length > 1 && char.IsSurrogatePair(str[0], str[1]))
            {
                Chr    = (Rune)char.ConvertToUtf32(str[0], str[1]);
                ChrStr = "" + Chr;
                return;
            }
        }
        
        Chr    = (Rune)0;
        ChrStr = string.Empty;
    }

    public Glyph(char chr, Color? fore = null, Color? back = null)
    {
        Chr = (Rune)chr;
        ChrStr = new string(chr, 1);
        Fore = fore;
        Back = back;
    }

    public Glyph(Glyph other)
    {
        Chr    = other.Chr;
        ChrStr = other.ChrStr;
        Fore   = other.Fore;
        Back   = other.Back;
    }

    public void CopyFrom(Glyph other)
    {
        Chr = other.Chr;
        ChrStr = other.ChrStr;
        Fore = other.Fore;
        Back = other.Back;
    }

    protected bool Equals(Glyph other)
    {
        return Chr.Equals(other.Chr) && 
               Nullable.Equals(Fore, other.Fore) && 
               Nullable.Equals(Back, other.Back);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Glyph)obj);
    }

    public override int GetHashCode()
    {
        // ReSharper disable NonReadonlyMemberInGetHashCode
        return HashCode.Combine(Chr, Fore, Back);
        // ReSharper restore NonReadonlyMemberInGetHashCode
    }

    public static bool operator ==(Glyph? left, Glyph? right) => Equals(left, right);
    public static bool operator !=(Glyph? left, Glyph? right) => !Equals(left, right);
    public static implicit operator Glyph(char ch) => new Glyph(ch);
    public static implicit operator Rune(Glyph glyph) => glyph.Chr;

    public override string ToString() => $"{ChrStr}";

    public object Clone() => new Glyph(this);
}
