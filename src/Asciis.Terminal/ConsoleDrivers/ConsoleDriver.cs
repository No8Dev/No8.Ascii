using Asciis.Terminal.ConsoleDrivers;
using Asciis.Terminal.ConsoleDrivers.CursesDriver;
using Asciis.Terminal.ConsoleDrivers.NetDriver;
using Asciis.Terminal.Core.Clipboard;
using Asciis.Terminal.Helpers;

namespace Asciis.Terminal.Core;


/// <summary>
/// ConsoleDriver is an abstract class that defines the requirements for a console driver.  
/// There are currently three implementations: <see cref="CursesDriver"/> (for Unix and Mac), <see cref="WindowsDriver"/>, and <see cref="SystemConsoleDriver"/> that uses the .NET Console API.
/// </summary>
public abstract class ConsoleDriver
{
    /// <summary>
    /// The handler fired when the terminal is resized.
    /// </summary>
    protected Action? TerminalResized;

    /// <summary>
    /// The current number of columns in the terminal.
    /// </summary>
    public abstract int Cols { get; }

    /// <summary>
    /// The current number of rows in the terminal.
    /// </summary>
    public abstract int Rows { get; }

    /// <summary>
    /// The current left in the terminal.
    /// </summary>
    public abstract int Left { get; }

    /// <summary>
    /// The current top in the terminal.
    /// </summary>
    public abstract int Top { get; }

    /// <summary>
    /// Get the operation system clipboard.
    /// </summary>
    public abstract IClipboard? Clipboard { get; }

    /// <summary>
    /// If false height is measured by the window height and thus no scrolling.
    /// If true then height is measured by the buffer height, enabling scrolling.
    /// </summary>
    public abstract bool HeightAsBuffer { get; set; }

    protected bool alwaysSetPosition;
    public virtual bool AlwaysSetPosition
    {
        get => alwaysSetPosition;
        set { }     // default is to ignore any update
    }

    // The format is rows, columns and 3 values on the last column: Rune, Attribute and Dirty Flag
    internal abstract CellValue[,]? Contents { get; }

    public ConsoleDriver()
    {
    }


    /// <summary>
    /// Initializes the driver
    /// </summary>
    /// <param name="terminalResized">Method to invoke when the terminal is resized.</param>
    public abstract void Init(Action terminalResized);

    /// <summary>
    /// Moves the cursor to the specified column and row.
    /// </summary>
    /// <param name="col">Column to move the cursor to.</param>
    /// <param name="row">Row to move the cursor to.</param>
    public abstract void Move(int col, int row);

    /// <summary>
    /// Adds the specified rune to the display at the current cursor position
    /// </summary>
    /// <param name="rune">Rune to add.</param>
    public abstract void AddRune(Rune rune);

    public abstract void AddRune(char ch);

    /// <summary>
    /// Ensures a Rune is not a control character and can be displayed by translating characters below 0x20
    /// to equivalent, printable, Unicode chars.
    /// </summary>
    /// <param name="c">Rune to translate</param>
    /// <returns></returns>
    public static Rune MakePrintable(Rune c)
    {
        if (c.Value <= 0x1F || c.Value >= 0x80 && c.Value <= 0x9F)
            // ASCII (C0) control characters.
            // C1 control characters (https://www.aivosto.com/articles/control-characters.html#c1)
            return (Rune)(c.Value + 0x2400);
        else
            return c;
    }

    /// <summary>
    /// Adds the specified
    /// </summary>
    /// <param name="str">String.</param>
    public abstract void AddStr(string str);

    /// <summary>
    /// Prepare the driver and set the key and mouse events handlers.
    /// </summary>
    /// <param name="mainLoop">The main loop.</param>
    /// <param name="keyHandler">The handler for ProcessKey</param>
    /// <param name="keyDownHandler">The handler for key down events</param>
    /// <param name="keyUpHandler">The handler for key up events</param>
    /// <param name="mouseHandler">The handler for mouse events</param>
    public abstract void PrepareToRun(
        MainLoop mainLoop,
        Action<KeyEvent> keyHandler,
        Action<KeyEvent> keyDownHandler,
        Action<KeyEvent> keyUpHandler,
        Action<MouseEvent> mouseHandler);

    /// <summary>
    /// Updates the screen to reflect all the changes that have been done to the display buffer
    /// </summary>
    public abstract void Refresh();

