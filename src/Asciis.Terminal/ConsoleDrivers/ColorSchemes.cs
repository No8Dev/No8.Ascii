namespace Asciis.Terminal.Core;

/// <summary>
///     The default <see cref="ColorScheme"/>s for the application.
/// </summary>
public class ColorSchemes : Dictionary<string, ColorScheme>
{
    public enum Key
    {
        TopLevel,
        Base,
        Dialog,
        Menu,
        Error
    }

    public ColorSchemes()
    {
        this[Key.TopLevel] = new ColorScheme();
        this[Key.Base] = new ColorScheme();
        this[Key.Dialog] = new ColorScheme();
        this[Key.Menu] = new ColorScheme();
        this[Key.Error] = new ColorScheme();
    }

    public ColorScheme this[Key key]
    {
        get => this[key.ToString()];
        set => this[key.ToString()] = value;
    }

    public ColorScheme TopLevel { get => this[Key.TopLevel]; set => this[Key.TopLevel] = value; }
    public ColorScheme Base { get => this[Key.Base]; set => this[Key.Base] = value; }
    public ColorScheme Dialog { get => this[Key.Dialog]; set => this[Key.Dialog] = value; }
    public ColorScheme Menu { get => this[Key.Menu]; set => this[Key.Menu] = value; }
    public ColorScheme Error { get => this[Key.Error]; set => this[Key.Error] = value; }
}
