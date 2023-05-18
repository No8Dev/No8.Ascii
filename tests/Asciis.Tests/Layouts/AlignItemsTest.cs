using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class AlignItemsTest : BaseTests
{
    public AlignItemsTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void align_items_stretch()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Height = 12 })
                   };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 12), rootA.Layout.Bounds);

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
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝
""",
            Canvas!.ToString());
    }

    [Fact]
    public void align_items_center()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 12, Height = 12 } )
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(10, 0, 12, 12), rootA.Layout.Bounds);

        Draw(root);
        Assert.Equal(
"""
╔═════════╔══════════╗═════════╗
║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
║░░░░░░░░░╚══════════╝░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝
""",
            Canvas!.ToString());
    }

    [Fact]
    public void align_items_flex_start()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 12, Height = 12 } )
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 12, 12), rootA.Layout.Bounds);

        Draw(root);
        Assert.Equal(
"""
╔══════════╗═══════════════════╗
║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
╚══════════╝░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝
""",
            Canvas!.ToString());
    }

    [Fact]
    public void align_items_flex_end()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.End, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Width = 12, Height = 12 } )
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(20, 0, 12, 12), rootA.Layout.Bounds);

        Draw(root);
        Assert.Equal(
"""
╔═══════════════════╔══════════╗
║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
║░░░░░░░░░░░░░░░░░░░╚══════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝
""",
            Canvas!.ToString());
    }

    [Fact]
    public void align_items_center_element_with_margin_bigger_than_container()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Align_Cross = AlignmentLine_Cross.Center, Width = 12, Height = 12 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center } )
                           {
                               new TestNode( out var rootAa, "B", new LayoutPlan { Width = 12, Height = 12, Margin = new Sides( 2, right: 2 ) } )
                           }
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 12, 12), root.Layout.Bounds);
        Assert.Equal(new RectF(-2, 0, 16, 12), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 12, 12), rootAa.Layout.Bounds);

        Draw(root, 12, 12);
        Assert.Equal(
"""
╔══════════╗
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
╚══════════╝
""",
            Canvas!.ToString());
    }

    [Fact]
    public void align_items_flex_end_element_with_margin_bigger_than_container()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Align_Cross = AlignmentLine_Cross.Center, Width = 12, Height = 12 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Align_Cross = AlignmentLine_Cross.End } )
                           {
                               new TestNode( out var rootAa, "B", new LayoutPlan { Width = 12, Height = 12, Margin = new Sides( 2, right: 2 ) } )
                           }
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 12, 12), root.Layout.Bounds);
        Assert.Equal(new RectF(-2, 0, 16, 12), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 12, 12), rootAa.Layout.Bounds);

        Draw(root, 12, 12);
        Assert.Equal(
"""
╔══════════╗
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
║BBBBBBBBBB║
╚══════════╝
""",
            Canvas!.ToString());
    }

    [Fact]
    public void align_items_center_element_without_margin_bigger_than_container()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Align_Cross = AlignmentLine_Cross.Center, Width = 12, Height = 12 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center } )
                           {
                               new TestNode( out var rootAa, "B", new LayoutPlan { Width = 16, Height = 16 } )
                           }
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 12, 12), root.Layout.Bounds);
        Assert.Equal(new RectF(-2, -2, 16, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(-2, -2, 16, 16), rootAa.Layout.Bounds);

        Draw(root, 12, 12);
        Assert.Equal(
"""
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
""",
            Canvas!.ToString());
    }

    [Fact]
    public void align_items_flex_end_element_without_margin_bigger_than_container()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Align_Cross = AlignmentLine_Cross.Center, Width = 12, Height = 12 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Align_Cross = AlignmentLine_Cross.End } )
                           {
                               new TestNode( out var rootAa, "B", new LayoutPlan { Width = 16, Height = 16 } )
                           }
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 12, 12), root.Layout.Bounds);
        Assert.Equal(new RectF(-2, -2, 16, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(-2, -2, 16, 16), rootAa.Layout.Bounds);

        Draw(root, 12, 12);
        Assert.Equal(
"""
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
BBBBBBBBBBBB
""",
            Canvas!.ToString());
    }

    [Fact]
    public void align_center_should_size_based_on_content()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16, Padding = new Sides(top: 2) })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, FlexShrink = 1 } )
                           {
                               new TestNode( out var rootAa, "B", new LayoutPlan { FlexGrow = 1, FlexShrink = 1 } )
                               {
                                   new TestNode( out var rootAaa, "C", new LayoutPlan { Width = 8, Height = 8 } )
                               }
                           }
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(12, 2, 8, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 2, 8, 8), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(12, 2, 8, 8), rootAaa.Layout.Bounds);

        //TODO: This look wrong

        Draw(root);
        Assert.Equal(
"""
┌──────────────────────────────┐
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
╔═══════════╔══════╗═══════════╗
║░░░░░░░░░░░║CCCCCC║░░░░░░░░░░░║
║░░░░░░░░░░░║CCCCCC║░░░░░░░░░░░║
║░░░░░░░░░░░║CCCCCC║░░░░░░░░░░░║
║░░░░░░░░░░░║CCCCCC║░░░░░░░░░░░║
║░░░░░░░░░░░║CCCCCC║░░░░░░░░░░░║
║░░░░░░░░░░░║CCCCCC║░░░░░░░░░░░║
║░░░░░░░░░░░╚══════╝░░░░░░░░░░░║
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
    public void align_strech_should_size_based_on_container()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16, Padding = new Sides(top: 2) })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, FlexShrink = 1 } )
                           {
                               new TestNode( out var rootAa, "B", new LayoutPlan { FlexGrow = 1, FlexShrink = 1 } )
                               {
                                   new TestNode( out var rootAaa, "C", new LayoutPlan { Width = 8, Height = 4 } )
                               }
                           }
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 2, 32, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 2, 32, 4), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 2, 8, 4), rootAaa.Layout.Bounds);

        Draw(root);
        Assert.Equal(
"""
┌──────────────────────────────┐
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
╔══════╗═══════════════════════╗
║CCCCCC║BBBBBBBBBBBBBBBBBBBBBBB║
║CCCCCC║BBBBBBBBBBBBBBBBBBBBBBB║
╚══════╝═══════════════════════╝
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
    public void align_flex_start_with_shrinking_elements()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start } )
                           {
                               new TestNode( out var rootAa, "B", new LayoutPlan { FlexGrow = 1, FlexShrink = 1 } )
                               {
                                   new TestNode( out var rootAaa, "C", new LayoutPlan { FlexGrow = 1, FlexShrink = 1 } )
                               }
                           }
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 0), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 0, 0), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 0, 0), rootAaa.Layout.Bounds);

        Draw(root);
        Assert.Equal(
"""
CAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
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
    }

    [Fact]
    public void align_flex_start_with_stretching_elements()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A" )
                           {
                               new TestNode( out var rootAa, "B", new LayoutPlan { FlexGrow = 1, FlexShrink = 1 } )
                               {
                                   new TestNode( out var rootAaa, "C", new LayoutPlan { FlexGrow = 1, FlexShrink = 1 } )
                               }
                           }
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 0), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 0), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 0), rootAaa.Layout.Bounds);

        Draw(root);
        Assert.Equal(
"""
CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC
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
    }

    [Fact]
    public void align_flex_start_with_shrinking_elements_with_stretch()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start } )
                           {
                               new TestNode( out var rootAa, "B", new LayoutPlan { FlexGrow = 1, FlexShrink = 1 } )
                               {
                                   new TestNode( out var rootAaa, "C", new LayoutPlan { FlexGrow = 1, FlexShrink = 1 } )
                               }
                           }
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 0), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 0, 0), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 0, 0), rootAaa.Layout.Bounds);

        Draw(root);
        Assert.Equal(
"""
CAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
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
    }
}
