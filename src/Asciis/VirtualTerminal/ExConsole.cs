using System.Runtime.InteropServices;
using No8.Ascii.TermInfo;

namespace No8.Ascii.VirtualTerminal;

using System;
using System.Diagnostics;
using System.Text;

/// <summary>
///     Extended Console
/// </summary>
public partial class ExConsole : IDisposable
{

    private static readonly PosixSignalRegistration? _sigIntRegistration;
    private static readonly PosixSignalRegistration? _sigQuitRegistration;
    private static readonly PosixSignalRegistration? _sigTermRegistration;
    private static readonly PosixSignalRegistration? _sigWinChRegistration;

    private static EventHandler _windowChanged;
    private static EventHandler _closeSignal; 

    static ExConsole()
    {
        _sigIntRegistration = PosixSignalRegistration.Create(PosixSignal.SIGINT, HandlePosixSignal);    // Interrupt
        _sigQuitRegistration = PosixSignalRegistration.Create(PosixSignal.SIGQUIT, HandlePosixSignal);  // Quit
        _sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, HandlePosixSignal);  // Termination
        
        // This doesn't work on Windows
        _sigWinChRegistration = PosixSignalRegistration.Create(PosixSignal.SIGWINCH, HandleSignalWinCh);   // Window Changed (resize)

        static void HandlePosixSignal(PosixSignalContext context)
        {
            Debug.Assert(
                context.Signal is 
                    PosixSignal.SIGINT or 
                    PosixSignal.SIGQUIT or 
                    PosixSignal.SIGTERM);
            context.Cancel = true;
            
            _closeSignal?.Invoke(null, EventArgs.Empty);
        }

