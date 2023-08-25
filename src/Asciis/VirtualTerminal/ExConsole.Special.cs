namespace No8.Ascii.VirtualTerminal;

public partial class ExConsole : ExConsole.IExConsoleSpecial
{
    public interface IExConsoleSpecial
    {
        /// <summary>
        ///     Reset to initial state
        /// </summary>
        void SoftReset();

        /// <summary>
        ///     Play Sound
        /// </summary>
        /// <param name="vol">0..7</param>
        /// <param name="duration">0..255 1/32nd of a second</param>
        /// <param name="note">
        /// Note Selection
        ///  0:silent    1:C5    2:C#5   3:D5    4:D#5(Eb)   5:E5    6:F5    7:F#5        
        ///  8:G5        9:G#5  10:A5   11:A#5  12:B5       13:C6   14:C#6  15:D6       
        /// 16:D#6      17:E6   18:F6   19:F#6  20:G6       21:G#6  22:A6   23:A#6   
        /// 24:B6       25:C7
        /// </param>
        void PlaySound(int vol, int duration, int note);
    }

    public IExConsoleSpecial Special => this;
    void IExConsoleSpecial.SoftReset() => CD.Write(TerminalSeq.ControlSeq.SoftTerminalReset);
    void IExConsoleSpecial.PlaySound(int vol, int duration, int note)
    {
        CD.Write( TerminalSeq.AudibleAttributes.PlaySound(
            Math.Clamp(vol, 0, 7), 
            Math.Clamp(duration, 0, 255), 
            Math.Clamp(note,0,25)) );
    }
}