    /// <summary>
    /// Updates the location of the cursor position
    /// </summary>
    public abstract void UpdateCursor();

    /// <summary>
    /// Retreive the cursor caret visibility
    /// </summary>
    /// <param name="visibility">The current <see cref="CursorVisibility"/></param>
    /// <returns>true upon success</returns>
    public abstract bool GetCursorVisibility(out CursorVisibility visibility);

    /// <summary>
    /// Change the cursor caret visibility
    /// </summary>
    /// <param name="visibility">The wished <see cref="CursorVisibility"/></param>
    /// <returns>true upon success</returns>
    public abstract bool SetCursorVisibility(CursorVisibility visibility);

    /// <summary>
    /// Ensure the cursor visibility
    /// </summary>
    /// <returns>true upon success</returns>
    public abstract bool EnsureCursorVisibility();

    /// <summary>
    /// Ends the execution of the console driver.
    /// </summary>
    public abstract void End();

    /// <summary>
    /// Redraws the physical screen with the contents that have been queued up via any of the printing commands.
    /// </summary>
    public abstract void UpdateScreen();

    /// <summary>
    /// Selects the specified attribute as the attribute to use for future calls to AddRune, AddString.
    /// </summary>
    /// <param name="c">C.</param>
    public abstract void SetAttribute(CellAttributes c);

    /// <summary>
    /// Set Colors from limit sets of colors.
    /// </summary>
    /// <param name="foreground">Foreground.</param>
    /// <param name="background">Background.</param>
    public abstract void SetColors(ConsoleColor foreground, ConsoleColor background);

    /// <summary>
    /// Allows sending keys without typing on a keyboard.
    /// </summary>
    /// <param name="keyChar">The character key.</param>
    /// <param name="key">The key.</param>
    /// <param name="shift">If shift key is sending.</param>
    /// <param name="alt">If alt key is sending.</param>
    /// <param name="control">If control key is sending.</param>
    public abstract void SendKeys(char keyChar, ConsoleKey key, bool shift, bool alt, bool control);

    /// <summary>
    /// Set the handler when the terminal is resized.
    /// </summary>
    /// <param name="terminalResized"></param>
    public void SetTerminalResized(Action terminalResized) { TerminalResized = terminalResized; }

    /// <summary>
    /// Draws the title for a Window-style view incorporating padding. 
    /// </summary>
    /// <param name="region">Screen relative region where the frame will be drawn.</param>
    /// <param name="title">The title for the window. The title will only be drawn if <c>title</c> is not null or empty and paddingTop is greater than 0.</param>
    /// <param name="paddingLeft">Number of columns to pad on the left (if 0 the border will not appear on the left).</param>
    /// <param name="paddingTop">Number of rows to pad on the top (if 0 the border and title will not appear on the top).</param>
    /// <param name="paddingRight">Number of columns to pad on the right (if 0 the border will not appear on the right).</param>
    /// <param name="paddingBottom">Number of rows to pad on the bottom (if 0 the border will not appear on the bottom).</param>
    /// <param name="textAlignment">Not yet implemented.</param>
    /// <remarks></remarks>
    public virtual void DrawWindowTitle(
        Rectangle region,
        string title,
        int paddingLeft,
        int paddingTop,
        int paddingRight,
        int paddingBottom,
        TextAlignment textAlignment = TextAlignment.Left)
    {
        var width = region.Width - (paddingLeft + 2) * 2;
        if (!string.IsNullOrEmpty(title) && width > 4 && region.Y + paddingTop <= region.Y + paddingBottom)
        {
            Move(region.X + 1 + paddingLeft, region.Y + paddingTop);
            AddRune((Rune)' ');
            var str = title.RuneCount() >= width ? title.Substring(0, width - 2) : title;
            AddStr(str);
            AddRune(' ');
        }
    }

    /// <summary>
    /// Enables diagnostic functions
    /// </summary>
    [Flags]
    public enum DiagnosticFlags : uint
    {
        /// <summary>
        /// All diagnostics off
        /// </summary>
        Off = 0b_0000_0000,

        /// <summary>
        /// When enabled, <see cref="ConsoleDriver.DrawWindowFrame(Rectangle, int, int, int, int, bool, bool, Border)"/> will draw a 
        /// ruler in the frame for any side with a padding value greater than 0.
        /// </summary>
        FrameRuler = 0b_0000_0001,

