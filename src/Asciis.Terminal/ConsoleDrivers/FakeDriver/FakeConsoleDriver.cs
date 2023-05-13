using System.Runtime.InteropServices;
using Asciis.Terminal.ConsoleDrivers.CursesDriver;
using Asciis.Terminal.Core;
using Asciis.Terminal.Core.Clipboard;

// Alias Console to MockConsole so we don't accidentally use Console
using Console = Asciis.Terminal.ConsoleDrivers.FakeDriver.FakeConsole;

namespace Asciis.Terminal.ConsoleDrivers.FakeDriver;

/// <summary>
/// Implements a mock ConsoleDriver for unit testing
/// </summary>
public class FakeConsoleDriver : ConsoleDriver
{
    private int cols, rows, left, top;
    public override int Cols => cols;

    public override int Rows => rows;

    // Only handling left here because not all terminals has a horizontal scroll bar.
    public override int Left => 0;
    public override int Top => 0;
    public override bool HeightAsBuffer { get; set; }
    public override IClipboard Clipboard { get; }

    // The format is rows, columns and 3 values on the last column: Rune, Attribute and Dirty Flag
    private CellValue[,] contents;
    private bool[] dirtyLine;

    /// <summary>
    /// Assists with testing, the format is rows, columns and 3 values on the last column: Rune, Attribute and Dirty Flag
    /// </summary>
    internal override CellValue[,] Contents => contents;

