using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class RoundingTest : BaseTests
{
    public RoundingTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void rounding_main_length_flex_grow_row_width_of_100()
    {
        var root =
            new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1 }),
                new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1 }),
                new TestNode(out var rootC, "C", new LayoutPlan { FlexGrow = 1 })
            };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═════════╗╔════════╗╔═════════╗
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ║AAAAAAAAA║║BBBBBBBB║║CCCCCCCCC║
            ╚═════════╝╚════════╝╚═════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 11, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(11, 0, 10, 16), rootB.Layout.Bounds);
        Assert.Equal(new RectF(21, 0, 11, 16), rootC.Layout.Bounds);
    }

    [Fact]
    public void rounding_main_length_flex_grow_row_prime_number_width()
    {
        var root =
            new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, MinWidth = 32, MinHeight = 16 })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1 }),
                new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1 }),
                new TestNode(out var rootC, "C", new LayoutPlan { FlexGrow = 1 }),
                new TestNode(out var rootD, "D", new LayoutPlan { FlexGrow = 1 }),
                new TestNode(out var rootE, "E", new LayoutPlan { FlexGrow = 1 }),
            };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔════╗╔═════╗╔════╗╔═════╗╔════╗
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ║AAAA║║BBBBB║║CCCC║║DDDDD║║EEEE║
            ╚════╝╚═════╝╚════╝╚═════╝╚════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 6, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(6, 0, 7, 16), rootB.Layout.Bounds);
        Assert.Equal(new RectF(13, 0, 6, 16), rootC.Layout.Bounds);
        Assert.Equal(new RectF(19, 0, 7, 16), rootD.Layout.Bounds);
        Assert.Equal(new RectF(26, 0, 6, 16), rootE.Layout.Bounds);
    }

    [Fact]
    public void rounding_main_length_flex_shrink_row()
    {
        var root =
            new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexShrink      = 1, ChildLayoutDirectionLength = 31 }),
                new TestNode(out var rootB, "B", new LayoutPlan { ChildLayoutDirectionLength = 4 }),
                new TestNode(out var rootC, "C", new LayoutPlan { ChildLayoutDirectionLength = 4 })
            };
        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════╗╔══╗╔══╗
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ║AAAAAAAAAAAAAAAAAAAAAA║║BB║║CC║
            ╚══════════════════════╝╚══╝╚══╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 24, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(24, 0, 4, 16), rootB.Layout.Bounds);
        Assert.Equal(new RectF(28, 0, 4, 16), rootC.Layout.Bounds);
    }

    [Fact]
    public void rounding_main_length_overrides_main_size()
    {
        var root =
            new TestNode(new LayoutPlan { Width = 32, Height = 16 })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 8, Height = 4 }),
                new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, Height          = 4 }),
                new TestNode(out var rootC, "C", new LayoutPlan { FlexGrow = 1, Height          = 4 })
            };

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
    public void rounding_total_fractal()
    {
        var root =
            new TestNode(new LayoutPlan { Width = 32.4f, Height = 16.4f })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 0.7f, ChildLayoutDirectionLength = 6.3f, Height = 4.3f }),
                new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1.6f, Height = 4 }),
                new TestNode(out var rootC, "C", new LayoutPlan { FlexGrow = 1.1f, Height = 4.7f })
            };

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
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 7), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 7, 32, 4), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 11, 32, 5), rootC.Layout.Bounds);
    }

    [Fact]
    public void rounding_total_fractal_nested()
    {
        var root =
            new TestNode(new LayoutPlan { Width = 32.4f, Height = 16.4f })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 0.7f, ChildLayoutDirectionLength = 8.3f, Height = 8.3f })
                {
                    new TestNode(out var rootAa, "B", new LayoutPlan { FlexGrow = 1f, ChildLayoutDirectionLength = 0.3f, Bottom = 3.3f, Height = 1.9f }),
                    new TestNode(out var rootAb, "C", new LayoutPlan { FlexGrow = 4f, ChildLayoutDirectionLength = 0.3f, Top = 3.3f, Height = 1.1f })
                },
                new TestNode(out var rootB, "D", new LayoutPlan { FlexGrow = 1.6f, Height = 1f }),
                new TestNode(out var rootC, "E", new LayoutPlan { FlexGrow = 1.1f, Height = 1.7f })
            };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╔══════════════════════════════╗
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ╔══════════════════════════════╗
            ║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
            ║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║EEEEEEEEEEEEEEEEEEEEEEEEEEEEEE║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 9), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, -3, 32, 2), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 5, 32, 8), rootAb.Layout.Bounds);
        Assert.Equal(new RectF(0, 9, 32, 4), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 13, 32, 3), rootC.Layout.Bounds);
    }

    [Fact]
    public void rounding_fractal_input_1()
    {
        var root =
            new TestNode(new LayoutPlan { Width = 32, Height = 16.4f })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 6, Height = 4 }),
                new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, Height          = 4 }),
                new TestNode(out var rootC, "C", new LayoutPlan { FlexGrow = 1, Height          = 4 })
            };

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
            ╔══════════════════════════════╗
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

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 7), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 7, 32, 5), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 12, 32, 4), rootC.Layout.Bounds);
    }

    [Fact]
    public void rounding_fractal_input_2()
    {
        var root =
            new TestNode(new LayoutPlan { Width = 32, Height = 16.6f })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 5, Height = 4 }),
                new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, Height          = 4 }),
                new TestNode(out var rootC, "C", new LayoutPlan { FlexGrow = 1, Height          = 4 })
            };

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 17), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 6), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 6, 32, 5), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 11, 32, 6), rootC.Layout.Bounds);
    }

    [Fact]
    public void rounding_fractal_input_3()
    {
        var root =
            new TestNode(new LayoutPlan { Top = 0.3f, Width = 32, Height = 16.4f })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 6, Height = 4 }),
                new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, Height          = 4 }),
                new TestNode(out var rootC, "C", new LayoutPlan { FlexGrow = 1, Height          = 4 })
            };

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
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
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 17), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 7), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 7, 32, 5), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 12, 32, 5), rootC.Layout.Bounds);
    }

    [Fact]
    public void rounding_fractal_input_4()
    {
        var root =
            new TestNode(new LayoutPlan { Top = 0.7f, Width = 32, Height = 16.4f })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 6, Height = 4 }),
                new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, Height          = 4 }),
                new TestNode(out var rootC, "C", new LayoutPlan { FlexGrow = 1, Height          = 4 })
            };

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            """

            ╔══════════════════════════════╗
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
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════════════════════════╗
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            ║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 1, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 1, 32, 7), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 4), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 13, 32, 5), rootC.Layout.Bounds);
    }

    [Fact]
    public void rounding_inner_node_controversy_horizontal()
    {
        var root =
            new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32 })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Height = 4 }),
                new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, Height = 4 })
                {
                    new TestNode(out var rootBa, "C", new LayoutPlan { FlexGrow = 1, Height = 4 })
                },
                new TestNode(out var rootC, "D", new LayoutPlan { FlexGrow = 1, Height = 4 })
            };

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            """
            ╔═════════╗╔════════╗╔═════════╗
            ║AAAAAAAAA║║CCCCCCCC║║DDDDDDDDD║
            ║AAAAAAAAA║║CCCCCCCC║║DDDDDDDDD║
            ╚═════════╝╚════════╝╚═════════╝
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
        Assert.Equal(new RectF(0, 0, 11, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(11, 0, 10, 4), rootB.Layout.Bounds);
        Assert.Equal(new RectF(11, 0, 10, 4), rootBa.Layout.Bounds);
        Assert.Equal(new RectF(21, 0, 11, 4), rootC.Layout.Bounds);
    }

    [Fact]
    public void rounding_inner_node_controversy_vertical()
    {
        var root =
            new TestNode(new LayoutPlan { Height = 16 })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Width = 8 }),
                new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, Width = 8 })
                {
                    new TestNode(out var rootBa, "C", new LayoutPlan { FlexGrow = 1, Width = 8 })
                },
                new TestNode(out var rootC, "D", new LayoutPlan { FlexGrow = 1, Width = 8 })
            };

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            """
            ╔══════╗═══════════════════════╗
            ║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
            ║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
            ║AAAAAA║░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
            ║CCCCCC║░░░░░░░░░░░░░░░░░░░░░░░║
            ║CCCCCC║░░░░░░░░░░░░░░░░░░░░░░░║
            ║CCCCCC║░░░░░░░░░░░░░░░░░░░░░░░║
            ║CCCCCC║░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════╝░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════╗░░░░░░░░░░░░░░░░░░░░░░░║
            ║DDDDDD║░░░░░░░░░░░░░░░░░░░░░░░║
            ║DDDDDD║░░░░░░░░░░░░░░░░░░░░░░░║
            ║DDDDDD║░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════╝═══════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 8, 5), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 5, 8, 6), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 5, 8, 6), rootBa.Layout.Bounds);
        Assert.Equal(new RectF(0, 11, 8, 5), rootC.Layout.Bounds);
    }

    [Fact]
    public void rounding_inner_node_controversy_combined()
    {
        var root =
            new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Height = 100.Percent() }),
                new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, Height = 100.Percent() })
                {
                    new TestNode(out var rootBa, "C", new LayoutPlan { FlexGrow = 1, Width = 100.Percent() }),
                    new TestNode(out var rootBb, "D", new LayoutPlan { FlexGrow = 1, Width = 100.Percent() })
                    {
                        new TestNode(out var rootBba, "E", new LayoutPlan { FlexGrow = 1, Width = 100.Percent() })
                    },
                    new TestNode(out var rootBc, "F", new LayoutPlan { FlexGrow = 1, Width = 100.Percent() })
                },
                new TestNode(out var rootC, "G", new LayoutPlan { FlexGrow = 1, Height = 100.Percent() })
            };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═════════╗╔════════╗╔═════════╗
            ║AAAAAAAAA║║CCCCCCCC║║GGGGGGGGG║
            ║AAAAAAAAA║║CCCCCCCC║║GGGGGGGGG║
            ║AAAAAAAAA║║CCCCCCCC║║GGGGGGGGG║
            ║AAAAAAAAA║╚════════╝║GGGGGGGGG║
            ║AAAAAAAAA║╔════════╗║GGGGGGGGG║
            ║AAAAAAAAA║║EEEEEEEE║║GGGGGGGGG║
            ║AAAAAAAAA║║EEEEEEEE║║GGGGGGGGG║
            ║AAAAAAAAA║║EEEEEEEE║║GGGGGGGGG║
            ║AAAAAAAAA║║EEEEEEEE║║GGGGGGGGG║
            ║AAAAAAAAA║╚════════╝║GGGGGGGGG║
            ║AAAAAAAAA║╔════════╗║GGGGGGGGG║
            ║AAAAAAAAA║║FFFFFFFF║║GGGGGGGGG║
            ║AAAAAAAAA║║FFFFFFFF║║GGGGGGGGG║
            ║AAAAAAAAA║║FFFFFFFF║║GGGGGGGGG║
            ╚═════════╝╚════════╝╚═════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 11, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(11, 0, 10, 16), rootB.Layout.Bounds);
        Assert.Equal(new RectF(11, 0, 10, 5), rootBa.Layout.Bounds);
        Assert.Equal(new RectF(11, 5, 10, 6), rootBb.Layout.Bounds);
        Assert.Equal(new RectF(11, 5, 10, 6), rootBba.Layout.Bounds);
        Assert.Equal(new RectF(11, 11, 10, 5), rootBc.Layout.Bounds);
        Assert.Equal(new RectF(21, 0, 11, 16), rootC.Layout.Bounds);
    }
}
