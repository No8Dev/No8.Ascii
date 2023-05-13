using System.Net.Mime;
using Asciis.Terminal.Helpers;

namespace Asciis.Terminal.Core;

/// <summary>
/// Provides text formatting capabilities for console apps. Supports, hotkeys, horizontal alignment, multiple lines, and word-based line wrap.
/// </summary>
public class TextFormatter
{
    private List<string> lines = new();
    private string text;
    private TextAlignment textAlignment;
    private VerticalTextAlignment textVerticalAlignment;
    private TextDirection textDirection;
    private CellAttributes textColor = CellAttributes.Empty;
    private bool needsFormat;
    private Key hotKey;
    private Size size;

    /// <summary>
    ///   The text to be displayed. This text is never modified.
    /// </summary>
    public virtual string Text
    {
        get => text;
        set
        {
            text = value;

            if (text.RuneCount() > 0 && (Size.Width == 0 || Size.Height == 0 || Size.Width != text.RuneCount()))
                // Provide a default size (width = length of longest line, height = 1)
                // TODO: It might makes more sense for the default to be width = length of first line?
                Size = new Size(MaxWidth(Text, int.MaxValue), 1);

            NeedsFormat = true;
        }
    }

    /// <summary>
    /// Used by <see cref="Text"/> to resize the view's <see cref="View.Bounds"/> with the <see cref="Size"/>.
    /// Setting <see cref="AutoSize"/> to true only work if the <see cref="View.Width"/> and <see cref="View.Height"/> are null or
    ///   <see cref="LayoutStyle.Absolute"/> values and doesn't work with <see cref="LayoutStyle.Computed"/> layout,
    ///   to avoid breaking the <see cref="Pos"/> and <see cref="Dim"/> settings.
    /// </summary>
    public bool AutoSize { get; set; }

    // TODO: Add Vertical Text Alignment
    /// <summary>
    /// Controls the horizontal text-alignment property. 
    /// </summary>
    /// <value>The text alignment.</value>
    public TextAlignment Alignment
    {
        get => textAlignment;
        set
        {
            textAlignment = value;
            NeedsFormat = true;
        }
    }

    /// <summary>
    /// Controls the vertical text-alignment property. 
    /// </summary>
    /// <value>The text vertical alignment.</value>
    public VerticalTextAlignment VerticalAlignment
    {
        get => textVerticalAlignment;
        set
        {
            textVerticalAlignment = value;
            NeedsFormat = true;
        }
    }

    /// <summary>
    /// Controls the text-direction property. 
    /// </summary>
    /// <value>The text vertical alignment.</value>
    public TextDirection Direction
    {
        get => textDirection;
        set
        {
            textDirection = value;
            NeedsFormat = true;
        }
    }

