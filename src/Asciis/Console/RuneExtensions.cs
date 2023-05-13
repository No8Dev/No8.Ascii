namespace No8.Ascii.Console;

public static class RuneExtensions
{
    /// <summary>
    ///     Used to calculate to width when displayed in a terminal
    /// </summary>
    private static readonly uint[,] Combining = new uint[,]
                                                {
                                                    { 0x0300, 0x036F }, { 0x0483, 0x0486 }, { 0x0488, 0x0489 },
                                                    { 0x0591, 0x05BD }, { 0x05BF, 0x05BF }, { 0x05C1, 0x05C2 },
                                                    { 0x05C4, 0x05C5 }, { 0x05C7, 0x05C7 }, { 0x0600, 0x0603 },
                                                    { 0x0610, 0x0615 }, { 0x064B, 0x065E }, { 0x0670, 0x0670 },
                                                    { 0x06D6, 0x06E4 }, { 0x06E7, 0x06E8 }, { 0x06EA, 0x06ED },
                                                    { 0x070F, 0x070F }, { 0x0711, 0x0711 }, { 0x0730, 0x074A },
                                                    { 0x07A6, 0x07B0 }, { 0x07EB, 0x07F3 }, { 0x0901, 0x0902 },
                                                    { 0x093C, 0x093C }, { 0x0941, 0x0948 }, { 0x094D, 0x094D },
                                                    { 0x0951, 0x0954 }, { 0x0962, 0x0963 }, { 0x0981, 0x0981 },
                                                    { 0x09BC, 0x09BC }, { 0x09C1, 0x09C4 }, { 0x09CD, 0x09CD },
                                                    { 0x09E2, 0x09E3 }, { 0x0A01, 0x0A02 }, { 0x0A3C, 0x0A3C },
                                                    { 0x0A41, 0x0A42 }, { 0x0A47, 0x0A48 }, { 0x0A4B, 0x0A4D },
                                                    { 0x0A70, 0x0A71 }, { 0x0A81, 0x0A82 }, { 0x0ABC, 0x0ABC },
                                                    { 0x0AC1, 0x0AC5 }, { 0x0AC7, 0x0AC8 }, { 0x0ACD, 0x0ACD },
                                                    { 0x0AE2, 0x0AE3 }, { 0x0B01, 0x0B01 }, { 0x0B3C, 0x0B3C },
                                                    { 0x0B3F, 0x0B3F }, { 0x0B41, 0x0B43 }, { 0x0B4D, 0x0B4D },
                                                    { 0x0B56, 0x0B56 }, { 0x0B82, 0x0B82 }, { 0x0BC0, 0x0BC0 },
                                                    { 0x0BCD, 0x0BCD }, { 0x0C3E, 0x0C40 }, { 0x0C46, 0x0C48 },
                                                    { 0x0C4A, 0x0C4D }, { 0x0C55, 0x0C56 }, { 0x0CBC, 0x0CBC },
                                                    { 0x0CBF, 0x0CBF }, { 0x0CC6, 0x0CC6 }, { 0x0CCC, 0x0CCD },
                                                    { 0x0CE2, 0x0CE3 }, { 0x0D41, 0x0D43 }, { 0x0D4D, 0x0D4D },
                                                    { 0x0DCA, 0x0DCA }, { 0x0DD2, 0x0DD4 }, { 0x0DD6, 0x0DD6 },
                                                    { 0x0E31, 0x0E31 }, { 0x0E34, 0x0E3A }, { 0x0E47, 0x0E4E },
                                                    { 0x0EB1, 0x0EB1 }, { 0x0EB4, 0x0EB9 }, { 0x0EBB, 0x0EBC },
                                                    { 0x0EC8, 0x0ECD }, { 0x0F18, 0x0F19 }, { 0x0F35, 0x0F35 },
                                                    { 0x0F37, 0x0F37 }, { 0x0F39, 0x0F39 }, { 0x0F71, 0x0F7E },
                                                    { 0x0F80, 0x0F84 }, { 0x0F86, 0x0F87 }, { 0x0F90, 0x0F97 },
                                                    { 0x0F99, 0x0FBC }, { 0x0FC6, 0x0FC6 }, { 0x102D, 0x1030 },
                                                    { 0x1032, 0x1032 }, { 0x1036, 0x1037 }, { 0x1039, 0x1039 },
                                                    { 0x1058, 0x1059 }, { 0x1160, 0x11FF }, { 0x135F, 0x135F },
                                                    { 0x1712, 0x1714 }, { 0x1732, 0x1734 }, { 0x1752, 0x1753 },
                                                    { 0x1772, 0x1773 }, { 0x17B4, 0x17B5 }, { 0x17B7, 0x17BD },
                                                    { 0x17C6, 0x17C6 }, { 0x17C9, 0x17D3 }, { 0x17DD, 0x17DD },
                                                    { 0x180B, 0x180D }, { 0x18A9, 0x18A9 }, { 0x1920, 0x1922 },
                                                    { 0x1927, 0x1928 }, { 0x1932, 0x1932 }, { 0x1939, 0x193B },
                                                    { 0x1A17, 0x1A18 }, { 0x1B00, 0x1B03 }, { 0x1B34, 0x1B34 },
                                                    { 0x1B36, 0x1B3A }, { 0x1B3C, 0x1B3C }, { 0x1B42, 0x1B42 },
                                                    { 0x1B6B, 0x1B73 }, { 0x1DC0, 0x1DCA }, { 0x1DFE, 0x1DFF },
                                                    { 0x200B, 0x200F }, { 0x202A, 0x202E }, { 0x2060, 0x2063 },
                                                    { 0x206A, 0x206F }, { 0x20D0, 0x20EF }, { 0x302A, 0x302F },
                                                    { 0x3099, 0x309A }, { 0xA806, 0xA806 }, { 0xA80B, 0xA80B },
                                                    { 0xA825, 0xA826 }, { 0xFB1E, 0xFB1E }, { 0xFE00, 0xFE0F },
                                                    { 0xFE20, 0xFE23 }, { 0xFEFF, 0xFEFF }, { 0xFFF9, 0xFFFB },
                                                    { 0x10A01, 0x10A03 }, { 0x10A05, 0x10A06 }, { 0x10A0C, 0x10A0F },
                                                    { 0x10A38, 0x10A3A }, { 0x10A3F, 0x10A3F }, { 0x1D167, 0x1D169 },
                                                    { 0x1D173, 0x1D182 }, { 0x1D185, 0x1D18B }, { 0x1D1AA, 0x1D1AD },
                                                    { 0x1D242, 0x1D244 }, { 0xE0001, 0xE0001 }, { 0xE0020, 0xE007F },
                                                    { 0xE0100, 0xE01EF }
                                                };

