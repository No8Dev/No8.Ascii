using Asciis.App.Platforms;

namespace Asciis.App;

public static class InputHelper
{
    public static KeyboardEvent ToKeyboardEvent(this Windows.KeyEventRecord args, KeyboardEventType eventType)
    {
        // Disable toggle flags
        args.ControlKeyState &= ~Windows.ControlKeyState.CapslockOn;
        args.ControlKeyState &= ~Windows.ControlKeyState.NumlockOn;
        args.ControlKeyState &= ~Windows.ControlKeyState.ScrolllockOn;

        var modifiers = KeyboardModifier.None;

        switch (args.ControlKeyState)
        {
        case Windows.ControlKeyState.RightAltPressed | Windows.ControlKeyState.LeftCtrlPressed | Windows.ControlKeyState.EnhancedKey:
        case Windows.ControlKeyState.EnhancedKey:
            modifiers = KeyboardModifier.Alt | KeyboardModifier.Control;
            break;
        }

        if (args.ControlKeyState.HasFlag(Windows.ControlKeyState.LeftCtrlPressed) || args.ControlKeyState.HasFlag(Windows.ControlKeyState.RightCtrlPressed))
            modifiers |= KeyboardModifier.Control;
        if (args.ControlKeyState.HasFlag(Windows.ControlKeyState.LeftAltPressed) || args.ControlKeyState.HasFlag(Windows.ControlKeyState.RightAltPressed))
            modifiers |= KeyboardModifier.Alt;
        if (args.ControlKeyState.HasFlag(Windows.ControlKeyState.ShiftPressed))
            modifiers |= KeyboardModifier.Shift;

        return new KeyboardEvent(DateTime.Now.ToBinary(), eventType, args.UnicodeChar, args.VirtualKeyCode, modifiers);
    }

    public static KeyboardEvent ToKeyboardEvent(this ConsoleKeyInfo args, KeyboardEventType eventType)
    {
        var modifiers = KeyboardModifier.None;
        if (args.Modifiers.HasFlag(ConsoleModifiers.Alt))
            modifiers |= KeyboardModifier.Alt;
        if (args.Modifiers.HasFlag(ConsoleModifiers.Control))
            modifiers |= KeyboardModifier.Control;
        if (args.Modifiers.HasFlag(ConsoleModifiers.Shift))
            modifiers |= KeyboardModifier.Shift;

        return new KeyboardEvent(DateTime.Now.ToBinary(), eventType, args.KeyChar, (VirtualKeyCode)args.Key, modifiers);
    }

}
