using System.Drawing;
using System.Text;

namespace Asciis.UI.Controls;

public class FrameLayout : Layout<Frame>
{
    public string? Text { get; set; }
    public Color? BorderColor { get; set; }
    public Edges Border { get; set; } = Edges.Zero;
    public LineSet LineSet { get; set; }

    public FrameLayout(Frame frame) : base(frame)
    {
    }

    public override void UpdateValues(Frame frame)
    {
        base.UpdateValues(frame);

        Text = frame.Text;
        Border = frame.Border ?? Edges.Zero;
        BorderColor = frame.BorderColor;
        LineSet = frame.LineSet ?? LineSet.Single;
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
