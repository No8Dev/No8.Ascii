using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class MinMaxDimensionTest : BaseTests
{
    public MinMaxDimensionTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void max_width()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { MaxWidth = 16, Height = 10 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════╗═══════════════╗
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 16, 10), rootA.Layout.Bounds);
    }

    [Fact]
    public void max_height()
    {
        
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 20, MaxHeight = 8 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════╗═══════════╗
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ╚══════════════════╝░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 20, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void min_height()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, MinHeight = 10 }))
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
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 13), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 13, 32, 3), rootB.Layout.Bounds);
    }

    [Fact]
    public void min_width()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, MinWidth = 20 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔════════════════════════╗╔════╗
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ║AAAAAAAAAAAAAAAAAAAAAAAA║║BBBB║
            ╚════════════════════════╝╚════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 26, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(26, 0, 6, 16), rootB.Layout.Bounds);
    }

    [Fact]
    public void justify_content_min_max()
    {
        
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 32, MinHeight = 16, MaxHeight = 20 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 20, Height = 10 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════════════╗░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
            ╚══════════════════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 3, 20, 10), rootA.Layout.Bounds);
    }

    [Fact]
    public void align_items_min_max()
    {
        
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, MinWidth = 32, MaxWidth = 40, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 20, Height = 10 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═════╔══════════════════╗═════╗
            ║░░░░░║AAAAAAAAAAAAAAAAAA║░░░░░║
            ║░░░░░║AAAAAAAAAAAAAAAAAA║░░░░░║
            ║░░░░░║AAAAAAAAAAAAAAAAAA║░░░░░║
            ║░░░░░║AAAAAAAAAAAAAAAAAA║░░░░░║
            ║░░░░░║AAAAAAAAAAAAAAAAAA║░░░░░║
            ║░░░░░║AAAAAAAAAAAAAAAAAA║░░░░░║
            ║░░░░░║AAAAAAAAAAAAAAAAAA║░░░░░║
            ║░░░░░║AAAAAAAAAAAAAAAAAA║░░░░░║
            ║░░░░░╚══════════════════╝░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(6, 0, 20, 10), rootA.Layout.Bounds);
    }

    [Fact]
    public void justify_content_overflow_min_max()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, MinHeight = 16, MaxHeight = 20 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 16, Height = 8 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 16, Height = 8 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 16, Height = 8 }));

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            """
            ║AAAAAAAAAAAAAA║═══════════════╗
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ╚══════════════╝░░░░░░░░░░░░░░░║
            ╔══════════════╗░░░░░░░░░░░░░░░║
            ║BBBBBBBBBBBBBB║░░░░░░░░░░░░░░░║
            ║BBBBBBBBBBBBBB║░░░░░░░░░░░░░░░║
            ║BBBBBBBBBBBBBB║░░░░░░░░░░░░░░░║
            ║BBBBBBBBBBBBBB║░░░░░░░░░░░░░░░║
            ║BBBBBBBBBBBBBB║░░░░░░░░░░░░░░░║
            ║BBBBBBBBBBBBBB║░░░░░░░░░░░░░░░║
            ╚══════════════╝░░░░░░░░░░░░░░░║
            ╔══════════════╗░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 20), root.Layout.Bounds);
        Assert.Equal(new RectF(0, -2, 16, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 6, 16, 8), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 14, 16, 8), rootC.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_to_min()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, MinHeight = 16, MaxHeight = 20 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, FlexShrink = 1 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 8 }));

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
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 8), rootB.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_in_at_most_container()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { ElementsDirection = LayoutDirection.Horz })
                    .Add(new TestNode(out var rootAa, "B", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 0 }))
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            B══════════════════════════════╗
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
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 0, 0), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 0, 0), rootAa.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_element()
    {
        
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 0, Height = 16 }));

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
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_within_constrained_min_max_column()
    {
        var root = new TestNode(new LayoutPlan { MinHeight = 16, MaxHeight = 20 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 8 }));

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
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 8), rootB.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_within_max_width()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, MaxWidth = 20 })
                    .Add(new TestNode(out var rootAa, "B", new LayoutPlan { FlexGrow = 1, Height = 10 }))
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════╗═══════════╗
            ║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
            ║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
            ║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
            ║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
            ║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
            ║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
            ║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
            ║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
            ╚══════════════════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 20, 10), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 20, 10), rootAa.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_within_constrained_max_width()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, MaxWidth = 40 })
                    .Add(new TestNode(out var rootAa, "B", new LayoutPlan { FlexGrow = 1, Height = 10 }))
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 10), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 10), rootAa.Layout.Bounds);
    }

    [Fact]
    public void flex_root_ignored()
    {
        var root = new TestNode(new LayoutPlan { FlexGrow = 1, Width = 32, MinHeight = 16, MaxHeight = 20 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 10 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 3 }));

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
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 13), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 13, 32, 3), rootB.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_root_minimized()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, MinHeight = 8, MaxHeight = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, MinHeight = 8, MaxHeight = 16 })
                         .Add(new TestNode(out var rootAa, "B", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 8 }))
                         .Add(new TestNode(out var rootAb, "C", new LayoutPlan { Height = 4 }))
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 12), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 12), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 4), rootAb.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_height_maximized()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, MinHeight = 8, MaxHeight = 16 })
                         .Add(new TestNode(out var rootAa, "B", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 12 }))
                         .Add(new TestNode(out var rootAb, "C", new LayoutPlan { Height = 12 }))
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 12), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 12, 32, 12), rootAb.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_within_constrained_min_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, MinWidth = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 16 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════╗╔══════════════╗
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ╚══════════════╝╚══════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(16, 0, 16, 16), rootB.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_within_constrained_min_column()
    {
        var root = new TestNode(new LayoutPlan { MinHeight = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 8 }));

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
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 8), rootB.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_within_constrained_max_row()
    {
        var root = new TestNode(new LayoutPlan { Width = 32 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, MaxWidth = 16, Height = 16 })
                        .Add(new TestNode(out var rootAa, "B", new LayoutPlan { FlexShrink = 1, ChildLayoutDirectionLength = 8 }))
                        .Add(new TestNode(out var rootAb, "C", new LayoutPlan { Width = 16 }))
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════╗═══════════════╗
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
            ╚══════════════╝═══════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 0, 16), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 16), rootAb.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_within_constrained_max_column()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, MaxHeight = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexShrink = 1, ChildLayoutDirectionLength = 16 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 8 }));

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
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 8), rootB.Layout.Bounds);
    }

    [Fact]
    public void element_min_max_width_flexing()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 8 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 0, MinWidth = 12 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 50.Percent(), MaxWidth = 16 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════╗╔══════════════╗
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ╚══════════════╝╚══════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 8), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(16, 0, 16, 8), rootB.Layout.Bounds);
    }

    [Fact]
    public void min_width_overrides_width()
    {
        var root = new TestNode(new LayoutPlan { Width = 16, MinWidth = 32 });

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 0), root.Layout.Bounds);
    }

    [Fact]
    public void max_width_overrides_width()
    {
        var root = new TestNode(new LayoutPlan { Width = 64, MaxWidth = 32 });

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 0), root.Layout.Bounds);
    }

    [Fact]
    public void min_height_overrides_height()
    {
        var root = new TestNode(new LayoutPlan { Height = 8, MinHeight = 16 });

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 0, 16), root.Layout.Bounds);
    }

    [Fact]
    public void max_height_overrides_height()
    {
        var root = new TestNode(new LayoutPlan { Height = 32, MaxHeight = 16 });

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 0, 16), root.Layout.Bounds);
    }

    [Fact]
    public void min_max_percent_no_width_height()
    {
        
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { MinWidth = 30.Percent(), MaxWidth = 30.Percent(), MinHeight = 30.Percent(), MaxHeight = 30.Percent() }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔════════╗═════════════════════╗
            ║AAAAAAAA║░░░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAA║░░░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAA║░░░░░░░░░░░░░░░░░░░░░║
            ╚════════╝░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 10, 5), rootA.Layout.Bounds);
    }
}
