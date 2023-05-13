//#define XTERM1006

using System.Runtime.InteropServices;

namespace Asciis.App.Platforms;

// ReSharper disable InconsistentNaming

internal class Curses
{
    public const int A_NORMAL    = 0x00_0000;
    public const int A_STANDOUT  = 0x01_0000;
    public const int A_UNDERLINE = 0x02_0000;
    public const int A_REVERSE   = 0x04_0000;
    public const int A_BLINK     = 0x08_0000;
    public const int A_DIM       = 0x10_0000;
    public const int A_BOLD      = 0x20_0000;
    public const int A_PROTECT   = 0x10_00000;
    public const int A_INVIS     = 0x80_0000;
    
    public const int ACS_LLCORNER = 0x40_006d;
    public const int ACS_LRCORNER = 0x40_006a;
    public const int ACS_HLINE    = 0x40_0071;
    public const int ACS_ULCORNER = 0x40_006c;
    public const int ACS_URCORNER = 0x40_006b;
    public const int ACS_VLINE    = 0x40_0078;
    public const int ACS_LTEE     = 0x40_0074;
    public const int ACS_RTEE     = 0x40_0075;
    public const int ACS_BTEE     = 0x40_0076;
    public const int ACS_TTEE     = 0x40_0077;
    public const int ACS_PLUS     = 0x40_006e;
    public const int ACS_S1       = 0x40_006f;
    public const int ACS_S9       = 0x40_0073;
    public const int ACS_DIAMOND  = 0x40_0060;
    public const int ACS_CKBOARD  = 0x40_0061;
    public const int ACS_DEGREE   = 0x40_0066;
    public const int ACS_PLMINUS  = 0x40_0067;
    public const int ACS_BULLET   = 0x40_007e;
    public const int ACS_LARROW   = 0x40_002c;
    public const int ACS_RARROW   = 0x40_002b;
    public const int ACS_DARROW   = 0x40_002e;
    public const int ACS_UARROW   = 0x40_002d;
    public const int ACS_BOARD    = 0x40_0068;
    public const int ACS_LANTERN  = 0x40_0069;
    public const int ACS_BLOCK    = 0x40_0030;
    
    public const int COLOR_BLACK   = 0x0;
    public const int COLOR_RED     = 0x1;
    public const int COLOR_GREEN   = 0x2;
    public const int COLOR_YELLOW  = 0x3;
    public const int COLOR_BLUE    = 0x4;
    public const int COLOR_MAGENTA = 0x5;
    public const int COLOR_CYAN    = 0x6;
    public const int COLOR_WHITE   = 0x7;
    public const int COLOR_GRAY    = 0x8;
    
    public const int KEY_CODE_YES = 0x100;
    public const int KEY_CODE_SEQ = 0x5b;
    
    public const int ERR = unchecked((int)0xffffffff);
    
    public const int TIOCGWINSZ     = 0x5413;
    public const int TIOCGWINSZ_MAC = 0x40087468;

    // ReSharper restore InconsistentNaming

    // For the ncurses-compatible functions only, BUTTON4_PRESSED and
    // BUTTON5_PRESSED are returned for mouse scroll wheel up and down;
    //   otherwise PDCurses doesn't support buttons 4 and 5
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum InputEvent : long
    {
        Button1Released      = 0x00000001L,
        Button1Pressed       = 0x00000002L,
        Button1Clicked       = 0x00000004L,
        Button1DoubleClicked = 0x00000008L,
        Button1TripleClicked = 0x00000010L,
        Button1Moved         = 0x00000010L,

        Button2Released      = 0x00000020L,
        Button2Pressed       = 0x00000040L,
        Button2Clicked       = 0x00000080L,
        Button2DoubleClicked = 0x00000100L,
        Button2TripleClicked = 0x00000200L,
        Button2Moved         = 0x00000200L,

        Button3Released      = 0x00000400L,
        Button3Pressed       = 0x00000800L,
        Button3Clicked       = 0x00001000L,
        Button3DoubleClicked = 0x00002000L,
        Button3TripleClicked = 0x00004000L,
        Button3Moved         = 0x00004000L,

        Button4Released      = 0x00008000L,
        Button4Pressed       = 0x00010000L,
        Button4Clicked       = 0x00020000L,
        Button4DoubleClicked = 0x00040000L,
        Button4TripleClicked = 0x00080000L,

        Button5Released      = 0x00100000L,
        Button5Pressed       = 0x00200000L,
        Button5Clicked       = 0x00400000L,
        Button5DoubleClicked = 0x00800000L,
        Button5TripleClicked = 0x01000000L,

        MouseWheelScroll      = 0x02000000L,
        ButtonModifierShift   = 0x04000000L,
        ButtonModifierControl = 0x08000000L,
        ButtonModifierAlt     = 0x10000000L,

        AllMouseEvents      = 0x1fffffffL,
        ReportMousePosition = 0x20000000L
    }
    
