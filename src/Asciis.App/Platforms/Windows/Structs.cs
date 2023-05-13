using System.Runtime.InteropServices;

namespace Asciis.App.Platforms;

public static partial class Windows
{
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct KeyEventRecord
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.Bool)]
        public bool IsKeyDown;

        [FieldOffset(4)]
        [MarshalAs(UnmanagedType.U2)]
        public ushort RepeatCount;

        [FieldOffset(6)]
        [MarshalAs(UnmanagedType.U2)]
        public VirtualKeyCode VirtualKeyCode;

        [FieldOffset(8)]
        [MarshalAs(UnmanagedType.U2)]
        public ushort VirtualScanCode;

        [FieldOffset(10)] 
        public char UnicodeChar;

        [FieldOffset(12)]
        [MarshalAs(UnmanagedType.U4)]
        public ControlKeyState ControlKeyState;

        public override string ToString()
        {
            string str = "";
            if (UnicodeChar == '\u001b')
                str = $"0x1b";
            else if (UnicodeChar >= 0 && UnicodeChar < ' ')
                str = $"{UnicodeChar:X2}";
            else 
                str = new string(UnicodeChar, 1);
            return $"KEY:{(IsKeyDown?"v":"^")}{VirtualKeyCode},{VirtualScanCode},{str}";
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MouseEventRecord
    {
        [FieldOffset(0)] public Coord Position;
        [FieldOffset(4)] public MouseButtonState ButtonState;
        [FieldOffset(8)] public ControlKeyState ControlKeyState;
        [FieldOffset(12)] public MouseEventFlags EventFlags;

        public override string ToString()
        {
            return $"[Mouse({Position},{ButtonState},{ControlKeyState},{EventFlags}";
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WindowBufferSizeRecord
    {
        public Coord size;

        public WindowBufferSizeRecord(short x, short y) { size = new Coord(x, y); }

        public override string ToString() { return $"[WindowBufferSize{size}"; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MenuEventRecord
    {
        public uint CommandId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FocusEventRecord
    {
        public uint SetFocus;
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct InputRecord
    {
        [FieldOffset(0)] public EventType EventType;
        [FieldOffset(4)] public KeyEventRecord KeyEvent;
        [FieldOffset(4)] public MouseEventRecord MouseEvent;
        [FieldOffset(4)] public WindowBufferSizeRecord WindowBufferSizeEvent;
        [FieldOffset(4)] public MenuEventRecord MenuEvent;
        [FieldOffset(4)] public FocusEventRecord FocusEvent;

        public override string ToString()
        {
            switch (EventType)
            {
                case EventType.Key:
                    return KeyEvent.ToString();
                case EventType.Focus:
                case EventType.Menu:
                case EventType.Mouse:
                case EventType.WindowBufferSize:
                    return EventType.ToString();
                default:
                    return "Unknown event type: " + EventType;
            }
        }
    };
    
    [StructLayout(LayoutKind.Sequential)]
    public struct ConsoleScreenBufferInfo
    {
        public Coord Size;
        public Coord CursorPosition;
        public ushort Attributes;
        public SmallRect WindowSize;
        public Coord MaximumWindowSize;

        public override string ToString()
        {
            return $"Console Screen Buffer: Size:{Size} Cursor:{CursorPosition} Attr:{Attributes:X} MaxWinSize:{MaximumWindowSize} WinSize:{WindowSize}";
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct ConsoleCursorInfo
    {
        public uint Size;
        public bool Visible;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Coord
    {
        public static readonly Coord Zero = new(0, 0);

        public short X;
        public short Y;

        public Coord(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public override string ToString() => $"({X},{Y})";
    };

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct CharUnion
    {
        [FieldOffset(0)] public char UnicodeChar;
        [FieldOffset(0)] public byte AsciiChar;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct CharInfo
    {
        [FieldOffset(0)] public CharUnion           Char;
        [FieldOffset(2)] public CharacterAttributes Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SmallRect
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;

        public SmallRect(short left, short top, short right, short bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static void MakeEmpty(ref SmallRect rect) { rect.Left = -1; }

        public static void Update(ref SmallRect rect, short col, short row)
        {
            if (rect.Left == -1)
            {
                //System.Diagnostics.Debugger.Log (0, "debug", $"damager From Empty {col},{row}\n");
                rect.Left = rect.Right = col;
                rect.Bottom = rect.Top = row;
                return;
            }

            if (col >= rect.Left && col <= rect.Right && row >= rect.Top && row <= rect.Bottom)
                return;
            if (col < rect.Left)
                rect.Left = col;
            if (col > rect.Right)
                rect.Right = col;
            if (row < rect.Top)
                rect.Top = row;
            if (row > rect.Bottom)
                rect.Bottom = row;
            //System.Diagnostics.Debugger.Log (0, "debug", $"Expanding {rect.ToString ()}\n");
        }

        public override string ToString() => $"Left={Left},Top={Top},Right={Right},Bottom={Bottom}";
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ConsoleKeyInfoEx
    {
        public ConsoleKeyInfo ConsoleKeyInfo;
        public bool CapsLock;
        public bool NumLock;

        public ConsoleKeyInfoEx(ConsoleKeyInfo consoleKeyInfo, bool capslock, bool numlock)
        {
            ConsoleKeyInfo = consoleKeyInfo;
            CapsLock = capslock;
            NumLock = numlock;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct SecurityAttributes
    {
        public int    nLength;
        public IntPtr lpSecurityDescriptor;
        public int    bInheritHandle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct StartupinfoEx
    {
        public Startupinfo StartupInfo;
        public IntPtr      lpAttributeList;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct Startupinfo
    {
        public int    cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public int    dwX;
        public int    dwY;
        public int    dwXSize;
        public int    dwYSize;
        public int    dwXCountChars;
        public int    dwYCountChars;
        public int    dwFillAttribute;
        public int    dwFlags;
        public short  wShowWindow;
        public short  cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessInformation
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int    dwProcessId;
        public int    dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ConsoleHistoryInfo
    {
        public ushort              Size;
        public ushort              HistoryBufferSize;
        public ushort              NumberOfHistoryBuffers;
        public ConsoleHistoryFlags Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ConsoleScreenBufferInfoEx
    {
        public uint                StructSize; // 96
        public Coord               ScreenSize;
        public Coord               CursorPosition;
        public CharacterAttributes Attributes;
        public SmallRect           Window;
        public Coord               MaximumWindowSize;

        public ushort PopupAttributes;
        public bool   IsFullscreenSupported;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public ColorRef[] ColorTable;

        public static ConsoleScreenBufferInfoEx Create() =>
            new ConsoleScreenBufferInfoEx { StructSize = (uint)Marshal.SizeOf<ConsoleScreenBufferInfoEx>() };

        public override string ToString() => 
            $"Screen Info( Size-{ScreenSize}, Cursor-{CursorPosition}, Window-{Window}, Max-{MaximumWindowSize} )";
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ColorRef
    {
        /// <summary>
        /// Color as 0x00BBGGRR
        /// </summary>
        public uint ColorDWORD;

        public ColorRef(Color color) { ColorDWORD = color.R + ((uint)color.G << 8) + ((uint)color.B << 16); }

        public ColorRef(int r, int g, int b) =>
            ColorDWORD = (uint)r + ((uint)g << 8) + ((uint)b << 16);

        public Color GetColor() =>
            Color.FromArgb(
                (int)(0x000000FFU & ColorDWORD),
                (int)(0x0000FF00U & ColorDWORD) >> 8,
                (int)(0x00FF0000U & ColorDWORD) >> 16);

        public void SetColor(Color color) =>
            ColorDWORD = color.R + ((uint)color.G << 8) + ((uint)color.B << 16);


        public void SetColor(uint rgbColor) =>
            ColorDWORD = ((rgbColor & 0x0000FF) << 16) +
                         (rgbColor & 0x00FF00) +
                         ((rgbColor & 0xFF0000) >> 16)

        ;
        public void SetColor(int r, int g, int b) =>
            ColorDWORD = (uint)r + ((uint)g << 8) + ((uint)b << 16);

        public void SetColor(string colorStr) =>
            ColorDWORD = Convert.ToUInt32(colorStr.Replace("#", ""), 16);

        public override string ToString() =>
            $"{ColorDWORD:x6}";
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ConsoleSelectionInfo
    {
        public ConsoleSelectionFlags Flags;
        public Coord                 SelectionAnchor;
        public SmallRect             Selection;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct ConsoleFontInfoEx
    {
        public uint               cbSize;
        public uint               Font;
        public Coord              FontSize;
        public FontPitchAndFamily FontFamily;
        public FontWeight         FontWeight;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FaceName;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct ConsoleFontInfo
    {
        public int   Font;
        public Coord FontSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ReadConsoleControl
    {
        public ulong nLength;
        public ulong nInitialChars;
        public ulong dwCtrlWakeupMask;
        public ulong dwControlKeyState;
    }
}