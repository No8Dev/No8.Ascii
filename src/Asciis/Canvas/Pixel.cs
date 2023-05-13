namespace No8.Ascii;

public static class Pixel
{
    public static class Block
    {
        public static readonly Rune LightShade  = (Rune)'░';
        public static readonly Rune MediumShade = (Rune)'▒';
        public static readonly Rune DarkShade   = (Rune)'▓';
        public static readonly Rune Solid       = (Rune)'█';

        public static readonly Rune QuadrantUpperLeft       = (Rune)'▘';
        public static readonly Rune QuadrantUpperRight      = (Rune)'▝';
        public static readonly Rune QuadrantLowerLeft       = (Rune)'▖';
        public static readonly Rune QuadrantLowerRight      = (Rune)'▗';
        public static readonly Rune QuadrantPointUpperLeft  = (Rune)'▛';
        public static readonly Rune QuadrantPointUpperRight = (Rune)'▜';
        public static readonly Rune QuadrantPointLowerLeft  = (Rune)'▙';
        public static readonly Rune QuadrantRight           = (Rune)'▐';
        public static readonly Rune QuadrantLeft            = (Rune)'▌';
        public static readonly Rune QuadrantUp              = (Rune)'▀';
        public static readonly Rune QuadrantDown            = (Rune)'▄';

    }

    public static class Shapes
    {
        public static readonly Rune SquareSolid   = (Rune)'■';
        public static readonly Rune SquareBorder  = (Rune)'□';
        public static readonly Rune SquareRounded = (Rune)'▢';

        public static readonly Rune TriangleSolidUp    = (Rune)'▲';
        public static readonly Rune TriangleSolidRight = (Rune)'▶';
        public static readonly Rune TriangleSolidDown  = (Rune)'▼';
        public static readonly Rune TriangleSolidLeft  = (Rune)'◀';

        public static readonly Rune TriangleBorderUp    = (Rune)'△';
        public static readonly Rune TriangleBorderRight = (Rune)'▷';
        public static readonly Rune TriangleBorderDown  = (Rune)'▽';
        public static readonly Rune TriangleBorderLeft  = (Rune)'◁';

        public static readonly Rune TriangleSolidUpperLeft  = (Rune)'◤';
        public static readonly Rune TriangleSolidUpperRight = (Rune)'◥';
        public static readonly Rune TriangleSolidLowerLeft  = (Rune)'◢';
        public static readonly Rune TriangleSolidLowerRight = (Rune)'◣';

        public static readonly Rune TriangleBorderUpperLeft  = (Rune)'◸';
        public static readonly Rune TriangleBorderUpperRight = (Rune)'◹';
        public static readonly Rune TriangleBorderLowerLeft  = (Rune)'◿';
        public static readonly Rune TriangleBorderLowerRight = (Rune)'◺';

        public static readonly Rune DiamondSolid  = (Rune)'◆';
        public static readonly Rune DiamondBorder = (Rune)'◇';

        public static readonly Rune CircleSolid  = (Rune)'●';
        public static readonly Rune CircleBorder = (Rune)'○';

        public static readonly Rune CircleHalfUp    = (Rune)'◓';
        public static readonly Rune CircleHalfRight = (Rune)'◑';
        public static readonly Rune CircleHalfDown  = (Rune)'◒';
        public static readonly Rune CircleHalfLeft  = (Rune)'◐';

        public static readonly Rune CircleQuadrantUpperLeft  = (Rune)'◜';
        public static readonly Rune CircleQuadrantUpperRight = (Rune)'◝';
        public static readonly Rune CircleQuadrantLowerLeft  = (Rune)'◞';
        public static readonly Rune CircleQuadrantLowerRight = (Rune)'◟';
        public static readonly Rune CircleQuadrantUpper      = (Rune)'◠';
        public static readonly Rune CircleQuadrantLower      = (Rune)'◡';
    }