        /// <summary>
        /// When Enabled, <see cref="ConsoleDriver.DrawWindowFrame(Rectangle, int, int, int, int, bool, bool, Border)"/> will use
        /// 'L', 'R', 'T', and 'B' for padding instead of ' '.
        /// </summary>
        FramePadding = 0b_0000_0010
    }

    /// <summary>
    /// Set flags to enable/disable <see cref="ConsoleDriver"/> diagnostics.
    /// </summary>
    public static DiagnosticFlags Diagnostics { get; set; }

    /// <summary>
    /// Draws a frame for a window with padding and an optional visible border inside the padding. 
    /// </summary>
    /// <param name="region">Screen relative region where the frame will be drawn.</param>
    /// <param name="paddingLeft">Number of columns to pad on the left (if 0 the border will not appear on the left).</param>
    /// <param name="paddingTop">Number of rows to pad on the top (if 0 the border and title will not appear on the top).</param>
    /// <param name="paddingRight">Number of columns to pad on the right (if 0 the border will not appear on the right).</param>
    /// <param name="paddingBottom">Number of rows to pad on the bottom (if 0 the border will not appear on the bottom).</param>
    /// <param name="border">If set to <c>true</c> and any padding dimension is > 0 the border will be drawn.</param>
    /// <param name="fill">If set to <c>true</c> it will clear the content area (the area inside the padding) with the current color, otherwise the content area will be left untouched.</param>
    /// <param name="borderContent">The <see cref="Border"/> to be used if defined.</param>
    public virtual void DrawWindowFrame(
        Rectangle region,
        int paddingLeft = 0,
        int paddingTop = 0,
        int paddingRight = 0,
        int paddingBottom = 0,
        bool border = true,
        bool fill = false,
        Border? borderContent = null)
    {
        var clearChar = (Rune)' ';
        var leftChar = clearChar;
        var rightChar = clearChar;
        var topChar = clearChar;
        var bottomChar = clearChar;

        if ((Diagnostics & DiagnosticFlags.FramePadding) == DiagnosticFlags.FramePadding)
        {
            leftChar = (Rune)'L';
            rightChar = (Rune)'R';
            topChar = (Rune)'T';
            bottomChar = (Rune)'B';
            clearChar = (Rune)'C';
        }

        void AddRuneAt(int col, int row, Rune ch)
        {
            Move(col, row);
            AddRune(ch);
        }

        // fwidth is count of hLine chars
        var fwidth = (int)(region.Width - (paddingRight + paddingLeft));

        // fheight is count of vLine chars
        var fheight = (int)(region.Height - (paddingBottom + paddingTop));

        // fleft is location of left frame line
        var fleft = region.X + paddingLeft - 1;

        // fright is location of right frame line
        var fright = fleft + fwidth + 1;

        // ftop is location of top frame line
        var ftop = region.Y + paddingTop - 1;

        // fbottom is location of bottom frame line
        var fbottom = ftop + fheight + 1;

        var borderStyle = borderContent == null ? BorderStyle.Single : borderContent.BorderStyle;

        var hLine = border ? borderStyle == BorderStyle.Single ? HLine : HDLine : clearChar;
        var vLine = border ? borderStyle == BorderStyle.Single ? VLine : VDLine : clearChar;
        var uRCorner = border ? borderStyle == BorderStyle.Single ? URCorner : URDCorner : clearChar;
        var uLCorner = border ? borderStyle == BorderStyle.Single ? ULCorner : ULDCorner : clearChar;
        var lLCorner = border ? borderStyle == BorderStyle.Single ? LLCorner : LLDCorner : clearChar;
        var lRCorner = border ? borderStyle == BorderStyle.Single ? LRCorner : LRDCorner : clearChar;

        // Outside top
        if (paddingTop > 1)
            for (var r = region.Y; r < ftop; r++)
            {
                for (var c = region.X; c < region.X + region.Width; c++) AddRuneAt(c, r, topChar);
            }

        // Outside top-left
        for (var c = region.X; c < fleft; c++) AddRuneAt(c, ftop, leftChar);

        // Frame top-left corner
        AddRuneAt(fleft, ftop, paddingTop >= 0 ? paddingLeft >= 0 ? uLCorner : hLine : leftChar);

        // Frame top
        for (var c = fleft + 1; c < fleft + 1 + fwidth; c++) AddRuneAt(c, ftop, paddingTop > 0 ? hLine : topChar);

        // Frame top-right corner
        if (fright > fleft)
            AddRuneAt(fright, ftop, paddingTop >= 0 ? paddingRight >= 0 ? uRCorner : hLine : rightChar);

        // Outside top-right corner
        for (var c = fright + 1; c < fright + paddingRight; c++) AddRuneAt(c, ftop, rightChar);

        // Left, Fill, Right
        if (fbottom > ftop)
        {
            for (var r = ftop + 1; r < fbottom; r++)
            {
                // Outside left
                for (var c = region.X; c < fleft; c++) AddRuneAt(c, r, leftChar);

                // Frame left
                AddRuneAt(fleft, r, paddingLeft > 0 ? vLine : leftChar);

                // Fill
                if (fill)
                    for (var x = fleft + 1; x < fright; x++)
                        AddRuneAt(x, r, clearChar);

                // Frame right
                if (fright > fleft)
                {
                    var v = vLine;
                    if ((Diagnostics & DiagnosticFlags.FrameRuler) == DiagnosticFlags.FrameRuler)
                        v = (Rune)(char)('0' + (r - ftop) % 10); // vLine;
                    AddRuneAt(fright, r, paddingRight > 0 ? v : rightChar);
                }

                // Outside right
                for (var c = fright + 1; c < fright + paddingRight; c++) AddRuneAt(c, r, rightChar);
            }

            // Outside Bottom
            for (var c = region.X; c < region.X + region.Width; c++) AddRuneAt(c, fbottom, leftChar);

            // Frame bottom-left
            AddRuneAt(fleft, fbottom, paddingLeft > 0 ? lLCorner : leftChar);

            if (fright > fleft)
            {
                // Frame bottom
                for (var c = fleft + 1; c < fright; c++)
                {
                    var h = hLine;
                    if ((Diagnostics & DiagnosticFlags.FrameRuler) == DiagnosticFlags.FrameRuler)
                        h = (Rune)(char)('0' + (c - fleft) % 10); // hLine;
                    AddRuneAt(c, fbottom, paddingBottom > 0 ? h : bottomChar);
                }

                // Frame bottom-right
                AddRuneAt(fright, fbottom, paddingRight > 0 ? paddingBottom > 0 ? lRCorner : hLine : rightChar);
            }

            // Outside right
            for (var c = fright + 1; c < fright + paddingRight; c++) AddRuneAt(c, fbottom, rightChar);
        }

        // Out bottom - ensure top is always drawn if we overlap
        if (paddingBottom > 0)
            for (var r = fbottom + 1; r < fbottom + paddingBottom; r++)
            {
                for (var c = region.X; c < region.X + region.Width; c++) AddRuneAt(c, r, bottomChar);
            }
    }

