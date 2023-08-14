using No8.Ascii.Platforms;

namespace No8.Ascii.VirtualTerminal;

using static TerminalSeq;

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming


/// <summary>
///     Terminal Constants and Control string creation
/// </summary>

public static class Terminal
{
    private static ConsoleDriver? _consoleDriver;
    private static ConsoleDriver CD => _consoleDriver ??= ConsoleDriver.Current;
    


    public static class Special
    {
        /// <summary>
        ///     Reset to initial state
        /// </summary>
        public static void SoftReset() => CD.Write(ControlSeq.SoftTerminalReset);

        /// <summary>
        ///     Play Sound
        /// </summary>
        /// <param name="vol">0..7</param>
        /// <param name="duration">0..255 1/32nd of a second</param>
        /// <param name="note">
        /// Note Selection
        ///  0:silent    1:C5    2:C#5   3:D5    4:D#5(Eb)   5:E5    6:F5    7:F#5        
        ///  8:G5        9:G#5  10:A5   11:A#5  12:B5       13:C6   14:C#6  15:D6       
        /// 16:D#6      17:E6   18:F6   19:F#6  20:G6       21:G#6  22:A6   23:A#6   
        /// 24:B6       25:C7
        /// </param>
        public static void PlaySound(int vol, int duration, int note)
        {
            CD.Write( AudibleAttributes.PlaySound(
                Math.Clamp(vol, 0, 7), 
                Math.Clamp(duration, 0, 255), 
                Math.Clamp(note,0,25)) );
        }
    }

    public static class Cursor
    {
        public static void Set(int row, int col) => CD.Write(TerminalSeq.Cursor.Set(row, col));

        public static void Up(int n = 1) => CD.Write(TerminalSeq.Cursor.Up(n));
        public static void Down(int n = 1) => CD.Write(TerminalSeq.Cursor.Down(n));
        public static void Right(int n = 1) => CD.Write(TerminalSeq.Cursor.Right(n));
        public static void Left(int n = 1) => CD.Write(TerminalSeq.Cursor.Left(n)); 
        

        public static void Show() => CD.Write(TerminalSeq.Cursor.Show);
        public static void Hide() => CD.Write(TerminalSeq.Cursor.Hide);


        /// <summary>
        ///     Set cursor Style
        /// </summary>
        public static class Style
        {
            public static void Blinking() => CD.Write(TerminalSeq.Cursor.Style.Blinking);
            public static void Steady() => CD.Write(TerminalSeq.Cursor.Style.Steady);
            public static void BlinkingUnderline() => CD.Write(TerminalSeq.Cursor.Style.BlinkingUnderline);
            public static void SteadyUnderline() => CD.Write(TerminalSeq.Cursor.Style.SteadyUnderline);
            public static void BlinkingBar() => CD.Write(TerminalSeq.Cursor.Style.BlinkingBar);
            public static void SteadyBar() => CD.Write(TerminalSeq.Cursor.Style.SteadyBar);
        }
    }

    public enum FillCharacterAttributes
    {
        Off = 0,
        Bold = 1,
        Underline = 4, 
        Blink = 5,
        Negative = 7,
        NoBold = 22,
        NoUnderline = 24,
        NoBlink = 25,
        NoNegative = 27
    }

    /// <summary>
    ///     Character attributes
    ///     Not all terminals support all attributes
    
    ///     Some attributes are mutually exclusive. e.g. Bold, NotBold 
    /// </summary>
    public enum CharacterAttributes
    {
        Normal = 0,	// reset or normal	All attributes become turned off
        Bold = 1,	// Bold or increased intensity	As with faint, the color change is a PC (SCO / CGA) invention.[38][better source needed]
        Faint = 2,	// Faint, decreased intensity, or dim	May be implemented as a light font weight like bold.[39]
        Italic = 3,	// Italic	Not widely supported. Sometimes treated as inverse or blink.[38]
        Underline = 4,	// Underline	Style extensions exist for Kitty, VTE, mintty and iTerm2.[40][41]
        SlowBlink = 5,	// Slow blink	Sets blinking to less than 150 times per minute
        FastBlink = 6,	// Rapid blink	MS-DOS ANSI.SYS, 150+ per minute; not widely supported
        Reverse = 7,	// Reverse video or invert	Swap foreground and background colors; inconsistent emulation[42][dubious – discuss]
        Conceal = 8,	// Conceal or hide	Not widely supported.
        CrossedOut = 9,	// Crossed-out, or strike	Characters legible but marked as if for deletion. Not supported in Terminal.app
        DoubleUnderline = 21,	// Doubly underlined; or: not bold	Double-underline per ECMA-48,[5]: 8.3.117  but instead disables bold intensity on several terminals, including in the Linux kernel's console before version 4.17.[43]
        NoIntense = 22,	// Normal intensity	Neither bold nor faint; color changes where intensity is implemented as such.
        NotItalic = 23,	// Neither italic, nor blackletter	
        NotUnderline = 24,	// Not underlined	Neither singly nor doubly underlined
        NotBlinking = 25,	// Not blinking	Turn blinking off
        NotReverse = 27,	// Not reversed	
        Reveal = 28,	// Reveal	Not concealed
        NotCrossedOut = 29,	// Not crossed out	
        Overlined = 53,	// Overlined	Not supported in Terminal.app
        NotOverlined = 55,	// Not overlined	
        Superscript = 73,	// Superscript	Implemented only in mintty[44]
        Subscript = 74,	// Subscript
        NotScript = 75,	// Neither superscript nor subscript
    }

