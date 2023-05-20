using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class FlexTest : BaseTests
{
    public FlexTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void main_length_flex_grow_column()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 8 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1 }));

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 12), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 12, 32, 4), rootB.Layout.Bounds);

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
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());
    }

    [Fact]
    public void flex_shrink_flex_grow_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 0, FlexShrink = 1, Width = 32, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 0, FlexShrink = 1, Width = 32, Height = 4 }));

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(16, 0, 16, 4), rootB.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════╗╔══════════════╗
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ╚══════════════╝╚══════════════╝
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
    }

    [Fact]
    public void flex_shrink_flex_grow_element_flex_shrink_other_element()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 0, FlexShrink = 1, Width = 32, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, FlexShrink = 1, Width = 32, Height = 4 }));

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(16, 0, 16, 4), rootB.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════╗╔══════════════╗
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ║AAAAAAAAAAAAAA║║BBBBBBBBBBBBBB║
            ╚══════════════╝╚══════════════╝
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
    }

    [Fact]
    public void main_length_flex_grow_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 8 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1 }));

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 20, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(20, 0, 12, 16), rootB.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════╗╔══════════╗
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAAA║║BBBBBBBBBB║
            ╚══════════════════╝╚══════════╝
            """,
            Canvas!.ToString());
    }

    [Fact]
    public void main_length_flex_shrink_column()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexShrink = 1, ChildLayoutDirectionLength = 12 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { ChildLayoutDirectionLength = 8 }));

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 8), rootB.Layout.Bounds);

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
    }

    [Fact]
    public void main_length_flex_shrink_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexShrink = 1, ChildLayoutDirectionLength = 16 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { ChildLayoutDirectionLength = 16 }));

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(16, 0, 16, 16), rootB.Layout.Bounds);

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
    }

    [Fact]
    public void flex_shrink_to_zero()
    {
        var root = new TestNode(new LayoutPlan { Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 16, Height = 8 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexShrink = 1, Width = 16, Height = 8 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 16, Height = 8 }));

        ElementArrange.Calculate(root);
        Draw(root,32,16);
        Assert.Equal(
            """
            ╔══════════════╗═══════════════╗
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ╚══════════════╝░░░░░░░░░░░░░░░║
            ╔══════════════╗░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 16, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 16, 0), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 16, 8), rootC.Layout.Bounds);
    }

    [Fact]
    public void main_length_overrides_main_size()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 8, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, Height = 4 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { FlexGrow = 1, Height = 4 }));

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
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 4), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 12, 32, 4), rootC.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_shrink_at_most()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A")
                   .Add(new TestNode(out var rootAa, "B", new LayoutPlan { FlexGrow = 1, FlexShrink = 1 })
                       ));

        ElementArrange.Calculate(root);
        Draw(root);
        Assert.Equal(
            """
            BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
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
        Assert.Equal(new RectF(0, 0, 32, 0), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 0), rootAa.Layout.Bounds);
    }

    [Fact]
    public void flex_grow_less_than_factor_one()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 0.2f, ChildLayoutDirectionLength = 3 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 0.2f }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { FlexGrow = 0.4f }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ╚══════════════════════════════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 6), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 6, 32, 2), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 5), rootC.Layout.Bounds);
    }
}
