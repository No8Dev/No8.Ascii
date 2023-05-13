//#define XTERM1006

using System.Runtime.InteropServices;

namespace Asciis.Terminal.ConsoleDrivers.CursesDriver;

public partial class Curses
{
    public const int A_NORMAL = unchecked((int)0x0);
    public const int A_STANDOUT = unchecked((int)0x10000);
    public const int A_UNDERLINE = unchecked((int)0x20000);
    public const int A_REVERSE = unchecked((int)0x40000);
    public const int A_BLINK = unchecked((int)0x80000);
    public const int A_DIM = unchecked((int)0x100000);
    public const int A_BOLD = unchecked((int)0x200000);
    public const int A_PROTECT = unchecked((int)0x1000000);
    public const int A_INVIS = unchecked((int)0x800000);
    public const int ACS_LLCORNER = unchecked((int)0x40006d);
    public const int ACS_LRCORNER = unchecked((int)0x40006a);
    public const int ACS_HLINE = unchecked((int)0x400071);
    public const int ACS_ULCORNER = unchecked((int)0x40006c);
    public const int ACS_URCORNER = unchecked((int)0x40006b);
    public const int ACS_VLINE = unchecked((int)0x400078);
    public const int ACS_LTEE = unchecked((int)0x400074);
    public const int ACS_RTEE = unchecked((int)0x400075);
    public const int ACS_BTEE = unchecked((int)0x400076);
    public const int ACS_TTEE = unchecked((int)0x400077);
    public const int ACS_PLUS = unchecked((int)0x40006e);
    public const int ACS_S1 = unchecked((int)0x40006f);
    public const int ACS_S9 = unchecked((int)0x400073);
    public const int ACS_DIAMOND = unchecked((int)0x400060);
    public const int ACS_CKBOARD = unchecked((int)0x400061);
    public const int ACS_DEGREE = unchecked((int)0x400066);
    public const int ACS_PLMINUS = unchecked((int)0x400067);
    public const int ACS_BULLET = unchecked((int)0x40007e);
    public const int ACS_LARROW = unchecked((int)0x40002c);
    public const int ACS_RARROW = unchecked((int)0x40002b);
    public const int ACS_DARROW = unchecked((int)0x40002e);
    public const int ACS_UARROW = unchecked((int)0x40002d);
    public const int ACS_BOARD = unchecked((int)0x400068);
    public const int ACS_LANTERN = unchecked((int)0x400069);
    public const int ACS_BLOCK = unchecked((int)0x400030);
    public const int COLOR_BLACK = unchecked((int)0x0);
    public const int COLOR_RED = unchecked((int)0x1);
    public const int COLOR_GREEN = unchecked((int)0x2);
    public const int COLOR_YELLOW = unchecked((int)0x3);
    public const int COLOR_BLUE = unchecked((int)0x4);
    public const int COLOR_MAGENTA = unchecked((int)0x5);
    public const int COLOR_CYAN = unchecked((int)0x6);
    public const int COLOR_WHITE = unchecked((int)0x7);
    public const int COLOR_GRAY = unchecked((int)0x8);
    public const int KEY_CODE_YES = unchecked((int)0x100);
    public const int KEY_CODE_SEQ = unchecked((int)0x5b);
    public const int ERR = unchecked((int)0xffffffff);
    public const int TIOCGWINSZ = unchecked((int)0x5413);
    public const int TIOCGWINSZ_MAC = unchecked((int)0x40087468);