    public static class Arrows
    {
        public static readonly Rune Left       = (Rune)'←';
        public static readonly Rune UpperLeft  = (Rune)'↖';
        public static readonly Rune Up         = (Rune)'↑';
        public static readonly Rune UpperRight = (Rune)'↗';
        public static readonly Rune Right      = (Rune)'→';
        public static readonly Rune LowerRight = (Rune)'↘';
        public static readonly Rune Down       = (Rune)'↓';
        public static readonly Rune LowerLeft  = (Rune)'↙';

        public static readonly Rune DoubleLeft       = (Rune)'⟸';
        public static readonly Rune DoubleUpperLeft  = (Rune)'⇖';
        public static readonly Rune DoubleUp         = (Rune)'⇑';
        public static readonly Rune DoubleUpperRight = (Rune)'⇗';
        public static readonly Rune DoubleRight      = (Rune)'⟹';
        public static readonly Rune DoubleLowerRight = (Rune)'⇘';
        public static readonly Rune DoubleDown       = (Rune)'⇓';
        public static readonly Rune DoubleLowerLeft  = (Rune)'⇙';

        public static readonly Rune Clockwise     = (Rune)'↷';
        public static readonly Rune AntiClockwise = (Rune)'↶';
        public static readonly Rune ReDo          = (Rune)'↻';
        public static readonly Rune UnDo          = (Rune)'↺';
    }

    public static class Misc
    {
        public static readonly Rune Watch           = (Rune)'⌚';
        public static readonly Rune Hourglass       = (Rune)'⌛';
        public static readonly Rune Keyboard        = (Rune)'⌨';
        public static readonly Rune Bell            = (Rune)'⍾';
        public static readonly Rune Prev            = (Rune)'⎗';
        public static readonly Rune Next            = (Rune)'⎘';
        public static readonly Rune Alarm           = (Rune)'⏰';
        public static readonly Rune Stopwatch       = (Rune)'⏱';
        public static readonly Rune Sun             = (Rune)'☀';
        public static readonly Rune Cloud           = (Rune)'☁';
        public static readonly Rune Umbrella        = (Rune)'☂';
        public static readonly Rune Snowman         = (Rune)'☃';
        public static readonly Rune StarSolid       = (Rune)'★';
        public static readonly Rune StarBorder      = (Rune)'☆';
        public static readonly Rune Telephone       = (Rune)'☎';
        public static readonly Rune BallotBox       = (Rune)'☐';
        public static readonly Rune BallotTick      = (Rune)'☑';
        public static readonly Rune BallotCross     = (Rune)'☒';
        public static readonly Rune Rain            = (Rune)'☔';
        public static readonly Rune Coffee          = (Rune)'☕';
        public static readonly Rune SkullCrossbones = (Rune)'☠';
        public static readonly Rune RadioActive     = (Rune)'☢';
        public static readonly Rune BioHazard       = (Rune)'☣';
        public static readonly Rune Peace           = (Rune)'☮';
        public static readonly Rune YinYang         = (Rune)'☯';
        public static readonly Rune Sad             = (Rune)'☹';
        public static readonly Rune Happy           = (Rune)'☺';
        public static readonly Rune Spade           = (Rune)'♠';
        public static readonly Rune Heart           = (Rune)'♡';
        public static readonly Rune Diamond         = (Rune)'♢';
        public static readonly Rune Clubs           = (Rune)'♣';
        public static readonly Rune Recycle         = (Rune)'♺';
        public static readonly Rune Warning         = (Rune)'⚠';
        public static readonly Rune Thunder         = (Rune)'⚡';
        public static readonly Rune NoEntry         = (Rune)'⛔';
    }

    public static class Dice
    {
        public static readonly Rune One   = (Rune)'⚀';
        public static readonly Rune Two   = (Rune)'⚁';
        public static readonly Rune Three = (Rune)'⚂';
        public static readonly Rune Four  = (Rune)'⚃';
        public static readonly Rune Five  = (Rune)'⚄';
        public static readonly Rune Six   = (Rune)'⚅';
    }