        static void HandleSignalWinCh(PosixSignalContext context)
        {
            _windowChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    /// <summary>
    ///     Static Destructor.
    /// </summary>
    static void ExConsole_dtor()
    {
        _sigIntRegistration?.Dispose();
        _sigQuitRegistration?.Dispose();
        _sigTermRegistration?.Dispose();
        _sigWinChRegistration?.Dispose();
    }

    private static readonly Destructor Finalize = new();
    private sealed class Destructor
    {
        ~Destructor() => ExConsole_dtor();
    }
    
    public class Options
    {
        public bool StartFullScreen { get; set; } = true;
        public bool StopOnControlC { get; set; } = true;
        public ConsoleDriver? ConsoleDriver { get; set; }
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
    public event EventHandler<WindowSize> WindowResized;
    public event EventHandler<PointerEvent> Pointer;

    
    private readonly ManualResetEventSlim _exitEvent = new(false);
    private readonly Options _options;
    private bool _fullScreen;
    private readonly bool _stopOnControlC;
    
    private readonly ConsoleDriver? _consoleDriver;
    private ConsoleDriver CD => _consoleDriver;
    
    public TermInfoDesc TermInfo { get; }


    private ExConsole(Options options)
    {
        _options = options;
        _consoleDriver = _options.ConsoleDriver ??= ConsoleDriver.Current;
    
        // Bug in optimiser. Must have full handler syntax
        Console.CancelKeyPress += new ConsoleCancelEventHandler(ConsoleCancelEventHandler);
        _stopOnControlC = options.StopOnControlC;

        _closeSignal += OnCloseSignal;
        _windowChanged += OnWindowChanged;

        TermInfo = TermInfoLoader.Load();
        InitConsole();
    }

    private void OnWindowChanged(object? sender, EventArgs e)
    {
        _consoleDriver.Write(TerminalSeq.ControlSeq.WindowManipulation("18")); // Report the size of the text area in characters.
    }

    private void OnCloseSignal(object? sender, EventArgs e)
    {
        if (_stopOnControlC)
            Stop();
    }

    private void InitConsole()
    {
        Send(TermInfo.Init1string);
        Send(TermInfo.Init2string);
        Send(TermInfo.ClearMargins);
        if (TermInfo.InitFile is not null)
        {
            Send(File.ReadAllText(TermInfo.InitFile));
        }
        Send(TermInfo.Init3string);
        
        Send(TermInfo.ExitAmMode);  // Auto Margin
    }

    public void Dispose()
    {
        _closeSignal -= OnCloseSignal;
        _windowChanged -= OnWindowChanged;

        NormalScreen();

        Send(TermInfo.ExitAttributeMode);
        Send(TermInfo.CursorNormal);
        
        Send(TermInfo.Reset1string);
        Send(TermInfo.Reset2string);
        if (TermInfo.ResetFile is not null)
        {
            Send(File.ReadAllText(TermInfo.ResetFile));
        }
        Send(TermInfo.Reset3string);
        
        _exitEvent.Dispose();
    }

    public void FullScreen()
    {
        if (_fullScreen)
            return;
        _fullScreen = true;
        Screen.ScreenAlt();
    }

    public void NormalScreen()
    {
        if (_fullScreen)
            Screen.ScreenNormal();
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
        
        Mouse.TrackingStart();
        Mouse.HighlightEnable();
        
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

            if (_firstMouseTime > 0)
            {
                var now = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
                if ((now - _firstMouseTime) > MaxClick)
                    ParseMouseEvents();
            }
        }
        
        // Leave
        try
        {
            //Terminal.Mouse.TrackingStop();
            //Terminal.Mouse.HighlightDisable();

            // only want to restore normal screen if entered Alt screen mode
            NormalScreen();
            Special.SoftReset();
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

        var conSeq = ConSeq.Parse(value);
        ProcessConSeq(conSeq);
        
        SequenceAvailable?.Invoke(null, value);
    }

    private TaskCompletionSource<ConsoleKeyInfo?>? tcsKeyAvailable;
    private TaskCompletionSource<string?>? tcsSequenceAvailable;
    
    public void Send(string? value)
    {
        if (value is null)
            return;
        
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

    private void ProcessConSeq(ConSeq? conSeq)
    {
        if (conSeq == null)
            return;

        switch (conSeq)
        {
            // Size of TextArea in characters
            case { Final: 't', Parameters: [8, _, _] }:
                WindowResized?.Invoke(this, new WindowSize(conSeq.Parameters[1], conSeq.Parameters[2]));
                break;
            
            // Cursor position
            case { Final:'R', Parameters: [_, _] }:
                break;

            case { Final: 'm', Parameters: [_, _, _] }:
                {
                    var mask = conSeq.Parameters[0];
                    var buttonId = mask & 0x03;
                    var shift = (mask & 0x04) == 0x04; 
                    var alt = (mask & 0x08) == 0x08; 
                    var ctrl = (mask & 0x10) == 0x10;

                    buttonId++;
                    var pointerEvent = new PointerEvent(
                        ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds(), 
                        PointerEventType.Released, 
                        conSeq.Parameters[1], 
                        conSeq.Parameters[2], 
                        buttonId, 0, shift, alt, ctrl);
                    
                    Pointer?.Invoke(this, pointerEvent);
                    PushPointerEvent(pointerEvent);
                }
                break;

            case { Final: 'M', Parameters: [_, _, _] }:
                {
                    var mask = conSeq.Parameters[0];
                    var x = conSeq.Parameters[1];
                    var y = conSeq.Parameters[2];
                    var buttonId = mask & 0x03;

                    var shift = (mask & 0x04) == 0x04; 
                    var alt = (mask & 0x08) == 0x08; 
                    var ctrl = (mask & 0x10) == 0x10;

                    var now = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();

                    // Pressed for mouse button 1,2,3
                    if (mask is >= 0 and <= 3)
                    {
                        buttonId++;
                        var pointerEvent = new PointerEvent(
                            now,
                            PointerEventType.Pressed,
                            x,
                            y,
                            buttonId, 0, shift, alt, ctrl);
                        PushPointerEvent(pointerEvent);
                        Pointer?.Invoke(this, pointerEvent);
                    }
                    // Mouse Wheel UP
                    else if (mask == 0x40)
                    {
                        var pointerEvent = new PointerEvent(
                            now,
                            PointerEventType.Wheel,
                            x,
                            y,
                            0, -1, shift, alt, ctrl);
                        Pointer?.Invoke(this, pointerEvent);
                    }
                    // Mouse wheel DOWN
                    else if (mask == 0x41)
                    {
                        var pointerEvent = new PointerEvent(
                            now,
                            PointerEventType.Wheel,
                            x,
                            y,
                            0, 1, shift, alt, ctrl);
                        Pointer?.Invoke(this, pointerEvent);
                    }
                    // Mouse move
                    else if ((mask & 0x20) == 0x20)
                    {
                        if (buttonId == 3)
                            buttonId = 0; // mouse move, no buttons pressed
                        else 
                            buttonId++;     // 0 = left button, 1 = middle button, 2 = right button => button 1,2,3 
                        
                        var pointerEvent = new PointerEvent(
                            now,
                            PointerEventType.Move,
                            x,
                            y,
                            buttonId,
                            0, shift, alt, ctrl);
                        Pointer?.Invoke(this, pointerEvent);
                    }
                }
                break;
            
            default:
                Trace.WriteLine(conSeq);
                break;
        }
    }

    private const int EV_MAX = 8;
    private const int MaxClick = 300;
    private readonly System.Collections.Concurrent.ConcurrentQueue<PointerEvent> _pointerEvents = new();
    private long _firstMouseTime;

    private void PushPointerEvent(PointerEvent pointerEvent)
    {
        if (_pointerEvents.Count == 0)
            _firstMouseTime = pointerEvent.TimeStamp;
        _pointerEvents.Enqueue(pointerEvent);

        var diff = pointerEvent.TimeStamp - _firstMouseTime;
        if (diff > MaxClick && pointerEvent.PointerEventType == PointerEventType.Released)
            ParseMouseEvents();
    }

    // Attempt to locate any click/double clicks
    // Called after MaxClick ms, or button released after MaxClick ms
    private void ParseMouseEvents()
    {
        if (_pointerEvents.IsEmpty)
            return;
        if (_pointerEvents.Count == 1 &&
            _pointerEvents.TryPeek(out var pe) &&
            pe.PointerEventType == PointerEventType.Pressed)
            return;
        
        var pointerEvents = _pointerEvents.ToArray();
        _pointerEvents.Clear();
        
        Trace.WriteLine("Parse Mouse Events:" + string.Join<PointerEvent>(", ", pointerEvents ));

        // If last event is a button pressed, then queue it for next mouse released
        var last = pointerEvents[^1]; 
        if (last.PointerEventType == PointerEventType.Pressed)
        {
            Trace.WriteLine("Last: " + last.ToString());
            _pointerEvents.Enqueue(last);
            _firstMouseTime = last.TimeStamp;
        }
        else
            _firstMouseTime = 0;

        for (int btn = 1; btn <= 3; btn++)
        {
            var count = pointerEvents.Count(
                pe => pe.PointerEventType == PointerEventType.Released && pe.ButtonId == btn);
            if (count > 0)
            {
                var btnLast = pointerEvents.Last(pe =>
                    pe.PointerEventType == PointerEventType.Released && pe.ButtonId == btn);
                var pointerEvent = btnLast with { PointerEventType = count > 1 ? PointerEventType.DoubleClick : PointerEventType.Click };
                
                Trace.WriteLine($"Clicks: {btn}: {count} {pointerEvent}");
                
                Pointer?.Invoke(this, pointerEvent);
            }
        }
    }
}