    [Flags]
    public enum Event : long
    {
        Button1Pressed = unchecked((int)0x2),
        Button1Released = unchecked((int)0x1),
        Button1Clicked = unchecked((int)0x4),
        Button1DoubleClicked = unchecked((int)0x8),
        Button1TripleClicked = unchecked((int)0x10),
        Button2Pressed = unchecked((int)0x40),
        Button2Released = unchecked((int)0x20),
        Button2Clicked = unchecked((int)0x80),
        Button2DoubleClicked = unchecked((int)0x100),
        Button2TrippleClicked = unchecked((int)0x200),
        Button3Pressed = unchecked((int)0x800),
        Button3Released = unchecked((int)0x400),
        Button3Clicked = unchecked((int)0x1000),
        Button3DoubleClicked = unchecked((int)0x2000),
        Button3TripleClicked = unchecked((int)0x4000),
        ButtonWheeledUp = unchecked((int)0x10000),
        ButtonWheeledDown = unchecked((int)0x200000),
        Button4Pressed = unchecked((int)0x80000),
        Button4Released = unchecked((int)0x40000),
        Button4Clicked = unchecked((int)0x100000),
        Button4DoubleClicked = unchecked((int)0x20000),
        Button4TripleClicked = unchecked((int)0x400000),
        ButtonShift = unchecked((int)0x4000000),
        ButtonCtrl = unchecked((int)0x2000000),
        ButtonAlt = unchecked((int)0x8000000),
        ReportMousePosition = unchecked((int)0x10000000),
        AllEvents = unchecked((int)0x7ffffff)
    }
#if XTERM1006
		public const int LeftRightUpNPagePPage = unchecked((int)0x8);
		public const int DownEnd = unchecked((int)0x6);
		public const int Home = unchecked((int)0x7);
#else
    public const int LeftRightUpNPagePPage = unchecked((int)0x0);
    public const int DownEnd = unchecked((int)0x0);
    public const int Home = unchecked((int)0x0);
#endif
    public const int KeyBackspace = unchecked((int)0x107);
    public const int KeyUp = unchecked((int)0x103);
    public const int KeyDown = unchecked((int)0x102);
    public const int KeyLeft = unchecked((int)0x104);
    public const int KeyRight = unchecked((int)0x105);
    public const int KeyNPage = unchecked((int)0x152);
    public const int KeyPPage = unchecked((int)0x153);
    public const int KeyHome = unchecked((int)0x106);
    public const int KeyMouse = unchecked((int)0x199);
    public const int KeyEnd = unchecked((int)0x168);
    public const int KeyDeleteChar = unchecked((int)0x14a);
    public const int KeyInsertChar = unchecked((int)0x14b);
    public const int KeyTab = unchecked((int)0x009);
    public const int KeyBackTab = unchecked((int)0x161);
    public const int KeyF1 = unchecked((int)0x109);
    public const int KeyF2 = unchecked((int)0x10a);
    public const int KeyF3 = unchecked((int)0x10b);
    public const int KeyF4 = unchecked((int)0x10c);
    public const int KeyF5 = unchecked((int)0x10d);
    public const int KeyF6 = unchecked((int)0x10e);
    public const int KeyF7 = unchecked((int)0x10f);
    public const int KeyF8 = unchecked((int)0x110);
    public const int KeyF9 = unchecked((int)0x111);
    public const int KeyF10 = unchecked((int)0x112);
    public const int KeyF11 = unchecked((int)0x113);
    public const int KeyF12 = unchecked((int)0x114);
    public const int KeyResize = unchecked((int)0x19a);
    public const int ShiftKeyUp = unchecked((int)0x151);
    public const int ShiftKeyDown = unchecked((int)0x150);
    public const int ShiftKeyLeft = unchecked((int)0x189);
    public const int ShiftKeyRight = unchecked((int)0x192);
    public const int ShiftKeyNPage = unchecked((int)0x18c);
    public const int ShiftKeyPPage = unchecked((int)0x18e);
    public const int ShiftKeyHome = unchecked((int)0x187);
    public const int ShiftKeyEnd = unchecked((int)0x182);
    public const int AltKeyUp = unchecked((int)0x234 + LeftRightUpNPagePPage);
    public const int AltKeyDown = unchecked((int)0x20b + DownEnd);
    public const int AltKeyLeft = unchecked((int)0x21f + LeftRightUpNPagePPage);
    public const int AltKeyRight = unchecked((int)0x22e + LeftRightUpNPagePPage);
    public const int AltKeyNPage = unchecked((int)0x224 + LeftRightUpNPagePPage);
    public const int AltKeyPPage = unchecked((int)0x229 + LeftRightUpNPagePPage);
    public const int AltKeyHome = unchecked((int)0x215 + Home);
    public const int AltKeyEnd = unchecked((int)0x210 + DownEnd);
    public const int CtrlKeyUp = unchecked((int)0x236 + LeftRightUpNPagePPage);
    public const int CtrlKeyDown = unchecked((int)0x20d + DownEnd);
    public const int CtrlKeyLeft = unchecked((int)0x221 + LeftRightUpNPagePPage);
    public const int CtrlKeyRight = unchecked((int)0x230 + LeftRightUpNPagePPage);
    public const int CtrlKeyNPage = unchecked((int)0x226 + LeftRightUpNPagePPage);
    public const int CtrlKeyPPage = unchecked((int)0x22b + LeftRightUpNPagePPage);
    public const int CtrlKeyHome = unchecked((int)0x217 + Home);
    public const int CtrlKeyEnd = unchecked((int)0x212 + DownEnd);
    public const int ShiftCtrlKeyUp = unchecked((int)0x237 + LeftRightUpNPagePPage);
    public const int ShiftCtrlKeyDown = unchecked((int)0x20e + DownEnd);
    public const int ShiftCtrlKeyLeft = unchecked((int)0x222 + LeftRightUpNPagePPage);
    public const int ShiftCtrlKeyRight = unchecked((int)0x231 + LeftRightUpNPagePPage);
    public const int ShiftCtrlKeyNPage = unchecked((int)0x227 + LeftRightUpNPagePPage);
    public const int ShiftCtrlKeyPPage = unchecked((int)0x22c + LeftRightUpNPagePPage);
    public const int ShiftCtrlKeyHome = unchecked((int)0x218 + Home);
    public const int ShiftCtrlKeyEnd = unchecked((int)0x213 + DownEnd);
    public const int ShiftAltKeyUp = unchecked((int)0x235 + LeftRightUpNPagePPage);
    public const int ShiftAltKeyDown = unchecked((int)0x20c + DownEnd);
    public const int ShiftAltKeyLeft = unchecked((int)0x220 + LeftRightUpNPagePPage);
    public const int ShiftAltKeyRight = unchecked((int)0x22f + LeftRightUpNPagePPage);
    public const int ShiftAltKeyNPage = unchecked((int)0x225 + LeftRightUpNPagePPage);
    public const int ShiftAltKeyPPage = unchecked((int)0x22a + LeftRightUpNPagePPage);
    public const int ShiftAltKeyHome = unchecked((int)0x216 + Home);
    public const int ShiftAltKeyEnd = unchecked((int)0x211 + DownEnd);
    public const int AltCtrlKeyNPage = unchecked((int)0x228 + LeftRightUpNPagePPage);
    public const int AltCtrlKeyPPage = unchecked((int)0x22d + LeftRightUpNPagePPage);
    public const int AltCtrlKeyHome = unchecked((int)0x219 + Home);
    public const int AltCtrlKeyEnd = unchecked((int)0x214 + DownEnd);

