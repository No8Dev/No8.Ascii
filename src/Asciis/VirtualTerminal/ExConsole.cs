using System.Runtime.InteropServices.JavaScript;

namespace No8.Ascii.VirtualTerminal;

using System;
using System.Diagnostics;
using System.Text;

/// <summary>
///     Extended Console
/// </summary>
public sealed class ExConsole : IDisposable
{
    public class Options
    {
        public bool StartFullScreen { get; set; } = true;
        public bool StopOnControlC { get; set; } = true;
    }

    public static ExConsole Create(Action<Options>? configure = null)
    {
        Options options = new();
        configure?.Invoke(options);
        
        return new ExConsole(options);
    }

    public Task Run()
    {
        if (Alive && RunningTask != null)
            return RunningTask;
        
        if (_options.StartFullScreen)
            FullScreen();

        RunningTask = Task.Run(MonitorInput); 
        RunningTask.ConfigureAwait(true);
        return RunningTask;
    }
    
    public bool Alive { get; private set; } = true;
    private Task? RunningTask { get; set; }

    
    public event EventHandler<ConsoleKeyInfo>? KeyAvailable;
    public event EventHandler<string>? SequenceAvailable;
    
    private readonly ManualResetEventSlim _exitEvent = new(false);
    private readonly Options _options;
    private bool _fullScreen;
    private readonly bool _stopOnControlC;

    private ExConsole(Options options)
    {
        _options = options;
        
        Console.CancelKeyPress += new ConsoleCancelEventHandler(ConsoleCancelEventHandler);
        _stopOnControlC = options.StopOnControlC;
    }

    public void FullScreen()
    {
        if (_fullScreen)
            return;
        _fullScreen = true;
        Terminal.Screen.ScreenAlt();
    }

    public void NormalScreen()
    {
        if (_fullScreen)
            Terminal.Screen.ScreenNormal();
        _fullScreen = false;
    }

    public void Stop()
    {
        #if _TRACECONN
            Trace.TraceInformation("Conn.Stop!");
        #endif

        if (!Alive)
            return;
        
        _exitEvent.Reset();
        Alive = false;

        // Wait for Input monitor thread to exit
        _exitEvent.Wait(500);
    }


    private void ConsoleCancelEventHandler(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = false;
        if (_stopOnControlC)
            Stop();
    }

    private void MonitorInput()
    {
        var oldOutputEncoding = Console.OutputEncoding;
        Console.OutputEncoding = Encoding.UTF8;
        
        if (_options.StartFullScreen)
            FullScreen();

        Terminal.Mouse.TrackingStart();
        Terminal.Mouse.HighlightEnable();
        
        while (Alive)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo? key = null;
                try
                {
                    key = Console.ReadKey(true);
                    if (key != null)
                        AddKey(key.Value);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception reading key: " + e);
                    break;
                }
            }
        }
        
        // Leave
        try
        {
            //Terminal.Mouse.TrackingStop();
            //Terminal.Mouse.HighlightDisable();

            // only want to restore normal screen if entered Alt screen mode
            NormalScreen();
            Terminal.Special.SoftReset();
        }
        catch
        {
            // ignored - should work, but sometimes..
            Console.OutputEncoding = oldOutputEncoding;
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
    private void AddKey(ConsoleKeyInfo key)
    {
        #if _TRACECONN
            Trace.TraceInformation($"IN: {key.Key}, {key.KeyChar}"
                      + (key.Modifiers.HasFlag(ConsoleModifiers.Alt) ? ":Alt" : "")
                      + (key.Modifiers.HasFlag(ConsoleModifiers.Control) ? ":Ctrl" : "")
                      + (key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? ":Shift" : ""));
        #endif

        var ch = key.KeyChar;
        if (ch == '\x1b' && !_escapeSequence)   // If ESC character
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
                if (ConSeq.IsFinal(ch))
                {
                    RaiseSequenceAvailable();
                }
            }

            return;
        }

        RaiseKeyAvailable(key);
    }

    private string CloseSequence()
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

    
    private readonly object SequenceLock = new();
    private bool _escapeSequence;
#pragma warning disable CS0414
    private bool _applicationProgramCommand; // APC
    private bool _controlSequenceIntroducer; // CSI
    private bool _deviceControlString;       // DCS
    private bool _startOfString;             // SOS
    private bool _operatingSystemCommand;    // OSC
    private bool _privacyMessage;            // PM
#pragma warning restore CS0414
    private readonly StringBuilder Sequence = new();

    private void StringTerminator()
    {
        var value = CloseSequence();
    }

    private void RaiseKeyAvailable(ConsoleKeyInfo e)
    {
        tcsKeyAvailable?.SetResult(e);
        tcsKeyAvailable = null;
        KeyAvailable?.Invoke(null, e);
    }

    private void RaiseSequenceAvailable()
    {
        var value = CloseSequence();
        tcsSequenceAvailable?.SetResult(value);
        tcsSequenceAvailable = null;
        SequenceAvailable?.Invoke(null, value);
    }

    private TaskCompletionSource<ConsoleKeyInfo?>? tcsKeyAvailable;
    private TaskCompletionSource<string?>? tcsSequenceAvailable;
    
    public void Send(string value)
    {
        #if _TRACECONN
            var sb = new StringBuilder("Send: ");
            foreach (var ch in value)
            {
                if (ch < ' ')
                    sb.Append($"<{(TerminalSeq.ControlChar)ch}>");
                else
                    sb.Append(ch);
            }
            Trace.TraceInformation(sb.ToString());
        #endif

        Console.Write(value);
    }
    
    public Task<string?> Post(string value, TimeSpan? timeout = null)
    {
        tcsSequenceAvailable = new TaskCompletionSource<string?>();
        Console.Write(value);

        return TaskHelpers.TaskWithTimeoutException(
            tcsSequenceAvailable.Task,
            timeout ?? TimeSpan.FromMilliseconds(100));
    }

    public Task<ConsoleKeyInfo?> ReadKey(TimeSpan? timeout = null)
    {
        tcsKeyAvailable = new TaskCompletionSource<ConsoleKeyInfo?>();

        return TaskHelpers.TaskWithTimeoutDefault(
            tcsKeyAvailable.Task, 
            timeout ?? TimeSpan.FromMilliseconds(200));
    }

    public void Dispose()
    {
        NormalScreen();
        _exitEvent.Dispose();
    }
}
