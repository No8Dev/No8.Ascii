using System.Drawing;
using System.Text;

namespace Asciis.UI.Controls;

public class CheckboxLayout : Layout<Checkbox>
{
    public string Text { get; set; } = "";
    public Edges Border { get; set; } = Edges.Zero;
    public Color? BorderColor { get; set; }
    public LineSet LineSet { get; set; }

    public CheckboxLayout(Checkbox checkbox) : base(checkbox)
    {
    }

    public override void UpdateValues(Checkbox checkbox)
    {
        base.UpdateValues(checkbox);

        Border = checkbox.Border ?? Edges.Zero;
        BorderColor = checkbox.BorderColor;
        LineSet = checkbox.LineSet ?? LineSet.Single;
        // `Text` is updated in the composer
    }

    public override Edges Edges =>
        Padding + Border;

    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        sb.Append($" Text={Text}");
        if (Border.IsNotZero)
        {
            sb.Append($" Border={Border}({BorderColor ?? Foreground})");
            sb.Append($" LineSet={LineSet}");
        }
    }
}
