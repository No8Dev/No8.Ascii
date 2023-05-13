using System;

using Asciis.UI.Helpers;

namespace Asciis.UI.Controls;

public class LabelComposer : Composer<Label, LabelLayout>
{
    public LabelComposer(Label label) : base(label)
    {
    }

    protected override (float maxWidth, float maxHeight) MeasureChildren(in RectF area)
    {
        if (Control.Children.Count > 0)
            return base.MeasureChildren(in area);

        var textWidth = Control.Text?.Length ?? 0f;
        var textHeight = 1f;
        var text = Layout.Text ?? "";

        if (Layout.TextWrap == Wrap.WordWrap)
        {
            if (textWidth > area.Width)
            {
                var lines = text.WrapText((int)area.Width);
                if (lines.Count > 1)
                    textHeight = Math.Min(area.Height, lines.Count);
                textWidth = area.Width;
            }
        }
        else
        {
            if (textWidth > area.Width)
            {
                textWidth = area.Width;
                if (Layout.TextWrap == Wrap.Truncate)
                    Layout.Text = text.TruncateWithEllipses((int)area.Width);
                else if (Layout.TextWrap == Wrap.TruncateWithWord)
                    Layout.Text = text.TruncateWithEllipses((int)area.Width, true);
                else
                {
                    switch (Control.HorzPosition)
                    {
                        case LayoutPosition.Start:
                            Layout.Text = text.Substring(0, (int)area.Width);

                            break;
                        case LayoutPosition.Center:
                            Layout.Text = text.Substring((int)((text.Length / 2f) - (area.Width / 2f)), (int)area.Width);

                            break;

                        case LayoutPosition.End:
                            Layout.Text = text.Substring((int)(text.Length - area.Width));

                            break;

                        case null:
                            break;
                    }
                }
            }
        }

        return (textWidth, textHeight);
    }

    public override void Draw(Canvas canvas, RectF? clip)
    {
        clip = (clip == null) ? Layout.Bounds : RectF.Intersect(clip, Layout.Bounds);
        canvas.PushClip(clip);

        base.Draw(canvas, clip);

        var text = Layout.Text;
        if (!string.IsNullOrWhiteSpace(text))
        {
            if (Layout.TextWrap == Wrap.WordWrap)
            {
                var lines = text.WrapText((int)Layout.Width);
                var numLines = Math.Min(Layout.Height, lines.Count);
                for (int i = 0; i < numLines; i++)
                {
                    canvas.DrawString((int)Layout.LayoutX,
                                      (int)Layout.LayoutY + i,
                                      lines[i],
                                      Layout.Foreground,
                                      Layout.Background);
                }
            }
            else
            {
                if (Layout.TextWrap == Wrap.Truncate)
                    text = text.TruncateWithEllipses((int)Layout.Width);
                if (Layout.TextWrap == Wrap.TruncateWithWord)
                    text = text.TruncateWithEllipses((int)Layout.Width, true);

                canvas.DrawString((int)Layout.LayoutX,
                                  (int)Layout.LayoutY,
                                  text,
                                  Layout.Foreground,
                                  Layout.Background);
            }
        }

        canvas.PopClip();
    }

}
