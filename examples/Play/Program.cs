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
//
// do
// {
     Terminal.Screen.Clear();
     Console.WriteLine(Figlet.Render("Play"));
//
//
//     var key = await con.ReadKey(TimeSpan.FromSeconds(10));
//     if (key is not null)
//     {
//         if (key.Value.KeyChar == 'q')
//             con.Stop();
//     }
//
// } while (con.Alive);
//
await runningTask;      // Good manors
//
//





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
            Console.WriteLine(Figlet.Render("Play"));
            break;
        case '!':
            con.NormalScreen();
            break;
        
        case '2':
            Terminal.Mouse.HighlightEnable();
            break;
        case '@':
            Terminal.Mouse.HighlightDisable();
            break;
        
        case '3':
            con.Send(TerminalSeq.ControlSeq.SelectLocatorEvents(1,3));
            con.Send(TerminalSeq.ControlSeq.EnableLocatorReporting(1, 2));
            break;
        case '#':
            con.Send(TerminalSeq.ControlSeq.EnableLocatorReporting(0, 0));
            con.Send(TerminalSeq.ControlSeq.SelectLocatorEvents(0));
            break;
        
        case '4':
            con.Send(TerminalSeq.Device.RequestTermInfoString("u8"));
            break;
        case '$':
            con.Send(TerminalSeq.Device.RequestTermInfoStringRaw("u8"));
            break;
        case '5':
            con.Send(TerminalSeq.ControlSeq.DeviceStatusReport(5));
            break;
        case '%':
            con.Send(TerminalSeq.ControlSeq.DeviceStatusReport(6));
            break;
        case '6':
            break;
        case '^':
            break;
        case '7':
            break;
        case '&':
            break;
        case '8':
            break;
        case '*':
            break;
        case '9':
            break;
        case '(':
            break;
        case '0':
            break;
        case ')':
            break;
        case 'a':
            break;
        case 'A':
            break;
        
        
        case 'c':
            con.Send(TerminalSeq.ControlSeq.DeviceStatusReport(6)); // Cursor position
            break;
        case 'd':
            con.Send("\x1b[1;2'z");
            break;
        case 'f':
            con.Send(TerminalSeq.ControlSeq.PrivateModeSetDec(1004)); // Send FocusIn/FocusOut events, xterm.
            break;
        case 'h':
            TerminalSeq.EditingControlFunctions.EraseInDisplay(2); // Clear
            Terminal.Cursor.Set(1, 1);
            TerminalSeq.EditingControlFunctions.EraseInDisplay(0);
            TerminalSeq.ControlSeq.EnableFilterRectangle(10, 1, 20, 80);
            ShowHighlight(10, 1, 20, 80);
            break;
        case 'i':
            break;
        case 'm':
            con.Send(TerminalSeq.ControlSeq.PrivateModeSetDec(1003)); // Use All Motion Mouse Tracking, xterm. See the section Any-event tracking.
            break;
        case 'q':
            con.Stop();
            break;
        case 'r':
            con.Send(TerminalSeq.ControlSeq.SoftTerminalReset);
            break;
        case 's':
            con.Send(TerminalSeq.ControlSeq.ClearScreen);
            break;
        case 't':
            con.Send(TerminalSeq.ControlSeq.WindowManipulation("11")); // Report xterm window state. 2 = minimized
            con.Send(TerminalSeq.ControlSeq.WindowManipulation("13")); // Report xterm window position
            con.Send(TerminalSeq.ControlSeq.WindowManipulation("13", "2")); // Report xterm window position
            con.Send(TerminalSeq.ControlSeq.WindowManipulation("14")); // Report xterm text area size in pixels. 
            con.Send(TerminalSeq.ControlSeq.WindowManipulation("14", "2")); // Report xterm text area size in pixels. 
            con.Send(TerminalSeq.ControlSeq.WindowManipulation("15")); // Report size of the screen in pixels. 
            con.Send(TerminalSeq.ControlSeq.WindowManipulation("16")); // Report xterm character cell size in pixels. 
            con.Send(TerminalSeq.ControlSeq.WindowManipulation("18")); // Report the size of the text area in characters.
            con.Send(TerminalSeq.ControlSeq.WindowManipulation("19")); // Report the size of the screen in characters.
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

    switch (conSeq?.Final)
    {
        case 'M':
            break;
    }
}

void ShowHighlight(int top, int left, int bottom, int right)
{
    con.Send(TerminalSeq.Cursor.Set(top, left)); Console.Write("+");
    con.Send(TerminalSeq.Cursor.Set(bottom, left)); Console.Write("+");
    con.Send(TerminalSeq.Cursor.Set(top, right)); Console.Write("+");
    con.Send(TerminalSeq.Cursor.Set(bottom, right)); Console.Write("+");
}

// MenuItem[] menu =
// {
//     new MenuItem('q', "Quit", () =>  ;
// };

public record MenuItem(char Key, string Description, System.Action func);


