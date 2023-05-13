using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;

namespace Asciis.UI;

public enum LineSet
{
    None,
    Single,
    Double,
    Rounded,
    SingleHorz,
    DoubleHorz,
    DoubleUnder,
    DoubleOver,
    DoubleRaised,
    DoublePressed
}

// ReSharper disable InconsistentNaming

public static class Pixel
{
    public static class Block
    {

        public const char LightShade = '░';
        public const char MediumShade = '▒';
        public const char DarkShade = '▓';
        public const char Solid = '█';

        public const char Quadrant_UpperLeft = '▘';
        public const char Quadrant_UpperRight = '▝';
        public const char Quadrant_LowerLeft = '▖';
        public const char Quadrant_LowerRight = '▗';
        public const char Quadrant_PointUpperLeft = '▛';
        public const char Quadrant_PointUpperRight = '▜';
        public const char Quadrant_PointLowerLeft = '▙';
        public const char Quadrant_Right = '▐';
        public const char Quadrant_Left = '▌';
        public const char Quadrant_Up = '▀';
        public const char Quadrant_Down = '▄';

    }

    public static class Shapes
    {
        public const char Square_Solid = '■';
        public const char Square_Border = '□';
        public const char Square_Rounded = '▢';

        public const char Triangle_Solid_Up = '▲';
        public const char Triangle_Solid_Right = '▶';
        public const char Triangle_Solid_Down = '▼';
        public const char Triangle_Solid_Left = '◀';

        public const char Triangle_Border_Up = '△';
        public const char Triangle_Border_Right = '▷';
        public const char Triangle_Border_Down = '▽';
        public const char Triangle_Border_Left = '◁';

        public const char Triangle_Solid_UpperLeft = '◤';
        public const char Triangle_Solid_UpperRight = '◥';
        public const char Triangle_Solid_LowerLeft = '◢';
        public const char Triangle_Solid_LowerRight = '◣';

        public const char Triangle_Border_UpperLeft = '◸';
        public const char Triangle_Border_UpperRight = '◹';
        public const char Triangle_Border_LowerLeft = '◿';
        public const char Triangle_Border_LowerRight = '◺';

        public const char Diamond_Solid = '◆';
        public const char Diamond_Border = '◇';

        public const char Circle_Solid = '●';
        public const char Circle_Border = '○';

        public const char CircleHalf_Up = '◓';
        public const char CircleHalf_Right = '◑';
        public const char CircleHalf_Down = '◒';
        public const char CircleHalf_Left = '◐';

        public const char CircleQuadrant_UpperLeft = '◜';
        public const char CircleQuadrant_UpperRight = '◝';
        public const char CircleQuadrant_LowerLeft = '◞';
        public const char CircleQuadrant_LowerRight = '◟';
        public const char CircleQuadrant_Upper = '◠';
        public const char CircleQuadrant_Lower = '◡';
    }

    public static class Arrows
    {
        public const char Left = '←';
        public const char UpperLeft = '↖';
        public const char Up = '↑';
        public const char UpperRight = '↗';
        public const char Right = '→';
        public const char LowerRight = '↘';
        public const char Down = '↓';
        public const char LowerLeft = '↙';

        public const char Double_Left = '⟸';
        public const char Double_UpperLeft = '⇖';
        public const char Double_Up = '⇑';
        public const char Double_UpperRight = '⇗';
        public const char Double_Right = '⟹';
        public const char Double_LowerRight = '⇘';
        public const char Double_Down = '⇓';
        public const char Double_LowerLeft = '⇙';

        public const char Clockwise = '↷';
        public const char AntiClockwise = '↶';
        public const char ReDo = '↻';
        public const char UnDo = '↺';
    }

    public static class Misc
    {
        public const char Watch = '⌚';
        public const char Hourglass = '⌛';
        public const char Keyboard = '⌨';
        public const char Bell = '⍾';
        public const char Prev = '⎗';
        public const char Next = '⎘';
        public const char Alarm = '⏰';
        public const char Stopwatch = '⏱';
        public const char Sun = '☀';
        public const char Cloud = '☁';
        public const char Umbrella = '☂';
        public const char Snowman = '☃';
        public const char Star_Solid = '★';
        public const char Star_Border = '☆';
        public const char Telephone = '☎';
        public const char Ballot_Box = '☐';
        public const char Ballot_Tick = '☑';
        public const char Ballot_Cross = '☒';
        public const char Rain = '☔';
        public const char Coffee = '☕';
        public const char SkullCrossbones = '☠';
        public const char RadioActive = '☢';
        public const char BioHazard = '☣';
        public const char Peace = '☮';
        public const char YinYang = '☯';
        public const char Sad = '☹';
        public const char Happy = '☺';
        public const char Spade = '♠';
        public const char Heart = '♡';
        public const char Diamond = '♢';
        public const char Clubs = '♣';
        public const char Recycle = '♺';
        public const char Warning = '⚠';
        public const char Thunder = '⚡';
        public const char NoEntry = '⛔';
    }

    public static class Dice
    {
        public const char One = '⚀';
        public const char Two = '⚁';
        public const char Three = '⚂';
        public const char Four = '⚃';
        public const char Five = '⚄';
        public const char Six = '⚅';
    }
}

// ReSharper restore InconsistentNaming

public class Canvas
{
    private const char DefaultChar = ' '; // (char)0x2588;

    /// The current canvas state
    public Array2D<Glyph> Glyphs { get; private set; }

    public int Width => Glyphs.Width;
    public int Height => Glyphs.Height;
    public Vec Size => Glyphs.Size;

    public Color Background { get; set; } = Color.Black;
    public Color Foreground { get; set; } = Color.White;

    public Vec Cursor { get; set; } = Vec.Unknown;

