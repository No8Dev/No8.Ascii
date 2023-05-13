using No8.Ascii.Console;
using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class JustifyContentTest : BaseTests
{
    public JustifyContentTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void justify_content_row_flex_start()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 4 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══╗╔══╗╔══╗═══════════════════╗
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
║AA║║BB║║CC║░░░░░░░░░░░░░░░░░░░║
╚══╝╚══╝╚══╝═══════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 4, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(4, 0, 4, 16), rootB.Layout.Bounds);
        Assert.Equal(new RectF(8, 0, 4, 16), rootC.Layout.Bounds);
    }

    [Fact]
    public void justify_content_row_flex_end()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.End, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 4 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔═══════════════════╔══╗╔══╗╔══╗
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
║░░░░░░░░░░░░░░░░░░░║AA║║BB║║CC║
╚═══════════════════╚══╝╚══╝╚══╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(20, 0, 4, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(24, 0, 4, 16), rootB.Layout.Bounds);
        Assert.Equal(new RectF(28, 0, 4, 16), rootC.Layout.Bounds);
    }

    [Fact]
    public void justify_content_row_center()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 4 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔═════════╔══╗╔══╗╔══╗═════════╗
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
║░░░░░░░░░║AA║║BB║║CC║░░░░░░░░░║
╚═════════╚══╝╚══╝╚══╝═════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(10, 0, 4, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(14, 0, 4, 16), rootB.Layout.Bounds);
        Assert.Equal(new RectF(18, 0, 4, 16), rootC.Layout.Bounds);
    }

    [Fact]
    public void justify_content_row_space_between()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceBetween, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 4 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══╗══════════╔══╗══════════╔══╗
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
║AA║░░░░░░░░░░║BB║░░░░░░░░░░║CC║
╚══╝══════════╚══╝══════════╚══╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 4, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(14, 0, 4, 16), rootB.Layout.Bounds);
        Assert.Equal(new RectF(28, 0, 4, 16), rootC.Layout.Bounds);
    }

    [Fact]
    public void justify_content_row_space_around()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceAround, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 4 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══╔══╗═══════╔══╗═══════╔══╗══╗
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
║░░║AA║░░░░░░░║BB║░░░░░░░║CC║░░║
╚══╚══╝═══════╚══╝═══════╚══╝══╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(3, 0, 4, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(14, 0, 4, 16), rootB.Layout.Bounds);
        Assert.Equal(new RectF(25, 0, 4, 16), rootC.Layout.Bounds);
    }

    [Fact]
    public void justify_content_column_flex_start()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Height = 3 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
╚══════════════════════════════╝
╔══════════════════════════════╗
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
╚══════════════════════════════╝
╔══════════════════════════════╗
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
╚══════════════════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 3), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 3, 32, 3), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 6, 32, 3), rootC.Layout.Bounds);
    }

    [Fact]
    public void justify_content_column_flex_end()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.End, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Height = 3 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════════════════════════╗
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
╚══════════════════════════════╝
╔══════════════════════════════╗
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
╚══════════════════════════════╝
╔══════════════════════════════╗
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 7, 32, 3), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 32, 3), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 13, 32, 3), rootC.Layout.Bounds);
    }

    [Fact]
    public void justify_content_column_center()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Height = 3 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════════════════════════╗
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
╚══════════════════════════════╝
╔══════════════════════════════╗
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
╚══════════════════════════════╝
╔══════════════════════════════╗
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
╚══════════════════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 32, 3), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 7, 32, 3), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 32,3), rootC.Layout.Bounds);
    }

    [Fact]
    public void justify_content_column_space_between()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceBetween, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Height = 3 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
╚══════════════════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════════════════════════╗
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
╚══════════════════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════════════════════════╗
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 3), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 7, 32, 3), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 13, 32, 3), rootC.Layout.Bounds);
    }

    [Fact]
    public void justify_content_column_space_around()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceAround, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Height = 3 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
