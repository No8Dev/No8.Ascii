namespace Asciis.Terminal.ConsoleDrivers.NetDriver;

internal class SystemConsoleEvents
{
    private ManualResetEventSlim inputReady = new(false);
    private ManualResetEventSlim waitForStart = new(false);
    private ManualResetEventSlim winChange = new(false);
    private Queue<InputResult?> inputResultQueue = new();
    private ConsoleDriver consoleDriver => Application.Driver;
    private int lastWindowHeight;
    private int largestWindowHeight;
#if PROCESS_REQUEST
		bool neededProcessRequest;
#endif
    public int NumberOfCSI { get; }

    private AsciiApplication Application { get; }

    public SystemConsoleEvents(AsciiApplication application, int numberOfCSI = 1)
    {
        Application = application;
        NumberOfCSI = numberOfCSI;
        Task.Run(ProcessInputResultQueue);
        Task.Run(CheckWinChange);
    }

    public InputResult? ReadConsoleInput()
    {
        while (true)
        {
            waitForStart.Set();
            winChange.Set();

            if (inputResultQueue.Count == 0)
            {
                inputReady.Wait();
                inputReady.Reset();
            }
#if PROCESS_REQUEST
				neededProcessRequest = false;
#endif
            if (inputResultQueue.Count > 0) return inputResultQueue.Dequeue();
        }
    }

    private void ProcessInputResultQueue()
    {
        while (true)
        {
            waitForStart.Wait();
            waitForStart.Reset();

            if (inputResultQueue.Count == 0) GetConsoleInputType(Console.ReadKey(true));

            inputReady.Set();
        }
    }

    private void CheckWinChange()
    {
        while (true)
        {
            winChange.Wait();
            winChange.Reset();
            WaitWinChange();
            inputReady.Set();
        }
    }

    private void WaitWinChange()
    {
        while (true)
            if (!consoleDriver.HeightAsBuffer)
            {
                if (Console.WindowWidth != consoleDriver.Cols || Console.WindowHeight != consoleDriver.Rows)
                {
                    var w = Math.Max(Console.WindowWidth, 0);
                    var h = Math.Max(Console.WindowHeight, 0);
                    GetWindowSizeEvent(new Size(w, h));
                    return;
                }
            }
            else
            {
                //largestWindowHeight = Math.Max (Console.BufferHeight, largestWindowHeight);
                largestWindowHeight = Console.BufferHeight;
                if (Console.BufferWidth != consoleDriver.Cols || largestWindowHeight != consoleDriver.Rows
                                                              || Console.WindowHeight != lastWindowHeight)
                {
                    lastWindowHeight = Console.WindowHeight;
                    GetWindowSizeEvent(new Size(Console.BufferWidth, lastWindowHeight));
                    return;
                }

                if (Console.WindowTop != consoleDriver.Top)
                {
                    // Top only working on Windows.
                    var winPositionEv = new WindowPositionEvent()
                    {
                        Top = Console.WindowTop
                    };
                    inputResultQueue.Enqueue(
                        new InputResult()
                        {
                            EventType = EventType.WindowPosition,
                            WindowPositionEvent = winPositionEv
                        });
                    return;
                }
#if PROCESS_REQUEST
					if (!neededProcessRequest) {
						Console.Out.Write ("\x1b[6n");
						neededProcessRequest = true;
					}
#endif
            }
    }

    private void GetWindowSizeEvent(Size size)
    {
        var windowSizeEvent = new WindowSizeEvent()
        {
            Size = size
        };

        inputResultQueue.Enqueue(
            new InputResult()
            {
                EventType = EventType.WindowSize,
                WindowSizeEvent = windowSizeEvent
            });
    }