    private void UpdateOffscreen()
    {
        var cols = Cols;
        var rows = Rows;

        contents = new CellValue[rows, cols];
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                contents[r, c] = new CellValue((Rune)' ', new(ConsoleColor.Gray, ConsoleColor.Black), 0);
            }
        }

        dirtyLine = new bool[rows];
        for (var row = 0; row < rows; row++)
            dirtyLine[row] = true;
    }

    private static bool sync = false;

    public FakeConsole FakeConsole = new FakeConsole();

    public FakeConsoleDriver()
    {
        if (OperatingSystem.IsWindows())
            Clipboard = new WindowsClipboard();
        else if (OperatingSystem.IsMacOS())
            Clipboard = new MacOSXClipboard();
        else if (OperatingSystem.IsLinux())
            Clipboard = new CursesClipboard();
    }

    private bool needMove;

    // Current row, and current col, tracked by Move/AddCh only
    private int ccol, crow;

    public override void Move(int col, int row)
    {
        ccol = col;
        crow = row;

        if (Clip.Contains(col, row))
        {
            FakeConsole.CursorTop = row;
            FakeConsole.CursorLeft = col;
            needMove = false;
        }
        else
        {
            FakeConsole.CursorTop = Clip.Y;
            FakeConsole.CursorLeft = Clip.X;
            needMove = true;
        }
    }

    public override void AddRune(char ch) { AddRune((Rune)ch); }

    public override void AddRune(Rune rune)
    {
        rune = MakePrintable(rune);
        if (Clip.Contains(ccol, crow))
        {
            if (needMove) //MockConsole.CursorLeft = ccol;
                          //MockConsole.CursorTop = crow;
                needMove = false;
            contents[crow, ccol] = new CellValue(rune, currentAttribute, 1);
            dirtyLine[crow] = true;
        }
        else
        {
            needMove = true;
        }

        ccol++;
        //if (ccol == Cols) {
        //	ccol = 0;
        //	if (crow + 1 < Rows)
        //		crow++;
        //}
        if (sync)
            UpdateScreen();
    }

    public override void AddStr(string str)
    {
        foreach (var rune in str.EnumerateRunes())
            AddRune(rune);
    }

    public override void End()
    {
        FakeConsole.ResetColor();
        FakeConsole.Clear();
    }

    public override void Init(Action terminalResized)
    {
        TerminalResized = terminalResized;

        cols = FakeConsole.WindowWidth = FakeConsole.BufferWidth = FakeConsole.WIDTH;
        rows = FakeConsole.WindowHeight = FakeConsole.BufferHeight = FakeConsole.HEIGHT;
        FakeConsole.Clear();
        ResizeScreen();
        UpdateOffScreen();

        Colors.TopLevel = new ColorScheme();
        Colors.Base = new ColorScheme();
        Colors.Dialog = new ColorScheme();
        Colors.Menu = new ColorScheme();
        Colors.Error = new ColorScheme();
        Clip = new Rectangle(0, 0, Cols, Rows);

        Colors.TopLevel.Normal = new(System.ConsoleColor.Green, System.ConsoleColor.Black);
        Colors.TopLevel.Focus = new(System.ConsoleColor.White, System.ConsoleColor.DarkCyan);
        Colors.TopLevel.HotNormal = new(System.ConsoleColor.DarkYellow, System.ConsoleColor.Black);
        Colors.TopLevel.HotFocus = new(System.ConsoleColor.DarkBlue, System.ConsoleColor.DarkCyan);
        Colors.TopLevel.Disabled = new(System.ConsoleColor.DarkGray, System.ConsoleColor.Black);

        Colors.Base.Normal = new(System.ConsoleColor.White, System.ConsoleColor.Blue);
        Colors.Base.Focus = new(System.ConsoleColor.Black, System.ConsoleColor.Cyan);
        Colors.Base.HotNormal = new(System.ConsoleColor.Yellow, System.ConsoleColor.Blue);
        Colors.Base.HotFocus = new(System.ConsoleColor.Yellow, System.ConsoleColor.Cyan);
        Colors.Base.Disabled = new(System.ConsoleColor.DarkGray, System.ConsoleColor.DarkBlue);

        // Focused,
        //    Selected, Hot: Yellow on Black
        //    Selected, text: white on black
        //    Unselected, hot: yellow on cyan
        //    unselected, text: same as unfocused
        Colors.Menu.HotFocus = new(System.ConsoleColor.Yellow, System.ConsoleColor.Black);
        Colors.Menu.Focus = new(System.ConsoleColor.White, System.ConsoleColor.Black);
        Colors.Menu.HotNormal = new(System.ConsoleColor.Yellow, System.ConsoleColor.Cyan);
        Colors.Menu.Normal = new(System.ConsoleColor.White, System.ConsoleColor.Cyan);
        Colors.Menu.Disabled = new(System.ConsoleColor.DarkGray, System.ConsoleColor.Cyan);

        Colors.Dialog.Normal = new(System.ConsoleColor.Black, System.ConsoleColor.Gray);
        Colors.Dialog.Focus = new(System.ConsoleColor.Black, System.ConsoleColor.Cyan);
        Colors.Dialog.HotNormal = new(System.ConsoleColor.Blue, System.ConsoleColor.Gray);
        Colors.Dialog.HotFocus = new(System.ConsoleColor.Blue, System.ConsoleColor.Cyan);
        Colors.Dialog.Disabled = new(System.ConsoleColor.DarkGray, System.ConsoleColor.Gray);

        Colors.Error.Normal = new(System.ConsoleColor.White, System.ConsoleColor.Red);
        Colors.Error.Focus = new(System.ConsoleColor.Black, System.ConsoleColor.Gray);
        Colors.Error.HotNormal = new(System.ConsoleColor.Yellow, System.ConsoleColor.Red);
        Colors.Error.HotFocus = new(System.ConsoleColor.Yellow, System.ConsoleColor.Red);
        Colors.Error.Disabled = new(System.ConsoleColor.DarkGray, System.ConsoleColor.White);

        //MockConsole.Clear ();
    }

    private CellAttributes redrawColor = CellAttributes.Empty;

    private void SetColor(CellAttributes color)
    {
        redrawColor = color;
        FakeConsole.BackgroundColor = color.Background;
        FakeConsole.ForegroundColor = color.Foreground;
    }

    public override void UpdateScreen()
    {
        var top = Top;
        var left = Left;
        var rows = Math.Min(FakeConsole.WindowHeight + top, Rows);
        var cols = Cols;

        FakeConsole.CursorTop = 0;
        FakeConsole.CursorLeft = 0;
        for (var row = top; row < rows; row++)
        {
            dirtyLine[row] = false;
            for (var col = left; col < cols; col++)
            {
                var cellValue = contents[row, col];
                contents[row, col] = new CellValue(cellValue.Rune, cellValue.Attr, 0);
                var color = cellValue.Attr;
                if (color != redrawColor)
                    SetColor(color);
                FakeConsole.Write(cellValue.Rune);
            }
        }
    }

    public override void Refresh()
    {
        var rows = Rows;
        var cols = Cols;

        var savedRow = FakeConsole.CursorTop;
        var savedCol = FakeConsole.CursorLeft;
        for (var row = 0; row < rows; row++)
        {
            if (!dirtyLine[row])
                continue;
            dirtyLine[row] = false;
            for (var col = 0; col < cols; col++)
            {
                if (contents[row, col].X != 1)
                    continue;

                FakeConsole.CursorTop = row;
                FakeConsole.CursorLeft = col;
                for (; col < cols && contents[row, col].X == 1; col++)
                {
                    var cellValue = contents[row, col];
                    var color = cellValue.Attr;
                    if (color != redrawColor)
                        SetColor(color);

                    FakeConsole.Write(cellValue.Rune);

                    contents[row, col] = new CellValue(cellValue.Rune, cellValue.Attr, 0);
                }
            }
        }

        FakeConsole.CursorTop = savedRow;
        FakeConsole.CursorLeft = savedCol;
    }

    private CellAttributes currentAttribute;
    public override void SetAttribute(CellAttributes c) { currentAttribute = c; }

    private Key MapKey(ConsoleKeyInfo keyInfo)
    {
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
                if (keyInfo.KeyChar == 0)
                    return Key.Unknown;

                return (Key)(uint)keyInfo.KeyChar;
        }

        var key = keyInfo.Key;
        if (key >= ConsoleKey.A && key <= ConsoleKey.Z)
        {
            var delta = key - ConsoleKey.A;
            if (keyInfo.Modifiers == ConsoleModifiers.Control)
                return (Key)((uint)Key.CtrlMask | ((uint)Key.A + delta));
            if (keyInfo.Modifiers == ConsoleModifiers.Alt) return (Key)((uint)Key.AltMask | ((uint)Key.A + delta));
            if ((keyInfo.Modifiers & (ConsoleModifiers.Alt | ConsoleModifiers.Control)) != 0)
            {
                if (keyInfo.KeyChar == 0)
                    return (Key)((uint)Key.AltMask | (uint)Key.CtrlMask | ((uint)Key.A + delta));
                else
                    return (Key)(uint)keyInfo.KeyChar;
            }

            return (Key)(uint)keyInfo.KeyChar;
        }

        if (key >= ConsoleKey.D0 && key <= ConsoleKey.D9)
        {
            var delta = key - ConsoleKey.D0;
            if (keyInfo.Modifiers == ConsoleModifiers.Alt) return (Key)((uint)Key.AltMask | ((uint)Key.D0 + delta));
            if (keyInfo.Modifiers == ConsoleModifiers.Control)
                return (Key)((uint)Key.CtrlMask | ((uint)Key.D0 + delta));
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

    private KeyModifiers keyModifiers;

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

    private Action<KeyEvent> keyHandler;
    private Action<KeyEvent> keyUpHandler;

    public override void PrepareToRun(
        MainLoop mainLoop,
        Action<KeyEvent> keyHandler,
        Action<KeyEvent> keyDownHandler,
        Action<KeyEvent> keyUpHandler,
        Action<MouseEvent> mouseHandler)
    {
        this.keyHandler = keyHandler;
        this.keyUpHandler = keyUpHandler;

        // Note: Net doesn't support keydown/up events and thus any passed keyDown/UpHandlers will never be called
        if (mainLoop.Driver is FakeMainLoopDriver fakeMainLoop)
            fakeMainLoop.KeyPressed += (consoleKey) => ProcessInput(consoleKey);
    }

    private void ProcessInput(ConsoleKeyInfo consoleKey)
    {
        var map = MapKey(consoleKey);
        if (map == (Key)0xffffffff)
            return;

        if (consoleKey.Modifiers.HasFlag(ConsoleModifiers.Alt)) keyModifiers.Alt = true;
        if (consoleKey.Modifiers.HasFlag(ConsoleModifiers.Shift)) keyModifiers.Shift = true;
        if (consoleKey.Modifiers.HasFlag(ConsoleModifiers.Control)) keyModifiers.Ctrl = true;

        keyHandler(new KeyEvent(map, keyModifiers));
        keyUpHandler(new KeyEvent(map, keyModifiers));
        keyModifiers.Clear();
    }

    public override CellAttributes GetAttribute() { return currentAttribute; }

    /// <inheritdoc/>
    public override bool GetCursorVisibility(out CursorVisibility visibility)
    {
        if (FakeConsole.CursorVisible)
            visibility = CursorVisibility.Default;
        else
            visibility = CursorVisibility.Invisible;

        return false;
    }

    /// <inheritdoc/>
    public override bool SetCursorVisibility(CursorVisibility visibility)
    {
        if (visibility == CursorVisibility.Invisible)
            FakeConsole.CursorVisible = false;
        else
            FakeConsole.CursorVisible = true;

        return false;
    }

    /// <inheritdoc/>
    public override bool EnsureCursorVisibility() { return false; }

    public override void SendKeys(char keyChar, ConsoleKey key, bool shift, bool alt, bool control)
    {
        ProcessInput(new ConsoleKeyInfo(keyChar, key, shift, alt, control));
    }

    public void SetBufferSize(int width, int height)
    {
        cols = FakeConsole.WindowWidth = FakeConsole.BufferWidth = width;
        rows = FakeConsole.WindowHeight = FakeConsole.BufferHeight = height;
        ProcessResize();
    }

    public void SetWindowSize(int width, int height)
    {
        FakeConsole.WindowWidth = width;
        FakeConsole.WindowHeight = height;
        if (width > cols || !HeightAsBuffer) cols = FakeConsole.BufferWidth = width;
        if (height > rows || !HeightAsBuffer) rows = FakeConsole.BufferHeight = height;
        ProcessResize();
    }

    public void SetWindowPosition(int left, int top)
    {
        if (HeightAsBuffer)
        {
            this.left = FakeConsole.WindowLeft = Math.Max(Math.Min(left, Cols - FakeConsole.WindowWidth), 0);
            this.top = FakeConsole.WindowTop = Math.Max(Math.Min(top, Rows - FakeConsole.WindowHeight), 0);
        }
        else if (this.left > 0 || this.top > 0)
        {
            this.left = FakeConsole.WindowLeft = 0;
            this.top = FakeConsole.WindowTop = 0;
        }
    }

    private void ProcessResize()
    {
        ResizeScreen();
        UpdateOffScreen();
        TerminalResized?.Invoke();
    }

    private void ResizeScreen()
    {
        if (!HeightAsBuffer)
        {
            if (FakeConsole.WindowHeight > 0) // Can raise an exception while is still resizing.
                try
                {
#pragma warning disable CA1416
                    FakeConsole.CursorTop = 0;
                    FakeConsole.CursorLeft = 0;
                    FakeConsole.WindowTop = 0;
                    FakeConsole.WindowLeft = 0;
#pragma warning restore CA1416
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
            try
            {
#pragma warning disable CA1416
                FakeConsole.WindowLeft = Math.Max(Math.Min(left, Cols - FakeConsole.WindowWidth), 0);
                FakeConsole.WindowTop = Math.Max(Math.Min(top, Rows - FakeConsole.WindowHeight), 0);
#pragma warning restore CA1416
            }
            catch (Exception)
            {
                return;
            }
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
    }

    #region Unused

    public override void UpdateCursor() { }
    public override void StartReportingMouseMoves() { }
    public override void StopReportingMouseMoves() { }
    public override void Suspend() { }
    public override void SetColors(ConsoleColor foreground, ConsoleColor background) { }
    public override void CookMouse() { }
    public override void UncookMouse() { }

    #endregion

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
