using System.Text;

namespace Asciis.UI.Controls;

public class Label : Control
{
    static Label()
    {
        Composer.Register(false, typeof(Label), typeof(LabelComposer));
    }

    private string? _text;
    private Wrap? _wrap;

    public string? Text
    {
        get => _text;
        set => ChangeDirtiesLayout(ref _text, value);
    }

    public Wrap? TextWrap
    {
        get => _wrap;
        set => ChangeDirtiesLayout(ref _wrap, value);
    }

    public Label(string? name = null) : base(name) { }

    public Label(out Label label, string? name = null) : this(name)
    {
        label = this;
    }

    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        if (Text != null) sb.Append(" Text=" + Text);
        if (TextWrap != null && TextWrap != Wrap.None)
            sb.Append(" TextWrap=" + TextWrap);
    }

    public override string ShortString =>
        base.ShortString + (_text ?? "");

}