    private readonly Stack<Rect> _clips = new Stack<Rect>();
    private Rect? _clip = null;

    public Canvas(int width, int height)
    {
        Glyphs = new Array2D<Glyph>(width, height, Glyph.Clear);
    }

    public void Clear()
    {
        Fill(0, 0, Width, Height);
    }

    private bool IsOutsideClip(int x, int y)
    {
        int clipX = _clip?.Left ?? 0;
        int clipY = _clip?.Top ?? 0;
        int clipMaxX = clipX + (_clip?.Width ?? Width);
        int clipMaxY = clipY + (_clip?.Height ?? Height);

        if (x < clipX) return true;
        if (x >= clipMaxX) return true;
        if (y < clipY) return true;
        if (y >= clipMaxY) return true;

        return false;
    }

    void SetGlyph(int x, int y, Glyph glyph)
    {
        if (IsOutsideClip(x, y))
            return;

        glyph.OffsetX = glyph.OffsetY = 0;
        if (glyph.Chr == 0)
        {
            var oldGlyph = Glyphs.Get(x, y);
            glyph = new Glyph(oldGlyph.Chr, oldGlyph.Fore, glyph.Back);
        }
        Glyphs.Set(x, y, glyph);
    }
    void SetGlyph(float x, float y, Glyph glyph)
    {
        var intX = (int)x;
        var intY = (int)y;

        if (IsOutsideClip(intX, intY))
            return;

        glyph.OffsetX = x - intX;
        glyph.OffsetY = y - intY;
        if (glyph.Chr == 0)
        {
            var oldGlyph = Glyphs.Get(intX, intY);
            glyph = new Glyph(oldGlyph.Chr, oldGlyph.Fore, glyph.Back);
        }
        Glyphs.Set(intX, intY, glyph);
    }

    public void WriteAt(int x, int y, string text, Color? foreground = null, Color? background = null)
    {
        foreground ??= Foreground;
        background ??= Background;

        if (x < 0) x = Width - text.Length;

        for (var i = 0; i < text.Length; i++)
        {
            if (x + i >= Width) break;
            SetGlyph(x + i, y, new Glyph(text[i], foreground, background));
        }
    }

    public void WriteAt(int x, int y, char chr, Color? foreground = null, Color? background = null)
    {
        foreground ??= Foreground;
        background ??= Background;

        SetGlyph(x, y, new Glyph(chr, foreground, background));
    }
    public void WriteAt(float x, float y, char chr, Color? foreground = null, Color? background = null)
    {
        foreground ??= Foreground;
        background ??= Background;

        SetGlyph(x, y, new Glyph(chr, foreground, background));
    }

    public void Resize(int wide, int high)
    {
        var newGlyphs = new Array2D<Glyph>(wide, high, Glyph.Clear);

        for (int x = 0; x < Math.Min(Glyphs.Width, wide); x++)
        {
            for (int y = 0; y < Math.Min(Glyphs.Height, high); y++)
            {
                newGlyphs.Set(x, y, Glyphs.Get(x, y));
            }
        }
        Glyphs = newGlyphs;
    }

    public override string ToString()
    {
        var sb = new StringBuilder(Glyphs.Width * Glyphs.Height + Glyphs.Height);

        for (int y = 0; y < Glyphs.Height; y++)
        {
            var line = new StringBuilder(Glyphs.Width + 2);
            for (int x = 0; x < Width; x++)
            {
                var g = Glyphs.Get(x, y);
                if (g == Glyph.Clear || g.Chr == 0)
                    line.Append(' ');
                else
                    line.Append(g.Chr);
            }
            sb.Append(line.ToString().TrimEnd());
            if (y < Glyphs.Height - 1)
                sb.AppendLine();
        }

        return sb.ToString().TrimEnd('\r', '\n');
    }