    /// <summary>
    /// Check if it is a horizontal direction
    /// </summary>
    public static bool IsHorizontalDirection(TextDirection textDirection)
    {
        switch (textDirection)
        {
            case TextDirection.LeftRight_TopBottom:
            case TextDirection.LeftRight_BottomTop:
            case TextDirection.RightLeft_TopBottom:
            case TextDirection.RightLeft_BottomTop:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Check if it is a vertical direction
    /// </summary>
    public static bool IsVerticalDirection(TextDirection textDirection)
    {
        switch (textDirection)
        {
            case TextDirection.TopBottom_LeftRight:
            case TextDirection.TopBottom_RightLeft:
            case TextDirection.BottomTop_LeftRight:
            case TextDirection.BottomTop_RightLeft:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Check if it is Left to Right direction
    /// </summary>
    public static bool IsLeftToRight(TextDirection textDirection)
    {
        switch (textDirection)
        {
            case TextDirection.LeftRight_TopBottom:
            case TextDirection.LeftRight_BottomTop:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Check if it is Top to Bottom direction
    /// </summary>
    public static bool IsTopToBottom(TextDirection textDirection)
    {
        switch (textDirection)
        {
            case TextDirection.TopBottom_LeftRight:
            case TextDirection.TopBottom_RightLeft:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    ///  Gets or sets the size of the area the text will be constrained to when formatted.
    /// </summary>
    public Size Size
    {
        get => size;
        set
        {
            size = value;
            NeedsFormat = true;
        }
    }

    /// <summary>
    /// The specifier character for the hotkey (e.g. '_'). Set to '\xffff' to disable hotkey support for this View instance. The default is '\xffff'.
    /// </summary>
    public Rune HotKeySpecifier { get; set; } = (Rune)0xFFFF;

    /// <summary>
    /// The position in the text of the hotkey. The hotkey will be rendered using the hot color.
    /// </summary>
    public int HotKeyPos
    {
        get => hotKeyPos;
        set => hotKeyPos = value;
    }

    /// <summary>
    /// Gets the hotkey. Will be an upper case letter or digit.
    /// </summary>
    public Key HotKey
    {
        get => hotKey;
        internal set => hotKey = value;
    }

    /// <summary>
    /// Specifies the mask to apply to the hotkey to tag it as the hotkey. The default value of <c>0x100000</c> causes
    /// the underlying Rune to be identified as a "private use" Unicode character.
    /// </summary>HotKeyTagMask
    public uint HotKeyTagMask { get; set; } = 0x100000;

    /// <summary>
    /// Gets the cursor position from <see cref="HotKey"/>. If the <see cref="HotKey"/> is defined, the cursor will be positioned over it.
    /// </summary>
    public int CursorPosition { get; set; }

    /// <summary>
    /// Gets the formatted lines.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Upon a 'get' of this property, if the text needs to be formatted (if <see cref="NeedsFormat"/> is <c>true</c>)
    /// <see cref="Format(string, int, bool, bool, bool, int)"/> will be called internally. 
    /// </para>
    /// </remarks>
    public List<string> Lines
    {
        get
        {
            // With this check, we protect against subclasses with overrides of Text
            if (string.IsNullOrEmpty(Text))
            {
                lines = new List<string>();
                lines.Add(string.Empty);
                NeedsFormat = false;
                return lines;
            }

            if (NeedsFormat)
            {
                var shown_text = text;
                if (FindHotKey(text, HotKeySpecifier, true, out hotKeyPos, out hotKey))
                {
                    shown_text = RemoveHotKeySpecifier(Text, hotKeyPos, HotKeySpecifier);
                    shown_text = ReplaceHotKeyWithTag(shown_text, hotKeyPos);
                }

                if (Size.IsEmpty) throw new InvalidOperationException("Size must be set before accessing Lines");

                if (IsVerticalDirection(textDirection))
                {
                    lines = Format(
                        shown_text,
                        Size.Height,
                        textVerticalAlignment == VerticalTextAlignment.Justified,
                        Size.Width > 1);
                    if (!AutoSize && lines.Count > Size.Width)
                        lines.RemoveRange(Size.Width, lines.Count - Size.Width);
                }
                else
                {
                    lines = Format(
                        shown_text,
                        Size.Width,
                        textAlignment == TextAlignment.Justified,
                        Size.Height > 1);
                    if (!AutoSize && lines.Count > Size.Height)
                        lines.RemoveRange(Size.Height, lines.Count - Size.Height);
                }

                NeedsFormat = false;
            }

            return lines;
        }
    }

    /// <summary>
    /// Gets or sets whether the <see cref="TextFormatter"/> needs to format the text when <see cref="TextFormatter.Draw(Rectangle, CellAttributes, CellAttributes)"/> is called.
    /// If it is <c>false</c> when Draw is called, the Draw call will be faster.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is set to true when the properties of <see cref="TextFormatter"/> are set.
    /// </para>
    /// </remarks>
    public bool NeedsFormat
    {
        get => needsFormat;
        set => needsFormat = value;
    }

    private static string StripCRLF(string str)
    {
        var runes = str.ToRuneList();
        for (var i = 0; i < runes.Count; i++)
            switch ((char)runes[i].Value)
            {
                case '\n':
                    runes.RemoveAt(i);
                    break;

                case '\r':
                    if (i + 1 < runes.Count && runes[i + 1].Value == '\n')
                    {
                        runes.RemoveAt(i);
                        runes.RemoveAt(i + 1);
                        i++;
                    }
                    else
                    {
                        runes.RemoveAt(i);
                    }

                    break;
            }

        return runes.MakeString();
    }

    private static string ReplaceCRLFWithSpace(string str)
    {
        var runes = str.ToRuneList();
        for (var i = 0; i < runes.Count; i++)
            switch (runes[i].Value)
            {
                case '\n':
                    runes[i] = (Rune)' ';
                    break;

                case '\r':
                    if (i + 1 < runes.Count && runes[i + 1].Value == '\n')
                    {
                        runes[i] = (Rune)' ';
                        runes.RemoveAt(i + 1);
                        i++;
                    }
                    else
                    {
                        runes[i] = (Rune)' ';
                    }

                    break;
            }

        return runes.MakeString();
    }


    /// <summary>
    /// Adds trailing whitespace or truncates <paramref name="text"/>
    /// so that it fits exactly <paramref name="width"/> console units.
    /// Note that some unicode characters take 2+ columns
    /// </summary>
    /// <param name="text"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static string ClipOrPad(string text, int width)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // if value is not wide enough
        if (text.Sum(c => ((Rune)c).ColumnWidth()) < width)
        {
            // pad it out with spaces to the given alignment
            var toPad = width - text.Sum(c => ((Rune)c).ColumnWidth());

            return text + new string(' ', toPad);
        }

        // value is too wide
        return new string(text.TakeWhile(c => (width -= ((Rune)c).ColumnWidth()) >= 0).ToArray());
    }

    /// <summary>
    /// Formats the provided text to fit within the width provided using word wrapping.
    /// </summary>
    /// <param name="text">The text to word wrap</param>
    /// <param name="width">The width to contain the text to</param>
    /// <param name="preserveTrailingSpaces">If <c>true</c>, the wrapped text will keep the trailing spaces.
    ///  If <c>false</c>, the trailing spaces will be trimmed.</param>
    /// <param name="tabWidth">The tab width.</param>
    /// <returns>Returns a list of word wrapped lines.</returns>
    /// <remarks>
    /// <para>
    /// This method does not do any justification.
    /// </para>
    /// <para>
    /// This method strips Newline ('\n' and '\r\n') sequences before processing.
    /// </para>
    /// </remarks>
    public static List<string> WordWrap(
        string text,
        int width,
        bool preserveTrailingSpaces = false,
        int tabWidth = 0)
    {
        if (width < 0) throw new ArgumentOutOfRangeException("Width cannot be negative.");

        int start = 0, end;
        var lines = new List<string>();

        if (string.IsNullOrEmpty(text)) return lines;

        var runes = StripCRLF(text).ToRuneList();
        if (!preserveTrailingSpaces)
            while ((end = start + width) < runes.Count)
            {
                while (runes[end].Value != ' ' && end > start)
                    end--;
                if (end == start)
                    end = start + width;
                lines.Add(runes.GetRange(start, end - start).MakeString());
                start = end;
                if (runes[end].Value == ' ') start++;
            }
        else
            while ((end = start) < runes.Count)
            {
                end = GetNextWhiteSpace(start, width);
                lines.Add(runes.GetRange(start, end - start).MakeString());
                start = end;
            }

        int GetNextWhiteSpace(int from, int cWidth, int cLength = 0)
        {
            var to = from;
            var length = cLength;

            while (length < cWidth && to < runes.Count)
            {
                var rune = runes[to];
                length += rune.ColumnWidth();
                if (rune.Value == ' ')
                {
                    if (length == cWidth)
                        return to + 1;
                    else if (length > cWidth)
                        return to;
                    else
                        return GetNextWhiteSpace(to + 1, cWidth, length);
                }
                else if (rune.Value == '\t')
                {
                    length += tabWidth + 1;
                    if (length == tabWidth && tabWidth > cWidth)
                        return to + 1;
                    else if (length > cWidth && tabWidth > cWidth)
                        return to;
                    else
                        return GetNextWhiteSpace(to + 1, cWidth, length);
                }

                to++;
            }

            if (cLength > 0 && to < runes.Count && runes[to].Value != ' ')
                return @from;
            else
                return to;
        }

        if (start < text.RuneCount()) lines.Add(runes.GetRange(start, runes.Count - start).MakeString());

        return lines;
    }

    /// <summary>
    /// Justifies text within a specified width. 
    /// </summary>
    /// <param name="text">The text to justify.</param>
    /// <param name="width">If the text length is greater that <c>width</c> it will be clipped.</param>
    /// <param name="talign">Alignment.</param>
    /// <returns>Justified and clipped text.</returns>
    public static string ClipAndJustify(string text, int width, TextAlignment talign)
    {
        return ClipAndJustify(text, width, talign == TextAlignment.Justified);
    }

    /// <summary>
    /// Justifies text within a specified width. 
    /// </summary>
    /// <param name="text">The text to justify.</param>
    /// <param name="width">If the text length is greater that <c>width</c> it will be clipped.</param>
    /// <param name="justify">Justify.</param>
    /// <returns>Justified and clipped text.</returns>
    public static string ClipAndJustify(string text, int width, bool justify)
    {
        if (width < 0) throw new ArgumentOutOfRangeException("Width cannot be negative.");
        if (string.IsNullOrEmpty(text)) return text;

        var runes = text.ToRuneList();
        var slen = runes.Count;
        if (slen > width)
        {
            return runes.GetRange(0, width).MakeString();
        }
        else
        {
            if (justify)
                return Justify(text, width);

            return text;
        }
    }

    /// <summary>
    /// Justifies the text to fill the width provided. Space will be added between words (demarked by spaces and tabs) to
    /// make the text just fit <c>width</c>. Spaces will not be added to the ends.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="width"></param>
    /// <param name="spaceChar">Character to replace whitespace and pad with. For debugging purposes.</param>
    /// <returns>The justified text.</returns>
    public static string Justify(string text, int width, char spaceChar = ' ')
    {
        if (width < 0) throw new ArgumentOutOfRangeException("Width cannot be negative.");
        if (string.IsNullOrEmpty(text)) return text;

        var words = text.Split(' ');
        var textCount = words.Sum(arg => arg.RuneCount());

        var spaces = words.Length > 1 ? (width - textCount) / (words.Length - 1) : 0;
        var extras = words.Length > 1 ? (width - textCount) % words.Length : 0;

        var s = new StringBuilder();
        for (var w = 0; w < words.Length; w++)
        {
            var x = words[w];
            s.Append(x);
            if (w + 1 < words.Length)
                for (var i = 0; i < spaces; i++)
                    s.Append(spaceChar);
            if (extras > 0) extras--;
        }

        return s.ToString();
    }

    private static char[] whitespace = new char[] { ' ', '\t' };
    private int hotKeyPos;

    /// <summary>
    /// Reformats text into lines, applying text alignment and optionally wrapping text to new lines on word boundaries.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="width">The width to bound the text to for word wrapping and clipping.</param>
    /// <param name="talign">Specifies how the text will be aligned horizontally.</param>
    /// <param name="wordWrap">If <c>true</c>, the text will be wrapped to new lines as need. If <c>false</c>, forces text to fit a single line. Line breaks are converted to spaces. The text will be clipped to <c>width</c></param>
    /// <param name="preserveTrailingSpaces">If <c>true</c> and 'wordWrap' also true, the wrapped text will keep the trailing spaces. If <c>false</c>, the trailing spaces will be trimmed.</param>
    /// <param name="tabWidth">The tab width.</param>
    /// <returns>A list of word wrapped lines.</returns>
    /// <remarks>
    /// <para>
    /// An empty <c>text</c> string will result in one empty line.
    /// </para>
    /// <para>
    /// If <c>width</c> is 0, a single, empty line will be returned.
    /// </para>
    /// <para>
    /// If <c>width</c> is int.MaxValue, the text will be formatted to the maximum width possible. 
    /// </para>
    /// </remarks>
    public static List<string> Format(
        string text,
        int width,
        TextAlignment talign,
        bool wordWrap,
        bool preserveTrailingSpaces = false,
        int tabWidth = 0)
    {
        return Format(text, width, talign == TextAlignment.Justified, wordWrap, preserveTrailingSpaces, tabWidth);
    }

    /// <summary>
    /// Reformats text into lines, applying text alignment and optionally wrapping text to new lines on word boundaries.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="width">The width to bound the text to for word wrapping and clipping.</param>
    /// <param name="justify">Specifies whether the text should be justified.</param>
    /// <param name="wordWrap">If <c>true</c>, the text will be wrapped to new lines as need. If <c>false</c>, forces text to fit a single line. Line breaks are converted to spaces. The text will be clipped to <c>width</c></param>
    /// <param name="preserveTrailingSpaces">If <c>true</c> and 'wordWrap' also true, the wrapped text will keep the trailing spaces. If <c>false</c>, the trailing spaces will be trimmed.</param>
    /// <param name="tabWidth">The tab width.</param>
    /// <returns>A list of word wrapped lines.</returns>
    /// <remarks>
    /// <para>
    /// An empty <c>text</c> string will result in one empty line.
    /// </para>
    /// <para>
    /// If <c>width</c> is 0, a single, empty line will be returned.
    /// </para>
    /// <para>
    /// If <c>width</c> is int.MaxValue, the text will be formatted to the maximum width possible. 
    /// </para>
    /// </remarks>
    public static List<string> Format(
        string text,
        int width,
        bool justify,
        bool wordWrap,
        bool preserveTrailingSpaces = false,
        int tabWidth = 0)
    {
        if (width < 0) throw new ArgumentOutOfRangeException("width cannot be negative");
        if (preserveTrailingSpaces && !wordWrap)
            throw new ArgumentException(
                "if 'preserveTrailingSpaces' is true, then 'wordWrap' must be true either.");
        List<string> lineResult = new();

        if (string.IsNullOrEmpty(text) || width == 0)
        {
            lineResult.Add(string.Empty);
            return lineResult;
        }

        if (wordWrap == false)
        {
            text = ReplaceCRLFWithSpace(text);
            lineResult.Add(ClipAndJustify(text, width, justify));
            return lineResult;
        }

        var runes = text.ToRuneList();
        var runeCount = runes.Count;
        var lp = 0;
        for (var i = 0; i < runeCount; i++)
        {
            var c = runes[i];
            if (c.Value == '\n')
            {
                var wrappedLines = WordWrap(
                    runes.GetRange(lp, i - lp).MakeString(),
                    width,
                    preserveTrailingSpaces,
                    tabWidth);
                foreach (var line in wrappedLines) lineResult.Add(ClipAndJustify(line, width, justify));
                if (wrappedLines.Count == 0) lineResult.Add(string.Empty);
                lp = i + 1;
            }
        }

        foreach (var line in WordWrap(
            runes.GetRange(lp, runeCount - lp).MakeString(),
            width,
            preserveTrailingSpaces,
            tabWidth)) lineResult.Add(ClipAndJustify(line, width, justify));

        return lineResult;
    }

    /// <summary>
    /// Computes the number of lines needed to render the specified text given the width.
    /// </summary>
    /// <returns>Number of lines.</returns>
    /// <param name="text">Text, may contain newlines.</param>
    /// <param name="width">The minimum width for the text.</param>
    public static int MaxLines(string text, int width)
    {
        var result = Format(text, width, false, true);
        return result.Count;
    }

    /// <summary>
    /// Computes the maximum width needed to render the text (single line or multiple lines) given a minimum width.
    /// </summary>
    /// <returns>Max width of lines.</returns>
    /// <param name="text">Text, may contain newlines.</param>
    /// <param name="width">The minimum width for the text.</param>
    public static int MaxWidth(string text, int width)
    {
        var result = Format(text, width, false, true);
        var max = 0;
        result.ForEach(
            s =>
            {
                var m = 0;
                s.ToRuneList().ForEach(r => m += r.ColumnWidth());
                if (m > max) max = m;
            });
        return max;
    }

    /// <summary>
    ///  Calculates the rectangle required to hold text, assuming no word wrapping.
    /// </summary>
    /// <param name="x">The x location of the rectangle</param>
    /// <param name="y">The y location of the rectangle</param>
    /// <param name="text">The text to measure</param>
    /// <param name="direction">The text direction.</param>
    /// <returns></returns>
    public static Rectangle CalcRect(
        int x,
        int y,
        string text,
        TextDirection direction = TextDirection.LeftRight_TopBottom)
    {
        if (string.IsNullOrEmpty(text)) return new Rectangle(new Point(x, y), Size.Empty);

        int w, h;

        if (IsHorizontalDirection(direction))
        {
            var mw = 0;
            var ml = 1;

            var cols = 0;
            foreach (var rune in text.EnumerateRunes())
                if (rune.Value == '\n')
                {
                    ml++;
                    if (cols > mw) mw = cols;
                    cols = 0;
                }
                else
                {
                    if (rune.Value != '\r')
                    {
                        cols++;
                        var rw = rune.ColumnWidth();
                        if (rw > 0) rw--;
                        cols += rw;
                    }
                }

            if (cols > mw) mw = cols;
            w = mw;
            h = ml;
        }
        else
        {
            var vw = 0;
            var vh = 0;

            var rows = 0;
            foreach (var rune in text.EnumerateRunes())
                if (rune.Value == '\n')
                {
                    vw++;
                    if (rows > vh) vh = rows;
                    rows = 0;
                }
                else
                {
                    if (rune.Value != '\r')
                    {
                        rows++;
                        var rw = rune.ColumnWidth();
                        if (rw < 0) rw++;
                        if (rw > vw) vw = rw;
                    }
                }

            if (rows > vh) vh = rows;
            w = vw;
            h = vh;
        }

        return new Rectangle(x, y, w, h);
    }

    /// <summary>
    /// Finds the hotkey and its location in text. 
    /// </summary>
    /// <param name="text">The text to look in.</param>
    /// <param name="hotKeySpecifier">The hotkey specifier (e.g. '_') to look for.</param>
    /// <param name="firstUpperCase">If <c>true</c> the legacy behavior of identifying the first upper case character as the hotkey will be enabled.
    /// Regardless of the value of this parameter, <c>hotKeySpecifier</c> takes precedence.</param>
    /// <param name="hotPos">Outputs the Rune index into <c>text</c>.</param>
    /// <param name="hotKey">Outputs the hotKey.</param>
    /// <returns><c>true</c> if a hotkey was found; <c>false</c> otherwise.</returns>
    public static bool FindHotKey(
        string text,
        Rune hotKeySpecifier,
        bool firstUpperCase,
        out int hotPos,
        out Key hotKey)
    {
        if (string.IsNullOrEmpty(text) || hotKeySpecifier == (Rune)0xFFFF)
        {
            hotPos = -1;
            hotKey = Key.Unknown;
            return false;
        }

        var hot_key = (Rune)0;
        var hot_pos = -1;

        // Use first hot_key char passed into 'hotKey'.
        // TODO: Ignore hot_key of two are provided
        // TODO: Do not support non-alphanumeric chars that can't be typed
        var i = 0;
        foreach (var c in text.EnumerateRunes())
        {
            if (c.Value != 0xFFFD)
            {
                if (c == hotKeySpecifier)
                {
                    hot_pos = i;
                }
                else if (hot_pos > -1)
                {
                    hot_key = c;
                    break;
                }
            }

            i++;
        }


        // Legacy support - use first upper case char if the specifier was not found
        if (hot_pos == -1 && firstUpperCase)
        {
            i = 0;
            foreach (var c in text.EnumerateRunes())
            {
                if (c.Value != 0xFFFD)
                    if (Rune.IsUpper(c))
                    {
                        hot_key = c;
                        hot_pos = i;
                        break;
                    }

                i++;
            }
        }

        if (hot_key != (Rune)0 && hot_pos != -1)
        {
            hotPos = hot_pos;

            if (hot_key.IsValid() && char.IsLetterOrDigit((char)hot_key.Value))
            {
                hotKey = (Key)char.ToUpperInvariant((char)hot_key.Value);
                return true;
            }
        }

        hotPos = -1;
        hotKey = Key.Unknown;
        return false;
    }

    /// <summary>
    /// Replaces the Rune at the index specified by the <c>hotPos</c> parameter with a tag identifying 
    /// it as the hotkey.
    /// </summary>
    /// <param name="text">The text to tag the hotkey in.</param>
    /// <param name="hotPos">The Rune index of the hotkey in <c>text</c>.</param>
    /// <returns>The text with the hotkey tagged.</returns>
    /// <remarks>
    /// The returned string will not render correctly without first un-doing the tag. To undo the tag, search for 
    /// Runes with a bitmask of <c>otKeyTagMask</c> and remove that bitmask.
    /// </remarks>
    public string ReplaceHotKeyWithTag(string text, int hotPos)
    {
        // Set the high bit
        var runes = text.ToRuneList();
        if (runes[hotPos].IsLetterOrNumber()) runes[hotPos] = (Rune)((uint)runes[hotPos].Value | HotKeyTagMask);
        return runes.MakeString();
    }

    /// <summary>
    /// Removes the hotkey specifier from text.
    /// </summary>
    /// <param name="text">The text to manipulate.</param>
    /// <param name="hotKeySpecifier">The hot-key specifier (e.g. '_') to look for.</param>
    /// <param name="hotPos">Returns the position of the hot-key in the text. -1 if not found.</param>
    /// <returns>The input text with the hotkey specifier ('_') removed.</returns>
    public static string RemoveHotKeySpecifier(string text, int hotPos, Rune hotKeySpecifier)
    {
        if (string.IsNullOrEmpty(text)) return text;

        // Scan 
        string start = string.Empty;
        var i = 0;
        foreach (Rune r in text)
        {
            if (r == hotKeySpecifier && i == hotPos)
            {
                i++;
                continue;
            }

            start += r.ToString();
            i++;
        }

        return start;
    }

    /// <summary>
    /// Draws the text held by <see cref="TextFormatter"/> to <see cref="MediaTypeNames.Application.Driver"/> using the colors specified.
    /// </summary>
    /// <param name="bounds">Specifies the screen-relative location and maximum size for drawing the text.</param>
    /// <param name="normalColor">The color to use for all text except the hotkey</param>
    /// <param name="hotColor">The color to use to draw the hotkey</param>
    public void Draw(ConsoleDriver driver, Rectangle bounds, CellAttributes normalColor, CellAttributes hotColor)
    {
        // With this check, we protect against subclasses with overrides of Text (like Button)
        if (string.IsNullOrEmpty(text)) return;

        driver?.SetAttribute(normalColor);

        // Use "Lines" to ensure a Format (don't use "lines"))

        var linesFormated = Lines;
        switch (textDirection)
        {
            case TextDirection.TopBottom_RightLeft:
            case TextDirection.LeftRight_BottomTop:
            case TextDirection.RightLeft_BottomTop:
            case TextDirection.BottomTop_RightLeft:
                linesFormated.Reverse();
                break;
        }

        for (var line = 0; line < linesFormated.Count; line++)
        {
            var isVertical = IsVerticalDirection(textDirection);

            if (isVertical && line > bounds.Width || !isVertical && line > bounds.Height)
                continue;

            var runes = lines[line].ToRuneList();

            switch (textDirection)
            {
                case TextDirection.RightLeft_BottomTop:
                case TextDirection.RightLeft_TopBottom:
                case TextDirection.BottomTop_LeftRight:
                case TextDirection.BottomTop_RightLeft:
                    runes.Reverse();
                    break;
            }

            // When text is justified, we lost left or right, so we use the direction to align. 

            int x, y;
            // Horizontal Alignment
            if (textAlignment == TextAlignment.Right ||
                textAlignment == TextAlignment.Justified && !IsLeftToRight(textDirection))
            {
                if (isVertical)
                {
                    x = bounds.Right - Lines.Count + line;
                    CursorPosition = bounds.Width - Lines.Count + hotKeyPos;
                }
                else
                {
                    x = bounds.Right - runes.Count;
                    CursorPosition = bounds.Width - runes.Count + hotKeyPos;
                }
            }
            else if (textAlignment == TextAlignment.Left || textAlignment == TextAlignment.Justified)
            {
                if (isVertical)
                    x = bounds.Left + line;
                else
                    x = bounds.Left;
                CursorPosition = hotKeyPos;
            }
            else if (textAlignment == TextAlignment.Centered)
            {
                if (isVertical)
                {
                    x = bounds.Left + line + (bounds.Width - Lines.Count) / 2;
                    CursorPosition = (bounds.Width - Lines.Count) / 2 + hotKeyPos;
                }
                else
                {
                    x = bounds.Left + (bounds.Width - runes.Count) / 2;
                    CursorPosition = (bounds.Width - runes.Count) / 2 + hotKeyPos;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            // Vertical Alignment
            if (textVerticalAlignment == VerticalTextAlignment.Bottom ||
                textVerticalAlignment == VerticalTextAlignment.Justified && !IsTopToBottom(textDirection))
            {
                if (isVertical)
                    y = bounds.Bottom - runes.Count;
                else
                    y = bounds.Bottom - Lines.Count + line;
            }
            else if (textVerticalAlignment == VerticalTextAlignment.Top ||
                     textVerticalAlignment == VerticalTextAlignment.Justified)
            {
                if (isVertical)
                    y = bounds.Top;
                else
                    y = bounds.Top + line;
            }
            else if (textVerticalAlignment == VerticalTextAlignment.Middle)
            {
                if (isVertical)
                {
                    var s = (bounds.Height - runes.Count) / 2;
                    y = bounds.Top + s;
                }
                else
                {
                    var s = (bounds.Height - Lines.Count) / 2;
                    y = bounds.Top + line + s;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            var start = isVertical ? bounds.Top : bounds.Left;
            var size = isVertical ? bounds.Height : bounds.Width;

            var current = start;
            for (var idx = start; idx < start + size; idx++)
            {
                if (idx < 0)
                {
                    current++;
                    continue;
                }

                var rune = (Rune)' ';
                if (isVertical)
                {
                    driver?.Move(x, current);
                    if (idx >= y && idx < y + runes.Count) rune = runes[idx - y];
                }
                else
                {
                    driver?.Move(current, y);
                    if (idx >= x && idx < x + runes.Count) rune = runes[idx - x];
                }

                if ((rune.Value & HotKeyTagMask) == HotKeyTagMask)
                {
                    if (isVertical && textVerticalAlignment == VerticalTextAlignment.Justified ||
                        !isVertical && textAlignment == TextAlignment.Justified)
                        CursorPosition = idx - start;
                    driver?.SetAttribute(hotColor);
                    driver?.AddRune((Rune)((uint)rune.Value & ~HotKeyTagMask));
                    driver?.SetAttribute(normalColor);
                }
                else
                {
                    driver?.AddRune(rune);
                }

                current += rune.ColumnWidth();
                if (idx + 1 < runes.Count && current + runes[idx + 1].ColumnWidth() > size) break;
            }
        }
    }
}