    private void GetConsoleInputType(ConsoleKeyInfo consoleKeyInfo)
    {
        var inputResult = new InputResult
        {
            EventType = EventType.Key
        };
        var newConsoleKeyInfo = consoleKeyInfo;
        ConsoleKey key = 0;
        var mouseEvent = new MouseEvent();
        var keyChar = consoleKeyInfo.KeyChar;
        switch ((uint)keyChar)
        {
            case 0:
                if (consoleKeyInfo.Key == (ConsoleKey)64) // Ctrl+Space in Windows.
                    newConsoleKeyInfo = new ConsoleKeyInfo(
                        ' ',
                        ConsoleKey.Spacebar,
                        (consoleKeyInfo.Modifiers & ConsoleModifiers.Shift) != 0,
                        (consoleKeyInfo.Modifiers & ConsoleModifiers.Alt) != 0,
                        (consoleKeyInfo.Modifiers & ConsoleModifiers.Control) != 0);
                break;
            case uint n when n >= '\u0001' && n <= '\u001a':
                if (consoleKeyInfo.Key == 0)
                {
                    key = (ConsoleKey)(char)(consoleKeyInfo.KeyChar + (uint)ConsoleKey.A - 1);
                    newConsoleKeyInfo = new ConsoleKeyInfo(
                        (char)key,
                        key,
                        (consoleKeyInfo.Modifiers & ConsoleModifiers.Shift) != 0,
                        (consoleKeyInfo.Modifiers & ConsoleModifiers.Alt) != 0,
                        true);
                }

                break;
            case 27:
                //case 91:
                ConsoleKeyInfo[] cki = new ConsoleKeyInfo[] { consoleKeyInfo };
                var mod = consoleKeyInfo.Modifiers;
                var delay = 0;
                while (delay < 100)
                {
                    if (Console.KeyAvailable)
                    {
                        do
                        {
                            var result = Console.ReadKey(true);
                            Array.Resize(ref cki, cki == null ? 1 : cki.Length + 1);
                            cki[cki.Length - 1] = result;
                        } while (Console.KeyAvailable);

                        break;
                    }

                    Thread.Sleep(50);
                    delay += 50;
                }

                SplitCSI(cki, ref inputResult, ref newConsoleKeyInfo, ref key, ref mouseEvent, ref mod);
                return;
            case 127:
                newConsoleKeyInfo = new ConsoleKeyInfo(
                    consoleKeyInfo.KeyChar,
                    ConsoleKey.Backspace,
                    (consoleKeyInfo.Modifiers & ConsoleModifiers.Shift) != 0,
                    (consoleKeyInfo.Modifiers & ConsoleModifiers.Alt) != 0,
                    (consoleKeyInfo.Modifiers & ConsoleModifiers.Control) != 0);
                break;
            default:
                newConsoleKeyInfo = consoleKeyInfo;
                break;
        }

        if (inputResult.EventType == EventType.Key)
            inputResult.ConsoleKeyInfo = newConsoleKeyInfo;
        else
            inputResult.MouseEvent = mouseEvent;

        inputResultQueue.Enqueue(inputResult);
    }

    private void SplitCSI(
        ConsoleKeyInfo[] cki,
        ref InputResult inputResult,
        ref ConsoleKeyInfo newConsoleKeyInfo,
        ref ConsoleKey key,
        ref MouseEvent mouseEvent,
        ref ConsoleModifiers mod)
    {
        ConsoleKeyInfo[] splitedCki = new ConsoleKeyInfo[] { };
        var length = 0;
        var kChar = GetKeyCharArray(cki);
        var nCSI = GetNumberOfCSI(kChar);
        var curCSI = 0;
        var previousKChar = '\0';
        if (nCSI > 1)
            for (var i = 0; i < cki.Length; i++)
            {
                var ck = cki[i];
                if (NumberOfCSI > 0 && nCSI - curCSI > NumberOfCSI)
                {
                    if (i + 1 < cki.Length && cki[i + 1].KeyChar == '\x1b' && previousKChar != '\0')
                    {
                        curCSI++;
                        previousKChar = '\0';
                    }
                    else
                    {
                        previousKChar = ck.KeyChar;
                    }

                    continue;
                }

                if (ck.KeyChar == '\x1b')
                {
                    if (ck.KeyChar == 'R') ResizeArray(ck);
                    if (splitedCki.Length > 1)
                        DecodeCSI(
                            ref inputResult,
                            ref newConsoleKeyInfo,
                            ref key,
                            ref mouseEvent,
                            splitedCki,
                            ref mod);
                    splitedCki = new ConsoleKeyInfo[] { };
                    length = 0;
                }

                ResizeArray(ck);
                if (i == cki.Length - 1 && splitedCki.Length > 0)
                    DecodeCSI(ref inputResult, ref newConsoleKeyInfo, ref key, ref mouseEvent, splitedCki, ref mod);
            }
        else
            DecodeCSI(ref inputResult, ref newConsoleKeyInfo, ref key, ref mouseEvent, cki, ref mod);

        void ResizeArray(ConsoleKeyInfo ck)
        {
            length++;
            Array.Resize(ref splitedCki, length);
            splitedCki[length - 1] = ck;
        }
    }

    private char[] GetKeyCharArray(ConsoleKeyInfo[] cki)
    {
        char[] kChar = new char[] { };
        var length = 0;
        foreach (var kc in cki)
        {
            length++;
            Array.Resize(ref kChar, length);
            kChar[length - 1] = kc.KeyChar;
        }

        return kChar;
    }

    private int GetNumberOfCSI(char[] csi)
    {
        var nCSI = 0;
        for (var i = 0; i < csi.Length; i++)
            if (csi[i] == '\x1b' || csi[i] == '[' && (i == 0 || i > 0 && csi[i - 1] != '\x1b'))
                nCSI++;

        return nCSI;
    }

