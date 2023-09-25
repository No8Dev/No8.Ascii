using No8.Ascii.Platforms;

namespace No8.Ascii;

public class ConsoleDriverCurses : ConsoleDriver
{
    public ConsoleDriverCurses()
    {
        Clipboard     = new ClipboardUnix();
    }
}