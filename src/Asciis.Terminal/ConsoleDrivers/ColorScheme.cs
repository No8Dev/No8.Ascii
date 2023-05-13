using Asciis.Terminal.ConsoleDrivers;
using System.Runtime.CompilerServices;

namespace Asciis.Terminal.Core;

/// <summary>
/// Color scheme definitions, they cover some common scenarios and are used
/// typically in containers such as <see cref="Window"/> and <see cref="FrameView"/> to set the scheme that is used by all the
/// views contained inside.
/// </summary>
public class ColorScheme : IEquatable<ColorScheme>
{
    private CellAttributes _normal;
    private CellAttributes _focus;
    private CellAttributes _hotNormal;
    private CellAttributes _hotFocus;
    private CellAttributes _disabled;
    internal string caller = "";

    /// <summary>
    /// The default color for text, when the view is not focused.
    /// </summary>
    public CellAttributes Normal
    {
        get => _normal;
        set => _normal = SetAttribute(value);
    }

    /// <summary>
    /// The color for text when the view has the focus.
    /// </summary>
    public CellAttributes Focus
    {
        get => _focus;
        set => _focus = SetAttribute(value);
    }

    /// <summary>
    /// The color for the hotkey when a view is not focused
    /// </summary>
    public CellAttributes HotNormal
    {
        get => _hotNormal;
        set => _hotNormal = SetAttribute(value);
    }

    /// <summary>
    /// The color for the hotkey when the view is focused.
    /// </summary>
    public CellAttributes HotFocus
    {
        get => _hotFocus;
        set => _hotFocus = SetAttribute(value);
    }

    /// <summary>
    /// The default color for text, when the view is disabled.
    /// </summary>
    public CellAttributes Disabled
    {
        get => _disabled;
        set => _disabled = SetAttribute(value);
    }

    private bool preparingScheme = false;

    private CellAttributes SetAttribute(CellAttributes attribute, [CallerMemberName] string? callerMemberName = null)
    {
        if (preparingScheme)
            return attribute;

        preparingScheme = true;
        switch (caller)
        {
            case "TopLevel":
                switch (callerMemberName)
                {
                    case "Normal":
                        HotNormal = new (HotNormal.Foreground, attribute.Background);
                        break;
                    case "Focus":
                        HotFocus = new (HotFocus.Foreground, attribute.Background);
                        break;
                    case "HotNormal":
                        HotFocus = new (attribute.Foreground, HotFocus.Background);
                        break;
                    case "HotFocus":
                        HotNormal = new (attribute.Foreground, HotNormal.Background);
                        if (Focus.Foreground != attribute.Background)
                            Focus = new (Focus.Foreground, attribute.Background);
                        break;
                }

                break;

            case "Base":
                switch (callerMemberName)
                {
                    case "Normal":
                        HotNormal = new (HotNormal.Foreground, attribute.Background);
                        break;
                    case "Focus":
                        HotFocus = new (HotFocus.Foreground, attribute.Background);
                        break;
                    case "HotNormal":
                        HotFocus = new (attribute.Foreground, HotFocus.Background);
                        Normal = new (Normal.Foreground, attribute.Background);
                        break;
                    case "HotFocus":
                        HotNormal = new (attribute.Foreground, HotNormal.Background);
                        if (Focus.Foreground != attribute.Background)
                            Focus = new (Focus.Foreground, attribute.Background);
                        break;
                }

                break;

            case "Menu":
                switch (callerMemberName)
                {
                    case "Normal":
                        if (Focus.Background != attribute.Background)
                            Focus = new (attribute.Foreground, Focus.Background);
                        HotNormal = new (HotNormal.Foreground, attribute.Background);
                        Disabled = new (Disabled.Foreground, attribute.Background);
                        break;
                    case "Focus":
                        Normal = new (attribute.Foreground, Normal.Background);
                        HotFocus = new (HotFocus.Foreground, attribute.Background);
                        break;
                    case "HotNormal":
                        if (Focus.Background != attribute.Background)
                            HotFocus = new (attribute.Foreground, HotFocus.Background);
                        Normal = new (Normal.Foreground, attribute.Background);
                        Disabled = new (Disabled.Foreground, attribute.Background);
                        break;
                    case "HotFocus":
                        HotNormal = new (attribute.Foreground, HotNormal.Background);
                        if (Focus.Foreground != attribute.Background)
                            Focus = new (Focus.Foreground, attribute.Background);
                        break;
                    case "Disabled":
                        if (Focus.Background != attribute.Background)
                            HotFocus = new (attribute.Foreground, HotFocus.Background);
                        Normal = new (Normal.Foreground, attribute.Background);
                        HotNormal = new (HotNormal.Foreground, attribute.Background);
                        break;
                }

                break;

            case "Dialog":
                switch (callerMemberName)
                {
                    case "Normal":
                        if (Focus.Background != attribute.Background)
                            Focus = new (attribute.Foreground, Focus.Background);
                        HotNormal = new (HotNormal.Foreground, attribute.Background);
                        break;
                    case "Focus":
                        Normal = new (attribute.Foreground, Normal.Background);
                        HotFocus = new (HotFocus.Foreground, attribute.Background);
                        break;
                    case "HotNormal":
                        if (Focus.Background != attribute.Background)
                            HotFocus = new (attribute.Foreground, HotFocus.Background);
                        if (Normal.Foreground != attribute.Background)
                            Normal = new (Normal.Foreground, attribute.Background);
                        break;
                    case "HotFocus":
                        HotNormal = new (attribute.Foreground, HotNormal.Background);
                        if (Focus.Foreground != attribute.Background)
                            Focus = new (Focus.Foreground, attribute.Background);
                        break;
                }

                break;

            case "Error":
                switch (callerMemberName)
                {
                    case "Normal":
                        HotNormal = new (HotNormal.Foreground, attribute.Background);
                        HotFocus = new (HotFocus.Foreground, attribute.Background);
                        break;
                    case "HotNormal":
                    case "HotFocus":
                        HotFocus = new (attribute.Foreground, attribute.Background);
                        Normal = new (Normal.Foreground, attribute.Background);
                        break;
                }

                break;
        }

        preparingScheme = false;
        return attribute;
    }

