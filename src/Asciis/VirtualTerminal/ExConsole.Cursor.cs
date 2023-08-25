namespace No8.Ascii.VirtualTerminal;

public partial class ExConsole :
    ExConsole.IExConsoleCursor,
    ExConsole.IExConsoleCursor.Style
{
    public IExConsoleCursor Cursor => this;
    public interface IExConsoleCursor
    {
        void Set(int row, int col);
        void Up(int n = 1);
        void Down(int n = 1);
        void Right(int n = 1);
        void Left(int n = 1);
        void Show();
        void Hide();


        /// <summary>
        ///     Set cursor Style
        /// </summary>
        public interface Style
        {
            void Blinking();
            void Steady();
            void BlinkingUnderline();
            void SteadyUnderline();
            void BlinkingBar();
            void SteadyBar();
        }
    }
    
    void IExConsoleCursor.Set(int row, int col) => CD.Write(TerminalSeq.Cursor.Set(row, col));
    void IExConsoleCursor.Up(int n = 1) => CD.Write(TermInfo.CursorUp ?? TerminalSeq.Cursor.Up(n));
    void IExConsoleCursor.Down(int n = 1) => CD.Write(TermInfo.CursorDown ?? TerminalSeq.Cursor.Down(n));
    void IExConsoleCursor.Right(int n = 1) => CD.Write(TermInfo.CursorRight ?? TerminalSeq.Cursor.Right(n));
    void IExConsoleCursor.Left(int n = 1) => CD.Write(TermInfo.CursorLeft ?? TerminalSeq.Cursor.Left(n)); 
        
    void IExConsoleCursor.Show() => CD.Write( TermInfo.CursorVisible ?? TerminalSeq.Cursor.Show);
    void IExConsoleCursor.Hide() => CD.Write(TermInfo.CursorInvisible ?? TerminalSeq.Cursor.Hide);
    
    void IExConsoleCursor.Style.Blinking() => CD.Write(TerminalSeq.Cursor.Style.Blinking);
    void IExConsoleCursor.Style.Steady() => CD.Write(TerminalSeq.Cursor.Style.Steady);
    void IExConsoleCursor.Style.BlinkingUnderline() => CD.Write(TerminalSeq.Cursor.Style.BlinkingUnderline);
    void IExConsoleCursor.Style.SteadyUnderline() => CD.Write(TerminalSeq.Cursor.Style.SteadyUnderline);
    void IExConsoleCursor.Style.BlinkingBar() => CD.Write(TerminalSeq.Cursor.Style.BlinkingBar);
    void IExConsoleCursor.Style.SteadyBar() => CD.Write(TerminalSeq.Cursor.Style.SteadyBar);
}