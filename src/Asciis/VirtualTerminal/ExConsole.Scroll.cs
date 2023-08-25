namespace No8.Ascii.VirtualTerminal;

using static TerminalSeq;

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

public partial class ExConsole :
    ExConsole.IExConsoleScroll
{
    public IExConsoleScroll Scroll => this;
    
    public interface IExConsoleScroll
    {
        void Smooth();
        void Jump();

        /// <summary>
        ///     Scrolling Speed
        /// </summary>
        /// <param name="speed">0-9</param>
        void Speed(int speed);
        void Set(int topRow, int bottomRow);
        void Up(int lines);
        void Down(int lines);
    }

    void IExConsoleScroll.Smooth() => CD.Write(TextProcessing.ScrollingMode.Smooth);
    void IExConsoleScroll.Jump() => CD.Write(TextProcessing.ScrollingMode.Jump);
    void IExConsoleScroll.Speed(int speed) => CD.Write(TextProcessing.SetScrollSpeed(speed));

    void IExConsoleScroll.Set(int topRow, int bottomRow) =>
        CD.Write(TerminalSeq.Scroll.Set(topRow, bottomRow));

    void IExConsoleScroll.Up(int lines) => CD.Write(TerminalSeq.Scroll.Up(lines));
    void IExConsoleScroll.Down(int lines) => CD.Write(TerminalSeq.Scroll.Down(lines));
}