    public static readonly Dictionary<ExChar, char> UnicodeChar =
        new Dictionary<ExChar, char>
        {
            [ExChar.BoxDoubleUpLeft]         = '╔',
            [ExChar.BoxDoubleUpRight]        = '╗',
            [ExChar.BoxDoubleDownLeft]       = '╚',
            [ExChar.BoxDoubleDownRight]      = '╝',
            [ExChar.BoxDoubleHorz]           = '═',
            [ExChar.BoxDoubleVert]           = '║',
            [ExChar.BoxDoubleCross]          = '╬',
            [ExChar.BoxDoubleLeftTee]        = '╠',
            [ExChar.BoxDoubleRightTee]       = '╣',
            [ExChar.BoxDoubleTopTee]         = '╦',
            [ExChar.BoxDoubleBottomTee]      = '╩',
            [ExChar.LineDoubleLeft]          = '╸',
            [ExChar.LineDoubleTop]           = '╹',
            [ExChar.LineDoubleRight]         = '╺',
            [ExChar.LineDoubleDown]          = '╻',
            [ExChar.BoxSingleUpLeft]         = '┌',
            [ExChar.BoxSingleUpRight]        = '┐',
            [ExChar.BoxSingleDownLeft]       = '└',
            [ExChar.BoxSingleDownRight]      = '┘',
            [ExChar.BoxSingleHorz]           = '─',
            [ExChar.BoxSingleVert]           = '│',
            [ExChar.BoxSingleCross]          = '┼',
            [ExChar.BoxSingleLeftTee]        = '├',
            [ExChar.BoxSingleRightTee]       = '┤',
            [ExChar.BoxSingleTopTee]         = '┬',
            [ExChar.BoxSingleBottomTee]      = '┴',
            [ExChar.LineSingleLeft]          = '╴',
            [ExChar.LineSingleTop]           = '╵',
            [ExChar.LineSingleRight]         = '╶',
            [ExChar.LineSingleDown]          = '╷',
            [ExChar.BoxDoubleHorzUpLeft]     = '╒',
            [ExChar.BoxDoubleHorzUpRight]    = '╕',
            [ExChar.BoxDoubleHorzDownLeft]   = '╘',
            [ExChar.BoxDoubleHorzDownRight]  = '╛',
            [ExChar.BoxDoubleHorzHorz]       = '═',
            [ExChar.BoxDoubleHorzVert]       = '│',
            [ExChar.BoxDoubleHorzCross]      = '╪',
            [ExChar.BoxDoubleHorzLeftTee]    = '╞',
            [ExChar.BoxDoubleHorzRightTee]   = '╡',
            [ExChar.BoxDoubleHorzTopTee]     = '╤',
            [ExChar.BoxDoubleHorzBottomTee]  = '╧',
            [ExChar.BoxSingleHorzUpLeft]     = '╓',
            [ExChar.BoxSingleHorzUpRight]    = '╖',
            [ExChar.BoxSingleHorzDownLeft]   = '╙',
            [ExChar.BoxSingleHorzDownRight]  = '╜',
            [ExChar.BoxSingleHorzHorz]       = '─',
            [ExChar.BoxSingleHorzVert]       = '║',
            [ExChar.BoxSingleHorzCross]      = '╫',
            [ExChar.BoxSingleHorzLeftTee]    = '╟',
            [ExChar.BoxSingleHorzRightTee]   = '╢',
            [ExChar.BoxSingleHorzTopTee]     = '╥',
            [ExChar.BoxSingleHorzBottomTee]  = '╨',
            [ExChar.AcuteAccent]             = '´',
            [ExChar.BlockDown]               = '▄',
            [ExChar.BlockFull]               = '█',
            [ExChar.BlockHigh]               = '▓',
            [ExChar.BlockLow]                = '░',
            [ExChar.BlockMed]                = '▒',
            [ExChar.BlockUp]                 = '▀',
            [ExChar.BlockLeft]               = '▌',
            [ExChar.BlockRight]              = '▐',
            [ExChar.Square]                  = '■',
            [ExChar.Cedilla]                 = '¸',
            [ExChar.Cent]                    = '¢',
            [ExChar.Congruence]              = '≡',
            [ExChar.Copyright]               = '©',
            [ExChar.Degree]                  = '°',
            [ExChar.Diaresis]                = '¨',
            [ExChar.DiphthongLower]          = 'æ',
            [ExChar.DiphthongUpper]          = 'Æ',
            [ExChar.Division]                = '÷',
            [ExChar.ExclamationMarkInverted] = '¡',
            [ExChar.Function]                = 'ƒ',
            [ExChar.GenericCurrency]         = '¤',
            [ExChar.LogicalNotation]         = '¬',
            [ExChar.Macron]                  = '¯',
            [ExChar.Multiplication]          = '×',
            [ExChar.OneHalf]                 = '½',
            [ExChar.OneQuarter]              = '¼',
            [ExChar.Paragraph]               = '¶',
            [ExChar.PlusMinus]               = '±',
            [ExChar.Pound]                   = '£',
            [ExChar.QuestionMarkInverted]    = '¿',
            [ExChar.QuoteMarkLeft]           = '«',
            [ExChar.QuoteMarkRight]          = '»',
            [ExChar.RegisteredTrademark]     = '®',
            [ExChar.Selection]               = '§',
            [ExChar.SlashZeroLower]          = 'ø',
            [ExChar.SlashZeroUpper]          = 'Ø',
            [ExChar.SpaceDot]                = '·',
            [ExChar.SubscriptOne]            = '¹',
            [ExChar.SubscriptThree]          = '³',
            [ExChar.SubScriptTwo]            = '²',
            [ExChar.ThreeQuarters]           = '¾',
            [ExChar.Underline]               = '‗',
            [ExChar.VertBroken]              = '¦',
            [ExChar.Yen]                     = '¥',
        };

