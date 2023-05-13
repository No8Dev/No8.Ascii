namespace Asciis.App.VirtualTerminal;

public class Terminal
{
    /// <summary>
    /// https://invisible-island.net/xterm/ctlseqs/ctlseqs.htm
    /// https://en.wikipedia.org/wiki/ANSI_escape_code
    /// https://docs.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences
    /// https://vt100.net/docs/vt100-ug/chapter3.html#S3.3
    /// </summary>

    public static class InputSeq
    {
        public static string UpArrow    = "\x1b[A";
        public static string DownArrow  = "\x1b[B";
        public static string RightArrow = "\x1b[C";
        public static string LeftArrow  = "\x1b[D";
        public static string HomeArrow  = "\x1b[H";
        public static string EndArrow   = "\x1b[F";

        public static string CtrlUpArrow    = "\x1b[1;5A";
        public static string CtrlDownArrow  = "\x1b[1;5B";
        public static string CtrlRightArrow = "\x1b[1;5C";
        public static string CtrlLeftArrow  = "\x1b[1;5D";

        public static string Insert   = "\x1b[2~";
        public static string Delete   = "\x1b[3~";
        public static string PageUp   = "\x1b[5~";
        public static string PageDown = "\x1b[6~";

        public static string F1  = "\x1bOP";
        public static string F2  = "\x1bOQ";
        public static string F3  = "\x1bOR";
        public static string F4  = "\x1bOS";
        public static string F5  = "\x1b[15~";
        public static string F6  = "\x1b[17~";
        public static string F7  = "\x1b[18~";
        public static string F8  = "\x1b[19~";
        public static string F9  = "\x1b[20~";
        public static string F10 = "\x1b[21~";
        public static string F11 = "\x1b[23~";
        public static string F12 = "\x1b[24~";
    }


    public static class Mode
    {
        public static string ScreenAlt    = "\x1b[?1049h";
        public static string ScreenNormal = "\x1b[?1049h";

        public static string LocatorReportingCells = "\x1b[1;2'z";
        public static string StopLocatorReporting  = "\x1b['z";

        public static string SendMouseXY         = "\x1b[?1000h";
        public static string MouseTrackingHilite = "\x1b[?1001h";
        public static string MouseTrackingCell   = "\x1b[?1002h";
        public static string MouseTrackingAll    = "\x1b[?1003h";
        public static string MouseTrackingFocus  = "\x1b[?1004h";
        public static string MouseTrackingUtf8   = "\x1b[?1005h";
        public static string MouseTrackingSGR    = "\x1b[?1006h";

        public static string StartTracking = "\x1b[?1003h\x1b[?1004h\x1b[?1006h";
        public static string StopTracking  = "\x1b[?1003l\x1b[?1004l\x1b[?1006l";

        public static string StopMouseXY             = "\x1b[?1000h";
        public static string StopMouseTrackingHilite = "\x1b[?1001h";
        public static string StopMouseTrackingCell   = "\x1b[?1002h";
        public static string StopMouseTrackingAll    = "\x1b[?1003h";
    }

    public static class Func
    {
        public static string SendDeviceAttributesPrimary   = "\x1b[c";
        public static string SendDeviceAttributesSecondary = "\x1b[>c";
        public static string SendDeviceAttributesTertiary  = "\x1b[=c";

        public static string DeviceStatusReport   = "\x1b[5n";
        public static string ReportCursorPosition = "\x1b[6n";
    }


    // ReSharper disable InconsistentNaming
    public const char   ESC = '\x1b';   // Escape
    public const string CSI = "\x1b[";  // Control Sequence Introducer
    public const string DCS = "\x1bP";  // Device Control String
    public const string OSC = "\x1b]";  // Operating System Command
    public const string SOS = "\x1bX";  // Start of String
    public const string ST  = "\x1b\\"; // String Terminator
    public const string PM  = "\x1b^";  // Privacy Message

    public const string APC = "\x1b_"; // Application Program Command

    // ReSharper restore InconsistentNaming

    public static class SpecialKey
    {
        // ReSharper disable InconsistentNaming
        public const char ACK = '\x06'; // Acknowledge
        public const char BEL = '\x07'; // Ctrl-G Bell
        public const char BS  = '\x08'; // Ctrl-H Backspace
        public const char CAN = '\x18'; // Cancel
        public const char CR  = '\x0D'; // Ctrl-M Carriage Return
        public const char DC1 = '\x11'; // Device Control 1
        public const char DC2 = '\x12'; // Device Control 2
        public const char DC3 = '\x13'; // Device Control 3
        public const char DC4 = '\x14'; // Device Control 4
        public const char DEL = '\x7F'; // Del
        public const char DLE = '\x10'; // Data Link Escape
        public const char EM  = '\x05'; // End of Medium
        public const char ENQ = '\x05'; // Ctrl-E Enquire
        public const char EOT = '\x04'; // End of Transmission
        public const char ESC = '\x1b'; // Escape
        public const char ETB = '\x17'; // End of Transmission Block
        public const char ETX = '\x03'; // End of Text
        public const char FF  = '\x0C'; // Ctrl-L Form Feed
        public const char FS  = '\x1C'; // File Separator
        public const char GS  = '\x1D'; // Group Separator
        public const char LF  = '\x0A'; // Ctrl-J Line Feed
        public const char NAK = '\x15'; // Negative Acknowledge
        public const char RS  = '\x1E'; // Record Separator
        public const char SI  = '\x0F'; // Ctrl-O Shift In
        public const char SO  = '\x0E'; // Ctrl-N Shift Out
        public const char SOH = '\x01'; // Start of Heading
        public const char SP  = '\x20'; // Space
        public const char STX = '\x02'; // Start of Text
        public const char SUB = '\x1A'; // Substitute
        public const char SYN = '\x16'; // Synchronous Idle
        public const char HTS = '\x09'; // Ctrl-I Tab
        public const char US  = '\x1F'; // Unit Separator
        public const char VT  = '\x0B'; // Ctrl-K Vertical Tab
        // ReSharper restore InconsistentNaming
    }

    public static class Control
    {
        public static string Index               = "\x1bD"; // IND 0x84
        public static string NextLine            = "\x1bE"; // NEL 0x85
        public static string TabSet              = "\x1bH"; // HTS 0x88
        public static string ReverseIndex        = "\x1bM"; // RI 0x8d
        public static string SingleShiftSelectG2 = "\x1bN"; // SS2 0x8e, VT220
        public static string SingleShiftSelectG3 = "\x1bO"; // SS3 0x8f, VT220
        public static string StartGuardedArea    = "\x1bV"; // SPA 0x96
        public static string EndGuardedArea      = "\x1bW"; // EPA 0x97
        public static string ReturnTerminalId    = "\x1bZ"; // DECID 0x9a obsolete

        public static string AnsiConformance1 = "\x1b L"; // 
        public static string AnsiConformance2 = "\x1b M"; // 
        public static string AnsiConformance3 = "\x1b N"; // 

        public static string BackIndex         = "\x1b6"; // ESC 6     Back Index (DECBI), VT420 and up.
        public static string SaveCursor        = "\x1b7"; // ESC 7     Save Cursor (DECSC), VT100.
        public static string RestoreCursor     = "\x1b8"; // ESC 8     Restore Cursor (DECRC), VT100.
        public static string ForwardIndex      = "\x1b9"; // ESC 9     Forward Index (DECFI), VT420 and up.
        public static string ApplicationKeypad = "\x1b="; // ESC =     Application Keypad (DECKPAM).
        public static string NormalKeypad      = "\x1b>"; // ESC >     Normal Keypad (DECKPNM), VT100.
        public static string CursorLowerLeft   = "\x1bF"; // ESC F     Cursor to lower left corner of screen.This is enabled by the hpLowerleftBugCompat resource.
        public static string FullReset         = "\x1bc"; // ESC c     Full Reset (RIS), VT100.
        public static string MemoryLock        = "\x1bl"; // ESC l     Memory Lock (per HP terminals).  Locks memory above the cursor.
        public static string MemoryUnlock      = "\x1bm"; // ESC m     Memory Unlock(per HP terminals).
        public static string LS2               = "\x1bn"; // ESC n     Invoke the G2 Character Set as GL(LS2).
        public static string LS3               = "\x1bo"; // ESC o     Invoke the G3 Character Set as GL(LS3).
        public static string LS3R              = "\x1b|"; // ESC |     Invoke the G3 Character Set as GR(LS3R).
        public static string LS2R              = "\x1b}"; // ESC }     Invoke the G2 Character Set as GR(LS2R).
        public static string LS1R              = "\x1b~"; // ESC ~     Invoke the G1 Character Set as GR(LS1R), VT100.
    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/ANSI_escape_code#SGR
    /// </summary>
    public static class Color
    {
        public static string Fore(int r, int g, int b) =>
            $"\x1b[38;2;{r};{g};{b}m";
        public static string Fore(System.Drawing.Color color) =>
            $"\x1b[38;2;{color.R};{color.G};{color.B}m";
        public static string Fore(int color256) =>
            $"\x1b[38;5;{color256}m";
        public static string Fore(Color4Bit color) =>
            $"\x1b[{(int)color + 30}m";
        public static string ForeDefault = "\x1b[39m";
        public static string Back(int r, int g, int b) =>
            $"\x1b[48;2;{r};{g};{b}m";
        public static string Back(int color256) =>
            $"\x1b[48;5;{color256}m";
        public static string Back(Color4Bit color) =>
            $"\x1b[{(int)color + 40}m";
        public static string BackDefault = "\x1b[49m";
        public static string Underline(int r, int g, int b) =>
            $"\x1b[58;2;{r};{g};{b}m";
        public static string UnderlineDefault = "\x1b[59m";
    }

