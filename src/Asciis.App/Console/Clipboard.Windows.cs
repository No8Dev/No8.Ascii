using System.Runtime.InteropServices;
using Asciis.App.Platforms;

namespace Asciis.App;

using static Windows;
using static Windows.User32;

public class ClipboardWindows : Clipboard
{
    public override bool IsSupported => IsClipboardFormatAvailable((int)ClipboardFormatAvailable.Unicodetext);

    public override string? Contents
    {
        get
        {
            try
            {
                DoOpenClipboard();

                var handle = GetClipboardData((int)ClipboardFormatAvailable.Unicodetext);
                if (handle == IntPtr.Zero)
                    return null;

                var pointer = IntPtr.Zero;

                try
                {
                    pointer = GlobalLock(handle);
                    if (pointer == IntPtr.Zero)
                        return null;

                    var    size = GlobalSize(handle);
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
        set
        {
            IntPtr hGlobal = default;
            try
            {
                DoOpenClipboard();
                EmptyClipboard();

                if (string.IsNullOrEmpty(value))
                    return;
            
                var bytes = Encoding.Unicode.GetBytes(value);
                
                hGlobal = Marshal.AllocHGlobal(bytes.Length);
                if (hGlobal == default) ThrowWin32();

                var target = GlobalLock(hGlobal);
                if (target == default) ThrowWin32();

                try
                {
                    Marshal.Copy(bytes, 0, target, bytes.Length);
                }
                finally
                {
                    GlobalUnlock(target);
                }

                if (SetClipboardData((int)Windows.ClipboardFormatAvailable.Unicodetext, hGlobal) == default) 
                    ThrowWin32();

                hGlobal = default;
            }
            finally
            {
                if (hGlobal != default) 
                    Marshal.FreeHGlobal(hGlobal);

                CloseClipboard();
            }
        }
    }
    
    private bool DoOpenClipboard()
    {
        var num = 10;
        while (true)
        {
            if (OpenClipboard(IntPtr.Zero)) 
                return true;

            if (--num == 0) 
                ThrowWin32();

            Thread.Sleep(100);
        }
    }

}