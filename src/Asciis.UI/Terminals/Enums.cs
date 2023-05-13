using System.Diagnostics.CodeAnalysis;

namespace Asciis.UI.Terminals;

[Flags]
public enum FormatMessageFlags
{
    FormatMessageFromSystem = 0x00001000
}

public enum StandardHandle : int
{
    Input = -10,
    Output = -11,
    Error = -12
}

// Enumerated type for the control messages sent to the handler routine
public enum CtrlTypes : uint
{
    CtrlCEvent = 0,
    CtrlBreakEvent = 1,
    CtrlCloseEvent = 2,
    CtrlLogoffEvent = 5,
    CtrlShutdownEvent = 6
}

public enum EventType : ushort
{
    Key = 0x01,
    Mouse = 0x02,
    WindowBufferSize = 0x04,
    Menu = 0x08,
    Focus = 0x10,
}

[Flags]
public enum MouseEventFlags : uint
{
    MouseMoved = 0x01,
    DoubleClick = 0x02,
    MouseWheeled = 0x04,
    MouseHorizontalWheeled = 0x08
}

[Flags]
public enum ConsoleInputModes : uint
{
    EnableProcessedInput = 0x0001,
    EnableLineInput = 0x0002,
    EnableEchoInput = 0x0004,
    EnableWindowInput = 0x0008,
    EnableMouseInput = 0x0010,
    EnableInsertMode = 0x0020,
    EnableQuickEditMode = 0x0040,
    EnableExtendedFlags = 0x0080,
    EnableAutoPosition = 0x0100,
    EnableVirtualTerminalInput = 0x0200
}

[Flags]
public enum ConsoleOutputModes : uint
{
    EnableProcessedOutput = 0x0001,
    EnableWrapAtEolOutput = 0x0002,
    EnableVirtualTerminalProcessing = 0x0004,
    DisableNewlineAutoReturn = 0x0008,
    EnableLvbGridWorldwide = 0x0010
}

[Flags]
public enum CharacterAttributes : ushort
{
    FgBlack = 0x0000,
    FgDarkBlue = 0x0001,
    FgDarkGreen = 0x0002,
    FgDarkCyan = 0x0003,
    FgDarkRed = 0x0004,
    FgDarkMagenta = 0x0005,
    FgDarkYellow = 0x0006,
    FgGrey = 0x0007,
    FgDarkGrey = 0x0008,
    FgBlue = 0x0009,
    FgGreen = 0x000A,
    FgCyan = 0x000B,
    FgRed = 0x000C,
    FgMagenta = 0x000D,
    FgYellow = 0x000E,
    FgWhite = 0x000F,

    BgBlack = 0x0000,
    BgDarkBlue = 0x0010,
    BgDarkGreen = 0x0020,
    BgDarkCyan = 0x0030,
    BgDarkRed = 0x0040,
    BgDarkMagenta = 0x0050,
    BgDarkYellow = 0x0060,
    BgGrey = 0x0070,
    BgDarkGrey = 0x0080,
    BgBlue = 0x0090,
    BgGreen = 0x00A0,
    BgCyan = 0x00B0,
    BgRed = 0x00C0,
    BgMagenta = 0x00D0,
    BgYellow = 0x00E0,
    BgWhite = 0x00F0,

    LeadingByte = 0x0100,
    TrailingByte = 0x0200,
    GridHorizontal = 0x0400,
    GridLVertical = 0x0800,
    GridRVertical = 0x1000,
    ReverseVideo = 0x4000,
    Underscore = 0x8000
}

public enum PixelType
{
    UpperHalfBlock = 0x2580,
    LowerHalfBlock = 0x2584,
    Solid = 0x2588,
    LeftHalfBlock = 0x258C,
    RightHalfBlock = 0x2590,
    LightShade = 0x2591,
    MediumShade = 0x2592,
    DarkShade = 0x2593,
    SolidSquare = 0x25A0,
    SolidRectangle = 0x25AC,
    TriangleUp = 0x25B2,
    TriangleRight = 0x25BA,
    TriangleDown = 0x25BC,
    TriangleLeft = 0x25C4, // ◄
    Lazenge = 0x25CA, // ◊
}

