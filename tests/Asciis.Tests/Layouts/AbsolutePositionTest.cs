using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

/*

    Draw(root);
    Assert.Equal(
        @"",
        Canvas!.ToString());

    Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
*/

[TestClass]
public class AbsolutePositionTest : BaseTests
{
    public AbsolutePositionTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void absolute_layout_width_height_start_top()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A",
                                    new LayoutPlan
                                    {
                                        PositionType = PositionType.Absolute,
                                        Start        = 4,
                                        Top          = 2,
                                        Width        = 8,
                                        Height       = 4
                                    } )
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(4, 2, 8, 4), rootA.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░╔══════╗░░░░░░░░░░░░░░░░░░░║
            ║░░░║AAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║░░░║AAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║░░░╚══════╝░░░░░░░░░░░░░░░░░░░║
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
    public void absolute_layout_width_height_end_bottom()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A",
                                    new LayoutPlan
                                    {
                                        PositionType = PositionType.Absolute,
                                        End          = 4,
                                        Bottom       = 4,
                                        Width        = 16,
                                        Height       = 8
                                    } )
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(12, 4, 16, 8), rootA.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════════════╗░░░║
            ║░░░░░░░░░░░║AAAAAAAAAAAAAA║░░░║
            ║░░░░░░░░░░░║AAAAAAAAAAAAAA║░░░║
            ║░░░░░░░░░░░║AAAAAAAAAAAAAA║░░░║
            ║░░░░░░░░░░░║AAAAAAAAAAAAAA║░░░║
            ║░░░░░░░░░░░║AAAAAAAAAAAAAA║░░░║
            ║░░░░░░░░░░░║AAAAAAAAAAAAAA║░░░║
            ║░░░░░░░░░░░╚══════════════╝░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());
    }

    [Fact]
    public void absolute_layout_start_top_end_bottom()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A",
                                    new LayoutPlan
                                    {
                                        PositionType = PositionType.Absolute,
                                        Start        = 4,
                                        Top          = 2,
                                        End          = 4,
                                        Bottom       = 2
                                    } )
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(4, 2, 24, 12), rootA.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░╔══════════════════════╗░░░║
            ║░░░║AAAAAAAAAAAAAAAAAAAAAA║░░░║
            ║░░░║AAAAAAAAAAAAAAAAAAAAAA║░░░║
            ║░░░║AAAAAAAAAAAAAAAAAAAAAA║░░░║
            ║░░░║AAAAAAAAAAAAAAAAAAAAAA║░░░║
            ║░░░║AAAAAAAAAAAAAAAAAAAAAA║░░░║
            ║░░░║AAAAAAAAAAAAAAAAAAAAAA║░░░║
            ║░░░║AAAAAAAAAAAAAAAAAAAAAA║░░░║
            ║░░░║AAAAAAAAAAAAAAAAAAAAAA║░░░║
            ║░░░║AAAAAAAAAAAAAAAAAAAAAA║░░░║
            ║░░░║AAAAAAAAAAAAAAAAAAAAAA║░░░║
            ║░░░╚══════════════════════╝░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

    }

    [Fact]
    public void absolute_layout_width_height_start_top_end_bottom()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A",
                                    new LayoutPlan
                                    {
                                        PositionType = PositionType.Absolute,
                                        Start        = 4,
                                        Top          = 2,
                                        End          = 4,
                                        Bottom       = 2,
                                        Width        = 8,
                                        Height       = 4
                                    } )
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(4, 2, 8, 4), rootA.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░╔══════╗░░░░░░░░░░░░░░░░░░░║
            ║░░░║AAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║░░░║AAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║░░░╚══════╝░░░░░░░░░░░░░░░░░░░║
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
    public void do_not_clamp_height_of_absolute_node_to_height_of_its_overflow_hidden_container()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Overflow = Overflow.Hidden, Width = 16, Height = 8 })
                       {
                           new TestNode( out var rootA, "A",
                                    new LayoutPlan { PositionType = PositionType.Absolute, Start = 0, Top = 0 } )
                           {
                               new TestNode( out var rootAb, "b", new LayoutPlan { Width = 32, Height = 16 } )
                           }
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 16, 8), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 16), rootAb.Layout.Bounds);

        Draw(root, 16, 8);
        Assert.Equal(
            """
            ╔═══════════════
            ║bbbbbbbbbbbbbbb
            ║bbbbbbbbbbbbbbb
            ║bbbbbbbbbbbbbbb
            ║bbbbbbbbbbbbbbb
            ║bbbbbbbbbbbbbbb
            ║bbbbbbbbbbbbbbb
            ║bbbbbbbbbbbbbbb
            """,
            Canvas!.ToString());

    }

    [Fact]
    public void absolute_layout_within()
    {
        new TestNode(out var root,
                 new LayoutPlan
                 {
                     Width = 32,
                     Height = 16,
                     Margin = 4,
                     Padding = 2
                 })
            {
                new TestNode( out var rootA, "A",
                         new LayoutPlan
                         {
                             PositionType = PositionType.Absolute,
                             Left         = 0,
                             Top          = 0,
                             Width        = 8,
                             Height       = 4
                         } ),
                new TestNode( out var rootB, "B",
                         new LayoutPlan
                         {
                             PositionType = PositionType.Absolute,
                             Right        = 0,
                             Bottom       = 0,
                             Width        = 8,
                             Height       = 4
                         } ),
                new TestNode( out var rootC, "C",
                         new LayoutPlan
                         {
                             PositionType = PositionType.Absolute,
                             Left         = 0,
                             Top          = 0,
                             Width        = 8,
                             Height       = 4,
                             Margin       = 3
                         } ),
                new TestNode( out var rootD, "D",
                         new LayoutPlan
                         {
                             PositionType = PositionType.Absolute,
                             Right        = 0,
                             Bottom       = 0,
                             Width        = 8,
                             Height       = 4,
                             Margin       = 3
                         } )
            };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(4, 4, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(4, 4, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(28, 16, 8, 4), rootB.Layout.Bounds);
        Assert.Equal(new RectF(7, 7, 8, 4), rootC.Layout.Bounds);
        Assert.Equal(new RectF(25, 13, 8, 4), rootD.Layout.Bounds);

        Draw(root, 40, 24);
        Assert.Equal(
            """




                ╔══════╗───────────────────────┐
                ║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░│
                ║AAAAAA║═════════════════════╗░│
                ╚══╔══════╗░░░░░░░░░░░░░░░░░░║░│
                │░║║CCCCCC║░░░░░░░░░░░░░░░░░░║░│
                │░║║CCCCCC║░░░░░░░░░░░░░░░░░░║░│
                │░║╚══════╝░░░░░░░░░░░░░░░░░░║░│
                │░║░░░░░░░░░░░░░░░░░░░░░░░░░░║░│
                │░║░░░░░░░░░░░░░░░░░░░░░░░░░░║░│
                │░║░░░░░░░░░░░░░░░░░░╔══════╗║░│
                │░║░░░░░░░░░░░░░░░░░░║DDDDDD║║░│
                │░║░░░░░░░░░░░░░░░░░░║DDDDDD║║░│
                │░║░░░░░░░░░░░░░░░░░░╚══════╝══╗
                │░╚═════════════════════║BBBBBB║
                │░░░░░░░░░░░░░░░░░░░░░░░║BBBBBB║
                └───────────────────────╚══════╝
            """,
            Canvas!.ToString());

    }

    [Fact]
    public void absolute_layout_align_items_and_justify_content_center()
    {
        var root = new TestNode(
                 new LayoutPlan
                 {
                     AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center,
                     Align_Cross = AlignmentLine_Cross.Center,
                     Width = 32,
                     Height = 16
                 })
            {
                new TestNode( out var rootA, "A", 
                         new LayoutPlan { PositionType = PositionType.Absolute, Width = 16, Height = 8 } )
            };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(8, 4, 16, 8), rootA.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░╔══════════════╗░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░╚══════════════╝░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

    }

    [Fact]
    public void absolute_layout_align_items_and_justify_content_flex_end()
    {
        new TestNode(out var root,
                 new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.End, Align_Cross = AlignmentLine_Cross.End, Width = 32, Height = 16 })
            {
                new TestNode( out var rootA, "A", 
                         new LayoutPlan { PositionType = PositionType.Absolute, Width = 16, Height = 8 } )
            };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(16, 8, 16, 8), rootA.Layout.Bounds);


        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(32, root.Layout.Width);
        Assert.Equal(16, root.Layout.Height);

        Assert.Equal(16, rootA.Layout.Left);
        Assert.Equal(8, rootA.Layout.Top);
        Assert.Equal(16, rootA.Layout.Width);
        Assert.Equal(8, rootA.Layout.Height);

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
            ║░░░░░░░░░░░░░░░╔══════════════╗
            ║░░░░░░░░░░░░░░░║AAAAAAAAAAAAAA║
            ║░░░░░░░░░░░░░░░║AAAAAAAAAAAAAA║
            ║░░░░░░░░░░░░░░░║AAAAAAAAAAAAAA║
            ║░░░░░░░░░░░░░░░║AAAAAAAAAAAAAA║
            ║░░░░░░░░░░░░░░░║AAAAAAAAAAAAAA║
            ║░░░░░░░░░░░░░░░║AAAAAAAAAAAAAA║
            ╚═══════════════╚══════════════╝
            """,
            Canvas!.ToString());

    }

    [Fact]
    public void absolute_layout_justify_content_center()
    {
        new TestNode(out var root, new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 32, Height = 16 })
            {
                new TestNode( out var rootA, "A", new LayoutPlan { PositionType = PositionType.Absolute, Width = 16, Height = 8 } )
            };

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(32, root.Layout.Width);
        Assert.Equal(16, root.Layout.Height);

        Assert.Equal(0, rootA.Layout.Left);
        Assert.Equal(4, rootA.Layout.Top);
        Assert.Equal(16, rootA.Layout.Width);
        Assert.Equal(8, rootA.Layout.Height);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
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
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

    }

    [Fact]
    public void absolute_layout_align_items_center()
    {
        new TestNode(out var root, new LayoutPlan { Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
            {
                new TestNode( out var rootA, "A", new LayoutPlan { PositionType = PositionType.Absolute, Width = 16, Height = 8 } )
            };

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(32, root.Layout.Width);
        Assert.Equal(16, root.Layout.Height);

        Assert.Equal(8, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.Equal(16, rootA.Layout.Width);
        Assert.Equal(8, rootA.Layout.Height);

        Draw(root);
        Assert.Equal(
            """
            ╔═══════╔══════════════╗═══════╗
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░╚══════════════╝░░░░░░░║
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
    public void absolute_layout_align_items_center_on_element_only()
    {
        new TestNode(out var root,
                 new LayoutPlan { Width = 32, Height = 16 })
            {
                new TestNode( out var rootA, "A",
                         new LayoutPlan { AlignSelf_Cross = AlignmentLine_Cross.Center, PositionType = PositionType.Absolute, Width = 16, Height = 8 } )
            };

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(32, root.Layout.Width);
        Assert.Equal(16, root.Layout.Height);

        Assert.Equal(8, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.Equal(16, rootA.Layout.Width);
        Assert.Equal(8, rootA.Layout.Height);

        Draw(root);
        Assert.Equal(
            """
            ╔═══════╔══════════════╗═══════╗
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░╚══════════════╝░░░░░░░║
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
    public void absolute_layout_align_items_and_justify_content_center_and_top_position()
    {
        new TestNode(out var root,
                 new LayoutPlan
                 {
                     AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center,
                     Align_Cross = AlignmentLine_Cross.Center,
                     FlexGrow = 1,
                     Width = 32,
                     Height = 16
                 })
            {
                new TestNode( out var rootA, "A",
                         new LayoutPlan { PositionType = PositionType.Absolute, Width = 16, Height = 8, Top = 2 } )
            };

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(32, root.Layout.Width);
        Assert.Equal(16, root.Layout.Height);

        Assert.Equal(8, rootA.Layout.Left);
        Assert.Equal(2, rootA.Layout.Top);
        Assert.Equal(16, rootA.Layout.Width);
        Assert.Equal(8, rootA.Layout.Height);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░╔══════════════╗░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░╚══════════════╝░░░░░░░║
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
    public void absolute_layout_align_items_and_justify_content_center_and_bottom_position()
    {
        new TestNode(out var root,
                 new LayoutPlan
                 {
                     AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center,
                     Align_Cross = AlignmentLine_Cross.Center,
                     FlexGrow = 1,
                     Width = 32,
                     Height = 16
                 })
            {
                new TestNode( out var rootA, "A",
                         new LayoutPlan { PositionType = PositionType.Absolute, Bottom = 2, Width = 16, Height = 8 } )
            };

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(32, root.Layout.Width);
        Assert.Equal(16, root.Layout.Height);

        Assert.Equal(8, rootA.Layout.Left);
        Assert.Equal(6, rootA.Layout.Top);
        Assert.Equal(16, rootA.Layout.Width);
        Assert.Equal(8, rootA.Layout.Height);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░╔══════════════╗░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░║AAAAAAAAAAAAAA║░░░░░░░║
            ║░░░░░░░╚══════════════╝░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

    }

    [Fact]
    public void absolute_layout_align_items_and_justify_content_center_and_left_position()
    {
        new TestNode(out var root,
                 new LayoutPlan
                 {
                     AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center,
                     Align_Cross = AlignmentLine_Cross.Center,
                     FlexGrow = 1,
                     Width = 32,
                     Height = 16
                 })
            {
                new TestNode( out var rootA, "A", 
                         new LayoutPlan { PositionType = PositionType.Absolute, Left = 4, Width = 16, Height = 8 } )
            };

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(32, root.Layout.Width);
        Assert.Equal(16, root.Layout.Height);

        Assert.Equal(4, rootA.Layout.Left);
        Assert.Equal(4, rootA.Layout.Top);
        Assert.Equal(16, rootA.Layout.Width);
        Assert.Equal(8, rootA.Layout.Height);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░╔══════════════╗░░░░░░░░░░░║
            ║░░░║AAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║░░░║AAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║░░░║AAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║░░░║AAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║░░░║AAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║░░░║AAAAAAAAAAAAAA║░░░░░░░░░░░║
            ║░░░╚══════════════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());


    }

    [Fact]
    public void absolute_layout_align_items_and_justify_content_center_and_right_position()
    {
        new TestNode(out var root,
                 new LayoutPlan
                 {
                     AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center,
                     Align_Cross = AlignmentLine_Cross.Center,
                     FlexGrow = 1,
                     Width = 32,
                     Height = 16
                 })
            {
                new TestNode( out var rootA, "A", 
                         new LayoutPlan { PositionType = PositionType.Absolute, Right = 4, Width = 16, Height = 8 } )
            };

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(32, root.Layout.Width);
        Assert.Equal(16, root.Layout.Height);

        Assert.Equal(12, rootA.Layout.Left);
        Assert.Equal(4, rootA.Layout.Top);
        Assert.Equal(16, rootA.Layout.Width);
        Assert.Equal(8, rootA.Layout.Height);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░╔══════════════╗░░░║
            ║░░░░░░░░░░░║AAAAAAAAAAAAAA║░░░║
            ║░░░░░░░░░░░║AAAAAAAAAAAAAA║░░░║
            ║░░░░░░░░░░░║AAAAAAAAAAAAAA║░░░║
            ║░░░░░░░░░░░║AAAAAAAAAAAAAA║░░░║
            ║░░░░░░░░░░░║AAAAAAAAAAAAAA║░░░║
            ║░░░░░░░░░░░║AAAAAAAAAAAAAA║░░░║
            ║░░░░░░░░░░░╚══════════════╝░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

    }

    [Fact]
    public void position_root_with_rtl_should_position_without_direction()
    {
        new TestNode(out var root, new LayoutPlan { Left = 4, Width = 8, Height = 8 });

        ElementArrange.Calculate(root);

        Assert.Equal(4, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(8, root.Layout.Width);
        Assert.Equal(8, root.Layout.Height);

        Draw(root);
        Assert.Equal(
            """
                ╔══════╗
                ║░░░░░░║
                ║░░░░░░║
                ║░░░░░░║
                ║░░░░░░║
                ║░░░░░░║
                ║░░░░░░║
                ╚══════╝
            """,
            Canvas!.ToString());

    }

    [Fact]
    public void absolute_layout_percentage_bottom_based_on_container_height()
    {
        new TestNode(out var root, new LayoutPlan { Width = 32, Height = 16 })
            {
                new TestNode( out var rootA, "A", new LayoutPlan { PositionType = PositionType.Absolute, Top = 50.Percent(), Left=2, Width = 8, Height = 4 } ),
                new TestNode( out var rootB, "B", new LayoutPlan { PositionType = PositionType.Absolute, Bottom = 50.Percent(), Left = 12, Width = 8, Height = 4 } ),
                new TestNode( out var rootC, "C", new LayoutPlan { PositionType = PositionType.Absolute, Top = 10.Percent(), Left = 22, Bottom = 10.Percent(), Width = 8 } )
            };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(2, 8, 8, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(12, 4, 8, 4), rootB.Layout.Bounds);
        Assert.Equal(new RectF(22, 2, 8, 12), rootC.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░╔══════╗░║
            ║░░░░░░░░░░░░░░░░░░░░░║CCCCCC║░║
            ║░░░░░░░░░░░╔══════╗░░║CCCCCC║░║
            ║░░░░░░░░░░░║BBBBBB║░░║CCCCCC║░║
            ║░░░░░░░░░░░║BBBBBB║░░║CCCCCC║░║
            ║░░░░░░░░░░░╚══════╝░░║CCCCCC║░║
            ║░╔══════╗░░░░░░░░░░░░║CCCCCC║░║
            ║░║AAAAAA║░░░░░░░░░░░░║CCCCCC║░║
            ║░║AAAAAA║░░░░░░░░░░░░║CCCCCC║░║
            ║░╚══════╝░░░░░░░░░░░░║CCCCCC║░║
            ║░░░░░░░░░░░░░░░░░░░░░║CCCCCC║░║
            ║░░░░░░░░░░░░░░░░░░░░░╚══════╝░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

    }
}
