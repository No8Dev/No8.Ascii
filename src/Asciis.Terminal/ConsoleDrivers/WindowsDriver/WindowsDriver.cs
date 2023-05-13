using System.Threading.Tasks;
using Asciis.Terminal.Core.Clipboard;
using Asciis.Terminal.Helpers;
using static Asciis.Terminal.ConsoleDrivers.WindowsConsole;

namespace Asciis.Terminal.ConsoleDrivers;
internal class WindowsDriver : ConsoleDriver
{
    private static bool sync = false;
    private WindowsConsole.CharInfo[] OutputBuffer = new WindowsConsole.CharInfo[0];
    private int cols, rows, left, top;
    private WindowsConsole.SmallRect damageRegion;
    private IClipboard clipboard;
    private CellValue[,] contents = new CellValue[0,0];

    public override int Cols => cols;
    public override int Rows => rows;
    public override int Left => left;
    public override int Top => top;
    public override bool HeightAsBuffer { get; set; }
    public override IClipboard Clipboard => clipboard;
    internal override CellValue[,] Contents => contents;

    public WindowsConsole WinConsole { get; private set; }

    private Action<KeyEvent>? keyHandler;
    private Action<KeyEvent>? keyDownHandler;
    private Action<KeyEvent>? keyUpHandler;
    private Action<MouseEvent>? mouseHandler;

    public WindowsDriver()
    {
        WinConsole = new WindowsConsole();
        clipboard = new WindowsClipboard();
    }

    public override void PrepareToRun(
        MainLoop mainLoop,
        Action<KeyEvent> keyHandler,
        Action<KeyEvent> keyDownHandler,
        Action<KeyEvent> keyUpHandler,
        Action<MouseEvent> mouseHandler)
    {
        this.keyHandler = keyHandler;
        this.keyDownHandler = keyDownHandler;
        this.keyUpHandler = keyUpHandler;
        this.mouseHandler = mouseHandler;

        if (mainLoop.Driver is WindowsMainLoop mLoop)
        {
            mLoop.ProcessInput = (e) => ProcessInput(e);
            mLoop.WinChanged = (e) => ChangeWin(e);
        }
    }

    private void ChangeWin(Size e)
    {
        if (!HeightAsBuffer)
        {
            var w = e.Width;
            if (w == cols - 3 && e.Height < rows) w += 3;
            var newSize = WinConsole.SetConsoleWindow(
                (short)Math.Max(w, 16),
                (short)e.Height);
            left = 0;
            top = 0;
            cols = newSize.Width;
            rows = newSize.Height;
            ResizeScreen();
            UpdateOffScreen();
            TerminalResized?.Invoke();
        }
    }

