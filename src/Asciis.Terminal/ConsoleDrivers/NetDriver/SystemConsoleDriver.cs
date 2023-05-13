using Asciis.Terminal.ConsoleDrivers.CursesDriver;
using Asciis.Terminal.Core;
using Asciis.Terminal.Core.Clipboard;
using Asciis.Terminal.Helpers;

namespace Asciis.Terminal.ConsoleDrivers.NetDriver;

internal class SystemConsoleDriver : ConsoleDriver
{
    private int cols, rows, top;
    public override int Cols => cols;
    public override int Rows => rows;
    public override int Left => 0;
    public override int Top => top;
    public override bool HeightAsBuffer { get; set; }

    public SystemConsole? NetWinConsole { get; }
    public override bool AlwaysSetPosition
    {
        get => alwaysSetPosition;
        set
        {
            alwaysSetPosition = true;
            Refresh();
        }
    }
    public override IClipboard Clipboard { get; }
    internal override CellValue[,] Contents => contents;

    private int largestWindowHeight;

    public SystemConsoleDriver()
    {
        largestWindowHeight = Console.BufferHeight;
        if (OperatingSystem.IsWindows())
        {
            NetWinConsole = new SystemConsole();
            Clipboard = new WindowsClipboard();
        }
        else if (OperatingSystem.IsMacOS())
            Clipboard = new MacOSXClipboard();
        else
            Clipboard = new CursesClipboard();
    }

    // The format is rows, columns and 3 values on the last column: Rune, Attribute and Dirty Flag
    private CellValue[,] contents = new CellValue[0, 0];
    private bool[] dirtyLine = Array.Empty<bool>();

    private static bool sync = false;

    // Current row, and current col, tracked by Move/AddCh only
    private int ccol, crow;

    public override void Move(int col, int row)
    {
        ccol = col;
        crow = row;
    }

    public override void AddRune(char ch) { AddRune((Rune)ch); }

    public override void AddRune(Rune rune)
    {
        if (contents.Length != Rows * Cols * 3) return;
        rune = MakePrintable(rune);
        var runeWidth = rune.ColumnWidth();
        if (Clip.Contains(ccol, crow) && ccol + Math.Max(runeWidth, 1) <= Cols)
        {
            contents[crow, ccol] = new CellValue(rune, currentAttribute, 1);
            dirtyLine[crow] = true;

            ccol++;
            if (runeWidth > 1)
            {
                for (var i = 1; i < runeWidth; i++)
                {
                    var cellValue = contents[crow, ccol];
                    if (ccol < cols)
                        contents[crow, ccol] = new CellValue(cellValue.Rune, cellValue.Attr, 0);
                    else
                        break;
                    ccol++;
                }
            }
        }
        else if (ccol > -1 && crow > -1 && ccol < cols && crow < rows)
        {
            var cellValue = contents[crow, ccol];
            contents[crow, ccol] = new CellValue(cellValue.Rune, cellValue.Attr, 0);
            dirtyLine[crow] = true;
        }

        //if (ccol == Cols) {
        //	ccol = 0;
        //	if (crow + 1 < Rows)
        //		crow++;
        //}
        if (sync) UpdateScreen();
    }

    public override void AddStr(string str)
    {
        foreach (var rune in str.EnumerateRunes())
            AddRune(rune);
    }

    public override void End()
    {
        NetWinConsole?.Cleanup();

        StopReportingMouseMoves();
        Console.ResetColor();
        Clear();
    }

    private void Clear()
    {
        if (Rows > 0)
        {
            Console.Clear();
            Console.Out.Write("\x1b[3J");
            //Console.Out.Write ("\x1b[?25l");
        }
    }

