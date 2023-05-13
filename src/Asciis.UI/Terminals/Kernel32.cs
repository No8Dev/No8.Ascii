using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32.SafeHandles;

namespace Asciis.UI.Terminals;

public static partial class Windows
{
    public static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

    // http://pinvoke.net/default.aspx/kernel32/AddConsoleAlias.html
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool AddConsoleAlias(string source,
                                               string target,
                                               string exeName);

    // http://pinvoke.net/default.aspx/kernel32/AllocConsole.html
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AllocConsole();

    // http://pinvoke.net/default.aspx/kernel32/AttachConsole.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool AttachConsole(uint processId);


    // https://docs.microsoft.com/en-us/windows/console/closepseudoconsole
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void ClosePseudoConsole(IntPtr hConsole);

    // https://docs.microsoft.com/en-us/windows/console/createpseudoconsole
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern int CreatePseudoConsole(Coord size,
                                                  IntPtr hInput,
                                                  IntPtr hOutput,
                                                  PseudoConsoleFlags flags,
                                                  out IntPtr hConsole);

    // https://docs.microsoft.com/en-us/windows/console/resizepseudoconsole
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern int ResizePseudoConsole(IntPtr hConsole, Coord size);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool CreatePipe(out IntPtr hReadPipe, out IntPtr hWritePipe, IntPtr lpPipeAttributes, int nSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WriteFile(IntPtr hFile,
                                           byte[] lpBuffer,
                                           uint nNumberOfBytesToWrite,
                                           out uint lpNumberOfBytesWritten,
                                           IntPtr lpOverlapped);

    // http://pinvoke.net/default.aspx/kernel32/CreateConsoleScreenBuffer.html
    [Obsolete("NOT Virtual Terminal compliant", UrlFormat = "https://docs.microsoft.com/en-us/windows/console/createconsolescreenbuffer")]
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr CreateConsoleScreenBuffer(DesiredAccess desiredAccess,
                                                                          System.IO.FileShare shareMode,
                                                                          IntPtr securityAttributes,
                                                                          uint flags,
                                                                          IntPtr screenBufferData);

    // http://pinvoke.net/default.aspx/kernel32/FillConsoleOutputAttribute.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool FillConsoleOutputAttribute(IntPtr consoleOutput,
                                                          ushort attribute,
                                                          uint length,
                                                          Coord writeCoord,
                                                          out uint numberOfAttrsWritten);

    // http://pinvoke.net/default.aspx/kernel32/FillConsoleOutputCharacter.html
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool FillConsoleOutputCharacter(IntPtr consoleOutput,
                                                          char character,
                                                          uint length,
                                                          Coord writeCoord,
                                                          out uint numberOfCharsWritten);

