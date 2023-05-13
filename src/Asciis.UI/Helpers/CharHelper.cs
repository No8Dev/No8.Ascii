using System;

namespace Asciis.UI;

public class CharHelper
{
    public static ConsoleKey[] MapKeyToConsoleKey =
    {
            // 0..7
            0,                    // Key.None = 0,
            0,                    // Key.Cancel = 1,
            ConsoleKey.Backspace, // Key.Back = 2,
            ConsoleKey.Tab,       // Key.Tab = 3,
            0,                    // Key.LineFeed = 4,
            ConsoleKey.Clear,     // Key.Clear = 5,
            ConsoleKey.Enter,     // Key.Enter = 6, Key.Return = 6,
            ConsoleKey.Pause,     // Key.Pause = 7,

            // 8..15
            0,                 // Key.Capital = 8, Key.CapsLock = 8,
            0,                 // Key.HangulMode = 9, Key.KanaMode = 9,
            0,                 // Key.JunjaMode = 10,
            0,                 // Key.FinalMode = 11,
            0,                 // Key.HanjaMode = 12, Key.KanjiMode = 12,
            ConsoleKey.Escape, // Key.Escape = 13,
            0,                 // Key.ImeConvert = 14,
            0,                 // Key.ImeNonConvert = 15,

            // 16..23
            0,                    // Key.ImeAccept = 16,
            0,                    // Key.ImeModeChange = 17,
            ConsoleKey.Spacebar,  // Key.Space = 18,
            ConsoleKey.PageUp,    // Key.PageUp = 19, Key.Prior = 19,
            ConsoleKey.PageDown,  // Key.PageDown = 20, Key.Next = 20,
            ConsoleKey.End,       // Key.End = 21,
            ConsoleKey.Home,      // Key.Home = 22,
            ConsoleKey.LeftArrow, // Key.Left = 23,

            // 24..31
            ConsoleKey.UpArrow,     // Key.Up = 24,
            ConsoleKey.RightArrow,  // Key.Right = 25,
            ConsoleKey.DownArrow,   // Key.Down = 26,
            ConsoleKey.Select,      // Key.Select = 27,
            ConsoleKey.Print,       // Key.Print = 28,
            ConsoleKey.Execute,     // Key.Execute = 29,
            ConsoleKey.PrintScreen, // Key.PrintScreen = 30, Key.Snapshot = 30,
            ConsoleKey.Insert,      // Key.Insert = 31,

            // 32..39
            ConsoleKey.Delete, // Key.Delete = 32,
            ConsoleKey.Help,   // Key.Help = 33,
            ConsoleKey.D0,     // Key.D0 = 34,
            ConsoleKey.D1,     // Key.D1 = 35,
            ConsoleKey.D2,     // Key.D2 = 36,
            ConsoleKey.D3,     // Key.D3 = 37,
            ConsoleKey.D4,     // Key.D4 = 38,
            ConsoleKey.D5,     // Key.D5 = 39,

            // 40..47
            ConsoleKey.D6, // Key.D6 = 40,
            ConsoleKey.D7, // Key.D7 = 41,
            ConsoleKey.D8, // Key.D8 = 42,
            ConsoleKey.D9, // Key.D9 = 43,
            ConsoleKey.A,  // Key.A = 44,
            ConsoleKey.B,  // Key.B = 45,
            ConsoleKey.C,  // Key.C = 46,
            ConsoleKey.D,  // Key.D = 47,

            // 48..55
            ConsoleKey.E, // Key.E = 48,
            ConsoleKey.F, // Key.F = 49,
            ConsoleKey.G, // Key.G = 50,
            ConsoleKey.H, // Key.H = 51,
            ConsoleKey.I, // Key.I = 52,
            ConsoleKey.J, // Key.J = 53,
            ConsoleKey.K, // Key.K = 54,
            ConsoleKey.L, // Key.L = 55,

            // 56..63
            ConsoleKey.M, // Key.M = 56,
            ConsoleKey.N, // Key.N = 57,
            ConsoleKey.O, // Key.O = 58,
            ConsoleKey.P, // Key.P = 59,
            ConsoleKey.Q, // Key.Q = 60,
            ConsoleKey.R, // Key.R = 61,
            ConsoleKey.S, // Key.S = 62,
            ConsoleKey.T, // Key.T = 63,

            // 64..71
            ConsoleKey.U,            // Key.U = 64,
            ConsoleKey.V,            // Key.V = 65,
            ConsoleKey.W,            // Key.W = 66,
            ConsoleKey.X,            // Key.X = 67,
            ConsoleKey.Y,            // Key.Y = 68,
            ConsoleKey.Z,            // Key.Z = 69,
            ConsoleKey.LeftWindows,  // Key.LWin = 70,
            ConsoleKey.RightWindows, // Key.RWin = 71,

            // 72..79
            ConsoleKey.Applications, // Key.Apps = 72,
            ConsoleKey.Sleep,        // Key.Sleep = 73,
            ConsoleKey.NumPad0,      // Key.NumPad0 = 74,
            ConsoleKey.NumPad1,      // Key.NumPad1 = 75,
            ConsoleKey.NumPad2,      // Key.NumPad2 = 76,
            ConsoleKey.NumPad3,      // Key.NumPad3 = 77,
            ConsoleKey.NumPad4,      // Key.NumPad4 = 78,
            ConsoleKey.NumPad5,      // Key.NumPad5 = 79,

            // 80..87
            ConsoleKey.NumPad6,   // Key.NumPad6 = 80,
            ConsoleKey.NumPad7,   // Key.NumPad7 = 81,
            ConsoleKey.NumPad8,   // Key.NumPad8 = 82,
            ConsoleKey.NumPad9,   // Key.NumPad9 = 83,
            ConsoleKey.Multiply,  // Key.Multiply = 84,
            ConsoleKey.Add,       // Key.Add = 85,
            ConsoleKey.Separator, // Key.Separator = 86,
            ConsoleKey.Subtract,  // Key.Subtract = 87,

            // 88..95
            ConsoleKey.Decimal, // Key.Decimal = 88,
            ConsoleKey.Divide,  // Key.Divide = 89,
            ConsoleKey.F1,      // Key.F1 = 90,
            ConsoleKey.F2,      // Key.F2 = 91,
            ConsoleKey.F3,      // Key.F3 = 92,
            ConsoleKey.F4,      // Key.F4 = 93,
            ConsoleKey.F5,      // Key.F5 = 94,
            ConsoleKey.F6,      // Key.F6 = 95,

            // 96..103
            ConsoleKey.F7,  // Key.F7 = 96,
            ConsoleKey.F8,  // Key.F8 = 97,
            ConsoleKey.F9,  // Key.F9 = 98,
            ConsoleKey.F10, // Key.F10 = 99,
            ConsoleKey.F11, // Key.F11 = 100,
            ConsoleKey.F12, // Key.F12 = 101,
            ConsoleKey.F13, // Key.F13 = 102,
            ConsoleKey.F14, // Key.F14 = 103,

            // 104..111
            ConsoleKey.F15, // Key.F15 = 104,
            ConsoleKey.F16, // Key.F16 = 105,
            ConsoleKey.F17, // Key.F17 = 106,
            ConsoleKey.F18, // Key.F18 = 107,
            ConsoleKey.F19, // Key.F19 = 108,
            ConsoleKey.F20, // Key.F20 = 109,
            ConsoleKey.F21, // Key.F21 = 110,
            ConsoleKey.F22, // Key.F22 = 111,

            // 112..119
            ConsoleKey.F23, // Key.F23 = 112,
            ConsoleKey.F24, // Key.F24 = 113,
            0,              // Key.NumLock = 114,
            0,              // Key.Scroll = 115,
            0,              // Key.LeftShift = 116,
            0,              // Key.RightShift = 117,
            0,              // Key.LeftCtrl = 118,
            0,              // Key.RightCtrl = 119,

            // 120..127
            0,                           // Key.LeftAlt = 120,
            0,                           // Key.RightAlt = 121,
            ConsoleKey.BrowserBack,      // Key.BrowserBack = 122,
            ConsoleKey.BrowserForward,   // Key.BrowserForward = 123,
            ConsoleKey.BrowserRefresh,   // Key.BrowserRefresh = 124,
            ConsoleKey.BrowserStop,      // Key.BrowserStop = 125,
            ConsoleKey.BrowserSearch,    // Key.BrowserSearch = 126,
            ConsoleKey.BrowserFavorites, // Key.BrowserFavorites = 127,

            // 128..135
            ConsoleKey.BrowserHome,   // Key.BrowserHome = 128,
            ConsoleKey.VolumeMute,    // Key.VolumeMute = 129,
            ConsoleKey.VolumeDown,    // Key.VolumeDown = 130,
            ConsoleKey.VolumeUp,      // Key.VolumeUp = 131,
            ConsoleKey.MediaNext,     // Key.MediaNextTrack = 132,
            ConsoleKey.MediaPrevious, // Key.MediaPreviousTrack = 133,
            ConsoleKey.MediaStop,     // Key.MediaStop = 134,
            ConsoleKey.MediaPlay,     // Key.MediaPlayPause = 135,

            // 136..143
            ConsoleKey.LaunchMail,        // Key.LaunchMail = 136,
            ConsoleKey.LaunchMediaSelect, // Key.SelectMedia = 137,
            ConsoleKey.LaunchApp1,        // Key.LaunchApplication1 = 138,
            ConsoleKey.LaunchApp2,        // Key.LaunchApplication2 = 139,
            ConsoleKey.Oem1,              // Key.Oem1 = 140, Key.OemSemicolon = 140,
            ConsoleKey.OemPlus,           // Key.OemPlus = 141,
            ConsoleKey.OemComma,          // Key.OemComma = 142,
            ConsoleKey.OemMinus,          // Key.OemMinus = 143,

            // 144..151
            0,               // Key.OemPeriod = 144,
            ConsoleKey.Oem2, // Key.Oem2 = 145, Key.OemQuestion = 145,
            ConsoleKey.Oem3, // Key.Oem3 = 146, Key.OemTilde = 146,
            0,               // Key.AbntC1 = 147,
            0,               // Key.AbntC2 = 148,
            ConsoleKey.Oem4, // Key.Oem4 = 149, Key.OemOpenBrackets = 149,
            ConsoleKey.Oem5, // Key.Oem5 = 150, Key.OemPipe = 150,
            ConsoleKey.Oem6, // Key.Oem6 = 151, Key.OemCloseBrackets = 151,

            // 152..159
            ConsoleKey.Oem7,      // Key.Oem7 = 152, Key.OemQuotes = 152,
            ConsoleKey.Oem8,      // Key.Oem8 = 153,
            ConsoleKey.Oem102,    // Key.Oem102 = 154, Key.OemBackslash = 154,
            0,                    // Key.ImeProcessed = 155,
            0,                    // Key.System = 156,
            ConsoleKey.Attention, // Key.OemAttn = 157, Key.DbeAlphanumeric = 157,
            0,                    // Key.DbeKatakana = 158, Key.OemFinish = 158,
            0,                    // Key.DbeHiragana = 159, Key.OemCopy = 159,

            // 160..167
            0,                         // Key.DbeSbcsChar = 160, Key.OemAuto = 160,
            0,                         // Key.DbeDbcsChar = 161, Key.OemEnlw = 161,
            0,                         // Key.DbeRoman = 162, Key.OemBackTab = 162,
            0,                         // Key.Attn = 163, Key.DbeNoRoman = 163,
            ConsoleKey.CrSel,          // Key.CrSel = 164, Key.DbeEnterWordRegisterMode = 164,
            ConsoleKey.ExSel,          // Key.ExSel = 165, Key.DbeEnterImeConfigureMode = 165,
            ConsoleKey.EraseEndOfFile, // Key.EraseEof = 166, Key.DbeFlushString = 166,
            ConsoleKey.Play,           // Key.Play = 167, Key.DbeCodeInput = 167,

            // 168..175
            ConsoleKey.Zoom,     // Key.Zoom = 168, Key.DbeNoCodeInput = 168,
            ConsoleKey.NoName,   // Key.NoName = 169, Key.DbeDetermineString = 169,
            ConsoleKey.Pa1,      // Key.Pa1 = 170, Key.DbeEnterDialogConversionMode = 170,
            ConsoleKey.OemClear, // Key.OemClear = 171,
            0,                   // Key.DeadCharProcessed = 172,
            0,
            0,
            0,

            // 176..183
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,

            // 184..191
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,

            // 192..199
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,

            // 200..207
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,

            // 208..215
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,

            // 216..223
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,

            // 224..231
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,

            // 232..239
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,

            // 240..247
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,

            // 248..255
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        };

