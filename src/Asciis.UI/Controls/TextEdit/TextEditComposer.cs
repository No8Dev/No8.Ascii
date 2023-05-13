using System.Diagnostics.CodeAnalysis;
using Asciis.UI.Terminals;

namespace Asciis.UI.Controls;

public class TextEditComposer : Composer<TextEdit, TextEditLayout>
{

    public TextEditComposer([NotNull] TextEdit control) : base(control)
    {
    }

    protected override (float maxWidth, float maxHeight) MeasureChildren(in RectF area)
    {
        if (Control.Children.Count > 0)
            return base.MeasureChildren(in area);

        var textWidth = Control.Text?.Length ?? 0f;
        var textHeight = 1f;
        var text = Layout.Text ?? "";

        if (textWidth > area.Width && area.Width > 0)
        {
            textWidth = area.Width;
            Layout.Text = text.Substring(0, (int)area.Width);
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
            canvas.DrawString((int)Layout.LayoutX,
                              (int)Layout.LayoutY,
                              text,
                              Layout.Foreground,
                              Layout.Background);
        }

        canvas.PopClip();
    }

    public override bool HandlePointerEvent(PointerEventArgs pointerEvent)
    {
        if (pointerEvent.PointerEventType == PointerEventType.Pressed)
        {
            if (!Control.IsEnabled)
                return false;
            if (!Control.CanFocus)
                return false;
            if (!Control.HasFocus)
                SetFocus();

            ClearSelection();
            _cursorOffsetX = pointerEvent.X - (int)(Layout.LayoutX + Layout.Edges.Left);
            CursorUpdated();
        }

        //
        return base.HandlePointerEvent(pointerEvent);
    }

    private void CursorUpdated() =>
        Control.Host?.FocusManager.CursorUpdated();

    private int _cursorOffsetX;
    private int _selectedLength = 0;

    public override Vec FocusCursor =>
        base.FocusCursor.Offset(_cursorOffsetX, 0);

    private void ClearSelection()
    {
        Control.SetNeedsPainting();
    }

    public override bool HandleKeyEvent(AKeyEventArgs keyArgs)
    {
        if (!Control.IsEnabled)
            return true;
        if (keyArgs.EventType != KeyEventType.Released)
            return base.HandleKeyEvent(keyArgs);

        var text = Layout.Text;
        switch (keyArgs.Key)
        {
            case VirtualKeyCode.VK_DELETE:
                if (_selectedLength > 0)
                {
                    // Deleted selected
                }
                else
                {
                    // nothing to delete
                    if (text.Length == 0 || _cursorOffsetX >= text.Length)
                        return true;

                    Control.Text = text.Remove(_cursorOffsetX, 1);
                    CursorUpdated();
                }
                return true;

            case VirtualKeyCode.VK_BACK:
                if (_selectedLength > 0)
                {
                    // Deleted selected
                }
                else
                {
                    // nothing to delete
                    if (text.Length == 0 || _cursorOffsetX <= 0)
                        return true;

                    _cursorOffsetX--;
                    Control.Text = text.Remove(_cursorOffsetX, 1);
                    CursorUpdated();
                }
                return true;
            case VirtualKeyCode.VK_HOME:
                _cursorOffsetX = 0;
                CursorUpdated();

                return true;
            case VirtualKeyCode.VK_END:
                _cursorOffsetX = text.Length;
                CursorUpdated();

                return true;

            case VirtualKeyCode.VK_LEFT:
                if (_cursorOffsetX > 0)
                {
                    _cursorOffsetX--;
                    CursorUpdated();
                }

                return true;
            case VirtualKeyCode.VK_RIGHT:
                if (_cursorOffsetX < text.Length)
                {
                    _cursorOffsetX++;
                    CursorUpdated();
                }

                return true;

            // case Shift Home:
            //  Select 0 - _cursorOffsetX;
            // case Shift End:
            //  Select _cursorOffsetX - Text.Length;
            // case Ctrl-Shift-Left:
            // case Ctrl-Shift-Right:
            //  Select word
            // case Ctrl-Left:
            // case Ctrl-Right:
            //  Move cursor word
            // case Undo  (Ctrl-Z)
            // case Redo  (Ctrl-Y)
            // case Cut   (Ctrl-X)
            // case Paste (Ctrl-X)
            // case Copy  (Ctrl-C)
            default:
                if (keyArgs.KeyChar < ' ' || char.IsControl(keyArgs.KeyChar))
                    return false;

                if (_selectedLength > 0)
                {
                    // remove selected text
                }
                Control.Text = text.Insert(_cursorOffsetX, $"{keyArgs.KeyChar}");
                _cursorOffsetX++;
                CursorUpdated();

                return true;
        }

        //return base.HandleKeyEvent(keyArgs);
    }
}