    public record CharacterAttr(
        bool italic = false,
        bool underline = false,
        bool reverse = false,
        bool conceal = false,
        bool strikeThrough = false,
        bool overline = false,
        bool doubleUnderline = false,
        CharacterAttr.Intense intensity = CharacterAttr.Intense.Normal,
        CharacterAttr.CharacterBlink blinks = CharacterAttr.CharacterBlink.None,
        CharacterAttr.CharacterScript script = CharacterAttr.CharacterScript.None)
    {
        public static CharacterAttr None = new();
        public static CharacterAttr Bold = new(intensity: Intense.Bold);
        public static CharacterAttr Italic = new(italic: true);
        public static CharacterAttr Underline = new(underline: true);
        public static CharacterAttr Reverse = new(reverse: true);
        
        public enum Intense
        {
            Normal,
            Faint,
            Bold
        }

        public enum CharacterBlink
        {
            None,
            Slow,
            Fast
        }
        public enum CharacterScript
        {
            None,
            Superscript,
            Subscript
        }
    }
    
    public static class Character
    {
        public static void UnderlineColor(System.Drawing.Color c) => CD.Write(Color.Underline(c));
        public static void UnderlineColorDefault() => CD.Write(Color.UnderlineDefault);
        
        public static void ForeColor(System.Drawing.Color c) => CD.Write(Color.Fore(c));
        public static void ForeColorDefault()  => CD.Write(Color.ForeDefault);
        
        public static void BackColor(System.Drawing.Color c) => CD.Write(Color.Back(c));
        public static void BackColorDefault()  => CD.Write(Color.BackDefault);

        public static void Attribute(params CharacterAttributes[] attrs)
        {
            foreach (var attr in attrs)
                CD.Write(Graphics.Set((int)attr));
        }
        
        public static void Attribute(CharacterAttr attr)
        {
            if (attr == CharacterAttr.None) CD.Write(Graphics.Reset);
            
            if (attr.italic)   CD.Write(Graphics.Italic);
            if (attr.reverse)   CD.Write(Graphics.Reverse);
            if (attr.underline)   CD.Write(Graphics.Underline);
            if (attr.doubleUnderline)   CD.Write(Graphics.DoubleUnderline);
            if (attr.overline)   CD.Write(Graphics.Overlined);
            if (attr.conceal)   CD.Write(Graphics.Conceal);
            if (attr.strikeThrough)   CD.Write(Graphics.Strike);
            
            // ReSharper disable ConvertIfStatementToSwitchStatement
            if (attr.intensity == CharacterAttr.Intense.Bold) CD.Write(Graphics.Bold);
            if (attr.intensity == CharacterAttr.Intense.Faint) CD.Write(Graphics.Faint);
            
            if (attr.blinks == CharacterAttr.CharacterBlink.Slow) CD.Write(Graphics.SlowBlink);
            if (attr.blinks == CharacterAttr.CharacterBlink.Fast) CD.Write(Graphics.FastBlink);
            
            if (attr.script == CharacterAttr.CharacterScript.Superscript) CD.Write(Graphics.SuperScript);
            if (attr.script == CharacterAttr.CharacterScript.Subscript) CD.Write(Graphics.SubScript);
            // ReSharper restore ConvertIfStatementToSwitchStatement
        }
    }

    public static class Scroll
    {
        public static void Smooth() => CD.Write(TextProcessing.ScrollingMode.Smooth);
        public static void Jump() => CD.Write(TextProcessing.ScrollingMode.Jump);
        /// <summary>
        ///     Scrolling Speed
        /// </summary>
        /// <param name="speed">0-9</param>
        public static void Speed(int speed) => CD.Write(TextProcessing.SetScrollSpeed(speed));

