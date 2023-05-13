namespace No8.Ascii.Platforms;

public class ClipboardFake : Clipboard
{
    public override bool IsSupported => true;
}