[Flags]
public enum PseudoConsoleFlags : ulong
{
    PseudoconsoleInheritCursor = 1
}


[Flags]
public enum DesiredAccess : uint
{
    GenericRead = 0x8000_0000,
    GenericWrite = 0x4000_0000,
    ReadWrite = GenericRead | GenericWrite
}

public enum FontWeight
{
    Dontcare = 0,
    Thin = 100,
    Extralight = 200,
    Light = 300,
    Normal = 400,
    Medium = 500,
    Semibold = 600,
    Bold = 700,
    Extrabold = 800,
    Heavy = 900
}

public enum FontCharSet : byte
{
    AnsiCharset = 0,
    DefaultCharset = 1,
    SymbolCharset = 2,
    ShiftjisCharset = 128,
    HangeulCharset = 129,
    HangulCharset = 129,
    Gb2312Charset = 134,
    Chinesebig5Charset = 136,
    OemCharset = 255,
    JohabCharset = 130,
    HebrewCharset = 177,
    ArabicCharset = 178,
    GreekCharset = 161,
    TurkishCharset = 162,
    VietnameseCharset = 163,
    ThaiCharset = 222,
    EasteuropeCharset = 238,
    RussianCharset = 204,
    MacCharset = 77,
    BalticCharset = 186
}

public enum FontPrecision : byte
{
    OutDefaultPrecis = 0,
    OutStringPrecis = 1,
    OutCharacterPrecis = 2,
    OutStrokePrecis = 3,
    OutTtPrecis = 4,
    OutDevicePrecis = 5,
    OutRasterPrecis = 6,
    OutTtOnlyPrecis = 7,
    OutOutlinePrecis = 8,
    OutScreenOutlinePrecis = 9,
    OutPsOnlyPrecis = 10
}

[Flags]
public enum FontClipPrecision : byte
{
    ClipDefaultPrecis = 0x00,
    ClipCharacterPrecis = 0x01,
    ClipStrokePrecis = 0x02,
    ClipMask = 0x0f,
    ClipLhAngles = 0x10,
    ClipTtAlways = 0x20,
    ClipDfaDisable = 0x40,
    ClipEmbedded = 0x80
}

public enum FontQuality : byte
{
    DefaultQuality = 0,
    DraftQuality = 1,
    ProofQuality = 2,
    NonantialiasedQuality = 3,
    AntialiasedQuality = 4,
    CleartypeQuality = 5,
    CleartypeNaturalQuality = 6
}

[Flags]
public enum FontPitchAndFamily : byte
{
    DefaultPitch = 0x00,
    FixedPitch = 0x01,
    VariablePitch = 0x02,

    FfDontcare = 0x00,
    FfRoman = 0x10,
    FfSwiss = 0x20,
    FfModern = 0x30,
    FfScript = 0x40,
    FfDecorative = 0x50
}

[Flags]
public enum ControlKeyState : uint
{
    RightAltPressed = 0x001,
    LeftAltPressed = 0x002,
    RightCtrlPressed = 0x004,
    LeftCtrlPressed = 0x008,
    ShiftPressed = 0x010,
    NumlockOn = 0x020,
    ScrolllockOn = 0x040,
    CapslockOn = 0x080,
    EnhancedKey = 0x100,
}

[Flags]
public enum MouseButtonState : uint
{
    FromLeft1StButtonPressed = 0x0001, // The leftmost mouse button.
    RightmostButtonPressed = 0x0002, // The rightmost mouse button.
    FromLeft2NdButtonPressed = 0x0004, // The second button from the left.
    FromLeft3RdButtonPressed = 0x0008, // The third button from the left.
    FromLeft4ThButtonPressed = 0x0010, // The fourth button from the left.
}

[Flags]
public enum ConsoleSelectionFlags : uint
{
    ConsoleNoSelection = 0x0000, // No selection
    ConsoleSelectionInProgress = 0x0001, // Selection has begun
    ConsoleSelectionNotEmpty = 0x0002, // Selection rectangle is not empty
    ConsoleMouseSelection = 0x0004, // Selecting with the mouse
    ConsoleMouseDown = 0x0008, // Mouse is down
}

