using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using No8.Ascii.Platforms;
using No8.Ascii.VirtualTerminal;

namespace No8.Ascii;

using static Windows;
using static Windows.Kernel32;

public sealed class ConsoleDriverWindows : ConsoleDriver
{
    internal readonly IntPtr            InputHandle, OutputHandle, ErrorHandle;

    private readonly Encoding           _originalOutputEncoding;
    private readonly ConsoleInputModes  _originalConsoleInputMode;
    private readonly ConsoleOutputModes _originalConsoleOutputMode;
    private readonly ConsoleOutputModes _originalConsoleErrorMode;
    
    public ConsoleDriverWindows()
    {
        Clipboard      = new ClipboardWindows();
        
        /*
        Trace       => May be sensitive.  Application internal
        Debug       => useful while debugging
        Information => Flow, long term value
        Warning     => Abnormal, unexpected
        Error       => Failure in current activity. Recoverable
        Critical    => Unrecoverable, crash
        None,
        */
        
        InputHandle    = GetStdHandle(StandardHandle.Input);
        OutputHandle   = GetStdHandle(StandardHandle.Output);
        ErrorHandle    = GetStdHandle(StandardHandle.Error);

        // Console Input Mode
        _originalConsoleInputMode = ConsoleInputMode;

        var newConsoleInputMode =
            _originalConsoleInputMode |
            ConsoleInputModes.EnableMouseInput |
            ConsoleInputModes.EnableExtendedFlags;
        newConsoleInputMode &= ~ConsoleInputModes.EnableQuickEditMode;
        ConsoleInputMode =  newConsoleInputMode;

        // Console Output Mode
        _originalConsoleOutputMode = ConsoleOutputMode;
        var newConsoleOutputMode =
            _originalConsoleOutputMode |
            ConsoleOutputModes.EnableVirtualTerminalProcessing |
            ConsoleOutputModes.DisableNewlineAutoReturn |
            ConsoleOutputModes.EnableLvbGridWorldwide;
        ConsoleOutputMode    =  newConsoleOutputMode;
        
        // Console Error Mode
        _originalConsoleErrorMode = ConsoleErrorMode;
        var newConsoleErrorMode =
            _originalConsoleErrorMode |
            ConsoleOutputModes.DisableNewlineAutoReturn;
        ConsoleErrorMode    =  newConsoleErrorMode;

        // Output Encoding
        _originalOutputEncoding = System.Console.OutputEncoding;
        System.Console.OutputEncoding = Encoding.UTF8;     // Default Output encoding so can support extended characters and emoji
    }

    private void Shutdown()
    {
        System.Console.OutputEncoding    = _originalOutputEncoding;
        ConsoleInputMode  = _originalConsoleInputMode;
        ConsoleOutputMode = _originalConsoleOutputMode;
        ConsoleErrorMode  = _originalConsoleErrorMode;

        System.Console.Out.Write(TerminalSeq.Mode.ScreenNormal);
    }

    ~ConsoleDriverWindows()
    {
        Shutdown();
    }

    public ConsoleInputModes ConsoleInputMode
    {
        get
        {
            GetConsoleInputMode(InputHandle, out var mode);
            return mode;
        }
        set => SetConsoleInputMode(InputHandle, value);
    }
    public ConsoleOutputModes ConsoleOutputMode
    {
        get
        {
            GetConsoleOutputMode(OutputHandle, out var mode);
            return mode;
        }
        set => SetConsoleOutputMode(OutputHandle, value);
    }
    public ConsoleOutputModes ConsoleErrorMode
    {
        get
        {
            GetConsoleOutputMode(ErrorHandle, out var mode);
            return mode;
        }
        set => SetConsoleOutputMode(ErrorHandle, value);
    }
}

