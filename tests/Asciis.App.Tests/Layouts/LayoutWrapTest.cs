using Asciis.App.Controls;
using Asciis.App.ElementLayout;
using Xunit;
using Xunit.Abstractions;
// ReSharper disable StringLiteralTypo

namespace Asciis.App.Tests.Layouts;

[TestClass]
public class LayoutWrapTest : BaseTests
{
    public LayoutWrapTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void wrap_column()
    {
        var root = new TestNode(new LayoutPlan { ElementsWrap = LayoutWrap.Wrap, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 8, Height = 5 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 5 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 8, Height = 5 }))
                  .Add(new TestNode(out var rootD, "D", new LayoutPlan { Width = 8, Height = 5 }));

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            @"╔══════╗╔══════╗═══════════════╗
║AAAAAA║║DDDDDD║░░░░░░░░░░░░░░░║
║AAAAAA║║DDDDDD║░░░░░░░░░░░░░░░║
║AAAAAA║║DDDDDD║░░░░░░░░░░░░░░░║
╚══════╝╚══════╝░░░░░░░░░░░░░░░║
╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
║CCCCCC║░░░░░░░░░░░░░░░░░░░░░░░║
║CCCCCC║░░░░░░░░░░░░░░░░░░░░░░░║
║CCCCCC║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 8, 5), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 5, 8, 5), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 8, 5), rootC.Layout.Bounds);
        Assert.Equal(new RectF(8, 0, 8, 5), rootD.Layout.Bounds);
    }

    [Fact]
    public void wrap_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, ElementsWrap = LayoutWrap.Wrap, Width = 32 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 10, Height = 8 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 10, Height = 8 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 10, Height = 8 }))
                  .Add(new TestNode(out var rootD, "D", new LayoutPlan { Width = 10, Height = 8 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔════════╗╔════════╗╔════════╗═╗
║AAAAAAAA║║BBBBBBBB║║CCCCCCCC║░║
║AAAAAAAA║║BBBBBBBB║║CCCCCCCC║░║
║AAAAAAAA║║BBBBBBBB║║CCCCCCCC║░║
║AAAAAAAA║║BBBBBBBB║║CCCCCCCC║░║
║AAAAAAAA║║BBBBBBBB║║CCCCCCCC║░║
║AAAAAAAA║║BBBBBBBB║║CCCCCCCC║░║
╚════════╝╚════════╝╚════════╝░║
╔════════╗░░░░░░░░░░░░░░░░░░░░░║
║DDDDDDDD║░░░░░░░░░░░░░░░░░░░░░║
║DDDDDDDD║░░░░░░░░░░░░░░░░░░░░░║
║DDDDDDDD║░░░░░░░░░░░░░░░░░░░░░║
║DDDDDDDD║░░░░░░░░░░░░░░░░░░░░░║
║DDDDDDDD║░░░░░░░░░░░░░░░░░░░░░║
║DDDDDDDD║░░░░░░░░░░░░░░░░░░░░░║
╚════════╝═════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 10, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(10, 0, 10, 8), rootB.Layout.Bounds);
        Assert.Equal(new RectF(20, 0, 10, 8), rootC.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 10, 8), rootD.Layout.Bounds);
    }

    [Fact]
    public void wrap_row_align_items_flex_end()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Align_Cross = AlignmentLine_Cross.End, ElementsWrap = LayoutWrap.Wrap, Width = 32 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 5, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 5, Height = 8 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 5, Height = 12 }))
                  .Add(new TestNode(out var rootD, "D", new LayoutPlan { Width = 5, Height = 12 }));

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            @"╔═════════╔═══╗╔═══╗═══════════╗
║░░░░░░░░░║CCC║║DDD║░░░░░░░░░░░║
║░░░░░░░░░║CCC║║DDD║░░░░░░░░░░░║
║░░░░░░░░░║CCC║║DDD║░░░░░░░░░░░║
║░░░░╔═══╗║CCC║║DDD║░░░░░░░░░░░║
║░░░░║BBB║║CCC║║DDD║░░░░░░░░░░░║
║░░░░║BBB║║CCC║║DDD║░░░░░░░░░░░║
║░░░░║BBB║║CCC║║DDD║░░░░░░░░░░░║
╔═══╗║BBB║║CCC║║DDD║░░░░░░░░░░░║
║AAA║║BBB║║CCC║║DDD║░░░░░░░░░░░║
║AAA║║BBB║║CCC║║DDD║░░░░░░░░░░░║
╚═══╝╚═══╝╚═══╝╚═══╝░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 5, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(5, 4, 5, 8), rootB.Layout.Bounds);
        Assert.Equal(new RectF(10, 0, 5, 12), rootC.Layout.Bounds);
        Assert.Equal(new RectF(15, 0, 5, 12), rootD.Layout.Bounds);
    }

    [Fact]
    public void wrap_row_align_items_center()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Align_Cross = AlignmentLine_Cross.Center, ElementsWrap = LayoutWrap.Wrap, Width = 32 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 5, Height = 4 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 5, Height = 8 }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 5, Height = 12 }))
                  .Add(new TestNode(out var rootD, "D", new LayoutPlan { Width = 5, Height = 12 }));

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            @"╔═════════╔═══╗╔═══╗═══════════╗
║░░░░░░░░░║CCC║║DDD║░░░░░░░░░░░║
║░░░░╔═══╗║CCC║║DDD║░░░░░░░░░░░║
║░░░░║BBB║║CCC║║DDD║░░░░░░░░░░░║
╔═══╗║BBB║║CCC║║DDD║░░░░░░░░░░░║
║AAA║║BBB║║CCC║║DDD║░░░░░░░░░░░║
║AAA║║BBB║║CCC║║DDD║░░░░░░░░░░░║
╚═══╝║BBB║║CCC║║DDD║░░░░░░░░░░░║
║░░░░║BBB║║CCC║║DDD║░░░░░░░░░░░║
║░░░░╚═══╝║CCC║║DDD║░░░░░░░░░░░║
║░░░░░░░░░║CCC║║DDD║░░░░░░░░░░░║
║░░░░░░░░░╚═══╝╚═══╝░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 5, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(5, 2, 5, 8), rootB.Layout.Bounds);
        Assert.Equal(new RectF(10, 0, 5, 12), rootC.Layout.Bounds);
        Assert.Equal(new RectF(15, 0, 5, 12), rootD.Layout.Bounds);
    }

    [Fact]
    public void layout_wrap_elements_with_min_main_overriding_main_length()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, ElementsWrap = LayoutWrap.Wrap, Width = 32 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { ChildLayoutDirectionLength = 8, MinWidth = 20, Height = 8 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { ChildLayoutDirectionLength = 8, MinWidth = 20, Height = 8 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════╗═══════════╗
║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
║AAAAAAAAAAAAAAAAAA║░░░░░░░░░░░║
╚══════════════════╝░░░░░░░░░░░║
╔══════════════════╗░░░░░░░░░░░║
║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
║BBBBBBBBBBBBBBBBBB║░░░░░░░░░░░║
╚══════════════════╝═══════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 20, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 20, 8), rootB.Layout.Bounds);
    }

    [Fact]
    public void layout_wrap_wrap_to_element_height()
    {
        var root = new TestNode()
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Align_Cross = AlignmentLine_Cross.Start, ElementsWrap = LayoutWrap.Wrap })
                          .Add(new TestNode(out var rootAa, "B", new LayoutPlan { Width = 32 })
                                  .Add(new TestNode(out var rootAaa, "C", new LayoutPlan { Width = 32, Height = 8 }))
                              )
                      )
                  .Add(new TestNode(out var rootB, "D", new LayoutPlan { Width = 32, Height = 8 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
╚══════════════════════════════╝
╔══════════════════════════════╗
║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootAaa.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 8), rootB.Layout.Bounds);
    }

    //[Ignore("Exactly the same result as the c++ library")]
    [Fact]
    public void layout_wrap_align_stretch_fits_one_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, ElementsWrap = LayoutWrap.Wrap, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 10 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 10 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"AAAAAAAAAABBBBBBBBBB═══════════╗
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
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 10, 0), rootA.Layout.Bounds);
        Assert.Equal(new RectF(10, 0, 10, 0), rootB.Layout.Bounds);
    }

    [Fact]
    public void wrapped_row_within_align_items_center()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, ElementsWrap = LayoutWrap.Wrap })
                        .Add(new TestNode(out var rootAa, "B", new LayoutPlan { Width = 28, Height = 6 }))
                        .Add(new TestNode(out var rootAb, "C", new LayoutPlan { Width = 14, Height = 6 }))
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════╗═══╗
║BBBBBBBBBBBBBBBBBBBBBBBBBB║AAA║
║BBBBBBBBBBBBBBBBBBBBBBBBBB║AAA║
║BBBBBBBBBBBBBBBBBBBBBBBBBB║AAA║
║BBBBBBBBBBBBBBBBBBBBBBBBBB║AAA║
╚══════════════════════════╝AAA║
╔════════════╗AAAAAAAAAAAAAAAAA║
║CCCCCCCCCCCC║AAAAAAAAAAAAAAAAA║
║CCCCCCCCCCCC║AAAAAAAAAAAAAAAAA║
║CCCCCCCCCCCC║AAAAAAAAAAAAAAAAA║
║CCCCCCCCCCCC║AAAAAAAAAAAAAAAAA║
╚════════════╝═════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 12), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 28, 6), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 6, 14, 6), rootAb.Layout.Bounds);
    }

    [Fact]
    public void wrapped_row_within_align_items_flex_start()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, ElementsWrap = LayoutWrap.Wrap })
                        .Add(new TestNode(out var rootAa, "B", new LayoutPlan { Width = 28, Height = 6 }))
                        .Add(new TestNode(out var rootAb, "C", new LayoutPlan { Width = 6, Height  = 6 }))
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════╗═══╗
║BBBBBBBBBBBBBBBBBBBBBBBBBB║AAA║
║BBBBBBBBBBBBBBBBBBBBBBBBBB║AAA║
║BBBBBBBBBBBBBBBBBBBBBBBBBB║AAA║
║BBBBBBBBBBBBBBBBBBBBBBBBBB║AAA║
╚══════════════════════════╝AAA║
╔════╗AAAAAAAAAAAAAAAAAAAAAAAAA║
║CCCC║AAAAAAAAAAAAAAAAAAAAAAAAA║
║CCCC║AAAAAAAAAAAAAAAAAAAAAAAAA║
║CCCC║AAAAAAAAAAAAAAAAAAAAAAAAA║
║CCCC║AAAAAAAAAAAAAAAAAAAAAAAAA║
╚════╝═════════════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 12), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 28, 6), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 6, 6, 6), rootAb.Layout.Bounds);
    }

    [Fact]
    public void wrapped_row_within_align_items_flex_end()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.End, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, ElementsWrap = LayoutWrap.Wrap })
                        .Add(new TestNode(out var rootAa, "B", new LayoutPlan { Width = 28, Height = 6 }))
                        .Add(new TestNode(out var rootAb, "C", new LayoutPlan { Width = 6, Height = 6 }))
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════╗═══╗
║BBBBBBBBBBBBBBBBBBBBBBBBBB║AAA║
║BBBBBBBBBBBBBBBBBBBBBBBBBB║AAA║
║BBBBBBBBBBBBBBBBBBBBBBBBBB║AAA║
║BBBBBBBBBBBBBBBBBBBBBBBBBB║AAA║
╚══════════════════════════╝AAA║
╔════╗AAAAAAAAAAAAAAAAAAAAAAAAA║
║CCCC║AAAAAAAAAAAAAAAAAAAAAAAAA║
║CCCC║AAAAAAAAAAAAAAAAAAAAAAAAA║
║CCCC║AAAAAAAAAAAAAAAAAAAAAAAAA║
║CCCC║AAAAAAAAAAAAAAAAAAAAAAAAA║
╚════╝═════════════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 12), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 28, 6), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 6, 6, 6), rootAb.Layout.Bounds);
    }

    [Fact]
    public void wrapped_column_max_height()
    {
        var root = new TestNode(new LayoutPlan
        {
            AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center,
            AlignElements_Cross = Alignment_Cross.Center,
            Align_Cross = AlignmentLine_Cross.Center,
            ElementsWrap = LayoutWrap.Wrap,
            Width = 32,
            Height = 16
        })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 4, Height = 16, MaxHeight = 6 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 6, Margin = new Sides(2) }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 8, Height = 8 }));
        
        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔═════════╔══╗═════════════════╗
