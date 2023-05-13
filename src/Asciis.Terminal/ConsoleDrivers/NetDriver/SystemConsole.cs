using System.Runtime.InteropServices;

namespace Asciis.Terminal.ConsoleDrivers;

internal class SystemConsole
{
    private IntPtr InputHandle, OutputHandle, ErrorHandle;
    private uint originalInputConsoleMode, originalOutputConsoleMode, originalErrorConsoleMode;

    public SystemConsole()
    {
        InputHandle = GetStdHandle(STD_INPUT_HANDLE);
        if (!GetConsoleMode(InputHandle, out var mode))
            throw new ApplicationException($"Failed to get input console mode, error code: {GetLastError()}.");
        originalInputConsoleMode = mode;
        if ((mode & ENABLE_VIRTUAL_TERMINAL_INPUT) < ENABLE_VIRTUAL_TERMINAL_INPUT)
        {
            mode |= ENABLE_VIRTUAL_TERMINAL_INPUT;
            if (!SetConsoleMode(InputHandle, mode))
                throw new ApplicationException($"Failed to set input console mode, error code: {GetLastError()}.");
        }

        OutputHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        if (!GetConsoleMode(OutputHandle, out mode))
            throw new ApplicationException($"Failed to get output console mode, error code: {GetLastError()}.");
        originalOutputConsoleMode = mode;
        if ((mode & (ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN)) <
            DISABLE_NEWLINE_AUTO_RETURN)
        {
            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if (!SetConsoleMode(OutputHandle, mode))
                throw new ApplicationException($"Failed to set output console mode, error code: {GetLastError()}.");
        }

        ErrorHandle = GetStdHandle(STD_ERROR_HANDLE);
        if (!GetConsoleMode(ErrorHandle, out mode))
            throw new ApplicationException($"Failed to get error console mode, error code: {GetLastError()}.");
        originalErrorConsoleMode = mode;
        if ((mode & DISABLE_NEWLINE_AUTO_RETURN) < DISABLE_NEWLINE_AUTO_RETURN)
        {
            mode |= DISABLE_NEWLINE_AUTO_RETURN;
            if (!SetConsoleMode(ErrorHandle, mode))
                throw new ApplicationException($"Failed to set error console mode, error code: {GetLastError()}.");
        }
    }

    public void Cleanup()
    {
        if (!SetConsoleMode(InputHandle, originalInputConsoleMode))
            throw new ApplicationException($"Failed to restore input console mode, error code: {GetLastError()}.");
        if (!SetConsoleMode(OutputHandle, originalOutputConsoleMode))
            throw new ApplicationException($"Failed to restore output console mode, error code: {GetLastError()}.");
        if (!SetConsoleMode(ErrorHandle, originalErrorConsoleMode))
            throw new ApplicationException($"Failed to restore error console mode, error code: {GetLastError()}.");
    }

    private const int STD_INPUT_HANDLE = -10;
    private const int STD_OUTPUT_HANDLE = -11;
    private const int STD_ERROR_HANDLE = -12;

    // Input modes.
    private const uint ENABLE_PROCESSED_INPUT = 1;
    private const uint ENABLE_LINE_INPUT = 2;
    private const uint ENABLE_ECHO_INPUT = 4;
    private const uint ENABLE_WINDOW_INPUT = 8;
    private const uint ENABLE_MOUSE_INPUT = 16;
    private const uint ENABLE_INSERT_MODE = 32;
    private const uint ENABLE_QUICK_EDIT_MODE = 64;
    private const uint ENABLE_EXTENDED_FLAGS = 128;
    private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 512;

    // Output modes.
    private const uint ENABLE_PROCESSED_OUTPUT = 1;
    private const uint ENABLE_WRAP_AT_EOL_OUTPUT = 2;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;
    private const uint DISABLE_NEWLINE_AUTO_RETURN = 8;
    private const uint ENABLE_LVB_GRID_WORLDWIDE = 10;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [DllImport("kernel32.dll")]
    private static extern uint GetLastError();
}
