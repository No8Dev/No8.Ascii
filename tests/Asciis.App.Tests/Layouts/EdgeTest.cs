using Asciis.App.Controls;
using Asciis.App.ElementLayout;
using Xunit;
using Xunit.Abstractions;
// ReSharper disable StringLiteralTypo

namespace Asciis.App.Tests.Layouts;

[TestClass]
public class EdgeTest : BaseTests
{
    public EdgeTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void start_overrides()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 10, Height = 10 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(start: 1, left: 2, right: 2) }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔╔═════╗═╗
║║AAAAA║░║
║║AAAAA║░║
║║AAAAA║░║
║║AAAAA║░║
║║AAAAA║░║
║║AAAAA║░║
║║AAAAA║░║
║║AAAAA║░║
╚╚═════╝═╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(1, 0, 7, 10), rootA.Layout.Bounds);
    }

    [Fact]
    public void end_overrides()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 10, Height = 10 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(end: 1, left: 2, right: 2) }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔═╔═════╗╗
║░║AAAAA║║
║░║AAAAA║║
║░║AAAAA║║
║░║AAAAA║║
║░║AAAAA║║
║░║AAAAA║║
║░║AAAAA║║
║░║AAAAA║║
╚═╚═════╝╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(2, 0, 7, 10), rootA.Layout.Bounds);
    }

    [Fact]
    public void horizontal_overridden()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 10, Height = 10 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(horizontal: 1, left: 2) }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔═╔═════╗╗
║░║AAAAA║║
║░║AAAAA║║
║░║AAAAA║║
║░║AAAAA║║
║░║AAAAA║║
║░║AAAAA║║
║░║AAAAA║║
║░║AAAAA║║
╚═╚═════╝╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(2, 0, 7, 10), rootA.Layout.Bounds);
    }

    [Fact]
    public void vertical_overridden()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Vert, Width = 10, Height = 10 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(vertical: 1, top: 2) }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔════════╗
║░░░░░░░░║
╔════════╗
║AAAAAAAA║
║AAAAAAAA║
║AAAAAAAA║
║AAAAAAAA║
║AAAAAAAA║
╚════════╝
╚════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 2, 10, 7), rootA.Layout.Bounds);
    }

    [Fact]
    public void horizontal_overrides_all()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Vert, Width = 10, Height = 10 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(horizontal: 1, all: 2) }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔════════╗
║░░░░░░░░║
║╔══════╗║
║║AAAAAA║║
║║AAAAAA║║
║║AAAAAA║║
║║AAAAAA║║
║╚══════╝║
║░░░░░░░░║
╚════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(1, 2, 8, 6), rootA.Layout.Bounds);
    }

    [Fact]
    public void vertical_overrides_all()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Vert, Width = 10, Height = 10 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(vertical: 1, all: 2) }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔════════╗
║░╔════╗░║
║░║AAAA║░║
║░║AAAA║░║
║░║AAAA║░║
║░║AAAA║░║
║░║AAAA║░║
║░║AAAA║░║
║░╚════╝░║
╚════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(2, 1, 6, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void all_overridden()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Vert, Width = 10, Height = 10 })
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Margin = new Sides(1, 1, 1, 1, all: 2) }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔════════╗
║╔══════╗║
║║AAAAAA║║
║║AAAAAA║║
║║AAAAAA║║
║║AAAAAA║║
║║AAAAAA║║
║║AAAAAA║║
║╚══════╝║
╚════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(1, 1, 8, 8), rootA.Layout.Bounds);
    }
}
