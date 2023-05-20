using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class MarginTest :BaseTests
{
    public MarginTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void margin_start()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(start: 2), Width = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═╔══╗═════════════════════════╗
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░║AA║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚═╚══╝═════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(2, 0, 4, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_top()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(top: 2), Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════════════════════════╗
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 2, 32, 4), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_end()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.End, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(end: 2), Width = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═════════════════════════╔══╗═╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░║AA║░║
            ╚═════════════════════════╚══╝═╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(26, 0, 4, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_bottom()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.End, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(bottom: 2), Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════════════════════════╗
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 32, 4), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_and_flex_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(start: 2, end: 2) }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═╔══════════════════════════╗═╗
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ╚═╚══════════════════════════╝═╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(2, 0, 28, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_and_flex_column()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(top: 2, bottom: 2) }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════════════════════════╗
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 2, 32, 12), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_and_stretch_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(top: 2, bottom: 2) }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════════════════════════╗
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 2, 32, 12), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_and_stretch_column()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(start: 2, end: 2) }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═╔══════════════════════════╗═╗
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAA║░║
            ╚═╚══════════════════════════╝═╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(2, 0, 28, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_with_sibling_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(end: 2) }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═════════════╗══╔═════════════╗
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ║AAAAAAAAAAAAA║░░║BBBBBBBBBBBBB║
            ╚═════════════╝══╚═════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 15, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(17, 0, 15, 16), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_with_sibling_column()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(bottom: 2) }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 7), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 9, 32, 7), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_bottom()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(bottom: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═══════════╔══════╗═══════════╗
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ╚═══════════╚══════╝═══════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(12, 0, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 12, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_top()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(top: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ╚═══════════╚══════╝═══════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(12, 8, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 12, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_bottom_and_top()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(top: Number.Auto, bottom: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ╚═══════════╚══════╝═══════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(12, 4, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 12, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_bottom_and_top_justify_center()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(top: Number.Auto, bottom: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
            ║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
            ║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════╝═══════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 12, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_multiple_elements_column()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(top: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Margin = new Sides(top: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║CCCCCC║░░░░░░░░░░░║
            ║░░░░░░░░░░░║CCCCCC║░░░░░░░░░░░║
            ╚═══════════╚══════╝═══════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(12, 2, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 8, 8, 4), rootB.Layout.Bounds);
        Assert.Equal(new RectF(12, 12, 8, 4), rootC.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_multiple_elements_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(right: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Margin = new Sides(right: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════╗░░░░╔══════╗░░░░╔══════╗
            ║AAAAAA║░░░░║BBBBBB║░░░░║CCCCCC║
            ║AAAAAA║░░░░║BBBBBB║░░░░║CCCCCC║
            ╚══════╝░░░░╚══════╝░░░░╚══════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 6, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 6, 8, 4), rootB.Layout.Bounds);
        Assert.Equal(new RectF(24, 6, 8, 4), rootC.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_left_and_right_column()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(Number.Auto, right: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));
        
        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░╔══════╗░░░░░░░░╔══════╗
            ║░░░░░░░║AAAAAA║░░░░░░░░║BBBBBB║
            ║░░░░░░░║AAAAAA║░░░░░░░░║BBBBBB║
            ║░░░░░░░╚══════╝░░░░░░░░╚══════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(8, 6, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(24, 6, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_left_and_right()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(Number.Auto, right: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═══════════╔══════╗═══════════╗
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
            ╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(12, 0, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_start_and_end_column()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(start: Number.Auto, end: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░╔══════╗░░░░░░░░╔══════╗
            ║░░░░░░░║AAAAAA║░░░░░░░░║BBBBBB║
            ║░░░░░░░║AAAAAA║░░░░░░░░║BBBBBB║
            ║░░░░░░░╚══════╝░░░░░░░░╚══════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(8, 6, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(24, 6, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_start_and_end()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(start: Number.Auto, end: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═══════════╔══════╗═══════════╗
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
            ╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(12, 0, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_left_and_right_column_and_center()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(Number.Auto, right: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═══════════╔══════╗═══════════╗
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(12, 0, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 4, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_left()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(left: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═══════════════════════╔══════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░║AAAAAA║
            ║░░░░░░░░░░░░░░░░░░░░░░░║AAAAAA║
            ║░░░░░░░░░░░░░░░░░░░░░░░╚══════╝
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(24, 0, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 4, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_right()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(right: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════╗═══════════════════════╗
            ║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
            ║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 4, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_left_and_right_stretch()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(Number.Auto, right: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═══════╔══════╗════════╔══════╗
            ║░░░░░░░║AAAAAA║░░░░░░░░║BBBBBB║
            ║░░░░░░░║AAAAAA║░░░░░░░░║BBBBBB║
            ║░░░░░░░╚══════╝░░░░░░░░╚══════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(8, 0, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(24, 0, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_top_and_bottom_stretch()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(top: Number.Auto, bottom: Number.Auto), Width = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
            ║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
            ║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════╝═══════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 12, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_should_not_be_part_of_max_height()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(top: 2), Width = 16, Height = 8, MaxHeight = 8 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════════╗░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ╚══════════════╝░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 2, 16, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_should_not_be_part_of_max_width()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(left: 2), Width = 16, MaxWidth = 16, Height = 8 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═╔══════════════╗═════════════╗
            ║░║AAAAAAAAAAAAAA║░░░░░░░░░░░░░║
            ║░║AAAAAAAAAAAAAA║░░░░░░░░░░░░░║
            ║░║AAAAAAAAAAAAAA║░░░░░░░░░░░░░║
            ║░║AAAAAAAAAAAAAA║░░░░░░░░░░░░░║
            ║░║AAAAAAAAAAAAAA║░░░░░░░░░░░░░║
            ║░║AAAAAAAAAAAAAA║░░░░░░░░░░░░░║
            ║░╚══════════════╝░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(2, 0, 16, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_left_right_element_bigger_than_container()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(Number.Auto, right: Number.Auto), Width = 40, Height = 20 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, -2, 40, 20), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_left_element_bigger_than_container()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(left: Number.Auto), Width = 40, Height = 20 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, -2, 40, 20), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_fix_left_auto_right_element_bigger_than_container()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(2, right: Number.Auto), Width = 40, Height = 20 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║░║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ╚═║AAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(2, -2, 40, 20), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_left_fix_right_element_bigger_than_container()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(Number.Auto, right: 2), Width = 40, Height = 20 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, -2, 40, 20), rootA.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_top_stretching_element()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, FlexShrink = 1, ChildLayoutDirectionLength = 0.Percent(), Margin = new Sides(top: Number.Auto) }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═══════════════A══════════════╗
            ║░░░░░░░░░░░░░░░A░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░A░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░A░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░A░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░A░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░A░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░A░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░A░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░A░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░A░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░A░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ╚═══════════╚══════╝═══════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(16, 0, 0, 12), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 12, 8, 4), rootB.Layout.Bounds);
    }

    [Fact]
    public void margin_auto_left_stretching_element()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, FlexShrink = 1, ChildLayoutDirectionLength = 0.Percent(), Margin = new Sides(left: Number.Auto) }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
            ╚═══════════╚══════╝═══════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(32, 0, 0, 12), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 12, 8, 4), rootB.Layout.Bounds);
    }
}
