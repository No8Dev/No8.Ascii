using System.Drawing;
using Asciis.UI.Terminals;

namespace Asciis.UI;

public static class ConsoleHelpers
{

    public static ushort ToTerminalColor(this Color c)
    {
        ushort result = 0x00;

        if (c == Color.DarkRed) return (ushort)CharacterAttributes.FgDarkRed;
        if (c == Color.DarkGreen) return (ushort)CharacterAttributes.FgDarkGreen;
        if (c == Color.DarkBlue) return (ushort)CharacterAttributes.FgDarkBlue;
        if (c == Color.DarkCyan) return (ushort)CharacterAttributes.FgDarkCyan;
        if (c == Color.DarkMagenta) return (ushort)CharacterAttributes.FgDarkMagenta;
        if (c == Color.Gold) return (ushort)CharacterAttributes.FgDarkYellow;
        if (c == Color.Gray) return (ushort)CharacterAttributes.FgGrey;
        if (c == Color.Red) return (ushort)CharacterAttributes.FgRed;
        if (c == Color.Green) return (ushort)CharacterAttributes.FgGreen;
        if (c == Color.Blue) return (ushort)CharacterAttributes.FgBlue;
        if (c == Color.Cyan) return (ushort)CharacterAttributes.FgCyan;
        if (c == Color.Magenta) return (ushort)CharacterAttributes.FgMagenta;
        if (c == Color.Yellow) return (ushort)CharacterAttributes.FgYellow;
        if (c == Color.White) return (ushort)CharacterAttributes.FgWhite;

        if (c.R >= 128) result += 0x04;
        if (c.G >= 128) result += 0x02;
        if (c.B >= 128) result += 0x01;
        if (c.R >= 250 || c.G >= 250 || c.B >= 250)
            result += 0x08; // intense

        return result;
    }

}
