using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Asciis.UI.Controls;
using Asciis.UI.Terminals;
using Microsoft.Extensions.Logging;

using static System.Diagnostics.Debug;

namespace Asciis.UI.Engine;

public interface IAsciiEngine
{
    Task Run(Screen screen);
    void Stop();
    void CloseScreen();
    void FocusUpdated(Composer? focused);
}

public partial class AsciiEngine
    : IAsciiEngine
{
    private bool _invalidateConsole;
    private readonly List<ScreenLayer> _screens = new();
    private readonly ILogger<AsciiEngine> _logger;

    private readonly bool[] _mouseOldState = new bool[5];
    private readonly bool[] _mouseNewState = new bool[5];
    private readonly byte[] _keyOldState = new byte[256];
    private readonly byte[] _keyNewState = new byte[256];

    private static bool _active;
    private static readonly object LockObject = new();
    private static readonly ManualResetEvent AppFinishedEvent = new(false);

    private Array2D<Glyph>? LastCanvas { get; set; }
    private Canvas MergedCanvas = new(1, 1);
    public Vec? Pointer { get; private set; } = Vec.Unknown;
    public KeyState[] Mouse { get; } = new KeyState[5];
    public KeyState[] VirtualKeys { get; } = new KeyState[256];
    public SmallRect Selection;

    public Func<bool> CanFinish = () => true;
    public Action<bool>? WindowFocusChange;

    public int MousePosX { get; private set; }
    public int MousePosY { get; private set; }
    public short MouseWheel { get; private set; }
    public short MouseHorizontalWheel { get; private set; }
    public bool IsFocused { get; private set; } = true;

    protected static bool Active
    {
        get => _active;
        set
        {
            if (_active != value)
            {
                lock (LockObject)
                    _active = value;
            }
        }
    }

    private Screen? TopScreen =>
        _screens.LastOrDefault()?.Screen;
    private Canvas? TopCanvas =>
        _screens.LastOrDefault()?.Canvas;

    public AsciiEngine(ILogger<AsciiEngine>? logger = null)
    {
        _logger = logger ?? BuildLogger();
        _logger.LogInformation("AsciiEngine");

        Console.OutputEncoding = Encoding.UTF8;

        MergedCanvas.Resize(Console.WindowWidth, Console.WindowHeight);
    }

    public KeyState GetVirtualKey(VirtualKeyCode code) =>
        VirtualKeys[(int)code];

    private ILogger<AsciiEngine> BuildLogger()
    {
        /*
        Trace       => May be sensitive.  Application internal
        Debug       => useful while debugging
        Information => Flow, long term value
        Warning     => Abnormal, unexpected
        Error       => Failure in current activity. Recoverable
        Critical    => Unrecoverable, crash
        None,
        */
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
#if DEBUG
                .AddFilter("Asciis.UI.Engine.AsciiEngine", LogLevel.Trace)
                .AddDebug();
#else
                .AddFilter("Asciis.UI.Engine.AsciiEngine", LogLevel.Information);
#endif
            });
        return loggerFactory.CreateLogger<AsciiEngine>();
    }

    private DateTime _screenTick = DateTime.Now;

    private bool RunOneLoop()
    {
        var nextTick = DateTime.Now;
        var elapsedTime = nextTick - _screenTick;
        _screenTick = nextTick;

        // Has screen changed size
        CheckCanvasSize();

        ProcessSelection();
        ProcessInputEvents();

        // For each screen layer, update layout (if required) and draw to canvas (if required)
        foreach (var (screen, canvas, tcs) in _screens.ToArray())
        {
            if (!screen.Update((float)elapsedTime.TotalSeconds))
            {
                Stop();

                return false;
            }
            if (screen.NeedsLayout && screen.Root != null)
            {
                screen.UpdateLayout();
                screen.NeedsLayout = false;
                screen.NeedsPainting = true;

                canvas.Clear();

                screen.RootComposer = Composer.MeasureArrange(screen.Root, canvas.Width - 1, canvas.Height - 1);
            }
            if (screen.NeedsPainting && screen.Root != null)
            {
                screen.RootComposer?.UpdateLayoutValues();
                screen.RootComposer?.Draw(canvas, screen.RootLayout?.Parent?.Bounds ?? new Rect(Vec.Zero, canvas.Size));

                var skin = SkinManager.Instance.Current;
                skin.Apply(screen.Root);

                InvalidateConsole();
            }

            screen.Clean();
        }

        if (_invalidateConsole)
        {
            WriteConsole(false);
            LastCanvas = MergedCanvas.Glyphs.Clone();
            UpdateCursor();
        }

        return true;
    }

    public Task Run(Screen screen)
    {
        screen.Engine = this;
        var layer = new ScreenLayer(
                                    screen,
                                    new Canvas(Console.WindowWidth, Console.WindowHeight),
                                    new TaskCompletionSource());
        _screens.Add(layer);

        if (!Active)
            new Thread(Start).Start();

        return layer.TaskCompletionSource.Task;
    }

    public void CloseScreen()
    {
        _screens.RemoveLast();
        InvalidateLayout();
    }

    public void FocusUpdated(Composer? focused)
    {
        if (TopCanvas != null)
        {
            TopCanvas.Cursor = focused?.FocusCursor ?? Vec.Zero;
            UpdateCursor();
        }
    }

    private void UpdateCursor()
    {
        // Note: XTerm cursor is 1 based.

        var cursor = TopCanvas?.Cursor;

        if (cursor != null && cursor != Vec.Unknown && TopCanvas != null)
        {
            if (cursor.X < TopCanvas.Width && cursor.Y < TopCanvas.Height)
            {
                var (left, top) = Console.GetCursorPosition();
                if (left != cursor.X || top != cursor.Y)
                {
                    Console.Write(Terminal.Cursor.Show);
                    Console.Write(Terminal.Cursor.Set(cursor.Y + 1, cursor.X + 1));
                }
            }
        }
        else
        {
            Console.Write(Terminal.Cursor.Set(1, 1));
            Console.Write(Terminal.Cursor.Hide);
        }
    }

    private void Start()
    {
        Active = true;
        Console.CursorVisible = false;

        int width = Console.WindowWidth;
        int height = Console.WindowHeight;

        LastCanvas = null;
        Resize(width, height);

        Console.CancelKeyPress += (_, args) =>
                                         {
                                             args.Cancel = true;
                                             Stop();
                                         };

        Init();
        Console.Write(Terminal.Mode.ScreenAlt);

        while (Active)
        {
            while (Active)
            {
                RunOneLoop();
            }

            // Calling program can cancel exit
            if (CanFinish?.Invoke() ?? true)
                AppFinishedEvent.Set();
            else
                Active = true;
        }

        foreach (var (screen, canvas, tcs) in _screens.ToArray())
            tcs.SetResult();

        Console.Write(Terminal.Mode.ScreenNormal);

        //Console.SetCursorPosition(0, Console.WindowHeight - 2);
        Console.CursorVisible = true;
    }

    public void Stop()
    {
        Active = false;
    }

    public bool CheckCanvasSize()
    {
        if (MergedCanvas.Width != Console.WindowWidth ||
            MergedCanvas.Height != Console.WindowHeight)
        {
            Resize(Console.WindowWidth, Console.WindowHeight);
            return true;
        }

        return false;
    }

    private void MergeScreens()
    {
        MergedCanvas.Clear();
        foreach (var screenLayer in _screens.ToArray())
            MergedCanvas.Overwrite(screenLayer.Canvas);
    }

    protected virtual void RaiseWindowFocusChange(bool focused) =>
        WindowFocusChange?.Invoke(focused);

    private bool HandleScrollWheelEvent(ScrollWheelEventArgs args)
    {
        if (args.Value == 0) return false;

        return TopScreen?.HandleScrollWheelEvent(args) ?? false;
    }

    /// <summary>
    /// Convert raw mouse events into Control and Screen mouse events
    /// </summary>
    private void HandlePointerEvent(PointerEventArgs pointerEvent)
    {
        var msg = $"{pointerEvent}";
        var lastPointer = Pointer;
        Pointer = new Vec(pointerEvent.X, pointerEvent.Y);

        var screen = TopScreen;
        if (screen == null) return;

        var lastComposers = screen.ComposersAt(lastPointer).ToList();
        var composersUnderPointer = screen.ComposersAt(Pointer).ToList();

        // composers that are no longer under the pointer
        var composersLeave = lastComposers.Where(c => !composersUnderPointer.Contains(c));
        var composersEnter = composersUnderPointer.Where(c => !lastComposers.Contains(c));
        foreach (var composer in composersLeave)
        {
            msg += $" PointerLeave: {composer.Control.ShortString}";
            composer.HandlePointerEvent(pointerEvent with { PointerEventType = PointerEventType.Leave });
        }
        foreach (var composer in composersEnter)
        {
            msg += $" PointerEnter: {composer.Control.ShortString}";
            composer.HandlePointerEvent(pointerEvent with { PointerEventType = PointerEventType.Enter });
        }

        foreach (var composer in composersUnderPointer)
        {
            var handled = composer.HandlePointerEvent(pointerEvent);
            if (handled)
            {
                msg += $", {composer.Control.ShortString}";
                break;
            }
        }

        if (lastPointer == Vec.Unknown)
        {
            msg += " PointerEnter: Screen";
            screen.HandlePointerEvent(pointerEvent with { PointerEventType = PointerEventType.Enter });
        }

        screen.HandlePointerEvent(pointerEvent);
        screen.NeedsPainting = true;

        // If mouse moves outside window
        if (pointerEvent.PointerEventType == PointerEventType.Leave &&
            lastPointer != null)
        {
            foreach (var composer in lastComposers)
            {
                msg += $" PointerLeave: Screen {composer.Control.ShortString}";
                composer.HandlePointerEvent(pointerEvent with
                {
                    PointerEventType = PointerEventType.Leave,
                    Handled = false
                });
            }

            msg += $" PointerLeave: Screen";
            screen.HandlePointerEvent(pointerEvent with { PointerEventType = PointerEventType.Leave });
            screen.NeedsPainting = true;
            Pointer = Vec.Unknown;
        }

        _logger.LogTrace(msg);
    }

    private bool HandleKeyEvent(AKeyEventArgs keyArgs)
    {
        _logger.LogTrace(keyArgs.ToString());

        // TopScreen?.HandleKeyEvent(key, keyArgs);
        var root = TopScreen?.RootComposer;
        if (root == null)
            return false;

        if (root.PreProcessKey(keyArgs))
            return true;

        if (root.HandleKeyEvent(keyArgs))
            return true;

        if (keyArgs.Key == VirtualKeyCode.VK_TAB && keyArgs.Modifier.HasFlag(AKeyModifier.Shift))
            keyArgs = keyArgs with { Key = VirtualKeyCode.VK_BROWSER_BACK };

        switch (keyArgs.Key)
        {
            case VirtualKeyCode.VK_TAB:
            case VirtualKeyCode.VK_BROWSER_FORWARD:
                if (keyArgs.EventType == KeyEventType.Released)
                {
                    if (TopScreen?.FocusManager.Next() ?? false)
                        return true;
                }
                break;

            case VirtualKeyCode.VK_BROWSER_BACK:
                if (keyArgs.EventType == KeyEventType.Released)
                {
                    if (TopScreen?.FocusManager.Prev() ?? false)
                        return true;
                }
                break;

            case VirtualKeyCode.VK_RIGHT:
            case VirtualKeyCode.VK_DOWN:
                if (keyArgs.EventType == KeyEventType.Released)
                {
                    if (TopScreen?.FocusManager.NextInGroup() ?? false)
                        return true;
                }
                break;

            case VirtualKeyCode.VK_LEFT:
            case VirtualKeyCode.VK_UP:
                if (keyArgs.EventType == KeyEventType.Released)
                {
                    if (TopScreen?.FocusManager.PrevInGroup() ?? false)
                        return true;
                }
                break;

        }

        return root.PostProcessKey(keyArgs);
    }

    public void InvalidateConsole() { _invalidateConsole = true; }

    public void Resize(int width, int height)
    {
        _logger.LogDebug("Resize ({Width}, {Height})", width, height);

        LastCanvas = null;
        MergedCanvas.Resize(width, height);
        foreach (var canvas in _screens.Select(s => s.Canvas).ToArray())
            canvas.Resize(width, height);
        InvalidateLayout();
    }

    public void InvalidateLayout()
    {
        foreach (var screenLayer in _screens.ToArray())
        {
            screenLayer.Screen.NeedsLayout = true;
            screenLayer.Screen.NeedsPainting = true;
        }
    }

    private record ScreenLayer(Screen Screen, Canvas Canvas, TaskCompletionSource TaskCompletionSource);
}

