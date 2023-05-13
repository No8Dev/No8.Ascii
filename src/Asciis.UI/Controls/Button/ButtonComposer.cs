using System.Drawing;
using Asciis.UI.Terminals;
using Asciis.UI.Helpers;
using static System.Math;

namespace Asciis.UI.Controls;

public class ButtonComposer : Composer<Button, ButtonLayout>
{
    public ButtonComposer(Button button) : base(button)
    {
    }

    public override void UpdateLayoutValues()
    {
        base.UpdateLayoutValues();

        var text = Control.Text ?? "";
        const int minTextLength = 3;

        // Make sure layout.Text is not too long
        var availableWidth = Layout.Width - Layout.Border.Left - Layout.Border.Right - Layout.Padding.Left - Layout.Padding.Right;

        if (availableWidth <= 0)
            Layout.Text = text;
        else
        {
            var max = (int)Clamp(text.Length, minTextLength, Max(minTextLength, availableWidth));
            Layout.Text = text.Length > max
                              ? $"{text.Substring(0, max - 2)}.."
                              : text;
        }
    }

    protected override (float maxWidth, float maxHeight) MeasureChildren(in RectF area)
    {
        if (Control.Children.Count > 0)
            return base.MeasureChildren(area);

        var height = 1;
        var width = Control.Text?.Length ?? 0;

        return (width, height);
    }

    public override void Draw(Canvas canvas, RectF? clip)
    {
        clip = (clip == null) ? Layout.Bounds : Rect.Intersect(clip, Layout.Bounds);
        canvas.PushClip(clip);

        var background = Layout.Background;
        var borderColor = Layout.BorderColor ?? Layout.Foreground;
        var foreground = Layout.Foreground;

        bool hasFocus = Control.HasFocus;
        var enabled = Control.IsEnabled;
        if (Control.IsMouseOver && enabled)
            background = background.AdjustBy(0.2f);

        if (!enabled)
            borderColor = foreground = Color.Gray.AdjustBy(0.2f);

        if (Layout.Border != Edges.Zero)
        {
            canvas.FillRect(Layout.Bounds, ' ', Layout.Foreground, background);
            canvas.DrawRect(Layout.Bounds,
                            hasFocus
                                ? LineSet.Double
                                : Layout.LineSet,
                            borderColor,
                            background);
        }

        base.Draw(canvas, clip);

        if (!string.IsNullOrWhiteSpace(Layout.Text))
        {
            canvas.DrawString(
                (int)(Layout.LayoutX + Layout.Border.Left + Layout.Padding.Left),
                (int)(Layout.LayoutY + Layout.Border.Top + Layout.Padding.Top),
                Layout.Text,
                foreground,
                background);
        }

        canvas.PopClip();
    }

    public override Vec FocusCursor =>
        new((int)(Layout.LayoutX + Layout.Border.Left + Layout.Padding.Left),
            (int)(Layout.LayoutY + Layout.Border.Top + Layout.Padding.Top));

    public override bool HandlePointerEvent(PointerEventArgs pointerEvent)
    {
        if (pointerEvent.PointerEventType == PointerEventType.Click &&
            pointerEvent.ButtonId == 0)
        {
            Control.RaiseClick();
            return true;
        }

        return base.HandlePointerEvent(pointerEvent);
    }

    public override bool HandleKeyEvent(AKeyEventArgs keyArgs)
    {
        if (!Control.HasFocus)
            return false;

        if (keyArgs.EventType == KeyEventType.Key)
        {
            switch (keyArgs.Key)
            {
                case VirtualKeyCode.VK_SPACE:
                case VirtualKeyCode.VK_RETURN:
                    Control.RaiseClick();

                    return true;
            }
        }

        return base.HandleKeyEvent(keyArgs);
    }
}
