namespace No8.Ascii.VirtualTerminal;

using static TerminalSeq;

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

public partial class ExConsole :
    ExConsole.IExConsoleCharacter 
{
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
        public static readonly CharacterAttr None = new();
        public static readonly CharacterAttr Bold = new(intensity: Intense.Bold);
        public static readonly CharacterAttr Italic = new(italic: true);
        public static readonly CharacterAttr Underline = new(underline: true);
        public static readonly CharacterAttr Reverse = new(reverse: true);
        
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
    
    public interface IExConsoleCharacter
    {
        void UnderlineColor(System.Drawing.Color c);
        void UnderlineColorDefault();
        void ForeColor(System.Drawing.Color c);
        void ForeColorDefault();
        void BackColor(System.Drawing.Color c);
        void BackColorDefault();
        void Attribute(params CharacterAttributes[] attrs);
        void Attribute(CharacterAttr attr);
    }
    
    void IExConsoleCharacter.UnderlineColor(System.Drawing.Color c) => Write(Color.Underline(c));
    void IExConsoleCharacter.UnderlineColorDefault() => Write(Color.UnderlineDefault);
    
    void IExConsoleCharacter.ForeColor(System.Drawing.Color c) => Write(Color.Fore(c));
    void IExConsoleCharacter.ForeColorDefault()  => Write(Color.ForeDefault);
    
    void IExConsoleCharacter.BackColor(System.Drawing.Color c) => Write(Color.Back(c));
    void IExConsoleCharacter.BackColorDefault()  => Write(Color.BackDefault);

    void IExConsoleCharacter.Attribute(params CharacterAttributes[] attrs)
    {
        foreach (var attr in attrs)
            Write(Graphics.Set((int)attr));
    }
    
    void IExConsoleCharacter.Attribute(CharacterAttr attr)
    {
        if (attr == CharacterAttr.None) Write(Graphics.Reset);
        
        if (attr.italic)   Write(Graphics.Italic);
        if (attr.reverse)   Write(Graphics.Reverse);
        if (attr.underline)   Write(Graphics.Underline);
        if (attr.doubleUnderline)   Write(Graphics.DoubleUnderline);
        if (attr.overline)   Write(Graphics.Overlined);
        if (attr.conceal)   Write(Graphics.Conceal);
        if (attr.strikeThrough)   Write(Graphics.Strike);
        
        // ReSharper disable ConvertIfStatementToSwitchStatement
        if (attr.intensity == CharacterAttr.Intense.Bold) Write(Graphics.Bold);
        if (attr.intensity == CharacterAttr.Intense.Faint) Write(Graphics.Faint);
        
        if (attr.blinks == CharacterAttr.CharacterBlink.Slow) Write(Graphics.SlowBlink);
        if (attr.blinks == CharacterAttr.CharacterBlink.Fast) Write(Graphics.FastBlink);
        
        if (attr.script == CharacterAttr.CharacterScript.Superscript) Write(Graphics.SuperScript);
        if (attr.script == CharacterAttr.CharacterScript.Subscript) Write(Graphics.SubScript);
        // ReSharper restore ConvertIfStatementToSwitchStatement
    }

}