    private void ProcessInput(WindowsConsole.InputRecord inputEvent)
    {
        switch (inputEvent.EventType)
        {
            case WindowsConsole.EventType.Key:
                var map = MapKey(ToConsoleKeyInfoEx(inputEvent.KeyEvent));
                if (map == (Key)0xffffffff)
                {
                    KeyEvent key = new();

                    // Shift = VK_SHIFT = 0x10
                    // Ctrl = VK_CONTROL = 0x11
                    // Alt = VK_MENU = 0x12

                    if (inputEvent.KeyEvent.dwControlKeyState.HasFlag(WindowsConsole.ControlKeyState.CapslockOn))
                        inputEvent.KeyEvent.dwControlKeyState &= ~WindowsConsole.ControlKeyState.CapslockOn;

                    if (inputEvent.KeyEvent.dwControlKeyState.HasFlag(WindowsConsole.ControlKeyState.ScrolllockOn))
                        inputEvent.KeyEvent.dwControlKeyState &= ~WindowsConsole.ControlKeyState.ScrolllockOn;

                    if (inputEvent.KeyEvent.dwControlKeyState.HasFlag(WindowsConsole.ControlKeyState.NumlockOn))
                        inputEvent.KeyEvent.dwControlKeyState &= ~WindowsConsole.ControlKeyState.NumlockOn;

                    switch (inputEvent.KeyEvent.dwControlKeyState)
                    {
                        case WindowsConsole.ControlKeyState.RightAltPressed:
                        case WindowsConsole.ControlKeyState.RightAltPressed |
                         WindowsConsole.ControlKeyState.LeftControlPressed |
                         WindowsConsole.ControlKeyState.EnhancedKey:
                        case WindowsConsole.ControlKeyState.EnhancedKey:
                            key = new KeyEvent(Key.CtrlMask | Key.AltMask, keyModifiers);
                            break;
                        case WindowsConsole.ControlKeyState.LeftAltPressed:
                            key = new KeyEvent(Key.AltMask, keyModifiers);
                            break;
                        case WindowsConsole.ControlKeyState.RightControlPressed:
                        case WindowsConsole.ControlKeyState.LeftControlPressed:
                            key = new KeyEvent(Key.CtrlMask, keyModifiers);
                            break;
                        case WindowsConsole.ControlKeyState.ShiftPressed:
                            key = new KeyEvent(Key.ShiftMask, keyModifiers);
                            break;
                        case WindowsConsole.ControlKeyState.NumlockOn:
                            break;
                        case WindowsConsole.ControlKeyState.ScrolllockOn:
                            break;
                        case WindowsConsole.ControlKeyState.CapslockOn:
                            break;
                        default:
                            switch (inputEvent.KeyEvent.wVirtualKeyCode)
                            {
                                case 0x10:
                                    key = new KeyEvent(Key.ShiftMask, keyModifiers);
                                    break;
                                case 0x11:
                                    key = new KeyEvent(Key.CtrlMask, keyModifiers);
                                    break;
                                case 0x12:
                                    key = new KeyEvent(Key.AltMask, keyModifiers);
                                    break;
                                default:
                                    key = new KeyEvent(Key.Unknown, keyModifiers);
                                    break;
                            }

                            break;
                    }

                    if (inputEvent.KeyEvent.bKeyDown)
                        keyDownHandler?.Invoke(key);
                    else
                        keyUpHandler?.Invoke(key);
                }
                else
                {
                    if (inputEvent.KeyEvent.bKeyDown)
                    {
                        // Key Down - Fire KeyDown Event and KeyStroke (ProcessKey) Event
                        keyDownHandler?.Invoke(new KeyEvent(map, keyModifiers));
                        keyHandler?.Invoke(new KeyEvent(map, keyModifiers));
                    }
                    else
                    {
                        keyUpHandler?.Invoke(new KeyEvent(map, keyModifiers));
                    }
                }

                if (!inputEvent.KeyEvent.bKeyDown)
                    keyModifiers.Clear();
                break;

            case WindowsConsole.EventType.Mouse:
                var me = ToDriverMouse(inputEvent.MouseEvent);
                mouseHandler?.Invoke(me);
                if (processButtonClick)
                {
                    mouseHandler?.Invoke(
                        new MouseEvent()
                        {
                            X = me.X,
                            Y = me.Y,
                            Flags = ProcessButtonClick(inputEvent.MouseEvent)
                        });
                    processButtonClick = false;
                }

                break;

            case WindowsConsole.EventType.WindowBufferSize:
                if (HeightAsBuffer)
                {
                    var winSize = WinConsole.GetConsoleBufferWindow(out var pos);
                    left = pos.X;
                    top = pos.Y;
                    cols = inputEvent.WindowBufferSizeEvent.size.X;
                    rows = inputEvent.WindowBufferSizeEvent.size.Y;
                    //System.Diagnostics.Debug.WriteLine ($"{HeightAsBuffer},{cols},{rows}");
                    ResizeScreen();
                    UpdateOffScreen();
                    TerminalResized?.Invoke();
                }

                break;

            case WindowsConsole.EventType.Focus:
                break;
        }
    }

    private WindowsConsole.ButtonState? lastMouseButtonPressed = null;
    private bool isButtonPressed = false;
    private bool isButtonReleased = false;
    private bool isButtonDoubleClicked = false;
    private Point point;
    private int buttonPressedCount;
    private bool isOneFingerDoubleClicked = false;
    private bool processButtonClick;