    private static int Bisearch(uint rune, uint[,] table, int max)
    {
        var min = 0;
        int mid;

        if (rune < table[0, 0] || rune > table[max, 1])
            return 0;
        while (max >= min)
        {
            mid = (min + max) / 2;
            if (rune > table[mid, 1])
                min = mid + 1;
            else if (rune < table[mid, 0])
                max = mid - 1;
            else
                return 1;
        }

        return 0;
    }

    /// <summary>
    /// Number of column positions of a wide-character code.   This is used to measure runes as displayed by text-based terminals.
    /// </summary>
    /// <returns>The width in columns, 0 if the argument is the null character, -1 if the value is not printable, otherwise the number of columns that the rune occupies.</returns>
    /// <param name="rune">The rune.</param>
    public static int ColumnWidth(this Rune rune)
    {
        var irune = (uint)rune.Value;
        if (irune < 32 || irune >= 0x7f && irune <= 0xa0)
            return -1;
        if (irune < 127)
            return 1;
        /* binary search in table of non-spacing characters */
        if (Bisearch(irune, Combining, Combining.GetLength(0) - 1) != 0)
            return 0;
        /* if we arrive here, ucs is not a combining or C0/C1 control character */
        return 1 +
               (irune >= 0x1100 &&
                (irune <= 0x115f ||                    /* Hangul Jamo init. consonants */
                 irune == 0x2329 || irune == 0x232a || /* Miscellaneous Technical */
                 irune >= 0x2e80 && irune <= 0xa4cf &&
                 irune != 0x303f ||                      /* CJK ... Yi */
                 irune >= 0xac00 && irune <= 0xd7a3 ||   /* Hangul Syllables */
                 irune >= 0xf900 && irune <= 0xfaff ||   /* CJK Compatibility Ideographs */
                 irune >= 0xfe10 && irune <= 0xfe19 ||   /* Vertical forms */
                 irune >= 0xfe30 && irune <= 0xfe6f ||   /* CJK Compatibility Forms */
                 irune >= 0xff00 && irune <= 0xff60 ||   /* Fullwidth Forms */
                 irune >= 0xffe0 && irune <= 0xffe6 ||   /* Alphabetic Presentation Forms*/
                 irune >= 0x1fa00 && irune <= 0x1facf || /* Chess Symbols*/
                 irune >= 0x20000 && irune <= 0x2fffd ||
                 irune >= 0x30000 && irune <= 0x3fffd)
                   ? 1
                   : 0);
    }


