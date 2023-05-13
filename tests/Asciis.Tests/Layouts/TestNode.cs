using No8.Ascii.Controls;

namespace No8.Ascii.Tests.Layouts;

public class TestNode : Control
{
    public TestNode(string name, LayoutPlan? plan = null, Style? style = null)
        : base(plan ?? new TestNodeLayoutPlan(), style ?? new TestNodeStyle())
    {
        Name = name;
    }

    public TestNode(LayoutPlan? plan = null, Style? style = null)
        : base(plan ?? new TestNodeLayoutPlan(), style ?? new TestNodeStyle())
    {
    }

    public TestNode(out TestNode node, string name, LayoutPlan? plan = null, Style? style = null)
        : this(name, plan, style)
    {
        node = this;
    }
    public TestNode(out TestNode node, LayoutPlan? plan = null, Style? style = null)
        : this(plan, style)
    {
        node = this;
    }

    private void DoDraw(No8.Ascii.Canvas canvas, RectF? bounds, LineSet lineSet)
    {
        if (bounds == null) return;
        if (bounds.Area.IsZero()) return;
        if (bounds.Height.IsZero() || bounds.Width.IsZero())
            canvas.DrawLine(bounds, lineSet);
        else 
            canvas.DrawRect(bounds, lineSet);
    }

    public override void OnDraw(No8.Ascii.Canvas canvas, RectF? bounds)
    {
        canvas.FillRect(
            Layout.Bounds, 
            Name?.Length > 0 ? Rune.GetRuneAt(Name, 0) : Pixel.Block.LightShade);

        DoDraw(canvas, Layout.Bounds, LineSet.Single);
        DoDraw(canvas, Layout.ContentBounds, LineSet.Double);

        base.OnDraw(canvas, bounds);
    }
}

public class TestNodeLayoutPlan : LayoutPlan { }

public class TestNodeStyle : Style { }