    public enum Color4Bit
    {
        Black         = 0,
        Red           = 1,
        Green         = 2,
        Yellow        = 3,
        Blue          = 4,
        Magenta       = 5,
        Cyan          = 6,
        White         = 7,
        BrightBlack   = 60,
        BrightRed     = 61,
        BrightGreen   = 62,
        BrightYellow  = 63,
        BrightBlue    = 64,
        BrightMagenta = 65,
        BrightCyan    = 66,
        BrightWhite   = 67,
    }


    public static class Cursor
    {
        public static string Set(int row, int col) =>
            $"\x1b[{row};{col}H";

        public static string Show = "\x1b[?25h";
        public static string Hide = "\x1b[?25l";
    }

    public static class Graphics
    {
        public static string Reset           = "\x1b[0m";  // Reset / Normal
        public static string Bold            = "\x1b[1m";  // Bold or increased intensity
        public static string Faint           = "\x1b[2m";  // Faint or decreased intensity
        public static string Italic          = "\x1b[3m";  // Italic Not widely supported
        public static string Underline       = "\x1b[4m";  // Underline
        public static string SlowBlink       = "\x1b[5m";  // Slow Blink  less than 150 per minute
        public static string FastBlink       = "\x1b[6m";  // Rapid Blink
        public static string Reverse         = "\x1b[7m";  // Reverse video
        public static string Conceal         = "\x1b[8m";  // Conceal aka Hide
        public static string Strike          = "\x1b[9m";  // Crossed-out	aka Strike
        public static string PrimaryFont     = "\x1b[10m"; // Primary(default) font
        public static string AltFont11       = "\x1b[11m"; // Alternative font
        public static string DoubleUnderline = "\x1b[21m"; // Doubly underline
        public static string NormalColor     = "\x1b[22m"; // Normal color or intensity
        public static string ItalicOff       = "\x1b[23m"; // Not italic
        public static string UnderlineOff    = "\x1b[24m"; // Underline off
        public static string BlinkOff        = "\x1b[25m"; // Blink off
        public static string ReverseOff      = "\x1b[27m"; // Reverse off
        public static string ConcealOff      = "\x1b[28m"; // Conceal off
        public static string StrikeOff       = "\x1b[29m"; // Strike off
        public static string Overlined       = "\x1b[53m"; // Overlined
        public static string OverlinedOff    = "\x1b[55m"; // Overlined off
        public static string SuperScript     = "\x1b[73m"; // superscript
        public static string SubScript       = "\x1b[74m"; // subscript
    }


    public static class Scroll
    {
        public static string Set(int topRow, int bottomRow) =>
            $"\x1b[{topRow};{bottomRow}r";
        public static string Clear = "\x1b[r";

        public static string Up(int count) =>
            $"\x1b[{count}S";
        public static string Down(int count) =>
            $"\x1b[{count}T";
    }

    public static class Tab
    {
        public static string Set      = Control.TabSet;
        public static string Clear    = "\x1b[g";
        public static string ClearAll = "\x1b[3g";
    }


    /// <summary>
    ///     Device Template sequences.
    ///     All values require a value replaced in the string
    /// </summary>
    internal static class Device
    {
        // ReSharper disable InconsistentNaming

        public static string DCS_RequestStatusString = "\x1bP$q{Pt}\x1b\\"; // DCS $ q Pt ST Request Status String(DECRQSS), VT420 and up.
        /*
        The string following the "q" is one of the following:
            m       ⇒  SGR
            " p     ⇒  DECSCL
            SP q    ⇒  DECSCUSR
            " q     ⇒  DECSCA
            r       ⇒  DECSTBM
            s       ⇒  DECSLRM
            t       ⇒  DECSLPP
            $ |     ⇒  DECSCPP
            * |     ⇒  DECSNLS
        xterm responds with DCS 1 $ r Pt ST for valid requests,
        replacing the Pt with the corresponding CSI string,
        or DCS 0 $ r Pt ST for invalid requests.
        */
        public static string DCS_RequestResourceValue = "\x1bP+Q{Pt}\x1b\\"; //DCS + Q Pt ST
        /*
        Request resource values (XTGETXRES), xterm.  The string following the "Q" is a list of names encoded in hexadecimal
        (2 digits per character) separated by ; which correspond to xterm resource names.
        Only boolean, numeric and string resources are supported by this query.
        xterm responds with DCS 1 + R Pt ST for valid requests,
        adding to Pt an = , and the value of the corresponding resource that xterm is using,
        or DCS 0 + R Pt ST for invalid requests.
        The strings are encoded in hexadecimal (2 digits per character).
        */
        public static string DCS_SetTermInfoData = "\x1bP+p{Pt}\x1b\\"; //DCS + p Pt ST
        /*
        Set Termcap/Terminfo Data (XTSETTCAP), xterm.  The string following the "p" is a name to use for retrieving data from the
        terminal database.  The data will be used for the "tcap" keyboard configuration's function- and special-keys, as well as
        by the Request Termcap/Terminfo String control.
        */

        public static string DCS_RequestTermInfoString = "\x1bP+q{Pt}\x1b\\"; //DCS + q Pt ST
        /*
        Request Termcap/Terminfo String (XTGETTCAP), xterm.  The string following the "q" is a list of names encoded in
        hexadecimal (2 digits per character) separated by ; which correspond to termcap or terminfo key names.
        A few special features are also recognized, which are not key names:
        o   Co for termcap colors (or colors for terminfo colors), and
        o   TN for termcap name (or name for terminfo name).
        o   RGB for the ncurses direct-color extension.
        Only a terminfo name is provided, since termcap applications cannot use this information. xterm responds with
            DCS 1 + r Pt ST for valid requests,
        adding to Pt an = , and the value of the corresponding string that xterm would send, or
            DCS 0 + r Pt ST for invalid requests.
        The strings are encoded in hexadecimal (2 digits per character).
        */

        // ReSharper restore InconsistentNaming
    }

    public static class ControlSeq
    {
        // ReSharper disable InconsistentNaming

        public static string CSI_ = "\x1b["; // 
        public static string ST_  = "\x1b\\"; // String terminator

        public static string ClearScreen = "\x1b[2J";

        public static string CursorPosition                 = "\x1b[{Ps};{Ps}H";                // CSI Ps; Ps H    Cursor Position[row; column] (default = [1,1]) (CUP).
        public static string ScrollDown                     = "\x1b[{Ps}T";                     // CSI Ps T    Scroll down Ps lines(default = 1) (SD), VT420.
        public static string CSI_InitHighlightMouseTracking = "\x1b[{Ps};{Ps};{Ps};{Ps};{Ps}T"; // CSI Ps ; Ps ; Ps ; Ps ; Ps T
        //Initiate highlight mouse tracking (XTHIMOUSE), xterm.  Parameters are [func;startx;starty;firstrow;lastrow].  
        public static string CSI_SendDeviceAttrPrimary = "\x1b[{Ps}c"; // CSI Ps c  Send Device Attributes (Primary DA).
        /*
        Ps = 0  or omitted ⇒  request attributes from terminal.The response depends on the decTerminalID resource setting.
            ⇒  CSI? 1 ; 2 c("VT100 with Advanced Video Option")
            ⇒  CSI? 1 ; 0 c("VT101 with No Options")
            ⇒  CSI? 4 ; 6 c("VT132 with Advanced Video and Graphics")
            ⇒  CSI? 6 c("VT102")
            ⇒  CSI? 7 c("VT131")
            ⇒  CSI? 12 ; Ps c("VT125")
            ⇒  CSI? 62 ; Ps c("VT220")
            ⇒  CSI? 63 ; Ps c("VT320")
            ⇒  CSI? 64 ; Ps c("VT420")
        The VT100-style response parameters do not mean anything by themselves.VT220(and higher) parameters do, telling the host what features the terminal supports:
            Ps = 1  ⇒  132-columns.
            Ps = 2  ⇒  Printer.
            Ps = 3  ⇒  ReGIS graphics.
            Ps = 4  ⇒  Sixel graphics.
            Ps = 6  ⇒  Selective erase.
            Ps = 8  ⇒  User-defined keys.
            Ps = 9  ⇒  National Replacement Character sets.
            Ps = 15  ⇒  Technical characters.
            Ps = 16  ⇒  Locator port.
            Ps = 17  ⇒  Terminal state interrogation.
            Ps = 18  ⇒  User windows.
            Ps = 21  ⇒  Horizontal scrolling.
            Ps = 22  ⇒  ANSI color, e.g., VT525.
            Ps = 28  ⇒  Rectangular editing.
            Ps = 29  ⇒  ANSI text locator (i.e., DEC Locator mode).
        XTerm supports part of the User windows feature, providing a single page(which corresponds to its visible window).  
        Rather than resizing the font to change the number of lines/columns
        in a fixed-size display, xterm uses the window extension controls(DECSNLS, DECSCPP, DECSLPP)
        to adjust its visible window's size.  The "cursor coupling" controls (DECHCCM, DECPCCM, DECVCCM) are ignored.
        */

