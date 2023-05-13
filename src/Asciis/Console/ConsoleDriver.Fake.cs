﻿using No8.Ascii.Platforms;

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

    public override Encoding InputEncoding { get; set; } = Encoding.UTF8;
    public override Encoding OutputEncoding { get; set; } = Encoding.UTF8;
    public override ConsoleColor BackgroundColor     { get; set; }
    public override ConsoleColor ForegroundColor     { get; set; }
    public override void         GatherInputEvents() { throw new NotImplementedException(); }
    public override bool         WindowSizeChanged() { throw new NotImplementedException(); }
    public override void         Write(string        str)    { throw new NotImplementedException(); }
    public override void         WriteConsole(Canvas canvas) { throw new NotImplementedException(); }
}