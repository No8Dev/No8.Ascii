using System.Diagnostics;
using No8.Ascii;
using No8.Ascii.VirtualTerminal;

Console.WriteLine(Figlet.Render("Allo, World."));

var exCon = ExConsole.Create(options =>
{
    options.StartFullScreen = true;
});
exCon.KeyAvailable += OnConOnKeyAvailable;
exCon.SequenceAvailable += OnConOnSequenceAvailable;
exCon.Pointer += OnConPointer;

var runningTask = exCon.Run();
//
// do
// {
     exCon.Screen.Clear();
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

void OnConPointer(object? obj, PointerEvent pointerEvent)
{
    Trace.WriteLine($"pointer: {pointerEvent}");
//        $"Pointer: {pointerEvent.X}, {pointerEvent.Y} {(pointerEvent.Shift ? "Shft" : "    ")} {(pointerEvent.Alt ? "Alt" : "   ")} {(pointerEvent.Ctrl ? "Ctrl" : "    ")} {pointerEvent.PointerEventType}"); 
}

async void OnConOnKeyAvailable(object? sender, ConsoleKeyInfo key)
{
    var ch = key.KeyChar;
    Trace.WriteLine($"Key pressed: {key.Key}, {key.KeyChar}" 
                    + (key.Modifiers.HasFlag(ConsoleModifiers.Alt) ? ":Alt" : "") 
                    + (key.Modifiers.HasFlag(ConsoleModifiers.Control) ? ":Ctrl" : "") 
                    + (key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? ":Shift" : ""));

    switch (ch)
    {
        case (char)0:
            break;
    
        case '1':
            exCon.FullScreen();
            exCon.Send(Figlet.Render("Play"));
            break;
        case '!':
            exCon.NormalScreen();
            break;
        
        case '2':
            exCon.Mouse.HighlightEnable();
            break;
        case '@':
            exCon.Mouse.HighlightDisable();
            break;
        
        case '3':
            exCon.Send(TerminalSeq.ControlSeq.SelectLocatorEvents(1,3));
            exCon.Send(TerminalSeq.ControlSeq.EnableLocatorReporting(1, 2));
            break;
        case '#':
            exCon.Send(TerminalSeq.ControlSeq.EnableLocatorReporting(0, 0));
            exCon.Send(TerminalSeq.ControlSeq.SelectLocatorEvents(0));
            break;
        
        case '4':
            exCon.Send(TerminalSeq.Device.RequestTermInfoString("u8"));
            break;
        case '$':
            exCon.Send(TerminalSeq.Device.RequestTermInfoStringRaw("u8"));
            break;
        case '5':
            exCon.Send(TerminalSeq.ControlSeq.DeviceStatusReport(5));
            break;
        case '%':
            exCon.Send(TerminalSeq.ControlSeq.DeviceStatusReport(6));
            break;
        case '6':
            exCon.Send(TerminalSeq.ControlSeq.DeviceStatusReportDec(6));    // Cursor position
            break;
        case '^':
            break;
        case '7':
            exCon.Send(TerminalSeq.ControlSeq.EnableLocatorReporting(1, 2));
            break;
        case '&':
            break;
        case '8':
            exCon.Send(TerminalSeq.ControlSeq.DeviceStatusReportDec(53));    // Locator status
            break;
        case '*':
            break;
        case '9':
            exCon.Send(TerminalSeq.ControlSeq.RequestTerminalParameters(""));
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
            exCon.Send(TerminalSeq.ControlSeq.DeviceStatusReport(6)); // Cursor position
            break;
        case 'd':
            exCon.Send("\x1b[1;2'z");
            break;
        
        case 'f':
            exCon.Send(TerminalSeq.ControlSeq.PrivateModeSetDec(1004)); // Send FocusIn/FocusOut events, xterm.
            break;
        case 'F':
            exCon.Send(TerminalSeq.ControlSeq.PrivateResetDec(1004)); // Stop FocusIn/FocusOut events, xterm.
            break;
        
        case 'h':
            TerminalSeq.EditingControlFunctions.EraseInDisplay(2); // Clear
            exCon.Cursor.Set(1, 1);
            TerminalSeq.EditingControlFunctions.EraseInDisplay(0);
            TerminalSeq.ControlSeq.EnableFilterRectangle(10, 1, 20, 80);
            ShowHighlight(10, 1, 20, 80);
            break;
        case 'i':
            // exCon.Send($"Buttons {exCon.TermInfo.BoxChars1}\n");
            break;
        
        case 'm':
            exCon.Send(TerminalSeq.ControlSeq.PrivateModeSetDec(1003)); // Use All Motion Mouse Tracking, xterm. See the section Any-event tracking.
            break;
        case 'M':
            exCon.Send(TerminalSeq.ControlSeq.PrivateResetDec(1003)); // Stop All Motion Mouse Tracking, xterm. See the section Any-event tracking.
            break;
        
        case 'q':
            exCon.Stop();
            break;
        case 'r':
            exCon.Send(TerminalSeq.ControlSeq.SoftTerminalReset);
            break;
        case 's':
            exCon.Send(TerminalSeq.ControlSeq.ClearScreen);
            break;
        case 't':
            //var aa = await exCon.Post(TerminalSeq.ControlSeq.WindowManipulation("11")); // Report xterm window state. 2 = minimized
            //var ab = await exCon.Post(TerminalSeq.ControlSeq.WindowManipulation("13")); // Report xterm window position
            //var ac = await exCon.Post(TerminalSeq.ControlSeq.WindowManipulation("13", "2")); // Report xterm window position
            //var ad = await exCon.Post(TerminalSeq.ControlSeq.WindowManipulation("14")); // Report xterm text area size in pixels. 
            //var ae = await exCon.Post(TerminalSeq.ControlSeq.WindowManipulation("14", "2")); // Report xterm text area size in pixels. 
            //var af = await exCon.Post(TerminalSeq.ControlSeq.WindowManipulation("15")); // Report size of the screen in pixels. 
            //var ag = await exCon.Post(TerminalSeq.ControlSeq.WindowManipulation("16")); // Report xterm character cell size in pixels. 
            var ah = await exCon.Post(TerminalSeq.ControlSeq.WindowManipulation("18")); // Report the size of the text area in characters.
            //var ai = await exCon.Post(TerminalSeq.ControlSeq.WindowManipulation("19")); // Report the size of the screen in characters.
            break;
        case 'w':
            exCon.Send("\x1b[?3h");   // 132 columns
            break;
        case 'x':
            exCon.Send("\x1b[?3l");   // 80 columns
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
    exCon.Send(TerminalSeq.Cursor.Set(top, left)); Console.Write("+");
    exCon.Send(TerminalSeq.Cursor.Set(bottom, left)); Console.Write("+");
    exCon.Send(TerminalSeq.Cursor.Set(top, right)); Console.Write("+");
    exCon.Send(TerminalSeq.Cursor.Set(bottom, right)); Console.Write("+");
}

// MenuItem[] menu =
// {
//     new MenuItem('q', "Quit", () =>  ;
// };

public record MenuItem(char Key, string Description, System.Action func);


