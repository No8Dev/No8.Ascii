using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using Asciis.Terminal.Core.Clipboard;

namespace Asciis.Terminal.ConsoleDrivers;

internal class WindowsClipboard : ClipboardBase
{
    public WindowsClipboard() { IsSupported = IsClipboardFormatAvailable(cfUnicodeText); }

    public override bool IsSupported { get; }

    protected override string GetClipboardDataImpl()
    {
        //if (!IsClipboardFormatAvailable (cfUnicodeText))
        //	return null;

        try
        {
            if (!OpenClipboard(IntPtr.Zero))
                return null;

            var handle = GetClipboardData(cfUnicodeText);
            if (handle == IntPtr.Zero)
                return null;

            var pointer = IntPtr.Zero;

            try
            {
                pointer = GlobalLock(handle);
                if (pointer == IntPtr.Zero)
                    return null;

                var size = GlobalSize(handle);
                byte[] buff = new byte[size];

                Marshal.Copy(pointer, buff, 0, size);

                return Encoding.Unicode.GetString(buff)
                               .TrimEnd('\0')
                               .Replace("\r\n", "\n");
            }
            finally
            {
                if (pointer != IntPtr.Zero)
                    GlobalUnlock(handle);
            }
        }
        finally
        {
            CloseClipboard();
        }
    }

    protected override void SetClipboardDataImpl(string text)
    {
        OpenClipboard();

        EmptyClipboard();
        IntPtr hGlobal = default;
        try
        {
            var bytes = (text.Length + 1) * 2;
            hGlobal = Marshal.AllocHGlobal(bytes);

            if (hGlobal == default) ThrowWin32();

            var target = GlobalLock(hGlobal);

            if (target == default) ThrowWin32();

            try
            {
                Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
            }
            finally
            {
                GlobalUnlock(target);
            }

            if (SetClipboardData(cfUnicodeText, hGlobal) == default) ThrowWin32();

            hGlobal = default;
        }
        finally
        {
            if (hGlobal != default) Marshal.FreeHGlobal(hGlobal);

            CloseClipboard();
        }
    }

    private void OpenClipboard()
    {
        var num = 10;
        while (true)
        {
            if (OpenClipboard(default)) break;

            if (--num == 0) ThrowWin32();

            Thread.Sleep(100);
        }
    }

    private const uint cfUnicodeText = 13;

    private void ThrowWin32() { throw new Win32Exception(Marshal.GetLastWin32Error()); }

    [DllImport("User32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsClipboardFormatAvailable(uint format);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern int GlobalSize(IntPtr handle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GlobalLock(IntPtr hMem);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalUnlock(IntPtr hMem);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool OpenClipboard(IntPtr hWndNewOwner);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetClipboardData(uint uFormat, IntPtr data);

    [DllImport("user32.dll")]
    private static extern bool EmptyClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetClipboardData(uint uFormat);
}
