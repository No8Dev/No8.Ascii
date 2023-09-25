namespace No8.Ascii.VirtualTerminal;

using static TerminalSeq;

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

public partial class ExConsole : ExConsole.IExConsoleScreen
{
    public IExConsoleScreen Screen => this;
    
    public interface IExConsoleScreen
    {
        /// <summary>
        ///     
        /// </summary>
        void Clear();

        /// <summary>
        ///     Switch to Full screen (Alt) mode
        ///     Cursor is save and Alt screen is cleared 
        /// </summary>
        void ScreenAlt();

        /// <summary>
        ///     Switch to normal screen mode
        ///     Cursor is also restored to last location
        /// </summary>
        void ScreenNormal();

        /// <summary>
        ///     Set Window Title
        /// </summary>
        /// <param name="title">Up to 30 characters</param>
        void SetWindowTitle(string title);
    }
    
    /// <summary>
    ///     
    /// </summary>
    void IExConsoleScreen.Clear() => Write(TermInfo.ClearScreen ?? ControlSeq.ClearScreen);
        
    /// <summary>
    ///     Switch to Full screen (Alt) mode
    ///     Cursor is save and Alt screen is cleared 
    /// </summary>
    void IExConsoleScreen.ScreenAlt() => Write(Mode.ScreenAltClear);
        
    /// <summary>
    ///     Switch to normal screen mode
    ///     Cursor is also restored to last location
    /// </summary>
    void IExConsoleScreen.ScreenNormal() => Write(Mode.ScreenNormal);
        
    /// <summary>
    ///     Set Window Title
    /// </summary>
    /// <param name="title">Up to 30 characters</param>
    void IExConsoleScreen.SetWindowTitle(string title) =>
        Write(Window.SetWindowTitle(title));

}

