using System.Text;

namespace Asciis.UI.Controls;

public class LabelLayout : Layout<Label>
{
    public string? Text { get; set; }

    public Wrap? TextWrap { get; set; }

    public LabelLayout(Label label) : base(label)
    {
    }

    public override void UpdateValues(Label label)
    {
        base.UpdateValues(label);

        Text = label.Text;
        TextWrap = label.TextWrap;
    }

    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        if (Text is not null) sb.AppendLine($" Text={Text}");
        if (TextWrap is not null) sb.AppendLine($" TextWrap={TextWrap}");
    }
}