    public static readonly Dictionary<ExChar, char> UnicodeChar =
new Dictionary<ExChar, char>
{
    [ExChar.BoxDoubleUpLeft] = '╔',
    [ExChar.BoxDoubleUpRight] = '╗',
    [ExChar.BoxDoubleDownLeft] = '╚',
    [ExChar.BoxDoubleDownRight] = '╝',
    [ExChar.BoxDoubleHorz] = '═',
    [ExChar.BoxDoubleVert] = '║',
    [ExChar.BoxDoubleCross] = '╬',
    [ExChar.BoxDoubleLeftTee] = '╠',
    [ExChar.BoxDoubleRightTee] = '╣',
    [ExChar.BoxDoubleTopTee] = '╦',
    [ExChar.BoxDoubleBottomTee] = '╩',
    [ExChar.LineDoubleLeft] = '╸',
    [ExChar.LineDoubleTop] = '╹',
    [ExChar.LineDoubleRight] = '╺',
    [ExChar.LineDoubleDown] = '╻',
    [ExChar.BoxSingleUpLeft] = '┌',
    [ExChar.BoxSingleUpRight] = '┐',
    [ExChar.BoxSingleDownLeft] = '└',
    [ExChar.BoxSingleDownRight] = '┘',
    [ExChar.BoxSingleHorz] = '─',
    [ExChar.BoxSingleVert] = '│',
    [ExChar.BoxSingleCross] = '┼',
    [ExChar.BoxSingleLeftTee] = '├',
    [ExChar.BoxSingleRightTee] = '┤',
    [ExChar.BoxSingleTopTee] = '┬',
    [ExChar.BoxSingleBottomTee] = '┴',
    [ExChar.LineSingleLeft] = '╴',
    [ExChar.LineSingleTop] = '╵',
    [ExChar.LineSingleRight] = '╶',
    [ExChar.LineSingleDown] = '╷',
    [ExChar.BoxDoubleHorzUpLeft] = '╒',
    [ExChar.BoxDoubleHorzUpRight] = '╕',
    [ExChar.BoxDoubleHorzDownLeft] = '╘',
    [ExChar.BoxDoubleHorzDownRight] = '╛',
    [ExChar.BoxDoubleHorzHorz] = '═',
    [ExChar.BoxDoubleHorzVert] = '│',
    [ExChar.BoxDoubleHorzCross] = '╪',
    [ExChar.BoxDoubleHorzLeftTee] = '╞',
    [ExChar.BoxDoubleHorzRightTee] = '╡',
    [ExChar.BoxDoubleHorzTopTee] = '╤',
    [ExChar.BoxDoubleHorzBottomTee] = '╧',
    [ExChar.BoxSingleHorzUpLeft] = '╓',
    [ExChar.BoxSingleHorzUpRight] = '╖',
    [ExChar.BoxSingleHorzDownLeft] = '╙',
    [ExChar.BoxSingleHorzDownRight] = '╜',
    [ExChar.BoxSingleHorzHorz] = '─',
    [ExChar.BoxSingleHorzVert] = '║',
    [ExChar.BoxSingleHorzCross] = '╫',
    [ExChar.BoxSingleHorzLeftTee] = '╟',
    [ExChar.BoxSingleHorzRightTee] = '╢',
    [ExChar.BoxSingleHorzTopTee] = '╥',
    [ExChar.BoxSingleHorzBottomTee] = '╨',
    [ExChar.AcuteAccent] = '´',
    [ExChar.BlockDown] = '▄',
    [ExChar.BlockFull] = '█',
    [ExChar.BlockHigh] = '▓',
    [ExChar.BlockLow] = '░',
    [ExChar.BlockMed] = '▒',
    [ExChar.BlockUp] = '▀',
    [ExChar.BlockLeft] = '▌',
    [ExChar.BlockRight] = '▐',
    [ExChar.Square] = '■',
    [ExChar.Cedilla] = '¸',
    [ExChar.Cent] = '¢',
    [ExChar.Congruence] = '≡',
    [ExChar.Copyright] = '©',
    [ExChar.Degree] = '°',
    [ExChar.Diaresis] = '¨',
    [ExChar.DiphthongLower] = 'æ',
    [ExChar.DiphthongUpper] = 'Æ',
    [ExChar.Division] = '÷',
    [ExChar.ExclamationMarkInverted] = '¡',
    [ExChar.Function] = 'ƒ',
    [ExChar.GenericCurrency] = '¤',
    [ExChar.LogicalNotation] = '¬',
    [ExChar.Macron] = '¯',
    [ExChar.Multiplication] = '×',
    [ExChar.OneHalf] = '½',
    [ExChar.OneQuarter] = '¼',
    [ExChar.Paragraph] = '¶',
    [ExChar.PlusMinus] = '±',
    [ExChar.Pound] = '£',
    [ExChar.QuestionMarkInverted] = '¿',
    [ExChar.QuoteMarkLeft] = '«',
    [ExChar.QuoteMarkRight] = '»',
    [ExChar.RegisteredTrademark] = '®',
    [ExChar.Selection] = '§',
    [ExChar.SlashZeroLower] = 'ø',
    [ExChar.SlashZeroUpper] = 'Ø',
    [ExChar.SpaceDot] = '·',
    [ExChar.SubscriptOne] = '¹',
    [ExChar.SubscriptThree] = '³',
    [ExChar.SubScriptTwo] = '²',
    [ExChar.ThreeQuarters] = '¾',
    [ExChar.Underline] = '‗',
    [ExChar.VertBroken] = '¦',
    [ExChar.Yen] = '¥',
};

