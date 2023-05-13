namespace Asciis.UI.Terminals;

class Terminal
{
    /// <summary>
    /// https://invisible-island.net/xterm/ctlseqs/ctlseqs.htm
    /// </summary>
    public static class Mode
    {
        public static string ScreenAlt = "\x1b[?1049h";
        public static string ScreenNormal = "\x1b[?1049l";
        public static string SmoothScroll = "\x1b[?4h";
        public static string SmoothScrollOff = "\x1b[?4l";
        public static string ReverseScreen = "\x1b[?5h";
        public static string ReverseScreenOff = "\x1b[?5l";
    }

    public static class Cursor
    {
        public static string Set(int row, int col) =>
            $"\x1b[{row};{col}H";

        public static string Show = "\x1b[?25h";
        public static string Hide = "\x1b[?25l";
    }

    public static class Scroll
    {
        public static string Set(int topRow, int bottomRow) =>
            $"\x1b[{topRow};{bottomRow}r";
        public static string Clear = "\x1b[r";

        public static string Up(int count) =>
            $"\x1b[{count}S";
        public static string Down(int count) =>
            $"\x1b[{count}T";
    }

    public static class Tab
    {
        public static string Set = "\x1bH";
        public static string Clear = "\x1b[g";
        public static string ClearAll = "\x1b[3g";
    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/ANSI_escape_code#SGR
    /// </summary>
    public static class Color
    {
        public static string Fore(int r, int g, int b) =>
            $"\x1b[38;2;{r};{g};{b}m";
        public static string Fore(int color256) =>
            $"\x1b[38;5;{color256}m";
        public static string Fore(Color4Bit color) =>
            $"\x1b[{color + 30}m";
        public static string ForeDefault = "\x1b[39m";
        public static string Back(int r, int g, int b) =>
            $"\x1b[48;2;{r};{g};{b}m";
        public static string Back(int color256) =>
            $"\x1b[48;5;{color256}m";
        public static string Back(Color4Bit color) =>
            $"\x1b[{color + 40}m";
        public static string BackDefault = "\x1b[49m";
        public static string Underline(int r, int g, int b) =>
            $"\x1b[58;2;{r};{g};{b}m";
        public static string UnderlineDefault = "\x1b[59m";
    }

    public enum Color4Bit
    {
        Black = 0,
        Red = 1,
        Green = 2,
        Yellow = 3,
        Blue = 4,
        Magenta = 5,
        Cyan = 6,
        White = 7,
        BrightBlack = 60,
        BrightRed = 61,
        BrightGreen = 62,
        BrightYellow = 63,
        BrightBlue = 64,
        BrightMagenta = 65,
        BrightCyan = 66,
        BrightWhite = 67,
    }

    public static class Graphics
    {
        public static string Reset = "\x1b[0m";  // Reset / Normal
        public static string Bold = "\x1b[1m";  // Bold or increased intensity
        public static string Faint = "\x1b[2m";  // Faint or decreased intensity
        public static string Italic = "\x1b[3m";  // Italic Not widely supported
        public static string Underline = "\x1b[4m";  // Underline
        public static string SlowBlink = "\x1b[5m";  // Slow Blink  less than 150 per minute
        public static string FastBlink = "\x1b[6m";  // Rapid Blink
        public static string Reverse = "\x1b[7m";  // Reverse video
        public static string Conceal = "\x1b[8m";  // Conceal aka Hide
        public static string Strike = "\x1b[9m";  // Crossed-out	aka Strike
        public static string PrimaryFont = "\x1b[10m"; // Primary(default) font
        public static string AltFont11 = "\x1b[11m"; // Alternative font
        public static string DoubleUnderline = "\x1b[21m"; // Doubly underline
        public static string NormalColor = "\x1b[22m"; // Normal color or intensity
        public static string ItalicOff = "\x1b[23m"; // Not italic
        public static string UnderlineOff = "\x1b[24m"; // Underline off
        public static string BlinkOff = "\x1b[25m"; // Blink off
        public static string ReverseOff = "\x1b[27m"; // Reverse off
        public static string ConcealOff = "\x1b[28m"; // Conceal off
        public static string StrikeOff = "\x1b[29m"; // Strike off
        public static string Overlined = "\x1b[53m"; // Overlined
        public static string OverlinedOff = "\x1b[55m"; // Overlined off
        public static string SuperScript = "\x1b[73m"; // superscript
        public static string SubScript = "\x1b[74m"; // subscript
    }

}