        public static string CSI_SendDeviceAttrSecondary = "\x1b[{Ps}>c"; // CSI > Ps c  Send Device Attributes (Secondary DA).
        /*
        Ps = 0  or omitted ⇒  request the terminal's identification code.  The response depends on the decTerminalID resource setting.  It should apply only to VT220 and up, but xterm extends this to VT100.
            ⇒  CSI  > Pp ; Pv ; Pc c
        where Pp denotes the terminal type
            Pp = 0  ⇒  "VT100".
            Pp = 1  ⇒  "VT220".
            Pp = 2  ⇒  "VT240" or "VT241".
            Pp = 18  ⇒  "VT330".
            Pp = 19  ⇒  "VT340".
            Pp = 24  ⇒  "VT320".
            Pp = 32  ⇒  "VT382".
            Pp = 41  ⇒  "VT420".
            Pp = 61  ⇒  "VT510".
            Pp = 64  ⇒  "VT520".
            Pp = 65  ⇒  "VT525".
        and Pv is the firmware version (for xterm, this was originally the XFree86 patch number, starting with 95).  
        In a DEC terminal, Pc indicates the ROM cartridge registration number and is always zero.
        */

        public static string HorzVertPosition = "\x1b[{Ps};{Ps}f"; // CSI Ps; Ps f Horizontal and Vertical Position[row; column] (default = [1,1]) (HVP).

        public static string TabClear = "\x1b[{Ps}g"; // CSI Ps g  Tab Clear (TBC). Ps = 0  ⇒  Clear     Current Column(default). Ps = 3  ⇒  Clear All.

        public static string SetMode = "\x1b[{Pm}h"; // CSI Pm h  Set Mode (SM).
        /*
            Ps = 2  ⇒  Keyboard  Action Mode(KAM).
            Ps = 4  ⇒  Insert    Mode(IRM).
            Ps = 1 2  ⇒  Send/receive(SRM).
            Ps = 2 0  ⇒  Automatic Newline(LNM).
        */

        public static string PrivateModeSetDec = "\x1b[?{Pm}h"; // CSI ? Pm h
        /*
        DEC Private Mode Set(DECSET).
        Ps = 1  ⇒  Application Cursor Keys(DECCKM), VT100.
        Ps = 2  ⇒  Designate USASCII for character sets G0-G3 (DECANM), VT100, and set VT100 mode.
        Ps = 3  ⇒  132 Column Mode (DECCOLM), VT100.
        Ps = 4  ⇒  Smooth (Slow) Scroll (DECSCLM), VT100.
        Ps = 5  ⇒  Reverse Video (DECSCNM), VT100.
        Ps = 6  ⇒  Origin Mode (DECOM), VT100.
        Ps = 7  ⇒  Auto-Wrap Mode (DECAWM), VT100.
        Ps = 8  ⇒  Auto-Repeat Keys (DECARM), VT100.
        Ps = 9  ⇒  Send Mouse X & Y on button press.  See the section Mouse Tracking.This is the X10 xterm mouse protocol.
        Ps = 10  ⇒  Show toolbar (rxvt).
        Ps = 12  ⇒  Start blinking cursor (AT&T 610).
        Ps = 13  ⇒  Start blinking cursor(set only via resource or menu).
        Ps = 14  ⇒  Enable XOR of blinking cursor control sequence and menu.
        Ps = 18  ⇒  Print Form Feed(DECPFF), VT220.
        Ps = 19  ⇒  Set print extent to full screen(DECPEX), VT220.
        Ps = 25  ⇒  Show cursor(DECTCEM), VT220.
        Ps = 30  ⇒  Show scrollbar(rxvt).
        Ps = 35  ⇒  Enable font-shifting functions(rxvt).
        Ps = 38  ⇒  Enter Tektronix mode(DECTEK), VT240, xterm.
        Ps = 40  ⇒  Allow 80 ⇒  132 mode, xterm.
        Ps = 41  ⇒  more(1) fix(see curses resource).
        Ps = 42  ⇒  Enable National Replacement Character sets (DECNRCM), VT220.
        Ps = 43  ⇒  Enable Graphics Expanded Print Mode(DECGEPM).
        Ps = 44  ⇒  Turn on margin bell, xterm.
        Ps = 44  ⇒  Enable Graphics Print Color Mode(DECGPCM).
        Ps = 45  ⇒  Reverse-wraparound mode, xterm.
        Ps = 45  ⇒  Enable Graphics Print ColorSpace(DECGPCS).
        Ps = 46  ⇒  Start logging, xterm.  This is normally disabled by a compile-time option.
        Ps = 47  ⇒  Use Alternate Screen Buffer, xterm.This may be disabled by the titeInhibit resource.
        Ps = 47  ⇒  Enable Graphics Rotated Print Mode (DECGRPM).
        Ps = 66  ⇒  Application keypad mode (DECNKM), VT320.
        Ps = 67  ⇒  Backarrow key sends backspace (DECBKM), VT340, VT420.This sets the backarrowKey resource to "true".
        Ps = 69  ⇒  Enable left and right margin mode (DECLRMM), VT420 and up.
        Ps = 80  ⇒  Enable Sixel Scrolling (DECSDM).
        Ps = 95  ⇒  Do not clear screen when DECCOLM is set/reset (DECNCSM), VT510 and up.
        Ps = 1000  ⇒  Send Mouse X & Y on button press and release.  See the section Mouse Tracking.This is the X11 xterm mouse protocol.
        Ps = 1001  ⇒  Use Hilite Mouse Tracking, xterm.
        Ps = 1002  ⇒  Use Cell Motion Mouse Tracking, xterm.See the section Button-event tracking.
        Ps = 1003  ⇒  Use All Motion Mouse Tracking, xterm. See the section Any-event tracking.
        Ps = 1004  ⇒  Send FocusIn/FocusOut events, xterm. 
        Ps = 1005  ⇒  Enable UTF-8 Mouse Mode, xterm.
        Ps = 1006  ⇒  Enable SGR Mouse Mode, xterm.
        Ps = 1007  ⇒  Enable Alternate Scroll Mode, xterm.  This corresponds to the alternateScroll resource.
        Ps = 1010  ⇒  Scroll to bottom on tty output (rxvt). This sets the scrollTtyOutput resource to "true".
        Ps = 1011  ⇒  Scroll to bottom on key press(rxvt).  This sets the scrollKey resource to "true".
        Ps = 1015  ⇒  Enable urxvt Mouse Mode.
        Ps = 1016  ⇒  Enable SGR Mouse PixelMode, xterm.
        Ps = 1034  ⇒  Interpret "meta" key, xterm.This sets the eighth bit of keyboard input (and enables the eightBitInput resource).
        Ps = 1035  ⇒  Enable special modifiers for Alt and NumLock keys, xterm.  This enables the numLock resource.
        Ps = 1036  ⇒  Send ESC   when Meta modifies a key, xterm. This enables the metaSendsEscape resource.
        Ps = 1037  ⇒  Send DEL from the editing-keypad Delete key, xterm.
        Ps = 1039  ⇒  Send ESC  when Alt modifies a key, xterm. This enables the altSendsEscape resource, xterm.
        Ps = 1040  ⇒  Keep selection even if not highlighted, xterm.  This enables the keepSelection resource.
        Ps = 1041  ⇒  Use the CLIPBOARD selection, xterm.  This enables the selectToClipboard resource.
        Ps = 1042  ⇒  Enable Urgency window manager hint when Control-G is received, xterm.This enables the bellIsUrgent resource.
        Ps = 1043  ⇒  Enable raising of the window when Control-G is received, xterm.This enables the popOnBell resource.
        Ps = 1044  ⇒  Reuse the most recent data copied to CLIP- BOARD, xterm.This enables the keepClipboard resource.
        Ps = 1046  ⇒  Enable switching to/from Alternate Screen Buffer, xterm.This works for terminfo-based systems, updating the titeInhibit resource.
        Ps = 1047  ⇒  Use Alternate Screen Buffer, xterm.This may be disabled by the titeInhibit resource.
        Ps = 1048  ⇒  Save cursor as in DECSC, xterm.This may be disabled by the titeInhibit resource.
        Ps = 1049  ⇒  Save cursor as in DECSC, xterm.After saving the cursor, switch to the Alternate Screen Buffer, 
                       clearing it first.This may be disabled by the titeInhibit resource.  
                       This control combines the effects of the 1047 and 1048  modes.
                       Use this with terminfo-based applications rather than the 47  mode.
        Ps = 1050  ⇒  Set terminfo/termcap function-key mode, xterm.
        Ps = 1051  ⇒  Set Sun function-key mode, xterm.
        Ps = 1052  ⇒  Set HP function-key mode, xterm.
        Ps = 1053  ⇒  Set SCO function-key mode, xterm.
        Ps = 1060  ⇒  Set legacy keyboard emulation, i.e, X11R6, xterm.
        Ps = 1061  ⇒  Set VT220 keyboard emulation, xterm.
        Ps = 2004  ⇒  Set bracketed paste mode, xterm.
        */

