namespace Asciis.App;

public record KeyboardEvent(long TimeStamp,
                             KeyboardEventType EventType,
                             char KeyChar,
                             VirtualKeyCode Key,
                             KeyboardModifier Modifier)

{
    public override string ToString()
    {
        var sb = new StringBuilder();

        if (Modifier != KeyboardModifier.None)
        {
            sb.Append("(");
            if (Modifier.HasFlag(KeyboardModifier.Alt)) sb.Append(" Alt");
            if (Modifier.HasFlag(KeyboardModifier.Shift)) sb.Append(" Shift");
            if (Modifier.HasFlag(KeyboardModifier.Control)) sb.Append(" Ctrl");
            sb.Append(" ) ");
        }

        sb.Append($"{Key}");
        if (EventType == KeyboardEventType.Pressed) sb.Append("[down]");
        if (EventType == KeyboardEventType.Released) sb.Append("[ up ]");
        if (KeyChar != 0) sb.Append($" -{KeyChar}-");

        return sb.ToString();
    }
}

[Flags]
public enum KeyboardModifier
{
    None    = 0x00,
    Alt     = 0x01,
    Control = 0x02,
    Shift   = 0x04
}

public enum KeyboardEventType
{
    Unknown,
    Pressed,
    Released,
    Key
}
