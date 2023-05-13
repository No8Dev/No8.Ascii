using System.Text;
using No8.Ascii;
using No8.Ascii.VirtualTerminal;

char ch = (char)0;
Console.WriteLine(Figlet.Render("Play"));
Conn.KeyAvailable += OnConOnKeyAvailable;
Conn.SequenceAvailable += OnConOnSequenceAvailable;

while (Conn.Alive)
{
    switch (ch)
    {
    case (char)0:
        break;
    
    case '1':
        Conn.Send("\x1b[?1049h");        // Alt screen buffer
        break;
    case '2':
        Conn.Send("\x1b[?1049l");        // Main screen buffer. This clears the screen
        break;
    case 'c':
        Conn.Send(Terminal.ControlSeq.DeviceStatusReport(6)); // Cursor position
        break;
    case 'd':
        Conn.Send("\x1b[1;2'z");
        break;
    case 'f':
        Conn.Send(Terminal.ControlSeq.PrivateModeSetDec(1004));
        break;
    case 'm':
        Conn.Send(Terminal.ControlSeq.PrivateModeSetDec(1003));
        break;
    case 'r':
        Conn.Send(Terminal.ControlSeq.SoftTerminalReset);
        break;
    case 's':
        Conn.Send(Terminal.ControlSeq.ClearScreen);
        break;
    case 'w':
        Conn.Send("\x1b[?3h");   // 132 columns
        break;
    case 'x':
        Conn.Send("\x1b[?3l");   // 80 columns
        break;
    default:
        Console.WriteLine($"Unknown command: {ch}");
        break;
    }
    ch = (char)0;
    Task.Delay(10);
}

void OnConOnKeyAvailable(object? sender, ConsoleKeyInfo key)
{
    ch = key.KeyChar;
    Console.WriteLine($"Key pressed: {key.Key}, {key.KeyChar}" 
                    + (key.Modifiers.HasFlag(ConsoleModifiers.Alt) ? ":Alt" : "") 
                    + (key.Modifiers.HasFlag(ConsoleModifiers.Control) ? ":Ctrl" : "") 
                    + (key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? ":Shift" : ""));
}

void OnConOnSequenceAvailable(object? sender, string s)
{
    var conSeq = ConSeq.Parse(s);
    Console.WriteLine("Recv: " + conSeq?.ToString() ?? "Unknown sequence");
}