        public static string PrivateResetDec = "\x1b[?{Pm}l"; // CSI ? Pm l
        /*
        DEC Private Mode Reset (DECRST).
        Ps = 1  ⇒  Normal Cursor Keys (DECCKM), VT100.
        Ps = 2  ⇒  Designate VT52 mode (DECANM), VT100.
        Ps = 3  ⇒  80 Column Mode (DECCOLM), VT100.
        Ps = 4  ⇒  Jump (Fast) Scroll (DECSCLM), VT100.
        Ps = 5  ⇒  Normal Video (DECSCNM), VT100.
        Ps = 6  ⇒  Normal Cursor Mode (DECOM), VT100.
        Ps = 7  ⇒  No Auto-Wrap Mode (DECAWM), VT100.
        Ps = 8  ⇒  No Auto-Repeat Keys (DECARM), VT100.
        Ps = 9  ⇒  Don't send Mouse X & Y on button press, xterm.
        Ps = 10  ⇒  Hide toolbar (rxvt).
        Ps = 12  ⇒  Stop blinking cursor (AT&T 610).
        Ps = 13  ⇒  Disable blinking cursor (reset only via resource or menu).
        Ps = 14  ⇒  Disable XOR of blinking cursor control sequence and menu.
        Ps = 18  ⇒  Don't Print Form Feed (DECPFF), VT220.
        Ps = 19  ⇒  Limit print to scrolling region (DECPEX), VT220.
        Ps = 25  ⇒  Hide cursor (DECTCEM), VT220.
        Ps = 30  ⇒  Don't show scrollbar (rxvt).
        Ps = 35  ⇒  Disable font-shifting functions (rxvt).
        Ps = 40  ⇒  Disallow 80 ⇒  132 mode, xterm.
        Ps = 41  ⇒  No more(1) fix (see curses resource).
        Ps = 42  ⇒  Disable National Replacement Character sets (DECNRCM), VT220.
        Ps = 43  ⇒  Disable Graphics Expanded Print Mode (DECGEPM).
        Ps = 44  ⇒  Turn off margin bell, xterm.
        Ps = 44  ⇒  Disable Graphics Print Color Mode (DECGPCM).
        Ps = 45  ⇒  No Reverse-wraparound mode, xterm.
        Ps = 45  ⇒  Disable Graphics Print ColorSpace (DECGPCS).
        Ps = 46  ⇒  Stop logging, xterm.  This is normally disabled by a compile-time option.
        Ps = 47  ⇒  Use Normal Screen Buffer, xterm.
        Ps = 47  ⇒  Disable Graphics Rotated Print Mode (DECGRPM).
        Ps = 66  ⇒  Numeric keypad mode (DECNKM), VT320.
        Ps = 67  ⇒  Backarrow key sends delete (DECBKM), VT340, VT420.  This sets the backarrowKey resource to "false".
        Ps = 69  ⇒  Disable left and right margin mode (DECLRMM), VT420 and up.
        Ps = 80  ⇒  Disable Sixel Scrolling (DECSDM).
        Ps = 95  ⇒  Clear screen when DECCOLM is set/reset (DECNCSM), VT510 and up.
        Ps = 1000  ⇒  Don't send Mouse X & Y on button press and release.  See the section Mouse Tracking.
        Ps = 1001  ⇒  Don't use Hilite Mouse Tracking, xterm.
        Ps = 1002  ⇒  Don't use Cell Motion Mouse Tracking, xterm.  See the section Button-event tracking.
        Ps = 1003  ⇒  Don't use All Motion Mouse Tracking, xterm. See the section Any-event tracking.
        Ps = 1004  ⇒  Don't send FocusIn/FocusOut events, xterm.
        Ps = 1005  ⇒  Disable UTF-8 Mouse Mode, xterm.
        Ps = 1006  ⇒  Disable SGR Mouse Mode, xterm.
        Ps = 1007  ⇒  Disable Alternate Scroll Mode, xterm.  This corresponds to the alternateScroll resource.
        Ps = 1010  ⇒  Don't scroll to bottom on tty output (rxvt).  This sets the scrollTtyOutput resource to "false".
        Ps = 1011  ⇒  Don't scroll to bottom on key press (rxvt). This sets the scrollKey resource to "false".
        Ps = 1015  ⇒  Disable urxvt Mouse Mode.
        Ps = 1016  ⇒  Disable SGR Mouse Pixel-Mode, xterm.
        Ps = 1034  ⇒  Don't interpret "meta" key, xterm.  This disables the eightBitInput resource.
        Ps = 1035  ⇒  Disable special modifiers for Alt and NumLock keys, xterm.  This disables the numLock resource.
        Ps = 1036  ⇒  Don't send ESC  when Meta modifies a key, xterm.  This disables the metaSendsEscape resource.
        Ps = 1037  ⇒  Send VT220 Remove from the editing-keypad Delete key, xterm.
        Ps = 1039  ⇒  Don't send ESC when Alt modifies a key, xterm.  This disables the altSendsEscape resource.
        Ps = 1040  ⇒  Do not keep selection when not highlighted, xterm.  This disables the keepSelection resource.
        Ps = 1041  ⇒  Use the PRIMARY selection, xterm.  This disables the selectToClipboard resource.
        Ps = 1042  ⇒  Disable Urgency window manager hint when Control-G is received, xterm.  This disables the bellIsUrgent resource.
        Ps = 1043  ⇒  Disable raising of the window when Control-G is received, xterm.  This disables the popOnBell resource.
        Ps = 1046  ⇒  Disable switching to/from Alternate Screen Buffer, xterm.  This works for terminfo-based systems, updat- ing the 
                        titeInhibit resource.  If currently using the Alternate Screen Buffer, xterm switches to the Normal Screen Buffer.
        Ps = 1047  ⇒  Use Normal Screen Buffer, xterm.  Clear the screen first if in the Alternate Screen Buffer.  
                        This may be disabled by the titeInhibit resource.
        Ps = 1048  ⇒  Restore cursor as in DECRC, xterm.  This may be disabled by the titeInhibit resource.
        Ps = 1049  ⇒  Use Normal Screen Buffer and restore cursor as in DECRC, xterm.  This may be disabled by the titeInhibit resource.  
                        This combines the effects of the 1047 and 1048  modes.  
                        Use this with terminfo-based applications rather than the 47 mode.
        Ps = 1050  ⇒  Reset terminfo/termcap function-key mode, xterm.
        Ps = 1051  ⇒  Reset Sun function-key mode, xterm.
        Ps = 1052  ⇒  Reset HP function-key mode, xterm.
        Ps = 1053  ⇒  Reset SCO function-key mode, xterm.
        Ps = 1060  ⇒  Reset legacy keyboard emulation, i.e, X11R6, xterm.
        Ps = 1061  ⇒  Reset keyboard emulation to Sun/PC style, xterm.
        Ps = 2004  ⇒  Reset bracketed paste mode, xterm.
         */