    // see #949
    public static int LC_ALL { get; private set; }

    static Curses()
    {
        LC_ALL = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.OSX)
            ? 0
            : 6;
    }

    public static int ColorPair(int n) { return 0 + n * 256; }

    //[StructLayout (LayoutKind.Sequential)]
    //public struct winsize {
    //	public ushort ws_row;
    //	public ushort ws_col;
    //	public ushort ws_xpixel;   /* unused */
    //	public ushort ws_ypixel;   /* unused */
    //};

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseEvent
    {
        public short ID;
        public int X, Y, Z;
        public Event ButtonState;
    }

    private static int lines, cols;
    private static Window main_window;
    private static IntPtr curscr_ptr, lines_ptr, cols_ptr;

    // If true, uses the DllImport into "ncurses", otherwise "libncursesw.so.5"
    //static bool use_naked_driver;

    private static UnmanagedLibrary curses_library;
    private static NativeMethods methods;


    [DllImport("libc")]
    public static extern int setlocale(int cate, [MarshalAs(UnmanagedType.LPStr)] string locale);

    //[DllImport ("libc")]
    //public extern static int ioctl (int fd, int cmd, out winsize argp);

    private static void LoadMethods()
    {
        var libs = UnmanagedLibrary.IsMacOS
            ? new string[] { "libncurses.dylib" }
            : new string[] { "libncursesw.so.6", "libncursesw.so.5" };
        curses_library = new UnmanagedLibrary(libs, false);
        methods = new NativeMethods(curses_library);
    }

    private static void FindNCurses()
    {
        LoadMethods();
        stdscr = read_static_ptr("stdscr");
        curscr_ptr = get_ptr("curscr");
        lines_ptr = get_ptr("LINES");
        cols_ptr = get_ptr("COLS");
    }

    public static Window initscr()
    {
        setlocale(LC_ALL, "");
        FindNCurses();

        // Prevents the terminal from being locked after exiting.
        reset_shell_mode();

        main_window = new Window(methods.initscr());
        try
        {
            console_sharp_get_dims(out lines, out cols);
        }
        catch (DllNotFoundException)
        {
            endwin();
            Console.Error.WriteLine(
                "Unable to find the @MONO_CURSES@ native library\n" +
                "this is different than the managed mono-curses.dll\n\n" +
                "Typically you need to install to a LD_LIBRARY_PATH directory\n" +
                "or DYLD_LIBRARY_PATH directory or run /sbin/ldconfig");
            Environment.Exit(1);
        }

        return main_window;
    }

    public static int Lines => lines;

    public static int Cols => cols;

    //
    // Returns true if the window changed since the last invocation, as a
    // side effect, the Lines and Cols properties are updated
    //
    public static bool CheckWinChange()
    {
        int l, c;

        console_sharp_get_dims(out l, out c);

        if (l == 1 || l != lines || c != cols)
        {
            lines = l;
            cols = c;
            //if (l <= 0 || c <= 0) {
            //	Console.Out.Write ($"\x1b[8;50;{c}t");
            //	Console.Out.Flush ();
            //	return false;
            //}
            return true;
        }

        return false;
    }

    public static int addstr(string format, params object[] args)
    {
        var s = string.Format(format, args);
        return addwstr(s);
    }

    private static char[] r = new char[1];

    //
    // Have to wrap the native addch, as it can not
    // display unicode characters, we have to use addstr
    // for that.   but we need addch to render special ACS
    // characters
    //
    public static int addch(int ch)
    {
        if (ch < 127 || ch > 0xffff)
            return methods.addch(ch);
        var c = (char)ch;
        return addwstr(new string(c, 1));
    }

    private static IntPtr stdscr;

    private static IntPtr get_ptr(string key)
    {
        var ptr = curses_library.LoadSymbol(key);

        if (ptr == IntPtr.Zero)
            throw new Exception("Could not load the key " + key);
        return ptr;
    }

    internal static IntPtr read_static_ptr(string key)
    {
        var ptr = get_ptr(key);
        return Marshal.ReadIntPtr(ptr);
    }

    internal static IntPtr console_sharp_get_stdscr() { return stdscr; }

    internal static IntPtr console_sharp_get_curscr() { return Marshal.ReadIntPtr(curscr_ptr); }

    internal static void console_sharp_get_dims(out int lines, out int cols)
    {
        lines = Marshal.ReadInt32(lines_ptr);
        cols = Marshal.ReadInt32(cols_ptr);

        //int cmd;
        //if (UnmanagedLibrary.IsMacOSPlatform) {
        //	cmd = TIOCGWINSZ_MAC;
        //} else {
        //	cmd = TIOCGWINSZ;
        //}

        //if (ioctl (1, cmd, out winsize ws) == 0) {
        //	lines = ws.ws_row;
        //	cols = ws.ws_col;

        //	if (lines == Lines && cols == Cols) {
        //		return;
        //	}

        //	resizeterm (lines, cols);
        //} else {
        //	lines = Lines;
        //	cols = Cols;
        //}
    }

    public static Event mousemask(Event newmask, out Event oldmask)
    {
        IntPtr e;
        var ret = (Event)methods.mousemask((IntPtr)newmask, out e);
        oldmask = (Event)e;
        return ret;
    }

    // We encode ESC + char (what Alt-char generates) as 0x2000 + char
    public const int KeyAlt = 0x2000;

    public static int IsAlt(int key)
    {
        if ((key & KeyAlt) != 0)
            return key & ~KeyAlt;
        return 0;
    }

    public static int StartColor() { return methods.start_color(); }

    public static bool HasColors => methods.has_colors();

    public static int InitColorPair(short pair, short foreground, short background)
    {
        return methods.init_pair(pair, (short)foreground, (short)background);
    }

    public static int UseDefaultColors() { return methods.use_default_colors(); }

    public static int ColorPairs => methods.COLOR_PAIRS();

    //
    // The proxy methods to call into each version
    //
    public static int endwin() { return methods.endwin(); }

    public static bool isendwin() { return methods.isendwin(); }

    public static int cbreak() { return methods.cbreak(); }

    public static int nocbreak() { return methods.nocbreak(); }

    public static int echo() { return methods.echo(); }

    public static int noecho() { return methods.noecho(); }

    public static int halfdelay(int t) { return methods.halfdelay(t); }

    public static int raw() { return methods.raw(); }

    public static int noraw() { return methods.noraw(); }

    public static void noqiflush() { methods.noqiflush(); }

    public static void qiflush() { methods.qiflush(); }

    public static int typeahead(IntPtr fd) { return methods.typeahead(fd); }

    public static int timeout(int delay) { return methods.timeout(delay); }

    public static int wtimeout(IntPtr win, int delay) { return methods.wtimeout(win, delay); }

    public static int notimeout(IntPtr win, bool bf) { return methods.notimeout(win, bf); }

    public static int keypad(IntPtr win, bool bf) { return methods.keypad(win, bf); }

    public static int meta(IntPtr win, bool bf) { return methods.meta(win, bf); }

    public static int intrflush(IntPtr win, bool bf) { return methods.intrflush(win, bf); }

    public static int clearok(IntPtr win, bool bf) { return methods.clearok(win, bf); }

    public static int idlok(IntPtr win, bool bf) { return methods.idlok(win, bf); }

    public static void idcok(IntPtr win, bool bf) { methods.idcok(win, bf); }

    public static void immedok(IntPtr win, bool bf) { methods.immedok(win, bf); }

    public static int leaveok(IntPtr win, bool bf) { return methods.leaveok(win, bf); }

    public static int wsetscrreg(IntPtr win, int top, int bot) { return methods.wsetscrreg(win, top, bot); }

    public static int scrollok(IntPtr win, bool bf) { return methods.scrollok(win, bf); }

    public static int nl() { return methods.nl(); }

    public static int nonl() { return methods.nonl(); }

    public static int setscrreg(int top, int bot) { return methods.setscrreg(top, bot); }

    public static int refresh() { return methods.refresh(); }

    public static int doupdate() { return methods.doupdate(); }

    public static int wrefresh(IntPtr win) { return methods.wrefresh(win); }

    public static int redrawwin(IntPtr win) { return methods.redrawwin(win); }

    //static public int wredrawwin (IntPtr win, int beg_line, int num_lines) => methods.wredrawwin (win, beg_line, num_lines);
    public static int wnoutrefresh(IntPtr win) { return methods.wnoutrefresh(win); }

    public static int move(int line, int col) { return methods.move(line, col); }

    public static int curs_set(int visibility) { return methods.curs_set(visibility); }

    //static public int addch (int ch) => methods.addch (ch);
    public static int addwstr(string s) { return methods.addwstr(s); }

    public static int wmove(IntPtr win, int line, int col) { return methods.wmove(win, line, col); }

    public static int waddch(IntPtr win, int ch) { return methods.waddch(win, ch); }

    public static int attron(int attrs) { return methods.attron(attrs); }

    public static int attroff(int attrs) { return methods.attroff(attrs); }

    public static int attrset(int attrs) { return methods.attrset(attrs); }

    public static int getch() { return methods.getch(); }

    public static int get_wch(out int sequence) { return methods.get_wch(out sequence); }

    public static int ungetch(int ch) { return methods.ungetch(ch); }

    public static int mvgetch(int y, int x) { return methods.mvgetch(y, x); }

    public static bool has_colors() { return methods.has_colors(); }

    public static int start_color() { return methods.start_color(); }

    public static int init_pair(short pair, short f, short b) { return methods.init_pair(pair, f, b); }

    public static int use_default_colors() { return methods.use_default_colors(); }

    public static int COLOR_PAIRS() { return methods.COLOR_PAIRS(); }

    public static uint getmouse(out MouseEvent ev) { return methods.getmouse(out ev); }

    public static uint ungetmouse(ref MouseEvent ev) { return methods.ungetmouse(ref ev); }

    public static int mouseinterval(int interval) { return methods.mouseinterval(interval); }

    public static bool is_term_resized(int lines, int columns) { return methods.is_term_resized(lines, columns); }

    public static int resize_term(int lines, int columns) { return methods.resize_term(lines, columns); }

    public static int resizeterm(int lines, int columns) { return methods.resizeterm(lines, columns); }

    public static void use_env(bool f) { methods.use_env(f); }

    public static int flushinp() { return methods.flushinp(); }

    public static int def_prog_mode() { return methods.def_prog_mode(); }

    public static int def_shell_mode() { return methods.def_shell_mode(); }

    public static int reset_prog_mode() { return methods.reset_prog_mode(); }

    public static int reset_shell_mode() { return methods.reset_shell_mode(); }

    public static int savetty() { return methods.savetty(); }

    public static int resetty() { return methods.resetty(); }

    public class Window
    {
        public readonly IntPtr Handle;
        private static Window curscr;
        private static Window stdscr;

        static Window()
        {
            initscr();
            stdscr = new Window(console_sharp_get_stdscr());
            curscr = new Window(console_sharp_get_curscr());
        }

        internal Window(IntPtr handle) { Handle = handle; }

        public static Window Standard => stdscr;

        public static Window Current => curscr;


        public int wtimeout(int delay) { return Curses.wtimeout(Handle, delay); }

        public int notimeout(bool bf) { return Curses.notimeout(Handle, bf); }

        public int keypad(bool bf) { return Curses.keypad(Handle, bf); }

        public int meta(bool bf) { return Curses.meta(Handle, bf); }

        public int intrflush(bool bf) { return Curses.intrflush(Handle, bf); }

        public int clearok(bool bf) { return Curses.clearok(Handle, bf); }

        public int idlok(bool bf) { return Curses.idlok(Handle, bf); }

        public void idcok(bool bf) { Curses.idcok(Handle, bf); }

        public void immedok(bool bf) { Curses.immedok(Handle, bf); }

        public int leaveok(bool bf) { return Curses.leaveok(Handle, bf); }

        public int setscrreg(int top, int bot) { return wsetscrreg(Handle, top, bot); }

        public int scrollok(bool bf) { return Curses.scrollok(Handle, bf); }

        public int wrefresh() { return Curses.wrefresh(Handle); }

        public int redrawwin() { return Curses.redrawwin(Handle); }

#if false
			public int wredrawwin (int beg_line, int num_lines)
			{
				return Curses.wredrawwin (Handle, beg_line, num_lines);
			}
#endif
        public int wnoutrefresh() { return Curses.wnoutrefresh(Handle); }

        public int move(int line, int col) { return wmove(Handle, line, col); }

        public int addch(char ch) { return waddch(Handle, ch); }

        public int refresh() { return Curses.wrefresh(Handle); }
    }

    // Currently unused, to do later
    internal class Screen
    {
        public readonly IntPtr Handle;

        internal Screen(IntPtr handle) { Handle = handle; }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}