    public static readonly Dictionary<ExChar, char> AsciiChar =
        new Dictionary<ExChar, char>
        {
            [ExChar.BoxDoubleUpLeft] = (char)201,
            [ExChar.BoxDoubleUpRight] = (char)187,
            [ExChar.BoxDoubleDownLeft] = (char)200,
            [ExChar.BoxDoubleDownRight] = (char)188,
            [ExChar.BoxDoubleHorz] = (char)205,
            [ExChar.BoxDoubleVert] = (char)186,
            [ExChar.BoxDoubleCross] = (char)206,
            [ExChar.BoxDoubleLeftTee] = (char)204,
            [ExChar.BoxDoubleRightTee] = (char)185,
            [ExChar.BoxDoubleTopTee] = (char)203,
            [ExChar.BoxDoubleBottomTee] = (char)202,
            [ExChar.BoxSingleUpLeft] = (char)218,
            [ExChar.BoxSingleUpRight] = (char)191,
            [ExChar.BoxSingleDownLeft] = (char)192,
            [ExChar.BoxSingleDownRight] = (char)217,
            [ExChar.BoxSingleHorz] = (char)196,
            [ExChar.BoxSingleVert] = (char)179,
            [ExChar.BoxSingleCross] = (char)197,
            [ExChar.BoxSingleLeftTee] = (char)195,
            [ExChar.BoxSingleRightTee] = (char)180,
            [ExChar.BoxSingleTopTee] = (char)194,
            [ExChar.BoxSingleBottomTee] = (char)193,
            [ExChar.BoxDoubleHorzUpLeft] = (char)213,
            [ExChar.BoxDoubleHorzUpRight] = (char)184,
            [ExChar.BoxDoubleHorzDownLeft] = (char)212,
            [ExChar.BoxDoubleHorzDownRight] = (char)190,
            [ExChar.BoxDoubleHorzHorz] = (char)196,
            [ExChar.BoxDoubleHorzVert] = (char)179,
            [ExChar.BoxDoubleHorzCross] = (char)216,
            [ExChar.BoxDoubleHorzLeftTee] = (char)198,
            [ExChar.BoxDoubleHorzRightTee] = (char)181,
            [ExChar.BoxDoubleHorzTopTee] = (char)209,
            [ExChar.BoxDoubleHorzBottomTee] = (char)207,
            [ExChar.BoxSingleHorzUpLeft] = (char)214,
            [ExChar.BoxSingleHorzUpRight] = (char)183,
            [ExChar.BoxSingleHorzDownLeft] = (char)211,
            [ExChar.BoxSingleHorzDownRight] = (char)189,
            [ExChar.BoxSingleHorzHorz] = (char)196,
            [ExChar.BoxSingleHorzVert] = (char)179,
            [ExChar.BoxSingleHorzCross] = (char)215,
            [ExChar.BoxSingleHorzLeftTee] = (char)199,
            [ExChar.BoxSingleHorzRightTee] = (char)182,
            [ExChar.BoxSingleHorzTopTee] = (char)210,
            [ExChar.BoxSingleHorzBottomTee] = (char)208,
            [ExChar.AcuteAccent] = (char)239,
            [ExChar.BlockDown] = (char)220,
            [ExChar.BlockFull] = (char)219,
            [ExChar.BlockHigh] = (char)178,
            [ExChar.BlockLow] = (char)177,
            [ExChar.BlockMed] = (char)176,
            [ExChar.BlockUp] = (char)223,
            [ExChar.BlockLeft] = (char)221,
            [ExChar.BlockRight] = (char)222,
            [ExChar.Square] = (char)254,
            [ExChar.Cedilla] = (char)247,
            [ExChar.Cent] = (char)155,
            [ExChar.Congruence] = (char)240,
            [ExChar.Copyright] = (char)184,
            [ExChar.Degree] = (char)167,
            [ExChar.Diaresis] = (char)249,
            [ExChar.DiphthongLower] = (char)145,
            [ExChar.DiphthongUpper] = (char)146,
            [ExChar.Division] = (char)246,
            [ExChar.ExclamationMarkInverted] = (char)173,
            [ExChar.Function] = (char)159,
            [ExChar.GenericCurrency] = (char)207,
            [ExChar.LogicalNotation] = (char)170,
            [ExChar.Macron] = (char)238,
            [ExChar.Multiplication] = (char)158,
            [ExChar.OneHalf] = (char)171,
            [ExChar.OneQuarter] = (char)172,
            [ExChar.Paragraph] = (char)244,
            [ExChar.PlusMinus] = (char)241,
            [ExChar.Pound] = (char)156,
            [ExChar.QuestionMarkInverted] = (char)168,
            [ExChar.QuoteMarkLeft] = (char)174,
            [ExChar.QuoteMarkRight] = (char)175,
            [ExChar.RegisteredTrademark] = (char)169,
            [ExChar.Selection] = (char)245,
            [ExChar.SlashZeroLower] = (char)155,
            [ExChar.SlashZeroUpper] = (char)157,
            [ExChar.SpaceDot] = (char)250,
            [ExChar.SubscriptOne] = (char)251,
            [ExChar.SubscriptThree] = (char)252,
            [ExChar.SubScriptTwo] = (char)253,
            [ExChar.ThreeQuarters] = (char)243,
            [ExChar.Underline] = (char)242,
            [ExChar.VertBroken] = (char)221,
            [ExChar.Yen] = (char)157,
        };

    //**************************************************
    //             '╔' '╗' '╚' '╝' '═' '║' '╬' '╠' '╣' '╦' '╩'
    // Double:     201 187 200 188 205 186 206 204 185 203 202

    //             '┌' '┐' '└' '┘' '─' '│' '┼' '├' '┤' '┬' '┴'
    // Single:     218 191 192 217 196 179 197 195 180 194 193

    //             '╒' '╕' '╘' '╛' '═' '│' '╪' '╞' '╡' '╤' '╧' 
    // DoubleHorz: 213 184 212 190 196 179 216 198 181 209 207

    //             '╓' '╖' '╙' '╜' '─' '║' '╫' '╟' '╢' '╥' '╨' 
    // SingleHorz: 214 183 211 189 196 179 215 199 182 210 208
    //**************************************************

    private static readonly LineDrawSet DoubleLine = new LineDrawSet(" ╥╺╔╨║╚╠╸╗═╦╝╣╩╬");
    private static readonly LineDrawSet SingleLine = new LineDrawSet(" ╷╶┌╵│└├╴┐─┬┘┤┴┼");
    private static readonly LineDrawSet RoundLine = new LineDrawSet(" ╷╶╭╵│╰├╴╮─┬╯┤┴┼");
    private static readonly LineDrawSet DoubleHorz = new LineDrawSet(" ╻╺╒╵│╘╞╸╕═╤╛╡╧╪");
    private static readonly LineDrawSet SingleHorz = new LineDrawSet(" ╷╶╓╹║╙╟╴╖─╥╜╢╨╫");
    private static readonly LineDrawSet NoLine = new LineDrawSet("                ");

    //**************************************************

    public void FillRect(Rect rect, char? chr = null, Color? foreground = null, Color? background = null)
    {
        int left = rect.Left;
        int right = rect.Right;
        int top = rect.Top;
        int bottom = rect.Bottom;

        for (int x = left; x <= right; x++)
        {
            for (int y = top; y <= bottom; y++)
            {
                var oldGlyph = Glyphs.Get(x, y);
                WriteAt(x, y,
                                 chr ?? oldGlyph.Chr,
                                 foreground ?? oldGlyph.Fore,
                                 background ?? oldGlyph.Back);
            }
        }
    }

    private bool IsLine(char ch)
    {
        return DoubleLine.Contains(ch) ||
            SingleLine.Contains(ch) ||
            DoubleHorz.Contains(ch) ||
            SingleHorz.Contains(ch);
    }

