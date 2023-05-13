using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Asciis.UI.Terminals;
using Microsoft.Extensions.Logging;

namespace Asciis.UI.Engine;

#if _WINDOWS
public partial class AsciiEngine
{
    private IntPtr _outputHandle;
    private IntPtr _inputHandle;

    private void Init()
    {
        _inputHandle = Windows.GetStdHandle(StandardHandle.Input);
        _outputHandle = Windows.GetStdHandle(StandardHandle.Output);

        Windows.GetConsoleInputMode(_inputHandle, out var inputModes);
        inputModes |= ConsoleInputModes.EnableMouseInput;
        inputModes |= ConsoleInputModes.EnableWindowInput;
        inputModes |= ConsoleInputModes.EnableExtendedFlags;
        inputModes |= ConsoleInputModes.EnableVirtualTerminalInput;
        inputModes &= ~ConsoleInputModes.EnableQuickEditMode;
        Windows.SetConsoleInputMode(_inputHandle, inputModes);

        Windows.GetConsoleOutputMode(_outputHandle, out var outputMode);
        outputMode &= ~ConsoleOutputModes.EnableWrapAtEolOutput;
        outputMode |= ConsoleOutputModes.EnableVirtualTerminalProcessing;
        outputMode |= ConsoleOutputModes.DisableNewlineAutoReturn;
        outputMode |= ConsoleOutputModes.EnableLvbGridWorldwide;
        Windows.SetConsoleOutputMode(_outputHandle, outputMode);

        for (var i = 0; i < 256; i++)
            VirtualKeys[i].IsPressed = VirtualKeys[i].IsToggled = false;
        Windows.GetKeyboardState(_keyOldState);
    }

    public void ProcessInputEvents()
    {
        Windows.GetNumberOfConsoleInputEvents(_inputHandle, out var events);
        if (events > 0)
        {
            var inBuf = new InputRecord[events];
            if (!Windows.ReadConsoleInput(_inputHandle, inBuf, events, out var actualEvents))
            {
                Error("ReadConsoleInput");
            }
            if (actualEvents < events)
                inBuf = inBuf.Take((int)actualEvents).ToArray();

            ProcessVirtualKeyboardState();

            ProcessInput(inBuf);
        }
    }

    private static readonly VirtualKeyCode[] CanToggleKey =
    {
            VirtualKeyCode.VK_NUMLOCK,
            VirtualKeyCode.VK_CAPITAL
        };

    private void ProcessVirtualKeyboardState()
    {
        var changed = false;

        Windows.GetKeyboardState(_keyNewState);

        // KEYBOARD
        for (var i = 0; i < 256; i++)
        {
            VirtualKeys[i].IsPressed = false;

            if (_keyNewState[i] != _keyOldState[i])
            {
                changed = true;
                if ((_keyNewState[i] & 0x80) != 0x00)
                    VirtualKeys[i].IsPressed = true;
                else
                    VirtualKeys[i].IsPressed = false;
                VirtualKeys[i].IsToggled = (_keyNewState[i] & 0x01) != 0x00;
            }

            _keyOldState[i] = _keyNewState[i];
        }
        if (changed)
        {
#if DEBUG
            var sb = new StringBuilder("VirtualKey: ");
            for (int i = 0; i < 256; i++)
            {
                var vk = (VirtualKeyCode)i;
                var key = VirtualKeys[i];
                var state = _keyOldState[i];
                if (key.IsPressed || key.IsToggled)
                {
                    var list = new List<string> { $"0x{state:X4}" };
                    if (key.IsPressed) list.Add("Pressed");
                    if (key.IsToggled && CanToggleKey.Contains(vk)) list.Add("Toggled");
                    sb.Append($"{vk} {string.Join(' ', list)} => ");
                }
            }
            _logger.LogDebug(sb.ToString());
#endif
        }
    }

