namespace No8.Ascii.VirtualTerminal;

/// <summary>
///     Console Sequence of bytes
/// </summary>
public class ConSeq
{
    public string Sequence { get; }
    public int[] Parameters => _parameters.ToArray();
    public char[] Intermediates => _intermediates.ToArray();
    public char Final { get; private set; }

    public string StartChars()
    {
        if (Sequence.Length <= 2)
            return Sequence;
        var len = 0;
        if (Sequence[0] == '\x1b')
            len = 1;
        ///     I...I (0 or more)   0x20..0x2F  <SP>!"#$%&'()*+,-./
        ///     P...P (up to 16)    0x30..0x3F  0123456789:;<=>?
        ///     Final (1)           0x40..0x7E  @ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
        if ("<=>!?$[ ]".Contains(Sequence[1]))
            len = 2;

        return Sequence.Substring(0, len);
    }
    public string FinalChars()
    {
        if (_intermediates.Count == 0)
            return new string(Final, 1);
        return new string(Intermediates) + Final;
    }

    private readonly List<int> _parameters = new();
    private readonly List<char> _intermediates = new();

    public ConSeq(string sequence)
    {
        Sequence = sequence;
    }

    private string SafeString(string value)
    {
        var sb = new StringBuilder();
        foreach (var ch in value)
        {
            if (ch < ' ')
                sb.Append($"<{(Terminal.ControlChar)ch}>");
            else
                sb.Append(ch);
        }

        return sb.ToString();
    }
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append( SafeString(Sequence) );
        sb.Append($"\t\t[{SafeString(StartChars())}] ");
        if (_parameters.Count > 0)
            sb.Append(" (" + string.Join(',', _parameters) + ")");
        if (Final != 0)
            sb.Append($" <{SafeString(FinalChars())}>");
        return sb.ToString();
    }

    public static ConSeq? Parse(string sequence)
    {
        if (string.IsNullOrEmpty(sequence))
            return null;

        var seq = new ConSeq(sequence);
        if (sequence.Length == 1)
            return seq;

        if (sequence[0] == '\x1b')
        {
            /// Control Sequence
            ///     CSI                 0x1b [
            ///     OSC                 0x1b ]
            ///     DCS                 0x1b P
            ///     P...P (up to 16)    0x30..0x3F  0123456789:;<=>?
            ///     I...I (0 or more)   0x20..0x2F  <SP>!"#$%&'()*+,-./
            ///     Final (1)           0x40..0x7E  @ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
            ///     Data String         *****
            ///     String Term         0x1b \

            var p          = -1;
            var startChars = seq.StartChars();
            
            for (int i = startChars.Length; i < sequence.Length; i++)
            {
                var ch = sequence[i];
                if (IsParameter(ch))
                {
                    if (p < 0)
                        p = 0;
                    if (char.IsNumber(ch))
                    {
                        p *= 10;
                        p += (ch - '0');
                    }
                    else
                    {
                        seq.AddParameter(p);
                        p = -1;
                    }
                }
                else
                {
                    if (p >= 0)
                    {
                        seq.AddParameter(p);
                        p = -1;
                    }

                    if (IsIntermediate(ch))
                    {
                        seq.AddIntermediate(ch);
                    }
                    else if (IsFinal(ch))
                    {
                        seq.Final = ch;
                    }
                }
            }
        }
        
        return seq;
    }

    private void AddParameter(int p) => _parameters.Add(p);
    private void AddIntermediate(char ch) => _intermediates.Add(ch);
    
    internal static bool IsIntermediate(char ch) => ch >= 0x20 && ch <= 0x2F;
    internal static bool IsParameter(char ch) => ch >= 0x30 && ch <= 0x3F;
    internal static bool IsFinal(char ch) => ch >= 0x40 && ch <= 0x7E;

}