    /// <summary>
    /// Draws a frame on the specified region with the specified padding around the frame.
    /// </summary>
    /// <param name="region">Screen relative region where the frame will be drawn.</param>
    /// <param name="padding">Padding to add on the sides.</param>
    /// <param name="fill">If set to <c>true</c> it will clear the contents with the current color, otherwise the contents will be left untouched.</param>
    /// <remarks>This API has been superseded by <see cref="ConsoleDriver.DrawWindowFrame(Rectangle, int, int, int, int, bool, bool, Border)"/>.</remarks>
    /// <remarks>This API is equivalent to calling <c>DrawWindowFrame(Rect, p - 1, p - 1, p - 1, p - 1)</c>. In other words,
    /// A padding value of 0 means there is actually a one cell border.
    /// </remarks>
    public virtual void DrawFrame(Rectangle region, int padding, bool fill)
    {
        // DrawFrame assumes the border is always at least one row/col thick
        // DrawWindowFrame assumes a padding of 0 means NO padding and no frame
        DrawWindowFrame(
            new Rectangle(region.X, region.Y, region.Width, region.Height),
            padding + 1,
            padding + 1,
            padding + 1,
            padding + 1,
            false,
            fill);
    }


    /// <summary>
    /// Suspend the application, typically needs to save the state, suspend the app and upon return, reset the console driver.
    /// </summary>
    public abstract void Suspend();

