using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Asciis.UI.Terminals;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace Asciis.UI;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct ConsoleFontInfoEx
{
    public uint cbSize;
    public uint Font;
    public Coord FontSize;
    public FontPitchAndFamily FontFamily;
    public FontWeight FontWeight;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string FaceName;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct StartupinfoEx
{
    public Startupinfo StartupInfo;
    public IntPtr lpAttributeList;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct Startupinfo
{
    public int cb;
    public string lpReserved;
    public string lpDesktop;
    public string lpTitle;
    public int dwX;
    public int dwY;
    public int dwXSize;
    public int dwYSize;
    public int dwXCountChars;
    public int dwYCountChars;
    public int dwFillAttribute;
    public int dwFlags;
    public short wShowWindow;
    public short cbReserved2;
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
    public int dwProcessId;
    public int dwThreadId;
}

[StructLayout(LayoutKind.Sequential)]
public struct SecurityAttributes
{
    public int nLength;
    public IntPtr lpSecurityDescriptor;
    public int bInheritHandle;
}

[StructLayout(LayoutKind.Sequential)]
public struct Coord
{
    public short X;
    public short Y;

    public static readonly Coord Zero = new Coord(0, 0);

    public Coord(short x, short y)
    {
        X = x;
        Y = y;
    }
    public Coord(int x, int y)
    {
        X = (short)x;
        Y = (short)y;
    }

    public static bool operator ==(Coord a, Coord b) =>
        a.Equals(b);

    public static bool operator !=(Coord a, Coord b) =>
        !a.Equals(b);

    public bool Equals(Coord other) =>
        X == other.X
     && Y == other.Y;

    public override bool Equals(object? obj) =>
        obj is Coord other && Equals(other);

    public override int GetHashCode() =>
        // ReSharper disable NonReadonlyMemberInGetHashCode
        HashCode.Combine(X, Y);
    // ReSharper restore NonReadonlyMemberInGetHashCode

    public override string ToString() =>
        $"{X:d3},{Y:d3}";
}

[StructLayout(LayoutKind.Sequential)]
public struct ConsoleScreenBufferInfo
{
    public Coord Size;
    public Coord CursorPosition;
    public CharacterAttributes Attributes;
    public SmallRect Window;
    public Coord MaximumWindowSize;
}

[StructLayout(LayoutKind.Sequential)]
public struct ConsoleScreenBufferInfoEx
{
    public uint StructSize; // 96
    public Coord ScreenSize;
    public Coord CursorPosition;
    public CharacterAttributes Attributes;
    public SmallRect Window;
    public Coord MaximumWindowSize;

    public ushort PopupAttributes;
    public bool IsFullscreenSupported;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public ColorRef[] ColorTable;

    public static ConsoleScreenBufferInfoEx Create() =>
        new ConsoleScreenBufferInfoEx { StructSize = (uint)Marshal.SizeOf<ConsoleScreenBufferInfoEx>() };
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
public struct ConsoleFontInfo
{
    public int Font;
    public Coord FontSize;
}

[StructLayout(LayoutKind.Explicit)]
public struct InputRecord
{
    [FieldOffset(0)]
    public EventType EventType;

    [FieldOffset(4)]
    public KeyEventRecord KeyEvent;

    [FieldOffset(4)]
    public MouseEventRecord MouseEvent;

    [FieldOffset(4)]
    public WindowBufferSizeRecord WindowBufferSizeEvent;

    [FieldOffset(4)]
    public MenuEventRecord MenuEvent;

    [FieldOffset(4)]
    public FocusEventRecord FocusEvent;
}

[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
public struct KeyEventRecord
{
    [FieldOffset(0), MarshalAs(UnmanagedType.Bool)]
    public bool bKeyDown;
    [FieldOffset(4), MarshalAs(UnmanagedType.U2)]
    public ushort wRepeatCount;
    [FieldOffset(6), MarshalAs(UnmanagedType.U2)]
    public VirtualKeyCode wVirtualKeyCode;
    [FieldOffset(8), MarshalAs(UnmanagedType.U2)]
    public ushort wVirtualScanCode;
    [FieldOffset(10)]
    public char UnicodeChar;
    [FieldOffset(12), MarshalAs(UnmanagedType.U4)]
    public ControlKeyState dwControlKeyState;

    public override string ToString() =>
        $"KeyEvent: (KeyDown: {bKeyDown}, Repeat: {wRepeatCount}, VirtualKey: {wVirtualKeyCode}, ScanCode: {wVirtualScanCode}, Unicode: {UnicodeChar}, ControlState: {dwControlKeyState})";
}

[StructLayout(LayoutKind.Explicit)]
public struct MouseEventRecord
{
    [FieldOffset(0)]
    public Coord MousePosition;

    [FieldOffset(4)]
    public MouseButtonState ButtonState;

    [FieldOffset(8)]
    public ControlKeyState ControlKeyState;

    [FieldOffset(12)]
    public MouseEventFlags Flags;

    public override string ToString()
    {
        var sb = new StringBuilder($"MouseEventRecord ({MousePosition.X},{MousePosition.Y}) Button={ButtonState:X} Control={ControlKeyState:X} Flags={Flags}");

        return sb.ToString();
    }

}

[StructLayout(LayoutKind.Sequential)]
public struct WindowBufferSizeRecord
{
    public readonly Coord Size;

    public WindowBufferSizeRecord(short x, short y) =>
        Size = new Coord(x, y);

    public override string ToString() =>
        Size.ToString();
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
[StructLayout(LayoutKind.Sequential)]
public struct SmallRect
{
    public short Left;
    public short Top;
    public short Right;
    public short Bottom;

    public static readonly SmallRect Empty = new SmallRect(0, 0, 0, 0);

    public SmallRect(short left, short top, short right, short bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public bool Equals(SmallRect other) =>
        Left == other.Left
     && Top == other.Top
     && Right == other.Right
     && Bottom == other.Bottom;

    public static bool operator ==(SmallRect a, SmallRect b) =>
        a.Equals(b);

    public static bool operator !=(SmallRect a, SmallRect b) =>
        !(a == b);

    public override bool Equals(object? obj) =>
        obj is SmallRect other && Equals(other);

    public override int GetHashCode()
    {
        // ReSharper disable NonReadonlyMemberInGetHashCode
        return HashCode.Combine(Left, Top, Right, Bottom);
        // ReSharper restore NonReadonlyMemberInGetHashCode
    }

    public override string ToString() =>
        $"{Left:d3},{Top:d3},{Right:d3},{Bottom:d3}";
}

[StructLayout(LayoutKind.Sequential)]
public struct ConsoleCursorInfo
{
    public uint Size;
    public bool Visible;
}

[StructLayout(LayoutKind.Sequential)]
public struct ConsoleHistoryInfo
{
    public ushort Size;
    public ushort HistoryBufferSize;
    public ushort NumberOfHistoryBuffers;
    public ConsoleHistoryFlags Flags;
}

[StructLayout(LayoutKind.Sequential)]
public struct ConsoleSelectionInfo
{
    public ConsoleSelectionFlags Flags;
    public Coord SelectionAnchor;
    public SmallRect Selection;
}

[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
public struct TerminalChar
{
    [FieldOffset(0)]
    public ushort UnicodeChar;

    [FieldOffset(2)]
    public CharacterAttributes Attributes;
}
