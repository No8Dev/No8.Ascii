using No8.Ascii.Platforms;

namespace No8.Ascii;

/// <summary>
///     No Input or Output.  Can be used in Tests or Graphic apps
/// </summary>
public class ConsoleDriverNoIO : ConsoleDriver
{
    public ConsoleDriverNoIO()
    {
        Clipboard = new ClipboardGeneric();
    }
}