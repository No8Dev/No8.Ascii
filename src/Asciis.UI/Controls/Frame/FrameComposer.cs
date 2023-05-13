namespace Asciis.UI.Controls;

public class FrameComposer : Composer<Frame, FrameLayout>
{
    public FrameComposer(Frame frame) : base(frame)
    {
    }

    public override void Draw(Canvas canvas, RectF? clip)
    {
        clip = (clip == null) ? Layout.Bounds : Rect.Intersect(clip, Layout.Bounds);
        canvas.PushClip(clip);

        if (Layout.Border.Left > 0 &&
            Layout.Border.Top > 0 &&
            Layout.Border.Right > 0 &&
            Layout.Border.Bottom > 0)
        {
            canvas.DrawRect(
                (int)Layout.LayoutX, 
                (int)Layout.LayoutY,
                (int)(Layout.LayoutX + Layout.Width - 1f), 
                (int)(Layout.LayoutY + Layout.Height - 1f),
                Layout.LineSet, Layout.BorderColor ?? Layout.Foreground, Layout.Background);
        }
        else
        {
            if (Layout.Border.Bottom > 0)
                canvas.DrawLine(
                    (int)(Layout.LayoutX),
                    (int)(Layout.LayoutY + Layout.Height - 1),
                    (int)(Layout.LayoutX + Layout.Width - 1),
                    (int)(Layout.LayoutY + Layout.Height - 1),
                    Layout.LineSet,
                    Layout.BorderColor ?? Layout.Foreground,
                    Layout.Background);
            if (Layout.Border.Right > 0)
                canvas.DrawLine(
                    (int)(Layout.LayoutX + Layout.Width - 1), 
                    (int)(Layout.LayoutY),
                    (int)(Layout.LayoutX + Layout.Width - 1), 
                    (int)(Layout.LayoutY + Layout.Height - 1),
                    Layout.LineSet, Layout.BorderColor ?? Layout.Foreground, Layout.Background);
            if (Layout.Border.Left > 0)
                canvas.DrawLine(
                    (int)(Layout.LayoutX), 
                    (int)(Layout.LayoutY),
                    (int)(Layout.LayoutX), 
                    (int)(Layout.LayoutY + Layout.Height - 1),
                    Layout.LineSet, Layout.BorderColor ?? Layout.Foreground, Layout.Background);
            if (Layout.Border.Top > 0)
                canvas.DrawLine(
                    (int)(Layout.LayoutX), 
                    (int)(Layout.LayoutY),
                    (int)(Layout.LayoutX + Layout.Width - 1), 
                    (int)(Layout.LayoutY),
                    Layout.LineSet, Layout.BorderColor ?? Layout.Foreground, Layout.Background);
        }
        if (!string.IsNullOrWhiteSpace(Layout.Text))
        {
            string str;
            var max = System.Math.Clamp(Layout.Text.Length, 3, Layout.Width - Layout.Border.Left - Layout.Border.Right - 4);
            if (Layout.Text.Length > max)
                str = $"[ {Layout.Text.Substring(0, (int)max - 2)}.. ]";
            else
                str = $"[ {Layout.Text} ]";
            canvas.DrawString(
                (int)(Layout.LayoutX + Layout.Border.Left + 1),
                (int)Layout.LayoutY,
                str,
                Layout.Foreground,
                Layout.Background);
        }

        base.Draw(canvas, clip);

        canvas.PopClip();
    }
}