    public override void Init(Action terminalResized)
    {
        TerminalResized = terminalResized;

        Console.TreatControlCAsInput = true;

        cols = Console.WindowWidth;
        rows = Console.WindowHeight;

        Clear();
        ResizeScreen();
        UpdateOffScreen();

        StartReportingMouseMoves();

        Colors.TopLevel = new ColorScheme();
        Colors.Base = new ColorScheme();
        Colors.Dialog = new ColorScheme();
        Colors.Menu = new ColorScheme();
        Colors.Error = new ColorScheme();

        Colors.TopLevel.Normal = new(ConsoleColor.Green, ConsoleColor.Black);
        Colors.TopLevel.Focus = new(ConsoleColor.White, ConsoleColor.DarkCyan);
        Colors.TopLevel.HotNormal = new(ConsoleColor.DarkYellow, ConsoleColor.Black);
        Colors.TopLevel.HotFocus = new(ConsoleColor.DarkBlue, ConsoleColor.DarkCyan);
        Colors.TopLevel.Disabled = new(ConsoleColor.DarkGray, ConsoleColor.Black);

        Colors.Base.Normal = new(ConsoleColor.White, ConsoleColor.DarkBlue);
        Colors.Base.Focus = new(ConsoleColor.Black, ConsoleColor.Gray);
        Colors.Base.HotNormal = new(ConsoleColor.DarkCyan, ConsoleColor.DarkBlue);
        Colors.Base.HotFocus = new(ConsoleColor.Blue, ConsoleColor.Gray);
        Colors.Base.Disabled = new(ConsoleColor.DarkGray, ConsoleColor.DarkBlue);

        // Focused,
        //    Selected, Hot: Yellow on Black
        //    Selected, text: white on black
        //    Unselected, hot: yellow on cyan
        //    unselected, text: same as unfocused
        Colors.Menu.Normal = new(ConsoleColor.White, ConsoleColor.DarkGray);
        Colors.Menu.Focus = new(ConsoleColor.White, ConsoleColor.Black);
        Colors.Menu.HotNormal = new(ConsoleColor.Yellow, ConsoleColor.DarkGray);
        Colors.Menu.HotFocus = new(ConsoleColor.Yellow, ConsoleColor.Black);
        Colors.Menu.Disabled = new(ConsoleColor.Gray, ConsoleColor.DarkGray);

        Colors.Dialog.Normal = new(ConsoleColor.Black, ConsoleColor.Gray);
        Colors.Dialog.Focus = new(ConsoleColor.White, ConsoleColor.DarkGray);
        Colors.Dialog.HotNormal = new(ConsoleColor.DarkBlue, ConsoleColor.Gray);
        Colors.Dialog.HotFocus = new(ConsoleColor.DarkBlue, ConsoleColor.DarkGray);
        Colors.Dialog.Disabled = new(ConsoleColor.DarkGray, ConsoleColor.Gray);

        Colors.Error.Normal = new(ConsoleColor.DarkRed, ConsoleColor.White);
        Colors.Error.Focus = new(ConsoleColor.White, ConsoleColor.DarkRed);
        Colors.Error.HotNormal = new(ConsoleColor.Black, ConsoleColor.White);
        Colors.Error.HotFocus = new(ConsoleColor.Black, ConsoleColor.DarkRed);
        Colors.Error.Disabled = new(ConsoleColor.DarkGray, ConsoleColor.White);
    }

    private void ResizeScreen()
    {
        if (!HeightAsBuffer)
        {
            if (Console.WindowHeight > 0) // Can raise an exception while is still resizing.
                try
                {
                    // Not supported on Unix.
                    if (OperatingSystem.IsWindows())
                    {
#pragma warning disable CA1416
                        Console.CursorTop = 0;
                        Console.CursorLeft = 0;
                        Console.WindowTop = 0;
                        Console.WindowLeft = 0;
                        Console.SetBufferSize(Cols, Rows);
#pragma warning restore CA1416
                    }
                    else
                    {
                        //Console.Out.Write ($"\x1b[8;{Console.WindowHeight};{Console.WindowWidth}t");
                        Console.Out.Write(
                            $"\x1b[0;0" +
                            $";{Rows};{Cols}w");
                    }
                }
                catch (System.IO.IOException)
                {
                    return;
                }
                catch (ArgumentOutOfRangeException)
                {
                    return;
                }
        }
        else
        {
            if (OperatingSystem.IsWindows() && Console.WindowHeight > 0) // Can raise an exception while is still resizing.
                try
                {
#pragma warning disable CA1416
                    Console.WindowTop = Math.Max(Math.Min(top, Rows - Console.WindowHeight), 0);
#pragma warning restore CA1416
                }
                catch (Exception)
                {
                    return;
                }
            else
                Console.Out.Write(
                    $"\x1b[{top};{Console.WindowLeft}" +
                    $";{Rows};{Cols}w");
        }

        Clip = new Rectangle(0, 0, Cols, Rows);

        contents = new CellValue[Rows, Cols];
        dirtyLine = new bool[Rows];
    }

