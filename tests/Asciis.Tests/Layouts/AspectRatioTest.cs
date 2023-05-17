using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class AspectRatioTest : BaseTests
{
    private static MeasureFunc _measure = (node, width, widthMode, height, heightMode) => new VecF(
                                                                                                     widthMode == MeasureMode.Exactly
                                                                                                         ? width
                                                                                                         : 16,
                                                                                                     heightMode == MeasureMode.Exactly
                                                                                                         ? height
                                                                                                         : 8);

    public AspectRatioTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void aspect_ratio_cross_defined()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 16, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════╗═══════════════╗
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
╚══════════════╝═══════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_main_defined()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 8, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_both_dimensions_defined_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 16, Height = 8, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════╗═══════════════╗
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
╚══════════════╝═══════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_both_dimensions_defined_column()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 16, Height = 8, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_align_stretch()
    {
        var root = new TestNode(new LayoutPlan { Width = 16, Height = 16 })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { AspectRatio = 1 })
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════╗
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
║AAAAAAAAAAAAAA║
╚══════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 16, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_flex_grow()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 8, FlexGrow = 1, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════╗═══════════════╗
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
╚══════════════╝═══════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_flex_shrink()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 16, FlexShrink = 1, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════╗═══════════════╗
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
╚══════════════╝═══════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_flex_shrink_2()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 100.Percent(), FlexShrink = 1, AspectRatio = 1 } ),
                           new TestNode( out var rootB, "B", new LayoutPlan { Height = 100.Percent(), FlexShrink = 1, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
║BBBBBB║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝═══════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 8, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 8, 8), rootB.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_main_length()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { ChildLayoutDirectionLength = 8, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_absolute_layout_width_defined()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A",
                                    new LayoutPlan
                                    {
                                        PositionType = PositionType.Absolute,
                                        Left         = 0,
                                        Top          = 0,
                                        Width        = 8,
                                        AspectRatio  = 1
                                    } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_absolute_layout_height_defined()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A",
                                    new LayoutPlan
                                    {
                                        PositionType = PositionType.Absolute,
                                        Left         = 0,
                                        Top          = 0,
                                        Height       = 8,
                                        AspectRatio  = 1
                                    } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_with_max_cross_defined()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 8, MaxWidth = 6, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔════╗═════════════════════════╗
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
╚════╝░░░░░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 6, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_with_max_main_defined()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 8, MaxHeight = 6, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔════╗═════════════════════════╗
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
╚════╝░░░░░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 6, 6), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_with_min_cross_defined()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 6, MinWidth = 8, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 8, 6), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_with_min_main_defined()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 6, MinHeight = 8, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_double_cross()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 8, AspectRatio = 2 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════╗═══════════════╗
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
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_half_cross()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 16, AspectRatio = 0.5f } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝═══════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 8, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_double_main()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 8, AspectRatio = 0.5f } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝═══════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 8, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_half_main()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 16, AspectRatio = 2 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════╗═══════════════╗
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
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_with_measure_func()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { AspectRatio = 1 } ) { MeasureFunc = _measure }
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_width_height_flex_grow_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 8, Height = 8, FlexGrow = 1, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
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
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 32), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_width_height_flex_grow_column()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 8, Height = 8, FlexGrow = 1, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════╗═══════════════╗
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░░║
╚══════════════╝═══════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_height_as_main_length()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 6, FlexGrow  = 1, AspectRatio = 1 } ),
                           new TestNode( out var rootB, "B", new LayoutPlan { Height = 8, FlexGrow = 1, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔═════════════╗╔═══════════════╗
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBB║
╚═════════════╝║BBBBBBBBBBBBBBB║
╚══════════════║BBBBBBBBBBBBBBB║",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 15, 15), rootA.Layout.Bounds);
        Assert.Equal(new RectF(15, 0, 17, 17), rootB.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_width_as_main_length()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 4, FlexGrow  = 1, AspectRatio = 1 } ),
                           new TestNode( out var rootB, "B", new LayoutPlan { Width = 8, FlexGrow = 1, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔════╗═════════════════════════╗
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
║AAAA║░░░░░░░░░░░░░░░░░░░░░░░░░║
╚════╝░░░░░░░░░░░░░░░░░░░░░░░░░║
╔════════╗░░░░░░░░░░░░░░░░░░░░░║
║BBBBBBBB║░░░░░░░░░░░░░░░░░░░░░║
║BBBBBBBB║░░░░░░░░░░░░░░░░░░░░░║
║BBBBBBBB║░░░░░░░░░░░░░░░░░░░░░║
║BBBBBBBB║░░░░░░░░░░░░░░░░░░░░░║
║BBBBBBBB║░░░░░░░░░░░░░░░░░░░░░║
║BBBBBBBB║░░░░░░░░░░░░░░░░░░░░░║
║BBBBBBBB║░░░░░░░░░░░░░░░░░░░░░║
║BBBBBBBB║░░░░░░░░░░░░░░░░░░░░░║
╚════════╝═════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 6, 6), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 6, 10, 10), rootB.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_overrides_flex_grow_row()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 8, FlexGrow = 1, AspectRatio = 0.5f } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
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
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 64), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_overrides_flex_grow_column()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 8, FlexGrow = 1, AspectRatio = 2 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
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
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_left_right_absolute()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A",
                                    new LayoutPlan
                                    {
                                        PositionType = PositionType.Absolute,
                                        Left         = 8,
                                        Top          = 2,
                                        Right        = 8,
                                        AspectRatio  = 1
                                    } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░╔══════════════╗░░░░░░░║