    private Rectangle clip;

    /// <summary>
    /// Controls the current clipping region that AddRune/AddStr is subject to.
    /// </summary>
    /// <value>The clip.</value>
    public Rectangle Clip
    {
        get => clip;
        set => clip = value;
    }
    public AsciiApplication Application { get; internal set; }

    /// <summary>
    /// Start of mouse moves.
    /// </summary>
    public abstract void StartReportingMouseMoves();

    /// <summary>
    /// Stop reporting mouses moves.
    /// </summary>
    public abstract void StopReportingMouseMoves();

    /// <summary>
    /// Disables the cooked event processing from the mouse driver.  At startup, it is assumed mouse events are cooked.
    /// </summary>
    public abstract void UncookMouse();

    /// <summary>
    /// Enables the cooked event processing from the mouse driver
    /// </summary>
    public abstract void CookMouse();

    /// <summary>
    /// Horizontal line character.
    /// </summary>
    public Rune HLine = Runes.HLine;

    /// <summary>
    /// Vertical line character.
    /// </summary>
    public Rune VLine = Runes.VLine;

    /// <summary>
    /// Stipple pattern
    /// </summary>
    public Rune Stipple = Runes.Stipple;

    /// <summary>
    /// Diamond character
    /// </summary>
    public Rune Diamond = Runes.Diamond;

    /// <summary>
    /// Upper left corner
    /// </summary>
    public Rune ULCorner = Runes.ULCorner;

    /// <summary>
    /// Lower left corner
    /// </summary>
    public Rune LLCorner = Runes.LLCorner;

    /// <summary>
    /// Upper right corner
    /// </summary>
    public Rune URCorner = Runes.URCorner;

    /// <summary>
    /// Lower right corner
    /// </summary>
    public Rune LRCorner = Runes.LRCorner;

    /// <summary>
    /// Left tee
    /// </summary>
    public Rune LeftTee = Runes.LeftTee;

    /// <summary>
    /// Right tee
    /// </summary>
    public Rune RightTee = Runes.RightTee;

    /// <summary>
    /// Top tee
    /// </summary>
    public Rune TopTee = Runes.TopTee;

    /// <summary>
    /// The bottom tee.
    /// </summary>
    public Rune BottomTee = Runes.BottomTee;

    /// <summary>
    /// Checkmark.
    /// </summary>
    public Rune Checked = Runes.Checked;

    /// <summary>
    /// Un-checked checkmark.
    /// </summary>
    public Rune UnChecked = Runes.UnChecked;

    /// <summary>
    /// Selected mark.
    /// </summary>
    public Rune Selected = Runes.Selected;

    /// <summary>
    /// Un-selected selected mark.
    /// </summary>
    public Rune UnSelected = Runes.UnSelected;

    /// <summary>
    /// Right Arrow.
    /// </summary>
    public Rune RightArrow = Runes.RightArrow;

    /// <summary>
    /// Left Arrow.
    /// </summary>
    public Rune LeftArrow = Runes.LeftArrow;

    /// <summary>
    /// Down Arrow.
    /// </summary>
    public Rune DownArrow = Runes.DownArrow;

    /// <summary>
    /// Up Arrow.
    /// </summary>
    public Rune UpArrow = Runes.UpArrow;

    /// <summary>
    /// Left indicator for default action (e.g. for <see cref="Button"/>).
    /// </summary>
    public Rune LeftDefaultIndicator = Runes.LeftDefaultIndicator;

    /// <summary>
    /// Right indicator for default action (e.g. for <see cref="Button"/>).
    /// </summary>
    public Rune RightDefaultIndicator = Runes.RightDefaultIndicator;

    /// <summary>
    /// Left frame/bracket (e.g. '[' for <see cref="Button"/>).
    /// </summary>
    public Rune LeftBracket = Runes.LeftBracket;

    /// <summary>
    /// Right frame/bracket (e.g. ']' for <see cref="Button"/>).
    /// </summary>
    public Rune RightBracket = Runes.RightBracket;

    /// <summary>
    /// Blocks Segment indicator for meter views (e.g. <see cref="ProgressBar"/>.
    /// </summary>
    public Rune BlocksMeterSegment = Runes.BlocksMeterSegment;