    private static bool IsModKey(KeyEventRecord ir)
    {
        // We should also skip over Shift, Control, and Alt, as well as caps lock.
        // Apparently we don't need to check for 0xA0 through 0xA5, which are keys like
        // Left Control & Right Control. See the ConsoleKey enum for these values.
        var keyCode = ir.wVirtualKeyCode;
        return (keyCode == VirtualKeyCode.VK_SHIFT
              || keyCode == VirtualKeyCode.VK_CONTROL
              || keyCode == VirtualKeyCode.VK_MENU
              || keyCode == VirtualKeyCode.VK_CAPITAL
              || keyCode == VirtualKeyCode.VK_NUMLOCK
              || keyCode == VirtualKeyCode.VK_SCROLL);
    }


    private void ProcessInput(InputRecord[] inBuf)
    {
        foreach (var input in inBuf)
        {
            switch (input.EventType)
            {
                case EventType.Key:
                    if (input.KeyEvent.bKeyDown)
                        HandleKeyEvent(input.KeyEvent.ToAKeyEventArgs(KeyEventType.Pressed));
                    else
                    {
                        HandleKeyEvent(input.KeyEvent.ToAKeyEventArgs(KeyEventType.Released));
                        if (!IsModKey(input.KeyEvent))
                            HandleKeyEvent(input.KeyEvent.ToAKeyEventArgs(KeyEventType.Key));
                    }

                    break;

                case EventType.Focus:
                    IsFocused = input.FocusEvent.SetFocus > 0;
                    _logger?.LogTrace("IsFocused {IsFocused}", IsFocused);

                    RaiseWindowFocusChange(IsFocused);
                    break;

                case EventType.Menu:
                    var menu = input.MenuEvent.CommandId;
                    _logger?.LogTrace("Menu CommandId: {menu:X8}", menu);
                    break;

                case EventType.Mouse:
                    ProcessMouseEvent(input.MouseEvent);
                    break;

                case EventType.WindowBufferSize:
                    var size = input.WindowBufferSizeEvent;
                    InvalidateLayout();
                    _logger?.LogTrace("WindowBufferSize Event {size}", size);
                    break;
            }
        }
    }

    private void ProcessMouseEvent(MouseEventRecord mouseEvent)
    {
#if FALSE
            _logger.LogTrace(mouseEvent.ToString());
#endif

        bool mouseMoved = MousePosX != mouseEvent.MousePosition.X ||
                          MousePosY != mouseEvent.MousePosition.Y;
        MousePosX = mouseEvent.MousePosition.X;
        MousePosY = mouseEvent.MousePosition.Y;

        // Flag will only have a single value
        switch (mouseEvent.Flags)
        {
            case MouseEventFlags.MouseMoved:
                if (mouseMoved)
                    HandlePointerEvent(
                                   new PointerEventArgs(
                                                        DateTime.Now.ToBinary(),
                                                        PointerEventType.Move,
                                                        MousePosX,
                                                        MousePosY));
                break;

            case MouseEventFlags.DoubleClick:
                for (int buttonId = 0; buttonId < 5; buttonId++)
                {
                    if (((ushort)mouseEvent.ButtonState & (1 << buttonId)) > 0)
                    {
                        HandlePointerEvent(
                                           new PointerEventArgs(
                                                                DateTime.Now.ToBinary(),
                                                                PointerEventType.DoubleClick,
                                                                MousePosX,
                                                                MousePosY,
                                                                buttonId));
                    }
                }
                break;

            case MouseEventFlags.MouseWheeled:
                MouseWheel = (short)((uint)mouseEvent.ButtonState >> 16);
                HandlePointerEvent(
                                   new PointerEventArgs(
                                                        DateTime.Now.ToBinary(),
                                                        PointerEventType.Wheel,
                                                        MousePosX,
                                                        MousePosY,
                                                        Value: MouseWheel
                                                       ));
                break;

            case MouseEventFlags.MouseHorizontalWheeled:
                MouseHorizontalWheel = (short)((uint)mouseEvent.ButtonState >> 16);
                HandlePointerEvent(
                                   new PointerEventArgs(
                                                        DateTime.Now.ToBinary(),
                                                        PointerEventType.HorizontalWheel,
                                                        MousePosX,
                                                        MousePosY,
                                                        Value: MouseHorizontalWheel
                                                       ));
                break;

            case 0:
                // mouse state or key state change
                // populate new state first
                for (int buttonId = 0; buttonId < 5; buttonId++)
                    _mouseNewState[buttonId] = ((ushort)mouseEvent.ButtonState & (1 << buttonId)) > 0;

                // What has changed
                for (int buttonId = 0; buttonId < 5; buttonId++)
                {
                    if (_mouseNewState[buttonId] != _mouseOldState[buttonId])
                    {
                        if (_mouseNewState[buttonId])
                        {
                            if (!Mouse[buttonId].IsPressed)
                                HandlePointerEvent(
                                                   new PointerEventArgs(
                                                                        DateTime.Now.ToBinary(),
                                                                        PointerEventType.Pressed,
                                                                        MousePosX,
                                                                        MousePosY,
                                                                        buttonId));
                        }
                        else
                        {
                            if (Mouse[buttonId].IsPressed)
                            {
                                HandlePointerEvent(
                                                   new PointerEventArgs(
                                                                        DateTime.Now.ToBinary(),
                                                                        PointerEventType.Released,
                                                                        MousePosX,
                                                                        MousePosY,
                                                                        buttonId));
                                HandlePointerEvent(
                                                   new PointerEventArgs(
                                                                        DateTime.Now.ToBinary(),
                                                                        PointerEventType.Click,
                                                                        MousePosX,
                                                                        MousePosY,
                                                                        buttonId));
                            }
                        }
                        Mouse[buttonId].IsPressed = _mouseNewState[buttonId];
                        _mouseOldState[buttonId] = _mouseNewState[buttonId];
                    }
                }

                break;
        }
    }

