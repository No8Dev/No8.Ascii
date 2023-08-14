using No8.Ascii.ElementLayout;

namespace No8.Ascii.Controls;

public class Row : Control
{
    internal static readonly Brush? DefaultBorderBrush = null;
    internal static readonly Edges DefaultBorder = Edges.Zero;
    internal static readonly LineSet DefaultLineSet = LineSet.Single;

    //**********************************************

    public Row() // RowLayoutPlan? plan = null, BorderStyle? style = null)
        : base(new RowLayoutPlan(new LayoutPlan()))
    {
    }

    public Row(out Control control) //, RowLayoutPlan? plan = null, BorderStyle? style = null)
        : this() //plan, style)
    {
        control = this;
    }

    //--

    public new RowLayoutPlan ControlPlan { get; } = new ();
    protected Brush? _borderBrush;
    protected LineSet? _lineSet;
    protected Edges? _border;

    public Edges Border
    {
        get => _border ?? Frame.DefaultBorder;
        set => ChangeDirtiesLayout(ref _border, value);
    }

    public Brush? BorderBrush
    {
        get => _borderBrush;
        set => ChangeDirtiesPainting(ref _borderBrush, value);
    }

    public LineSet LineSet
    {
        get => _lineSet ?? LineSet.Single;
        set => ChangeDirtiesPainting(ref _lineSet, value);
    }

    // -- 

    public override void OnDraw(Canvas canvas, RectF? clip)
    {
        var borderBrush = BorderBrush ?? ForegroundBrush ?? SolidColorBrush.White.Value;
        var bounds      = Layout.Bounds;
        clip = (clip == null) ? bounds : RectF.Intersect(clip.Value, bounds);
        canvas.PushClip(clip);

        Color? foreground = (borderBrush as SolidColorBrush)?.Color;
        Color? background = (BackgroundBrush as SolidColorBrush)?.Color;

        if (BackgroundBrush is not null)
            canvas.PaintBackground(bounds, BackgroundBrush);
        if (ForegroundBrush is not null)
            canvas.PaintForeground(bounds, ForegroundBrush);
        if (BorderBrush is not null)
            canvas.PaintBorderForeground(bounds, BorderBrush);

        var border = Border;

        if (border.Left > 0 &&
            border.Top > 0 &&
            border.Right > 0 &&
            border.Bottom > 0)
        {
            canvas.DrawRect(
                (int)bounds.Left,
                (int)bounds.Top,
                (int)bounds.Right - 1,
                (int)bounds.Bottom - 1,
                LineSet,
                foreground, background);
        }
        else
        {
            if (border.Bottom > 0)
                canvas.DrawLine(
                    (int)bounds.Left,
                    (int)bounds.Bottom - 1,
                    (int)bounds.Right - 1,
                    (int)bounds.Bottom - 1,
                    LineSet, foreground, background);
            if (border.Right > 0)
                canvas.DrawLine(
                    (int)bounds.Right - 1,
                    (int)bounds.Top,
                    (int)bounds.Right - 1,
                    (int)bounds.Bottom - 1,
                    LineSet, foreground, background);
            if (border.Left > 0)
                canvas.DrawLine(
                    (int)bounds.Left,
                    (int)bounds.Top,
                    (int)bounds.Left,
                    (int)bounds.Bottom - 1,
                    LineSet, foreground, background);
            if (border.Top > 0)
                canvas.DrawLine(
                    (int)bounds.Left,
                    (int)bounds.Top,
                    (int)bounds.Right - 1,
                    (int)bounds.Top,
                    LineSet, foreground, background);
        }

        base.OnDraw(canvas, clip);

        canvas.PopClip();
    }

    // --
}