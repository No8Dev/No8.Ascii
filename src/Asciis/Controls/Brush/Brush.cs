namespace No8.Ascii.Controls;

public abstract class Brush
{
    protected Brush() { }

    public abstract Color GetColorAt(float offset);

    //** Public methods
    //** Public properties
    //** Protected methods
    //** Protected properties
    //** Internal methods
    //** Internal properties
    //** Dependency properties
    //** Internal fields
    //** Constructors
    //** Serialization
    //** ToString

    public static implicit operator Brush(Color color) => new SolidColorBrush(color);
}