    /// <summary>
    /// Returns the number of columns used by the unicode string on console applications.  This is done by calling the Rune.ColumnWidth on each rune.
    /// </summary>
    public static int ConsoleWidth(this string str) { return str.ToRuneArray().Sum(r => r.ColumnWidth()); }

    /// <summary>
    /// Gets the rune count of the string.
    /// </summary>
    /// <value>The rune count.</value>
    public static int RuneCount(this string str) { return str.EnumerateRunes().ToArray().Length; }

    /// <summary>
    /// Converts a string into a rune array.
    /// </summary>
    /// <returns>An array containing the runes for the string up to the specified limit.</returns>
    /// <param name="str">Source string</param>
    /// <param name="limit">Maximum number of entries to return, or -1 for no limits.</param>
    public static List<Rune> ToRuneList(this string str, int limit = -1)
    {
        var max = str.RuneCount();
        if (limit > -1 && max > limit)
            max = limit;

        return str
              .EnumerateRunes()
              .Take(max)
              .ToList();
    }

    /// <summary>
    /// Converts a string into a rune array.
    /// </summary>
    /// <returns>An array containing the runes for the string up to the specified limit.</returns>
    /// <param name="str">Source string</param>
    /// <param name="limit">Maximum number of entries to return, or -1 for no limits.</param>
    public static Rune[] ToRuneArray(this string str, int limit = -1)
    {
        var max = str.RuneCount();
        if (limit > -1 && max > limit)
            max = limit;

        return str
              .EnumerateRunes()
              .Take(max)
              .ToArray();
    }

    /// <summary>
    /// Count the number of non-overlapping instances of substr in the string.
    /// </summary>
    /// <returns>If substr is an empty string, Count returns 1 + the number of Unicode code points in the string, otherwise the count of non-overlapping instances in string.</returns>
    public static int CountSubString(this string str, string? substr)
    {
        if (substr is null)
            throw new ArgumentNullException(nameof(substr));

        var n = 0;
        if (substr.Length == 0)
            return str.RuneCount() + 1;
        var offset = 0;
        var len = str.Length;
        var slen = substr.Length;
        while (offset < len)
        {
            var i = str.IndexOf(substr, offset, StringComparison.Ordinal);
            if (i == -1)
                break;
            n++;
            offset = i + slen;
        }

        return n;
    }

    public static string MakeString(this List<Rune> runes)
    {
        var sb = new StringBuilder();
        foreach (var rune in runes)
            sb.Append(rune.ToString());
        return sb.ToString();
    }

    public static string MakeString(this Rune[] runes)
    {
        var sb = new StringBuilder();
        foreach (var rune in runes)
            sb.Append(rune.ToString());
        return sb.ToString();
    }

    public static Rune[] Substring(this Rune[] runes, int startIndex, int length = -1)
    {
        if (length < 0)
            length = runes.Length - startIndex;
        return runes[startIndex..(startIndex + length)].ToArray();
    }

    public static bool IsEmpty(this string str) { return string.IsNullOrEmpty(str); }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:System.Rune"/> can be encoded as UTF-8
    /// </summary>
    /// <value><c>true</c> if is valid; otherwise, <c>false</c>.</value>
    public static bool IsValid(this Rune rune)
    {
        if (0 <= rune.Value && rune.Value <= SurrogateMin)
            return true;
        if (SurrogateMax <= rune.Value && rune.Value <= MaxRune)
            return true;
        return false;
    }

    public static bool IsLetterOrNumber(this Rune rune) { return Rune.IsLetter(rune) || Rune.IsNumber(rune); }

    // Code points in the surrogate range are not valid for UTF-8.
    private const uint SurrogateMin = 0xd800;
    private const uint SurrogateMax = 0xdfff;
    private const uint MaxRune = 0x10ffff;
}
