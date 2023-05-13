using System.Text;
using Asciis.UI.Terminals;

namespace Asciis.UI;

public record AKeyEventArgs(long TimeStamp,
                             KeyEventType EventType,
                             char KeyChar,
                             VirtualKeyCode Key,
                             AKeyModifier Modifier,
                             bool Handled = false)

{
    public override string ToString()
    {
        var sb = new StringBuilder();

        if (Modifier != AKeyModifier.None)
        {
            sb.Append("(");
            if (Modifier.HasFlag(AKeyModifier.Alt)) sb.Append(" Alt");
            if (Modifier.HasFlag(AKeyModifier.Shift)) sb.Append(" Shift");
            if (Modifier.HasFlag(AKeyModifier.Control)) sb.Append(" Ctrl");
            sb.Append(" ) ");
        }

        sb.Append($"{Key}");
        if (EventType == KeyEventType.Pressed) sb.Append("[down]");
        if (EventType == KeyEventType.Released) sb.Append("[ up ]");
        if (KeyChar != 0) sb.Append($" -{KeyChar}-");

        return sb.ToString();
    }
}

[Flags]
public enum AKeyModifier
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4
}

public enum KeyEventType
{
    Unknown,
    Pressed,
    Released,
    Key
}
