namespace Asciis.UI;

/*
// https://en.wikipedia.org/wiki/ANSI_escape_code#Colors

// https://en.wikipedia.org/wiki/ANSI_escape_code#3-bit_and_4-bit

ESC[ ⟨n⟩ m      Select foreground color
ESC[ ⟨n⟩ m      Select background color

Name           FG    BG    VGA          WinConsole   PowerShell   VSCode      Win10 Console Terminal.app PuTTY        mIRC         xterm        Ubuntu
Black          30    40    0,0,0        0,0,0        0,0,0        0,0,0        12,12,12     0,0,0        0,0,0        0,0,0        0,0,0        1,1,1
Red            31    41    170,0,0      128,0,0      128,0,0      205,49,49    197,15,31    194,54,33    187,0,0      127,0,0      205,0,0      222,56,43
Green          32    42    0,170,0      0,128,0      0,128,0      13,188,121   19,161,14    37,188,36    0,187,0      0,147,0      0,205,0      57,181,74
Yellow         33    43    170,85,0     128,128,0    238,237,240  229,229,16   193,156,0    173,173,39   187,187,0    252,127,0    205,205,0    255,199,6
Blue           34    44    0,0,170      0,0,128      0,0,128      36,114,200   0,55,218     73,46,225    0,0,187      0,0,127      0,0,238      0,111,184
Magenta        35    45    170,0,170    128,0,128    1,36,86      188,63,188   136,23,152   211,56,211   187,0,187    156,0,156    205,0,205    118,38,113
Cyan           36    46    0,170,170    0,128,128    0,128,128    17,168,205   58,150,221   51,187,200   0,187,187    0,147,147    0,205,205    44,181,233
White          37    47    170,170,170  192,192,192  192,192,192  229,229,229  204,204,204  203,204,205  187,187,187  210,210,210  229,229,229  204,204,204
Bright Black   90    100   85,85,85     128,128,128  128,128,128  102,102,102  118,118,118  129,131,131  85,85,85     127,127,127  127,127,127  128,128,128
Bright Red     91    101   255,85,85    255,0,0      255,0,0      241,76,76    231,72,86    252,57,31    255,85,85    255,0,0      255,0,0      255,0,0
Bright Green   92    102   85,255,85    0,255,0      0,255,0      35,209,139   22,198,12    49,231,34    85,255,85    0,252,0      0,255,0      0,255,0
Bright Yellow  93    103   255,255,85   255,255,0    255,255,0    245,245,67   249,241,165  234,236,35   255,255,85   255,255,0    255,255,0    255,255,0
Bright Blue    94    104   85,85,255    0,0,255      0,0,255      59,142,234   59,120,255   88,51,255    85,85,255    0,0,252      92,92,255    0,0,255
Bright Magenta 95    105   255,85,255   255,0,255    255,0,255    214,112,214  180,0,158    249,53,248   255,85,255   255,0,255    255,0,255    255,0,255
Bright Cyan    96    106   85,255,255   0,255,255    0,255,255    41,184,219   97,214,214   20,240,240   85,255,255   0,255,255    0,255,255    0,255,255
Bright White   97    107   255,255,255  255,255,255  255,255,255  229,229,229  242,242,242  233,235,235  255,255,255  255,255,255  255,255,255  255,255,255


// https://en.wikipedia.org/wiki/ANSI_escape_code#8-bit

ESC[ 38;5;⟨n⟩ m         Select foreground color
ESC[ 48;5;⟨n⟩ m         Select background color

Standard colors	                     High-intensity colors
    0 	 1 	 2 	 3 	 4 	 5 	 6 	 7 		 8 	 9 	10	11	12	13	14	15
216 colors
16	17	18	19	20	21	22	23	24	25	26	27	28	29	30	31	32	33	34	35	36	37	38	39	40	41	42	43	44	45	46	47	48	49	50	51
52	53	54	55	56	57	58	59	60	61	62	63	64	65	66	67	68	69	70	71	72	73	74	75	76	77	78	79	80	81	82	83	84	85	86	87
88	89	90	91	92	93	94	95	96	97	98	99	100	101	102	103	104	105	106	107	108	109	110	111	112	113	114	115	116	117	118	119	120	121	122	123
124	125	126	127	128	129	130	131	132	133	134	135	136	137	138	139	140	141	142	143	144	145	146	147	148	149	150	151	152	153	154	155	156	157	158	159
160	161	162	163	164	165	166	167	168	169	170	171	172	173	174	175	176	177	178	179	180	181	182	183	184	185	186	187	188	189	190	191	192	193	194	195
196	197	198	199	200	201	202	203	204	205	206	207	208	209	210	211	212	213	214	215	216	217	218	219	220	221	222	223	224	225	226	227	228	229	230	231
Grayscale colors
232	233	234	235	236	237	238	239	240	241	242	243	244	245	246	247	248	249	250	251	252	253	254	255


// https://en.wikipedia.org/wiki/ANSI_escape_code#24-bit

ESC[ 38;2;⟨r⟩;⟨g⟩;⟨b⟩ m         Select RGB foreground color
ESC[ 48;2;⟨r⟩;⟨g⟩;⟨b⟩ m         Select RGB background color

    */