║░░░░░░░░░║AA║░░░░░░░░░░░░░░░░░║
║░░░░░░░░░║AA║░░░░░░░░░░░░░░░░░║
║░░░░░░░░░║AA║░░░░░░░░░░░░░░░░░║
║░░░░░░░░░║AA║░░░░╔══════╗░░░░░║
║░░░░░░░░░╚══╝░░░░║CCCCCC║░░░░░║
║░░░░░░░░░░░░░░░░░║CCCCCC║░░░░░║
║░░░░░░░░░░░░░░░░░║CCCCCC║░░░░░║
║░░░░░░░╔══════╗░░║CCCCCC║░░░░░║
║░░░░░░░║BBBBBB║░░║CCCCCC║░░░░░║
║░░░░░░░║BBBBBB║░░║CCCCCC║░░░░░║
║░░░░░░░║BBBBBB║░░╚══════╝░░░░░║
║░░░░░░░║BBBBBB║░░░░░░░░░░░░░░░║
║░░░░░░░╚══════╝░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(10, 0, 4, 6), rootA.Layout.Bounds);
        Assert.Equal(new RectF(8, 8, 8, 6), rootB.Layout.Bounds);
        Assert.Equal(new RectF(18, 4, 8, 8), rootC.Layout.Bounds);
    }

    [Fact]
    public void wrapped_column_max_height_flex()
    {
        var root = new TestNode(new LayoutPlan
        {
            AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center,
            AlignElements_Cross = Alignment_Cross.Center,
            Align_Cross = AlignmentLine_Cross.Center,
            ElementsWrap = LayoutWrap.Wrap,
            Width = 32,
            Height = 16
        })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan
                                                        {
                                                            FlexGrow = 1,
                                                            FlexShrink = 1,
                                                            ChildLayoutDirectionLength = 0.Percent(),
                                                            Width = 10,
                                                            Height = 6,
                                                            MaxHeight = 10
                                                        }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan
                                                        {
                                                            FlexGrow = 1,
                                                            FlexShrink = 1,
                                                            ChildLayoutDirectionLength = 0.Percent(),
                                                            Margin = new Sides(2),
                                                            Width = 8,
                                                            Height = 8
                                                        }))
                  .Add(new TestNode(out var rootC, "C", new LayoutPlan { Width = 4, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════╔════════╗══════════╗
║░░░░░░░░░░║AAAAAAAA║░░░░░░░░░░║
║░░░░░░░░░░║AAAAAAAA║░░░░░░░░░░║
║░░░░░░░░░░╚════════╝░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
║░░░░░░░░░░░║BBBBBB║░░░░░░░░░░░║
║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░╔══╗░░░░░░░░░░░░░║
║░░░░░░░░░░░░░║CC║░░░░░░░░░░░░░║
║░░░░░░░░░░░░░║CC║░░░░░░░░░░░░░║
╚═════════════╚══╝═════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(11, 0, 10, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 6, 8, 4), rootB.Layout.Bounds);
        Assert.Equal(new RectF(14, 12, 4, 4), rootC.Layout.Bounds);
    }

    [Fact]
    public void wrap_nodes_with_content_sizing_overflowing_margin()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, ElementsWrap = LayoutWrap.Wrap, Width = 20 })
                        .Add(new TestNode(out var rootAa, "B")
                                .Add(new TestNode(out var rootAaa, "C", new LayoutPlan { Width = 10, Height = 6 }))
                            )
                        .Add(new TestNode(out var rootAb, "D", new LayoutPlan { Margin = new Sides(right: 2) })
                                .Add(new TestNode(out var rootAba, "E", new LayoutPlan { Width = 10, Height = 6 }))
                            )
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔════════╗═════════╗═══════════╗
║CCCCCCCC║AAAAAAAAA║░░░░░░░░░░░║
║CCCCCCCC║AAAAAAAAA║░░░░░░░░░░░║
║CCCCCCCC║AAAAAAAAA║░░░░░░░░░░░║
║CCCCCCCC║AAAAAAAAA║░░░░░░░░░░░║
╚════════╝AAAAAAAAA║░░░░░░░░░░░║
╔════════╗AAAAAAAAA║░░░░░░░░░░░║
║EEEEEEEE║AAAAAAAAA║░░░░░░░░░░░║
║EEEEEEEE║AAAAAAAAA║░░░░░░░░░░░║
║EEEEEEEE║AAAAAAAAA║░░░░░░░░░░░║
║EEEEEEEE║AAAAAAAAA║░░░░░░░░░░░║
╚════════╝═════════╝░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 20, 12), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 10, 6), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 10, 6), rootAaa.Layout.Bounds);
        Assert.Equal(new RectF(0, 6, 10, 6), rootAb.Layout.Bounds);
        Assert.Equal(new RectF(0, 6, 10, 6), rootAba.Layout.Bounds);
    }

    [Fact]
    public void wrap_nodes_with_content_sizing_margin_cross()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, ElementsWrap = LayoutWrap.Wrap, Width = 12 })
                        .Add(new TestNode(out var rootAa, "B")
                                .Add(new TestNode(out var rootAaa, "C", new LayoutPlan { Width = 7, Height = 6 }))
                            )
                        .Add(new TestNode(out var rootAb, "D", new LayoutPlan { Margin = new Sides(top: 2) })
                        .Add(new TestNode(out var rootAba, "E", new LayoutPlan { Width = 7, Height = 6 }))
                            )
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔═════╗════╗═══════════════════╗
║CCCCC║AAAA║░░░░░░░░░░░░░░░░░░░║
║CCCCC║AAAA║░░░░░░░░░░░░░░░░░░░║
║CCCCC║AAAA║░░░░░░░░░░░░░░░░░░░║
║CCCCC║AAAA║░░░░░░░░░░░░░░░░░░░║
╚═════╝AAAA║░░░░░░░░░░░░░░░░░░░║
║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
╔═════╗AAAA║░░░░░░░░░░░░░░░░░░░║
║EEEEE║AAAA║░░░░░░░░░░░░░░░░░░░║
║EEEEE║AAAA║░░░░░░░░░░░░░░░░░░░║
║EEEEE║AAAA║░░░░░░░░░░░░░░░░░░░║
║EEEEE║AAAA║░░░░░░░░░░░░░░░░░░░║
╚═════╝════╝░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 12, 14), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 7, 6), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 7, 6), rootAaa.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 7, 6), rootAb.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 7, 6), rootAba.Layout.Bounds);
    }
}
