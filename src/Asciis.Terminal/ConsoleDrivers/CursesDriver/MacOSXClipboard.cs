using System.Runtime.InteropServices;
using Asciis.Terminal.Core.Clipboard;

namespace Asciis.Terminal.ConsoleDrivers.CursesDriver;

internal class MacOSXClipboard : ClipboardBase
{
    private IntPtr nsString = objc_getClass("NSString");
    private IntPtr nsPasteboard = objc_getClass("NSPasteboard");
    private IntPtr utfTextType;
    private IntPtr generalPasteboard;
    private IntPtr initWithUtf8Register = sel_registerName("initWithUTF8String:");
    private IntPtr allocRegister = sel_registerName("alloc");
    private IntPtr setStringRegister = sel_registerName("setString:forType:");
    private IntPtr stringForTypeRegister = sel_registerName("stringForType:");
    private IntPtr utf8Register = sel_registerName("UTF8String");
    private IntPtr nsStringPboardType;
    private IntPtr generalPasteboardRegister = sel_registerName("generalPasteboard");
    private IntPtr clearContentsRegister = sel_registerName("clearContents");

    public MacOSXClipboard()
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
        IsSupported = CheckSupport();
    }

    public override bool IsSupported { get; }

    private bool CheckSupport()
    {
        var result = BashRunner.Run("which pbcopy");
        if (!result.FileExists()) return false;
        result = BashRunner.Run("which pbpaste");
        return result.FileExists();
    }

    protected override string GetClipboardDataImpl()
    {
        var ptr = objc_msgSend(generalPasteboard, stringForTypeRegister, nsStringPboardType);
        var charArray = objc_msgSend(ptr, utf8Register);
        return Marshal.PtrToStringAnsi(charArray);
    }

    protected override void SetClipboardDataImpl(string text)
    {
        IntPtr str = default;
        try
        {
            str = objc_msgSend(objc_msgSend(nsString, allocRegister), initWithUtf8Register, text);
            objc_msgSend(generalPasteboard, clearContentsRegister);
            objc_msgSend(generalPasteboard, setStringRegister, str, utfTextType);
        }
        finally
        {
            if (str != default) objc_msgSend(str, sel_registerName("release"));
        }
    }

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern IntPtr objc_getClass(string className);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, string arg1);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

    [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
    private static extern IntPtr sel_registerName(string selectorName);
}