    private MouseEvent ToDriverMouse(WindowsConsole.MouseEventRecord mouseEvent)
    {
        var mouseFlag = MouseFlags.AllEvents;

        //System.Diagnostics.Debug.WriteLine ($"ButtonState: {mouseEvent.ButtonState};EventFlags: {mouseEvent.EventFlags}");

        if (isButtonDoubleClicked || isOneFingerDoubleClicked)
            Application.MainLoop.AddIdle(
                () =>
                {
                    Task.Run(async () => await ProcessButtonDoubleClickedAsync());
                    return false;
                });

        // The ButtonState member of the MouseEvent structure has bit corresponding to each mouse button.
        // This will tell when a mouse button is pressed. When the button is released this event will
        // be fired with it's bit set to 0. So when the button is up ButtonState will be 0.
        // To map to the correct driver events we save the last pressed mouse button so we can
        // map to the correct clicked event.
        if ((lastMouseButtonPressed != null || isButtonReleased) && mouseEvent.ButtonState != 0)
        {
            lastMouseButtonPressed = null;
            //isButtonPressed = false;
            isButtonReleased = false;
        }

        var p = new Point()
        {
            X = mouseEvent.MousePosition.X,
            Y = mouseEvent.MousePosition.Y
        };

        if (!isButtonPressed && buttonPressedCount < 2
                             && mouseEvent.EventFlags == WindowsConsole.EventFlags.MouseMoved
                             && (mouseEvent.ButtonState == WindowsConsole.ButtonState.Button1Pressed
                                 || mouseEvent.ButtonState == WindowsConsole.ButtonState.Button2Pressed
                                 || mouseEvent.ButtonState == WindowsConsole.ButtonState.Button3Pressed))
        {
            lastMouseButtonPressed = mouseEvent.ButtonState;
            buttonPressedCount++;
        }
        else if (!isButtonPressed && buttonPressedCount > 0 && mouseEvent.ButtonState == 0
                 && mouseEvent.EventFlags == 0)
        {
            buttonPressedCount++;
        }
        //System.Diagnostics.Debug.WriteLine ($"isButtonPressed: {isButtonPressed};buttonPressedCount: {buttonPressedCount};lastMouseButtonPressed: {lastMouseButtonPressed}");
        //System.Diagnostics.Debug.WriteLine ($"isOneFingerDoubleClicked: {isOneFingerDoubleClicked}");

        if (buttonPressedCount == 1 && lastMouseButtonPressed != null
                                    && lastMouseButtonPressed == WindowsConsole.ButtonState.Button1Pressed
            || lastMouseButtonPressed == WindowsConsole.ButtonState.Button2Pressed
            || lastMouseButtonPressed == WindowsConsole.ButtonState.Button3Pressed)
        {
            switch (lastMouseButtonPressed)
            {
                case WindowsConsole.ButtonState.Button1Pressed:
                    mouseFlag = MouseFlags.Button1DoubleClicked;
                    break;

                case WindowsConsole.ButtonState.Button2Pressed:
                    mouseFlag = MouseFlags.Button2DoubleClicked;
                    break;

                case WindowsConsole.ButtonState.Button3Pressed:
                    mouseFlag = MouseFlags.Button3DoubleClicked;
                    break;
            }

            isOneFingerDoubleClicked = true;
        }
        else if (buttonPressedCount == 3 && lastMouseButtonPressed != null && isOneFingerDoubleClicked
                 && lastMouseButtonPressed == WindowsConsole.ButtonState.Button1Pressed
                 || lastMouseButtonPressed == WindowsConsole.ButtonState.Button2Pressed
                 || lastMouseButtonPressed == WindowsConsole.ButtonState.Button3Pressed)
        {
            switch (lastMouseButtonPressed)
            {
                case WindowsConsole.ButtonState.Button1Pressed:
                    mouseFlag = MouseFlags.Button1TripleClicked;
                    break;

                case WindowsConsole.ButtonState.Button2Pressed:
                    mouseFlag = MouseFlags.Button2TripleClicked;
                    break;

                case WindowsConsole.ButtonState.Button3Pressed:
                    mouseFlag = MouseFlags.Button3TripleClicked;
                    break;
            }

            buttonPressedCount = 0;
            lastMouseButtonPressed = null;
            isOneFingerDoubleClicked = false;
            isButtonReleased = false;
        }
        else if (mouseEvent.ButtonState != 0 && mouseEvent.EventFlags == 0 && lastMouseButtonPressed == null &&
                 !isButtonDoubleClicked ||
                 lastMouseButtonPressed == null && mouseEvent.EventFlags == WindowsConsole.EventFlags.MouseMoved &&
                 mouseEvent.ButtonState != 0 && !isButtonReleased && !isButtonDoubleClicked)
        {
            switch (mouseEvent.ButtonState)
            {
                case WindowsConsole.ButtonState.Button1Pressed:
                    mouseFlag = MouseFlags.Button1Pressed;
                    break;

                case WindowsConsole.ButtonState.Button2Pressed:
                    mouseFlag = MouseFlags.Button2Pressed;
                    break;

                case WindowsConsole.ButtonState.RightmostButtonPressed:
                    mouseFlag = MouseFlags.Button3Pressed;
                    break;
            }

            if (mouseEvent.EventFlags == WindowsConsole.EventFlags.MouseMoved)
            {
                mouseFlag |= MouseFlags.ReportMousePosition;
                point = new Point();
                isButtonReleased = false;
                processButtonClick = false;
            }
            else
            {
                point = new Point()
                {
                    X = mouseEvent.MousePosition.X,
                    Y = mouseEvent.MousePosition.Y
                };
            }

            lastMouseButtonPressed = mouseEvent.ButtonState;
            isButtonPressed = true;

            if ((mouseFlag & MouseFlags.ReportMousePosition) == 0)
                Application.MainLoop.AddIdle(
                    () =>
                    {
                        Task.Run(async () => await ProcessContinuousButtonPressedAsync(mouseEvent, mouseFlag));
                        return false;
                    });
        }
        else if (lastMouseButtonPressed != null && mouseEvent.EventFlags == 0
                                                && !isButtonReleased && !isButtonDoubleClicked &&
                                                !isOneFingerDoubleClicked)
        {
            switch (lastMouseButtonPressed)
            {
                case WindowsConsole.ButtonState.Button1Pressed:
                    mouseFlag = MouseFlags.Button1Released;
                    break;

                case WindowsConsole.ButtonState.Button2Pressed:
                    mouseFlag = MouseFlags.Button2Released;
                    break;

                case WindowsConsole.ButtonState.RightmostButtonPressed:
                    mouseFlag = MouseFlags.Button3Released;
                    break;
            }

            isButtonPressed = false;
            isButtonReleased = true;
            if (point.X == mouseEvent.MousePosition.X && point.Y == mouseEvent.MousePosition.Y)
                processButtonClick = true;
        }
        else if (mouseEvent.EventFlags == WindowsConsole.EventFlags.MouseMoved
                 && !isOneFingerDoubleClicked && isButtonReleased && p == point)
        {
            mouseFlag = ProcessButtonClick(mouseEvent);
        }
        else if (mouseEvent.EventFlags.HasFlag(WindowsConsole.EventFlags.DoubleClick))
        {
            switch (mouseEvent.ButtonState)
            {
                case WindowsConsole.ButtonState.Button1Pressed:
                    mouseFlag = MouseFlags.Button1DoubleClicked;
                    break;

                case WindowsConsole.ButtonState.Button2Pressed:
                    mouseFlag = MouseFlags.Button2DoubleClicked;
                    break;

                case WindowsConsole.ButtonState.RightmostButtonPressed:
                    mouseFlag = MouseFlags.Button3DoubleClicked;
                    break;
            }

            isButtonDoubleClicked = true;
        }
        else if (mouseEvent.EventFlags == 0 && mouseEvent.ButtonState != 0 && isButtonDoubleClicked)
        {
            switch (mouseEvent.ButtonState)
            {
                case WindowsConsole.ButtonState.Button1Pressed:
                    mouseFlag = MouseFlags.Button1TripleClicked;
                    break;

                case WindowsConsole.ButtonState.Button2Pressed:
                    mouseFlag = MouseFlags.Button2TripleClicked;
                    break;

                case WindowsConsole.ButtonState.RightmostButtonPressed:
                    mouseFlag = MouseFlags.Button3TripleClicked;
                    break;
            }

            isButtonDoubleClicked = false;
        }
        else if (mouseEvent.EventFlags == WindowsConsole.EventFlags.MouseWheeled)
        {
            switch ((int)mouseEvent.ButtonState)
            {
                case int v when v > 0:
                    mouseFlag = MouseFlags.WheeledUp;
                    break;

                case int v when v < 0:
                    mouseFlag = MouseFlags.WheeledDown;
                    break;
            }
        }
        else if (mouseEvent.EventFlags == WindowsConsole.EventFlags.MouseWheeled &&
                 mouseEvent.ControlKeyState == WindowsConsole.ControlKeyState.ShiftPressed)
        {
            switch ((int)mouseEvent.ButtonState)
            {
                case int v when v > 0:
                    mouseFlag = MouseFlags.WheeledLeft;
                    break;

                case int v when v < 0:
                    mouseFlag = MouseFlags.WheeledRight;
                    break;
            }
        }
        else if (mouseEvent.EventFlags == WindowsConsole.EventFlags.MouseHorizontalWheeled)
        {
            switch ((int)mouseEvent.ButtonState)
            {
                case int v when v < 0:
                    mouseFlag = MouseFlags.WheeledLeft;
                    break;

                case int v when v > 0:
                    mouseFlag = MouseFlags.WheeledRight;
                    break;
            }
        }
        else if (mouseEvent.EventFlags == WindowsConsole.EventFlags.MouseMoved)
        {
            if (mouseEvent.MousePosition.X != point.X || mouseEvent.MousePosition.Y != point.Y)
            {
                mouseFlag = MouseFlags.ReportMousePosition;
                point = new Point();
            }
            else
            {
                mouseFlag = 0;
            }
        }
        else if (mouseEvent.ButtonState == 0 && mouseEvent.EventFlags == 0)
        {
            mouseFlag = 0;
        }

        mouseFlag = SetControlKeyStates(mouseEvent, mouseFlag);

        return new MouseEvent()
        {
            X = mouseEvent.MousePosition.X,
            Y = mouseEvent.MousePosition.Y,
            Flags = mouseFlag
        };
    }

