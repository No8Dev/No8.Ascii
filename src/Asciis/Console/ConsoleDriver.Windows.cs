using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using No8.Ascii.Platforms;
using No8.Ascii.VirtualTerminal;

namespace No8.Ascii;

using static Windows;
using static Windows.Kernel32;
using static Windows.User32;

public sealed class ConsoleDriverWindows : ConsoleDriver
{
    internal readonly IntPtr            InputHandle, OutputHandle, ErrorHandle;

    private readonly Encoding           _originalOutputEncoding;
    private readonly ConsoleInputModes  _originalConsoleInputMode;
    private readonly ConsoleOutputModes _originalConsoleOutputMode;
    private readonly ConsoleOutputModes _originalConsoleErrorMode;
    
    private readonly bool[]               _mouseOldState = new bool[5];
    private readonly bool[]               _mouseNewState = new bool[5];
    private readonly byte[]               _keyOldState   = new byte[256];
    private readonly byte[]               _keyNewState   = new byte[256];
    
    private static readonly VirtualKeyCode[] CanToggleKey =
    {
        VirtualKeyCode.NUMLOCK,
        VirtualKeyCode.CAPITAL
    };

    ConsoleCtrlDelegate _consoleCtrlHandler;

    public ConsoleDriverWindows()
    {
        /*
        Trace       => May be sensitive.  Application internal
        Debug       => useful while debugging
        Information => Flow, long term value
        Warning     => Abnormal, unexpected
        Error       => Failure in current activity. Recoverable
        Critical    => Unrecoverable, crash
        None,
        */
        
        Clipboard      = new ClipboardWindows();
        InputHandle    = GetStdHandle(StandardHandle.Input);
        OutputHandle   = GetStdHandle(StandardHandle.Output);
        ErrorHandle    = GetStdHandle(StandardHandle.Error);

        Terminal.Screen.ScreenAlt();

        // Console Input Mode
        _originalConsoleInputMode = ConsoleInputMode;

        var newConsoleInputMode =
            _originalConsoleInputMode |
            ConsoleInputModes.EnableMouseInput |
            ConsoleInputModes.EnableExtendedFlags;
        newConsoleInputMode &= ~ConsoleInputModes.EnableQuickEditMode;

        //ConsoleInputModes.EnableVirtualTerminalInput;
        //ConsoleInputModes.EnableProcessedInput |
        //ConsoleInputModes.EnableLineInput |
        //ConsoleInputModes.EnableEchoInput |
        //ConsoleInputModes.EnableWindowInput |
        //ConsoleInputModes.EnableInsertMode |
        //ConsoleInputModes.EnableQuickEditMode |
        //ConsoleInputModes.EnableAutoPosition |

        ConsoleInputMode =  newConsoleInputMode;

        // Console Output Mode
        _originalConsoleOutputMode = ConsoleOutputMode;
        var newConsoleOutputMode =
            _originalConsoleOutputMode |
            ConsoleOutputModes.EnableVirtualTerminalProcessing |
            ConsoleOutputModes.DisableNewlineAutoReturn |
            ConsoleOutputModes.EnableLvbGridWorldwide;
        ConsoleOutputMode    =  newConsoleOutputMode;
        
        // Console Error Mode
        _originalConsoleErrorMode = ConsoleErrorMode;
        var newConsoleErrorMode =
            _originalConsoleErrorMode |
            ConsoleOutputModes.DisableNewlineAutoReturn;
        ConsoleErrorMode    =  newConsoleErrorMode;

        // Output Encoding
        _originalOutputEncoding = OutputEncoding;
        OutputEncoding = Encoding.UTF8;     // Default Output encoding so can support extended characters and emoji

        GetKeyboardState(_keyOldState);

        GetConsoleOutputWindow(out var size, out var position);
        Pointer = position;
        Cols    = size.Width;
        Rows    = size.Height;

        SetConsoleWindow(Cols, Rows);

        // weird syntax, but is required to work properly
        _consoleCtrlHandler += new ConsoleCtrlDelegate(BreakEvent);
        SetConsoleCtrlHandler(_consoleCtrlHandler, true);
    }

    private static bool BreakEvent(CtrlTypes ctrlType)
    {
        Debug.WriteLine($"BreakEvent {ctrlType}");
        switch(ctrlType)
        {
            case CtrlTypes.CtrlCEvent:
            case CtrlTypes.CtrlBreakEvent:
                break;
            case CtrlTypes.CtrlCloseEvent:
            case CtrlTypes.CtrlLogoffEvent:
            case CtrlTypes.CtrlShutdownEvent:
                break;
        }
        return true;
    }