║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
╚═══════║AAAAAAAAAAAAAA║═══════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(8, 2, 16, 16), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_top_bottom_absolute()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A",
                                    new LayoutPlan
                                    {
                                        PositionType = PositionType.Absolute,
                                        Left         = 2,
                                        Top          = 2,
                                        Bottom       = 2,
                                        AspectRatio  = 1
                                    } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░╔══════════╗░░░░░░░░░░░░░░░░░║
║░║AAAAAAAAAA║░░░░░░░░░░░░░░░░░║
║░║AAAAAAAAAA║░░░░░░░░░░░░░░░░░║
║░║AAAAAAAAAA║░░░░░░░░░░░░░░░░░║
║░║AAAAAAAAAA║░░░░░░░░░░░░░░░░░║
║░║AAAAAAAAAA║░░░░░░░░░░░░░░░░░║
║░║AAAAAAAAAA║░░░░░░░░░░░░░░░░░║
║░║AAAAAAAAAA║░░░░░░░░░░░░░░░░░║
║░║AAAAAAAAAA║░░░░░░░░░░░░░░░░░║
║░║AAAAAAAAAA║░░░░░░░░░░░░░░░░░║
║░║AAAAAAAAAA║░░░░░░░░░░░░░░░░░║
║░╚══════════╝░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(2, 2, 12, 12), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_width_overrides_align_stretch_row()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 8, AspectRatio = 1 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_height_overrides_align_stretch_column()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Height = 8, AspectRatio = 1 })
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════╗═══════════════════════╗
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_allow_element_overflow_container_size()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Align_Cross = AlignmentLine_Cross.Start })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 8, AspectRatio = 4 } )
                       };

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            @"╔══════════════════════════════╗
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
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
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), rootA.Layout.Bounds);

    }

    [Fact]
    public void aspect_ratio_defined_main_with_margin()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Height = 8, AspectRatio = 1, Margin = new Sides( 2, right: 2 ) } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(12, 4, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_defined_cross_with_margin()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 8, AspectRatio = 1, Margin = new Sides( 4, right: 4 ) } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(12, 4, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void aspect_ratio_defined_cross_with_main_margin()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 8, AspectRatio = 1, Margin = new Sides( top: 2, bottom: 2 ) } )
                       };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░╔══════╗░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░║AAAAAA║░░░░░░░░░░░║
║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(12, 4, 8, 8), rootA.Layout.Bounds);
    }
}