    private LineDrawSet? GetLineDrawSet(LineSet lineSet)
    {
        return lineSet switch
        {
            LineSet.Single => SingleLine,
            LineSet.Double => DoubleLine,
            LineSet.Rounded => RoundLine,
            LineSet.SingleHorz => SingleHorz,
            LineSet.DoubleHorz => DoubleHorz,
            LineSet.DoubleOver => SingleLine,
            LineSet.DoubleUnder => SingleLine,
            LineSet.DoubleRaised => SingleLine,
            LineSet.DoublePressed => SingleLine,
            LineSet.None => NoLine,
            _ => null
        };
    }

    public void DrawLine(int startX, int startY, int endX, int endY,
                          LineSet lineSet = LineSet.Single,
                          Color? foreground = null,
                          Color? background = null)
    {
        LineDrawSet lineDraw = GetLineDrawSet(lineSet) ?? throw new ArgumentException(nameof(lineSet));

        // Force line to go left to right
        if (endX < startX)
        {
            var tmp = startX;
            startX = endX;
            endX = tmp;
            tmp = startY;
            startY = endY;
            endY = tmp;
        }
        var width = endX - startX; // always zero or positive
        var height = endY - startY; // may be negative
        bool goingUp = endY < startY;
        bool horizontal = width > Math.Abs(height);

        float stepX;
        float stepY;
        int stepCount;

        if (horizontal)
        {
            stepX = 1f;
            if (height < 0)
                stepY = (height - 0.9f) / width;
            else if (height == 0)
                stepY = 0;
            else
                stepY = (height + 0.9f) / width;

            stepCount = width + 1;
        }
        else
        {
            stepX = width == 0 ? 0 : (width + 0.9f) / height;
            stepY = goingUp ? -1f : 1f;
            stepCount = Math.Abs(height) + 1;
        }
        int lastX = -1;
        int lastY = -1;
        for (int step = 0; step < stepCount; step++)
        {
            var x = startX + stepX * step;
            var y = startY + stepY * step;

            var yToCompare = (int)(goingUp
                                        ? Math.Ceiling(y)
                                        : y);

            char chr;
            if (horizontal)
            {
                if (step == 0)
                    chr = lineDraw.HorzStart;
                else if (lastY != yToCompare)
                {
                    if (goingUp)
                    {
                        chr = TryCombine((int)x, lastY, lineDraw.BotRight, lineSet);
                        WriteAt(x, lastY, chr, foreground, background);
                    }
                    else
                    {
                        chr = TryCombine((int)x, lastY, lineDraw.TopRight, lineSet);
                        WriteAt(x, lastY, chr, foreground, background);
                    }
                    chr = goingUp ? lineDraw.TopLeft : lineDraw.BotLeft;
                }
                else if (step == stepCount - 1)
                    chr = lineDraw.HorzEnd;
                else
                    chr = lineDraw.Horz;
            }
            else
            {
                if (step == 0)
                    chr = goingUp ? lineDraw.VertEnd : lineDraw.VertStart;
                else if (lastX != (int)x)
                {
                    chr = TryCombine(lastX, (int)y, goingUp ? lineDraw.TopRight : lineDraw.BotLeft, lineSet);
                    WriteAt(lastX, y, chr, foreground, background);
                    chr = goingUp ? lineDraw.BotLeft : lineDraw.TopRight;
                }
                //else if (nextX != (int)x)
                //    chr = lineDraw.BotLeft;
                else if (step == stepCount - 1)
                    chr = goingUp ? lineDraw.VertStart : lineDraw.VertEnd;
                else
                    chr = lineDraw.Vert;
            }
            chr = TryCombine((int)x, yToCompare, chr, lineSet);
            WriteAt(x, yToCompare, chr, foreground, background);

            lastX = (int)x;
            lastY = yToCompare;
        }
    }

    public void DrawRect(
        Rect rect,
        LineSet lineSet = LineSet.Single,
        Color? foreground = null,
        Color? background = null)
    {
        DrawRect(rect.Left, rect.Top, rect.Right, rect.Bottom, lineSet, foreground, background);
    }

    public void DrawRect(
        int left,
        int top,
        int right,
        int bottom,
        LineSet lineSet = LineSet.Single,
        Color? foreground = null,
        Color? background = null)

    {
        if (lineSet == LineSet.DoubleUnder ||
            lineSet == LineSet.DoubleOver ||
            lineSet == LineSet.DoubleRaised ||
            lineSet == LineSet.DoublePressed)
        {
            // Draw singles
            // Top
            if (lineSet == LineSet.DoubleUnder || lineSet == LineSet.DoubleRaised)
                DrawLine(left, top, right, top, LineSet.Single, foreground, background);
            // Left
            if (lineSet == LineSet.DoubleUnder || lineSet == LineSet.DoubleOver || lineSet == LineSet.DoubleRaised)
                DrawLine(left, top, left, bottom, LineSet.Single, foreground, background);
            // Right
            if (lineSet == LineSet.DoubleUnder || lineSet == LineSet.DoubleOver || lineSet == LineSet.DoublePressed)
                DrawLine(right, top, right, bottom, LineSet.Single, foreground, background);
            // Bottom
            if (lineSet == LineSet.DoubleOver || lineSet == LineSet.DoublePressed)
                DrawLine(left, bottom, right, bottom, LineSet.Single, foreground, background);

            // Draw Doubles
            // Top
            if (lineSet == LineSet.DoubleOver || lineSet == LineSet.DoublePressed)
                DrawLine(left, top, right, top, LineSet.Double, foreground, background);
            // Left
            if (lineSet == LineSet.DoublePressed)
                DrawLine(left, top, left, bottom, LineSet.Double, foreground, background);
            // Right
            if (lineSet == LineSet.DoubleRaised)
                DrawLine(right, top, right, bottom, LineSet.Double, foreground, background);
            // Bottom
            if (lineSet == LineSet.DoubleUnder || lineSet == LineSet.DoubleRaised)
                DrawLine(left, bottom, right, bottom, LineSet.Double, foreground, background);

            return;
        }

        LineDrawSet lineDraw = GetLineDrawSet(lineSet) ?? throw new ArgumentException(nameof(lineSet));

        for (int x = left; x <= right; x++)
        {
            for (int y = top; y <= bottom; y++)
            {
                char chr = ' ';
                if (x == left)
                {
                    if (y == top)
                        chr = lineDraw.TopLeft;
                    else if (y == bottom)
                        chr = lineDraw.BotLeft;
                    else
                        chr = lineDraw.Vert;
                }
                else if (x == right)
                {
                    if (y == top)
                        chr = lineDraw.TopRight;
                    else if (y == bottom)
                        chr = lineDraw.BotRight;
                    else
                        chr = lineDraw.Vert;
                }
                else if (y == top || y == bottom)
                    chr = lineDraw.Horz;
                else if (background == null)
                    continue;

                chr = TryCombine(x, y, chr, lineSet);
                WriteAt(x, y, chr, foreground, background);
            }
        }
    }