        public static string SetCharacterAttr = "\x1b[{Pm}m"; // CSI Pm m  Character Attributes (SGR).
        /*
            Ps = 0  ⇒  Normal (default), VT100.
            Ps = 1  ⇒  Bold, VT100.
            Ps = 2  ⇒  Faint, decreased intensity, ECMA-48 2nd.
            Ps = 3  ⇒  Italicized, ECMA-48 2nd.
            Ps = 4  ⇒  Underlined, VT100.
            Ps = 5  ⇒  Blink, VT100. This appears as Bold in X11R6 xterm.
            Ps = 7  ⇒  Inverse, VT100.
            Ps = 8  ⇒  Invisible, i.e., hidden, ECMA-48 2nd, VT300.
            Ps = 9  ⇒  Crossed-out characters, ECMA-48 3rd.
            Px = 10 ⇒  Primary(default) font 
            Px = 11 ⇒  Alternative font
            Ps = 21  ⇒  Doubly-underlined, ECMA-48 3rd.
            Ps = 22  ⇒  Normal (neither bold nor faint), ECMA-48 3rd.
            Ps = 23  ⇒  Not italicized, ECMA-48 3rd.
            Ps = 24  ⇒  Not underlined, ECMA-48 3rd.
            Ps = 25  ⇒  Steady (not blinking), ECMA-48 3rd.
            Ps = 27  ⇒  Positive (not inverse), ECMA-48 3rd.
            Ps = 28  ⇒  Visible, i.e., not hidden, ECMA-48 3rd, VT300.
            Ps = 29  ⇒  Not crossed-out, ECMA-48 3rd.
            Ps = 30  ⇒  Set foreground color to Black.
            Ps = 31  ⇒  Set foreground color to Red.
            Ps = 32  ⇒  Set foreground color to Green.
            Ps = 33  ⇒  Set foreground color to Yellow.
            Ps = 34  ⇒  Set foreground color to Blue.
            Ps = 35  ⇒  Set foreground color to Magenta.
            Ps = 36  ⇒  Set foreground color to Cyan.
            Ps = 37  ⇒  Set foreground color to White.
            Ps = 39  ⇒  Set foreground color to default, ECMA-48 3rd.
            Ps = 40  ⇒  Set background color to Black.
            Ps = 41  ⇒  Set background color to Red.
            Ps = 42  ⇒  Set background color to Green.
            Ps = 43  ⇒  Set background color to Yellow.
            Ps = 44  ⇒  Set background color to Blue.
            Ps = 45  ⇒  Set background color to Magenta.
            Ps = 46  ⇒  Set background color to Cyan.
            Ps = 47  ⇒  Set background color to White.
            Ps = 49  ⇒  Set background color to default, ECMA-48 3rd.

        Some of the above note the edition of ECMA-48 which first describes a feature.  In its successive editions from 1979 to
        1991 (2nd 1979, 3rd 1984, 4th 1986, and 5th 1991), ECMA-48 listed codes through 6 5 (skipping several toward the end of
        the range).  Most of the ECMA-48 codes not implemented in xterm were never implemented in a hardware terminal.  Several
        (such as 39 and 49 ) are either noted in ECMA-48 as implementation defined, or described in vague terms.

        The successive editions of ECMA-48 give little attention to changes from one edition to the next, except to 
        comment on features which have become obsolete.  ECMA-48 1st (1976) is unavailable; there is no reliable 
        source of information which states whether "ANSI" color was defined in that edition, or later (1979).  
        The VT100 (1978) implemented the most commonly used non-color video attributes which are given in the 2nd edition.

        While 8-color support is described in ECMA-48 2nd edition, the VT500 series (introduced in 1993) were the 
        first DEC terminals implementing "ANSI" color.  The DEC terminal's use of color is known to differ from xterm; 
        useful documentation on this series became available too late to influence xterm.

        If 16-color support is compiled, the following aixterm controls apply.  Assume that xterm's resources are set so that
        the ISO color codes are the first 8 of a set of 16.  Then the aixterm colors are the bright versions of the ISO colors:

            Ps = 90  ⇒  Set foreground color to Black.
            Ps = 91  ⇒  Set foreground color to Red.
            Ps = 92  ⇒  Set foreground color to Green.
            Ps = 93  ⇒  Set foreground color to Yellow.
            Ps = 94  ⇒  Set foreground color to Blue.
            Ps = 95  ⇒  Set foreground color to Magenta.
            Ps = 96  ⇒  Set foreground color to Cyan.
            Ps = 97  ⇒  Set foreground color to White.
            Ps = 100  ⇒  Set background color to Black.
            Ps = 101  ⇒  Set background color to Red.
            Ps = 102  ⇒  Set background color to Green.
            Ps = 103  ⇒  Set background color to Yellow.
            Ps = 104  ⇒  Set background color to Blue.
            Ps = 105  ⇒  Set background color to Magenta.
            Ps = 106  ⇒  Set background color to Cyan.
            Ps = 107  ⇒  Set background color to White.

        If xterm is compiled with the 16-color support disabled, it
        supports the following, from rxvt:
            Ps = 100  ⇒  Set foreground and background color to default.

        XTerm maintains a color palette whose entries are identified by an index beginning with zero.  
        If 88- or 256-color support is compiled, the following apply:
        o   All parameters are decimal integers.
        o   RGB values range from zero (0) to 255.
        o   The 88- and 256-color support uses subparameters described in ISO-8613-6 for indexed color.  
            ISO-8613-6 also mentions direct color, using a similar scheme.  xterm supports that, too.
        o   xterm allows either colons (standard) or semicolons (legacy) to separate the subparameters 
            (but after the first colon, colons must be used).

        The indexed- and direct-color features are summarized in the
        FAQ, which explains why semicolon is accepted as a subparame-
        ter delimiter:

        Can I set a color by its number?

        These ISO-8613-6 controls (marked in ECMA-48 5th edition as "reserved for future standardization") 
        are supported by xterm:
            Ps = 38 : 2 : Pi : Pr : Pg : Pb ⇒  Set foreground color using RGB values.  
        If xterm is not compiled with direct-color support, it uses the closest match in its palette for the given RGB Pr/Pg/Pb.  
        The color space identifier Pi is ignored.
            Ps = 38 : 5 : Ps ⇒  Set foreground color to Ps, using indexed color.
            Ps = 48 : 2 : Pi : Pr : Pg : Pb ⇒  Set background color using RGB values.  
        If xterm is not compiled with direct-color support, it uses the closest match in its palette for the given RGB Pr/Pg/Pb.  
        The color space identifier Pi is ignored.
            Ps = 48 : 5 : Ps ⇒  Set background color to Ps, using indexed color.

        This variation on ISO-8613-6 is supported for compatibility with KDE konsole:
            Ps = 38 ; 2 ; Pr ; Pg ; Pb ⇒  Set foreground color using RGB values.  
        If xterm is not compiled with direct-color support, it uses the closest match in its palette for the given RGB Pr/Pg/Pb.
            Ps = 48 ; 2 ; Pr ; Pg ; Pb ⇒  Set background color using RGB values.  
        If xterm is not compiled with direct-color support, it uses the closest match in its palette for the given RGB Pr/Pg/Pb.

        In each case, if xterm is compiled with direct-color support, and the resource directColor is true, 
        then rather than choosing the closest match, xterm asks the X server to directly render a given color.
        */

        public static string SetKeyModifiers = "\x1b[>{Pp};{Pv}m"; // CSI > Pp ; Pv m
        public static string SetKeyModifier  = "\x1b[>{Pp}m";      // CSI > Pp m
        /*            
        Set/reset key modifier options (XTMODKEYS), xterm.  Set or reset resource-values used by xterm to decide whether to construct 
        escape sequences holding information about the modifiers pressed with a given key.

        The first parameter Pp identifies the resource to set/reset. 
        The second parameter Pv is the value to assign to the resource.

        If the second parameter is omitted, the resource is reset to its initial value.  Values 3  and 5  are reserved for 
        keypad-keys and string-keys.

            Pp = 0  ⇒  modifyKeyboard.
            Pp = 1  ⇒  modifyCursorKeys.
            Pp = 2  ⇒  modifyFunctionKeys.
            Pp = 4  ⇒  modifyOtherKeys.

        If no parameters are given, all resources are reset to their initial values.
        */

        public static string DisableKeyModifier = "\x1b[>{Ps}n"; // CSI > Ps n
        /*
        Disable key modifier options, xterm.  These modifiers may be enabled via the CSI > Pm m sequence.  This control sequence
        corresponds to a resource value of "-1", which cannot be set with the other sequence.

        The parameter identifies the resource to be disabled:

            Ps = 0  ⇒  modifyKeyboard.
            Ps = 1  ⇒  modifyCursorKeys.
            Ps = 2  ⇒  modifyFunctionKeys.
            Ps = 4  ⇒  modifyOtherKeys.

        If the parameter is omitted, modifyFunctionKeys is disabled. When modifyFunctionKeys is disabled, xterm uses the modifier
        keys to make an extended sequence of function keys rather than adding a parameter to each function key to denote the 
        modifiers.
        */

        public static string DSR_DeviceStatusReport = "\x1b[{Ps}n"; // CSI Ps n  Device Status Report (DSR).
        /*
            Ps = 5  ⇒  Status Report. Result ("OK") is CSI 0 n
            Ps = 6  ⇒  Report Cursor Position (CPR) [row;column]. Result is CSI r ; c R

        Note: it is possible for this sequence to be sent by a function key.  For example, with the default keyboard 
        configuration the shifted F1 key may send (with shift-, control-, alt- modifiers)

            CSI 1 ; 2  R , or
            CSI 1 ; 5  R , or
            CSI 1 ; 6  R , etc.

        The second parameter encodes the modifiers; values range from 2 to 16.  See the section PC-Style Function Keys for the
        codes.  The modifyFunctionKeys and modifyKeyboard resources can change the form of the string sent from the 
        modified F1 key.
        */

        public static string DSR_DeviceStatusReportDec = "\x1b[?{Ps}n"; // CSI ? Ps n
        /*
        Device Status Report (DSR, DEC-specific).
        Ps = 6  ⇒  Report Cursor Position (DECXCPR).  The response [row;column] is returned as CSI ? r ; c R (assumes the default page, i.e., "1").
        Ps = 15  ⇒  Report Printer status.  The response is CSI ? 10 n  (ready).  or CSI ? 11 n  (not ready).
        Ps = 25  ⇒  Report UDK status.  The response is CSI ? 20 n  (unlocked) or CSI ? 21 n  (locked).
        Ps = 26  ⇒  Report Keyboard status.  The response is CSI ? 27 ; 1 ; 0 ; 0 n  (North American).

        The last two parameters apply to VT300 & up (keyboard ready) and VT400 & up (LK01) respectively.

        Ps = 53  ⇒  Report Locator status.  The response is CSI ? 53 n  Locator available, if compiled-in, or CSI ? 50 n  No Locator, if not.
        Ps = 55  ⇒  Report Locator status.  The response is CSI ? 53 n  Locator available, if compiled-in, or CSI ? 50 n  No Locator, if not.
        Ps = 56  ⇒  Report Locator type.  The response is CSI ? 57 ; 1 n  Mouse, if compiled-in, or CSI ? 57 ; 0 n  Cannot identify, if not.
        Ps = 62  ⇒  Report macro space (DECMSR).  The response is CSI Pn *  { .
        Ps = 63  ⇒  Report memory checksum (DECCKSR), VT420 and up.
                        The response is DCS Pt ! ~ x x x x ST .
                        Pt is the request id (from an optional parameter to the request).
                        The x's are hexadecimal digits 0-9 and A-F.
        Ps = 75  ⇒  Report data integrity.  The response is CSI ? 70 n  (ready, no errors).
        Ps = 85  ⇒  Report multi-session configuration.  The response is CSI ? 83 n  (not configured for multiple-session operation).
        */

