using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

// ReSharper disable InconsistentNaming

namespace Asciis.UI.Terminals;

public class Capabilities
{
    private static Capabilities? _instance;
    public static Capabilities Instance =>
        _instance ??= new Capabilities();

    public bool IsWindows { get; }
    public bool IsLinux { get; }
    public bool IsMacOS { get; }
    public bool IsMono { get; }
    public bool IsDotNetCore { get; }
    public bool IsXamarinIOS { get; }
    public bool IsXamarinAndroid { get; }
    public bool IsXamarin =>
        IsXamarinAndroid || IsXamarinIOS;
    public bool Is64Bit { get; }

    private bool? _supportsAnsi;

    public Capabilities()
    {
        const string monoRuntimeClassName = "Mono.Runtime";
        const string mathFClassName = "System.MathF";
        const string xamarinIosClassName = "Foundation.NSObject, Xamarin.iOS";
        const string xamarinAndroidClassName = "Java.Lang.Object, Mono.Android";

        var platform = Environment.OSVersion.Platform;

        IsMacOS = platform == PlatformID.Unix && GetUname() == "Darwin";
        IsLinux = platform == PlatformID.Unix && !IsMacOS;
        IsWindows = platform == PlatformID.Win32NT || platform == PlatformID.Win32S || platform == PlatformID.Win32Windows;
        IsMono = Type.GetType(monoRuntimeClassName) != null;
        if (!IsMono)
            IsDotNetCore = Type.GetType(mathFClassName) != null;
        IsXamarinIOS = Type.GetType(xamarinIosClassName) != null;
        IsXamarinAndroid = Type.GetType(xamarinAndroidClassName) != null;
        Is64Bit = Marshal.SizeOf(typeof(IntPtr)) == 8;
    }

    [DllImport("libc")]
    static extern int uname(IntPtr buf);

    static string GetUname()
    {
        var buffer = Marshal.AllocHGlobal(8192);
        try
        {
            if (uname(buffer) == 0)
                return Marshal.PtrToStringAnsi(buffer) ?? string.Empty;

            return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
        finally
        {
            if (buffer != IntPtr.Zero)
                Marshal.FreeHGlobal(buffer);
        }
    }

    private bool? _supportsTrueColor;
    public bool SupportsTrueColor
    {
        get
        {
            if (_supportsTrueColor.HasValue)
                return _supportsTrueColor.Value;

            if (IsWindows)
            {
                _supportsTrueColor = false;
                var regex = new Regex("^Microsoft Windows (?'major'[0-9]*).(?'minor'[0-9]*).(?'build'[0-9]*)\\s*$");
                var match = regex.Match(RuntimeInformation.OSDescription);
                if (match.Success && int.TryParse(match.Groups["major"].Value, out var major))
                {
                    if (major > 10)
                        _supportsTrueColor = true;
                    else if (major == 10 && int.TryParse(match.Groups["build"].Value, out var build) && build >= 15063)
                        _supportsTrueColor = true;
                }

                return _supportsTrueColor.Value;
            }

            var envVars = Environment.GetEnvironmentVariables();

            if (envVars.Contains("NO_COLOR"))
                _supportsTrueColor = false;

            if (envVars.Contains("COLORTERM"))
            {
                var colorTerm = envVars["COLORTERM"] as string;
                if (!string.IsNullOrWhiteSpace(colorTerm))
                {
                    if (colorTerm.Equals("truecolor", StringComparison.OrdinalIgnoreCase) ||
                        colorTerm.Equals("24bit", StringComparison.OrdinalIgnoreCase))
                    {
                        _supportsTrueColor = true;

                        return true;
                    }
                }
            }
            _supportsTrueColor = false;
            return false;
        }
    }

    public bool SupportsAnsi
    {
        get
        {
            if (_supportsAnsi.HasValue)
                return _supportsAnsi.Value;

            _supportsAnsi = false;

            // Github action doesn't setup a correct PTY but supports ANSI.
            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GITHUB_ACTION")))
                _supportsAnsi = true;

            if (IsWindows)
            {
                var conEmu = Environment.GetEnvironmentVariable("ConEmuANSI");

                if (!string.IsNullOrEmpty(conEmu) && conEmu.Equals("On", StringComparison.OrdinalIgnoreCase))
                    _supportsAnsi = true;

#if _WINDOWS
                var hOutput = Windows.GetStdHandle(StandardHandle.Output);
                if (Windows.GetConsoleOutputMode(hOutput, out var mode))
                {
                    if (mode.HasFlag(ConsoleOutputModes.EnableVirtualTerminalProcessing))
                        _supportsAnsi = true;

                    mode |= ConsoleOutputModes.DisableNewlineAutoReturn | ConsoleOutputModes.EnableVirtualTerminalProcessing;
                    if (Windows.SetConsoleOutputMode(hOutput, mode))
                        _supportsAnsi = true;
                }
#endif
                return _supportsAnsi.Value;
            }

            // Check if the terminal is of type ANSI/VT100/xterm compatible.
            var term = Environment.GetEnvironmentVariable("TERM");
            if (!string.IsNullOrWhiteSpace(term))
            {
                Regex[] regexes =
                {
                        new Regex( "^xterm" ),  // xterm, PuTTY, Mintty
                        new Regex( "^rxvt" ),   // RXVT
                        new Regex( "^eterm" ),  // Eterm
                        new Regex( "^screen" ), // GNU screen, tmux
                        new Regex( "^vt100" ),  // DEC VT series
                        new Regex( "^vt102" ),  // DEC VT series
                        new Regex( "^vt220" ),  // DEC VT series
                        new Regex( "^vt320" ),  // DEC VT series
                        new Regex( "ansi" ),    // ANSI
                        new Regex( "bvterm" ),  // Bitvise SSH Client
                        new Regex( "cygwin" ),  // Cygwin, MinGW
                        new Regex( "konsole" ), // Konsole
                        new Regex( "linux" ),   // Linux console
                        new Regex( "scoansi" ), // SCO ANSI
                        new Regex( "tmux" ),    // tmux
                    };

                if (regexes.Any(regex => regex.IsMatch(term)))
                    _supportsAnsi = true;
            }

            return _supportsAnsi.Value;
        }
    }
}
