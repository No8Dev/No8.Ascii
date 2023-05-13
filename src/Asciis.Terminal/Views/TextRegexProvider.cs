using System.Text.RegularExpressions;
using Asciis.Terminal.Helpers;

namespace Asciis.Terminal.Views.TextValidateProviders;

/// <summary>
/// Regex Provider for TextValidateField.
/// </summary>
public class TextRegexProvider : ITextValidateProvider
{
    private Regex regex;
    private List<Rune> text;
    private List<Rune> pattern;

    /// <summary>
    /// Empty Constructor.
    /// </summary>
    public TextRegexProvider(string pattern) { Pattern = pattern; }

    /// <summary>
    /// Regex pattern property.
    /// </summary>
    public string Pattern
    {
        get => pattern.MakeString();
        set
        {
            pattern = value.ToRuneList();
            CompileMask();
            SetupText();
        }
    }

    ///<inheritdoc/>
    public string Text
    {
        get => text.MakeString();
        set
        {
            text = value != string.Empty ? value.ToRuneList() : null;
            SetupText();
        }
    }

    ///<inheritdoc/>
    public string DisplayText => Text;

    ///<inheritdoc/>
    public bool IsValid => Validate(text);

    ///<inheritdoc/>
    public bool Fixed => false;

    /// <summary>
    /// When true, validates with the regex pattern on each input, preventing the input if it's not valid.
    /// </summary>
    public bool ValidateOnInput { get; set; } = true;


    private bool Validate(List<Rune> text)
    {
        var match = regex.Match(text.MakeString());
        return match.Success;
    }

    ///<inheritdoc/>
    public int Cursor(int pos)
    {
        if (pos < 0)
            return CursorStart();
        else if (pos >= text.Count)
            return CursorEnd();
        else
            return pos;
    }

    ///<inheritdoc/>
    public int CursorStart() { return 0; }

    ///<inheritdoc/>
    public int CursorEnd() { return text.Count; }

    ///<inheritdoc/>
    public int CursorLeft(int pos)
    {
        if (pos > 0) return pos - 1;
        return pos;
    }

    ///<inheritdoc/>
    public int CursorRight(int pos)
    {
        if (pos < text.Count) return pos + 1;
        return pos;
    }

    ///<inheritdoc/>
    public bool Delete(int pos)
    {
        if (text.Count > 0 && pos < text.Count) text.RemoveAt(pos);
        return true;
    }

    ///<inheritdoc/>
    public bool InsertAt(char ch, int pos)
    {
        var aux = text.ToList();
        aux.Insert(pos, (Rune)ch);
        if (Validate(aux) || ValidateOnInput == false)
        {
            text.Insert(pos, (Rune)ch);
            return true;
        }

        return false;
    }

    private void SetupText()
    {
        if (text != null && IsValid) return;

        text = new List<Rune>();
    }

    /// <summary>
    /// Compiles the regex pattern for validation./>
    /// </summary>
    private void CompileMask() { regex = new Regex(pattern.MakeString(), RegexOptions.Compiled); }
}