        public static string SetResourceValue = "\x1b[>{Ps}p"; // CSI > Ps p
        /*
        Set resource value pointerMode (XTSMPOINTER), xterm.  
        This is used by xterm to decide whether to hide the pointer cursor as the user types.

        Valid values for the parameter:
            Ps = 0  ⇒  never hide the pointer.
            Ps = 1  ⇒  hide if the mouse tracking mode is not enabled.
            Ps = 2  ⇒  always hide the pointer, except when leaving the window.
            Ps = 3  ⇒  always hide the pointer, even if leaving/entering the window.

        If no parameter is given, xterm uses the default, which is 1 .
        */

        public static string SoftTerminalReset = "\x1b[!p"; // CSI ! p   Soft terminal reset (DECSTR), VT220 and up.

        public static string ReportNameVersion = "\x1b[>{Ps}q"; // CSI > Ps q
        // Ps = 0  ⇒  Report xterm name and version(XTVERSION).
        // The response is a DSR sequence identifying the version: DCS > | text ST


            
        public static string LoadLEDs = "\x1b[{Ps}q"; // CSI Ps q  Load LEDs (DECLL), VT100.
        /*
            Ps = 0  ⇒  Clear all LEDS (default).
            Ps = 1  ⇒  Light Num Lock.
            Ps = 2  ⇒  Light Caps Lock.
            Ps = 3  ⇒  Light Scroll Lock.
            Ps = 21  ⇒  Extinguish Num Lock.
            Ps = 22  ⇒  Extinguish Caps Lock.
            Ps = 23  ⇒  Extinguish Scroll Lock.
        */

        public static string SetCursorStyle = "\x1b[{Ps} q"; // CSI Ps SP q
        /*
        Set cursor style (DECSCUSR), VT520.
            Ps = 0  ⇒  blinking block.
            Ps = 1  ⇒  blinking block (default).
            Ps = 2  ⇒  steady block.
            Ps = 3  ⇒  blinking underline.
            Ps = 4  ⇒  steady underline.
            Ps = 5  ⇒  blinking bar, xterm.
            Ps = 6  ⇒  steady bar, xterm.
        */

            
        public static string SetScrollingRegion = "\x1b[{Ps};{Ps}r"; // CSI Ps ; Ps r
        // Set Scrolling Region [top;bottom] (default = full size of window) (DECSTBM), VT100.

        public static string RestorePrivateModeValuesDec = "\x1b[?{Pm}r"; // CSI ? Pm r
        // Restore DEC Private Mode Values (XTRESTORE), xterm.  The value of Ps previously saved is restored.  
        // Ps values are the same as for DECSET.

        public static string ChangeAttrInArea = "\x1b[{Pt};{Pl};{Pb};{Pr};{Ps}$r"; // CSI Pt ; Pl ; Pb ; Pr ; Ps $ r
        // Change Attributes in Rectangular Area (DECCARA), VT400 and up.
        //  Pt ; Pl ; Pb ; Pr denotes the rectangle.
        //  Ps denotes the SGR attributes to change: 0, 1, 4, 5, 7.

        public static string SaveCursor = "\x1b[s"; // CSI s     Save cursor, available only when DECLRMM is disabled (SCOSC, also ANSI.SYS).

        public static string SetMargins = "\x1b[{Pl};{Pr}s"; // CSI Pl ; Pr s
        // Set left and right margins (DECSLRM), VT420 and up.  This is available only when DECLRMM is enabled.

        public static string SavePrivateModeValuesDec = "\x1b[?{Pm}s"; // CSI ? Pm s
        //Save DEC Private Mode Values (XTSAVE), xterm.  Ps values are the same as for DECSET.

            
        public static string WindowManipulation = "\x1b[{Ps};{Ps};{Ps}t"; // CSI Ps ; Ps ; Ps t
        /*
        Window manipulation (XTWINOPS), dtterm, extended by xterm. These controls may be disabled using the allowWindowOps resource.

        xterm uses Extended Window Manager Hints (EWMH) to maximize the window.  
        Some window managers have incomplete support for EWMH.  For instance, fvwm, flwm and quartz-wm advertise support for 
        maximizing windows horizontally or vertically, but in fact equate those to the maximize operation.

        Valid values for the first (and any additional parameters) are:
        Ps = 1  ⇒  De-iconify window.
        Ps = 2  ⇒  Iconify window.
        Ps = 3 ;  x ;  y ⇒  Move window to [x, y].
        Ps = 4 ;  height ;  width ⇒  Resize the xterm window to given height and width in pixels.  Omitted parameters reuse the current height or width.  Zero parameters use the display's height or width.
        Ps = 5  ⇒  Raise the xterm window to the front of the stacking order.
        Ps = 6  ⇒  Lower the xterm window to the bottom of the stacking order.
        Ps = 7  ⇒  Refresh the xterm window.
        Ps = 8 ;  height ;  width ⇒  Resize the text area to given height and width in characters.  Omitted parameters reuse the current height or width.  Zero parameters use the display's height or width.
        Ps = 9 ;  0  ⇒  Restore maximized window.
        Ps = 9 ;  1  ⇒  Maximize window (i.e., resize to screen size).
        Ps = 9 ;  2  ⇒  Maximize window vertically.
        Ps = 9 ;  3  ⇒  Maximize window horizontally.
        Ps = 10 ;  0  ⇒  Undo full-screen mode.
        Ps = 10 ;  1  ⇒  Change to full-screen.
        Ps = 10 ;  2  ⇒  Toggle full-screen.
        Ps = 11  ⇒  Report xterm window state. If the xterm window is non-iconified, it returns CSI 1 t . If the xterm window is iconified, it 
                        returns CSI 2 t .
        Ps = 13  ⇒  Report xterm window position. Note: X Toolkit positions can be negative, but the reported values are unsigned, in the range 0-65535.  Negative values correspond to 32768-65535. 
                        Result is CSI 3 ; x ; y t
        Ps = 13 ;  2  ⇒  Report xterm text-area position. 
                        Result is CSI 3 ; x ; y t
        Ps = 14  ⇒  Report xterm text area size in pixels. 
                        Result is CSI  4 ;  height ;  width t
        Ps = 14 ;  2  ⇒  Report xterm window size in pixels. Normally xterm's window is larger than its text area, since it includes the frame (or decoration) applied by the window manager, as well as the area used by a scroll-bar. 
                        Result is CSI  4 ;  height ;  width t
        Ps = 15  ⇒  Report size of the screen in pixels. 
                        Result is CSI  5 ;  height ;  width t
        Ps = 16  ⇒  Report xterm character cell size in pixels. 
                        Result is CSI  6 ;  height ;  width t
        Ps = 18  ⇒  Report the size of the text area in characters. 
                        Result is CSI  8 ;  height ;  width t
        Ps = 19  ⇒  Report the size of the screen in characters. 
                        Result is CSI  9 ;  height ;  width t
        Ps = 20  ⇒  Report xterm window's icon label. 
                        Result is OSC  L  label ST
        Ps = 21  ⇒  Report xterm window's title. 
                        Result is OSC  l  label ST
        Ps = 22 ; 0  ⇒  Save xterm icon and window title on stack.
        Ps = 22 ; 1  ⇒  Save xterm icon title on stack.
        Ps = 22 ; 2  ⇒  Save xterm window title on stack.
        Ps = 23 ; 0  ⇒  Restore xterm icon and window title from stack.
        Ps = 23 ; 1  ⇒  Restore xterm icon title from stack.
        Ps = 23 ; 2  ⇒  Restore xterm window title from stack.
        Ps >= 24  ⇒  Resize to Ps lines (DECSLPP), VT340 and VT420. xterm adapts this by resizing its window.
        */

        public static string TitleStuff = "\x1b[>{Pm}t"; // CSI > Pm t
        /*
        This xterm control sets one or more features of the title modes (XTSMTITLE), xterm.  Each parameter enables a single feature.
            Ps = 0  ⇒  Set window/icon labels using hexadecimal.
            Ps = 1  ⇒  Query window/icon labels using hexadecimal.
            Ps = 2  ⇒  Set window/icon labels using UTF-8.
            Ps = 3  ⇒  Query window/icon labels using UTF-8.  (See discussion of Title Modes)
        */

        public static string WarningBellVolume = "\x1b[{Ps} t"; // CSI Ps SP t
        /*
        Set warning-bell volume (DECSWBV), VT520.
            Ps = 0  or 1  ⇒  off.
            Ps = 2 , 3  or 4  ⇒  low.
            Ps = 5 , 6 , 7 , or 8  ⇒  high.
        */

        public static string ReverseAttrArea = "\x1b[{Pt};{Pl};{Pb};{Pr};{Ps}$t"; // CSI Pt ; Pl ; Pb ; Pr ; Ps $ t
        /*
        Reverse Attributes in Rectangular Area (DECRARA), VT400 and up.
            Pt ; Pl ; Pb ; Pr denotes the rectangle.
            Ps denotes the attributes to reverse, i.e.,  1, 4, 5, 7.
        */

