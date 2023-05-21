using No8.Ascii.ElementLayout;

namespace No8.Ascii.Controls;

public class Row : Control, IHasStyle<RowStyle>, IHasLayoutPlan<RowLayoutPlan>
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
    public new RowStyle Style => (RowStyle)_style!;

    public Brush? BorderBrush
    {
        get => Style.BorderBrush;
        set => Style.BorderBrush = value;
    }

    public LineSet LineSet
    {
        get => Style.LineSet ?? LineSet.Single;
        set => Style.LineSet = value;
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

        var border = Style.Border;

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

public class RowLayoutPlan : ControlPlan
{
    public ControlAlign HorzAlign
    {
        get => ChildrenHorzAlign;
        set => ChildrenHorzAlign = value;
    }
    
    public ControlAlign VertAlign
    {
        get => ChildrenVertAlign;
        set => ChildrenVertAlign = value;
    }
    
    public LayoutWrap ElementsWrap
    {
        get => LayoutPlan.ElementsWrap;
        set => LayoutPlan.ElementsWrap = value;
    }

    public float FlexShrink
    {
        get => LayoutPlan.FlexShrink;
        set => LayoutPlan.FlexShrink = value;
    }

    public float FlexGrow
    {
        get => LayoutPlan.FlexGrow;
        set => LayoutPlan.FlexGrow = value;
    }

    public RowLayoutPlan(LayoutPlan? plan = null) : base(plan)
    {
        // Default values for Row
        //LayoutPlan.Padding = 1;
        LayoutPlan.ElementsDirection = LayoutDirection.Horz;
        HorzAlign = ControlAlign.Stretch;
        VertAlign = ControlAlign.Start;
        //LayoutPlan.Width = 100.Percent();
        //LayoutPlan.Height = 100.Percent();

    }

}