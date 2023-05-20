using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class PercentageTest : BaseTests
{
    public PercentageTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void percentage_width_height()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 30.Percent(), Height = 30.Percent() }));

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

    [Fact]
    public void percentage_position_left_top()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Left = 10.Percent(), Top = 20.Percent(), Width = 45.Percent(), Height = 55.Percent() }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░╔════════════╗░░░░░░░░░░░░░░░║
            ║░║AAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║░║AAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║░║AAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║░║AAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║░║AAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║░║AAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║░║AAAAAAAAAAAA║░░░░░░░░░░░░░░░║
            ║░╚════════════╝░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(2, 6, 14, 9), rootA.Layout.Bounds);
    }

    [Fact]
    public void percentage_position_bottom_right()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Right = 20.Percent(), Bottom = 10.Percent(), Width = 55.Percent(), Height = 15.Percent() }));

        ElementArrange.Calculate(root);

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
        Assert.Equal(new RectF(-3, -3, 17, 2), rootA.Layout.Bounds);
    }

    [Fact]
    public void percentage_main_length()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 50.Percent() }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 25.Percent() }));

        ElementArrange.Calculate(root);

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

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 20, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(20, 0, 12, 16), rootB.Layout.Bounds);
    }

    [Fact]
    public void percentage_main_length_cross()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 50.Percent() }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 25.Percent() }));

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
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 10), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 32, 6), rootB.Layout.Bounds);
    }

    [Fact]
    public void percentage_main_length_cross_min_height()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, MinHeight = 60.Percent() }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 2, MinHeight = 10.Percent() }));

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
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 11), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 11, 32, 5), rootB.Layout.Bounds);
    }

    [Fact]
    public void percentage_main_length_main_max_height()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 10.Percent(), MaxHeight = 60.Percent() }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 4, ChildLayoutDirectionLength = 10.Percent(), MaxHeight = 20.Percent() }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════╗╔══════════════════════╗
            ║AAAAAA║║BBBBBBBBBBBBBBBBBBBBBB║
            ║AAAAAA║╚══════════════════════╝
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
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 8, 10), rootA.Layout.Bounds);
        Assert.Equal(new RectF(8, 0, 24, 3), rootB.Layout.Bounds);
    }

    [Fact]
    public void percentage_main_length_cross_max_height()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 10.Percent(), MaxHeight = 60.Percent() }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 4, ChildLayoutDirectionLength = 10.Percent(), MaxHeight = 20.Percent() }));

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
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 10), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 32, 3), rootB.Layout.Bounds);
    }

    [Fact]
    public void percentage_main_length_main_max_width()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 15.Percent(), MaxWidth = 60.Percent() }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 4, ChildLayoutDirectionLength = 10.Percent(), MaxWidth = 20.Percent() }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═════════════════╗╔═════╗═════╗
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ║AAAAAAAAAAAAAAAAA║║BBBBB║░░░░░║
            ╚═════════════════╝╚═════╝═════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 19, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(19, 0, 7, 16), rootB.Layout.Bounds);
    }

    [Fact]
    public void percentage_main_length_cross_max_width()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 10.Percent(), MaxWidth = 60.Percent() }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 4, ChildLayoutDirectionLength = 15.Percent(), MaxWidth = 20.Percent() }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═════════════════╗════════════╗
            ║AAAAAAAAAAAAAAAAA║░░░░░░░░░░░░║
            ║AAAAAAAAAAAAAAAAA║░░░░░░░░░░░░║
            ╚═════════════════╝░░░░░░░░░░░░║
            ╔════╗░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBB║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBB║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBB║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBB║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBB║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBB║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBB║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBB║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBB║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║BBBB║░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚════╝═════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 19, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 6, 12), rootB.Layout.Bounds);
    }

    [Fact]
    public void percentage_main_length_main_min_width()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 15.Percent(), MinWidth = 60.Percent() }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 4, ChildLayoutDirectionLength = 10.Percent(), MinWidth = 20.Percent() }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═════════════════╗╔═══════════╗
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ║AAAAAAAAAAAAAAAAA║║BBBBBBBBBBB║
            ╚═════════════════╝╚═══════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 19, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(19, 0, 13, 16), rootB.Layout.Bounds);
    }

    [Fact]
    public void percentage_main_length_cross_min_width()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 10.Percent(), MinWidth = 60.Percent() }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 4, ChildLayoutDirectionLength = 15.Percent(), MinWidth = 20.Percent() }));

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
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
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
        Assert.Equal(new RectF(0, 0, 32, 4), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 32, 12), rootB.Layout.Bounds);
    }

    [Fact]
    public void percentage_multiple_nested_with_padding_margin_and_percentage_values()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan
                                                        {
                                                            FlexGrow        = 1,
                                                            ChildLayoutDirectionLength = 20.Percent(),
                                                            Margin          = 2,
                                                            Padding         = 1,
                                                            MinWidth        = 60.Percent()
                                                        })
                          .Add(new TestNode(out var rootAa, "B", new LayoutPlan { Margin = 2, Padding = 3.Percent(), Width = 50.Percent() })
                                  .Add(new TestNode(out var rootAaa, "C", new LayoutPlan { Margin = 5.Percent(), Padding = 1, Width = 45.Percent() }))))
                  .Add(new TestNode(out var rootB, "D", new LayoutPlan { FlexGrow = 2, ChildLayoutDirectionLength = 5.Percent(), MinWidth = 20.Percent() }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░┌──────────────────────────┐░║
            ║░│╔════════════════════════╗│░║
            ║░│║AAAAAAAAAAAAAAAAAAAAAAAA║│░║
            ║░│║A╔═════════╗─┐AAAAAAAAAA║│░║
            ║░│╚═║┌────┐BBB║B│══════════╝│░║
            ║░└──╚└────┘═══╝B│───────────┘░║
            ║░░░░│BBBBBBBBBBB│░░░░░░░░░░░░░║
            ║░░░░└───────────┘░░░░░░░░░░░░░║
            ╔══════════════════════════════╗
            ║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
            ║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
            ║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
            ║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(2, 2, 28, 6), rootA.Layout.Bounds);
        Assert.Equal(new RectF(5, 5, 13, 5), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(6, 6, 6, 2), rootAaa.Layout.Bounds);
        Assert.Equal(new RectF(0, 10, 32, 6), rootB.Layout.Bounds);
    }

    [Fact]
    public void percentage_margin_should_calculate_based_only_on_width()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = 10.Percent() })
                    .Add(new TestNode(out var rootAa, "B", new LayoutPlan { Width = 10, Height = 10 })));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░╔════════╗═══════════════╗░░║
            ║░░║BBBBBBBB║AAAAAAAAAAAAAAA║░░║
            ║░░║BBBBBBBB║AAAAAAAAAAAAAAA║░░║
            ║░░║BBBBBBBB║AAAAAAAAAAAAAAA║░░║
            ║░░║BBBBBBBB║AAAAAAAAAAAAAAA║░░║
            ║░░║BBBBBBBB║AAAAAAAAAAAAAAA║░░║
            ║░░║BBBBBBBB║AAAAAAAAAAAAAAA║░░║
            ║░░║BBBBBBBB║AAAAAAAAAAAAAAA║░░║
            ║░░║BBBBBBBB║AAAAAAAAAAAAAAA║░░║
            ║░░╚════════╝═══════════════╝░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(3, 3, 26, 10), rootA.Layout.Bounds);
        Assert.Equal(new RectF(3, 3, 10, 10), rootAa.Layout.Bounds);
    }

    [Fact]
    public void percentage_padding_should_calculate_based_only_on_width()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Padding = 10.Percent() })
                    .Add(new TestNode(out var rootAa, "B", new LayoutPlan { Width = 10, Height = 10 })));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA│
            │AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA│
            │AA╔════════╗══════════════╗AAA│
            │AA║BBBBBBBB║AAAAAAAAAAAAAA║AAA│
            │AA║BBBBBBBB║AAAAAAAAAAAAAA║AAA│
            │AA║BBBBBBBB║AAAAAAAAAAAAAA║AAA│
            │AA║BBBBBBBB║AAAAAAAAAAAAAA║AAA│
            │AA║BBBBBBBB║AAAAAAAAAAAAAA║AAA│
            │AA║BBBBBBBB║AAAAAAAAAAAAAA║AAA│
            │AA║BBBBBBBB║AAAAAAAAAAAAAA║AAA│
            │AA║BBBBBBBB║══════════════╝AAA│
            │AA╚════════╝AAAAAAAAAAAAAAAAAA│
            │AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA│
            │AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA│
            └──────────────────────────────┘
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(3, 3, 10, 10), rootAa.Layout.Bounds);
    }

    [Fact]
    public void percentage_absolute_position()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan
                                                 {
                                                     PositionType = PositionType.Absolute,
                                                     Left = 30.Percent(),
                                                     Top = 10.Percent(),
                                                     Width = 10,
                                                     Height = 10
                                                 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░╔════════╗░░░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAA║░░░░░░░░░░░║
            ║░░░░░░░░░╚════════╝░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(10, 2, 10, 10), rootA.Layout.Bounds);
    }

    [Fact]
    public void percentage_width_height_undefined_container_size()
    {
        var root = new TestNode()
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 50.Percent(), Height = 50.Percent() }));

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
            ╚══════════════╝░░░░░░░░░░░░░░░║
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
        Assert.Equal(new RectF(0, 0, 16, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void percent_within_flex_grow()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 10 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1 })
                           .Add(new TestNode(out var rootBa, "C", new LayoutPlan { Width = 100.Percent() })))
                  .Add(new TestNode(out var rootC, "D", new LayoutPlan { Width = 10 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔════════╗CCCCCCCCCCCC╔════════╗
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ║AAAAAAAA║║BBBBBBBBBB║║DDDDDDDD║
            ╚════════╝╚══════════╝╚════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 10, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(10, 0, 12, 16), rootB.Layout.Bounds);
        Assert.Equal(new RectF(10, 0, 12, 0), rootBa.Layout.Bounds);
        Assert.Equal(new RectF(22, 0, 10, 16), rootC.Layout.Bounds);

    }

    [Fact]
    public void percentage_container_in_wrapping_container()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Align_Cross = AlignmentLine_Cross.Center, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A")
                   .Add(new TestNode(out var rootAa, "B", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center, Width = 100.Percent() })
                                  .Add(new TestNode(out var rootAaa, "C", new LayoutPlan { Width = 16, Height = 8 }))
                                  .Add(new TestNode(out var rootAab, "D", new LayoutPlan { Width = 16, Height = 8 }))
                       )
               );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╔══════════════╗╔══════════════╗
            ║CCCCCCCCCCCCCC║║DDDDDDDDDDDDDD║
            ║CCCCCCCCCCCCCC║║DDDDDDDDDDDDDD║
            ║CCCCCCCCCCCCCC║║DDDDDDDDDDDDDD║
            ║CCCCCCCCCCCCCC║║DDDDDDDDDDDDDD║
            ║CCCCCCCCCCCCCC║║DDDDDDDDDDDDDD║
            ║CCCCCCCCCCCCCC║║DDDDDDDDDDDDDD║
            ╚══════════════╝╚══════════════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 32, 8), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 32, 8), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(0, 4, 16, 8), rootAaa.Layout.Bounds);
        Assert.Equal(new RectF(16, 4, 16, 8), rootAab.Layout.Bounds);
    }

    [Fact]
    public void percent_absolute_position()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan
                                                 {
                                                     ElementsDirection = LayoutDirection.Horz,
                                                     PositionType = PositionType.Absolute,
                                                     Left = 50.Percent(),
                                                     Width = 100.Percent(),
                                                     Height = 6
                                                 })
                         .Add(new TestNode(out var rootAa, "B", new LayoutPlan { Width = 100.Percent() }))
                         .Add(new TestNode(out var rootAb, "C", new LayoutPlan { Width = 100.Percent() })));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔═══════════════╔═══════════════
            ║░░░░░░░░░░░░░░░║BBBBBBBBBBBBBBB
            ║░░░░░░░░░░░░░░░║BBBBBBBBBBBBBBB
            ║░░░░░░░░░░░░░░░║BBBBBBBBBBBBBBB
            ║░░░░░░░░░░░░░░░║BBBBBBBBBBBBBBB
            ║░░░░░░░░░░░░░░░╚═══════════════
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
        Assert.Equal(new RectF(16, 0, 32, 6), rootA.Layout.Bounds);
        Assert.Equal(new RectF(16, 0, 32, 6), rootAa.Layout.Bounds);
        Assert.Equal(new RectF(48, 0, 32, 6), rootAb.Layout.Bounds);
    }
}