    private void ProcessSelection()
    {
        if (!Windows.GetConsoleSelectionInfo(out var selectionInfo))
            return;

        if (selectionInfo.Flags.HasFlag(ConsoleSelectionFlags.ConsoleSelectionNotEmpty))
            Selection = selectionInfo.Selection;
        else
            Selection = SmallRect.Empty;
    }

    public void WriteConsole(bool force)
    {
        if (!force && !_invalidateConsole)
            return;

        _invalidateConsole = false;
        //_logger.LogTrace("Windows.WriteConsoleOutput({Width}, {Height})", MergedCanvas.Width, MergedCanvas.Height);

        MergeScreens();
#if false

            var buffer = new TerminalChar[ Canvas.Height, Canvas.Width ];

            for (int y = 0; y < Canvas.Height; y++)
            {
                for (int x = 0; x < Canvas.Width; x++)
                {
                    var glyph = Canvas.Glyphs[y, x];
                    buffer[y, x].UnicodeChar = glyph.Chr;
                    buffer[y, x].Attributes = ColorScheme.Attributes( glyph.Fore, glyph.Back );
                }
            }
            var rectWindow = new SmallRect(0, 0, (short)(Canvas.Width-1), (short)(Canvas.Height-1));
            Windows.WriteConsoleOutput( _outputHandle, buffer, new Coord( Canvas.Width, Canvas.Height ), Coord.Zero, ref rectWindow );
#else
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;

        var sb = new StringBuilder();
        Color? lastForeground = null;
        Color? lastBackground = null;
        for (int y = 0; y < MergedCanvas.Height; y++)
        {
            if (!LineChanged(MergedCanvas, LastCanvas, y))
                continue;

            sb.Append(Terminal.Cursor.Set(y + 1, 1));
            for (int x = 0; x < MergedCanvas.Width; x++)
            {
                var tc = MergedCanvas.Glyphs[y, x];
                if (tc.Fore != lastForeground)
                {
                    sb.Append(Terminal.Color.Fore(tc.Fore.R, tc.Fore.G, tc.Fore.B));
                    lastForeground = tc.Fore;
                }
                if (tc.Back != lastBackground)
                {
                    sb.Append(Terminal.Color.Back(tc.Back.R, tc.Back.G, tc.Back.B));
                    lastBackground = tc.Back;
                }
                sb.Append(tc.Chr);
            }
        }
        Console.Write(sb.ToString());
#endif
    }

