using System.Runtime.InteropServices;

namespace No8.Ascii.Platforms;

using static UnmanagedLibrary.MacOS.AppKit;

public class ClipboardMacOS : Clipboard
{
    private readonly IntPtr nsString = objc_getClass("NSString");
    private readonly IntPtr nsPasteboard = objc_getClass("NSPasteboard");
    private readonly IntPtr utfTextType;
    private readonly IntPtr generalPasteboard;
    private readonly IntPtr initWithUtf8Register = sel_registerName("initWithUTF8String:");
    private readonly IntPtr allocRegister = sel_registerName("alloc");
    private readonly IntPtr setStringRegister = sel_registerName("setString:forType:");
    private readonly IntPtr stringForTypeRegister = sel_registerName("stringForType:");
    private readonly IntPtr utf8Register = sel_registerName("UTF8String");
    private readonly IntPtr nsStringPboardType;
    private readonly IntPtr generalPasteboardRegister = sel_registerName("generalPasteboard");
    private readonly IntPtr clearContentsRegister = sel_registerName("clearContents");

    public ClipboardMacOS()
    {
        utfTextType = objc_msgSend(
            objc_msgSend(nsString, allocRegister),
            initWithUtf8Register,
            "public.utf8-plain-text");
        nsStringPboardType = objc_msgSend(
            objc_msgSend(nsString, allocRegister),
            initWithUtf8Register,
            "NSStringPboardType");
        generalPasteboard = objc_msgSend(nsPasteboard, generalPasteboardRegister);
    }

    public override bool IsSupported
    {
        get
        {
            var result = Unix.BashRun("which pbcopy");
            if (!result.FileExists())
                return false;
            result = Unix.BashRun("which pbpaste");
            return result.FileExists();

        }
    }

    public override string? Contents
    {
        get
        {
            var ptr = objc_msgSend(generalPasteboard, stringForTypeRegister, nsStringPboardType);
            var charArray = objc_msgSend(ptr, utf8Register);
            return Marshal.PtrToStringAnsi(charArray);
        }
        set
        {
            if (value == null) return;
            IntPtr str = default;
            try
            {
                str = objc_msgSend(objc_msgSend(nsString, allocRegister), initWithUtf8Register, value);
                objc_msgSend(generalPasteboard, clearContentsRegister);
                objc_msgSend(generalPasteboard, setStringRegister, str, utfTextType);
            }
            finally
            {
                if (str != default) objc_msgSend(str, sel_registerName("release"));
            }
        }
    }

}
