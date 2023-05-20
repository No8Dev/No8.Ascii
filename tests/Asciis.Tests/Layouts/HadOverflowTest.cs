using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class HadOverflowTest : BaseTests
{
    public HadOverflowTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void elements_overflow_no_wrap_and_no_flex_elements()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 12, Height = 8, Margin = new Sides(top: 1, bottom: 1) }),
                       new TestNode(out var rootB, "B", new LayoutPlan { Width = 12, Height = 6, Margin = new Sides(bottom: 1) })
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ╔══════════╗░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ╚══════════╝░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════╗░░░░░░░░░░░░░░░░░░░║
            ║BBBBBBBBBB║░░░░░░░░░░░░░░░░░░░║
            ║BBBBBBBBBB║░░░░░░░░░░░░░░░░░░░║
            ║BBBBBBBBBB║░░░░░░░░░░░░░░░░░░░║
            ║BBBBBBBBBB║░░░░░░░░░░░░░░░░░░░║
            ╚══════════╝═══════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 1, 12, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 12, 6), rootB.Layout.Bounds);

        Assert.True(root.Layout.HadOverflow);
    }

    [Fact]
    public void spacing_overflow_no_wrap_and_no_flex_elements()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 12, Height = 8, Margin = new Sides(top: 1, bottom: 1) }),
                       new TestNode(out var rootB, "B", new LayoutPlan { Width = 12, Height = 6, Margin = new Sides(bottom: 5) })
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ╔══════════╗░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ╚══════════╝░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════╗░░░░░░░░░░░░░░░░░░░║
            ║BBBBBBBBBB║░░░░░░░░░░░░░░░░░░░║
            ║BBBBBBBBBB║░░░░░░░░░░░░░░░░░░░║
            ║BBBBBBBBBB║░░░░░░░░░░░░░░░░░░░║
            ║BBBBBBBBBB║░░░░░░░░░░░░░░░░░░░║
            ╚══════════╝═══════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 1, 12, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 12, 6), rootB.Layout.Bounds);

        Assert.True(root.Layout.HadOverflow);
    }

    [Fact]
    public void no_overflow_no_wrap_and_flex_elements()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 12, Height = 8, Margin = new Sides(top: 1, bottom: 1) }),
                       new TestNode(out var rootB, "B", new LayoutPlan { Width = 12, Height = 6, Margin = new Sides(bottom: 5), FlexShrink = 1 })
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ╔══════════╗░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ╚══════════╝░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════╗░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 1, 12, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 12, 1), rootB.Layout.Bounds);

        Assert.False(root.Layout.HadOverflow);
    }

    [Fact]
    public void hadOverflow_gets_reset_if_not_logger_valid()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 12, Height = 8, Margin = new Sides(top: 1, bottom: 1) }),
                       new TestNode(out var rootB, "B", new LayoutPlan { Width = 12, Height = 6, Margin = new Sides(bottom: 5) })
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ╔══════════╗░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ╚══════════╝░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════╗░░░░░░░░░░░░░░░░░░░║
            ║BBBBBBBBBB║░░░░░░░░░░░░░░░░░░░║
            ║BBBBBBBBBB║░░░░░░░░░░░░░░░░░░░║
            ║BBBBBBBBBB║░░░░░░░░░░░░░░░░░░░║
            ║BBBBBBBBBB║░░░░░░░░░░░░░░░░░░░║
            ╚══════════╝═══════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 1, 12, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 12, 6), rootB.Layout.Bounds);

        Assert.True(root.Layout.HadOverflow);

        rootB.Plan.FlexShrink = 1;
        ElementArrange.Calculate(root);
        Assert.False(root.Layout.HadOverflow);
    }

    [Fact]
    public void spacing_overflow_in_nested_nodes()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 12, Height = 8, Margin = new Sides(top: 1, bottom: 1) }),
                       new TestNode(out var rootB, "B", new LayoutPlan { Width = 12, Height = 8 })
                       {
                           new TestNode(out var rootBa, "C", new LayoutPlan { Width = 12, Height = 8, Margin = new Sides(bottom: 5) })
                       }
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ╔══════════╗░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ╚══════════╝░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════╗░░░░░░░░░░░░░░░░░░░║
            ║CCCCCCCCCC║░░░░░░░░░░░░░░░░░░░║
            ║CCCCCCCCCC║░░░░░░░░░░░░░░░░░░░║
            ║CCCCCCCCCC║░░░░░░░░░░░░░░░░░░░║
            ║CCCCCCCCCC║░░░░░░░░░░░░░░░░░░░║
            ║CCCCCCCCCC║═══════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 1, 12, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 12, 8), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 12, 8), rootBa.Layout.Bounds);

        Assert.True(root.Layout.HadOverflow);
    }
}
