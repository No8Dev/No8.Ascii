namespace Asciis.App;

public abstract class Clipboard
{
    public static Clipboard Create()
    {
        if (UnitTestDetector.IsRunningFromNUnit) return new ClipboardGeneric();
        if (OperatingSystem.IsWindows()) return new ClipboardWindows();
        if (OperatingSystem.IsMacOS()) return new ClipboardMacOS();
        if (OperatingSystem.IsLinux()) return new ClipboardUnix();
        return new ClipboardGeneric();
   }

    public virtual bool    IsSupported => true;
    public virtual string? Contents { get; set; }
}