using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace No8.Ascii;

public static class StringHelpers
{
    public static bool IsNullOrWhiteSpace(this IEnumerable<char>? value)
    {
        if (value == null) return true;

        foreach (var c in value)
            if (!char.IsWhiteSpace(c))
                return false;

        return true;
    }

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value) { return string.IsNullOrWhiteSpace(value); }

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value) { return string.IsNullOrEmpty(value); }

    public static bool HasValue([NotNullWhen(true)] this string? value) { return !string.IsNullOrWhiteSpace(value); }

    /// <summary>
    /// Parse a string into multiple arguments (similar to command-line args[]).
    /// Can be a multi-line string
    /// </summary>
    public static List<string>? ParseArguments(this string lines)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (lines == null)
            return default;

        var list = new List<string>();
        var span = lines.Trim().AsSpan();

        var start = -1;
        for (var i = 0; i < span.Length; i++)
        {
            var ch = span[i];

            if (ch == '"' || ch == '\'')
            {
                if (start >= 0)
                    AddArg(span, ref start, i);

                var end = ch;
                start = i + 1;

                for (i++; i < span.Length; i++)
                {
                    ch = span[i];

                    if (ch == end)
                        break;
                }

                AddArg(span, ref start, i);
            }
            // Any word separating character
            else if (char.IsWhiteSpace(ch) || ch == '\r' || ch == '\n' || char.IsSeparator(ch))
            {
                if (start >= 0)
                    AddArg(span, ref start, i);
            }
            // everything else is part of the word
            else
            {
                if (start < 0)
                    start = i;
            }
        }

        if (start >= 0)
            AddArg(span, ref start, span.Length);

        return list;

        void AddArg(ReadOnlySpan<char> spanReadOnly, ref int argStart, int end)
        {
            var arg = spanReadOnly.Slice(argStart, end - argStart).ToString().Trim();
            if (arg.HasValue())
                list.Add(arg);
            argStart = -1;
        }
    }

    public static IEnumerable<string> EnumerateLines(this TextReader reader)
    {
        string? line;

        while ((line = reader.ReadLine()) != null)
            yield return line;
    }

    public static IEnumerable<string> EnumerateLines(this string str)
    {
        using var reader = new StringReader(str);
        return EnumerateLines(reader);
    }

    public static char TryGet(this StringBuilder sb, int index)
    {
        return sb.Length > index
            ? sb[index]
            : '\0';
    }

    public static char TryGet(this string str, int index)
    {
        return str.Length > index
            ? str[index]
            : '\0';
    }

    public static bool ContainsWithComparison(this IEnumerable<string> list, string value, StringComparison comparisonType = StringComparison.Ordinal)
    {
        return list.Any(str => str.Equals(value, comparisonType));
    }

    public static bool ContainsWithComparison(this IDictionary dict, string value, StringComparison comparisonType = StringComparison.Ordinal)
    {
        return dict.Keys
                   .Cast<string>()
                   .Any(str => str.Equals(value, comparisonType));
    }

    public static StringBuilder AppendWithCRLF(this StringBuilder sb, string? value)
    {
        if (value != null)
            sb.Append(value);
        return sb.Append("\r\n");
    }

    public static StringBuilder AppendCRLF(this StringBuilder sb) => 
        sb.Append("\r\n");

    public static int FindNull(this Span<char> data, int start)
    {
        for (var i = start; i < data.Length; i++)
        {
            if (data[i] == 0)
                return i;
        }

        return -1;
    }

    // public static int FindNull<T>(this Span<T> data, int start) where T : struct
    // {
    //     for (var i = start; i < data.Length; i++)
    //     {
    //         if (data[i].Equals(default))
    //             return i;
    //     }
    //
    //     return -1;
    // }
    
    public static string ReadNullTerminatedString(this string source, int offset = 0)
    {
        var sb = new StringBuilder();
        for (var pos = offset; pos < source.Length; pos++)
        {
            if (source[pos] == '\0')
                break;

            sb.Append(source[pos]);
        }

        return sb.ToString();
    }
}
