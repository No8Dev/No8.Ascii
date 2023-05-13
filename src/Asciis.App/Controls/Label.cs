namespace Asciis.App.Controls;

public class Label : Control, IHasStyle<LabelStyle>, IHasLayoutPlan<LabelLayoutPlan>
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

    public Label(LayoutPlan? plan = null, LabelStyle? style = null)
        : base(plan ?? new LabelLayoutPlan(), style)
    {
    }

    public Label(out Label label, LayoutPlan? plan = null, LabelStyle? style = null)
        : this(plan, style)
    {
        label = this;
    }

    //--

    public new LabelLayoutPlan Plan  => (LabelLayoutPlan)_plan!;
    public new LabelStyle      Style => (LabelStyle)_style!;

    public void Add(string text) => Text = text;

    //--

    public override void OnDraw(Canvas canvas, RectF? clip)
    {
        var bounds = Layout.Bounds;
        clip = (clip == null) ? bounds : RectF.Intersect(clip, bounds);
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

public class LabelLayoutPlan : LayoutPlan
{
    public LabelLayoutPlan()
    {
        IsText    = true;
        MinHeight = 1;
        MinWidth  = 1;
    }
}

public class LabelStyle : Style
{

}

