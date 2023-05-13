using System;
using System.Text;

namespace Asciis.UI.Controls;

public class TextEdit : Control
{
    static TextEdit()
    {
        Composer.Register(false, typeof(TextEdit), typeof(TextEditComposer));
    }

    private string? _text;

    public string? Text
    {
        get => _text;
        set => ChangeDirtiesLayout(ref _text, value);
    }

    public TextEdit(string? name = null) : base(name)
    {
        CanFocus = true;
        OnPropertyChanged<bool>(
            nameof(HasFocus),
            (old, value) =>
            {
                Console.CursorVisible = HasFocus;
            });
    }

    public TextEdit(out TextEdit textEdit, string? name = null) : this(name)
    {
        textEdit = this;
    }

    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        if (Text != null) sb.Append(" Text=" + Text);
    }

    public override string ShortString =>
        base.ShortString + (_text ?? "");
}
