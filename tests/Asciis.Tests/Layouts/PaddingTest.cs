using No8.Ascii.Console;
using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class PaddingTest : BaseTests
{
    public PaddingTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void padding_no_size()
    {
        var root = new TestNode(new LayoutPlan { Padding = 4 });

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            @"┌──────────────────────────────┐
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░╔══════════════════════╗░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░╚══════════════════════╝░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
└──────────────────────────────┘",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(4, 4, 24, 8), root.Layout.ContentBounds);
    }

    [Fact]
    public void padding_container_match_element()
    {
        var root = new TestNode(new LayoutPlan { Padding = new Sides(4) })
           .Add(new TestNode(out var rootA, new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            @"┌──────────────────────────────┐
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░╔══════╗═══════════════╗░░░│
│░░░║░░░░░░║░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░║░░░░░░░░░░░░░░░║░░░│
│░░░╚══════╝░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░╚══════════════════════╝░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
└──────────────────────────────┘",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(4, 4, 24, 8), root.Layout.ContentBounds);
        Assert.Equal(new RectF(4, 4, 8, 4), rootA.Layout.Bounds);
    }

    [Fact]
    public void padding_flex_element()
    {
        var root = new TestNode(new LayoutPlan { Padding = new Sides(4), Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, new LayoutPlan { FlexGrow = 1, Width = 8 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"┌──────────────────────────────┐
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░╔══════╗═══════════════╗░░░│
│░░░║░░░░░░║░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░║░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░║░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░║░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░║░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░║░░░░░░░░░░░░░░░║░░░│
│░░░╚══════╝═══════════════╝░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
└──────────────────────────────┘",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(4, 4, 24, 8), root.Layout.ContentBounds);
        Assert.Equal(new RectF(4, 4, 8, 8), rootA.Layout.Bounds);
    }

    [Fact]
    public void padding_stretch_element()
    {
        var root = new TestNode(new LayoutPlan { Padding = new Sides(4), Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, new LayoutPlan { Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"┌──────────────────────────────┐
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░╔══════════════════════╗░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░╚══════════════════════╝░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░║░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░░░╚══════════════════════╝░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
└──────────────────────────────┘",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(4, 4, 24, 8), root.Layout.ContentBounds);
        Assert.Equal(new RectF(4, 4, 24, 4), rootA.Layout.Bounds);
    }

    [Fact]
    public void padding_center_element()
    {
        var root = new TestNode(new LayoutPlan
        {
            AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center,
            Align_Cross = AlignmentLine_Cross.Center,
            Padding = new Sides(start: 2, end: 4, bottom: 4),
            Width = 32,
            Height = 16
        })
           .Add(new TestNode(out var rootA, new LayoutPlan { Width = 8, Height = 4 }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"┌─╔════════════════════════╗───┐
│░║░░░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░║░░░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░║░░░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░║░░░░░░░░╔══════╗░░░░░░░░║░░░│
│░║░░░░░░░░║░░░░░░║░░░░░░░░║░░░│
│░║░░░░░░░░║░░░░░░║░░░░░░░░║░░░│
│░║░░░░░░░░╚══════╝░░░░░░░░║░░░│
│░║░░░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░║░░░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░║░░░░░░░░░░░░░░░░░░░░░░░░║░░░│
│░╚════════════════════════╝░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│
└──────────────────────────────┘",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(2, 0, 26, 12), root.Layout.ContentBounds);
        Assert.Equal(new RectF(11, 4, 8, 4), rootA.Layout.Bounds);
    }

    [Fact]
    public void element_with_padding_align_end()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.End, Align_Cross = AlignmentLine_Cross.End, Width = 32, Height = 16 })
           .Add(new TestNode(out var rootA, new LayoutPlan { Width = 16, Height = 16, Padding = new Sides(4) }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔═══════════════┌──────────────┐
║░░░░░░░░░░░░░░░│░░░░░░░░░░░░░░│
║░░░░░░░░░░░░░░░│░░░░░░░░░░░░░░│
║░░░░░░░░░░░░░░░│░░░░░░░░░░░░░░│
║░░░░░░░░░░░░░░░│░░░╔══════╗░░░│
║░░░░░░░░░░░░░░░│░░░║░░░░░░║░░░│
║░░░░░░░░░░░░░░░│░░░║░░░░░░║░░░│
║░░░░░░░░░░░░░░░│░░░║░░░░░░║░░░│
║░░░░░░░░░░░░░░░│░░░║░░░░░░║░░░│
║░░░░░░░░░░░░░░░│░░░║░░░░░░║░░░│
║░░░░░░░░░░░░░░░│░░░║░░░░░░║░░░│
║░░░░░░░░░░░░░░░│░░░╚══════╝░░░│
║░░░░░░░░░░░░░░░│░░░░░░░░░░░░░░│
║░░░░░░░░░░░░░░░│░░░░░░░░░░░░░░│
║░░░░░░░░░░░░░░░│░░░░░░░░░░░░░░│
╚═══════════════└──────────────┘",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(16, 0, 16, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(20, 4, 8, 8), rootA.Layout.ContentBounds);
    }
}
