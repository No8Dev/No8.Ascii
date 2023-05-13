using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Asciis.App.Platforms;

public static partial class Windows
{
    public static class Kernel32
    {
        internal static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibrary(string filename);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);


        // http://pinvoke.net/default.aspx/kernel32/AddConsoleAlias.html
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool AddConsoleAlias(
            string source,
            string target,
            string exeName);

        // http://pinvoke.net/default.aspx/kernel32/AllocConsole.html
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AllocConsole();

        // http://pinvoke.net/default.aspx/kernel32/AttachConsole.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AttachConsole(uint processId);


        // https://docs.microsoft.com/en-us/windows/console/closepseudoconsole
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern void ClosePseudoConsole(IntPtr hConsole);

        // https://docs.microsoft.com/en-us/windows/console/createpseudoconsole
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int CreatePseudoConsole(
            Coord              size,
            IntPtr             hInput,
            IntPtr             hOutput,
            PseudoConsoleFlags flags,
            out IntPtr         hConsole);

        // https://docs.microsoft.com/en-us/windows/console/resizepseudoconsole
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int ResizePseudoConsole(IntPtr hConsole, Coord size);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool CreatePipe(
            out IntPtr hReadPipe,
            out IntPtr hWritePipe,
            IntPtr     lpPipeAttributes,
            int        nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadFile(
            IntPtr       hFile,
            [Out] byte[] lpBuffer,
            uint         nNumberOfBytesToRead,
            out uint     lpNumberOfBytesRead,
            IntPtr       lpOverlapped);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool WriteFile(
            IntPtr   hFile,
            byte[]   lpBuffer,
            uint     nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr   lpOverlapped);

        // http://pinvoke.net/default.aspx/kernel32/CreateConsoleScreenBuffer.html
        [Obsolete(
            "NOT Virtual Terminal compliant",
            UrlFormat = "https://docs.microsoft.com/en-us/windows/console/createconsolescreenbuffer")]
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateConsoleScreenBuffer(
            DesiredAccess desiredAccess,
            FileShare     shareMode,
            IntPtr        securityAttributes,
            uint          flags,
            IntPtr        screenBufferData);

        // http://pinvoke.net/default.aspx/kernel32/FillConsoleOutputAttribute.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool FillConsoleOutputAttribute(
            IntPtr   consoleOutput,
            ushort   attribute,
            uint     length,
            Coord    writeCoord,
            out uint numberOfAttrsWritten);

        // http://pinvoke.net/default.aspx/kernel32/FillConsoleOutputCharacter.html
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool FillConsoleOutputCharacter(
            IntPtr   consoleOutput,
            char     character,
            uint     length,
            Coord    writeCoord,
            out uint numberOfCharsWritten);

        // http://pinvoke.net/default.aspx/kernel32/FlushConsoleInputBuffer.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool FlushConsoleInputBuffer(IntPtr consoleInput);

        // http://pinvoke.net/default.aspx/kernel32/FreeConsole.html
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeConsole();

