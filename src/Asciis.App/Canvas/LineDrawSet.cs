namespace Asciis.App;

internal class LineDrawSet
{
    //    ^
    //  <- ->    <Left(8), ^Top(4), >Right(2), vBottom(1)
    //    v
    // 0 lines
    public const byte IndexZero = 0b_0000; // 0
                                           // 1 Line
    public const byte IndexLeft = 0b_1000; // 8
    public const byte IndexTop = 0b_0100; // 4
    public const byte IndexRight = 0b_0010; // 2
    public const byte IndexBot = 0b_0001; // 1
                                          // 2 lines - Horz + Vert
    public const byte IndexLeftRight = 0b_1010; // 10
    public const byte IndexTopBot = 0b_0101; // 5
                                             // 2 Lines - Corner
    public const byte IndexLeftTop = 0b_1100; // 12
    public const byte IndexTopRight = 0b_0110; // 6
    public const byte IndexRightBot = 0b_0011; // 3
    public const byte IndexBotLeft = 0b_1001; // 9
                                              // 3 Lines - Intersection
    public const byte IndexLeftTopRight = 0b_1110; // 14
    public const byte IndexTopRightBot = 0b_0111; // 7
    public const byte IndexRightBotLeft = 0b_1011; // 11
    public const byte IndexBotLeftTop = 0b_1101; // 13
                                                 // 4 Lines - Cross
    public const byte IndexLeftTopRightBot = 0b_1111; //  15


    /*
     Only need to combine lines when new top line is horz or vert.
     All other line type will overwrite.

     Under is Single, over is Double
      ┌ ┐ └ ┘ ─ │ ┼ ├ ┤ ┬ ┴
    ═ ╤ ╤ ╧ ╧ ═ ╪ ╪ ╪ ╪ ╤ ╧ 
    ║ ╟ ╢ ╟ ╢ ╫ ║ ╫ ╟ ╢ ╫ ╫ 

    Under is Double, over is Single
      ╔ ╗ ╚ ╝ ═ ║ ╬ ╠ ╣ ╦ ╩
    ─ ╥ ╥ ╨ ╨ ─ ╫ ╫ ╫ ╫ ╥ ╨       
    │ ╞ ╡ ╞ ╡ ╪ │ ╪ ╞ ╡ ╪ ╪  
    */

    private readonly List<Rune> _chars;

    public LineDrawSet(string chars)
    {
        if (chars == null || chars.Length != 16)
            throw new ArgumentException(nameof(chars));

        _chars = chars.ToRuneList();
    }

    public Rune Horz      => _chars[IndexLeftRight];
    public Rune HorzStart => _chars[IndexRight];
    public Rune HorzEnd   => _chars[IndexLeft];
    public Rune Vert      => _chars[IndexTopBot];
    public Rune VertStart => _chars[IndexBot];
    public Rune VertEnd   => _chars[IndexTop];
    public Rune TopLeft   => _chars[IndexRightBot];
    public Rune TopRight  => _chars[IndexBotLeft];
    public Rune BotLeft   => _chars[IndexTopRight];
    public Rune BotRight  => _chars[IndexLeftTop];
    public Rune LeftSide  => _chars[IndexTopRightBot];
    public Rune Top       => _chars[IndexRightBotLeft];
    public Rune RightSide => _chars[IndexBotLeftTop];
    public Rune Bottom    => _chars[IndexLeftTopRight];
    public Rune Cross     => _chars[IndexLeftTopRightBot];


    public bool Contains(Rune       rune) => rune.IsValid() && _chars.Contains(rune);
    public bool IsStraightLine(Rune rune) => rune == _chars[IndexLeftRight] || rune == _chars[IndexTopBot];
    public int  IndexOf(Rune        rune) => _chars.IndexOf(rune);

    public Rune Combine(Rune underChar, Rune overChar, bool isStart = false, bool isEnd = false)
    {
        var underIndex = _chars.IndexOf(underChar);
        var overIndex = _chars.IndexOf(overChar);

        if (isStart || isEnd)
        {
            if (isStart && overIndex == IndexLeftRight)
                overIndex = IndexRight;
            else if (isEnd && overIndex == IndexLeftRight)
                overIndex = IndexLeft;
            if (isStart && overIndex == IndexTopBot)
                overIndex = IndexBot;
            else if (isEnd && overIndex == IndexTopBot)
                overIndex = IndexTop;
        }

        return _chars[(underIndex | overIndex) & 0b_1111];
    }

    public Rune this[int index] =>
        _chars[index & 0b_1111];
}
