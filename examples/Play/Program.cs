using No8.Ascii;
using No8.Ascii.VirtualTerminal;

Console.WriteLine(Figlet.Render("Allo, World."));

var con = ExConsole.Create(options =>
{
    options.StartFullScreen = true;
});
con.KeyAvailable += OnConOnKeyAvailable;
con.SequenceAvailable += OnConOnSequenceAvailable;

var runningTask = con.Run();

Console.WriteLine(Figlet.Render("Play"));

await runningTask;

// con.Run();
//
// Console.WriteLine(Figlet.Render("Play"));
// while (con.Alive)
// {
//     await Task.Delay(10);
// }


void OnConOnKeyAvailable(object? sender, ConsoleKeyInfo key)
{
    var ch = key.KeyChar;
    Console.WriteLine($"Key pressed: {key.Key}, {key.KeyChar}" 
                    + (key.Modifiers.HasFlag(ConsoleModifiers.Alt) ? ":Alt" : "") 
                    + (key.Modifiers.HasFlag(ConsoleModifiers.Control) ? ":Ctrl" : "") 
                    + (key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? ":Shift" : ""));

    switch (ch)
    {
        case (char)0:
            break;
    
        case '1':
            con.FullScreen();
            break;
        case '2':
            con.NormalScreen();
            break;
        case 'c':
            con.Send(Terminal.ControlSeq.DeviceStatusReport(6)); // Cursor position
            break;
        case 'd':
            con.Send("\x1b[1;2'z");
            break;
        case 'f':
            con.Send(Terminal.ControlSeq.PrivateModeSetDec(1004)); // Send FocusIn/FocusOut events, xterm.
            break;
        case 'm':
            con.Send(Terminal.ControlSeq.PrivateModeSetDec(1003)); // Use All Motion Mouse Tracking, xterm. See the section Any-event tracking.
            break;
        case 'q':
            con.Stop();
            break;
        case 'r':
            con.Send(Terminal.ControlSeq.SoftTerminalReset);
            break;
        case 's':
            con.Send(Terminal.ControlSeq.ClearScreen);
            break;
        case 'w':
            con.Send("\x1b[?3h");   // 132 columns
            break;
        case 'x':
            con.Send("\x1b[?3l");   // 80 columns
            break;
        default:
            Console.WriteLine($"Unknown command: {ch}");
            break;
    }
}

void OnConOnSequenceAvailable(object? sender, string s)
{
    var conSeq = ConSeq.Parse(s);
    Console.WriteLine("Recv: " + conSeq?.ToString() ?? "Unknown sequence");
}
