using No8.Ascii.Platforms;

namespace No8.Ascii;

/// <summary>
/// Only use System.Console
/// </summary>
public class ConsoleDriverGeneric : ConsoleDriver
{
    public ConsoleDriverGeneric()
    {
        Clipboard = new ClipboardWindows();
    }
}