    public static readonly Dictionary<ExChar, char> AsciiChar =
        new Dictionary<ExChar, char>
        {
            [ExChar.BoxDoubleUpLeft]         = (char)201,
            [ExChar.BoxDoubleUpRight]        = (char)187,
            [ExChar.BoxDoubleDownLeft]       = (char)200,
            [ExChar.BoxDoubleDownRight]      = (char)188,
            [ExChar.BoxDoubleHorz]           = (char)205,
            [ExChar.BoxDoubleVert]           = (char)186,
            [ExChar.BoxDoubleCross]          = (char)206,
            [ExChar.BoxDoubleLeftTee]        = (char)204,
            [ExChar.BoxDoubleRightTee]       = (char)185,
            [ExChar.BoxDoubleTopTee]         = (char)203,
            [ExChar.BoxDoubleBottomTee]      = (char)202,
            [ExChar.BoxSingleUpLeft]         = (char)218,
            [ExChar.BoxSingleUpRight]        = (char)191,
            [ExChar.BoxSingleDownLeft]       = (char)192,
            [ExChar.BoxSingleDownRight]      = (char)217,
            [ExChar.BoxSingleHorz]           = (char)196,
            [ExChar.BoxSingleVert]           = (char)179,
            [ExChar.BoxSingleCross]          = (char)197,
            [ExChar.BoxSingleLeftTee]        = (char)195,
            [ExChar.BoxSingleRightTee]       = (char)180,
            [ExChar.BoxSingleTopTee]         = (char)194,
            [ExChar.BoxSingleBottomTee]      = (char)193,
            [ExChar.BoxDoubleHorzUpLeft]     = (char)213,
            [ExChar.BoxDoubleHorzUpRight]    = (char)184,
            [ExChar.BoxDoubleHorzDownLeft]   = (char)212,
            [ExChar.BoxDoubleHorzDownRight]  = (char)190,
            [ExChar.BoxDoubleHorzHorz]       = (char)196,
            [ExChar.BoxDoubleHorzVert]       = (char)179,
            [ExChar.BoxDoubleHorzCross]      = (char)216,
            [ExChar.BoxDoubleHorzLeftTee]    = (char)198,
            [ExChar.BoxDoubleHorzRightTee]   = (char)181,
            [ExChar.BoxDoubleHorzTopTee]     = (char)209,
            [ExChar.BoxDoubleHorzBottomTee]  = (char)207,
            [ExChar.BoxSingleHorzUpLeft]     = (char)214,
            [ExChar.BoxSingleHorzUpRight]    = (char)183,
            [ExChar.BoxSingleHorzDownLeft]   = (char)211,
            [ExChar.BoxSingleHorzDownRight]  = (char)189,
            [ExChar.BoxSingleHorzHorz]       = (char)196,
            [ExChar.BoxSingleHorzVert]       = (char)179,
            [ExChar.BoxSingleHorzCross]      = (char)215,
            [ExChar.BoxSingleHorzLeftTee]    = (char)199,
            [ExChar.BoxSingleHorzRightTee]   = (char)182,
            [ExChar.BoxSingleHorzTopTee]     = (char)210,
            [ExChar.BoxSingleHorzBottomTee]  = (char)208,
            [ExChar.AcuteAccent]             = (char)239,
            [ExChar.BlockDown]               = (char)220,
            [ExChar.BlockFull]               = (char)219,
            [ExChar.BlockHigh]               = (char)178,
            [ExChar.BlockLow]                = (char)177,
            [ExChar.BlockMed]                = (char)176,
            [ExChar.BlockUp]                 = (char)223,
            [ExChar.BlockLeft]               = (char)221,
            [ExChar.BlockRight]              = (char)222,
            [ExChar.Square]                  = (char)254,
            [ExChar.Cedilla]                 = (char)247,
            [ExChar.Cent]                    = (char)155,
            [ExChar.Congruence]              = (char)240,
            [ExChar.Copyright]               = (char)184,
            [ExChar.Degree]                  = (char)167,
            [ExChar.Diaresis]                = (char)249,
            [ExChar.DiphthongLower]          = (char)145,
            [ExChar.DiphthongUpper]          = (char)146,
            [ExChar.Division]                = (char)246,
            [ExChar.ExclamationMarkInverted] = (char)173,
            [ExChar.Function]                = (char)159,
            [ExChar.GenericCurrency]         = (char)207,
            [ExChar.LogicalNotation]         = (char)170,
            [ExChar.Macron]                  = (char)238,
            [ExChar.Multiplication]          = (char)158,
            [ExChar.OneHalf]                 = (char)171,
            [ExChar.OneQuarter]              = (char)172,
            [ExChar.Paragraph]               = (char)244,
            [ExChar.PlusMinus]               = (char)241,
            [ExChar.Pound]                   = (char)156,
            [ExChar.QuestionMarkInverted]    = (char)168,
            [ExChar.QuoteMarkLeft]           = (char)174,
            [ExChar.QuoteMarkRight]          = (char)175,
            [ExChar.RegisteredTrademark]     = (char)169,
            [ExChar.Selection]               = (char)245,
            [ExChar.SlashZeroLower]          = (char)155,
            [ExChar.SlashZeroUpper]          = (char)157,
            [ExChar.SpaceDot]                = (char)250,
            [ExChar.SubscriptOne]            = (char)251,
            [ExChar.SubscriptThree]          = (char)252,
            [ExChar.SubScriptTwo]            = (char)253,
            [ExChar.ThreeQuarters]           = (char)243,
            [ExChar.Underline]               = (char)242,
            [ExChar.VertBroken]              = (char)221,
            [ExChar.Yen]                     = (char)157,
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

    internal static readonly LineDrawSet DoubleLine = new(" ╥╺╔╨║╚╠╸╗═╦╝╣╩╬");
    internal static readonly LineDrawSet SingleLine = new(" ╷╶┌╵│└├╴┐─┬┘┤┴┼");
    internal static readonly LineDrawSet RoundLine  = new(" ╷╶╭╵│╰├╴╮─┬╯┤┴┼");
    internal static readonly LineDrawSet DoubleHorz = new(" ╻╺╒╵│╘╞╸╕═╤╛╡╧╪");
    internal static readonly LineDrawSet SingleHorz = new(" ╷╶╓╹║╙╟╴╖─╥╜╢╨╫");
    internal static readonly LineDrawSet NoLine     = new("                ");

    //**************************************************
}