// <summary>
//      Models used across the library 
// </summary>

namespace Asciis.App;

public struct KeyState
{
    public bool IsPressed;
    public bool IsToggled; // e.g. CAPS_LOCK

    public override string ToString() =>
        $"({(IsPressed ? "Pressed" : "")},{(IsToggled ? "ON " : "off")})";
}