public enum ConsoleHistoryFlags : uint
{
    HistoryNoDupFlag = 0x01
}

public enum ButtonState : uint
{
    FromLeft1StButtonPressed = 0x0001, // The leftmost mouse button.
    RightmostButtonPressed = 0x0002, // The rightmost mouse button.
    FromLeft2NdButtonPressed = 0x0004, // The second button from the left.
    FromLeft3RdButtonPressed = 0x0008, // The third button from the left.
    FromLeft4ThButtonPressed = 0x0010, // The fourth button from the left.
}

/// <summary>
/// Just like ConsoleKey, but better
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Because")]
public enum VirtualKeyCode : ushort
{
    VK_LBUTTON = 0x01, // Left mouse button
    VK_RBUTTON = 0x02, // Right mouse button
    VK_CANCEL = 0x03, // Control-break processing
    VK_MBUTTON = 0x04, // Middle mouse button (three-button mouse)
    VK_XBUTTON1 = 0x05, // X1 mouse button
    VK_XBUTTON2 = 0x06, // X2 mouse button
    VK_UNDEFINED1 = 0x07, // Undefined
    VK_BACK = 0x08, // BACKSPACE key
    VK_TAB = 0x09, // TAB key
    VK_UNDEFINED2 = 0x0A, //Reserved
    VK_UNDEFINED3 = 0x0B, //Reserved
    VK_CLEAR = 0x0C, // CLEAR key
    VK_RETURN = 0x0D, // ENTER key
    VK_UNDEFINED4 = 0x0E, // Undefined
    VK_UNDEFINED5 = 0x0F, // Undefined

    VK_SHIFT = 0x10, // SHIFT key
    VK_CONTROL = 0x11, // CTRL key
    VK_MENU = 0x12, // ALT key
    VK_PAUSE = 0x13, // PAUSE key
    VK_CAPITAL = 0x14, //CAPS LOCK key
    VK_KANA = 0x15, //IME Kana mode
    VK_HANGUEL = 0x15, //IME Hanguel mode (maintained for compatibility; use VK_HANGUL)
    VK_HANGUL = 0x15, // IME Hangul mode
    VK_IME_ON = 0x16, // IME On
    VK_JUNJA = 0x17, // IME Junja mode
    VK_FINAL = 0x18, // IME final mode
    VK_HANJA = 0x19, // IME Hanja mode
    VK_KANJI = 0x19, // IME Kanji mode
    VK_IME_OFF = 0x1A, // IME Off
    VK_ESCAPE = 0x1B, // ESC key
    VK_CONVERT = 0x1C, // IME convert
    VK_NONCONVERT = 0x1D, // IME nonconvert
    VK_ACCEPT = 0x1E, // IME accept
    VK_MODECHANGE = 0x1F, // IME mode change request

    VK_SPACE = 0x20, // SPACEBAR
    VK_PRIOR = 0x21, // PAGE UP key
    VK_NEXT = 0x22, // PAGE DOWN key
    VK_END = 0x23, // END key
    VK_HOME = 0x24, // HOME key
    VK_LEFT = 0x25, // LEFT ARROW key
    VK_UP = 0x26, // UP ARROW key
    VK_RIGHT = 0x27, // RIGHT ARROW key
    VK_DOWN = 0x28, // DOWN ARROW key
    VK_SELECT = 0x29, // SELECT key
    VK_PRINT = 0x2A, // PRINT key
    VK_EXECUTE = 0x2B, // EXECUTE key
    VK_SNAPSHOT = 0x2C, // PRINT SCREEN key
    VK_INSERT = 0x2D, // INS key
    VK_DELETE = 0x2E, // DEL key
    VK_HELP = 0x2F, // HELP key

    vk_0 = 0x30, // 0 key
    VK_1 = 0x31, // 1 KEY
    VK_2 = 0x32, // 2 KEY
    VK_3 = 0x33, // 3 KEY
    VK_4 = 0x34, // 4 KEY
    VK_5 = 0x35, // 5 KEY
    VK_6 = 0x36, // 6 KEY
    VK_7 = 0x37, // 7 KEY
    VK_8 = 0x38, // 8 KEY
    VK_9 = 0x39, // 9 KEY   - 0x3A-40 Undefined

