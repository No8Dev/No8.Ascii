using System.Drawing;
using Asciis.UI.Terminals;
using Asciis.UI.Helpers;
using static System.Math;

namespace Asciis.UI.Controls;

public class CheckboxComposer : Composer<Checkbox, CheckboxLayout>
{
    public CheckboxComposer(Checkbox checkbox) : base(checkbox)
    {
    }

    public override void UpdateLayoutValues()
    {
        base.UpdateLayoutValues();

        var checkedString = Control.CheckedString ?? "[X]";
        var uncheckedString = Control.UncheckedString ?? "[ ]";
        var unknownString = Control.UnknownString ?? "[-]";

        var checkedStr = Control.Checked == null ? unknownString :
                         Control.Checked == true ? checkedString : uncheckedString;
        var text = checkedStr + (Control.Text ?? "");

        // Make sure layout.Text is not too long
        var availableWidth = Layout.Width - Layout.Border.Left - Layout.Border.Right - Layout.Padding.Left - Layout.Padding.Right;

        if (availableWidth <= 0)
            Layout.Text = text;
        else
        {
            var minTextLen = Max(checkedString.Length,
                                 Max(uncheckedString.Length,
                                     unknownString.Length));
            var maxTextLen = Clamp(text.Length, minTextLen, Max(minTextLen, availableWidth));

            Layout.Text = text.Length > maxTextLen
                       ? $"{text.Substring(0, (int)maxTextLen - 2)}.."
                       : text;
        }
    }

    protected override (float maxWidth, float maxHeight) MeasureChildren(in RectF area)
    {
        var height = 1;
        var width = Layout.Text.Length;

        return (width, height);
    }

    public override void Draw(Canvas canvas, RectF? clip)
    {
        clip = (clip == null) ? Layout.Bounds : RectF.Intersect(clip, Layout.Bounds);
        canvas.PushClip(clip);

        var background = Layout.Background;
        var borderColor = Layout.BorderColor ?? Layout.Foreground;
        var foreground = Layout.Foreground;

        if (Control.IsMouseOver && Control.IsEnabled)
            background = background.AdjustBy(0.2f);

        if (!Control.IsEnabled)
            borderColor = foreground = Color.Gray.AdjustBy(0.2f);

        if (Layout.Border != Edges.Zero)
        {
            canvas.FillRect(Layout.Bounds, ' ', Layout.Foreground, background);
            canvas.DrawRect(Layout.Bounds,
                            Control.HasFocus
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
        new((int)(Layout.LayoutX + Layout.Border.Left + Layout.Padding.Left + 1),
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