    private void Swap<T>(ref T x, ref T y)
    {
        T t = x;
        x = y;
        y = t;
    }

    private char TryCombine(int x, int y, char ch, LineSet lineSet)
    {
        // Can we combine this character with whatever is underneath
        var oldGlyph = Glyphs.Get(x, y);
        var oldCh = oldGlyph.Chr;

        if (IsLine(oldCh))
        {
            if (lineSet == LineSet.Single && SingleLine.Contains(oldCh))
                return SingleLine.Combine(oldCh, ch);

            if (lineSet == LineSet.Rounded && RoundLine.Contains(oldCh))
                return RoundLine.Combine(oldCh, ch);

            if (lineSet == LineSet.Double && DoubleLine.Contains(oldCh))
                return DoubleLine.Combine(oldCh, ch);

            return TryCombine(oldCh, ch);
        }

        return ch;
    }

    private char TryCombine(char oldCh, char ch)
    {
        int underIndex = -1;
        int overIndex = -1;
        if (DoubleLine.Contains(oldCh))
        {
            underIndex = DoubleLine.IndexOf(oldCh);
            overIndex = SingleLine.IndexOf(ch);
        }
        else if (SingleLine.Contains(oldCh))
        {
            underIndex = SingleLine.IndexOf(oldCh);
            overIndex = DoubleLine.IndexOf(ch);
        }
        else if (RoundLine.Contains(oldCh))
        {
            underIndex = RoundLine.IndexOf(oldCh);
            overIndex = DoubleLine.IndexOf(ch);
        }

        if (underIndex >= 0 && overIndex >= 0)
        {
            if (ch == SingleLine.Horz ||
                 ch == SingleLine.HorzStart ||
                 ch == SingleLine.HorzEnd ||
                 ch == DoubleLine.Vert ||
                 ch == DoubleLine.VertStart ||
                 ch == DoubleLine.VertEnd)
                return SingleHorz[underIndex | overIndex];
            if (ch == DoubleLine.Horz ||
                 ch == DoubleLine.HorzStart ||
                 ch == DoubleLine.HorzEnd ||
                 ch == SingleLine.Vert ||
                 ch == SingleLine.VertStart ||
                 ch == SingleLine.VertEnd)
                return DoubleHorz[underIndex | overIndex];
        }

        return ch;
    }

    public virtual void Draw(int x, int y, char c = DefaultChar, Color? foreground = null, Color? background = null)
    {
        SetGlyph(x, y, new Glyph(c, foreground, background));
    }

    public void Fill(
        int x,
        int y,
        int width,
        int height,
        char c = DefaultChar,
        Color? foreground = null,
        Color? background = null)
    {
        background ??= Background;
        foreground ??= Foreground;

        var glyph = new Glyph(DefaultChar, foreground, background);

        for (var py = y; py < y + height; py++)
        {
            for (var px = x; px < x + width; px++)
            {
                SetGlyph(px, py, glyph);
            }
        }
    }

    public void DrawString(int x, int y, string str, Color? foreground = null, Color? background = null)
    {
        for (int i = 0; i < str.Length; i++)
            SetGlyph(x + i, y, new Glyph(str[i], foreground, background));
    }