    VK_A = 0x41, // A key
    VK_B = 0x42, // B key
    VK_C = 0x43, // C key
    VK_D = 0x44, // D key
    VK_E = 0x45, // E key
    VK_F = 0x46, // F key
    VK_G = 0x47, // G key
    VK_H = 0x48, // H key
    VK_I = 0x49, // I key
    VK_J = 0x4A, // J key
    VK_K = 0x4B, // K key
    VK_L = 0x4C, // L key
    VK_M = 0x4D, // M key
    VK_N = 0x4E, // N key
    VK_O = 0x4F, // O key

    VK_P = 0x50, // P key
    VK_Q = 0x51, // Q key
    VK_R = 0x52, // R key
    VK_S = 0x53, // S key
    VK_T = 0x54, // T key
    VK_U = 0x55, // U key
    VK_V = 0x56, // V key
    VK_W = 0x57, // W key
    VK_X = 0x58, // X key
    VK_Y = 0x59, // Y key
    VK_Z = 0x5A, // Z key
    VK_LWIN = 0x5B, // Left Windows key (Natural keyboard)
    VK_RWIN = 0x5C, // Right Windows key (Natural keyboard)
    VK_APPS = 0x5D, // Applications key (Natural keyboard)
    VK_UNDEFINED6 = 0x5E, // Reserved
    VK_SLEEP = 0x5F, // Computer Sleep key

    VK_NUMPAD0 = 0x60, // Numeric keypad 0 key
    VK_NUMPAD1 = 0x61, // Numeric keypad 1 key
    VK_NUMPAD2 = 0x62, // Numeric keypad 2 key
    VK_NUMPAD3 = 0x63, // Numeric keypad 3 key
    VK_NUMPAD4 = 0x64, // Numeric keypad 4 key
    VK_NUMPAD5 = 0x65, // Numeric keypad 5 key
    VK_NUMPAD6 = 0x66, // Numeric keypad 6 key
    VK_NUMPAD7 = 0x67, // Numeric keypad 7 key
    VK_NUMPAD8 = 0x68, // Numeric keypad 8 key
    VK_NUMPAD9 = 0x69, // Numeric keypad 9 key
    VK_MULTIPLY = 0x6A, // Multiply key
    VK_ADD = 0x6B, // Add key
    VK_SEPARATOR = 0x6C, // Separator key
    VK_SUBTRACT = 0x6D, // Subtract key
    VK_DECIMAL = 0x6E, // Decimal key
    VK_DIVIDE = 0x6F, // Divide key

    VK_F1 = 0x70, // F1 key
    VK_F2 = 0x71, // F2 key
    VK_F3 = 0x72, // F3 key
    VK_F4 = 0x73, // F4 key
    VK_F5 = 0x74, // F5 key
    VK_F6 = 0x75, // F6 key
    VK_F7 = 0x76, // F7 key
    VK_F8 = 0x77, // F8 key
    VK_F9 = 0x78, // F9 key
    VK_F10 = 0x79, // F10 key
    VK_F11 = 0x7A, // F11 key
    VK_F12 = 0x7B, // F12 key
    VK_F13 = 0x7C, // F13 key
    VK_F14 = 0x7D, // F14 key
    VK_F15 = 0x7E, // F15 key
    VK_F16 = 0x7F, // F16 key

    VK_F17 = 0x80, // F17 key
    VK_F18 = 0x81, // F18 key
    VK_F19 = 0x82, // F19 key
    VK_F20 = 0x83, // F20 key
    VK_F21 = 0x84, // F21 key
    VK_F22 = 0x85, // F22 key
    VK_F23 = 0x86, // F23 key
    VK_F24 = 0x87, // F24 key = 0x88-8F,           // Unassigned

    VK_NUMLOCK = 0x90, // NUM LOCK key
    VK_SCROLL = 0x91, // SCROLL LOCK key
                      // = 0x92-96, OEM specific
                      // = 0x97-9F, Unassigned

