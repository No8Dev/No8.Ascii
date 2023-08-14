using No8.Ascii.ElementLayout;

namespace No8.Ascii.Controls;

public class Label : Control
{
    public string? Text
    {
        get => _text;
        set
        {
            ChangeDirtiesLayout(ref _text, value);
            if (Layout.HasLayout && _text != null)
            {
                if (Bounds.Width > 0)
                {
                    _lines = _text?.SplitIntoLines((int)Bounds.Width);
                    Plan.Height = _lines?.Count ?? 1;
                }
            }
        }
    }

    //**********************************************

    public Label(ControlPlan? plan = null, LabelStyle? style = null)
        : base(plan ?? new LabelLayoutPlan(new LayoutPlan()), style)
    {
    }

    public Label(out Label label, ControlPlan? plan = null, LabelStyle? style = null)
        : this(plan, style)
    {
        label = this;
    }

    //--

    public new LabelLayoutPlan ControlPlan  => (LabelLayoutPlan)_controlPlan!;

    public void Add(string text) => Text = text;

    //--

    public override void OnDraw(Canvas canvas, RectF? clip)
    {
        var bounds = Layout.Bounds;
        clip = (clip == null) ? bounds : RectF.Intersect(clip.Value, bounds);
        canvas.PushClip(clip);

        base.OnDraw(canvas, bounds);

        if (!string.IsNullOrWhiteSpace(Text))
        {
            var max = Math.Clamp(Text.Length, 3, (int)Layout.Width);
            if (Text.Length <= max)
            {
                canvas.DrawString(
                    (int)bounds.Left,
                    (int)bounds.Top,
                    Text,
                    ForegroundBrush,
                    BackgroundBrush);
            }
            else if (_lines != null)
            {
                for (int y = 0; y < _lines.Count && y < Bounds.Height; y++)
                {
                    canvas.DrawString(
                        (int)bounds.Left,
                        (int)bounds.Top + y,
                        _lines[y],
                        ForegroundBrush,
                        BackgroundBrush);
                }
            }
        }

        canvas.PopClip();
    }

    //--
    private string? _text;
    private List<string>? _lines;
}