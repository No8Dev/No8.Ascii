namespace Asciis.App;

/// <summary>
///     Event arguments for the <see cref="Asciis.AsciiApplication.Resized"/> event.
/// </summary>
public class ResizedEventArgs : EventArgs
{
    public int Rows { get; set; }
    public int Cols { get; set; }
}