    private bool LineChanged(Canvas canvas, Array2D<Glyph>? lastCanvas, in int line)
    {
        if (lastCanvas == null) return true;
        if (lastCanvas.Size != canvas.Glyphs.Size) return true;

        for (int x = 0; x < canvas.Width; x++)
        {
            var nextGlyph = canvas.Glyphs[line, x];
            var lastGlyph = lastCanvas[line, x];

            if (nextGlyph != lastGlyph)
                return true;
        }

        return false;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private uint Error(string msg)
    {
        var lastError = (uint)Marshal.GetLastWin32Error();
        var sb = new StringBuilder(1024);
        Windows.FormatMessage((int)FormatMessageFlags.FormatMessageFromSystem,
                               IntPtr.Zero,
                               lastError,
                               0,
                               sb,
                               (uint)sb.Capacity,
                               null);
        _logger?.LogError("ERROR: {msg}\n\t{sb}\n", msg, sb);

        return lastError;
    }

    public bool SetFontSize(string filename, string faceName, short fontWidth, short fontHeight)
    {
        var result = Windows.AddFontResourceEx(filename, 0, IntPtr.Zero);
        if (result != 1)
        {
            Error("AddFontResourceEx");

            return false;
        }

        var fontInfo = new ConsoleFontInfoEx
        {
            cbSize = (uint)Marshal.SizeOf<ConsoleFontInfoEx>(),
            Font = 0,
            FontSize = new Coord(fontWidth, fontHeight),
            FontFamily = FontPitchAndFamily.FfModern,
            FontWeight = FontWeight.Normal,
            FaceName = faceName
        };
        if (!Windows.SetCurrentConsoleFontEx(_outputHandle, false, ref fontInfo))
        {
            Error("SetCurrentConsoleFontEx");

            return false;
        }

        return true;
    }

    private void BeginResize(int width, int height)
    {
        Console.SetCursorPosition(0, 0);

        var csbi = ConsoleScreenBufferInfoEx.Create();
        if (!Windows.GetConsoleScreenBufferInfoEx(_outputHandle, ref csbi))
        {
            Error("GetConsoleScreenBufferInfoEx");
        }

        var currentWidth = (short)(csbi.Window.Right - csbi.Window.Left + 1);
        var currentHeight = (short)(csbi.Window.Bottom - csbi.Window.Top + 1);

        csbi.Window = new SmallRect(0, 0, (short)(width - 1), (short)(height - 1));
        Windows.SetConsoleWindowInfo(_outputHandle, true, ref csbi.Window);
        if (!Windows.SetConsoleScreenBufferSize(_outputHandle, new Coord(width, height)))
        {
            Error("SetConsoleScreenBufferSize");
        }

        if (width != currentWidth || height != currentHeight)
        {
            csbi.Window = new SmallRect(0, 0, (short)(width - 1), (short)(height - 1));
            if (!Windows.SetConsoleWindowInfo(_outputHandle, true, ref csbi.Window))
            {
                Error("SetConsoleWindowInfo");
            }
        }

        //UpdateColorScheme();
    }

    //private void UpdateColorScheme()
    //{
    //    var screenBuffer = ConsoleScreenBufferInfoEx.Create();

    //    if (!Windows.GetConsoleScreenBufferInfoEx(_outputHandle, ref screenBuffer))
    //        Error("GetConsoleScreenBufferInfoEx");

    //    for (int i = 0; i < 16; i++)
    //        screenBuffer.ColorTable[i] = _colorScheme.Colors[i];

    //    screenBuffer.Window.Bottom++; // Windows " Bug "
    //    screenBuffer.Window.Right++;
    //    if (!Windows.SetConsoleScreenBufferInfoEx(_outputHandle, screenBuffer))
    //        Error("SetConsoleScreenBufferInfoEx");
    //}

}
#endif