    /// <summary>
    /// Continuous Segment indicator for meter views (e.g. <see cref="ProgressBar"/>.
    /// </summary>
    public Rune ContinuousMeterSegment = Runes.ContinuousMeterSegment;

    /// <summary>
    /// Horizontal double line character.
    /// </summary>
    public Rune HDLine = Runes.HDLine;

    /// <summary>
    /// Vertical double line character.
    /// </summary>
    public Rune VDLine = Runes.VDLine;

    /// <summary>
    /// Upper left double corner
    /// </summary>
    public Rune ULDCorner = Runes.ULDCorner;

    /// <summary>
    /// Lower left double corner
    /// </summary>
    public Rune LLDCorner = Runes.LLDCorner;

    /// <summary>
    /// Upper right double corner
    /// </summary>
    public Rune URDCorner = Runes.URDCorner;

    /// <summary>
    /// Lower right double corner
    /// </summary>
    public Rune LRDCorner = Runes.LRDCorner;

    /// <summary>
    /// Make the attribute for the foreground and background colors.
    /// </summary>
    /// <param name="fore">Foreground.</param>
    /// <param name="back">Background.</param>
    /// <returns></returns>
    public CellAttributes MakeCellColor(ConsoleColor fore, ConsoleColor back) => new CellAttributes(fore, back);

    /// <summary>
    /// Gets the current <see cref="CellAttributes"/>.
    /// </summary>
    /// <returns>The current attribute.</returns>
    public abstract CellAttributes GetAttribute();


    public string Dump()
    {
        var sb = new StringBuilder();
        DumpRuler(sb);
        DumpRunes(sb);
        DumpRuler(sb);
        DumpBackground(sb);
        DumpRuler(sb);
        DumpForeground(sb);
        DumpRuler(sb);
        return sb.ToString();
    }
    public string DumpRuler(StringBuilder? sb)
    {
        sb ??= new StringBuilder();
        sb.Append("/");
        for (int col = 0; col < Cols; col++)
            sb.Append($"{col % 10}");
        sb.AppendLine("\\");
        return sb.ToString();
    }
    public string DumpRunes(StringBuilder? sb)
    {
        sb ??= new StringBuilder();

        sb.Append("/");
        for (int col = 0; col < Cols; col++)
            sb.Append("V");
        sb.AppendLine("\\");

        for (var row = 0; row < Rows; row++)
        {
            sb.Append(">");
            for (var col = 0; col < Cols; col++)
            {
                var x = Contents[row, col];
                sb.Append(x.Rune);
            }
            sb.AppendLine($"< {row,2}");
        }
        sb.Append("\\");
        for (int col = 0; col < Cols; col++)
            sb.Append("^");
        sb.AppendLine("/");
        return sb.ToString();
    }
    public string DumpBackground(StringBuilder? sb)
    {
        sb ??= new StringBuilder();

        sb.Append("/");
        for (int col = 0; col < Cols; col++)
            sb.Append("V");
        sb.AppendLine("\\");

        for (var row = 0; row < Rows; row++)
        {
            sb.Append(">");
            for (var col = 0; col < Cols; col++)
            {
                var i = (int)Contents[row, col].Attr.Background;
                var x = i == 0 ? "." : string.Format("{0:X1}", i);
                sb.Append($"{x}");
            }
            sb.AppendLine($"< {row,2}");
        }
        sb.Append("\\");
        for (int col = 0; col < Cols; col++)
            sb.Append("^");
        sb.AppendLine("/");
        return sb.ToString();
    }
    public string DumpForeground(StringBuilder? sb)
    {
        sb ??= new StringBuilder();

        sb.Append("/");
        for (int col = 0; col < Cols; col++)
            sb.Append("V");
        sb.AppendLine("\\");

        for (var row = 0; row < Rows; row++)
        {
            sb.Append(">");
            for (var col = 0; col < Cols; col++)
            {
                var i = (int)Contents[row, col].Attr.Foreground;
                var x = i == 0 ? "." : string.Format("{0:X1}", i);
                sb.Append($"{x}");
            }
            sb.AppendLine($"< {row,2}");
        }
        sb.Append("\\");
        for (int col = 0; col < Cols; col++)
            sb.Append("^");
        sb.AppendLine("/");
        return sb.ToString();
    }
}

internal record CellValue(Rune Rune, CellAttributes Attr, int X);