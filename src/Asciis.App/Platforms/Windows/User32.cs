using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Asciis.App.Platforms;

public static partial class Windows
{
    public static class User32
    {
        [DllImport("user32.dll")]
        internal static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        internal static extern short GetKeyState(int vKey);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AttachThreadInput(
            IntPtr idAttach,
            IntPtr idAttachTo,
            bool   fAttach
        );

        internal static void ThrowWin32() { throw new Win32Exception(Marshal.GetLastWin32Error()); }

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsClipboardFormatAvailable(uint format);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int GlobalSize(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

        [DllImport("user32.dll")]
        internal static extern bool EmptyClipboard();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetClipboardData(uint uFormat);
    }

    // ReSharper disable CommentTypo
    // ReSharper disable IdentifierTypo
    internal enum ClipboardFormatAvailable
    {
        Bitmap = 2, //  A handle to a bitmap (HBITMAP).
        Dib = 8, // A memory object containing a BITMAPINFO structure followed by the bitmap bits.
        Dibv5 = 17, // A memory object containing a BITMAPV5HEADER structure followed by the bitmap color space information and the bitmap bits.
        Dif = 5, // Software Arts' Data Interchange Format.

        // Bitmap display format associated with a private format. The hMem parameter must be a handle to data that can be displayed in bitmap format in lieu of the privately formatted data.
        Dspbitmap = 0x0082,

        // Enhanced metafile display format associated with a private format. The hMem parameter must be a handle to data that can be displayed in enhanced metafile format in lieu of the privately formatted data.
        Dspenhmetafile = 0x008E,

        // Metafile-picture display format associated with a private format. The hMem parameter must be a handle to data that can be displayed in metafile-picture format in lieu of the privately formatted data.
        Dspmetafilepict = 0x0083,
        Dsptext = 0x0081, // Text display format associated with a private format. The hMem parameter must be a handle to data that can be displayed in text format in lieu of the privately formatted data.
        Enhmetafile = 14, // A handle to an enhanced metafile (HENHMETAFILE).

        // Start of a range of integer values for application-defined GDI object clipboard formats. The end of the range is CF_GDIOBJLAST.
        // Handles associated with clipboard formats in this range are not automatically deleted using the GlobalFree function when the clipboard is emptied. Also, when using values in this range, the hMem parameter is not a handle to a GDI object, but is a handle allocated by the GlobalAlloc function with the GMEM_MOVEABLE flag.
        Gdiobjfirst = 0x0300,
        Gdiobjlast = 0x03FF, // See CF_GDIOBJFIRST.
        Hdrop = 15, // A handle to type HDROP that identifies a list of files. An application can retrieve information about the files by passing the handle to the DragQueryFile function.
        Locale = 16, // The data is a handle (HGLOBAL) to the locale identifier (LCID) associated with text in the clipboard. When you close the clipboard, if it contains CF_TEXT data but no CF_LOCALE data, the system automatically sets the CF_LOCALE format to the current input language. You can use the CF_LOCALE format to associate a different locale with the clipboard text.

        // Handle to a metafile picture format as defined by the METAFILEPICT structure. When passing a CF_METAFILEPICT handle by means of DDE, the application responsible for deleting hMem should also free the metafile referred to by the CF_METAFILEPICT handle.
        Metafilepict = 3,
        Oemtext = 7, // Text format containing characters in the OEM character set. Each line ends with a carriage return/linefeed (CR-LF) combination. A null character signals the end of the data.

        // Owner-display format. The clipboard owner must display and update the clipboard viewer window, and receive the WM_ASKCBFORMATNAME, WM_HSCROLLCLIPBOARD, WM_PAINTCLIPBOARD, WM_SIZECLIPBOARD, and WM_VSCROLLCLIPBOARD messages. The hMem parameter must be NULL.
        Ownerdisplay = 0x0080,

        // Handle to a color palette. Whenever an application places data in the clipboard that depends on or assumes a color palette, it should place the palette on the clipboard as well.
        // If the clipboard contains data in the CF_PALETTE (logical color palette) format, the application should use the SelectPalette and RealizePalette functions to realize (compare) any other data in the clipboard against that logical palette.
        // When displaying clipboard data, the clipboard always uses as its current palette any object on the clipboard that is in the CF_PALETTE format.
        Palette = 9,
        Pendata = 10, // Data for the pen extensions to the Microsoft Windows for Pen Computing.

        // Start of a range of integer values for private clipboard formats. The range ends with CF_PRIVATELAST. Handles associated with private clipboard formats are not freed automatically; the clipboard owner must free such handles, typically in response to the WM_DESTROYCLIPBOARD message.
        Privatefirst = 0x0200,
        Privatelast = 0x02FF, // See CF_PRIVATEFIRST.
        Riff = 11, // Represents audio data more complex than can be represented in a CF_WAVE standard wave format.
        Sylk = 4, // Microsoft Symbolic Link (SYLK) format.
        Text = 1, // Text format. Each line ends with a carriage return/linefeed (CR-LF) combination. A null character signals the end of the data. Use this format for ANSI text.
        Tiff = 6, // Tagged-image file format.

        // Unicode text format. Each line ends with a carriage return/linefeed (CR-LF) combination. A null character signals the end of the data.
        Unicodetext = 13,
        Wave        = 12, // Represents audio data in one of the standard wave formats, such as 11 kHz or 22 kHz PCM.
    }
    // ReSharper restore IdentifierTypo
    // ReSharper restore CommentTypo
}