    private MouseFlags ProcessButtonClick(WindowsConsole.MouseEventRecord mouseEvent)
    {
        MouseFlags mouseFlag = 0;
        switch (lastMouseButtonPressed)
        {
            case WindowsConsole.ButtonState.Button1Pressed:
                mouseFlag = MouseFlags.Button1Clicked;
                break;

            case WindowsConsole.ButtonState.Button2Pressed:
                mouseFlag = MouseFlags.Button2Clicked;
                break;

            case WindowsConsole.ButtonState.RightmostButtonPressed:
                mouseFlag = MouseFlags.Button3Clicked;
                break;
        }

        point = new Point()
        {
            X = mouseEvent.MousePosition.X,
            Y = mouseEvent.MousePosition.Y
        };
        lastMouseButtonPressed = null;
        isButtonReleased = false;
        return mouseFlag;
    }

    private async Task ProcessButtonDoubleClickedAsync()
    {
        await Task.Delay(300);
        isButtonDoubleClicked = false;
        isOneFingerDoubleClicked = false;
        buttonPressedCount = 0;
    }

    private async Task ProcessContinuousButtonPressedAsync(
        WindowsConsole.MouseEventRecord mouseEvent,
        MouseFlags mouseFlag)
    {
        await Task.Delay(200);
        while (isButtonPressed)
        {
            await Task.Delay(100);
            var me = new MouseEvent()
            {
                X = mouseEvent.MousePosition.X,
                Y = mouseEvent.MousePosition.Y,
                Flags = mouseFlag
            };

            var view = Application.wantContinuousButtonPressedView;
            if (view == null) break;
            if (isButtonPressed && (mouseFlag & MouseFlags.ReportMousePosition) == 0)
                Application.MainLoop.Invoke(() => mouseHandler?.Invoke(me));
        }
    }

