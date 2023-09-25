namespace No8.Ascii.VirtualTerminal;

public record WindowSize(int Rows, int Cols);

public class ExConOptions
{
    public bool StartFullScreen { get; set; } = true;
    public bool StopOnControlC { get; set; } = true;
    public ConsoleDriver? ConsoleDriver { get; set; }
}
