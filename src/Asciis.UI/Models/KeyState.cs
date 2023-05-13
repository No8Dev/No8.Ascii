namespace Asciis.UI;

public struct KeyState
{
    public bool IsPressed;
    public bool IsToggled;  // e.g. CAPS_LOCK

    public override string ToString() =>
        $"({(IsPressed ? "Pressed" : "")},{(IsToggled ? "ON " : "off")})";
}