╔══════════════════════════════╗
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
╚══════════════════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════════════════════════╗
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
╚══════════════════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════════════════════════╗
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
╚══════════════════════════════╝
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 1, 32, 3), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 7, 32, 3), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 12, 32, 3), rootC.Layout.Bounds);
    }

    [Fact]
    public void justify_content_row_min_width_and_margin()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Margin = new Sides(left: 4), MinWidth = 12 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 10, Height = 8 }));

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            @"    ╔════════╔════════╗════════╗
    ║░░░░░░░░║AAAAAAAA║░░░░░░░░║
    ║░░░░░░░░║AAAAAAAA║░░░░░░░░║
    ║░░░░░░░░║AAAAAAAA║░░░░░░░░║
    ║░░░░░░░░║AAAAAAAA║░░░░░░░░║
    ║░░░░░░░░║AAAAAAAA║░░░░░░░░║
    ║░░░░░░░░║AAAAAAAA║░░░░░░░░║
    ║░░░░░░░░╚════════╝░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ╚══════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(4, 0, 28, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(13, 0, 10, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void justify_content_min_width_with_padding_element_width_greater_than_container()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_Cross = Alignment_Cross.Stretch, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_Cross = Alignment_Cross.Stretch })
                    .Add(new TestNode(out var rootAa, "B", new LayoutPlan
                                                           {
                                                               ElementsDirection = LayoutDirection.Horz,
                                                               AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center,
                                                               AlignElements_Cross = Alignment_Cross.Stretch,
                                                               MinWidth = 20,
                                                               Padding = new Sides(horizontal: 2)
                                                           })
                             .Add(new TestNode(out var rootAaa, "C", new LayoutPlan
                                                                     {
                                                                         ElementsDirection = LayoutDirection.Horz,
                                                                         AlignElements_Cross = Alignment_Cross.Stretch,
                                                                         Width = 8,
                                                                         Height = 8
                                                                     }))
                        )
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"┌─╔═══╔══════╗═══╗─┐═══════════╗
│B║BBB║CCCCCC║BBB║B│AAAAAAAAAAA║
│B║BBB║CCCCCC║BBB║B│AAAAAAAAAAA║
│B║BBB║CCCCCC║BBB║B│AAAAAAAAAAA║
│B║BBB║CCCCCC║BBB║B│AAAAAAAAAAA║
│B║BBB║CCCCCC║BBB║B│AAAAAAAAAAA║
│B║BBB║CCCCCC║BBB║B│AAAAAAAAAAA║
└─╚═══╚══════╝═══╝─┘═══════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 20, 8), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(6, 0, 8, 8), rootAaa.Layout.Bounds);
    }

    [Fact]
    public void justify_content_min_width_with_padding_element_width_lower_than_container()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_Cross = Alignment_Cross.Stretch, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_Cross = Alignment_Cross.Stretch })
                    .Add(new TestNode(out var rootAa, "B", new LayoutPlan
                                                           {
                                                               ElementsDirection = LayoutDirection.Horz,
                                                               AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center,
                                                               AlignElements_Cross = Alignment_Cross.Stretch,
                                                               MinWidth = 24,
                                                               Padding = new Sides(horizontal: 2)
                                                           })
                             .Add(new TestNode(out var rootAaa, "C", new LayoutPlan
                                                                     {
                                                                         ElementsDirection = LayoutDirection.Horz,
                                                                         AlignElements_Cross = Alignment_Cross.Stretch,
                                                                         Width = 8,
                                                                         Height = 8
                                                                     }))
                        )
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"┌─╔═════╔══════╗═════╗─┐═══════╗
│B║BBBBB║CCCCCC║BBBBB║B│AAAAAAA║
│B║BBBBB║CCCCCC║BBBBB║B│AAAAAAA║
│B║BBBBB║CCCCCC║BBBBB║B│AAAAAAA║
│B║BBBBB║CCCCCC║BBBBB║B│AAAAAAA║
│B║BBBBB║CCCCCC║BBBBB║B│AAAAAAA║
│B║BBBBB║CCCCCC║BBBBB║B│AAAAAAA║
└─╚═════╚══════╝═════╝─┘═══════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 24, 8), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(8, 0, 8, 8), rootAaa.Layout.Bounds);
    }

    [Fact]
    public void justify_content_row_max_width_and_margin()
    {
        var root = new TestNode(new LayoutPlan
        {
            ElementsDirection = LayoutDirection.Horz,
            AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center,
            Margin = new Sides(left: 4),
            Width = 32,
            MaxWidth = 28
        })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 8, Height = 8 }));

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            @"    ╔═════════╔══════╗═════════╗
    ║░░░░░░░░░║AAAAAA║░░░░░░░░░║
    ║░░░░░░░░░║AAAAAA║░░░░░░░░░║
    ║░░░░░░░░░║AAAAAA║░░░░░░░░░║
    ║░░░░░░░░░║AAAAAA║░░░░░░░░░║
    ║░░░░░░░░░║AAAAAA║░░░░░░░░░║
    ║░░░░░░░░░║AAAAAA║░░░░░░░░░║
    ║░░░░░░░░░╚══════╝░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ║░░░░░░░░░░░░░░░░░░░░░░░░░░║
    ╚══════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(4, 0, 28, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(14, 0, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void justify_content_column_min_height_and_margin()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Margin = new Sides(top: 2), MinHeight = 14 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 8, Height = 8 }));

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            @"

╔══════════════════════════════╗
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 2, 32, 14), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 5, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void justify_content_column_max_height_and_margin()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Margin = new Sides(top: 2), Height = 20, MaxHeight = 14 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 8, Height = 8 }));

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            @"

╔══════════════════════════════╗
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 2, 32, 14), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 5, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void justify_content_column_space_evenly()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceEvenly, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Height = 3 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════════════════════════╗
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
╚══════════════════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════════════════════════╗
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
╚══════════════════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════════════════════════╗
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
╚══════════════════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 2, 32, 3), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 7, 32, 3), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 11, 32, 3), rootC.Layout.Bounds);

    }

    [Fact]
    public void justify_content_row_space_evenly()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceEvenly, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Height = 3 }))
                  .Add(new TestNode(out var rootC, "B", new LayoutPlan { Height = 3 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔═══════A═══════B═══════B══════╗
║░░░░░░░A░░░░░░░B░░░░░░░B░░░░░░║
║░░░░░░░A░░░░░░░B░░░░░░░B░░░░░░║
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
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(8, 0, 0, 3), rootA.Layout.Bounds);
        Assert.Equal(new RectF(16, 0, 0, 3), rootB.Layout.Bounds);
        Assert.Equal(new RectF(24, 0, 0, 3), rootC.Layout.Bounds);
    }
}
