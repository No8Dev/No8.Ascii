using Asciis.App.Platforms;

namespace Asciis.App;

public class ClipboardUnix : Clipboard
{
    public ClipboardUnix()
    {
        IsSupported = CheckSupport();
    }

    public override bool IsSupported { get; }

    public override string? Contents
    {
        get
        {
            var tempFileName = Path.GetTempFileName();
            try
            {
                // BashRunner.Run ($"xsel -o --clipboard > {tempFileName}");
                Unix.BashRun($"xclip -selection clipboard -o > {tempFileName}");
                return File.ReadAllText(tempFileName);
            }
            finally
            {
                File.Delete(tempFileName);
            }
        }
        set
        {
            if (value != null)
                Unix.BashRun("xclip -selection clipboard -i", false, value);
        }
    }

    private bool CheckSupport()
    {
        try
        {
            var result = Unix.BashRun("which xclip", runCurses: false);
            return result.FileExists();
        }
        catch (Exception)
        {
            // Permissions issue.
            return false;
        }
    }

}