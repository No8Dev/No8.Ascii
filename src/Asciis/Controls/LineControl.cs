namespace No8.Ascii.Controls;

public class LineControl : Control
{
    //**********************************************

    public LineControl(LayoutPlan? plan = null)
        : base(plan)
    {
    }

    public LineControl(
        out Control control,
        LayoutPlan? plan = null)
        : this(plan)
    {
        control = this;
    }

    //--

    public LineSet LineSet
    {
        get => _lineSet;
        set => ChangeDirtiesPainting(ref _lineSet, value);
    }

    private LineSet _lineSet = LineSet.Single;

    //--

    public override void OnDraw(Canvas canvas, RectF? clip)
    {
        Color? foreground = (ForegroundBrush as SolidColorBrush)?.Color;
        Color? background = (BackgroundBrush as SolidColorBrush)?.Color;

        base.OnDraw(canvas, clip);

        var bounds = Layout.Bounds;

        if (BackgroundBrush is not null)
            canvas.PaintBackground(bounds, BackgroundBrush);
        if (ForegroundBrush is not null)
            canvas.PaintForeground(bounds, ForegroundBrush);

        canvas.DrawLine(
            bounds, 
            LineSet, 
            foreground, 
            background);
    }
}