namespace No8.Ascii;

public static class Helpers
{
    public static void Swap<T>(ref T a, ref T b)
    {
        (a, b) = (b, a);
    }

    public static List<string> WrapText(this string text, int maxLineLength)
    {
        List<string> result = new();

        if (string.IsNullOrWhiteSpace(text) || maxLineLength <= 0)
            return result;

        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        foreach (var one in lines)
        {
            var line = one.TrimEnd();

            if (line.Length < maxLineLength)
            {
                result.Add(line);
                continue;
            }

            int currentIndex;
            var lastWrap = 0;
            var whitespace = new[] { ' ', '\t' };
            do
            {
                currentIndex = lastWrap + maxLineLength >= line.Length
                                   ? line.Length
                                   : line.LastIndexOfAny(new[] { ' ', '\n', '\r' },
                                                          Math.Min(line.Length - 1, lastWrap + maxLineLength)) + 1;
                if (currentIndex <= lastWrap)
                    currentIndex = Math.Min(lastWrap + maxLineLength, line.Length);
                result.Add(line.Substring(lastWrap, currentIndex - lastWrap).Trim(whitespace));
                lastWrap = currentIndex;
            } while (currentIndex < line.Length);
        }

        // remove any trailing empty lines
        while (result.Count > 0)
        {
            if (result[^1].Length > 0)
                break;
            result.RemoveAt(result.Count - 1);
        }
        return result;
    }

    public static string TruncateWithEllipses(this string text, int maxLineLength, bool atWord = false)
    {
        if (maxLineLength < 3)
            return text;

        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        if (text.Length <= maxLineLength)
            return text;

        if (atWord)
        {
            int lastSpace = text.LastIndexOf(" ", maxLineLength - 2, StringComparison.Ordinal);

            if (lastSpace > 0)
                return text.Substring(0, lastSpace) + "..";
        }

        return text.Substring(0, maxLineLength - 2) + "..";
    }
}