    /// <summary>
    /// Compares two <see cref="ColorScheme"/> objects for equality.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>true if the two objects are equal</returns>
    public override bool Equals(object obj) { return Equals(obj as ColorScheme); }

    /// <summary>
    /// Compares two <see cref="ColorScheme"/> objects for equality.
    /// </summary>
    /// <param name="other"></param>
    /// <returns>true if the two objects are equal</returns>
    public bool Equals(ColorScheme other)
    {
        return other != null &&
               EqualityComparer<CellAttributes>.Default.Equals(_normal, other._normal) &&
               EqualityComparer<CellAttributes>.Default.Equals(_focus, other._focus) &&
               EqualityComparer<CellAttributes>.Default.Equals(_hotNormal, other._hotNormal) &&
               EqualityComparer<CellAttributes>.Default.Equals(_hotFocus, other._hotFocus) &&
               EqualityComparer<CellAttributes>.Default.Equals(_disabled, other._disabled);
    }

    /// <summary>
    /// Returns a hashcode for this instance.
    /// </summary>
    /// <returns>hashcode for this instance</returns>
    public override int GetHashCode()
    {
        var hashCode = -1242460230;
        hashCode = hashCode * -1521134295 + _normal.GetHashCode();
        hashCode = hashCode * -1521134295 + _focus.GetHashCode();
        hashCode = hashCode * -1521134295 + _hotNormal.GetHashCode();
        hashCode = hashCode * -1521134295 + _hotFocus.GetHashCode();
        hashCode = hashCode * -1521134295 + _disabled.GetHashCode();
        return hashCode;
    }

    /// <summary>
    /// Compares two <see cref="ColorScheme"/> objects for equality.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns><c>true</c> if the two objects are equivalent</returns>
    public static bool operator ==(ColorScheme left, ColorScheme right)
    {
        return EqualityComparer<ColorScheme>.Default.Equals(left, right);
    }

    /// <summary>
    /// Compares two <see cref="ColorScheme"/> objects for inequality.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns><c>true</c> if the two objects are not equivalent</returns>
    public static bool operator !=(ColorScheme left, ColorScheme right) { return !(left == right); }
}
