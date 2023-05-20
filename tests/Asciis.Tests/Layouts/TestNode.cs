using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;

namespace No8.Ascii.Tests.Layouts;

public class TestNode : Control
{
    public TestNode(string name, LayoutPlan? plan = null, Style? style = null)
        : base(new ControlPlan(plan ?? new TestPlan()), style ?? new TestStyle())
    {
        Name = name;
    }

    public TestNode(LayoutPlan? plan = null, Style? style = null)
        : base(new ControlPlan(plan ?? new TestPlan()), style ?? new TestStyle())
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
        if (bounds.Value.Area.IsZero()) return;
        if (bounds.Value.Height.IsZero() || bounds.Value.Width.IsZero())
            canvas.DrawLine(bounds.Value, lineSet);
        else 
            canvas.DrawRect(bounds.Value, lineSet);
    }

    public override void OnDraw(No8.Ascii.Canvas canvas, RectF? bounds)
    {
        canvas.FillRect(
            Layout.Bounds, 
            Name?.Length > 0 ? Rune.GetRuneAt(Name, 0) : Runes.Block.LightShade);

        DoDraw(canvas, Layout.Bounds, LineSet.Single);
        DoDraw(canvas, Layout.ContentBounds, LineSet.Double);

        base.OnDraw(canvas, bounds);
    }
}

public class TestPlan : LayoutPlan
{
    
}

public class TestStyle : Style { }