    public static char[] MapCharToUnicode =
    {
            // 1 - 15
            (char)0,
            CharCode.WhiteSmilingFace, //: 1,☺
            CharCode.BlackSmilingFace, //: 2,
            CharCode.BlackHeartSuit,   //: 3,
            CharCode.BlackDiamondSuit, //: 4,
            CharCode.BlackClubSuit,    //: 5,
            CharCode.BlackSpadeSuit,   //: 6,
            CharCode.Bullet,           //: 7,

            CharCode.InverseBullet,      //: 8,
            CharCode.WhiteCircle,        //: 9,
            CharCode.InverseWhiteCircle, //: 10,
            CharCode.MaleSign,           //: 11,
            CharCode.FemaleSign,         //: 12,
            CharCode.EighthNote,         //: 13,
            CharCode.BeamedEighthNotes,  //: 14,
            CharCode.WhiteSunWithRays,   //: 15

            // 16 - 31
            CharCode.BlackRightPointingPointer, //:16,
            CharCode.BlackLeftPointingPointer,  //:17,
            CharCode.UpDownArrow,               //:18,
            CharCode.DoubleExclamationMark,     //:19,
            CharCode.Pilcrow,                   //:20,
            CharCode.SectionSign,               //:21,
            CharCode.BlackRectangle,            //:22,
            CharCode.UpDownArrowWithBase,       //:23,

            CharCode.UpwardsArrow,              //:24,
            CharCode.DownwardsArrow,            //:25,
            CharCode.RightwardsArrow,           //:26,
            CharCode.LeftwardsArrow,            //:27,
            CharCode.RightAngle,                //:28,
            CharCode.LeftRightArrow,            //:29,
            CharCode.BlackUpPointingTriangle,   //:30,
            CharCode.BlackDownPointingTriangle, //:31,

            // 32..
            ' ',
            '!',
            '"',
            '#',
            '$',
            '%',
            '&',
            '\'',
            '(',
            ')',
            '*',
            '+',
            ',',
            '-',
            '.',
            '/',

            // 48..
            '0',
            '1',
            '2',
            '3',
            '4',
            '5',
            '6',
            '7',
            '8',
            '9',
            ':',
            ';',
            '<',
            '=',
            '>',
            '?',

            // 64..
            '@',
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',

            // 80..
            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z',
            '[',
            '\\',
            ']',
            '^',
            '_',

            // 96..
            '`',
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'g',
            'h',
            'i',
            'j',
            'k',
            'l',
            'm',
            'n',
            'o',

            // 112..127
            'p',
            'q',
            'r',
            's',
            't',
            'u',
            'v',
            'w',
            'x',
            'y',
            'z',
            '{',
            '|',
            '}',
            '~',
            CharCode.House, //:127,

            // 128..143
            CharCode.LatinCapitalLetterCWithCedilla,  //:128,Ç
            CharCode.LatinSmallLetterUWithDiaeresis,  //:129,ü
            CharCode.LatinSmallLetterEWithAcute,      //:130,é
            CharCode.LatinSmallLetterAWithCircumflex, //:131,â
            CharCode.LatinSmallLetterAWithDiaeresis,  //:132,ä
            CharCode.LatinSmallLetterAWithGrave,      //:133,à
            CharCode.LatinSmallLetterAWithRingAbove,  //:134,å
            CharCode.LatinSmallLetterCWithCedilla,    //:135,ç

            CharCode.LatinSmallLetterEWithCircumflex,  //:136,ê
            CharCode.LatinSmallLetterEWithDiaeresis,   //:137,ë
            CharCode.LatinSmallLetterEWithGrave,       //:138,è
            CharCode.LatinSmallLetterIWithDiaeresis,   //:139,ï
            CharCode.LatinSmallLetterIWithCircumflex,  //:140,î
            CharCode.LatinSmallLetterIWithGrave,       //:141,ì
            CharCode.LatinCapitalLetterAWithDiaeresis, //:142,Ä
            CharCode.LatinCapitalLetterAWithRingAbove, //:143,Å

            // 144..159
            CharCode.LatinCapitalLetterEWithAcute,    //:144,É
            CharCode.LatinSmallLetterAe,              //:145,æ
            CharCode.LatinCapitalLetterAe,            //:146,Æ
            CharCode.LatinSmallLetterOWithCircumflex, //:147,ô
            CharCode.LatinSmallLetterOWithDiaeresis,  //:148,ö
            CharCode.LatinSmallLetterOWithGrave,      //:149,ò
            CharCode.LatinSmallLetterUWithCircumflex, //:150,û
            CharCode.LatinSmallLetterUWithGrave,      //:151,ù

            CharCode.LatinSmallLetterYWithDiaeresis,   //:152,ÿ
            CharCode.LatinCapitalLetterOWithDiaeresis, //:153,Ö
            CharCode.LatinCapitalLetterUWithDiaeresis, //:154,Ü
            CharCode.CentSign,                         //:155,¢
            CharCode.PoundSign,                        //:156,£
            CharCode.YenSign,                          //:157,¥
            CharCode.PesetaSign,                       //:158,₧
            CharCode.LatinSmallLetterFWithHook,        //:159,ƒ

            // 160..175
            CharCode.LatinSmallLetterAWithAcute,   //:160,á
            CharCode.LatinSmallLetterIWithAcute,   //:161,í
            CharCode.LatinSmallLetterOWithAcute,   //:162,ó
            CharCode.LatinSmallLetterUWithAcute,   //:163,ú
            CharCode.LatinSmallLetterNWithTilde,   //:164,ñ
            CharCode.LatinCapitalLetterNWithTilde, //:165,Ñ
            CharCode.FeminineOrdinalIndicator,     //:166,ª
            CharCode.MasculineOrdinalIndicator,    //:167,º

            CharCode.InvertedQuestionMark,                  //:168,¿
            CharCode.ReversedNotSign,                       //:169,⌐
            CharCode.NotSign,                               //:170,¬
            CharCode.VulgarFractionOneHalf,                 //:171,½
            CharCode.VulgarFractionOneQuarter,              //:172,¼
            CharCode.InvertedExclamationMark,               //:173,¡
            CharCode.LeftPointingDoubleAngleQuotationMark,  //:174,«
            CharCode.RightPointingDoubleAngleQuotationMark, //:175,»

            // 176..191
            CharCode.LightShade,                             //:176,░
            CharCode.MediumShade,                            //:177,▒
            CharCode.DarkShade,                              //:178,▓
            CharCode.BoxDrawingsLightVertical,               //:179,│
            CharCode.BoxDrawingsLightVerticalAndLeft,        //:180,┤
            CharCode.BoxDrawingsVerticalSingleAndLeftDouble, //:181,╡
            CharCode.BoxDrawingsVerticalDoubleAndLeftSingle, //:182,╢
            CharCode.BoxDrawingsDownDoubleAndLeftSingle,     //:183,╖

            CharCode.BoxDrawingsDownSingleAndLeftDouble, //:184,╕
            CharCode.BoxDrawingsDoubleVerticalAndLeft,   //:185,╣
            CharCode.BoxDrawingsDoubleVertical,          //:186,║
            CharCode.BoxDrawingsDoubleDownAndLeft,       //:187,╗
            CharCode.BoxDrawingsDoubleUpAndLeft,         //:188,╝
            CharCode.BoxDrawingsUpDoubleAndLeftSingle,   //:189,╜
            CharCode.BoxDrawingsUpSingleAndLeftDouble,   //:190,╛
            CharCode.BoxDrawingsLightDownAndLeft,        //:191,┐

            // 192..207
            CharCode.BoxDrawingsLightUpAndRight,              //:192,└
            CharCode.BoxDrawingsLightUpAndHorizontal,         //:193,┴
            CharCode.BoxDrawingsLightDownAndHorizontal,       //:194,┬
            CharCode.BoxDrawingsLightVerticalAndRight,        //:195,├
            CharCode.BoxDrawingsLightHorizontal,              //:196,─
            CharCode.BoxDrawingsLightVerticalAndHorizontal,   //:197,┼
            CharCode.BoxDrawingsVerticalSingleAndRightDouble, //:198,╞
            CharCode.BoxDrawingsVerticalDoubleAndRightSingle, //:199,╟

            CharCode.BoxDrawingsDoubleUpAndRight,            //:200,╚
            CharCode.BoxDrawingsDoubleDownAndRight,          //:201,╔
            CharCode.BoxDrawingsDoubleUpAndHorizontal,       //:202,╩
            CharCode.BoxDrawingsDoubleDownAndHorizontal,     //:203,╦
            CharCode.BoxDrawingsDoubleVerticalAndRight,      //:204,╠
            CharCode.BoxDrawingsDoubleHorizontal,            //:205,═
            CharCode.BoxDrawingsDoubleVerticalAndHorizontal, //:206,╬
            CharCode.BoxDrawingsUpSingleAndHorizontalDouble, //:207,╧

            // 208..223
            CharCode.BoxDrawingsUpDoubleAndHorizontalSingle,       //:208,╨
            CharCode.BoxDrawingsDownSingleAndHorizontalDouble,     //:209,╤
            CharCode.BoxDrawingsDownDoubleAndHorizontalSingle,     //:210,╥
            CharCode.BoxDrawingsUpDoubleAndRightSingle,            //:211,╙
            CharCode.BoxDrawingsUpSingleAndRightDouble,            //:212,╘
            CharCode.BoxDrawingsDownSingleAndRightDouble,          //:213,╒
            CharCode.BoxDrawingsDownDoubleAndRightSingle,          //:214,╓
            CharCode.BoxDrawingsVerticalDoubleAndHorizontalSingle, //:215,╫

            CharCode.BoxDrawingsVerticalSingleAndHorizontalDouble, //:216,╪
            CharCode.BoxDrawingsLightUpAndLeft,                    //:217,┘
            CharCode.BoxDrawingsLightDownAndRight,                 //:218,┌
            CharCode.FullBlock,                                    //:219,█
            CharCode.LowerHalfBlock,                               //:220,▄
            CharCode.LeftHalfBlock,                                //:221,▌
            CharCode.RightHalfBlock,                               //:222,▐
            CharCode.UpperHalfBlock,                               //:223,▀

            // 224..239
            CharCode.GreekSmallLetterAlpha,   //:224,α
            CharCode.LatinSmallLetterSharpS,  //:225,ß
            CharCode.GreekCapitalLetterGamma, //:226,Γ
            CharCode.GreekSmallLetterPi,      //:227,π
            CharCode.GreekCapitalLetterSigma, //:228,Σ
            CharCode.GreekSmallLetterSigma,   //:229,σ
            CharCode.MicroSign,               //:230,µ
            CharCode.GreekSmallLetterTau,     //:231,τ

            CharCode.GreekCapitalLetterPhi,   //:232,Φ
            CharCode.GreekCapitalLetterTheta, //:233,Θ
            CharCode.GreekCapitalLetterOmega, //:234,Ω
            CharCode.GreekSmallLetterDelta,   //:235,δ
            CharCode.Infinity,                //:236,∞
            CharCode.GreekSmallLetterPhi,     //:237,φ
            CharCode.GreekSmallLetterEpsilon, //:238,ε
            CharCode.Intersection,            //:239,∩

            // 240..255
            CharCode.IdenticalTo,          //:240,≡
            CharCode.PlusMinusSign,        //:241,±
            CharCode.GreaterThanOrEqualTo, //:242,≥
            CharCode.LessThanOrEqualTo,    //:243,≤
            CharCode.TopHalfIntegral,      //:244,⌠
            CharCode.BottomHalfIntegral,   //:245,⌡
            CharCode.DivisionSign,         //:246,÷
            CharCode.AlmostEqualTo,        //:247,≈

            CharCode.DegreeSign,                   //:248,°
            CharCode.BulletOperator,               //:249,∙
            CharCode.MiddleDot,                    //:250,·
            CharCode.SquareRoot,                   //:251,√
            CharCode.SuperscriptLatinSmallLetterN, //:252,ⁿ
            CharCode.SuperscriptTwo,               //:253,²
            CharCode.BlackSquare,                  //:254,■
            (char)0
        };

