namespace No8.Ascii.VirtualTerminal;

using System;
using System.Diagnostics;
using System.Text;

public static class Conn
{
    public static event EventHandler<ConsoleKeyInfo>? KeyAvailable;
    public static event EventHandler<string>? SequenceAvailable;

    public static bool Alive
    {
        get => _alive;
    }

    public static void Stop()
    {
        Trace.TraceInformation("Con.Stop!");
        _exitEvent.Reset();
        _alive = false;

        // Wait for Input monitor thread to exit
        _exitEvent.Wait(500);
    }

    private static bool _alive;
    private static readonly ManualResetEventSlim _exitEvent = new(false);

    static Conn()
    {
        Console.OutputEncoding =  Encoding.UTF8;
        Console.CancelKeyPress += new ConsoleCancelEventHandler(ConsoleCancelEventHandler);
        _alive                 =  true;
        
        Task.Run(MonitorInput);
    }

    private static void ConsoleCancelEventHandler(object? sender, ConsoleCancelEventArgs e) =>
        Stop();

    private static void MonitorInput()
    {
        Send("\x1b[?1006h"); // SGR mouse mode
        Send("\x1b[?1003h"); // all mouse
        //Send("\x1b[?1001h"); // highlight mouse
        Send("\x1b[?1004h"); // Focus
        
        while (_alive)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo? key = null;
                try
                {
                    key = Console.ReadKey(true);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception reading key: " + e);
                    break;
                }
                if (key != null)
                    AddKey(key.Value);
            }
        }
        
        // Leave
        try
        {
            Send("\x1b[?1004l"); // Focus
            Send("\x1b[?1003l"); // all mouse
            //Send("\x1b[?1001l"); // highlight mouse
            Send("\x1b[?1006l"); // SGR mouse mode
        }
        catch
        {
            // ignored - should work, but sometimes..
        }

        _exitEvent.Set();
    }

    /// Escape Sequence
    ///     ESC                 0x1b
    ///     Intermediate        0x20..0x2F  <SP>!"#$%&'()*+,-./
    ///     Final               0x30..0x7E  0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
    /// Control Sequence
    ///     CSI                 0x1b [
    ///     P...P (up to 16)    0x30..0x3F  0123456789:;<=>?
    ///     I...I (0 or more)   0x20..0x2F  <SP>!"#$%&'()*+,-./
    ///     Final (1)           0x40..0x7E  @ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
    /// Device Control Strings
    ///     DCS                 0x1b P
    ///     P...P (up to 16)    0x30..0x3F  0123456789:;<=>?
    ///     I...I (0 or more)   0x20..0x2F  <SP>!"#$%&'()*+,-./
    ///     Final (1)           0x40..0x7E  @ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
    ///     Data String         *****
    ///     String Term         0x1b \
    /// Parameters are always unsigned decimal numbers, separated by ;
    /// CAN (0x18) Cancel can cancel a sequence. Indicates an error
    /// SUB (0x1a) Cancel a sequence in progress
    private static void AddKey(ConsoleKeyInfo key)
    {
        Trace.TraceInformation($"IN: {key.Key}, {key.KeyChar}"
                      + (key.Modifiers.HasFlag(ConsoleModifiers.Alt) ? ":Alt" : "")
                      + (key.Modifiers.HasFlag(ConsoleModifiers.Control) ? ":Ctrl" : "")
                      + (key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? ":Shift" : ""));

        var ch = key.KeyChar;
        if (ch == (char)ControlChar.ESC && !_escapeSequence)
        {
            _escapeSequence = true;
            Sequence.Clear();
            Sequence.Append(ch);
            return;
        }

        if (_escapeSequence)
        {
            Sequence.Append(ch);
            if (Sequence.Length == 2)
            {
                switch (ch)
                {
                case 'I':   // In Focus
                    RaiseSequenceAvailable();
                    break;
                case 'O':   // Out of focus
                    RaiseSequenceAvailable();
                    break;
                
                case 'P':  // DCS   Device control string
                    _deviceControlString = true;
                    break;
                case 'X': // SOS  Start of String
                    _startOfString = true;
                    break;
                case '[': // CSI  Control Sequence Introducer
                    _controlSequenceIntroducer = true;
                    break;
                case '\\': // ST   String Terminator
                    StringTerminator();
                    break;
                case ']': // OSC  Operating System Command
                    _operatingSystemCommand = true;
                    break;
                case '^': // PM  Privacy Message
                    _privacyMessage = true;
                    break;
                case '_': // APC    Application Program Command
                    _applicationProgramCommand = true;
                    break;
                }
            }
            else
            {
                if (IsFinal(ch))
                {
                    RaiseSequenceAvailable();
                }
            }

            return;
        }

        RaiseKeyAvailable(key);
    }

    private static string CloseSequence()
    {
        string value = null!;
        lock (SequenceLock)
        {
            value = Sequence.ToString();
            Sequence.Clear();
            _escapeSequence            = false;
            _applicationProgramCommand = false;
            _controlSequenceIntroducer = false;
            _deviceControlString       = false;
            _operatingSystemCommand    = false;
            _privacyMessage            = false;
            _startOfString             = false;
        }

        return value;
    }

    
    private static readonly object SequenceLock = new();
    private static bool _escapeSequence;
#pragma warning disable CS0414
    private static bool _applicationProgramCommand; // APC
    private static bool _controlSequenceIntroducer; // CSI
    private static bool _deviceControlString;       // DCS
    private static bool _startOfString;             // SOS
    private static bool _operatingSystemCommand;    // OSC
    private static bool _privacyMessage;            // PM
#pragma warning restore CS0414
    private static readonly StringBuilder Sequence = new();

    internal static bool IsIntermediate(this char ch) => ch >= 0x20 && ch <= 0x2F;
    internal static bool IsParameter(this char ch) => ch >= 0x30 && ch <= 0x3F;
    internal static bool IsFinal(this char ch) => ch >= 0x40 && ch <= 0x7E;

    private static void StringTerminator()
    {
        var value = CloseSequence();
    }

    private static void RaiseKeyAvailable(ConsoleKeyInfo e) => KeyAvailable?.Invoke(null, e);

    private static void RaiseSequenceAvailable()
    {
        var value = CloseSequence();
        SequenceAvailable?.Invoke(null, value);
    }

    public static void Send(string value)
    {
        if (!_alive)
            return;
        
        #if TRACE
        var sb = new StringBuilder("Send: ");
        foreach (var ch in value)
        {
            if (ch < ' ')
                sb.Append($"<{(ControlChar)ch}>");
            else
                sb.Append(ch);
        }
        Trace.TraceInformation(sb.ToString());
        #endif
            
        Console.Write(value);
    }
}