    private static MouseFlags SetControlKeyStates(WindowsConsole.MouseEventRecord mouseEvent, MouseFlags mouseFlag)
    {
        if (mouseEvent.ControlKeyState.HasFlag(WindowsConsole.ControlKeyState.RightControlPressed) ||
            mouseEvent.ControlKeyState.HasFlag(WindowsConsole.ControlKeyState.LeftControlPressed))
            mouseFlag |= MouseFlags.ButtonCtrl;

        if (mouseEvent.ControlKeyState.HasFlag(WindowsConsole.ControlKeyState.ShiftPressed))
            mouseFlag |= MouseFlags.ButtonShift;

        if (mouseEvent.ControlKeyState.HasFlag(WindowsConsole.ControlKeyState.RightAltPressed) ||
            mouseEvent.ControlKeyState.HasFlag(WindowsConsole.ControlKeyState.LeftAltPressed))
            mouseFlag |= MouseFlags.ButtonAlt;
        return mouseFlag;
    }

    private KeyModifiers keyModifiers;

    public WindowsConsole.ConsoleKeyInfoEx ToConsoleKeyInfoEx(WindowsConsole.KeyEventRecord keyEvent)
    {
        var state = keyEvent.dwControlKeyState;

        var shift = (state & WindowsConsole.ControlKeyState.ShiftPressed) != 0;
        var alt = (state & (WindowsConsole.ControlKeyState.LeftAltPressed |
                            WindowsConsole.ControlKeyState.RightAltPressed)) != 0;
        var control = (state & (WindowsConsole.ControlKeyState.LeftControlPressed |
                                WindowsConsole.ControlKeyState.RightControlPressed)) != 0;
        var capslock = (state & WindowsConsole.ControlKeyState.CapslockOn) != 0;
        var numlock = (state & WindowsConsole.ControlKeyState.NumlockOn) != 0;
        var scrolllock = (state & WindowsConsole.ControlKeyState.ScrolllockOn) != 0;

        if (shift)
            keyModifiers.Shift = shift;
        if (alt)
            keyModifiers.Alt = alt;
        if (control)
            keyModifiers.Ctrl = control;
        if (capslock)
            keyModifiers.Capslock = capslock;
        if (numlock)
            keyModifiers.Numlock = numlock;
        if (scrolllock)
            keyModifiers.Scrolllock = scrolllock;

        var ConsoleKeyInfo = new ConsoleKeyInfo(
            keyEvent.UnicodeChar,
            (ConsoleKey)keyEvent.wVirtualKeyCode,
            shift,
            alt,
            control);
        return new WindowsConsole.ConsoleKeyInfoEx(ConsoleKeyInfo, capslock, numlock);
    }

