using System.Runtime.InteropServices;

namespace Asciis.App.Platforms;

internal static class Unix
{

    public static string BashRun(string commandLine, bool output = true, string inputText = "", bool runCurses = true)
    {
        var arguments = $"-c \"{commandLine}\"";

        if (output)
        {
            var errorBuilder  = new StringBuilder();
            var outputBuilder = new StringBuilder();

            using var process = new System.Diagnostics.Process
                                {
                                    StartInfo = new System.Diagnostics.ProcessStartInfo
                                                {
                                                    FileName               = "bash",
                                                    Arguments              = arguments,
                                                    RedirectStandardOutput = true,
                                                    RedirectStandardError  = true,
                                                    UseShellExecute        = false,
                                                    CreateNoWindow         = false
                                                }
                                };
            process.Start();
            process.OutputDataReceived += (_, args) => outputBuilder.AppendLine(args.Data);
            process.BeginOutputReadLine();
            process.ErrorDataReceived += (_, args) => errorBuilder.AppendLine(args.Data);
            process.BeginErrorReadLine();
            if (!process.DoubleWaitForExit())
            {
                var timeoutError = $@"Process timed out. Command line: bash {arguments}.
							Output: {outputBuilder}
							Error: {errorBuilder}";
                throw new Exception(timeoutError);
            }

            if (process.ExitCode == 0)
            {
                if (runCurses)
                {
                    Curses.Raw();
                    Curses.NoEcho();
                }

                return outputBuilder.ToString();
            }

            var error = $@"Could not execute process. Command line: bash {arguments}.
						Output: {outputBuilder}
						Error: {errorBuilder}";
            throw new Exception(error);
        }
        else
        {
            using var process = new System.Diagnostics.Process
                                {
                                    StartInfo = new System.Diagnostics.ProcessStartInfo
                                                {
                                                    FileName              = "bash",
                                                    Arguments             = arguments,
                                                    RedirectStandardInput = true,
                                                    UseShellExecute       = false,
                                                    CreateNoWindow        = false
                                                }
                                };
            process.Start();
            process.StandardInput.Write(inputText);
            process.StandardInput.Close();
            process.WaitForExit();
            if (runCurses)
            {
                Curses.Raw();
                Curses.NoEcho();
            }

            return inputText;
        }
    }

    public static bool DoubleWaitForExit(this System.Diagnostics.Process process)
    {
        var result = process.WaitForExit(500);
        if (result) process.WaitForExit();
        return result;
    }

    public static bool FileExists(this string value)
    {
        return !string.IsNullOrEmpty(value) && !value.Contains("not found");
    }


    // ReSharper disable StringLiteralTypo
    // ReSharper disable InconsistentNaming
    // ReSharper disable IdentifierTypo

