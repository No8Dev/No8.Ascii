using System.Collections.Concurrent;
using System.Diagnostics;
using Asciis.App.DependencyInjection;

namespace Asciis.App;

public abstract class ConsoleDriver
{
    public static ConsoleDriver Create(DependencyInjectionContainer dic)
    {
        dic.Register<ConsoleDriverNoIO>().AsSingleton();
        dic.Register<ConsoleDriverGeneric>().AsSingleton();
        if (OperatingSystem.IsWindows())
            dic.Register<ConsoleDriverWindows>().AsSingleton();
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            dic.Register<ConsoleDriverCurses>().AsSingleton();

        if (UnitTestDetector.IsRunningFromNUnit) 
            return dic.Resolve<ConsoleDriverNoIO>()!;
        if (OperatingSystem.IsWindows()) 
            return dic.Resolve<ConsoleDriverWindows>()!;
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()) 
            return dic.Resolve<ConsoleDriverCurses>()!;

        return dic.Resolve<ConsoleDriverGeneric>()!;
    }


    protected Glyph[]? LastCanvas { get; set; }

    protected const ConsoleColor DefaultBackgroundColor = ConsoleColor.Black;
    protected const ConsoleColor DefaultForegroundColor = ConsoleColor.White;

    public readonly ConcurrentQueue<KeyboardEvent> KbEvents = new();
    public readonly ConcurrentQueue<PointerEvent> PointerEvents = new();

    public  Point           Pointer     { get; protected set; } = Point.Empty;
    public  KeyState[]      Mouse       { get; }                = new KeyState[5];
    public  KeyState[]      VirtualKeys { get; }                = new KeyState[256];

    internal virtual void Shutdown() { }

    public event EventHandler<Size>? TerminalResized;
    public event EventHandler<bool>? WindowFocusChange;
    
    
    /// <summary>
    /// The current number of columns in the terminal.
    /// </summary>
    public int Cols { get; protected set; }

    /// <summary>
    /// The current number of rows in the terminal.
    /// </summary>
    public int Rows { get; protected set; }

    /// <summary>
    /// The current left in the terminal.
    /// </summary>
    public int Left { get; protected set; }

    /// <summary>
    /// The current top in the terminal.
    /// </summary>
    public int Top { get; protected set; }

    public bool       IsFocused            { get; protected set; } = true;
    public Clipboard? Clipboard            { get; protected set; }
    public int        MousePosX            { get; protected set; }
    public int        MousePosY            { get; protected set; }
    public short      MouseWheel           { get; protected set; }
    public short      MouseHorizontalWheel { get; protected set; }

    
    public abstract Encoding InputEncoding { get; set; }
    public abstract Encoding OutputEncoding { get; set; }
    
    public abstract ConsoleColor BackgroundColor { get; set; }
    public abstract ConsoleColor ForegroundColor { get; set; }

    public abstract void GatherInputEvents();
    public abstract bool WindowSizeChanged();
    
    public abstract void Write(string str);
    

    public virtual bool AlwaysSetPosition { get; set; }
    public virtual void Refresh()         { }


    public void AddKeyboardEvent(KeyboardEvent kbEvent)
    {
        Trace.WriteLine(kbEvent);
        KbEvents.Enqueue(kbEvent);
    }
    
    public void AddPointerEvent(PointerEvent pointerEvent)
    {
        Trace.WriteLine(pointerEvent.ToString());
        PointerEvents.Enqueue(pointerEvent);
    }

    public void InvalidateLayout() { }

    public abstract void WriteConsole(Canvas canvas);

    protected bool LineChanged(Canvas canvas, Glyph[]? lastCanvas, in int line)
    {
        if (lastCanvas == null) 
            return true;
        if (!lastCanvas.Length.Equals(canvas.Size)) 
            return true;

        for (int x = 0; x < canvas.Width; x++)
        {
            var nextGlyph = canvas[line, x];
            var lastGlyph = lastCanvas[line * canvas.Width + x];

            if (nextGlyph != lastGlyph)
                return true;
        }

        return false;
    }

    protected virtual void RaiseWindowFocusChange(bool focused) =>
        WindowFocusChange?.Invoke(this, focused);

    protected virtual void RaiseTerminalResized(Size size) => 
        TerminalResized?.Invoke(this, size);

}

