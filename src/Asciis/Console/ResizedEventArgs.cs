namespace No8.Ascii;

/// <summary>
///     Event arguments for the Resized event.
/// </summary>
public class ResizedEventArgs : EventArgs
{
    public int Rows { get; set; }
    public int Cols { get; set; }
}