    // We encode ESC + char (what Alt-char generates) as 0x2000 + char
    public const int KeyAlt = 0x2000;

    
#if XTERM1006
	public const int LeftRightUpNPagePPage = 0x8;
	public const int DownEnd = 0x6;
	public const int Home = 0x7;
#else
    public const int LeftRightUpNPagePPage = 0x0;
    public const int DownEnd               = 0x0;
    public const int Home                  = 0x0;
#endif
    public const int KeyBackspace      = 0x107;
    public const int KeyUp             = 0x103;
    public const int KeyDown           = 0x102;
    public const int KeyLeft           = 0x104;
    public const int KeyRight          = 0x105;
    public const int KeyNPage          = 0x152;
    public const int KeyPPage          = 0x153;
    public const int KeyHome           = 0x106;
    public const int KeyMouse          = 0x199;
    public const int KeyEnd            = 0x168;
    public const int KeyDeleteChar     = 0x14a;
    public const int KeyInsertChar     = 0x14b;
    public const int KeyTab            = 0x009;
    public const int KeyBackTab        = 0x161;
    public const int KeyF1             = 0x109;
    public const int KeyF2             = 0x10a;
    public const int KeyF3             = 0x10b;
    public const int KeyF4             = 0x10c;
    public const int KeyF5             = 0x10d;
    public const int KeyF6             = 0x10e;
    public const int KeyF7             = 0x10f;
    public const int KeyF8             = 0x110;
    public const int KeyF9             = 0x111;
    public const int KeyF10            = 0x112;
    public const int KeyF11            = 0x113;
    public const int KeyF12            = 0x114;
    public const int KeyResize         = 0x19a;
    
    public const int ShiftKeyUp        = 0x151;
    public const int ShiftKeyDown      = 0x150;
    public const int ShiftKeyLeft      = 0x189;
    public const int ShiftKeyRight     = 0x192;
    public const int ShiftKeyNPage     = 0x18c;
    public const int ShiftKeyPPage     = 0x18e;
    public const int ShiftKeyHome      = 0x187;
    public const int ShiftKeyEnd       = 0x182;
    
    public const int AltKeyUp          = unchecked(0x234 + LeftRightUpNPagePPage);
    public const int AltKeyDown        = unchecked(0x20b + DownEnd);
    public const int AltKeyLeft        = unchecked(0x21f + LeftRightUpNPagePPage);
    public const int AltKeyRight       = unchecked(0x22e + LeftRightUpNPagePPage);
    public const int AltKeyNPage       = unchecked(0x224 + LeftRightUpNPagePPage);
    public const int AltKeyPPage       = unchecked(0x229 + LeftRightUpNPagePPage);
    public const int AltKeyHome        = unchecked(0x215 + Home);
    public const int AltKeyEnd         = unchecked(0x210 + DownEnd);
    
    public const int CtrlKeyUp         = unchecked(0x236 + LeftRightUpNPagePPage);
    public const int CtrlKeyDown       = unchecked(0x20d + DownEnd);
    public const int CtrlKeyLeft       = unchecked(0x221 + LeftRightUpNPagePPage);
    public const int CtrlKeyRight      = unchecked(0x230 + LeftRightUpNPagePPage);
    public const int CtrlKeyNPage      = unchecked(0x226 + LeftRightUpNPagePPage);
    public const int CtrlKeyPPage      = unchecked(0x22b + LeftRightUpNPagePPage);
    public const int CtrlKeyHome       = unchecked(0x217 + Home);
    public const int CtrlKeyEnd        = unchecked(0x212 + DownEnd);
    
    public const int ShiftCtrlKeyUp    = unchecked(0x237 + LeftRightUpNPagePPage);
    public const int ShiftCtrlKeyDown  = unchecked(0x20e + DownEnd);
    public const int ShiftCtrlKeyLeft  = unchecked(0x222 + LeftRightUpNPagePPage);
    public const int ShiftCtrlKeyRight = unchecked(0x231 + LeftRightUpNPagePPage);
    public const int ShiftCtrlKeyNPage = unchecked(0x227 + LeftRightUpNPagePPage);
    public const int ShiftCtrlKeyPPage = unchecked(0x22c + LeftRightUpNPagePPage);
    public const int ShiftCtrlKeyHome  = unchecked(0x218 + Home);
    public const int ShiftCtrlKeyEnd   = unchecked(0x213 + DownEnd);
    