    private void UpdateOffScreen()
    {
        // Can raise an exception while is still resizing.
        try
        {
            for (var row = 0; row < rows; row++)
            {
                for (var c = 0; c < cols; c++)
                {
                    contents[row, c] = new CellValue((Rune)' ', Colors.TopLevel.Normal, 0);
                    dirtyLine[row] = true;
                }
            }
        }
        catch (IndexOutOfRangeException) { }

        winChanging = false;
    }

    private CellAttributes redrawColor = CellAttributes.Empty;

    private void SetColor(CellAttributes color)
    {
        redrawColor = color;
        Console.ForegroundColor = color.Foreground;
        Console.BackgroundColor = color.Background;
    }

    public override void Refresh() { UpdateScreen(); }

    public override void UpdateScreen()
    {
        if (winChanging || Console.WindowHeight == 0 || contents.Length != Rows * Cols * 3
            || !HeightAsBuffer && Rows != Console.WindowHeight
            || HeightAsBuffer && Rows != largestWindowHeight)
            return;

        var top = Top;
        var rows = Math.Min(Console.WindowHeight + top, Rows);
        var cols = Cols;

        Console.CursorVisible = false;
        for (var row = top; row < rows; row++)
        {
            if (!dirtyLine[row]) continue;
            dirtyLine[row] = false;
            for (var col = 0; col < cols; col++)
            {
                if (contents[row, col].X != 1) continue;
                if (Console.WindowHeight > 0 && !SetCursorPosition(col, row)) return;
                for (; col < cols && contents[row, col].X == 1; col++)
                {
                    var cellValue = contents[row, col];
                    var color = cellValue.Attr;
                    if (color != redrawColor) 
                        SetColor(color);

                    if (AlwaysSetPosition && !SetCursorPosition(col, row)) 
                        return;

                    Console.Write(cellValue.Rune);

                    contents[row, col] = new CellValue(cellValue.Rune, cellValue.Attr, 0);
                }
            }
        }

        Console.CursorVisible = true;
        UpdateCursor();
    }