        public static string CursorRestore = "\x1b[u"; // CSI u     Restore cursor (SCORC, also ANSI.SYS).

        public static string MarginBellVolume = "\x1b[{Ps} u"; // CSI Ps SP u
        /*
        Set margin-bell volume (DECSMBV), VT520.
            Ps = 0 , 5 , 6 , 7 , or 8  ⇒  high.
            Ps = 1  ⇒  off.
            Ps = 2 , 3  or 4  ⇒  low.
        */

        public static string CopyArea = "\x1b[{Pt};{Pl};{Pb};{Pr};{Pp};{Pt}{Pl}{Pp}$v"; // CSI Pt ; Pl ; Pb ; Pr ; Pp ; Pt ; Pl ; Pp $ v
        /*
        Copy Rectangular Area (DECCRA), VT400 and up.
            Pt ; Pl ; Pb ; Pr denotes the rectangle.
            Pp denotes the source page.
            Pt ; Pl denotes the target location.
            Pp denotes the target page.
        */

        public static string RequestPresentationPageReport = "\x1b[{Ps}$w"; // CSI Ps $ w
        /*
        Request presentation state report (DECRQPSR), VT320 and up.
            Ps = 0  ⇒  error.
            Ps = 1  ⇒  cursor information report (DECCIR).
        Response is
            DCS 1 $ u Pt ST
        Refer to the VT420 programming manual, which requires six pages to document the data string Pt,
            Ps = 2  ⇒  tab stop report (DECTABSR).
        Response is
            DCS 2 $ u Pt ST
        The data string Pt is a list of the tab-stops, separated by "/" characters.
        */

        public static string EnableFilterRectange = "\x1b[{Pt};{Pl};{Pb};{Pr}'w"; // CSI Pt ; Pl ; Pb ; Pr ' w
        /*
        Enable Filter Rectangle (DECEFR), VT420 and up.
        Parameters are [top;left;bottom;right].
        Defines the coordinates of a filter rectangle and activates it.  Anytime the locator is detected outside of the filter
        rectangle, an outside rectangle event is generated and the rectangle is disabled.  Filter rectangles are always treated
        as "one-shot" events.  Any parameters that are omitted default to the current locator position.  If all parameters are 
        omitted, any locator motion will be reported.  DECELR always cancels any previous rectangle definition.
        */

        public static string RequestTerminalParameters = "\x1b[{Ps}x"; // CSI Ps x  Request Terminal Parameters (DECREQTPARM).
        /*
        if Ps is a "0" (default) or "1", and xterm is emulating VT100, the control sequence elicits a response of the 
        same form whose parameters describe the terminal:
            Ps ⇒  the given Ps incremented by 2.
            Pn = 1  ⇐  no parity.
            Pn = 1  ⇐  eight bits.
            Pn = 1  ⇐  2 8  transmit 38.4k baud.
            Pn = 1  ⇐  2 8  receive 38.4k baud.
            Pn = 1  ⇐  clock multiplier.
            Pn = 0  ⇐  STP flags.
        */

        public static string SelectAttrChangeExtent = "\x1b[{Ps}*x"; // CSI Ps * x
        /*
        Select Attribute Change Extent (DECSACE), VT420 and up.
            Ps = 0  ⇒  from start to end position, wrapped.
            Ps = 1  ⇒  from start to end position, wrapped.
            Ps = 2  ⇒  rectangle (exact).
        */

        public static string FillArea = "\x1b[{Pt};{Pl};{Pb};{Pr}$x"; // CSI Pc ; Pt ; Pl ; Pb ; Pr $ x
        /*
        Fill Rectangular Area (DECFRA), VT420 and up.
            Pc is the character to use.
            Pt ; Pl ; Pb ; Pr denotes the rectangle.
        */

        public static string EnableLocatorReporting = "\x1b[{Ps};{Pu}'z"; // CSI Ps ; Pu ' z
        /*
        Enable Locator Reporting (DECELR).
        Valid values for the first parameter:
            Ps = 0  ⇒  Locator disabled (default).
            Ps = 1  ⇒  Locator enabled.
            Ps = 2  ⇒  Locator enabled for one report, then disabled.
        The second parameter specifies the coordinate unit for locator reports.
        Valid values for the second parameter:
            Pu = 0  or omitted ⇒  default to character cells.
            Pu = 1  ⇐  device physical pixels.
            Pu = 2  ⇐  character cells.
        */

        public static string EraseArea = "\x1b[{Pt};{Pl};{Pb};{Pr}$z"; // CSI Pt ; Pl ; Pb ; Pr $ z
        /*
        Erase Rectangular Area (DECERA), VT400 and up.
            Pt ; Pl ; Pb ; Pr denotes the rectangle.
        */

        public static string SelectLocatorEvents = "\x1b[{Pm}'{"; // CSI Pm ' {
        /*
        Select Locator Events (DECSLE).
        Valid values for the first (and any additional parameters) are:
            Ps = 0  ⇒  only respond to explicit host requests (DECRQLP).
        This is default.  It also cancels any filter rectangle.
            Ps = 1  ⇒  report button down transitions.
            Ps = 2  ⇒  do not report button down transitions.
            Ps = 3  ⇒  report button up transitions.
            Ps = 4  ⇒  do not report button up transitions.
        */

        public static string PushVideoAttr  = "\x1b[#{";     // CSI # {
        public static string PushVideoAttrs = "\x1b[{Pm}#{"; // CSI Pm # {
        /*
        Push video attributes onto stack (XTPUSHSGR), xterm.  The optional parameters correspond to the SGR encoding for video
        attributes, except for colors (which do not have a unique SGR code):
            Ps = 1  ⇒  Bold.
            Ps = 2  ⇒  Faint.
            Ps = 3  ⇒  Italicized.
            Ps = 4  ⇒  Underlined.
            Ps = 5  ⇒  Blink.
            Ps = 7  ⇒  Inverse.
            Ps = 8  ⇒  Invisible.
            Ps = 9  ⇒  Crossed-out characters.
            Ps = 2 1  ⇒  Doubly-underlined.
            Ps = 3 0  ⇒  Foreground color.
            Ps = 3 1  ⇒  BackgroundBrush color.

        If no parameters are given, all of the video attributes are saved.  The stack is limited to 10 levels.
        */

        public static string SelectiveEraseArea = "\x1b[{Pt};{Pl};{Pb};{Pr}${"; // CSI Pt ; Pl ; Pb ; Pr $ {
        /*
        Selective Erase Rectangular Area (DECSERA), VT400 and up.
            Pt ; Pl ; Pb ; Pr denotes the rectangle.
        */

        public static string ReportSelectdGfxRend = "\x1b[{Pt};{Pl};{Pb};{Pr}#|"; // CSI Pt ; Pl ; Pb ; Pr # |
        /*
        Report selected graphic rendition (XTREPORTSGR), xterm.  The response is an SGR sequence which contains the attributes
        which are common to all cells in a rectangle.
            Pt ; Pl ; Pb ; Pr denotes the rectangle.
        */

        public static string SelectColumnsPP = "\x1b[{Ps}$|"; // CSI Ps $ |
        /*
        Select columns per page (DECSCPP), VT340.
            Ps = 0  ⇒  80 columns, default if Ps omitted.
            Ps = 80  ⇒  80 columns.
            Ps = 132  ⇒  132 columns.
        */

        public static string RequestLocatorPosition = "\x1b[{Ps{'|"; // CSI Ps ' |
        /*
        Request Locator Position (DECRQLP).
        Valid values for the parameter are:
            Ps = 0 , 1 or omitted ⇒  transmit a single DECLRP locator report.

        If Locator Reporting has been enabled by a DECELR, xterm will respond with a DECLRP Locator Report.  This report is also
        generated on button up and down events if they have been enabled with a DECSLE, or when the locator is detected outside
        of a filter rectangle, if filter rectangles have been enabled with a DECEFR.

            ⇐  CSI Pe ; Pb ; Pr ; Pc ; Pp &  w

        Parameters are [event;button;row;column;page].
        Valid values for the event:
            Pe = 0  ⇐  locator unavailable - no other parameters sent.
            Pe = 1  ⇐  request - xterm received a DECRQLP.
            Pe = 2  ⇐  left button down.
            Pe = 3  ⇐  left button up.
            Pe = 4  ⇐  middle button down.
            Pe = 5  ⇐  middle button up.
            Pe = 6  ⇐  right button down.
            Pe = 7  ⇐  right button up.
            Pe = 8  ⇐  M4 button down.
            Pe = 9  ⇐  M4 button up.
            Pe = 10  ⇐  locator outside filter rectangle.
        The "button" parameter is a bitmask indicating which buttons are pressed:
            Pb = 0  ⇐  no buttons down.
            Pb & 1  ⇐  right button down.
            Pb & 2  ⇐  middle button down.
            Pb & 4  ⇐  left button down.
            Pb & 8  ⇐  M4 button down.
        The "row" and "column" parameters are the coordinates of the locator position in the xterm window, encoded as ASCII decimal.
        The "page" parameter is not used by xterm.
        */

