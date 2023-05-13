using System.Drawing;
using System.Text;

namespace Asciis.UI.Controls;

public class OptionboxLayout : Layout<Optionbox>
{
    public string Text { get; set; } = "";
    public Edges Border { get; set; } = Edges.Zero;
    public Color? BorderColor { get; set; }
    public LineSet LineSet { get; set; }

    public OptionboxLayout(Optionbox optionbox) : base(optionbox)
    {
    }

    public override void UpdateValues(Optionbox optionbox)
    {
        base.UpdateValues(optionbox);

        Border = optionbox.Border ?? Edges.Zero;
        BorderColor = optionbox.BorderColor;
        LineSet = optionbox.LineSet ?? LineSet.Single;
        // `Text` is updated in the Composer
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
