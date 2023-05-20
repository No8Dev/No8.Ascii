using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class FlexDirectionTest : BaseTests
{
    public FlexDirectionTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void flex_direction_column_no_height()
    {
        var root = new TestNode(new LayoutPlan { Width = 32 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 4 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        TestContext.WriteLine(Canvas!.ToString());

        Assert.Equal(
            """
            ╔══════════════════════════════╗
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
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 32, 4), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 4), rootC.Layout.Bounds);
    }

    [Fact]
    public void flex_direction_row_no_width()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 8 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 8 }));

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            """
            ╔══════╗╔══════╗╔══════╗═══════╗
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ╚══════╝╚══════╝╚══════╝═══════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 8, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(8, 0, 8, 16), rootB.Layout.Bounds);
        Assert.Equal(new RectF(16, 0, 8, 16), rootC.Layout.Bounds);
    }

    [Fact]
    public void flex_direction_column()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 4 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
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
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 32, 4), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 4), rootC.Layout.Bounds);
    }

    [Fact]
    public void flex_direction_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 8 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 8 }));

        ElementArrange.Calculate(root);

        Draw(root);


        Assert.Equal(
            """
            ╔══════╗╔══════╗╔══════╗═══════╗
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ║AAAAAA║║BBBBBB║║CCCCCC║░░░░░░░║
            ╚══════╝╚══════╝╚══════╝═══════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 8, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(8, 0, 8, 16), rootB.Layout.Bounds);
        Assert.Equal(new RectF(16, 0, 8, 16), rootC.Layout.Bounds);
    }
}