    internal override void Shutdown()
    {
        SetConsoleCtrlHandler(_consoleCtrlHandler, false);

        OutputEncoding    = _originalOutputEncoding;
        ConsoleInputMode  = _originalConsoleInputMode;
        ConsoleOutputMode = _originalConsoleOutputMode;
        ConsoleErrorMode  = _originalConsoleErrorMode;

        Terminal.Screen.ScreenNormal();
    }

    ~ConsoleDriverWindows()
    {
        Shutdown();
    }

    private static readonly object SyncObject = new();

    public override Encoding InputEncoding
    {
        get => GetSupportedConsoleEncoding((int)GetConsoleCP());
        set => SetConsoleCP((uint)value.CodePage);
    }

    public override Encoding OutputEncoding
    {
        get => GetSupportedConsoleEncoding((int)GetConsoleOutputCP());
        set => SetConsoleOutputCP((uint)value.CodePage);
    }

    public ConsoleInputModes ConsoleInputMode
    {
        get
        {
            GetConsoleInputMode(InputHandle, out var mode);
            return mode;
        }
        set => SetConsoleInputMode(InputHandle, value);
    }
    public ConsoleOutputModes ConsoleOutputMode
    {
        get
        {
            GetConsoleOutputMode(OutputHandle, out var mode);
            return mode;
        }
        set => SetConsoleOutputMode(OutputHandle, value);
    }
    public ConsoleOutputModes ConsoleErrorMode
    {
        get
        {
            GetConsoleOutputMode(ErrorHandle, out var mode);
            return mode;
        }
        set => SetConsoleOutputMode(ErrorHandle, value);
    }

    public override ConsoleColor BackgroundColor      { get; set; }
    public override ConsoleColor ForegroundColor      { get; set; }

    internal void GetConsoleOutputWindow(out Size size, out Point position)
    {
        var csbi = ConsoleScreenBufferInfoEx.Create();
        if (!GetConsoleScreenBufferInfoEx(OutputHandle, ref csbi))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        size = new Size(csbi.Window.Right - csbi.Window.Left + 1,
            csbi.Window.Bottom - csbi.Window.Top + 1);
        position = new Point(csbi.Window.Left, csbi.Window.Top);
    }

    internal Size SetConsoleWindow(int cols, int rows)
    {
        var csbi = ConsoleScreenBufferInfoEx.Create();
        if (!GetConsoleScreenBufferInfoEx(OutputHandle, ref csbi))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        Debug.WriteLine($"Set Window Size( {cols}, {rows} )  " + csbi);

        var origSize = csbi.ScreenSize;

        var maxWinSize = GetLargestConsoleWindowSize(OutputHandle);
        cols = (short)Math.Min(cols, maxWinSize.X);
        rows = (short)Math.Min(rows, maxWinSize.Y);

        //var currentWidth  = (short)(csbi.Window.Right - csbi.Window.Left + 1);
        //var currentHeight = (short)(csbi.Window.Bottom - csbi.Window.Top + 1);

        if (cols > Cols || rows > Rows)
            csbi.Window = new SmallRect(0, 0, 1, 1);
        else
            csbi.Window = new SmallRect(0, 0, (short)(cols - 1), (short)(rows - 1));

        Debug.WriteLine($"SetConsoleWindowInfo( {csbi.Window} )");
        //if (!SetConsoleWindowInfo(OutputHandle, true, ref csbi.Window))
        //    Error("SetConsoleWindow => SetConsoleWindowInfo");

        Debug.WriteLine($"SetConsoleScreenBufferSize( {cols}, {rows} )");
        //if (!SetConsoleScreenBufferSize(OutputHandle, new Coord((short)cols, (short)rows)))
        //    Error("SetConsoleWindow => SetConsoleScreenBufferSize");

        if (cols != Cols || rows != Rows)
        {
            //Write(Terminal.ControlSeq.ClearScreen);

            Cols = cols;
            Rows = rows;

            var winSize = new SmallRect(0, 0, (short)(cols - 1), (short)(rows - 1));
            Debug.WriteLine($"SetConsoleWindowInfo( {winSize} )");
            //if (!SetConsoleWindowInfo(OutputHandle, true, ref winSize))
            //    Error("SetConsoleWindow => SetConsoleWindowInfo Bigger");
        }

        return new Size(cols, rows);
    }


    public override void GatherInputEvents()
    {
        GetNumberOfConsoleInputEvents(InputHandle, out var events);
        if (events > 0)
        {
            var inBuf = new InputRecord[events];
            if (!ReadConsoleInput(InputHandle, inBuf, events, out var actualEvents))
                Error("ReadConsoleInput");

            if (actualEvents < events)
                inBuf = inBuf.Take((int)actualEvents).ToArray();

            ProcessVirtualKeyboardState();
            ProcessInputRecords(inBuf);
        }
    }