        public static string SelectLinesPerScreen = "\x1b[{Ps}*|"; // CSI Ps * |
        // Select number of lines per screen (DECSNLS), VT420 and up.

        public static string PopVideaAttr = "\x1b[#}"; // CSI # }   Pop video attributes from stack (XTPOPSGR), xterm.  Popping
        // restores the video-attributes which were saved using XTPUSHSGR to their previous state.

        public static string InsertColumns = "\x1b[{Ps}'}"; // CSI Ps ' }
        // Insert Ps Column(s) (default = 1) (DECIC), VT420 and up.

        public static string DeleteColumns = "\x1b[{Ps}'~"; // CSI Ps ' ~
        // Delete Ps Column(s) (default = 1) (DECDC), VT420 and up.             



            
        public static string SetTextParameter  = "\x1b]{Ps};{Pt}\x07";   // OSC Ps ; Pt BEL
        public static string SetTextParameters = "\x1b]{Ps};{Pt}\x1b\\"; // OSC Ps ; Pt ST
        /*
        Set Text Parameters.  Some control sequences return information:
        o   For colors and font, if Pt is a "?", the control sequence elicits a response which consists of the control sequence which would set the corresponding value.
        o   The dtterm control sequences allow you to determine the icon name and window title.

        XTerm accepts either BEL  or ST  for terminating OSC sequences, and when returning information, uses the same terminator 
        used in a query.  While the latter is preferred, the former is supported for legacy applications:
        o   Although documented in the changes for X.V10R4 (December 1986), BEL  as a string terminator dates from X11R4 (December 1989).
        o   Since XFree86-3.1.2Ee (August 1996), xterm has accepted ST (the documented string terminator in ECMA-48).

        Ps specifies the type of operation to perform:
            Ps = 0  ⇒  Change Icon Name and Window Title to Pt.
            Ps = 1  ⇒  Change Icon Name to Pt.
            Ps = 2  ⇒  Change Window Title to Pt.
            Ps = 3  ⇒  Set X property on top-level window.  Pt should be in the form "prop=value", or just "prop" to delete the property.
            Ps = 4 ; c ; spec ⇒  Change Color Number c to the color specified by spec.  
                        This can be a name or RGB specification as per XParseColor.  Any number of c/spec pairs may be given.
                        The color numbers correspond to the ANSI colors 0-7, their bright versions 8-15, and if supported, 
                        the remainder of the 88-color or 256-color table.

        If a "?" is given rather than a name or RGB specification, xterm replies with a control sequence of the same form which
        can be used to set the corresponding color.  Because more than one pair of color number and specification can be given in one
        control sequence, xterm can make more than one reply.

            Ps = 5 ; c ; spec ⇒  Change Special Color Number c to the color specified by spec.  
                        This can be a name or RGB specification 
                        as per XParseColor.  Any number of c/spec pairs may be given.  The special colors can also be set by 
                        adding the maximum  number of colors to these codes in an OSC 4  control:

                        Pc = 0  ⇐  resource colorBD (BOLD).
                        Pc = 1  ⇐  resource colorUL (UNDERLINE).
                        Pc = 2  ⇐  resource colorBL (BLINK).
                        Pc = 3  ⇐  resource colorRV (REVERSE).
                        Pc = 4  ⇐  resource colorIT (ITALIC).

            Ps = 6 ; c ; f ⇒  Enable/disable Special Color Number c.
                        The second parameter tells xterm to enable the corresponding color mode if nonzero, disable it if zero.  
                        OSC 6  is the same as OSC 106 .

                        The 10 colors (below) which may be set or queried using 10 through 19  are denoted dynamic colors, 
                        since the corresponding control sequences were the first means for setting xterm's colors dynamically, 
                        i.e., after it was started.  
                        They are not the same as the ANSI colors (however, the dynamic text foreground and background colors 
                        are used when ANSI colors are reset using SGR 3 9  and 4 9 , respectively).  These controls may be 
                        disabled using the allowColorOps resource.  At least one parameter is expected for Pt.  Each 
                        successive parameter changes the next color in the list.  The value of Ps tells the starting point in 
                        the list.  The colors are specified by name or RGB specification as per XParseColor.

                        If a "?" is given rather than a name or RGB specification, xterm replies with a control sequence of 
                        the same form which can be used to set the corresponding dynamic color.  Because more than one pair 
                        of color number and specification can be given in one control sequence, xterm can make more than one reply.

            Ps = 10  ⇒  Change VT100 text foreground color to Pt.
            Ps = 11  ⇒  Change VT100 text background color to Pt.
            Ps = 12  ⇒  Change text cursor color to Pt.
            Ps = 13  ⇒  Change pointer foreground color to Pt.
            Ps = 14  ⇒  Change pointer background color to Pt.
            Ps = 15  ⇒  Change Tektronix foreground color to Pt.
            Ps = 16  ⇒  Change Tektronix background color to Pt.
            Ps = 17  ⇒  Change highlight background color to Pt.
            Ps = 18  ⇒  Change Tektronix cursor color to Pt.
            Ps = 19  ⇒  Change highlight foreground color to Pt.

            Ps = 46  ⇒  Change Log File to Pt.  This is normally disabled by a compile-time option.

            Ps = 50  ⇒  Set Font to Pt.  These controls may be disabled using the allowFontOps resource.  If Pt begins with a "#",
                        index in the font menu, relative (if the next character is a plus or minus sign) or absolute.  
                        A number is expected but not required after the sign (the default is the current entry for
                        relative, zero for absolute indexing).

                        The same rule (plus or minus sign, optional number) is used when querying the font.  
                        The remainder of Pt is ignored.
                        A font can be specified after a "#" index expression, by adding a space and then the font specifier.
                        If the TrueType Fonts menu entry is set (the renderFont resource), then this control sets/queries the faceName resource.

            Ps = 51  ⇒  reserved for Emacs shell.

            Ps = 52  ⇒  Manipulate Selection Data.  These controls may be disabled using the allowWindowOps resource.  
                        The parameter Pt is parsed as Pc ; Pd
                        The first, Pc, may contain zero or more characters from the set 
                            c , p , q , s , 0 , 1 , 2 , 3 , 4 , 5 , 6 , and 7 .  
                        It is used to construct a list of selection parameters for clipboard, primary, secondary, select, 
                        or cut buffers 0 through 7 respectively, in the order given.  If the parameter is empty,
                        xterm uses s 0 , to specify the configurable primary/clipboard selection and cut buffer 0.

                        The second parameter, Pd, gives the selection data.  Normally
                        this is a string encoded in base64 (RFC-4648).  The data
                        becomes the new selection, which is then available for pasting
                        by other applications.

                        If the second parameter is a ? , xterm replies to the host
                        with the selection data encoded using the same protocol.  It
                        uses the first selection found by asking successively for each
                        item from the list of selection parameters.

                        If the second parameter is neither a base64 string nor ? ,
                        then the selection is cleared.

            Ps = 104 ; c ⇒  Reset Color Number c.  
                        It is reset to the color specified by the corresponding X resource.  Any number
                        of c parameters may be given.  These parameters correspond to
                        the ANSI colors 0-7, their bright versions 8-15, and if sup-
                        ported, the remainder of the 88-color or 256-color table.  If
                        no parameters are given, the entire table will be reset.

            Ps = 105 ; c ⇒  Reset Special Color Number c.  
                        It is reset to the color specified by the corresponding X resource.  Any
                        number of c parameters may be given.  These parameters corre-
                        spond to the special colors which can be set using an OSC 5
                        control (or by adding the maximum number of colors using an
                        OSC 4  control).

            Ps = 106 ; c ; f ⇒  Enable/disable Special Color Number c.
                        The second parameter tells xterm to enable the corresponding
                        color mode if nonzero, disable it if zero.
        
                        Pc = 0  ⇐  resource colorBDMode (BOLD).
                        Pc = 1  ⇐  resource colorULMode (UNDERLINE).
                        Pc = 2  ⇐  resource colorBLMode (BLINK).
                        Pc = 3  ⇐  resource colorRVMode (REVERSE).
                        Pc = 4  ⇐  resource colorITMode (ITALIC).
                        Pc = 5  ⇐  resource colorAttrMode (Override ANSI).

            The dynamic colors can also be reset to their default (resource) values:
            Ps = 110  ⇒  Reset VT100 text foreground color.
            Ps = 111  ⇒  Reset VT100 text background color.
            Ps = 112  ⇒  Reset text cursor color.
            Ps = 113  ⇒  Reset pointer foreground color.
            Ps = 114  ⇒  Reset pointer background color.
            Ps = 115  ⇒  Reset Tektronix foreground color.
            Ps = 116  ⇒  Reset Tektronix background color.
            Ps = 117  ⇒  Reset highlight color.
            Ps = 118  ⇒  Reset Tektronix cursor color.
            Ps = 119  ⇒  Reset highlight foreground color.

            Ps = I  ; c ⇒  Set icon to file.  Sun shelltool, CDE dtterm.
                        The file is expected to be XPM format, and uses the same
                        search logic as the iconHint resource.

            Ps = l  ; c ⇒  Set window title.  Sun shelltool, CDE dtterm.

            Ps = L  ; c ⇒  Set icon label.  Sun shelltool, CDE dtterm.
         */

        // ReSharper restore InconsistentNaming
    }
}