    private void DecodeCSI(
        ref InputResult inputResult,
        ref ConsoleKeyInfo newConsoleKeyInfo,
        ref ConsoleKey key,
        ref MouseEvent mouseEvent,
        ConsoleKeyInfo[] cki,
        ref ConsoleModifiers mod)
    {
        switch (cki.Length)
        {
            case 2:
                if ((uint)cki[1].KeyChar >= 1 && (uint)cki[1].KeyChar <= 26)
                {
                    key = (ConsoleKey)(char)(cki[1].KeyChar + (uint)ConsoleKey.A - 1);
                    newConsoleKeyInfo = new ConsoleKeyInfo(
                        cki[1].KeyChar,
                        key,
                        false,
                        true,
                        true);
                }
                else
                {
                    if (cki[1].KeyChar >= 97 && cki[1].KeyChar <= 122)
                        key = (ConsoleKey)cki[1].KeyChar.ToString().ToUpper()[0];
                    else
                        key = (ConsoleKey)cki[1].KeyChar;
                    newConsoleKeyInfo = new ConsoleKeyInfo(
                        (char)key,
                        (ConsoleKey)Math.Min((uint)key, 255),
                        false,
                        true,
                        false);
                }

                break;
            case 3:
                if (cki[1].KeyChar == '[' || cki[1].KeyChar == 79)
                    key = GetConsoleKey(cki[2].KeyChar, ref mod, cki.Length);
                newConsoleKeyInfo = new ConsoleKeyInfo(
                    '\0',
                    key,
                    (mod & ConsoleModifiers.Shift) != 0,
                    (mod & ConsoleModifiers.Alt) != 0,
                    (mod & ConsoleModifiers.Control) != 0);
                break;
            case 4:
                if (cki[1].KeyChar == '[' && cki[3].KeyChar == 126)
                {
                    key = GetConsoleKey(cki[2].KeyChar, ref mod, cki.Length);
                    newConsoleKeyInfo = new ConsoleKeyInfo(
                        '\0',
                        key,
                        (mod & ConsoleModifiers.Shift) != 0,
                        (mod & ConsoleModifiers.Alt) != 0,
                        (mod & ConsoleModifiers.Control) != 0);
                }

                break;
            case 5:
                if (cki[1].KeyChar == '[' && (cki[2].KeyChar == 49 || cki[2].KeyChar == 50)
                                          && cki[4].KeyChar == 126)
                {
                    key = GetConsoleKey(cki[3].KeyChar, ref mod, cki.Length);
                }
                else if (cki[1].KeyChar == 49 && cki[2].KeyChar == ';')
                { // For WSL
                    mod |= GetConsoleModifiers(cki[3].KeyChar);
                    key = ConsoleKey.End;
                }

                newConsoleKeyInfo = new ConsoleKeyInfo(
                    '\0',
                    key,
                    (mod & ConsoleModifiers.Shift) != 0,
                    (mod & ConsoleModifiers.Alt) != 0,
                    (mod & ConsoleModifiers.Control) != 0);
                break;
            case 6:
                if (cki[1].KeyChar == '[' && cki[2].KeyChar == 49 && cki[3].KeyChar == ';')
                {
                    mod |= GetConsoleModifiers(cki[4].KeyChar);
                    key = GetConsoleKey(cki[5].KeyChar, ref mod, cki.Length);
                }
                else if (cki[1].KeyChar == '[' && cki[3].KeyChar == ';')
                {
                    mod |= GetConsoleModifiers(cki[4].KeyChar);
                    key = GetConsoleKey(cki[2].KeyChar, ref mod, cki.Length);
                }

                newConsoleKeyInfo = new ConsoleKeyInfo(
                    '\0',
                    key,
                    (mod & ConsoleModifiers.Shift) != 0,
                    (mod & ConsoleModifiers.Alt) != 0,
                    (mod & ConsoleModifiers.Control) != 0);
                break;
            case 7:
                GetRequestEvent(GetKeyCharArray(cki));
                return;
            case int n when n >= 8:
                GetMouseEvent(cki);
                return;
        }

        if (inputResult.EventType == EventType.Key)
            inputResult.ConsoleKeyInfo = newConsoleKeyInfo;
        else
            inputResult.MouseEvent = mouseEvent;

        inputResultQueue.Enqueue(inputResult);
    }

    private Point lastCursorPosition;