    public override bool WindowSizeChanged() 
    { 
        GetConsoleOutputWindow(out var size, out var point);

        if (size.Width != Cols || size.Height != Rows)
        {
            SetConsoleWindow(size.Width, size.Height);

            Cols = size.Width;
            Rows = size.Height;
            RaiseTerminalResized(size);
            return true;
        }

        return false;
    }

    private void ProcessVirtualKeyboardState()
    {
        var changed = false;
        GetKeyboardState(_keyNewState);

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
                var vk    = (VirtualKeyCode)i;
                var key   = VirtualKeys[i];
                var state = _keyOldState[i];
                if (key.IsPressed || key.IsToggled)
                {
                    var list = new List<string> { $"0x{state:X4}" };
                    if (key.IsPressed) list.Add("Pressed");
                    if (key.IsToggled && CanToggleKey.Contains(vk)) list.Add("Toggled");
                    sb.Append($"{vk} {string.Join(' ', list)} => ");
                }
            }
            Debug.WriteLine(sb.ToString());
        #endif
        }
    }
    
    private void ProcessInputRecords(InputRecord[] inBuf)
    {
        foreach (var input in inBuf)
        {
            switch (input.EventType)
            {
            case EventType.Key:
                    //if (input.KeyEvent.IsKeyDown)
                    //    AddKeyboardEvent(input.KeyEvent.ToKeyboardEvent(KeyboardEventType.Pressed));
                    //else
                    //{
                    //    AddKeyboardEvent(input.KeyEvent.ToKeyboardEvent(KeyboardEventType.Released));
                    //    if (!IsModKey(input.KeyEvent))
                    //        AddKeyboardEvent(input.KeyEvent.ToKeyboardEvent(KeyboardEventType.Key));
                    //}
                    Debug.WriteLine(input.ToString());
                    if ((input.KeyEvent.ControlKeyState == ControlKeyState.LeftCtrlPressed ||
                        input.KeyEvent.ControlKeyState == ControlKeyState.RightCtrlPressed) &&
                        input.KeyEvent.VirtualKeyCode == VirtualKeyCode.VK_C)
                    {
                        Debug.WriteLine("Ctrl-C");
                    }

                break;

            case EventType.Focus:
                IsFocused = input.FocusEvent.SetFocus > 0;
                Trace.WriteLine($"IsFocused {IsFocused}");

                RaiseWindowFocusChange(IsFocused);
                break;

            case EventType.Menu:
                var menu = input.MenuEvent.CommandId;
                Trace.WriteLine($"Menu CommandId: {menu:X8}");
                break;

            case EventType.Mouse:
                ProcessMouseEvent(input.MouseEvent);
                break;

#if DEBUG
            case EventType.WindowBufferSize:
                ProcessWindowBufferSize(input.WindowBufferSizeEvent);
                break;
#endif
            }
        }
    }

    private void ProcessWindowBufferSize(WindowBufferSizeRecord bufferSizeEvent)
    {
#if DEBUG
        var buffSize = bufferSizeEvent.size;
        GetConsoleOutputWindow(out var size, out var position);

        Debug.WriteLine($"WindowBufferSize Event: Buffer[{buffSize}] OutputWinSize[{size}]  Old[{Cols}, {Rows}] ");
#endif
    }

    private void ProcessMouseEvent(MouseEventRecord mouseEvent)
    {
        #if FALSE
            _logger.LogTrace(mouseEvent.ToString());
        #endif

        bool mouseMoved = MousePosX != mouseEvent.Position.X ||
                          MousePosY != mouseEvent.Position.Y;
        MousePosX = mouseEvent.Position.X;
        MousePosY = mouseEvent.Position.Y;
        var now = DateTime.Now.ToBinary();

        // Flag will only have a single value
        switch (mouseEvent.EventFlags)
        {
            case MouseEventFlags.MouseMoved:
                if (mouseMoved)
                    AddPointerEvent(
                        new PointerEvent(
                            now,
                            PointerEventType.Move,
                            MousePosX,
                            MousePosY));
                break;

            case MouseEventFlags.DoubleClick:
                for (int buttonId = 0; buttonId < 5; buttonId++)
                {
                    if (((ushort)mouseEvent.ButtonState & (1 << buttonId)) > 0)
                    {
                        AddPointerEvent(
                            new PointerEvent(
                                now,
                                PointerEventType.DoubleClick,
                                MousePosX,
                                MousePosY,
                                buttonId));
                    }
                }
                break;

            case MouseEventFlags.MouseWheeled:
                MouseWheel = (short)((uint)mouseEvent.ButtonState >> 16);
                AddPointerEvent(
                    new PointerEvent(
                        now,
                        PointerEventType.Wheel,
                        MousePosX,
                        MousePosY,
                        Value: MouseWheel
                    ));
                break;

            case MouseEventFlags.MouseHorizontalWheeled:
                MouseHorizontalWheel = (short)((uint)mouseEvent.ButtonState >> 16);
                AddPointerEvent(
                    new PointerEvent(
                        now,
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
                                AddPointerEvent(
                                    new PointerEvent(
                                        now,
                                        PointerEventType.Pressed,
                                        MousePosX,
                                        MousePosY,
                                        buttonId));
                        }
                        else
                        {
                            if (Mouse[buttonId].IsPressed)
                            {
                                AddPointerEvent(
                                    new PointerEvent(
                                        now,
                                        PointerEventType.Released,
                                        MousePosX,
                                        MousePosY,
                                        buttonId));
                                AddPointerEvent(
                                    new PointerEvent(
                                        now,
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
    
    private uint Error(string msg)
    {
        var lastError = (uint)Marshal.GetLastWin32Error();
        var sb        = new StringBuilder(1024);
        FormatMessage((uint)FormatMessageFlags.FromSystem,
            IntPtr.Zero,
            lastError,
            0,
            sb,
            (uint)sb.Capacity,
            null);
        Debug.WriteLine($"ERROR: {msg}\n\t{sb}\n");

        return lastError;
    }
    
    private static bool IsModKey(KeyEventRecord ir)
    {
        // We should also skip over Shift, Control, and Alt, as well as caps lock.
        // Apparently we don't need to check for 0xA0 through 0xA5, which are keys like
        // Left Control & Right Control. See the ConsoleKey enum for these values.
        var keyCode = ir.VirtualKeyCode;
        return keyCode 
            is VirtualKeyCode.SHIFT 
            or VirtualKeyCode.CONTROL 
            or VirtualKeyCode.MENU 
            or VirtualKeyCode.CAPITAL 
            or VirtualKeyCode.NUMLOCK 
            or VirtualKeyCode.SCROLL;
    }

    public override void Write(string str)
    {
        if (string.IsNullOrEmpty(str))
            return;

        Kernel32.WriteConsole(OutputHandle, str, (uint)str.Length, out var written, IntPtr.Zero);
    }

    public override void WriteConsole(Canvas canvas)
    {
#if false
        // Line by Line
        var sb = new StringBuilder(canvas.Width * canvas.Height);
        Color? lastForeground = null;
        Color? lastBackground = null;
        int linesChanged = 0;
        for (int y = 0; y < canvas.Height; y++)
        {
            if (!LineChanged(canvas, LastCanvas, y))
                continue;

            linesChanged++;
            sb.Append(Terminal.Cursor.Set(y + 1, 1));
            for (int x = 0; x < canvas.Width; x++)
            {
                var tc = canvas.Glyphs[y, x];
                if (tc.Fore != lastForeground && tc.Fore != null)
                {
                    sb.Append(Terminal.Color.Fore(tc.Fore.Value.R, tc.Fore.Value.G, tc.Fore.Value.B));
                    lastForeground = tc.Fore;
                }
                if (tc.Back != lastBackground && tc.Back != null)
                {
                    sb.Append(Terminal.Color.Back(tc.Back.Value.R, tc.Back.Value.G, tc.Back.Value.B));
                    lastBackground = tc.Back;
                }
                sb.Append(tc.Chr);
            }
        }
        Write(sb.ToString());
        Debug.WriteLine($"ConsoleChanged( {linesChanged} )");

        if (LastCanvas == null || LastCanvas.Width != canvas.Width || LastCanvas.Height != canvas.Height)
            LastCanvas = new Array2D<Glyph>(canvas.Glyphs);
        else
            canvas.CloneTo(LastCanvas);
#else
        // Glyph by Glyph
        var sb = new StringBuilder(canvas.Width * canvas.Height);
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

#endif
    }

    internal static Encoding GetSupportedConsoleEncoding(int codepage)
    {
        int defaultEncCodePage = Encoding.GetEncoding(0).CodePage;

        if (defaultEncCodePage == codepage || defaultEncCodePage != Encoding.UTF8.CodePage)
            return Encoding.GetEncoding(codepage);

        if (codepage != Encoding.UTF8.CodePage)
            return Encoding.UTF8;

        return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    }

}

