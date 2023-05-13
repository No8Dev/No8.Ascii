using System.Drawing;
using System.Text;

namespace Asciis.UI.Controls;
public class ButtonLayout : Layout<Button>
{
    public string? Text { get; set; }
    public Edges Border { get; set; } = Edges.Zero;
    public Color? BorderColor { get; set; }
    public LineSet LineSet { get; set; }

    public ButtonLayout(Button button) : base(button)
    {
    }

    public override void UpdateValues(Button button)
    {
        base.UpdateValues(button);

        Text = button.Text;
        Border = button.Border ?? Edges.Zero;
        BorderColor = button.BorderColor;
        LineSet = button.LineSet ?? LineSet.Single;
    }

    public override Edges Edges =>
        Padding + Border;

    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        if (Text != null) sb.Append($" Text={Text}");
        if (Border.IsNotZero)
        {
            sb.Append($" Border={Border}({BorderColor ?? Foreground})");
            sb.Append($" LineSet={LineSet}");
        }
    }
}
