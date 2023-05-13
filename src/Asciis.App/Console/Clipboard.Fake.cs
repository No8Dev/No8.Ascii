namespace Asciis.App;

public class ClipboardFake : Clipboard
{
    public override bool IsSupported => true;
}