using System.Diagnostics;
using No8.Ascii.Platforms;
using No8.Ascii.VirtualTerminal;

namespace No8.Ascii;

/// <summary>
/// Only use System.Console
/// </summary>
public class ConsoleDriverGeneric : ConsoleDriver
{
    public ConsoleDriverGeneric()
    {
        Clipboard = new ClipboardWindows();
        Rows = System.Console.WindowHeight;
        Cols = System.Console.WindowWidth;
    }

    public override Encoding InputEncoding
    {
        get => System.Console.InputEncoding;
        set => System.Console.InputEncoding = value;
    }

    public override Encoding OutputEncoding
    {
        get => System.Console.OutputEncoding;
        set => System.Console.OutputEncoding = value;
    }
    public override ConsoleColor BackgroundColor     { get; set; } = ConsoleColor.Black;
    public override ConsoleColor ForegroundColor     { get; set; } = ConsoleColor.White;
    public override void         GatherInputEvents() {  }
    public override bool         WindowSizeChanged() { return false; }

    public override void Write(string str)
    {
        if (string.IsNullOrEmpty(str))
            return;
        System.Console.Out.Write(str);
    }

    public override void WriteConsole(Canvas canvas)
    {
        // Glyph by Glyph
        var sb = new StringBuilder(canvas.Width * canvas.Height * 40);      // possible size of string 
        Color? lastForeground = null;
        Color? lastBackground = null;
        int lastIndex = -2;
        int count = 0;

        if (LastCanvas == null || LastCanvas.Length != (canvas.Width * canvas.Height))
            LastCanvas = new Glyph[canvas.Width * canvas.Height];

        for (int y = 0; y < canvas.Height; y++)
        {
            for (int x = 0; x < canvas.Width; x++)
            {
                var index = y * canvas.Width + x;
                var chr = canvas[y, x];
                var lastChr = LastCanvas[index.Clamp(0, LastCanvas.Length - 1)];

                if (lastChr != chr)
                {
                    count++;

                    if (index != lastIndex + 1)
                        sb.Append(TerminalSeq.Cursor.Set(y + 1, x + 1));
                    lastIndex = index;

                    if (chr.Fore != lastForeground && chr.Fore != null)
                    {
                        sb.Append(TerminalSeq.Color.Fore(chr.Fore.Value));
                        lastForeground = chr.Fore;
                    }
                    if (chr.Back != lastBackground && chr.Back != null)
                    {
                        sb.Append(TerminalSeq.Color.Back(chr.Back.Value.R, chr.Back.Value.G, chr.Back.Value.B));
                        lastBackground = chr.Back;
                    }
                    sb.Append(chr.Chr);

                    LastCanvas[index] = (Glyph)chr.Clone();
                }
            }
        }
        Write(sb.ToString());
        Debug.WriteLine($"ConsoleChanged( {count} )");
    }
}