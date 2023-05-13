namespace No8.Ascii.Controls;

public class WindowClosingEventArgs : EventArgs
{
    public Window RequestingWindow { get; }

    public bool Cancel { get; set; }

    public WindowClosingEventArgs(Window requestingWindow) { RequestingWindow = requestingWindow; }
}