    public const int ShiftAltKeyUp     = unchecked(0x235 + LeftRightUpNPagePPage);
    public const int ShiftAltKeyDown   = unchecked(0x20c + DownEnd);
    public const int ShiftAltKeyLeft   = unchecked(0x220 + LeftRightUpNPagePPage);
    public const int ShiftAltKeyRight  = unchecked(0x22f + LeftRightUpNPagePPage);
    public const int ShiftAltKeyNPage  = unchecked(0x225 + LeftRightUpNPagePPage);
    public const int ShiftAltKeyPPage  = unchecked(0x22a + LeftRightUpNPagePPage);
    public const int ShiftAltKeyHome   = unchecked(0x216 + Home);
    public const int ShiftAltKeyEnd    = unchecked(0x211 + DownEnd);
    
    public const int AltCtrlKeyNPage   = unchecked(0x228 + LeftRightUpNPagePPage);
    public const int AltCtrlKeyPPage   = unchecked(0x22d + LeftRightUpNPagePPage);
    public const int AltCtrlKeyHome    = unchecked(0x219 + Home);
    public const int AltCtrlKeyEnd     = unchecked(0x214 + DownEnd);

    // see #949
    public static int LC_ALL { get; private set; }

    static Curses()
    {
        LC_ALL = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? 0 : 6;
        
        var libs = UnmanagedLibrary.IsMacOS
            ? new string[] { "libncurses.dylib" }
            : new string[] { "libncursesw.so.6", "libncursesw.so.5" };
        CursesLibrary = new UnmanagedLibrary(libs, false);
        Methods       = new Unix.NativeMethods(CursesLibrary);

        StdScr    = ReadStaticPtr("stdscr");
        CurScrPtr = GetPtr("curscr");
        LinesPtr  = GetPtr("LINES");
        ColsPtr   = GetPtr("COLS");
        
        setlocale(LC_ALL, "");
    }

    public static int ColorPair(int n) => 0 + n * 256;

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseEvent
    {
        public short ID;
        public int X, Y, Z;
        public InputEvent ButtonState;
    }

    private static          int                _lines, _cols;
    private static          Window?            _mainWindow;
    private static readonly IntPtr             CurScrPtr;
    private static readonly IntPtr             LinesPtr;
    private static readonly IntPtr             ColsPtr;
    private static readonly IntPtr             StdScr;
    private static readonly UnmanagedLibrary   CursesLibrary;
    private static readonly Unix.NativeMethods Methods;
    
    // If true, uses the DllImport into "ncurses", otherwise "libncursesw.so.5"
    //static bool use_naked_driver;

    public static int Lines => _lines;
    public static int Cols  => _cols;

    [DllImport("libc")]
    public static extern int setlocale(int cate, [MarshalAs(UnmanagedType.LPStr)] string locale);

    public static Window InitScr()
    {
        // Prevents the terminal from being locked after exiting.
        ResetShellMode();

        _mainWindow = new Window(Methods.initscr());
        try
        {
            ConsoleSharpGetDims(out _lines, out _cols);
        }
        catch (DllNotFoundException)
        {
            EndWin();
            Console.Error.WriteLine(
                "Unable to find the @MONO_CURSES@ native library\n" +
                "this is different than the managed mono-curses.dll\n\n" +
                "Typically you need to install to a LD_LIBRARY_PATH directory\n" +
                "or DYLD_LIBRARY_PATH directory or run /sbin/ldconfig");
            Environment.Exit(1);
        }

        return _mainWindow;
    }


    /// <summary>
    /// Returns true if the window changed since the last invocation, as a side effect, the Lines and Cols properties are updated
    /// </summary>
    public static bool CheckWinChange()
    {
        ConsoleSharpGetDims(out var lines, out var cols);

        if (lines == 1 || lines != _lines || cols != _cols)
        {
            _lines = lines;
            _cols = cols;
            //if (l <= 0 || c <= 0) {
            //	Console.Out.Write ($"\x1b[8;50;{c}t");
            //	Console.Out.Flush ();
            //	return false;
            //}
            return true;
        }

        return false;
    }

    public static int AddStr(string format, params object[] args)
    {
        var s = string.Format(format, args);
        return AddWStr(s);
    }