    public Key MapKey(WindowsConsole.ConsoleKeyInfoEx keyInfoEx)
    {
        var keyInfo = keyInfoEx.consoleKeyInfo;
        switch (keyInfo.Key)
        {
            case ConsoleKey.Escape:
                return MapKeyModifiers(keyInfo, Key.Esc);
            case ConsoleKey.Tab:
                return keyInfo.Modifiers == ConsoleModifiers.Shift ? Key.BackTab : Key.Tab;
            case ConsoleKey.Home:
                return MapKeyModifiers(keyInfo, Key.Home);
            case ConsoleKey.End:
                return MapKeyModifiers(keyInfo, Key.End);
            case ConsoleKey.LeftArrow:
                return MapKeyModifiers(keyInfo, Key.CursorLeft);
            case ConsoleKey.RightArrow:
                return MapKeyModifiers(keyInfo, Key.CursorRight);
            case ConsoleKey.UpArrow:
                return MapKeyModifiers(keyInfo, Key.CursorUp);
            case ConsoleKey.DownArrow:
                return MapKeyModifiers(keyInfo, Key.CursorDown);
            case ConsoleKey.PageUp:
                return MapKeyModifiers(keyInfo, Key.PageUp);
            case ConsoleKey.PageDown:
                return MapKeyModifiers(keyInfo, Key.PageDown);
            case ConsoleKey.Enter:
                return MapKeyModifiers(keyInfo, Key.Enter);
            case ConsoleKey.Spacebar:
                return MapKeyModifiers(keyInfo, keyInfo.KeyChar == 0 ? Key.Space : (Key)keyInfo.KeyChar);
            case ConsoleKey.Backspace:
                return MapKeyModifiers(keyInfo, Key.Backspace);
            case ConsoleKey.Delete:
                return MapKeyModifiers(keyInfo, Key.DeleteChar);
            case ConsoleKey.Insert:
                return MapKeyModifiers(keyInfo, Key.InsertChar);

            case ConsoleKey.NumPad0:
                return keyInfoEx.NumLock ? Key.D0 : Key.InsertChar;
            case ConsoleKey.NumPad1:
                return keyInfoEx.NumLock ? Key.D1 : Key.End;
            case ConsoleKey.NumPad2:
                return keyInfoEx.NumLock ? Key.D2 : Key.CursorDown;
            case ConsoleKey.NumPad3:
                return keyInfoEx.NumLock ? Key.D3 : Key.PageDown;
            case ConsoleKey.NumPad4:
                return keyInfoEx.NumLock ? Key.D4 : Key.CursorLeft;
            case ConsoleKey.NumPad5:
                return keyInfoEx.NumLock ? Key.D5 : (Key)(uint)keyInfo.KeyChar;
            case ConsoleKey.NumPad6:
                return keyInfoEx.NumLock ? Key.D6 : Key.CursorRight;
            case ConsoleKey.NumPad7:
                return keyInfoEx.NumLock ? Key.D7 : Key.Home;
            case ConsoleKey.NumPad8:
                return keyInfoEx.NumLock ? Key.D8 : Key.CursorUp;
            case ConsoleKey.NumPad9:
                return keyInfoEx.NumLock ? Key.D9 : Key.PageUp;

            case ConsoleKey.Oem1:
            case ConsoleKey.Oem2:
            case ConsoleKey.Oem3:
            case ConsoleKey.Oem4:
            case ConsoleKey.Oem5:
            case ConsoleKey.Oem6:
            case ConsoleKey.Oem7:
            case ConsoleKey.Oem8:
            case ConsoleKey.Oem102:
            case ConsoleKey.OemPeriod:
            case ConsoleKey.OemComma:
            case ConsoleKey.OemPlus:
            case ConsoleKey.OemMinus:
                if (keyInfo.KeyChar == 0)
                    return Key.Unknown;

                return (Key)(uint)keyInfo.KeyChar;
        }

        var key = keyInfo.Key;
        //var alphaBase = ((keyInfo.Modifiers == ConsoleModifiers.Shift) ^ (keyInfoEx.CapsLock)) ? 'A' : 'a';

        if (key >= ConsoleKey.A && key <= ConsoleKey.Z)
        {
            var delta = key - ConsoleKey.A;
            if (keyInfo.Modifiers == ConsoleModifiers.Control)
                return (Key)((uint)Key.CtrlMask | ((uint)Key.A + delta));
            if (keyInfo.Modifiers == ConsoleModifiers.Alt) return (Key)((uint)Key.AltMask | ((uint)Key.A + delta));
            if ((keyInfo.Modifiers & (ConsoleModifiers.Alt | ConsoleModifiers.Control)) != 0)
                if (keyInfo.KeyChar == 0 || keyInfo.KeyChar != 0 && keyInfo.KeyChar >= 1 && keyInfo.KeyChar <= 26)
                    return MapKeyModifiers(keyInfo, (Key)((uint)Key.A + delta));
            //return (Key)((uint)alphaBase + delta);
            return (Key)(uint)keyInfo.KeyChar;
        }

        if (key >= ConsoleKey.D0 && key <= ConsoleKey.D9)
        {
            var delta = key - ConsoleKey.D0;
            if (keyInfo.Modifiers == ConsoleModifiers.Alt) return (Key)((uint)Key.AltMask | ((uint)Key.D0 + delta));
            if (keyInfo.Modifiers == ConsoleModifiers.Control)
                return (Key)((uint)Key.CtrlMask | ((uint)Key.D0 + delta));
            if ((keyInfo.Modifiers & (ConsoleModifiers.Alt | ConsoleModifiers.Control)) != 0)
                if (keyInfo.KeyChar == 0 || keyInfo.KeyChar == 30)
                    return MapKeyModifiers(keyInfo, (Key)((uint)Key.D0 + delta));
            return (Key)(uint)keyInfo.KeyChar;
        }

        if (key >= ConsoleKey.F1 && key <= ConsoleKey.F12)
        {
            var delta = key - ConsoleKey.F1;
            if ((keyInfo.Modifiers & (ConsoleModifiers.Shift | ConsoleModifiers.Alt | ConsoleModifiers.Control)) !=
                0) return MapKeyModifiers(keyInfo, (Key)((uint)Key.F1 + delta));

            return (Key)((uint)Key.F1 + delta);
        }

        if (keyInfo.KeyChar != 0) return MapKeyModifiers(keyInfo, (Key)(uint)keyInfo.KeyChar);

        return (Key)0xffffffff;
    }