        public static void Set(int topRow, int bottomRow) =>
            CD.Write(TerminalSeq.Scroll.Set(topRow, bottomRow));

        public static void Up(int lines) => CD.Write(TerminalSeq.Scroll.Up(lines));
        public static void Down(int lines) => CD.Write(TerminalSeq.Scroll.Down(lines));

    }

    public static class Editing
    {
        public static void CharacterDelete(int n = 1) => CD.Write(EditingControlFunctions.DeleteCharacter(n));
        public static void CharacterErase(int n = 1) => CD.Write(EditingControlFunctions.EraseCharacter(n));
        public static void CharacterInsert(int n = 1) => CD.Write(EditingControlFunctions.InsertCharacter(n));
        
        
        public static void ColumnDelete(int n = 1) => CD.Write(EditingControlFunctions.DeleteColumn(n));
        public static void ColumnInsert(int n = 1) => CD.Write(EditingControlFunctions.InsertColumn(n));
        
        public static void LineDelete(int n = 1) => CD.Write(EditingControlFunctions.DeleteLine(n));
        public static void LineInsert(int n = 1) => CD.Write(EditingControlFunctions.InsertLine(n));


        public static void EraseCursorToEndOfDisplay() => CD.Write(EditingControlFunctions.EraseInDisplay(0));
        public static void EraseCursorToEndOfLine() => CD.Write(EditingControlFunctions.EraseInLine(0));
        
        public static void EraseTopOfDisplayToCursor() => CD.Write(EditingControlFunctions.EraseInDisplay(1));
        public static void EraseStartOfLineToCursor() => CD.Write(EditingControlFunctions.EraseInLine(1));

        public static void EraseRectangle(int top, int left, int bottom, int right) =>
            CD.Write(RectangleAreaProcessing.EraseRectangularArea(top, left, bottom, right));
        public static void FillRectangle(int top, int left, int bottom, int right, char ch) =>
            CD.Write(RectangleAreaProcessing.FillRectangleArea(ch, top, left, bottom, right));

        /// <summary>
        ///     Set of clear all character attributes in a given rectangle 
        /// </summary>
        public static void RectangleAttributes(int top, int left, int bottom, int right, params FillCharacterAttributes[] flags)
        {
            if (!flags.Any())
            {
                CD.Write(TerminalSeq.RectangleAreaProcessing.ChangeAttributeInRectangle(
                    top, left, bottom, right,
                    (int)FillCharacterAttributes.Off));
                return;
            }

            CD.Write(RectangleAreaProcessing.ChangeAttributeInRectangle(
                top, left, bottom, right, 
                flags.OfType<int>().ToArray()));
        }
    }

    public static class Screen
    {
        /// <summary>
        ///     
        /// </summary>
        public static void Clear() => CD.Write(ControlSeq.ClearScreen);
        
        /// <summary>
        ///     Switch to Full screen (Alt) mode
        ///     Cursor is save and Alt screen is cleared 
        /// </summary>
        public static void ScreenAlt() => CD.Write(Mode.ScreenAltClear);
        
        /// <summary>
        ///     Switch to normal screen mode
        ///     Cursor is also restored to last location
        /// </summary>
        public static void ScreenNormal() => CD.Write(Mode.ScreenNormal);
        
        /// <summary>
        ///     Set Window Title
        /// </summary>
        /// <param name="title">Up to 30 characters</param>
        public static void SetWindowTitle(string title) =>
            CD.Write(Window.SetWindowTitle(title));
    }

    public static class Mouse
    {
        public static void HighlightEnable()
        {
            CD.Write(ControlSeq.SetResourceValue(1));   // This should be the default anyway
            CD.Write(Mode.MouseTrackingHilite);
        }

        public static void HighlightDisable()
        {
            CD.Write(Mode.StopMouseTrackingHilite);
            CD.Write(ControlSeq.SetResourceValue(1));
        }

        internal static void TrackingStart()
        {
            CD.Write(ControlSeq.PrivateModeSetDec(
                1003,   // use all mouse tracking on any event
                1004,   // Send focus in/out events
                1006   // SGR ext mouse mode
                ));
        }
        internal static void TrackingStop()
        {
            CD.Write(ControlSeq.PrivateResetDec(
                1003,   // use all mouse tracking on any event
                1004,   // Send focus in/out events
                1006   // SGR ext mouse mode
            ));
        }
    }
    
}


// ReSharper restore InconsistentNaming
// ReSharper restore IdentifierTypo
// ReSharper restore CommentTypo