    //
    // Have to wrap the native addch, as it can not
    // display unicode characters, we have to use addstr
    // for that.   but we need addch to render special ACS
    // characters
    //
    public static int AddCh(int ch)
    {
        if (ch < 127 || ch > 0xffff)
            return Methods.addch(ch);
        var c = (char)ch;
        return AddWStr(new string(c, 1));
    }


    private static IntPtr GetPtr(string key)
    {
        var ptr = CursesLibrary.LoadSymbol(key);

        if (ptr == IntPtr.Zero)
            throw new Exception("Could not load the key " + key);
        return ptr;
    }

    internal static IntPtr ReadStaticPtr(string key)
    {
        var ptr = GetPtr(key);
        return Marshal.ReadIntPtr(ptr);
    }

    internal static IntPtr ConsoleSharpGetStdScr() => StdScr;

    internal static IntPtr ConsoleSharpGetCurScr() => Marshal.ReadIntPtr(CurScrPtr);

    internal static void ConsoleSharpGetDims(out int lines, out int cols)
    {
        lines = Marshal.ReadInt32(LinesPtr);
        cols = Marshal.ReadInt32(ColsPtr);
    }

    public static InputEvent MouseMask(InputEvent newmask, out InputEvent oldmask)
    {
        IntPtr e;
        var ret = (InputEvent)Methods.mousemask((IntPtr)newmask, out e);
        oldmask = (InputEvent)e;
        return ret;
    }


    public static int IsAlt(int key)
    {
        if ((key & KeyAlt) != 0)
            return key & ~KeyAlt;
        return 0;
    }

    public static bool HasColors  => Methods.has_colors();
    public static int  ColorPairs => Methods.COLOR_PAIRS();

    public static int StartColor()       => Methods.start_color();
    public static int UseDefaultColors() => Methods.use_default_colors();

    public static int InitColorPair(short pair, short foreground, short background) =>
        Methods.init_pair(pair, foreground, background);


    //
    // The proxy methods to call into each version
    //
    public static int  EndWin()                                  => Methods.endwin();
    public static bool IsEndWin()                                => Methods.isendwin();
    public static int  CBreak()                                  => Methods.cbreak();
    public static int  NoCBreak()                                => Methods.nocbreak();
    public static int  Echo()                                    => Methods.echo();
    public static int  NoEcho()                                  => Methods.noecho();
    public static int  HalfDelay(int t)                          => Methods.halfdelay(t);
    public static int  Raw()                                     => Methods.raw();
    public static int  NoRaw()                                   => Methods.noraw();
    public static void NoQiFlush()                               => Methods.noqiflush();
    public static void QiFlush()                                 => Methods.qiflush();
    public static int  TypeAhead(IntPtr  fd)                     => Methods.typeahead(fd);
    public static int  Timeout(int       delay)                  => Methods.timeout(delay);
    public static int  WTimeout(IntPtr   win, int  delay)        => Methods.wtimeout(win, delay);
    public static int  NoTimeout(IntPtr  win, bool bf)           => Methods.notimeout(win, bf);
    public static int  Keypad(IntPtr     win, bool bf)           => Methods.keypad(win, bf);
    public static int  Meta(IntPtr       win, bool bf)           => Methods.meta(win, bf);
    public static int  IntrFlush(IntPtr  win, bool bf)           => Methods.intrflush(win, bf);
    public static int  ClearOk(IntPtr    win, bool bf)           => Methods.clearok(win, bf);
    public static int  IdlOk(IntPtr      win, bool bf)           => Methods.idlok(win, bf);
    public static void IdCok(IntPtr      win, bool bf)           => Methods.idcok(win, bf);
    public static void ImmedOk(IntPtr    win, bool bf)           => Methods.immedok(win, bf);
    public static int  LeaveOk(IntPtr    win, bool bf)           => Methods.leaveok(win, bf);
    public static int  WSetScrReg(IntPtr win, int  top, int bot) => Methods.wsetscrreg(win, top, bot);
    public static int  ScrollOk(IntPtr   win, bool bf) => Methods.scrollok(win, bf);
    public static int  Nl()                        => Methods.nl();
    public static int  NoNl()                      => Methods.nonl();
    public static int  SetScrReg(int top, int bot) => Methods.setscrreg(top, bot);
    public static int  Refresh()                          => Methods.refresh();
    public static int  DoUpdate()                         => Methods.doupdate();
    public static int  WRefresh(IntPtr     win)           => Methods.wrefresh(win);
    public static int  RedrawWin(IntPtr    win)           => Methods.redrawwin(win);
    public static int  WNoutRefresh(IntPtr win)           => Methods.wnoutrefresh(win);
    public static int  Move(int            line, int col) => Methods.move(line, col);

