namespace Asciis.App;

/// <summary>
/// Only use System.Console
/// </summary>
public class ConsoleDriverGeneric : ConsoleDriver
{
    public ConsoleDriverGeneric()
    {
        Clipboard = new ClipboardWindows();
    }
    
    public override Encoding     InputEncoding       { get; set; } = Encoding.UTF8;
    public override Encoding     OutputEncoding      { get; set; } = Encoding.UTF8;
    public override ConsoleColor BackgroundColor     { get; set; } = ConsoleColor.Black;
    public override ConsoleColor ForegroundColor     { get; set; } = ConsoleColor.White;
    public override void         GatherInputEvents() {  }
    public override bool         WindowSizeChanged() { return false; }
    public override void         Write(string        str)    {  }
    public override void         WriteConsole(Canvas canvas) { throw new NotImplementedException(); }
}