    private Key MapKeyModifiers(ConsoleKeyInfo keyInfo, Key key)
    {
        var keyMod = new Key();
        if ((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0)
            keyMod = Key.ShiftMask;
        if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
            keyMod |= Key.CtrlMask;
        if ((keyInfo.Modifiers & ConsoleModifiers.Alt) != 0)
            keyMod |= Key.AltMask;

        return keyMod != Key.Null ? keyMod | key : key;
    }

    public override void Init(Action terminalResized)
    {
        TerminalResized = terminalResized;

        var winSize = WinConsole.GetConsoleOutputWindow(out var pos);
        cols = winSize.Width;
        rows = winSize.Height;

        WindowsConsole.SmallRect.MakeEmpty(ref damageRegion);

        ResizeScreen();
        UpdateOffScreen();

        Colors.TopLevel = new ColorScheme();
        Colors.Base = new ColorScheme();
        Colors.Dialog = new ColorScheme();
        Colors.Menu = new ColorScheme();
        Colors.Error = new ColorScheme();

        Colors.TopLevel.Normal = new(System.ConsoleColor.Green, System.ConsoleColor.Black);
        Colors.TopLevel.Focus = new(System.ConsoleColor.White, System.ConsoleColor.DarkCyan);
        Colors.TopLevel.HotNormal = new(System.ConsoleColor.DarkYellow, System.ConsoleColor.Black);
        Colors.TopLevel.HotFocus = new(System.ConsoleColor.DarkBlue, System.ConsoleColor.DarkCyan);
        Colors.TopLevel.Disabled = new(System.ConsoleColor.DarkGray, System.ConsoleColor.Black);

        Colors.Base.Normal = new(System.ConsoleColor.White, System.ConsoleColor.DarkBlue);
        Colors.Base.Focus = new(System.ConsoleColor.Black, System.ConsoleColor.Gray);
        Colors.Base.HotNormal = new(System.ConsoleColor.DarkCyan, System.ConsoleColor.DarkBlue);
        Colors.Base.HotFocus = new(System.ConsoleColor.Blue, System.ConsoleColor.Gray);
        Colors.Base.Disabled = new(System.ConsoleColor.DarkGray, System.ConsoleColor.DarkBlue);

        Colors.Menu.Normal = new(System.ConsoleColor.White, System.ConsoleColor.DarkGray);
        Colors.Menu.Focus = new(System.ConsoleColor.White, System.ConsoleColor.Black);
        Colors.Menu.HotNormal = new(System.ConsoleColor.Yellow, System.ConsoleColor.DarkGray);
        Colors.Menu.HotFocus = new(System.ConsoleColor.Yellow, System.ConsoleColor.Black);
        Colors.Menu.Disabled = new(System.ConsoleColor.Gray, System.ConsoleColor.DarkGray);

        Colors.Dialog.Normal = new(System.ConsoleColor.Black, System.ConsoleColor.Gray);
        Colors.Dialog.Focus = new(System.ConsoleColor.White, System.ConsoleColor.DarkGray);
        Colors.Dialog.HotNormal = new(System.ConsoleColor.DarkBlue, System.ConsoleColor.Gray);
        Colors.Dialog.HotFocus = new(System.ConsoleColor.DarkBlue, System.ConsoleColor.DarkGray);
        Colors.Dialog.Disabled = new(System.ConsoleColor.DarkGray, System.ConsoleColor.Gray);

        Colors.Error.Normal = new(System.ConsoleColor.DarkRed, System.ConsoleColor.White);
        Colors.Error.Focus = new(System.ConsoleColor.White, System.ConsoleColor.DarkRed);
        Colors.Error.HotNormal = new(System.ConsoleColor.Black, System.ConsoleColor.White);
        Colors.Error.HotFocus = new(System.ConsoleColor.Black, System.ConsoleColor.DarkRed);
        Colors.Error.Disabled = new(System.ConsoleColor.DarkGray, System.ConsoleColor.White);
    }

    private void ResizeScreen()
    {
        OutputBuffer = new WindowsConsole.CharInfo[Rows * Cols];
        Clip = new Rectangle(0, 0, Cols, Rows);
        damageRegion = new WindowsConsole.SmallRect()
        {
            Top = 0,
            Left = 0,
            Bottom = (short)Rows,
            Right = (short)Cols
        };
        WinConsole.ForceRefreshCursorVisibility();
    }

    private ushort MakeConsoleAttribute(CellAttributes cellColor)
    {
        var v = ((int)cellColor.Background << 4 | (int)cellColor.Foreground);
        if (cellColor.Underline)
            v |= (int)CharInfoAttributes.COMMON_LVB_UNDERSCORE;
        if (cellColor.Reverse)
            v |= (int)CharInfoAttributes.COMMON_LVB_REVERSE_VIDEO;

        return (ushort)v;
    }

    private void UpdateOffScreen()
    {
        contents = new CellValue[rows, cols];
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                var position = row * cols + col;
                OutputBuffer[position].Attributes = MakeConsoleAttribute(Colors.TopLevel.Normal);
                OutputBuffer[position].Char.UnicodeChar = ' ';

                contents[row, col] = new CellValue( 
                    (Rune)OutputBuffer[position].Char.UnicodeChar,
                    Colors.TopLevel.Normal, 
                    0);
            }
        }
    }

    private int ccol, crow;

    public override void Move(int col, int row)
    {
        ccol = col;
        crow = row;
    }

    public override void AddRune(char ch) { AddRune((Rune)ch); }

    public override void AddRune(Rune rune)
    {
        rune = MakePrintable(rune);
        var position = crow * Cols + ccol;

        if (Clip.Contains(ccol, crow))
        {
            OutputBuffer[position].Attributes = MakeConsoleAttribute(currentAttribute);
            OutputBuffer[position].Char.UnicodeChar = (char)rune.Value;
            contents[crow, ccol] = new CellValue(rune, currentAttribute, 1);
            WindowsConsole.SmallRect.Update(ref damageRegion, (short)ccol, (short)crow);
        }

        ccol++;
        var runeWidth = rune.ColumnWidth();
        if (runeWidth > 1)
        {
            for (var i = 1; i < runeWidth; i++)
                AddStr(" ");
        }
        if (sync)
            UpdateScreen();
    }

    public override void AddStr(string str)
    {
        foreach (var rune in str.EnumerateRunes())
            AddRune(rune);
    }

    private CellAttributes currentAttribute;

    public override void SetAttribute(CellAttributes c) { currentAttribute = c; }

    public override void Refresh()
    {
        UpdateScreen();

        WinConsole.SetInitialCursorVisibility();
#if false
			var bufferCoords = new WindowsConsole.Coord (){
				X = (short)Clip.Width,
				Y = (short)Clip.Height
			};

			var window = new WindowsConsole.SmallRect (){
				Top = 0,
				Left = 0,
				Right = (short)Clip.Right,
				Bottom = (short)Clip.Bottom
			};

			UpdateCursor();
			WinConsole.WriteToConsole (OutputBuffer, bufferCoords, window);
#endif
    }

    public override void UpdateScreen()
    {
        if (damageRegion.Left == -1)
            return;

        var bufferCoords = new WindowsConsole.Coord()
        {
            X = (short)Clip.Width,
            Y = (short)Clip.Height
        };

        //var window = new WindowsConsole.SmallRect () {
        //	Top = 0,
        //	Left = 0,
        //	Right = (short)Clip.Right,
        //	Bottom = (short)Clip.Bottom
        //};

        UpdateCursor();
        WinConsole.WriteToConsole(new Size(Cols, Rows), OutputBuffer, bufferCoords, damageRegion);
        // System.Diagnostics.Debugger.Log (0, "debug", $"Region={damageRegion.Right - damageRegion.Left},{damageRegion.Bottom - damageRegion.Top}\n");
        WindowsConsole.SmallRect.MakeEmpty(ref damageRegion);
    }

    public override void UpdateCursor()
    {
        var position = new WindowsConsole.Coord()
        {
            X = (short)ccol,
            Y = (short)crow
        };
        WinConsole.SetCursorPosition(position);
    }

    public override void End()
    {
        WinConsole.Cleanup();
    }

    public override CellAttributes GetAttribute() { return currentAttribute; }

    /// <inheritdoc/>
    public override bool GetCursorVisibility(out CursorVisibility visibility)
    {
        return WinConsole.GetCursorVisibility(out visibility);
    }

    /// <inheritdoc/>
    public override bool SetCursorVisibility(CursorVisibility visibility)
    {
        return WinConsole.SetCursorVisibility(visibility);
    }

    /// <inheritdoc/>
    public override bool EnsureCursorVisibility() { return WinConsole.EnsureCursorVisibility(); }

    public override void SendKeys(char keyChar, ConsoleKey key, bool shift, bool alt, bool control)
    {
        var input = new WindowsConsole.InputRecord
        {
            EventType = WindowsConsole.EventType.Key
        };

        var keyEvent = new WindowsConsole.KeyEventRecord
        {
            bKeyDown = true
        };
        var controlKey = new WindowsConsole.ControlKeyState();
        if (shift)
        {
            controlKey |= WindowsConsole.ControlKeyState.ShiftPressed;
            keyEvent.UnicodeChar = '\0';
            keyEvent.wVirtualKeyCode = 16;
        }

        if (alt)
        {
            controlKey |= WindowsConsole.ControlKeyState.LeftAltPressed;
            controlKey |= WindowsConsole.ControlKeyState.RightAltPressed;
            keyEvent.UnicodeChar = '\0';
            keyEvent.wVirtualKeyCode = 18;
        }

        if (control)
        {
            controlKey |= WindowsConsole.ControlKeyState.LeftControlPressed;
            controlKey |= WindowsConsole.ControlKeyState.RightControlPressed;
            keyEvent.UnicodeChar = '\0';
            keyEvent.wVirtualKeyCode = 17;
        }

        keyEvent.dwControlKeyState = controlKey;

        input.KeyEvent = keyEvent;

        if (shift || alt || control) ProcessInput(input);

        keyEvent.UnicodeChar = keyChar;
        if ((shift || alt || control)
            && (key >= ConsoleKey.A && key <= ConsoleKey.Z
                || key >= ConsoleKey.D0 && key <= ConsoleKey.D9))
            keyEvent.wVirtualKeyCode = (ushort)key;
        else
            keyEvent.wVirtualKeyCode = '\0';

        input.KeyEvent = keyEvent;

        try
        {
            ProcessInput(input);
        }
        catch (OverflowException) { }
        finally
        {
            keyEvent.bKeyDown = false;
            input.KeyEvent = keyEvent;
            ProcessInput(input);
        }
    }

#region Unused

    public override void SetColors(ConsoleColor foreground, ConsoleColor background) { }
    public override void Suspend() { }
    public override void StartReportingMouseMoves() { }
    public override void StopReportingMouseMoves() { }
    public override void UncookMouse() { }
    public override void CookMouse() { }

    #endregion
}
