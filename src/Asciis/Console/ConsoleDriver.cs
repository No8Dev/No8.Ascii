using System.Collections.Concurrent;
using System.Diagnostics;
using No8.Ascii.DependencyInjection;
using No8.Ascii.Platforms;
using No8.Ascii.VirtualTerminal;

namespace No8.Ascii;

public abstract class ConsoleDriver
{
    internal object _outLock = new();
    internal object _inLock = new();
    
    public static ConsoleDriver Create(DependencyInjectionContainer dic)
    {
        dic.Register(Current);
        return Current;
    }

    private static ConsoleDriver? _current;
    public static ConsoleDriver Current
    {
        get
        {
            if (_current != null)
                return _current;
            if (OperatingSystem.IsWindows())
                _current = new ConsoleDriverWindows();
            else if (OperatingSystem.IsLinux())
                _current = new ConsoleDriverCurses();
            else if (UnitTestDetector.IsRunningFromNUnit)
                _current = new ConsoleDriverNoIO();
            else
                _current = new ConsoleDriverGeneric();
            return _current;
        }
    }

    protected Glyph[]? LastCanvas { get; set; }

    public Clipboard? Clipboard            { get; protected set; }
    
    public void WriteConsole(Canvas canvas)
    {
        // Glyph by Glyph
        var sb = new StringBuilder(canvas.Width * canvas.Height * 40);
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
        System.Console.Out.Write(sb.ToString());
    }
}