    VK_LSHIFT = 0xA0, // Left SHIFT key
    VK_RSHIFT = 0xA1, // Right SHIFT key
    VK_LCONTROL = 0xA2, // Left CONTROL key
    VK_RCONTROL = 0xA3, // Right CONTROL key
    VK_LMENU = 0xA4, // Left MENU key
    VK_RMENU = 0xA5, // Right MENU key
    VK_BROWSER_BACK = 0xA6, // Browser Back key
    VK_BROWSER_FORWARD = 0xA7, // Browser Forward key
    VK_BROWSER_REFRESH = 0xA8, // Browser Refresh key
    VK_BROWSER_STOP = 0xA9, // Browser Stop key
    VK_BROWSER_SEARCH = 0xAA, // Browser Search key
    VK_BROWSER_FAVORITES = 0xAB, // Browser Favorites key
    VK_BROWSER_HOME = 0xAC, // Browser Start and Home key
    VK_VOLUME_MUTE = 0xAD, // Volume Mute key
    VK_VOLUME_DOWN = 0xAE, // Volume Down key
    VK_VOLUME_UP = 0xAF, // Volume Up key

    VK_MEDIA_NEXT_TRACK = 0xB0, // Next Track key
    VK_MEDIA_PREV_TRACK = 0xB1, // Previous Track key
    VK_MEDIA_STOP = 0xB2, // Stop Media key
    VK_MEDIA_PLAY_PAUSE = 0xB3, // Play/Pause Media key
    VK_LAUNCH_MAIL = 0xB4, // Start Mail key
    VK_LAUNCH_MEDIA_SELECT = 0xB5, // Select Media key
    VK_LAUNCH_APP1 = 0xB6, // Start Application 1 key
    VK_LAUNCH_APP2 = 0xB7, // Start Application 2 key
                           // - 0xB8-B9, //  Reserved
    VK_OEM_1 = 0xBA, // Used for miscellaneous characters; it can vary by keyboard.
                     // For the US standard keyboard, the ';:' key
    VK_OEM_PLUS = 0xBB, // For any country/region, the '+' key
    VK_OEM_COMMA = 0xBC, // For any country/region, the ',' key
    VK_OEM_MINUS = 0xBD, // For any country/region, the '-' key
    VK_OEM_PERIOD = 0xBE, // For any country/region, the '.' key
    VK_OEM_2 = 0xBF, // Used for miscellaneous characters; it can vary by keyboard.

    // For the US standard keyboard, the '/?' key
    VK_OEM_3 = 0xC0, // Used for miscellaneous characters; it can vary by keyboard.
                     // For the US standard keyboard, the '`~' key
                     // =0xC1-D7, // Reserved

    // =0xD8-DA, // Unassigned
    VK_OEM_4 = 0xDB, // Used for miscellaneous characters; it can vary by keyboard.
    VK_OEM_5 = 0xDC, // For the US standard keyboard, the '[{' key
    VK_OEM_6 = 0xDD, // For the US standard keyboard, the '\|' key
    VK_OEM_7 = 0xDE, // For the US standard keyboard, the ']}' key
    VK_OEM_8 = 0xDF, // For the US standard keyboard, the 'single-quote/double-quote' key

    // =0xE0, // Reserved
    // =0xE1, // OEM specific
    VK_OEM_102 = 0xE2, // Either the angle bracket key or the backslash key on the RT 102-key keyboard
                       // =0xE3-E4, // OEM specific
    VK_PROCESSKEY = 0xE5, // IME PROCESS key
                          // =0xE6, // OEM specific
    VK_PACKET = 0xE7, // Used to pass Unicode characters as if they were keystrokes.
                      // The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods.For more

    // information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
    // =-0xE8,    // Unassigned
    // =0xE9-F5, // OEM specific

    VK_ATTN = 0xF6, // Attn key
    VK_CRSEL = 0xF7, // CrSel key
    VK_EXSEL = 0xF8, // ExSel key
    VK_EREOF = 0xF9, // Erase EOF key
    VK_PLAY = 0xFA, // Play key
    VK_ZOOM = 0xFB, // Zoom key
    VK_NONAME = 0xFC, // Reserved
    VK_PA1 = 0xFD, // PA1 key
    VK_OEM_CLEAR = 0xFE, // Clear k
}
