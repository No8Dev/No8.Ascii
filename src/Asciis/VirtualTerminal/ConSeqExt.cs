namespace No8.Ascii.VirtualTerminal;


/// <summary>
/// 
/// </summary>
/// <remarks>
///     https://vt100.net/emu/ctrlseq_dec.html
///     https://www.xfree86.org/current/ctlseqs.html
///     https://alpha2phi.medium.com/neovim-101-terminal-and-shell-5be83b9f2b88
/// </remarks>

public static class ConSeqExt
{
    public static bool IsCSI(this ConSeq conSeq) => conSeq.Sequence.StartsWith("\x1b[");
}