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
    
    void IExConsoleCursor.Set(int row, int col) => Write(TerminalSeq.Cursor.Set(row, col));
    void IExConsoleCursor.Up(int n) => Write(TermInfo.CursorUp ?? TerminalSeq.Cursor.Up(n));
    void IExConsoleCursor.Down(int n) => Write(TermInfo.CursorDown ?? TerminalSeq.Cursor.Down(n));
    void IExConsoleCursor.Right(int n) => Write(TermInfo.CursorRight ?? TerminalSeq.Cursor.Right(n));
    void IExConsoleCursor.Left(int n) => Write(TermInfo.CursorLeft ?? TerminalSeq.Cursor.Left(n)); 
        
    void IExConsoleCursor.Show() => Write( TermInfo.CursorVisible ?? TerminalSeq.Cursor.Show);
    void IExConsoleCursor.Hide() => Write(TermInfo.CursorInvisible ?? TerminalSeq.Cursor.Hide);
    
    void IExConsoleCursor.Style.Blinking() => Write(TerminalSeq.Cursor.Style.Blinking);
    void IExConsoleCursor.Style.Steady() => Write(TerminalSeq.Cursor.Style.Steady);
    void IExConsoleCursor.Style.BlinkingUnderline() => Write(TerminalSeq.Cursor.Style.BlinkingUnderline);
    void IExConsoleCursor.Style.SteadyUnderline() => Write(TerminalSeq.Cursor.Style.SteadyUnderline);
    void IExConsoleCursor.Style.BlinkingBar() => Write(TerminalSeq.Cursor.Style.BlinkingBar);
    void IExConsoleCursor.Style.SteadyBar() => Write(TerminalSeq.Cursor.Style.SteadyBar);
}