    // http://pinvoke.net/default.aspx/kernel32/FlushConsoleInputBuffer.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool FlushConsoleInputBuffer(IntPtr consoleInput);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern uint FormatMessage(uint flags,
                                             IntPtr source,
                                             uint messageId,
                                             uint languageId,
                                             [Out] StringBuilder buffer,
                                             uint size,
                                             string[]? arguments);

    // http://pinvoke.net/default.aspx/kernel32/FreeConsole.html
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    public static extern bool FreeConsole();

    // http://pinvoke.net/default.aspx/kernel32/GenerateConsoleCtrlEvent.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GenerateConsoleCtrlEvent(uint ctrlEvent,
                                                        uint processGroupId);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleAlias.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleAlias(string source,
                                               out StringBuilder targetBuffer,
                                               uint targetBufferLength,
                                               string exeName);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleAliases.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetConsoleAliases(StringBuilder[] targetBuffer,
                                                 uint targetBufferLength,
                                                 string exeName);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleAliasesLength.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetConsoleAliasesLength(string exeName);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleAliasExes.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetConsoleAliasExes(out StringBuilder exeNameBuffer,
                                                   uint exeNameBufferLength);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleAliasExesLength.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetConsoleAliasExesLength();

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleCP.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetConsoleCP();

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleCursorInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleCursorInfo(IntPtr consoleOutput,
                                                    out ConsoleCursorInfo consoleCursorInfo);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleDisplayMode.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleDisplayMode(out uint modeFlags);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleFontSize.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern Coord GetConsoleFontSize(IntPtr consoleOutput,
                                                                                    int font);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleHistoryInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleHistoryInfo(out ConsoleHistoryInfo consoleHistoryInfo);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleMode.html
    [DllImport("kernel32.dll", EntryPoint = "GetConsoleMode", SetLastError = true)]
    public static extern bool GetConsoleInputMode(IntPtr consoleHandle,
                                                   out ConsoleInputModes mode);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleMode.html
    [DllImport("kernel32.dll", EntryPoint = "GetConsoleMode", SetLastError = true)]
    public static extern bool GetConsoleOutputMode(IntPtr consoleHandle,
                                                    out ConsoleOutputModes mode);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleOriginalTitle.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetConsoleOriginalTitle(out StringBuilder consoleTitle,
                                                       uint size);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleOutputCP.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetConsoleOutputCP();

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleProcessList.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetConsoleProcessList(out uint[] processList,
                                                     uint processCount);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleScreenBufferInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleScreenBufferInfo(IntPtr consoleOutput,
                                                          out ConsoleScreenBufferInfo consoleScreenBufferInfo);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleScreenBufferInfoEx.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleScreenBufferInfoEx(IntPtr consoleOutput,
                                                            ref ConsoleScreenBufferInfoEx consoleScreenBufferInfo);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleSelectionInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleSelectionInfo([Out] out ConsoleSelectionInfo consoleSelectionInfo);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleTitle.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetConsoleTitle([Out] StringBuilder consoleTitle,
                                               uint size);

    // http://pinvoke.net/default.aspx/kernel32/GetConsoleWindow.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetConsoleWindow();

    // http://pinvoke.net/default.aspx/kernel32/GetCurrentConsoleFont.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetCurrentConsoleFont(IntPtr consoleOutput,
                                                     bool maximumWindow,
                                                     ref ConsoleFontInfo consoleCurrentFont);

    // http://pinvoke.net/default.aspx/kernel32/GetCurrentConsoleFontEx.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetCurrentConsoleFontEx(IntPtr consoleOutput,
                                                       bool maximumWindow,
                                                       ref ConsoleFontInfoEx consoleCurrentFont);

    // http://pinvoke.net/default.aspx/kernel32/GetLargestConsoleWindowSize.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern Coord GetLargestConsoleWindowSize(IntPtr consoleOutput);

    // http://pinvoke.net/default.aspx/kernel32/GetNumberOfConsoleInputEvents.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetNumberOfConsoleInputEvents(IntPtr consoleInput,
                                                             out uint numberOfEvents);

    // http://pinvoke.net/default.aspx/kernel32/GetNumberOfConsoleMouseButtons.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetNumberOfConsoleMouseButtons(ref uint numberOfMouseButtons);

    // http://pinvoke.net/default.aspx/kernel32/GetStdHandle.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(StandardHandle stdHandle);

    // http://pinvoke.net/default.aspx/kernel32/GetStdHandle.html
    [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true)]
    public static extern SafeFileHandle GetSafeStdHandle(int stdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool InitializeProcThreadAttributeList(
        IntPtr lpAttributeList,
        int dwAttributeCount,
        int dwFlags,
        ref IntPtr lpSize);

    // http://pinvoke.net/default.aspx/kernel32/PeekConsoleInput.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool PeekConsoleInput(IntPtr consoleInput,
                                                [ Out ]
                                                    InputRecord[] buffer,
                                                uint length,
                                                out uint numberOfEventsRead);

    // http://pinvoke.net/default.aspx/kernel32/ReadConsole.html
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool ReadConsole(IntPtr consoleInput,
                                           [ Out ]
                                               StringBuilder buffer,
                                           uint numberOfCharsToRead,
                                           out uint numberOfCharsRead,
                                           IntPtr reserved);

    // http://pinvoke.net/default.aspx/kernel32/ReadConsoleInput.html
    [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool ReadConsoleInput(IntPtr consoleInput,
                                                [ Out ]
                                                    InputRecord[] buffer,
                                                uint length,
                                                out uint numberOfEventsRead);

    // http://pinvoke.net/default.aspx/kernel32/ReadConsoleOutput.html
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool ReadConsoleOutput(IntPtr consoleOutput,
                                                 [ Out ] [ MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 2 ) ]
                                                     TerminalChar[ , ] buffer,
                                                 Coord bufferSize,
                                                 Coord bufferCoord,
                                                 ref SmallRect readRegion);

    // http://pinvoke.net/default.aspx/kernel32/ReadConsoleOutputAttribute.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadConsoleOutputAttribute(IntPtr consoleOutput,
                                                          [Out] ushort[] attribute,
                                                          uint length,
                                                          Coord readCoord,
                                                          out uint numberOfAttrsRead);

    // http://pinvoke.net/default.aspx/kernel32/ReadConsoleOutputCharacter.html
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool ReadConsoleOutputCharacter(IntPtr consoleOutput,
                                                          [ Out ]
                                                              StringBuilder character,
                                                          uint length,
                                                          Coord readCoord,
                                                          out uint numberOfCharsRead);

    // http://pinvoke.net/default.aspx/kernel32/ScrollConsoleScreenBuffer.html
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool ScrollConsoleScreenBuffer(IntPtr consoleOutput,
                                                         [In] ref SmallRect scrollRectangle,
                                                         IntPtr clipRectangle,
                                                         Coord destinationOrigin,
                                                         [In] ref TerminalChar fill);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleActiveScreenBuffer.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleActiveScreenBuffer(IntPtr consoleOutput);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleCP.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleCP(uint codePageId);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleCtrlHandler.html
    [DllImport("kernel32.dll")]
    public static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate handlerRoutine,
                                                     bool add);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleCursorInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleCursorInfo(IntPtr consoleOutput,
                                                    [In] ref ConsoleCursorInfo consoleCursorInfo);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleCursorPosition.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleCursorPosition(IntPtr consoleOutput,
                                                        Coord cursorPosition);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleDisplayMode.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleDisplayMode(IntPtr consoleOutput,
                                                     uint flags,
                                                     out Coord newScreenBufferDimensions);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleHistoryInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleHistoryInfo(ConsoleHistoryInfo consoleHistoryInfo);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleMode.html
    [DllImport("kernel32.dll", EntryPoint = "SetConsoleMode", SetLastError = true)]
    public static extern bool SetConsoleInputMode(IntPtr consoleHandle,
                                                   ConsoleInputModes mode);
    // http://pinvoke.net/default.aspx/kernel32/SetConsoleMode.html
    [DllImport("kernel32.dll", EntryPoint = "SetConsoleMode", SetLastError = true)]
    public static extern bool SetConsoleOutputMode(IntPtr consoleHandle,
                                                    ConsoleOutputModes mode);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleOutputCP.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleOutputCP(uint codePageId);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleScreenBufferInfoEx.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleScreenBufferInfoEx(IntPtr consoleOutput,
                                                            ConsoleScreenBufferInfoEx consoleScreenBufferInfoEx);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleScreenBufferSize.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleScreenBufferSize(IntPtr consoleOutput,
                                                         Coord size);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleTextAttribute.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleTextAttribute(IntPtr consoleOutput,
                                                      ushort attributes);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleTitle.html
    [DllImport("kernel32.dll")]
    public static extern bool SetConsoleTitle(string consoleTitle);

    // http://pinvoke.net/default.aspx/kernel32/SetConsoleWindowInfo.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleWindowInfo(IntPtr consoleOutput,
                                                    bool absolute,
                                                    [In] ref SmallRect consoleWindow);

    // http://pinvoke.net/default.aspx/kernel32/SetCurrentConsoleFontEx.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetCurrentConsoleFontEx(IntPtr consoleOutput,
                                                       bool maximumWindow,
                                                       ref ConsoleFontInfoEx consoleCurrentFontEx);

    // http://pinvoke.net/default.aspx/kernel32/SetStdHandle.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetStdHandle(uint stdHandle,
                                            IntPtr handle);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UpdateProcThreadAttribute(
        IntPtr attributeList,
        uint flags,
        IntPtr attribute,
        IntPtr value,
        IntPtr size,
        IntPtr previousValue,
        IntPtr returnSize);

    // http://pinvoke.net/default.aspx/kernel32/WriteConsole.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteConsole(IntPtr consoleOutput,
                                            string buffer,
                                            uint numberOfCharsToWrite,
                                            out uint numberOfCharsWritten,
                                            IntPtr reserved);

    // http://pinvoke.net/default.aspx/kernel32/WriteConsoleInput.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteConsoleInput(IntPtr consoleInput,
                                                 InputRecord[] buffer,
                                                 uint length,
                                                 out uint numberOfEventsWritten);

    // http://pinvoke.net/default.aspx/kernel32/WriteConsoleOutput.html
    [DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputW", SetLastError = true)]
    public static extern bool WriteConsoleOutput(IntPtr consoleOutput,
                                                 [ In ] [ MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 2 ) ]
                                                     TerminalChar[ , ] buffer,
                                                 Coord bufferSize,
                                                 Coord bufferCoord,
                                                 ref SmallRect writeRegion);

    // http://pinvoke.net/default.aspx/kernel32/WriteConsoleOutputAttribute.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteConsoleOutputAttribute(IntPtr consoleOutput,
                                                           ushort[] attribute,
                                                           uint length,
                                                           Coord writeCoord,
                                                           out uint numberOfAttrsWritten);

    // http://pinvoke.net/default.aspx/kernel32/WriteConsoleOutputCharacter.html
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool WriteConsoleOutputCharacter(IntPtr consoleOutput,
                                                           string character,
                                                           uint length,
                                                           Coord writeCoord,
                                                           out uint numberOfCharsWritten);

    // http://pinvoke.net/default.aspx/kernel32/HandlerRoutine.html
    public delegate bool ConsoleCtrlDelegate(CtrlTypes ctrlType);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CreateProcess(string lpApplicationName,
                                               string lpCommandLine,
                                               ref SecurityAttributes lpProcessAttributes,
                                               ref SecurityAttributes lpThreadAttributes,
                                               bool bInheritHandles,
                                               uint dwCreationFlags,
                                               IntPtr lpEnvironment,
                                               string lpCurrentDirectory,
                                               [In] ref StartupinfoEx lpStartupInfo,
                                               out ProcessInformation lpProcessInformation);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteProcThreadAttributeList(IntPtr lpAttributeList);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadLibrary(string filename);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

}