    public void DrawStringAlpha(int x, int y, string c, Color? foreground = null, Color? background = null)
    {
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] != ' ')
                SetGlyph(x + i, y, new Glyph(c[i], foreground, background));
        }
    }

    public void Clip(ref int x, ref int y)
    {
        if (x < 0)
            x = 0;
        if (x >= Glyphs.Width)
            x = Glyphs.Width;
        if (y < 0)
            y = 0;
        if (y >= Glyphs.Height)
            y = Glyphs.Height;
    }

    public void DrawLine(int x1, int y1, int x2, int y2, char c = DefaultChar, Color? foreground = null, Color? background = null)
    {
        int x, y, dx, dy, dx1, dy1, px, py, xe, ye;
        dx = x2 - x1; dy = y2 - y1;
        dx1 = Math.Abs(dx); dy1 = Math.Abs(dy);
        px = 2 * dy1 - dx1; py = 2 * dx1 - dy1;
        if (dy1 <= dx1)
        {
            if (dx >= 0)
            { x = x1; y = y1; xe = x2; }
            else
            { x = x2; y = y2; xe = x1; }

            Draw(x, y, c, foreground, background);

            while (x < xe)
            {
                x = x + 1;
                if (px < 0)
                    px = px + 2 * dy1;
                else
                {
                    if (dx < 0 && dy < 0 || dx > 0 && dy > 0) y = y + 1; else y = y - 1;
                    px = px + 2 * (dy1 - dx1);
                }
                Draw(x, y, c, foreground, background);
            }
        }
        else
        {
            if (dy >= 0)
            { x = x1; y = y1; ye = y2; }
            else
            { x = x2; y = y2; ye = y1; }

            Draw(x, y, c, foreground, background);

            while (y < ye)
            {
                y = y + 1;
                if (py <= 0)
                    py = py + 2 * dx1;
                else
                {
                    if (dx < 0 && dy < 0 || dx > 0 && dy > 0) x = x + 1; else x = x - 1;
                    py = py + 2 * (dx1 - dy1);
                }
                Draw(x, y, c, foreground, background);
            }
        }
    }

    public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, char c, Color? foreground = null, Color? background = null)
    {
        DrawLine(x1, y1, x2, y2, c, foreground, background);
        DrawLine(x2, y2, x3, y3, c, foreground, background);
        DrawLine(x3, y3, x1, y1, c, foreground, background);
    }

    // https://www.avrfreaks.net/sites/default/files/triangles.c
    public void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3, char c, Color? foreground = null, Color? background = null)
    {
        int t1x, t2x, y, minx, maxx, t1xp, t2xp;
        bool changed1 = false;
        bool changed2 = false;
        int signx1, signx2;
        int e1, e2;

        // Sort vertices
        if (y1 > y2) { Swap(ref y1, ref y2); Swap(ref x1, ref x2); }
        if (y1 > y3) { Swap(ref y1, ref y3); Swap(ref x1, ref x3); }
        if (y2 > y3) { Swap(ref y2, ref y3); Swap(ref x2, ref x3); }

        t1x = t2x = x1;
        y = y1;   // Starting points
        var dx1 = x2 - x1;
        if (dx1 < 0)
        {
            dx1 = -dx1;
            signx1 = -1;
        }
        else
            signx1 = 1;
        var dy1 = y2 - y1;

        var dx2 = x3 - x1;
        if (dx2 < 0)
        {
            dx2 = -dx2;
            signx2 = -1;
        }
        else
            signx2 = 1;
        var dy2 = y3 - y1;

        if (dy1 > dx1)
        {   // swap values
            Swap(ref dx1, ref dy1);
            changed1 = true;
        }
        if (dy2 > dx2)
        {   // swap values
            Swap(ref dy2, ref dx2);
            changed2 = true;
        }

        e2 = dx2 >> 1;
        // Flat top, just process the second half
        if (y1 == y2) goto next;
        e1 = dx1 >> 1;

        for (int i = 0; i < dx1;)
        {
            t1xp = 0; t2xp = 0;
            if (t1x < t2x)
            {
                minx = t1x;
                maxx = t2x;
            }
            else
            {
                minx = t2x;
                maxx = t1x;
            }
            // process first line until y value is about to change
            while (i < dx1)
            {
                i++;
                e1 += dy1;
                while (e1 >= dx1)
                {
                    e1 -= dx1;
                    if (changed1)
                        t1xp = signx1;
                    else
                        goto next1;
                }
                if (changed1)
                    break;
                t1x += signx1;
            }
        next1:

            // process second line until y value is about to change
            while (true)
            {
                e2 += dy2;
                while (e2 >= dx2)
                {
                    e2 -= dx2;
                    if (changed2) t2xp = signx2;
                    else goto next2;
                }
                if (changed2)
                    break;
                t2x += signx2;
            }
        next2:

            if (minx > t1x)
                minx = t1x;
            if (minx > t2x)
                minx = t2x;
            if (maxx < t1x)
                maxx = t1x;
            if (maxx < t2x)
                maxx = t2x;
            Drawline(minx, maxx, y, c, foreground, background);    // Draw line from min to max points found on the y
                                                                 // Now increase y
            if (!changed1)
                t1x += signx1;
            t1x += t1xp;
            if (!changed2)
                t2x += signx2;
            t2x += t2xp;
            y += 1;
            if (y == y2)
                break;

        }
    next:

        // Second half
        dx1 = x3 - x2;
        if (dx1 < 0)
        {
            dx1 = -dx1;
            signx1 = -1;
        }
        else
            signx1 = 1;
        dy1 = y3 - y2;
        t1x = x2;

        if (dy1 > dx1)
        {   // swap values
            Swap(ref dy1, ref dx1);
            changed1 = true;
        }
        else
            changed1 = false;

        e1 = dx1 >> 1;

        for (int i = 0; i <= dx1; i++)
        {
            t1xp = 0; t2xp = 0;
            if (t1x < t2x)
            {
                minx = t1x;
                maxx = t2x;
            }
            else
            {
                minx = t2x;
                maxx = t1x;
            }
            // process first line until y value is about to change
            while (i < dx1)
            {
                e1 += dy1;
                while (e1 >= dx1)
                {
                    e1 -= dx1;

                    if (changed1)
                    {
                        t1xp = signx1;
                        break;
                    } //t1x += signx1;
                    goto next3;
                }
                if (changed1)
                    break;
                t1x += signx1;
                if (i < dx1)
                    i++;
            }
        next3:

            // process second line until y value is about to change
            while (t2x != x3)
            {
                e2 += dy2;
                while (e2 >= dx2)
                {
                    e2 -= dx2;
                    if (changed2)
                        t2xp = signx2;
                    else
                        goto next4;
                }
                if (changed2)
                    break;
                t2x += signx2;
            }
        next4:

            if (minx > t1x)
                minx = t1x;
            if (minx > t2x)
                minx = t2x;
            if (maxx < t1x)
                maxx = t1x;
            if (maxx < t2x)
                maxx = t2x;
            Drawline(minx, maxx, y, c, foreground, background);
            if (!changed1)
                t1x += signx1;
            t1x += t1xp;
            if (!changed2)
                t2x += signx2;
            t2x += t2xp;
            y += 1;
            if (y > y3)
                return;
        }
    }

    public void DrawCircle(int xc, int yc, int r, char c, Color? foreground = null, Color? background = null)
    {
        int x = 0;
        int y = r;
        int p = 3 - 2 * r;
        if (r == 0) return;

        while (y >= x) // only formulate 1/8 of circle
        {
            Draw(xc - x, yc - y, c, foreground, background);   //upper left left
            Draw(xc - y, yc - x, c, foreground, background);   //upper upper left
            Draw(xc + y, yc - x, c, foreground, background);   //upper upper right
            Draw(xc + x, yc - y, c, foreground, background);   //upper right right
            Draw(xc - x, yc + y, c, foreground, background);   //lower left left
            Draw(xc - y, yc + x, c, foreground, background);   //lower lower left
            Draw(xc + y, yc + x, c, foreground, background);   //lower lower right
            Draw(xc + x, yc + y, c, foreground, background);   //lower right right
            if (p < 0)
                p += 4 * x++ + 6;
            else
                p += 4 * (x++ - y--) + 10;
        }
    }

    public void FillCircle(int xc, int yc, int r, char c, Color? foreground = null, Color? background = null)
    {
        int x = 0;
        int y = r;
        int p = 3 - 2 * r;
        if (r == 0)
            return;

        while (y >= x)
        {
            Drawline(xc - x, xc + x, yc - y, c, foreground, background);
            Drawline(xc - y, xc + y, yc - x, c, foreground, background);
            Drawline(xc - x, xc + x, yc + y, c, foreground, background);
            Drawline(xc - y, xc + y, yc + x, c, foreground, background);
            if (p < 0) p += 4 * x++ + 6;
            else p += 4 * (x++ - y--) + 10;
        }
    }

    private void Drawline(int sx, int ex, int ny, char c, Color? foreground = null, Color? background = null)
    {
        for (int i = sx; i <= ex; i++)
            Draw(i, ny, c, foreground, background);
    }

    public void DrawSprite(int x, int y, [NotNull] Sprite sprite)
    {
        for (int i = 0; i < sprite.Width; i++)
        {
            for (int j = 0; j < sprite.Height; j++)
            {
                if (sprite.GetGlyph(i, j) != ' ')
                    Draw(x + i, y + j, sprite.GetGlyph(i, j), sprite.GetForeColour(i, j));
            }
        }
    }

    public Sprite? ExportSprite(int x, int y, int width, int height)
    {
        if (x < 0 || y < 0 || x + width >= Width || y + height >= Height)
            return null;

        var sprite = new Sprite(width, height);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var glyph = Glyphs.Get(x + i, y + j);
                sprite.SetGlyph(i, j, glyph.Chr);
                sprite.SetColour(i, j, glyph.Fore, glyph.Back);
            }
        }

        return sprite;
    }

    public void DrawPartialSprite(int x, int y, [NotNull] Sprite sprite, int ox, int oy, int w, int h)
    {
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                if (sprite.GetGlyph(i + ox, j + oy) != ' ')
                    Draw(x + i, y + j, sprite.GetGlyph(i + ox, j + oy), sprite.GetForeColour(i + ox, j + oy));
            }
        }
    }

    public void DrawWireFrameModel(
        List<(float, float)> coordinates,
        float x,
        float y,
        float r = 0.0f,
        float s = 1.0f,
        Color? foreground = null,
        Color? background = null,
        char c = Pixel.Block.Solid)
    {
        int count = coordinates.Count;
        var transformedCoordinates = new (float, float)[count];

        for (int i = 0; i < count; i++)
        {
            float item1 = coordinates[i].Item1;
            float item2 = coordinates[i].Item2;

            // Rotate
            if (Math.Abs(r) > float.Epsilon)
            {
                item1 = (float)(coordinates[i].Item1 * Math.Cos(r) - coordinates[i].Item2 * Math.Sin(r));
                item2 = (float)(coordinates[i].Item1 * Math.Sin(r) + coordinates[i].Item2 * Math.Cos(r));
            }

            // Scale
            if (Math.Abs(s - 1.0) > float.Epsilon)
            {
                item1 *= s;
                item2 *= s;
            }

            // Translate
            transformedCoordinates[i] = (item1 + x, item2 + y);
        }

        // Draw Closed Polygon
        for (int i = 0; i < count + 1; i++)
        {
            int j = i + 1;
            DrawLine(
                     (int)transformedCoordinates[i % count].Item1,
                     (int)transformedCoordinates[i % count].Item2,
                     (int)transformedCoordinates[j % count].Item1,
                     (int)transformedCoordinates[j % count].Item2,
                     (char)c, foreground, background);
        }
    }

    internal bool PushClip(Rect? clip)
    {
        if (clip == null)
            return false;
        _clips.Push(clip);
        _clip = clip;
        return true;
    }

    internal void PopClip()
    {
        if (_clips.Count <= 0)
        {
            _clip = null;
            return;
        }
        _clip = _clips.Pop();
    }

    public void Overwrite(Canvas other)
    {
        var minWidth = Math.Min(Width, other.Width);
        var minHeight = Math.Min(Height, other.Height);

        for (int y = 0; y < minHeight; y++)
        {
            for (int x = 0; x < minWidth; x++)
            {
                var g = other.Glyphs[y, x];
                if (g?.Chr >= 32)
                    Glyphs[y, x] = other.Glyphs[y, x];
            }
        }
    }
}