    private bool SetCursorPosition(int col, int row)
    {
        // Could happens that the windows is still resizing and the col is bigger than Console.WindowWidth.
        try
        {
            Console.SetCursorPosition(col, row);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override void UpdateCursor()
    {
        // Prevents the exception of size changing during resizing.
        try
        {
            if (ccol >= 0 && ccol <= cols && crow >= 0 && crow <= rows) Console.SetCursorPosition(ccol, crow);
        }
        catch (System.IO.IOException) { }
        catch (ArgumentOutOfRangeException) { }
    }

    public override void StartReportingMouseMoves()
    {
        Console.Out.Write("\x1b[?1003h\x1b[?1015h\x1b[?1006h");
        Console.Out.Flush();
    }

    public override void StopReportingMouseMoves()
    {
        Console.Out.Write("\x1b[?1003l\x1b[?1015l\x1b[?1006l");
        Console.Out.Flush();
    }

    public override void Suspend() { }

    private CellAttributes currentAttribute;
    public override void SetAttribute(CellAttributes c) { currentAttribute = c; }

    private Key MapKey(ConsoleKeyInfo keyInfo)
    {
        MapKeyModifiers(keyInfo, (Key)keyInfo.Key);
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
                return (Key)keyInfo.KeyChar;
        }

        var key = keyInfo.Key;
        if (key >= ConsoleKey.A && key <= ConsoleKey.Z)
        {
            var delta = key - ConsoleKey.A;
            if (keyInfo.Modifiers == ConsoleModifiers.Control)
                return (Key)((uint)Key.CtrlMask | (uint)Key.A + delta);
            if (keyInfo.Modifiers == ConsoleModifiers.Alt) return (Key)((uint)Key.AltMask | (uint)Key.A + delta);
            if ((keyInfo.Modifiers & (ConsoleModifiers.Alt | ConsoleModifiers.Control)) != 0)
                if (keyInfo.KeyChar == 0 || keyInfo.KeyChar != 0 && keyInfo.KeyChar >= 1 && keyInfo.KeyChar <= 26)
                    return MapKeyModifiers(keyInfo, (Key)((uint)Key.A + delta));
            return (Key)keyInfo.KeyChar;
        }

        if (key >= ConsoleKey.D0 && key <= ConsoleKey.D9)
        {
            var delta = key - ConsoleKey.D0;
            if (keyInfo.Modifiers == ConsoleModifiers.Alt) return (Key)((uint)Key.AltMask | (uint)Key.D0 + delta);
            if (keyInfo.Modifiers == ConsoleModifiers.Control)
                return (Key)((uint)Key.CtrlMask | (uint)Key.D0 + delta);
            if ((keyInfo.Modifiers & (ConsoleModifiers.Alt | ConsoleModifiers.Control)) != 0)
                if (keyInfo.KeyChar == 0 || keyInfo.KeyChar == 30)
                    return MapKeyModifiers(keyInfo, (Key)((uint)Key.D0 + delta));
            return (Key)keyInfo.KeyChar;
        }

        if (key >= ConsoleKey.F1 && key <= ConsoleKey.F12)
        {
            var delta = key - ConsoleKey.F1;
            if ((keyInfo.Modifiers & (ConsoleModifiers.Shift | ConsoleModifiers.Alt | ConsoleModifiers.Control)) !=
                0) return MapKeyModifiers(keyInfo, (Key)((uint)Key.F1 + delta));

            return (Key)((uint)Key.F1 + delta);
        }

        if (keyInfo.KeyChar != 0) return MapKeyModifiers(keyInfo, (Key)keyInfo.KeyChar);

        return (Key)0xffffffff;
    }

    private KeyModifiers keyModifiers;

    private Key MapKeyModifiers(ConsoleKeyInfo keyInfo, Key key)
    {
        var keyMod = new Key();
        if ((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0)
        {
            keyMod = Key.ShiftMask;
            keyModifiers.Shift = true;
        }

        if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
        {
            keyMod |= Key.CtrlMask;
            keyModifiers.Ctrl = true;
        }

        if ((keyInfo.Modifiers & ConsoleModifiers.Alt) != 0)
        {
            keyMod |= Key.AltMask;
            keyModifiers.Alt = true;
        }

        return keyMod != Key.Null ? keyMod | key : key;
    }

    private Action<KeyEvent>? keyHandler;
    private Action<KeyEvent>? keyDownHandler;
    private Action<KeyEvent>? keyUpHandler;
    private Action<MouseEvent>? mouseHandler;

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

        if (mainLoop.Driver is SystemConsoleMainLoop mLoop)
            // Note: Net doesn't support keydown/up events and thus any passed keyDown/UpHandlers will be simulated to be called.
            mLoop.ProcessInput = (e) => ProcessInput(e);
    }

    private void ProcessInput(SystemConsoleEvents.InputResult inputEvent)
    {
        switch (inputEvent.EventType)
        {
            case SystemConsoleEvents.EventType.Key:
                var map = MapKey(inputEvent.ConsoleKeyInfo);
                if (map == (Key)0xffffffff) return;
                keyDownHandler?.Invoke(new KeyEvent(map, keyModifiers));
                keyHandler?.Invoke(new KeyEvent(map, keyModifiers));
                keyUpHandler?.Invoke(new KeyEvent(map, keyModifiers));
                keyModifiers.Clear();
                break;
            case SystemConsoleEvents.EventType.Mouse:
                mouseHandler?.Invoke(ToDriverMouse(inputEvent.MouseEvent));
                break;
            case SystemConsoleEvents.EventType.WindowSize:
                ChangeWin();
                break;
            case SystemConsoleEvents.EventType.WindowPosition:
                var newTop = inputEvent.WindowPositionEvent.Top;
                if (HeightAsBuffer && top != newTop)
                {
                    top = newTop;
                    Refresh();
                }

                break;
        }
    }

    private bool winChanging;

    private void ChangeWin()
    {
        winChanging = true;
        const int Min_WindowWidth = 14;
        var size = new Size();
        if (!HeightAsBuffer)
        {
            size = new Size(
                Math.Max(Min_WindowWidth, Console.WindowWidth),
                Console.WindowHeight);
            top = 0;
        }
        else
        {
            //largestWindowHeight = Math.Max (Console.BufferHeight, largestWindowHeight);
            largestWindowHeight = Console.BufferHeight;
            size = new Size(Console.BufferWidth, largestWindowHeight);
        }

        cols = size.Width;
        rows = size.Height;
        ResizeScreen();
        UpdateOffScreen();
        if (!winChanging) 
            TerminalResized?.Invoke();
    }

    private MouseEvent ToDriverMouse(SystemConsoleEvents.MouseEvent me)
    {
        MouseFlags mouseFlag = 0;

        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button1Pressed) != 0)
            mouseFlag |= MouseFlags.Button1Pressed;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button1Released) != 0)
            mouseFlag |= MouseFlags.Button1Released;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button1Clicked) != 0)
            mouseFlag |= MouseFlags.Button1Clicked;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button1DoubleClicked) != 0)
            mouseFlag |= MouseFlags.Button1DoubleClicked;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button1TripleClicked) != 0)
            mouseFlag |= MouseFlags.Button1TripleClicked;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button2Pressed) != 0)
            mouseFlag |= MouseFlags.Button2Pressed;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button2Released) != 0)
            mouseFlag |= MouseFlags.Button2Released;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button2Clicked) != 0)
            mouseFlag |= MouseFlags.Button2Clicked;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button2DoubleClicked) != 0)
            mouseFlag |= MouseFlags.Button2DoubleClicked;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button2TrippleClicked) != 0)
            mouseFlag |= MouseFlags.Button2TripleClicked;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button3Pressed) != 0)
            mouseFlag |= MouseFlags.Button3Pressed;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button3Released) != 0)
            mouseFlag |= MouseFlags.Button3Released;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button3Clicked) != 0)
            mouseFlag |= MouseFlags.Button3Clicked;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button3DoubleClicked) != 0)
            mouseFlag |= MouseFlags.Button3DoubleClicked;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button3TripleClicked) != 0)
            mouseFlag |= MouseFlags.Button3TripleClicked;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.ButtonWheeledUp) != 0) mouseFlag |= MouseFlags.WheeledUp;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.ButtonWheeledDown) != 0)
            mouseFlag |= MouseFlags.WheeledDown;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.ButtonWheeledLeft) != 0)
            mouseFlag |= MouseFlags.WheeledLeft;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.ButtonWheeledRight) != 0)
            mouseFlag |= MouseFlags.WheeledRight;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button4Pressed) != 0)
            mouseFlag |= MouseFlags.Button4Pressed;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button4Released) != 0)
            mouseFlag |= MouseFlags.Button4Released;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button4Clicked) != 0)
            mouseFlag |= MouseFlags.Button4Clicked;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button4DoubleClicked) != 0)
            mouseFlag |= MouseFlags.Button4DoubleClicked;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.Button4TripleClicked) != 0)
            mouseFlag |= MouseFlags.Button4TripleClicked;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.ReportMousePosition) != 0)
            mouseFlag |= MouseFlags.ReportMousePosition;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.ButtonShift) != 0) mouseFlag |= MouseFlags.ButtonShift;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.ButtonCtrl) != 0) mouseFlag |= MouseFlags.ButtonCtrl;
        if ((me.ButtonState & SystemConsoleEvents.MouseButtonState.ButtonAlt) != 0) mouseFlag |= MouseFlags.ButtonAlt;

        return new MouseEvent()
        {
            X = me.Position.X,
            Y = me.Position.Y,
            Flags = mouseFlag
        };
    }

    public override CellAttributes GetAttribute() { return currentAttribute; }

    /// <inheritdoc/>
    public override bool GetCursorVisibility(out CursorVisibility visibility)
    {
        visibility = CursorVisibility.Default;

        return false;
    }

    /// <inheritdoc/>
    public override bool SetCursorVisibility(CursorVisibility visibility) { return false; }

    /// <inheritdoc/>
    public override bool EnsureCursorVisibility() { return false; }

    public override void SendKeys(char keyChar, ConsoleKey key, bool shift, bool alt, bool control)
    {
        var input = new SystemConsoleEvents.InputResult();
        ConsoleKey ck;
        if (char.IsLetter(keyChar))
            ck = key;
        else
            ck = (ConsoleKey)'\0';
        input.EventType = SystemConsoleEvents.EventType.Key;
        input.ConsoleKeyInfo = new ConsoleKeyInfo(keyChar, ck, shift, alt, control);

        try
        {
            ProcessInput(input);
        }
        catch (OverflowException) { }
    }

    #region Unused

    public override void SetColors(ConsoleColor foreground, ConsoleColor background) { }
    public override void CookMouse() { }
    public override void UncookMouse() { }

    #endregion

    //
    // These are for the .NET driver, but running natively on Windows, wont run
    // on the Mono emulation
    //
}