    public static ConsoleKey[] MapVirtualKeyToConsoleKey =
    {
            0,
            0,                            // VK_LBUTTON    = 0x01, // Left mouse button
            0,                            // VK_RBUTTON    = 0x02, // Right mouse button
            0,                            // VK_CANCEL     = 0x03, // Control-break processing
            0,                            // VK_MBUTTON    = 0x04, // Middle mouse button (three-button mouse)
            0,                            // VK_XBUTTON1   = 0x05, // X1 mouse button
            0,                            // VK_XBUTTON2   = 0x06, // X2 mouse button
            0,                            // VK_UNDEFINED1 = 0x07, // Undefined
            ConsoleKey.Backspace,         // VK_BACK       = 0x08, // BACKSPACE key
            ConsoleKey.Tab,               // VK_TAB        = 0x09, // TAB key
            0,                            // VK_UNDEFINED2 = 0x0A, //Reserved
            0,                            // VK_UNDEFINED3 = 0x0B, //Reserved
            ConsoleKey.Clear,             // VK_CLEAR      = 0x0C, // CLEAR key
            ConsoleKey.Enter,             // VK_RETURN     = 0x0D, // ENTER key
            0,                            // VK_UNDEFINED4 = 0x0E, // Undefined
            0,                            // VK_UNDEFINED5 = 0x0F, // uNDEFINED
            0,                            // VK_SHIFT      = 0x10, // SHIFT key
            0,                            // VK_CONTROL    = 0x11, // CTRL key
            0,                            // VK_MENU       = 0x12, // ALT key
            ConsoleKey.Pause,             // VK_PAUSE      = 0x13, // PAUSE key
            0,                            // VK_CAPITAL    = 0x14, //CAPS LOCK key
            0,                            // VK_KANA       = 0x15, //IME Kana mode
            0,                            // VK_HANGUEL    = 0x15, //IME Hanguel mode (maintained for compatibility; use VK_HANGUL)
            0,                            // VK_HANGUL     = 0x15, // IME Hangul mode
            0,                            // VK_IME_ON     = 0x16, // IME On
            0,                            // VK_JUNJA      = 0x17, // IME Junja mode
            0,                            // VK_FINAL      = 0x18, // IME final mode
            0,                            // VK_HANJA      = 0x19, // IME Hanja mode
            0,                            // VK_KANJI      = 0x19, // IME Kanji mode
            0,                            // VK_IME_OFF    = 0x1A, // IME Off
            ConsoleKey.Escape,            // VK_ESCAPE     = 0x1B, // ESC key
            0,                            // VK_CONVERT    = 0x1C, // IME convert
            0,                            // VK_NONCONVERT = 0x1D, // IME nonconvert
            0,                            // VK_ACCEPT     = 0x1E, // IME accept
            0,                            // VK_MODECHANGE = 0x1F, // IME mode change request
            ConsoleKey.Spacebar,          // VK_SPACE      = 0x20, // SPACEBAR
            ConsoleKey.PageUp,            // VK_PRIOR      = 0x21, // PAGE UP key
            ConsoleKey.PageDown,          // VK_NEXT       = 0x22, // PAGE DOWN key
            ConsoleKey.End,               // VK_END        = 0x23, // END key
            ConsoleKey.Home,              // VK_HOME       = 0x24, // HOME key
            ConsoleKey.LeftArrow,         // VK_LEFT       = 0x25, // LEFT ARROW key
            ConsoleKey.UpArrow,           // VK_UP         = 0x26, // UP ARROW key
            ConsoleKey.RightArrow,        // VK_RIGHT      = 0x27, // RIGHT ARROW key
            ConsoleKey.DownArrow,         // VK_DOWN       = 0x28, // DOWN ARROW key
            ConsoleKey.Select,            // VK_SELECT     = 0x29, // SELECT key
            ConsoleKey.Print,             // VK_PRINT      = 0x2A, // PRINT key
            ConsoleKey.Execute,           // VK_EXECUTE    = 0x2B, // EXECUTE key
            0,                            // VK_SNAPSHOT   = 0x2C, // PRINT SCREEN key
            ConsoleKey.Insert,            // VK_INSERT     = 0x2D, // INS key
            ConsoleKey.Delete,            // VK_DELETE     = 0x2E, // DEL key
            ConsoleKey.Help,              // VK_HELP       = 0x2F, // HELP key
            ConsoleKey.D0,                // vk_0          = 0x30, // 0 key
            ConsoleKey.D1,                // VK_1          = 0x31, // 1 KEY
            ConsoleKey.D2,                // VK_2          = 0x32, // 2 KEY
            ConsoleKey.D3,                // VK_3          = 0x33, // 3 KEY
            ConsoleKey.D4,                // VK_4          = 0x34, // 4 KEY
            ConsoleKey.D5,                // VK_5          = 0x35, // 5 KEY
            ConsoleKey.D6,                // VK_6          = 0x36, // 6 KEY
            ConsoleKey.D7,                // VK_7          = 0x37, // 7 KEY
            ConsoleKey.D8,                // VK_8          = 0x38, // 8 KEY
            ConsoleKey.D9,                // VK_9          = 0x39, // 9 KEY   - 0x3A-40 Undefined
            ConsoleKey.A,                 // VK_A          = 0x41, // A key
            ConsoleKey.B,                 // VK_B          = 0x42, // B key
            ConsoleKey.C,                 // VK_C          = 0x43, // C key
            ConsoleKey.D,                 // VK_D          = 0x44, // D key
            ConsoleKey.E,                 // VK_E          = 0x45, // E key
            ConsoleKey.F,                 // VK_F          = 0x46, // F key
            ConsoleKey.G,                 // VK_G          = 0x47, // G key
            ConsoleKey.H,                 // VK_H          = 0x48, // H key
            ConsoleKey.I,                 // VK_I          = 0x49, // I key
            ConsoleKey.J,                 // VK_J          = 0x4A, // J key
            ConsoleKey.K,                 // VK_K          = 0x4B, // K key
            ConsoleKey.L,                 // VK_L          = 0x4C, // L key
            ConsoleKey.M,                 // VK_M          = 0x4D, // M key
            ConsoleKey.N,                 // VK_N          = 0x4E, // N key
            ConsoleKey.O,                 // VK_O          = 0x4F, // O key
            ConsoleKey.P,                 // VK_P          = 0x50, // P key
            ConsoleKey.Q,                 // VK_Q          = 0x51, // Q key
            ConsoleKey.R,                 // VK_R          = 0x52, // R key
            ConsoleKey.S,                 // VK_S          = 0x53, // S key
            ConsoleKey.T,                 // VK_T          = 0x54, // T key
            ConsoleKey.U,                 // VK_U          = 0x55, // U key
            ConsoleKey.V,                 // VK_V          = 0x56, // V key
            ConsoleKey.W,                 // VK_W          = 0x57, // W key
            ConsoleKey.X,                 // VK_X          = 0x58, // X key
            ConsoleKey.Y,                 // VK_Y          = 0x59, // Y key
            ConsoleKey.Z,                 // VK_Z          = 0x5A, // Z key
            ConsoleKey.LeftWindows,       // VK_LWIN       = 0x5B, // Left Windows key (Natural keyboard)
            ConsoleKey.RightWindows,      // VK_RWIN       = 0x5C, // Right Windows key (Natural keyboard)
            ConsoleKey.Applications,      // VK_APPS       = 0x5D, // Applications key (Natural keyboard)
            0,                            // VK_UNDEFINED6 = 0x5E, // Reserved
            ConsoleKey.Sleep,             // VK_SLEEP      = 0x5F, // Computer Sleep key
            ConsoleKey.NumPad0,           // VK_NUMPAD0    = 0x60, // Numeric keypad 0 key
            ConsoleKey.NumPad1,           // VK_NUMPAD1    = 0x61, // Numeric keypad 1 key
            ConsoleKey.NumPad2,           // VK_NUMPAD2    = 0x62, // Numeric keypad 2 key
            ConsoleKey.NumPad3,           // VK_NUMPAD3    = 0x63, // Numeric keypad 3 key
            ConsoleKey.NumPad4,           // VK_NUMPAD4    = 0x64, // Numeric keypad 4 key
            ConsoleKey.NumPad5,           // VK_NUMPAD5    = 0x65, // Numeric keypad 5 key
            ConsoleKey.NumPad6,           // VK_NUMPAD6    = 0x66, // Numeric keypad 6 key
            ConsoleKey.NumPad7,           // VK_NUMPAD7    = 0x67, // Numeric keypad 7 key
            ConsoleKey.NumPad8,           // VK_NUMPAD8    = 0x68, // Numeric keypad 8 key
            ConsoleKey.NumPad9,           // VK_NUMPAD9    = 0x69, // Numeric keypad 9 key
            ConsoleKey.Multiply,          // VK_MULTIPLY   = 0x6A, // Multiply key
            ConsoleKey.Add,               // VK_ADD        = 0x6B, // Add key
            ConsoleKey.Separator,         // VK_SEPARATOR  = 0x6C, // Separator key
            ConsoleKey.Subtract,          // VK_SUBTRACT   = 0x6D, // Subtract key
            ConsoleKey.Decimal,           // VK_DECIMAL    = 0x6E, // Decimal key
            ConsoleKey.Divide,            // VK_DIVIDE     = 0x6F, // Divide key
            ConsoleKey.F1,                // VK_F1         = 0x70, // F1 key
            ConsoleKey.F2,                // VK_F2         = 0x71, // F2 key
            ConsoleKey.F3,                // VK_F3         = 0x72, // F3 key
            ConsoleKey.F4,                // VK_F4         = 0x73, // F4 key
            ConsoleKey.F5,                // VK_F5         = 0x74, // F5 key
            ConsoleKey.F6,                // VK_F6         = 0x75, // F6 key
            ConsoleKey.F7,                // VK_F7         = 0x76, // F7 key
            ConsoleKey.F8,                // VK_F8         = 0x77, // F8 key
            ConsoleKey.F9,                // VK_F9         = 0x78, // F9 key
            ConsoleKey.F10,               // VK_F10        = 0x79, // F10 key
            ConsoleKey.F11,               // VK_F11        = 0x7A, // F11 key
            ConsoleKey.F12,               // VK_F12        = 0x7B, // F12 key
            ConsoleKey.F13,               // VK_F13        = 0x7C, // F13 key
            ConsoleKey.F14,               // VK_F14        = 0x7D, // F14 key
            ConsoleKey.F15,               // VK_F15        = 0x7E, // F15 key
            ConsoleKey.F16,               // VK_F16        = 0x7F, // F16 key
            ConsoleKey.F17,               // VK_F17        = 0x80, // F17 key
            ConsoleKey.F18,               // VK_F18        = 0x81, // F18 key
            ConsoleKey.F19,               // VK_F19        = 0x82, // F19 key
            ConsoleKey.F20,               // VK_F20        = 0x83, // F20 key
            ConsoleKey.F21,               // VK_F21        = 0x84, // F21 key
            ConsoleKey.F22,               // VK_F22        = 0x85, // F22 key
            ConsoleKey.F23,               // VK_F23        = 0x86, // F23 key
            ConsoleKey.F24,               // VK_F24        = 0x87, // F24 key = 0x88-8F,           // Unassigned
            0,                            // VK_NUMLOCK    = 0x90, // NUM LOCK key
            0,                            // VK_SCROLL     = 0x91, // SCROLL LOCK key
            0,                            //               = 0x92, // OEM specific
            0,                            //               = 0x93, // OEM specific
            0,                            //               = 0x94, // OEM specific
            0,                            //               = 0x95, // OEM specific
            0,                            //               = 0x96, // OEM specific
            0,                            //               = 0x97, // Unassigned
            0,                            //               = 0x98, // Unassigned
            0,                            //               = 0x99, // Unassigned
            0,                            //               = 0x9A, // Unassigned
            0,                            //               = 0x9B, // Unassigned
            0,                            //               = 0x9C, // Unassigned
            0,                            //               = 0x9D, // Unassigned
            0,                            //               = 0x9E, // Unassigned
            0,                            //               = 0x9F, // Unassigned
            0,                            // VK_LSHIFT              = 0xA0, // Left SHIFT key
            0,                            // VK_RSHIFT              = 0xA1, // Right SHIFT key
            0,                            // VK_LCONTROL            = 0xA2, // Left CONTROL key
            0,                            // VK_RCONTROL            = 0xA3, // Right CONTROL key
            0,                            // VK_LMENU               = 0xA4, // Left MENU key
            0,                            // VK_RMENU               = 0xA5, // Right MENU key
            ConsoleKey.BrowserBack,       // VK_BROWSER_BACK        = 0xA6, // Browser Back key
            ConsoleKey.BrowserForward,    // VK_BROWSER_FORWARD     = 0xA7, // Browser Forward key
            ConsoleKey.BrowserRefresh,    // VK_BROWSER_REFRESH     = 0xA8, // Browser Refresh key
            ConsoleKey.BrowserStop,       // VK_BROWSER_STOP        = 0xA9, // Browser Stop key
            ConsoleKey.BrowserSearch,     // VK_BROWSER_SEARCH      = 0xAA, // Browser Search key
            ConsoleKey.BrowserFavorites,  // VK_BROWSER_FAVORITES   = 0xAB, // Browser Favorites key
            ConsoleKey.BrowserHome,       // VK_BROWSER_HOME        = 0xAC, // Browser Start and Home key
            ConsoleKey.VolumeMute,        // VK_VOLUME_MUTE         = 0xAD, // Volume Mute key
            ConsoleKey.VolumeDown,        // VK_VOLUME_DOWN         = 0xAE, // Volume Down key
            ConsoleKey.VolumeUp,          // VK_VOLUME_UP           = 0xAF, // Volume Up key
            ConsoleKey.MediaNext,         // VK_MEDIA_NEXT_TRACK    = 0xB0, // Next Track key
            ConsoleKey.MediaPrevious,     // VK_MEDIA_PREV_TRACK    = 0xB1, // Previous Track key
            ConsoleKey.MediaStop,         // VK_MEDIA_STOP          = 0xB2, // Stop Media key
            ConsoleKey.MediaPlay,         // VK_MEDIA_PLAY_PAUSE    = 0xB3, // Play/Pause Media key
            ConsoleKey.LaunchMail,        // VK_LAUNCH_MAIL         = 0xB4, // Start Mail key
            ConsoleKey.LaunchMediaSelect, // VK_LAUNCH_MEDIA_SELECT = 0xB5, // Select Media key
            ConsoleKey.LaunchApp1,        // VK_LAUNCH_APP1         = 0xB6, // Start Application 1 key
            ConsoleKey.LaunchApp2,        // VK_LAUNCH_APP2         = 0xB7, // Start Application 2 key
            0,                            // = 0xB8, // Reserved
            0,                            // = 0xB9, // Reserved
            ConsoleKey.Oem1,              // VK_OEM_1 = 0xBA, // Used for miscellaneous characters; it can vary by keyboard.
            ConsoleKey.OemPlus,           // VK_OEM_PLUS   = 0xBB, // For any country/region, the '+' key // For the US standard keyboard, the ';:' key
            ConsoleKey.OemComma,          // VK_OEM_COMMA  = 0xBC, // For any country/region, the ',' key
            ConsoleKey.OemMinus,          // VK_OEM_MINUS  = 0xBD, // For any country/region, the '-' key
            ConsoleKey.OemPeriod,         // VK_OEM_PERIOD = 0xBE, // For any country/region, the '.' key
            ConsoleKey.Oem2,              // VK_OEM_2      = 0xBF, // Used for miscellaneous characters; it can vary by keyboard.
            ConsoleKey.Oem3,              // VK_OEM_3 = 0xC0, // Used for miscellaneous characters; it can vary by keyboard. // For the US standard keyboard, the '/?' key
            0,                            // = 0xC1, // Reserved // For the US standard keyboard, the '`~' key
            0,                            // = 0xC2, // Reserved
            0,                            // = 0xC3, // Reserved
            0,                            // = 0xC4, // Reserved
            0,                            // = 0xC5, // Reserved
            0,                            // = 0xC6, // Reserved
            0,                            // = 0xC7, // Reserved
            0,                            // = 0xC8, // Reserved
            0,                            // = 0xC9, // Reserved
            0,                            // = 0xCA, // Reserved
            0,                            // = 0xCB, // Reserved
            0,                            // = 0xCC, // Reserved
            0,                            // = 0xCD, // Reserved
            0,                            // = 0xCE, // Reserved
            0,                            // = 0xCF, // Reserved
            0,                            // = 0xD0, // Reserved
            0,                            // = 0xD1, // Reserved
            0,                            // = 0xD2, // Reserved
            0,                            // = 0xD3, // Reserved
            0,                            // = 0xD4, // Reserved
            0,                            // = 0xD5, // Reserved
            0,                            // = 0xD6, // Reserved
            0,                            // = 0xD7, // Reserved
            0,                            // = 0xD8, // Unassigned
            0,                            // = 0xD9, // Unassigned
            0,                            // = 0xDA, // Unassigned
            ConsoleKey.Oem4,              // VK_OEM_4 = 0xDB, // Used for miscellaneous characters; it can vary by keyboard.
            ConsoleKey.Oem5,              // VK_OEM_5 = 0xDC, // Used for miscellaneous characters; it can vary by keyboard. // For the US standard keyboard, the '[{' key
            ConsoleKey.Oem6,              // VK_OEM_6 = 0xDD, // Used for miscellaneous characters; it can vary by keyboard. // For the US standard keyboard, the '\|' key
            ConsoleKey.Oem7,              // VK_OEM_7 = 0xDE, // Used for miscellaneous characters; it can vary by keyboard. // For the US standard keyboard, the ']}' key
            ConsoleKey.Oem8,              // VK_OEM_8 = 0xDF, // Used for miscellaneous characters; it can vary by keyboard. // For the US standard keyboard, the 'single-quote/double-quote' key
            0,                            // =0xE0, // Reserved
            0,                            // =0xE1, // OEM specific
            0,                            // VK_OEM_102 = 0xE2, // Either the angle bracket key or the backslash key on the RT 102-key keyboard
            0,                            // =0xE3, // OEM specific
            0,                            // =0xE4, // OEM specific
            0,                            // VK_PROCESSKEY = 0xE5, // IME PROCESS key
            0,                            // =0xE6, // OEM specific
            0,                            // VK_PACKET = 0xE7, // Used to pass Unicode characters as if they were keystrokes.The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods.For more
            0,                            // = 0xE8, // Unassigned     // information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
            0,                            // = 0xE9, // OEM specific
            0,                            // = 0xEA, // OEM specific
            0,                            // = 0xEB, // OEM specific
            0,                            // = 0xEC, // OEM specific
            0,                            // = 0xED, // OEM specific
            0,                            // = 0xEE, // OEM specific
            0,                            // = 0xEF, // OEM specific
            0,                            // = 0xF0, // OEM specific
            0,                            // = 0xF1, // OEM specific
            0,                            // = 0xF2, // OEM specific
            0,                            // = 0xF3, // OEM specific
            0,                            // = 0xF4, // OEM specific
            0,                            // = 0xF5, // OEM specific
            ConsoleKey.Attention,         // VK_ATTN      = 0xF6, // Attn key
            ConsoleKey.CrSel,             // VK_CRSEL     = 0xF7, // CrSel key
            ConsoleKey.ExSel,             // VK_EXSEL     = 0xF8, // ExSel key
            ConsoleKey.EraseEndOfFile,    // VK_EREOF     = 0xF9, // Erase EOF key
            ConsoleKey.Play,              // VK_PLAY      = 0xFA, // Play key
            ConsoleKey.Zoom,              // VK_ZOOM      = 0xFB, // Zoom key
            ConsoleKey.NoName,            // VK_NONAME    = 0xFC, // Reserved
            ConsoleKey.Pa1,               // VK_PA1       = 0xFD, // PA1 key
            ConsoleKey.OemClear,          // VK_OEM_CLEAR = 0xFE, // Clear k
            0                             // 0xFF
        };
}
