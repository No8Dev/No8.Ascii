using Asciis.Terminal.Core.Clipboard;

namespace Asciis.Terminal.ConsoleDrivers.CursesDriver;

internal class CursesClipboard : ClipboardBase
{
    public CursesClipboard() { IsSupported = CheckSupport(); }

    public override bool IsSupported { get; }

    private bool CheckSupport()
    {
        try
        {
            var result = BashRunner.Run("which xclip", runCurses: false);
            return result.FileExists();
        }
        catch (Exception)
        {
            // Permissions issue.
            return false;
        }
    }

    protected override string GetClipboardDataImpl()
    {
        var tempFileName = System.IO.Path.GetTempFileName();
        try
        {
            // BashRunner.Run ($"xsel -o --clipboard > {tempFileName}");
            BashRunner.Run($"xclip -selection clipboard -o > {tempFileName}");
            return System.IO.File.ReadAllText(tempFileName);
        }
        finally
        {
            System.IO.File.Delete(tempFileName);
        }
    }

    protected override void SetClipboardDataImpl(string text)
    {
        // var tempFileName = System.IO.Path.GetTempFileName ();
        // System.IO.File.WriteAllText (tempFileName, text);
        // try {
        // 	// BashRunner.Run ($"cat {tempFileName} | xsel -i --clipboard");
        // 	BashRunner.Run ($"cat {tempFileName} | xclip -selection clipboard");
        // } finally {
        // 	System.IO.File.Delete (tempFileName);
        // }

        BashRunner.Run("xclip -selection clipboard -i", false, text);
    }
}