    public static int CursSet(int visibility) => Methods.curs_set(visibility);
    public static int  AddWStr(string s)                      => Methods.addwstr(s);
    public static int  WMove(IntPtr   win, int line, int col) => Methods.wmove(win, line, col);
    public static int  WAddCh(IntPtr  win, int ch) => Methods.waddch(win, ch);
    public static int  AttrOn(int     attrs)                  => Methods.attron(attrs);
    public static int  AttrOff(int    attrs)                  => Methods.attroff(attrs);
    public static int  AttrSet(int    attrs)                  => Methods.attrset(attrs);
    public static int  GetCh()                                => Methods.getch();
    public static int  GetWCh(out int sequence)               => Methods.get_wch(out sequence);
    public static int  UnGetCh(int    ch)                     => Methods.ungetch(ch);
    public static int  MvGetCh(int    y,    int   x)          => Methods.mvgetch(y, x);
    public static int  InitPair(short pair, short f, short b) => Methods.init_pair(pair, f, b);
    public static int  COLOR_PAIRS()                                 => Methods.COLOR_PAIRS();
    public static uint GetMouse(out   MouseEvent ev)                 => Methods.getmouse(out ev);
    public static uint UnGetMouse(ref MouseEvent ev)                 => Methods.ungetmouse(ref ev);
    public static int  MouseInterval(int         interval)           => Methods.mouseinterval(interval);
    public static bool IsTermResized(int         lines, int columns) => Methods.is_term_resized(lines, columns);
    public static int  resize_term(int           lines, int columns) => Methods.resize_term(lines, columns);
    public static int  ResizeTerm(int            lines, int columns) => Methods.resizeterm(lines, columns);
    public static void UseEnv(bool               f) => Methods.use_env(f);
    public static int  FlushInp()                   => Methods.flushinp();
    public static int  DefProgMode()                => Methods.def_prog_mode();
    public static int  DefShellMode()               => Methods.def_shell_mode();
    public static int  ResetProgMode()              => Methods.reset_prog_mode();
    public static int  ResetShellMode()             => Methods.reset_shell_mode();
    public static int  SaveTty()                    => Methods.savetty();
    public static int  ReseTty()                    => Methods.resetty();

    public class Window
    {
        public readonly         IntPtr Handle;

        static Window()
        {
            InitScr();
            Standard = new Window(ConsoleSharpGetStdScr());
            Current = new Window(ConsoleSharpGetCurScr());
        }

        internal Window(IntPtr handle) { Handle = handle; }

        public static Window Standard                     { get; }
        public static Window Current                      { get; }
        public        int    WTimeout(int   delay)        => Curses.WTimeout(Handle, delay);
        public        int    NoTimeout(bool bf)           => Curses.NoTimeout(Handle, bf);
        public        int    Keypad(bool    bf)           => Curses.Keypad(Handle, bf);
        public        int    Meta(bool      bf)           => Curses.Meta(Handle, bf);
        public        int    IntrFlush(bool bf)           => Curses.IntrFlush(Handle, bf);
        public        int    ClearOk(bool   bf)           => Curses.ClearOk(Handle, bf);
        public        int    IdlOk(bool     bf)           => Curses.IdlOk(Handle, bf);
        public        void   IdcOk(bool     bf)           => Curses.IdCok(Handle, bf);
        public        void   ImmedOk(bool   bf)           => Curses.ImmedOk(Handle, bf);
        public        int    LeaveOk(bool   bf)           => Curses.LeaveOk(Handle, bf);
        public        int    SetScrReg(int  top, int bot) => WSetScrReg(Handle, top, bot);
        public        int    ScrollOk(bool  bf)        => Curses.ScrollOk(Handle, bf);
        public        int    WRefresh()                => Curses.WRefresh(Handle);
        public        int    Redrawwin()               => Curses.RedrawWin(Handle);
        public        int    Wnoutrefresh()            => Curses.WNoutRefresh(Handle);
        public        int    Move(int   line, int col) => WMove(Handle, line, col);
        public        int    Addch(char ch)            => WAddCh(Handle, ch);
        public        int    Refresh()                 => Curses.WRefresh(Handle);
    }

    // Currently unused, to do later
    internal class CursesScreen
    {
        public readonly IntPtr Handle;
        internal CursesScreen(IntPtr handle) { Handle = handle; }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}