    internal static class Delegates
    {
        public delegate IntPtr initscr();
        public delegate int endwin();
        public delegate bool isendwin();
        public delegate int cbreak();
        public delegate int nocbreak();
        public delegate int echo();
        public delegate int noecho();
        public delegate int halfdelay(int t);
        public delegate int raw();
        public delegate int noraw();
        public delegate void noqiflush();
        public delegate void qiflush();
        public delegate int typeahead(IntPtr fd);
        public delegate int timeout(int delay);
        public delegate int wtimeout(IntPtr win, int delay);
        public delegate int notimeout(IntPtr win, bool bf);
        public delegate int keypad(IntPtr win, bool bf);
        public delegate int meta(IntPtr win, bool bf);
        public delegate int intrflush(IntPtr win, bool bf);
        public delegate int clearok(IntPtr win, bool bf);
        public delegate int idlok(IntPtr win, bool bf);
        public delegate void idcok(IntPtr win, bool bf);
        public delegate void immedok(IntPtr win, bool bf);
        public delegate int leaveok(IntPtr win, bool bf);
        public delegate int wsetscrreg(IntPtr win, int top, int bot);
        public delegate int scrollok(IntPtr win, bool bf);
        public delegate int nl();
        public delegate int nonl();
        public delegate int setscrreg(int top, int bot);
        public delegate int refresh();
        public delegate int doupdate();
        public delegate int wrefresh(IntPtr win);
        public delegate int redrawwin(IntPtr win);
        public delegate int wnoutrefresh(IntPtr win);
        public delegate int move(int line, int col);
        public delegate int curs_set(int visibility);
        public delegate int addch(int ch);
        public delegate int addwstr([MarshalAs(UnmanagedType.LPWStr)] string s);
        public delegate int wmove(IntPtr win, int line, int col);
        public delegate int waddch(IntPtr win, int ch);
        public delegate int attron(int attrs);
        public delegate int attroff(int attrs);
        public delegate int attrset(int attrs);
        public delegate int getch();
        public delegate int get_wch(out int sequence);
        public delegate int ungetch(int ch);
        public delegate int mvgetch(int y, int x);
        public delegate bool has_colors();
        public delegate int start_color();
        public delegate int init_pair(short pair, short f, short b);
        public delegate int use_default_colors();
        public delegate int COLOR_PAIRS();
        public delegate uint getmouse(out Curses.MouseEvent ev);
        public delegate uint ungetmouse(ref Curses.MouseEvent ev);
        public delegate int mouseinterval(int interval);
        public delegate IntPtr mousemask(IntPtr newmask, out IntPtr oldMask);
        public delegate bool is_term_resized(int lines, int columns);
        public delegate int resize_term(int lines, int columns);
        public delegate int resizeterm(int lines, int columns);
        public delegate void use_env(bool f);
        public delegate int flushinp();
        public delegate int def_prog_mode();
        public delegate int def_shell_mode();
        public delegate int reset_prog_mode();
        public delegate int reset_shell_mode();
        public delegate int savetty();
        public delegate int resetty();
    }