/*
public class ColorScheme
{
    public readonly ColorRef[] Colors = new ColorRef[16];
    public bool IsLight { get; }

    public ColorScheme(
        uint primary,
        uint secondary,
        uint background,
        uint error,
        uint onPrimary,
        uint onSecondary,
        uint onBackground,
        uint onError,

        uint? surface = null,
        uint? onSurface = null,
        uint? altPrimary = null,
        uint? onAltPrimary = null,
        uint? altSecondary = null,
        uint? onAltSecondary = null,
        bool isLight = false)
    {
        Colors[(int)ColorSlot.Black].SetColor(0x000000u);
        Colors[(int)ColorSlot.Primary].SetColor(primary);
        Colors[(int)ColorSlot.AltPrimary].SetColor(altPrimary ?? primary);
        Colors[(int)ColorSlot.Secondary].SetColor(secondary);
        Colors[(int)ColorSlot.AltSecondary].SetColor(altSecondary ?? secondary);
        Colors[(int)ColorSlot.Background].SetColor(background);
        Colors[(int)ColorSlot.Surface].SetColor(surface ?? background);
        Colors[(int)ColorSlot.Error].SetColor(error);
        Colors[(int)ColorSlot.OnPrimary].SetColor(onPrimary);
        Colors[(int)ColorSlot.OnAltPrimary].SetColor(onAltPrimary ?? onPrimary);
        Colors[(int)ColorSlot.OnSecondary].SetColor(onSecondary);
        Colors[(int)ColorSlot.OnAltSecondary].SetColor(onAltSecondary ?? onSecondary);
        Colors[(int)ColorSlot.OnBackground].SetColor(onBackground);
        Colors[(int)ColorSlot.OnSurface].SetColor(onSurface ?? onBackground);
        Colors[(int)ColorSlot.OnError].SetColor(onError);
        Colors[(int)ColorSlot.White].SetColor(0xFFFFFFu);

        IsLight = isLight;
    }

    public static CharacterAttributes Attributes(ColorSlot fore, ColorSlot back, bool reverse = false)
    {
        var attr = (CharacterAttributes)((uint)fore + ((uint)back << 4));
        if (reverse)
            attr |= CharacterAttributes.ReverseVideo;

        return attr;
    }

    public void GetRGB(ColorSlot slot, out uint r, out uint g, out uint b)
    {
        var color = Colors[(int)slot].ColorDWORD;
        r = color & 0x0000FFU;
        g = (color & 0x00FF00U) >> 8;
        b = (color & 0xFF0000U) >> 16;
    }

    public CharacterAttributes Normal => Attributes(ColorSlot.Surface, ColorSlot.OnSurface);
    public CharacterAttributes Primary => Attributes(ColorSlot.Surface, ColorSlot.Primary);
    public CharacterAttributes Secondary => Attributes(ColorSlot.Surface, ColorSlot.Secondary);
    public CharacterAttributes Background => Attributes(ColorSlot.Background, ColorSlot.OnBackground);
    public CharacterAttributes Surface => Attributes(ColorSlot.Surface, ColorSlot.OnSurface);
    public CharacterAttributes PrimaryBackground => Attributes(ColorSlot.Primary, ColorSlot.OnPrimary);
    public CharacterAttributes SecondaryBackground => Attributes(ColorSlot.Secondary, ColorSlot.OnSecondary);
    public CharacterAttributes AltPrimaryBackground => Attributes(ColorSlot.AltPrimary, ColorSlot.OnPrimary);
    public CharacterAttributes AltSecondaryBackground => Attributes(ColorSlot.AltSecondary, ColorSlot.OnSecondary);
}
*/

