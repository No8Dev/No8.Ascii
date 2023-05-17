using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class DisplayTest : BaseTests
{
    public DisplayTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void display_none()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                       .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1 }))
                       .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, Atomic = true }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"B══════════════════════════════╗
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
        Assert.Equal(new RectF(0, 0, 0, 0), rootB.Layout.Bounds);
    }

    [Fact]
    public void display_none_fixed_size()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                       .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1 }))
                       .Add(new TestNode(out var rootB, "B", new LayoutPlan { Width = 16, Height = 8, Atomic = true }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"B══════════════════════════════╗
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
        Assert.Equal(new RectF(0, 0, 0, 0), rootB.Layout.Bounds);
    }

    [Fact]
    public void display_none_with_margin()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                       .Add(new TestNode(out var rootA, "A", new LayoutPlan { Margin = new Sides(left: 2, top: 2, right: 2, bottom: 2), Width = 16, Height = 8, Atomic = true }))
                       .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
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
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
╚══════════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 0, 0), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 16), rootB.Layout.Bounds);
    }

    [Fact]
    public void display_none_with_element()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                       .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, FlexShrink = 1, ChildLayoutDirectionLength = 0.Percent() }))
                       .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, FlexShrink = 1, ChildLayoutDirectionLength = 0.Percent(), Atomic = true })
                               .Add(new TestNode(out var rootBa, "C", new LayoutPlan { FlexGrow = 1, FlexShrink = 1, ChildLayoutDirectionLength = 0.Percent(), Width = 16, MinWidth = 0, MinHeight = 0 })))
                       .Add(new TestNode(out var rootC, "D", new LayoutPlan { FlexGrow = 1, FlexShrink = 1, ChildLayoutDirectionLength = 0.Percent() }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"C══════════════╗╔══════════════╗
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
║AAAAAAAAAAAAAA║║DDDDDDDDDDDDDD║
╚══════════════╝╚══════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 16, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 0, 0), rootB.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 0, 0), rootBa.Layout.Bounds);
        Assert.Equal(new RectF(16, 0, 16, 16), rootC.Layout.Bounds);
    }

    [Fact]
    public void display_none_with_position()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32, Height = 16 })
                       .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1 }))
                       .Add(new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, Top = 10, Atomic = true }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"B══════════════════════════════╗
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
        Assert.Equal(new RectF(0, 0, 0, 0), rootB.Layout.Bounds);
    }
}