    internal class NativeMethods
    {
        public          Delegates.initscr            initscr            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.initscr>("initscr");
        public          Delegates.endwin             endwin             => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.endwin>("endwin");
        public          Delegates.isendwin           isendwin           => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.isendwin>("isendwin");
        public          Delegates.cbreak             cbreak             => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.cbreak>("cbreak");
        public          Delegates.nocbreak           nocbreak           => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.nocbreak>("nocbreak");
        public          Delegates.echo               echo               => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.echo>("echo");
        public          Delegates.noecho             noecho             => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.noecho>("noecho");
        public          Delegates.halfdelay          halfdelay          => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.halfdelay>("halfdelay");
        public          Delegates.raw                raw                => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.raw>("raw");
        public          Delegates.noraw              noraw              => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.noraw>("noraw");
        public          Delegates.noqiflush          noqiflush          => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.noqiflush>("noqiflush");
        public          Delegates.qiflush            qiflush            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.qiflush>("qiflush");
        public          Delegates.typeahead          typeahead          => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.typeahead>("typeahead");
        public          Delegates.timeout            timeout            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.timeout>("timeout");
        public          Delegates.wtimeout           wtimeout           => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.wtimeout>("wtimeout");
        public          Delegates.notimeout          notimeout          => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.notimeout>("notimeout");
        public          Delegates.keypad             keypad             => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.keypad>("keypad");
        public          Delegates.meta               meta               => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.meta>("meta");
        public          Delegates.intrflush          intrflush          => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.intrflush>("intrflush");
        public          Delegates.clearok            clearok            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.clearok>("clearok");
        public          Delegates.idlok              idlok              => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.idlok>("idlok");
        public          Delegates.idcok              idcok              => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.idcok>("idcok");
        public          Delegates.immedok            immedok            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.immedok>("immedok");
        public          Delegates.leaveok            leaveok            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.leaveok>("leaveok");
        public          Delegates.wsetscrreg         wsetscrreg         => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.wsetscrreg>("wsetscrreg");
        public          Delegates.scrollok           scrollok           => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.scrollok>("scrollok");
        public          Delegates.nl                 nl                 => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.nl>("nl");
        public          Delegates.nonl               nonl               => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.nonl>("nonl");
        public          Delegates.setscrreg          setscrreg          => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.setscrreg>("setscrreg");
        public          Delegates.refresh            refresh            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.refresh>("refresh");
        public          Delegates.doupdate           doupdate           => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.doupdate>("doupdate");
        public          Delegates.wrefresh           wrefresh           => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.wrefresh>("wrefresh");
        public          Delegates.redrawwin          redrawwin          => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.redrawwin>("redrawwin");
        public          Delegates.wnoutrefresh       wnoutrefresh       => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.wnoutrefresh>("wnoutrefresh");
        public          Delegates.move               move               => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.move>("move");
        public          Delegates.curs_set           curs_set           => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.curs_set>("curs_set");
        public          Delegates.addch              addch              => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.addch>("addch");
        public          Delegates.addwstr            addwstr            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.addwstr>("addwstr");
        public          Delegates.wmove              wmove              => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.wmove>("wmove");
        public          Delegates.waddch             waddch             => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.waddch>("waddch");
        public          Delegates.attron             attron             => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.attron>("attron");
        public          Delegates.attroff            attroff            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.attroff>("attroff");
        public          Delegates.attrset            attrset            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.attrset>("attrset");
        public          Delegates.getch              getch              => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.getch>("getch");
        public          Delegates.get_wch            get_wch            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.get_wch>("get_wch");
        public          Delegates.ungetch            ungetch            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.ungetch>("ungetch");
        public          Delegates.mvgetch            mvgetch            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.mvgetch>("mvgetch");
        public          Delegates.has_colors         has_colors         => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.has_colors>("has_colors");
        public          Delegates.start_color        start_color        => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.start_color>("start_color");
        public          Delegates.init_pair          init_pair          => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.init_pair>("init_pair");
        public          Delegates.use_default_colors use_default_colors => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.use_default_colors>("use_default_colors");
        public          Delegates.COLOR_PAIRS        COLOR_PAIRS        => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.COLOR_PAIRS>("COLOR_PAIRS");
        public          Delegates.getmouse           getmouse           => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.getmouse>("getmouse");
        public          Delegates.ungetmouse         ungetmouse         => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.ungetmouse>("ungetmouse");
        public          Delegates.mouseinterval      mouseinterval      => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.mouseinterval>("mouseinterval");
        public          Delegates.mousemask          mousemask          => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.mousemask>("mousemask");
        public          Delegates.is_term_resized    is_term_resized    => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.is_term_resized>("is_term_resized");
        public          Delegates.resize_term        resize_term        => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.resize_term>("resize_term");
        public          Delegates.resizeterm         resizeterm         => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.resizeterm>("resizeterm");
        public          Delegates.use_env            use_env            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.use_env>("use_env");
        public          Delegates.flushinp           flushinp           => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.flushinp>("flushinp");
        public          Delegates.def_prog_mode      def_prog_mode      => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.def_prog_mode>("def_prog_mode");
        public          Delegates.def_shell_mode     def_shell_mode     => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.def_shell_mode>("def_shell_mode");
        public          Delegates.reset_prog_mode    reset_prog_mode    => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.reset_prog_mode>("reset_prog_mode");
        public          Delegates.reset_shell_mode   reset_shell_mode   => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.reset_shell_mode>("reset_shell_mode");
        public          Delegates.savetty            savetty            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.savetty>("savetty");
        public          Delegates.resetty            resetty            => UnmanagedLibrary.GetNativeMethodDelegate<Delegates.resetty>("resetty");
        public readonly UnmanagedLibrary             UnmanagedLibrary;

        public NativeMethods(UnmanagedLibrary lib)
        {
            UnmanagedLibrary   = lib;
        }
    }
    // ReSharper restore InconsistentNaming
    // ReSharper restore IdentifierTypo
    // ReSharper restore StringLiteralTypo

}