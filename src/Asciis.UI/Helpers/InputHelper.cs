
using Asciis.UI.Terminals;

namespace Asciis.UI;

public static class InputHelper
{
    public static AKeyEventArgs ToAKeyEventArgs(this KeyEventRecord args, KeyEventType eventType)
    {
        var modifiers = AKeyModifier.None;
        if (args.dwControlKeyState.HasFlag(ControlKeyState.LeftAltPressed) || args.dwControlKeyState.HasFlag(ControlKeyState.RightAltPressed))
            modifiers |= AKeyModifier.Alt;
        if (args.dwControlKeyState.HasFlag(ControlKeyState.LeftCtrlPressed) || args.dwControlKeyState.HasFlag(ControlKeyState.RightCtrlPressed))
            modifiers |= AKeyModifier.Control;
        if (args.dwControlKeyState.HasFlag(ControlKeyState.ShiftPressed))
            modifiers |= AKeyModifier.Shift;

        return new AKeyEventArgs(DateTime.Now.ToBinary(), eventType, args.UnicodeChar, args.wVirtualKeyCode, modifiers);
    }

    public static AKeyEventArgs ToAKeyEventArgs(this ConsoleKeyInfo args, KeyEventType eventType)
    {
        var modifiers = AKeyModifier.None;
        if (args.Modifiers.HasFlag(ConsoleModifiers.Alt))
            modifiers |= AKeyModifier.Alt;
        if (args.Modifiers.HasFlag(ConsoleModifiers.Control))
            modifiers |= AKeyModifier.Control;
        if (args.Modifiers.HasFlag(ConsoleModifiers.Shift))
            modifiers |= AKeyModifier.Shift;

        return new AKeyEventArgs(DateTime.Now.ToBinary(), eventType, args.KeyChar, (VirtualKeyCode)args.Key, modifiers);
    }

}
