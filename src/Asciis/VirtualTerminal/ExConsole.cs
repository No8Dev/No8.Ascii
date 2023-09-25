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
    private static readonly PosixSignalRegistration? SigIntRegistration;
    private static readonly PosixSignalRegistration? SigQuitRegistration;
    private static readonly PosixSignalRegistration? SigTermRegistration;
    private static readonly PosixSignalRegistration? SigWinChRegistration;

    private static event EventHandler? WindowChanged;
    private static event EventHandler? CloseSignal; 

    static ExConsole()
    {
        SigIntRegistration = PosixSignalRegistration.Create(PosixSignal.SIGINT, HandlePosixSignal);    // Interrupt
        SigQuitRegistration = PosixSignalRegistration.Create(PosixSignal.SIGQUIT, HandlePosixSignal);  // Quit
        SigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, HandlePosixSignal);  // Termination

        if (!OperatingSystem.IsWindows())
        {
            // This doesn't work on Windows
            SigWinChRegistration =
                PosixSignalRegistration.Create(PosixSignal.SIGWINCH, HandleSignalWinCh); // Window Changed (resize)
        }

        static void HandlePosixSignal(PosixSignalContext context)
        {
            Debug.Assert(
                context.Signal is 
                    PosixSignal.SIGINT or 
                    PosixSignal.SIGQUIT or 
                    PosixSignal.SIGTERM);

            if (CloseSignal == null)
                return;

            context.Cancel = true;
            CloseSignal.Invoke(null, EventArgs.Empty);
        }

        static void HandleSignalWinCh(PosixSignalContext context)
        {
            WindowChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    /// <summary>
    ///     Static Destructor.
    /// </summary>
    static void ExConsole_dtor()
    {
        SigIntRegistration?.Dispose();
        SigQuitRegistration?.Dispose();
        SigTermRegistration?.Dispose();
        SigWinChRegistration?.Dispose();
    }

    // ReSharper disable once UnusedMember.Local    Fake a static de-constructor
    private static readonly Destructor Finalize = new();
    private sealed class Destructor
    {
        ~Destructor() => ExConsole_dtor();
    }
    
    public static ExConsole Create(Action<ExConOptions>? configure = null)
    {
        ExConOptions options = new();
        configure?.Invoke(options);
        
        return new ExConsole(options);
    }

    public Task Run()
    {
        if (Alive && RunningTask != null)
            return RunningTask;
        
        if (_options.StartFullScreen)
            FullScreen();
        
        return RunningTask = Task.Run(MonitorInput); 
    }
    
    public bool Alive { get; private set; } = true;
    private Task? RunningTask { get; set; }

    
    public event EventHandler<ConsoleKeyInfo>? KeyAvailable;
    public event EventHandler<string>? SequenceAvailable;
    public event EventHandler<WindowSize>? WindowResized;
    public event EventHandler<PointerEvent>? Pointer;

    
    private readonly ManualResetEventSlim _exitEvent = new(false);
    private readonly ExConOptions _options;
    private bool _fullScreen;
    private readonly bool _stopOnControlC;
    
    private readonly ConsoleDriver? _consoleDriver;
    internal ConsoleDriver CD => _consoleDriver!;
    
    public TermInfoDesc? TermInfo { get; }


    private ExConsole(ExConOptions options)
    {
        _options = options;
        _consoleDriver = _options.ConsoleDriver ??= ConsoleDriver.Current;

        // Stop the dotNet Console CancelKeyPress
        // Bug in optimiser. Must have full handler syntax
        Console.CancelKeyPress += new ConsoleCancelEventHandler(ConsoleCancelEventHandler);
        _stopOnControlC = options.StopOnControlC;

        CloseSignal += OnCloseSignal;
        WindowChanged += OnWindowChanged;

        TermInfo = TermInfoLoader.Load();
        InitConsole();
    }

    private void OnWindowChanged(object? sender, EventArgs e)
    {
        System.Console.Out.Write(TerminalSeq.ControlSeq.WindowManipulation("18")); // Report the size of the text area in characters.
    }

    private void OnCloseSignal(object? sender, EventArgs e)
    {
        if (_stopOnControlC)
            Stop();
    }

    private void InitConsole()
    {
        if (TermInfo == null)
            return;
        
        Write(TermInfo.Init1string);
        Write(TermInfo.Init2string);
        Write(TermInfo.ClearMargins);
        if (TermInfo.InitFile is not null)
        {
            Write(File.ReadAllText(TermInfo.InitFile));
        }
        Write(TermInfo.Init3string);
        
        Write(TermInfo.ExitAmMode);  // Auto Margin
        Flush();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        
        CloseSignal -= CloseSignal;
        WindowChanged -= WindowChanged;

        NormalScreen();

        if (TermInfo != null)
        {
            Write(TermInfo.ExitAttributeMode);
            Write(TermInfo.CursorNormal);

            Write(TermInfo.Reset1string);
            Write(TermInfo.Reset2string);
            if (TermInfo.ResetFile is not null)
            {
                Write(File.ReadAllText(TermInfo.ResetFile));
            }

            Write(TermInfo.Reset3string);
            Flush();
        }

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
        e.Cancel = true;
        if (_stopOnControlC)
        {
            // Stop();
        }
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
    
    /*
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

        Console.Out.Write(value);
    }
    */

    public virtual void Write(string? value)
    {
        if (value is null)
            return;

        Console.Out.Write(value);
    }

    public virtual void Flush()
    {
        Console.Out.Flush();
    }
    
    public virtual void WriteLine(string? value) => Console.Out.WriteLine(value);

    public Task<string?> Post(string value, TimeSpan? timeout = null)
    {
        tcsSequenceAvailable = new TaskCompletionSource<string?>();
        Console.Out.Write(value);

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
                    _mouseX = conSeq.Parameters[1];
                    _mouseY = conSeq.Parameters[2];
                    var buttonId = mask & 0x03;
                    var shift = (mask & 0x04) == 0x04; 
                    var alt = (mask & 0x08) == 0x08; 
                    var ctrl = (mask & 0x10) == 0x10;

                    buttonId++;
                    var pointerEvent = new PointerEvent(
                        ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds(), 
                        PointerEventType.Released, 
                        _mouseX, 
                        _mouseY, 
                        buttonId, 0, shift, alt, ctrl);
                    
                    Pointer?.Invoke(this, pointerEvent);
                    PushPointerEvent(pointerEvent);
                }
                break;

            case { Final: 'M', Parameters: [_, _, _] }:
                {
                    var mask = conSeq.Parameters[0];
                    _mouseX = conSeq.Parameters[1];
                    _mouseY = conSeq.Parameters[2];
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
                            _mouseX,
                            _mouseY,
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
                            _mouseX,
                            _mouseY,
                            0, -1, shift, alt, ctrl);
                        Pointer?.Invoke(this, pointerEvent);
                    }
                    // Mouse wheel DOWN
                    else if (mask == 0x41)
                    {
                        var pointerEvent = new PointerEvent(
                            now,
                            PointerEventType.Wheel,
                            _mouseX,
                            _mouseY,
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
                            _mouseX,
                            _mouseY,
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
        if (_pointerEvents.IsEmpty)
            _firstMouseTime = pointerEvent.TimeStamp;
        _pointerEvents.Enqueue(pointerEvent);

        var diff = pointerEvent.TimeStamp - _firstMouseTime;
        if (diff > MaxClick && 
            pointerEvent.PointerEventType == PointerEventType.Released)
            ParseMouseEvents();
        else if (pointerEvent.PointerEventType == PointerEventType.Released &&
                 MouseEventsDoubleClick())
            ParseMouseEvents();
    }

    /// <summary>
    ///     Specific check if double clicked. No need to wait for full MaxClick
    /// </summary>
    private bool MouseEventsDoubleClick()
    {
        var pointerEvents = _pointerEvents.ToArray();

        for (int btn = 1; btn <= 3; btn++)
        {
            var count = pointerEvents.Count(
                pe => pe.PointerEventType == PointerEventType.Released && pe.ButtonId == btn);
            if (count > 1)
                return true;
        }
        return false;
    }

    /// <summary>
    ///     Attempt to locate any click/double clicks
    ///     Called after MaxClick ms, or button released after MaxClick ms
    /// </summary>
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