        // http://pinvoke.net/default.aspx/kernel32/GenerateConsoleCtrlEvent.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GenerateConsoleCtrlEvent(
            uint ctrlEvent,
            uint processGroupId);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleAlias.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleAlias(
            string            source,
            out StringBuilder targetBuffer,
            uint              targetBufferLength,
            string            exeName);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleAliases.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint GetConsoleAliases(
            StringBuilder[] targetBuffer,
            uint            targetBufferLength,
            string          exeName);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleAliasesLength.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint GetConsoleAliasesLength(string exeName);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleAliasExes.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint GetConsoleAliasExes(
            out StringBuilder exeNameBuffer,
            uint              exeNameBufferLength);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleAliasExesLength.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint GetConsoleAliasExesLength();

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleCP.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint GetConsoleCP();

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleCursorInfo.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleCursorInfo(
            IntPtr                consoleOutput,
            out ConsoleCursorInfo consoleCursorInfo);


        // http://pinvoke.net/default.aspx/kernel32/GetConsoleDisplayMode.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleDisplayMode(out ConsoleDisplayMode modeFlags);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleFontSize.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Coord GetConsoleFontSize(
            IntPtr consoleOutput,
            int    font);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleHistoryInfo.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleHistoryInfo(out ConsoleHistoryInfo consoleHistoryInfo);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleMode.html
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleMode", SetLastError = true)]
        internal static extern bool GetConsoleInputMode(
            IntPtr                consoleHandle,
            out ConsoleInputModes mode);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleMode.html
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleMode", SetLastError = true)]
        internal static extern bool GetConsoleOutputMode(
            IntPtr                 consoleHandle,
            out ConsoleOutputModes mode);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleOriginalTitle.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint GetConsoleOriginalTitle(
            out StringBuilder consoleTitle,
            uint              size);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleOutputCP.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint GetConsoleOutputCP();

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleProcessList.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint GetConsoleProcessList(
            out uint[] processList,
            uint       processCount);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleScreenBufferInfo.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleScreenBufferInfo(
            IntPtr                      consoleOutput,
            out ConsoleScreenBufferInfo consoleScreenBufferInfo);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleScreenBufferInfoEx.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleScreenBufferInfoEx(
            IntPtr                        consoleOutput,
            ref ConsoleScreenBufferInfoEx consoleScreenBufferInfo);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleSelectionInfo.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleSelectionInfo([Out] out ConsoleSelectionInfo consoleSelectionInfo);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleTitle.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint GetConsoleTitle(
            [Out] StringBuilder consoleTitle,
            uint                size);

        // http://pinvoke.net/default.aspx/kernel32/GetConsoleWindow.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetConsoleWindow();

        // http://pinvoke.net/default.aspx/kernel32/GetCurrentConsoleFont.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetCurrentConsoleFont(
            IntPtr              consoleOutput,
            bool                maximumWindow,
            ref ConsoleFontInfo consoleCurrentFont);

        // http://pinvoke.net/default.aspx/kernel32/GetCurrentConsoleFontEx.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetCurrentConsoleFontEx(
            IntPtr                consoleOutput,
            bool                  maximumWindow,
            ref ConsoleFontInfoEx consoleCurrentFont);

        // http://pinvoke.net/default.aspx/kernel32/GetLargestConsoleWindowSize.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Coord GetLargestConsoleWindowSize(IntPtr consoleOutput);

        // http://pinvoke.net/default.aspx/kernel32/GetNumberOfConsoleInputEvents.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetNumberOfConsoleInputEvents(
            IntPtr   consoleInput,
            out uint numberOfEvents);

        // http://pinvoke.net/default.aspx/kernel32/GetNumberOfConsoleMouseButtons.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetNumberOfConsoleMouseButtons(ref uint numberOfMouseButtons);

        // http://pinvoke.net/default.aspx/kernel32/GetStdHandle.html
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(StandardHandle stdHandle);

        // http://pinvoke.net/default.aspx/kernel32/GetStdHandle.html
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true)]
        internal static extern SafeFileHandle GetSafeStdHandle(int stdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool InitializeProcThreadAttributeList(
            IntPtr     lpAttributeList,
            int        dwAttributeCount,
            int        dwFlags,
            ref IntPtr lpSize);

        // http://pinvoke.net/default.aspx/kernel32/PeekConsoleInput.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool PeekConsoleInput(
            IntPtr              consoleInput,
            [Out] InputRecord[] buffer,
            uint                length,
            out uint            numberOfEventsRead);

        // http://pinvoke.net/default.aspx/kernel32/ReadConsole.html
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool ReadConsole(
            IntPtr                 consoleInput,
            [Out] StringBuilder    buffer,
            uint                   numberOfCharsToRead,
            ref uint               numberOfCharsRead,
            ref ReadConsoleControl readConsoleControl);

        // http://pinvoke.net/default.aspx/kernel32/ReadConsoleInput.html
        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool ReadConsoleInput(
            IntPtr              consoleInput,
            [Out] InputRecord[] buffer,
            uint                length,
            out uint            numberOfEventsRead);

        // http://pinvoke.net/default.aspx/kernel32/ReadConsoleOutputAttribute.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadConsoleOutputAttribute(
            IntPtr         consoleOutput,
            [Out] ushort[] attribute,
            uint           length,
            Coord          readCoord,
            out uint       numberOfAttrsRead);

        // http://pinvoke.net/default.aspx/kernel32/ReadConsoleOutputCharacter.html
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool ReadConsoleOutputCharacter(
            IntPtr              consoleOutput,
            [Out] StringBuilder character,
            uint                length,
            Coord               readCoord,
            out uint            numberOfCharsRead);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleActiveScreenBuffer.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleActiveScreenBuffer(IntPtr consoleOutput);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleCP.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleCP(uint codePageId);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleCtrlHandler.html
        [DllImport("kernel32.dll")]
        internal static extern bool SetConsoleCtrlHandler(
            ConsoleCtrlDelegate handlerRoutine,
            bool                add);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleCursorInfo.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleCursorInfo(
            IntPtr                     consoleOutput,
            [In] ref ConsoleCursorInfo consoleCursorInfo);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleCursorPosition.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleCursorPosition(
            IntPtr consoleOutput,
            Coord  cursorPosition);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleDisplayMode.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleDisplayMode(
            IntPtr             consoleOutput,
            ConsoleDisplayMode flags,
            out Coord          newScreenBufferDimensions);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleHistoryInfo.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleHistoryInfo(ConsoleHistoryInfo consoleHistoryInfo);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleMode.html
        [DllImport("kernel32.dll", EntryPoint = "SetConsoleMode", SetLastError = true)]
        internal static extern bool SetConsoleInputMode(
            IntPtr            consoleHandle,
            ConsoleInputModes mode);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleMode.html
        [DllImport("kernel32.dll", EntryPoint = "SetConsoleMode", SetLastError = true)]
        internal static extern bool SetConsoleOutputMode(
            IntPtr             consoleHandle,
            ConsoleOutputModes mode);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleOutputCP.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleOutputCP(uint codePageId);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleScreenBufferInfoEx.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleScreenBufferInfoEx(
            IntPtr                        consoleOutput,
            ref ConsoleScreenBufferInfoEx consoleScreenBufferInfoEx);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleScreenBufferSize.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleScreenBufferSize(
            IntPtr consoleOutput,
            Coord  size);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleTextAttribute.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleTextAttribute(
            IntPtr consoleOutput,
            ushort attributes);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleTitle.html
        [DllImport("kernel32.dll")]
        internal static extern bool SetConsoleTitle(string consoleTitle);

        // http://pinvoke.net/default.aspx/kernel32/SetConsoleWindowInfo.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleWindowInfo(
            IntPtr             consoleOutput,
            bool               absolute,
            [In] ref SmallRect consoleWindow);

        // http://pinvoke.net/default.aspx/kernel32/SetCurrentConsoleFontEx.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetCurrentConsoleFontEx(
            IntPtr                consoleOutput,
            bool                  maximumWindow,
            ref ConsoleFontInfoEx consoleCurrentFontEx);

        // http://pinvoke.net/default.aspx/kernel32/SetStdHandle.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetStdHandle(
            uint   stdHandle,
            IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UpdateProcThreadAttribute(
            IntPtr attributeList,
            uint   flags,
            IntPtr attribute,
            IntPtr value,
            IntPtr size,
            IntPtr previousValue,
            IntPtr returnSize);

        // http://pinvoke.net/default.aspx/kernel32/WriteConsole.html
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleW", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WriteConsole(
            IntPtr   consoleOutput,
            string   buffer,
            uint     numberOfCharsToWrite,
            out uint numberOfCharsWritten,
            IntPtr   reserved);

        // http://pinvoke.net/default.aspx/kernel32/WriteConsoleInput.html
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteConsoleInput(
            IntPtr        consoleInput,
            InputRecord[] buffer,
            uint          length,
            out uint      numberOfEventsWritten);

        // http://pinvoke.net/default.aspx/kernel32/WriteConsoleOutputAttribute.html
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteConsoleOutputAttribute(
            IntPtr   consoleOutput,
            ushort[] attribute,
            uint     length,
            Coord    writeCoord,
            out uint numberOfAttrsWritten);

        // http://pinvoke.net/default.aspx/kernel32/WriteConsoleOutputCharacter.html
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WriteConsoleOutputCharacter(
            IntPtr   consoleOutput,
            string   character,
            uint     length,
            Coord    writeCoord,
            out uint numberOfCharsWritten);

        // http://pinvoke.net/default.aspx/kernel32/HandlerRoutine.html
        internal delegate bool ConsoleCtrlDelegate(CtrlTypes ctrlType);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CreateProcess(
            string                      lpApplicationName,
            string                      lpCommandLine,
            ref SecurityAttributes      lpProcessAttributes,
            ref SecurityAttributes      lpThreadAttributes,
            bool                        bInheritHandles,
            uint                        dwCreationFlags,
            IntPtr                      lpEnvironment,
            string                      lpCurrentDirectory,
            [In] ref StartupinfoEx      lpStartupInfo,
            out      ProcessInformation lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteProcThreadAttributeList(IntPtr lpAttributeList);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
        internal static extern bool ReadConsoleInput(
            IntPtr   hConsoleInput,
            IntPtr   lpBuffer,
            uint     nLength,
            out uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool ReadConsoleOutput(
            IntPtr           hConsoleOutput,
            [Out] CharInfo[] lpBuffer,
            Coord            dwBufferSize,
            Coord            dwBufferCoord,
            ref SmallRect    lpReadRegion
        );

        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutput", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool WriteConsoleOutput(
            IntPtr        hConsoleOutput,
            CharInfo[]    lpBuffer,
            Coord         dwBufferSize,
            Coord         dwBufferCoord,
            ref SmallRect lpWriteRegion
        );
        
        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);


        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateConsoleScreenBuffer(
            DesiredAccess dwDesiredAccess,
            ShareMode     dwShareMode,
            IntPtr        secutiryAttributes,
            uint          flags,
            IntPtr        screenBufferData
        );

        // http://pinvoke.net/default.aspx/kernel32/ScrollConsoleScreenBuffer.html
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ScrollConsoleScreenBuffer(
            IntPtr             consoleOutput,
            [In] ref SmallRect scrollRectangle,
            IntPtr             clipRectangle,
            Coord              destinationOrigin,
            [In] ref CharInfo  fill);




        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "FormatMessageW", SetLastError = true, BestFitMapping = true, ExactSpelling = true)]
        internal static extern uint FormatMessage(
            uint                flags,
            IntPtr              source,
            uint                messageId,
            uint                languageId,
            [Out] StringBuilder buffer,
            uint                size,
            string[]?           arguments);

        /// <summary>
        ///     Returns a string message for the specified Win32 error code.
        /// </summary>
        internal static string GetMessage(int errorCode) =>
            GetMessage(errorCode, IntPtr.Zero);

        internal static string GetMessage(int errorCode, IntPtr moduleHandle)
        {
            var flags = FormatMessageFlags.IgnoreInserts | FormatMessageFlags.FromSystem | FormatMessageFlags.ArgumentArray;
            if (moduleHandle != IntPtr.Zero)
                flags |= FormatMessageFlags.FromHmodule;

            var sb        = new StringBuilder(1024);
            FormatMessage(
                (uint)flags,
                IntPtr.Zero,
                (uint)errorCode,
                0,
                sb,
                (uint)sb.Capacity,
                null);

            return sb.ToString();
        }
    }
}



