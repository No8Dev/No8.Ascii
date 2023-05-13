using System.Text;

namespace Asciis.UI.Controls;

public class TextEditLayout : Layout<TextEdit>
{
    public string Text { get; set; } = "";

    public TextEditLayout(TextEdit textEdit) : base(textEdit)
    {
    }

    public override void UpdateValues(TextEdit control)
    {
        base.UpdateValues(control);

        Text = control.Text ?? "";
    }

    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        sb.Append($" Text={Text}");
    }
}
