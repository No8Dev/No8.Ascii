using No8.Ascii.ElementLayout;

namespace No8.Ascii.Controls;

public class Frame : Control, IHasStyle<FrameStyle>, IHasLayoutPlan<FrameLayoutPlan>
{
    internal static readonly Brush? DefaultBorderBrush = null;
    internal static readonly Edges DefaultBorder = Edges.One;
    internal static readonly LineSet DefaultLineSet = LineSet.Single;

    //**********************************************

    public Frame(FrameLayoutPlan? plan = null, FrameStyle? style = null)
        : base(plan ?? new FrameLayoutPlan(new LayoutPlan()), style)
    {
    }

    public Frame(out Control control, FrameLayoutPlan? plan = null, FrameStyle? style = null)
        : this(plan, style)
    {
        control = this;
    }

    //--

    public new FrameLayoutPlan ControlPlan { get; } = new ();
    public new FrameStyle Style => (FrameStyle)_style!;

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

    public string? Text
    {
        get => _text; 
        set => ChangeDirtiesLayout(ref _text, value);
    }


    // -- 

    public void Add(string text) => Text = text;

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
        if (!string.IsNullOrWhiteSpace(Text))
        {
            var max = System.Math.Clamp(Text.Length, 3, Layout.Width - border.Left - border.Right - 4);

            var str = Text.Length > max ? $"[ {Text.Substring(0, (int)max - 2)}.. ]" : $"[ {Text} ]";
            canvas.DrawString(
                (int)(bounds.Left + border.Left + 1),
                (int)bounds.Top,
                str,
                foreground,
                background);
        }

        base.OnDraw(canvas, clip);

        canvas.PopClip();
    }

    // --

    private string?    _text;
}

public class FrameLayoutPlan : ControlPlan
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
    

    public FrameLayoutPlan(LayoutPlan? plan = null) : base(plan)
    {
        // Default values for Frame
        LayoutPlan.Padding = 1;
        LayoutPlan.Width = 100.Percent();
        LayoutPlan.Height = 100.Percent();
    }
}

public class FrameStyle : Style
{
    public Edges Border
    {
        get => Get<Edges?>(nameof(Border)) ?? Frame.DefaultBorder;
        set => Set(nameof(Border), value);
    }

    public Brush? BorderBrush
    {
        get => Get<Brush?>(nameof(BorderBrush)) ?? Frame.DefaultBorderBrush;
        set => Set(nameof(BorderBrush), value);
    }
    public LineSet? LineSet
    {
        get => Get<LineSet?>(nameof(LineSet)) ?? Frame.DefaultLineSet;
        set => Set(nameof(LineSet), value);
    }
}

