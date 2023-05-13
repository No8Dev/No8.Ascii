using No8.Ascii.Platforms;

namespace No8.Ascii;

public static class ConsoleHelpers
{

    public static ushort ToTerminalColor(this Color c)
    {
        ushort result = 0x00;

        if (c == Color.DarkRed) return (ushort)Windows.CharacterAttributes.FgDarkRed;
        if (c == Color.DarkGreen) return (ushort)Windows.CharacterAttributes.FgDarkGreen;
        if (c == Color.DarkBlue) return (ushort)Windows.CharacterAttributes.FgDarkBlue;
        if (c == Color.DarkCyan) return (ushort)Windows.CharacterAttributes.FgDarkCyan;
        if (c == Color.DarkMagenta) return (ushort)Windows.CharacterAttributes.FgDarkMagenta;
        if (c == Color.Gold) return (ushort)Windows.CharacterAttributes.FgDarkYellow;
        if (c == Color.Gray) return (ushort)Windows.CharacterAttributes.FgGrey;
        if (c == Color.Red) return (ushort)Windows.CharacterAttributes.FgRed;
        if (c == Color.Green) return (ushort)Windows.CharacterAttributes.FgGreen;
        if (c == Color.Blue) return (ushort)Windows.CharacterAttributes.FgBlue;
        if (c == Color.Cyan) return (ushort)Windows.CharacterAttributes.FgCyan;
        if (c == Color.Magenta) return (ushort)Windows.CharacterAttributes.FgMagenta;
        if (c == Color.Yellow) return (ushort)Windows.CharacterAttributes.FgYellow;
        if (c == Color.White) return (ushort)Windows.CharacterAttributes.FgWhite;

        if (c.R >= 128) result += 0x04;
        if (c.G >= 128) result += 0x02;
        if (c.B >= 128) result += 0x01;
        if (c.R >= 250 || c.G >= 250 || c.B >= 250)
            result += 0x08; // intense

        return result;
    }

}