    private void GetRequestEvent(char[] kChar)
    {
        var eventType = new EventType();
        var point = new Point();
        var foundPoint = 0;
        string value = "";
        for (var i = 0; i < kChar.Length; i++)
        {
            var c = kChar[i];
            if (c == '[')
            {
                foundPoint++;
            }
            else if (foundPoint == 1 && c != ';')
            {
                value += c.ToString();
            }
            else if (c == ';')
            {
                if (foundPoint == 1) point.Y = int.Parse(value) - 1;
                value = "";
                foundPoint++;
            }
            else if (foundPoint > 0 && i < kChar.Length - 1)
            {
                value += c.ToString();
            }
            else if (i == kChar.Length - 1)
            {
                point.X = int.Parse(value) + Console.WindowTop - 1;

                switch (c)
                {
                    case 'R':
                        if (lastCursorPosition.Y != point.Y)
                        {
                            lastCursorPosition = point;
                            eventType = EventType.WindowPosition;
                            var winPositionEv = new WindowPositionEvent()
                            {
                                CursorPosition = point
                            };
                            inputResultQueue.Enqueue(
                                new InputResult()
                                {
                                    EventType = eventType,
                                    WindowPositionEvent = winPositionEv
                                });
                        }
                        else
                        {
                            return;
                        }

                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        inputReady.Set();
    }

    private MouseEvent lastMouseEvent;
    private bool isButtonPressed;
    private bool isButtonClicked;
    private bool isButtonDoubleClicked;
    private bool isButtonTripleClicked;
    private bool isProcContBtnPressedRuning;

    private int buttonPressedCount;
    //bool isButtonReleased;

    private void GetMouseEvent(ConsoleKeyInfo[] cki)
    {
        var mouseEvent = new MouseEvent();
        MouseButtonState buttonState = 0;
        var point = new Point();
        var buttonCode = 0;
        var foundButtonCode = false;
        var foundPoint = 0;
        string value = "";
        var kChar = GetKeyCharArray(cki);
        for (var i = 0; i < kChar.Length; i++)
        {
            var c = kChar[i];
            if (c == '<')
            {
                foundButtonCode = true;
            }
            else if (foundButtonCode && c != ';')
            {
                value += c.ToString();
            }
            else if (c == ';')
            {
                if (foundButtonCode)
                {
                    foundButtonCode = false;
                    buttonCode = int.Parse(value);
                }

                if (foundPoint == 1) point.X = int.Parse(value) - 1;
                value = "";
                foundPoint++;
            }
            else if (foundPoint > 0 && c != 'm' && c != 'M')
            {
                value += c.ToString();
            }
            else if (c == 'm' || c == 'M')
            {
                point.Y = int.Parse(value) + Console.WindowTop - 1;

                //if (c == 'M') {
                //	isButtonPressed = true;
                //} else if (c == 'm') {
                //	isButtonPressed = false;
                //}

                switch (buttonCode)
                {
                    case 0:
                    case 8:
                    case 16:
                    case 24:
                    case 32:
                    case 36:
                    case 40:
                    case 48:
                    case 56:
                        buttonState = c == 'M'
                            ? MouseButtonState.Button1Pressed
                            : MouseButtonState.Button1Released;
                        break;
                    case 1:
                    case 9:
                    case 17:
                    case 25:
                    case 33:
                    case 37:
                    case 41:
                    case 45:
                    case 49:
                    case 53:
                    case 57:
                    case 61:
                        buttonState = c == 'M'
                            ? MouseButtonState.Button2Pressed
                            : MouseButtonState.Button2Released;
                        break;
                    case 2:
                    case 10:
                    case 14:
                    case 18:
                    case 22:
                    case 26:
                    case 30:
                    case 34:
                    case 42:
                    case 46:
                    case 50:
                    case 54:
                    case 58:
                    case 62:
                        buttonState = c == 'M'
                            ? MouseButtonState.Button3Pressed
                            : MouseButtonState.Button3Released;
                        break;
                    case 35:
                    case 39:
                    case 43:
                    case 47:
                    case 55:
                    case 59:
                    case 63:
                        buttonState = MouseButtonState.ReportMousePosition;
                        break;
                    case 64:
                        buttonState = MouseButtonState.ButtonWheeledUp;
                        break;
                    case 65:
                        buttonState = MouseButtonState.ButtonWheeledDown;
                        break;
                    case 68:
                    case 72:
                    case 80:
                        buttonState = MouseButtonState.ButtonWheeledLeft; // Shift/Ctrl+ButtonWheeledUp
                        break;
                    case 69:
                    case 73:
                    case 81:
                        buttonState = MouseButtonState.ButtonWheeledRight; // Shift/Ctrl+ButtonWheeledDown
                        break;
                }

                // Modifiers.
                switch (buttonCode)
                {
                    case 8:
                    case 9:
                    case 10:
                    case 43:
                        buttonState |= MouseButtonState.ButtonAlt;
                        break;
                    case 14:
                    case 47:
                        buttonState |= MouseButtonState.ButtonAlt | MouseButtonState.ButtonShift;
                        break;
                    case 16:
                    case 17:
                    case 18:
                    case 51:
                        buttonState |= MouseButtonState.ButtonCtrl;
                        break;
                    case 22:
                    case 55:
                        buttonState |= MouseButtonState.ButtonCtrl | MouseButtonState.ButtonShift;
                        break;
                    case 24:
                    case 25:
                    case 26:
                    case 59:
                        buttonState |= MouseButtonState.ButtonAlt | MouseButtonState.ButtonCtrl;
                        break;
                    case 30:
                    case 63:
                        buttonState |= MouseButtonState.ButtonCtrl | MouseButtonState.ButtonShift |
                                       MouseButtonState.ButtonAlt;
                        break;
                    case 32:
                    case 33:
                    case 34:
                        buttonState |= MouseButtonState.ReportMousePosition;
                        break;
                    case 36:
                    case 37:
                        buttonState |= MouseButtonState.ReportMousePosition | MouseButtonState.ButtonShift;
                        break;
                    case 39:
                    case 68:
                    case 69:
                        buttonState |= MouseButtonState.ButtonShift;
                        break;
                    case 40:
                    case 41:
                    case 42:
                        buttonState |= MouseButtonState.ReportMousePosition | MouseButtonState.ButtonAlt;
                        break;
                    case 45:
                    case 46:
                        buttonState |= MouseButtonState.ReportMousePosition | MouseButtonState.ButtonAlt |
                                       MouseButtonState.ButtonShift;
                        break;
                    case 48:
                    case 49:
                    case 50:
                        buttonState |= MouseButtonState.ReportMousePosition | MouseButtonState.ButtonCtrl;
                        break;
                    case 53:
                    case 54:
                        buttonState |= MouseButtonState.ReportMousePosition | MouseButtonState.ButtonCtrl |
                                       MouseButtonState.ButtonShift;
                        break;
                    case 56:
                    case 57:
                    case 58:
                        buttonState |= MouseButtonState.ReportMousePosition | MouseButtonState.ButtonCtrl |
                                       MouseButtonState.ButtonAlt;
                        break;
                    case 61:
                    case 62:
                        buttonState |= MouseButtonState.ReportMousePosition | MouseButtonState.ButtonCtrl |
                                       MouseButtonState.ButtonShift | MouseButtonState.ButtonAlt;
                        break;
                }
            }
        }

        mouseEvent.Position.X = point.X;
        mouseEvent.Position.Y = point.Y;
        mouseEvent.ButtonState = buttonState;
        //System.Diagnostics.Debug.WriteLine ($"ButtonState: {mouseEvent.ButtonState} X: {mouseEvent.Position.X} Y: {mouseEvent.Position.Y}");

        if (isButtonDoubleClicked)
            Application.MainLoop.AddIdle(
                () =>
                {
                    Task.Run(async () => await ProcessButtonDoubleClickedAsync());
                    return false;
                });

        if ((buttonState & MouseButtonState.Button1Pressed) != 0
            || (buttonState & MouseButtonState.Button2Pressed) != 0
            || (buttonState & MouseButtonState.Button3Pressed) != 0)
        {
            if ((buttonState & MouseButtonState.ReportMousePosition) == 0)
                buttonPressedCount++;
            else
                buttonPressedCount = 0;
            //System.Diagnostics.Debug.WriteLine ($"buttonPressedCount: {buttonPressedCount}");
            isButtonPressed = true;
        }
        else
        {
            isButtonPressed = false;
            buttonPressedCount = 0;
        }

        if (buttonPressedCount == 2 && !isButtonDoubleClicked
                                    && (lastMouseEvent.ButtonState == MouseButtonState.Button1Pressed
                                        || lastMouseEvent.ButtonState == MouseButtonState.Button2Pressed
                                        || lastMouseEvent.ButtonState == MouseButtonState.Button3Pressed))
        {
            isButtonDoubleClicked = true;
            ProcessButtonDoubleClicked(mouseEvent);
            inputReady.Set();
            return;
        }
        else if (buttonPressedCount == 3 && isButtonDoubleClicked
                                         && (lastMouseEvent.ButtonState == MouseButtonState.Button1Pressed
                                             || lastMouseEvent.ButtonState == MouseButtonState.Button2Pressed
                                             || lastMouseEvent.ButtonState == MouseButtonState.Button3Pressed))
        {
            isButtonDoubleClicked = false;
            isButtonTripleClicked = true;
            buttonPressedCount = 0;
            ProcessButtonTripleClicked(mouseEvent);
            lastMouseEvent = mouseEvent;
            inputReady.Set();
            return;
        }

        //System.Diagnostics.Debug.WriteLine ($"isButtonClicked: {isButtonClicked} isButtonDoubleClicked: {isButtonDoubleClicked} isButtonTripleClicked: {isButtonTripleClicked}");
        if ((isButtonClicked || isButtonDoubleClicked || isButtonTripleClicked)
            && ((buttonState & MouseButtonState.Button1Released) != 0
                || (buttonState & MouseButtonState.Button2Released) != 0
                || (buttonState & MouseButtonState.Button3Released) != 0))
        {
            //isButtonClicked = false;
            //isButtonDoubleClicked = false;
            isButtonTripleClicked = false;
            buttonPressedCount = 0;
            return;
        }

        if (isButtonClicked && !isButtonDoubleClicked && lastMouseEvent.Position != default &&
            lastMouseEvent.Position == point
            && ((buttonState & MouseButtonState.Button1Pressed) != 0
                || (buttonState & MouseButtonState.Button2Pressed) != 0
                || (buttonState & MouseButtonState.Button3Pressed) != 0
                || (buttonState & MouseButtonState.Button1Released) != 0
                || (buttonState & MouseButtonState.Button2Released) != 0
                || (buttonState & MouseButtonState.Button3Released) != 0))
        {
            isButtonClicked = false;
            isButtonDoubleClicked = true;
            ProcessButtonDoubleClicked(mouseEvent);
            Application.MainLoop.AddIdle(
                () =>
                {
                    Task.Run(
                        async () =>
                        {
                            await Task.Delay(600);
                            isButtonDoubleClicked = false;
                        });
                    return false;
                });
            inputReady.Set();
            return;
        }

        if (isButtonDoubleClicked && lastMouseEvent.Position != default && lastMouseEvent.Position == point
            && ((buttonState & MouseButtonState.Button1Pressed) != 0
                || (buttonState & MouseButtonState.Button2Pressed) != 0
                || (buttonState & MouseButtonState.Button3Pressed) != 0
                || (buttonState & MouseButtonState.Button1Released) != 0
                || (buttonState & MouseButtonState.Button2Released) != 0
                || (buttonState & MouseButtonState.Button3Released) != 0))
        {
            isButtonDoubleClicked = false;
            isButtonTripleClicked = true;
            ProcessButtonTripleClicked(mouseEvent);
            inputReady.Set();
            return;
        }

        //if (!isButtonPressed && !isButtonClicked && !isButtonDoubleClicked && !isButtonTripleClicked
        //	&& !isButtonReleased && lastMouseEvent.ButtonState != 0
        //	&& ((buttonState & MouseButtonState.Button1Released) == 0
        //	&& (buttonState & MouseButtonState.Button2Released) == 0
        //	&& (buttonState & MouseButtonState.Button3Released) == 0)) {
        //	ProcessButtonReleased (lastMouseEvent);
        //	inputReady.Set ();
        //	return;
        //}

        inputResultQueue.Enqueue(
            new InputResult()
            {
                EventType = EventType.Mouse,
                MouseEvent = mouseEvent
            });

        if (!isButtonClicked && !lastMouseEvent.ButtonState.HasFlag(MouseButtonState.ReportMousePosition)
                             && lastMouseEvent.Position != default && lastMouseEvent.Position == point
                             && ((buttonState & MouseButtonState.Button1Released) != 0
                                 || (buttonState & MouseButtonState.Button2Released) != 0
                                 || (buttonState & MouseButtonState.Button3Released) != 0))
        {
            isButtonClicked = true;
            ProcessButtonClicked(mouseEvent);
            Application.MainLoop.AddIdle(
                () =>
                {
                    Task.Run(
                        async () =>
                        {
                            await Task.Delay(300);
                            isButtonClicked = false;
                        });
                    return false;
                });
            inputReady.Set();
            return;
        }

        lastMouseEvent = mouseEvent;
        if (isButtonPressed && !isButtonClicked && !isButtonDoubleClicked && !isButtonTripleClicked &&
            !isProcContBtnPressedRuning)
        {
            //isButtonReleased = false;
            if ((buttonState & MouseButtonState.ReportMousePosition) != 0)
                point = new Point();
            else
                point = new Point()
                {
                    X = mouseEvent.Position.X,
                    Y = mouseEvent.Position.Y
                };
            if ((buttonState & MouseButtonState.ReportMousePosition) == 0)
                Application.MainLoop.AddIdle(
                    () =>
                    {
                        Task.Run(async () => await ProcessContinuousButtonPressedAsync());
                        return false;
                    });
        }

        inputReady.Set();
    }

    private void ProcessButtonClicked(MouseEvent mouseEvent)
    {
        var me = new MouseEvent()
        {
            Position = mouseEvent.Position,
            ButtonState = mouseEvent.ButtonState
        };
        if ((mouseEvent.ButtonState & MouseButtonState.Button1Released) != 0)
        {
            me.ButtonState &= ~MouseButtonState.Button1Released;
            me.ButtonState |= MouseButtonState.Button1Clicked;
        }
        else if ((mouseEvent.ButtonState & MouseButtonState.Button2Released) != 0)
        {
            me.ButtonState &= ~MouseButtonState.Button2Released;
            me.ButtonState |= MouseButtonState.Button2Clicked;
        }
        else if ((mouseEvent.ButtonState & MouseButtonState.Button3Released) != 0)
        {
            me.ButtonState &= ~MouseButtonState.Button3Released;
            me.ButtonState |= MouseButtonState.Button3Clicked;
        }
        //isButtonReleased = true;

        inputResultQueue.Enqueue(
            new InputResult()
            {
                EventType = EventType.Mouse,
                MouseEvent = me
            });
    }

    private async Task ProcessButtonDoubleClickedAsync()
    {
        await Task.Delay(300);
        isButtonDoubleClicked = false;
        buttonPressedCount = 0;
    }

    private void ProcessButtonDoubleClicked(MouseEvent mouseEvent)
    {
        var me = new MouseEvent()
        {
            Position = mouseEvent.Position,
            ButtonState = mouseEvent.ButtonState
        };
        if ((mouseEvent.ButtonState & MouseButtonState.Button1Pressed) != 0)
        {
            me.ButtonState &= ~MouseButtonState.Button1Pressed;
            me.ButtonState |= MouseButtonState.Button1DoubleClicked;
        }
        else if ((mouseEvent.ButtonState & MouseButtonState.Button2Pressed) != 0)
        {
            me.ButtonState &= ~MouseButtonState.Button2Pressed;
            me.ButtonState |= MouseButtonState.Button2DoubleClicked;
        }
        else if ((mouseEvent.ButtonState & MouseButtonState.Button3Pressed) != 0)
        {
            me.ButtonState &= ~MouseButtonState.Button3Pressed;
            me.ButtonState |= MouseButtonState.Button3DoubleClicked;
        }
        //isButtonReleased = true;

        inputResultQueue.Enqueue(
            new InputResult()
            {
                EventType = EventType.Mouse,
                MouseEvent = me
            });
    }

    private void ProcessButtonTripleClicked(MouseEvent mouseEvent)
    {
        var me = new MouseEvent()
        {
            Position = mouseEvent.Position,
            ButtonState = mouseEvent.ButtonState
        };
        if ((mouseEvent.ButtonState & MouseButtonState.Button1Pressed) != 0)
        {
            me.ButtonState &= ~MouseButtonState.Button1Pressed;
            me.ButtonState |= MouseButtonState.Button1TripleClicked;
        }
        else if ((mouseEvent.ButtonState & MouseButtonState.Button2Pressed) != 0)
        {
            me.ButtonState &= ~MouseButtonState.Button2Pressed;
            me.ButtonState |= MouseButtonState.Button2TrippleClicked;
        }
        else if ((mouseEvent.ButtonState & MouseButtonState.Button3Pressed) != 0)
        {
            me.ButtonState &= ~MouseButtonState.Button3Pressed;
            me.ButtonState |= MouseButtonState.Button3TripleClicked;
        }
        //isButtonReleased = true;

        inputResultQueue.Enqueue(
            new InputResult()
            {
                EventType = EventType.Mouse,
                MouseEvent = me
            });
    }

    private async Task ProcessContinuousButtonPressedAsync()
    {
        isProcContBtnPressedRuning = true;
        await Task.Delay(200);
        while (isButtonPressed)
        {
            await Task.Delay(100);
            var view = Application.wantContinuousButtonPressedView;
            if (view == null) break;
            if (isButtonPressed && (lastMouseEvent.ButtonState & MouseButtonState.ReportMousePosition) == 0)
            {
                inputResultQueue.Enqueue(
                    new InputResult()
                    {
                        EventType = EventType.Mouse,
                        MouseEvent = lastMouseEvent
                    });
                inputReady.Set();
            }
        }

        isProcContBtnPressedRuning = false;
        //isButtonPressed = false;
    }

    //void ProcessButtonReleased (MouseEvent mouseEvent)
    //{
    //	var me = new MouseEvent () {
    //		Position = mouseEvent.Position,
    //		ButtonState = mouseEvent.ButtonState
    //	};
    //	if ((mouseEvent.ButtonState & MouseButtonState.Button1Pressed) != 0) {
    //		me.ButtonState &= ~(MouseButtonState.Button1Pressed | MouseButtonState.ReportMousePosition);
    //		me.ButtonState |= MouseButtonState.Button1Released;
    //	} else if ((mouseEvent.ButtonState & MouseButtonState.Button2Pressed) != 0) {
    //		me.ButtonState &= ~(MouseButtonState.Button2Pressed | MouseButtonState.ReportMousePosition);
    //		me.ButtonState |= MouseButtonState.Button2Released;
    //	} else if ((mouseEvent.ButtonState & MouseButtonState.Button3Pressed) != 0) {
    //		me.ButtonState &= ~(MouseButtonState.Button3Pressed | MouseButtonState.ReportMousePosition);
    //		me.ButtonState |= MouseButtonState.Button3Released;
    //	}
    //	isButtonReleased = true;
    //	lastMouseEvent = me;

    //	inputResultQueue.Enqueue (new InputResult () {
    //		EventType = EventType.Mouse,
    //		MouseEvent = me
    //	});
    //}

    private ConsoleModifiers GetConsoleModifiers(uint keyChar)
    {
        switch (keyChar)
        {
            case 50:
                return ConsoleModifiers.Shift;
            case 51:
                return ConsoleModifiers.Alt;
            case 52:
                return ConsoleModifiers.Shift | ConsoleModifiers.Alt;
            case 53:
                return ConsoleModifiers.Control;
            case 54:
                return ConsoleModifiers.Shift | ConsoleModifiers.Control;
            case 55:
                return ConsoleModifiers.Alt | ConsoleModifiers.Control;
            case 56:
                return ConsoleModifiers.Shift | ConsoleModifiers.Alt | ConsoleModifiers.Control;
            default:
                return 0;
        }
    }

    private ConsoleKey GetConsoleKey(char keyChar, ref ConsoleModifiers mod, int length)
    {
        ConsoleKey key;
        switch (keyChar)
        {
            case 'A':
                key = ConsoleKey.UpArrow;
                break;
            case 'B':
                key = ConsoleKey.DownArrow;
                break;
            case 'C':
                key = ConsoleKey.RightArrow;
                break;
            case 'D':
                key = ConsoleKey.LeftArrow;
                break;
            case 'F':
                key = ConsoleKey.End;
                break;
            case 'H':
                key = ConsoleKey.Home;
                break;
            case 'P':
                key = ConsoleKey.F1;
                break;
            case 'Q':
                key = ConsoleKey.F2;
                break;
            case 'R':
                key = ConsoleKey.F3;
                break;
            case 'S':
                key = ConsoleKey.F4;
                break;
            case 'Z':
                key = ConsoleKey.Tab;
                mod |= ConsoleModifiers.Shift;
                break;
            case '0':
                key = ConsoleKey.F9;
                break;
            case '1':
                key = ConsoleKey.F10;
                break;
            case '2':
                key = ConsoleKey.Insert;
                break;
            case '3':
                if (length == 5)
                    key = ConsoleKey.F11;
                else
                    key = ConsoleKey.Delete;
                break;
            case '4':
                key = ConsoleKey.F12;
                break;
            case '5':
                if (length == 5)
                    key = ConsoleKey.F5;
                else
                    key = ConsoleKey.PageUp;
                break;
            case '6':
                key = ConsoleKey.PageDown;
                break;
            case '7':
                key = ConsoleKey.F6;
                break;
            case '8':
                key = ConsoleKey.F7;
                break;
            case '9':
                key = ConsoleKey.F8;
                break;
            default:
                key = 0;
                break;
        }

        return key;
    }

    public enum EventType
    {
        Key = 1,
        Mouse = 2,
        WindowSize = 3,
        WindowPosition = 4
    }

    [Flags]
    public enum MouseButtonState
    {
        Button1Pressed = 0x1,
        Button1Released = 0x2,
        Button1Clicked = 0x4,
        Button1DoubleClicked = 0x8,
        Button1TripleClicked = 0x10,
        Button2Pressed = 0x20,
        Button2Released = 0x40,
        Button2Clicked = 0x80,
        Button2DoubleClicked = 0x100,
        Button2TrippleClicked = 0x200,
        Button3Pressed = 0x400,
        Button3Released = 0x800,
        Button3Clicked = 0x1000,
        Button3DoubleClicked = 0x2000,
        Button3TripleClicked = 0x4000,
        ButtonWheeledUp = 0x8000,
        ButtonWheeledDown = 0x10000,
        ButtonWheeledLeft = 0x20000,
        ButtonWheeledRight = 0x40000,
        Button4Pressed = 0x80000,
        Button4Released = 0x100000,
        Button4Clicked = 0x200000,
        Button4DoubleClicked = 0x400000,
        Button4TripleClicked = 0x800000,
        ButtonShift = 0x1000000,
        ButtonCtrl = 0x2000000,
        ButtonAlt = 0x4000000,
        ReportMousePosition = 0x8000000,

        AllEvents = Button1Pressed | Button1Released | Button1Clicked | Button1DoubleClicked |
                    Button1TripleClicked | Button2Pressed | Button2Released | Button2Clicked |
                    Button2DoubleClicked | Button2TrippleClicked | Button3Pressed | Button3Released |
                    Button3Clicked | Button3DoubleClicked | Button3TripleClicked | ButtonWheeledUp |
                    ButtonWheeledDown | ButtonWheeledLeft | ButtonWheeledRight | Button4Pressed | Button4Released |
                    Button4Clicked | Button4DoubleClicked | Button4TripleClicked | ReportMousePosition
    }

    public struct MouseEvent
    {
        public Point Position;
        public MouseButtonState ButtonState;
    }

    public struct WindowSizeEvent
    {
        public Size Size;
    }

    public struct WindowPositionEvent
    {
        public int Top;
        public Point CursorPosition;
    }

    public struct InputResult
    {
        public EventType EventType;
        public ConsoleKeyInfo ConsoleKeyInfo;
        public MouseEvent MouseEvent;
        public WindowSizeEvent WindowSizeEvent;
        public WindowPositionEvent WindowPositionEvent;
    }
}
