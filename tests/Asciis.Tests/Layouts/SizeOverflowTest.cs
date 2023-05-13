using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class SizeOverflowTest : BaseTests
{
    public SizeOverflowTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void nested_overflowing_element()
    {
        var root =
            new TestNode(new LayoutPlan { Width = 32, Height = 16 })
            {
                new TestNode(out var rootA, "A")
                {
                    new TestNode(out var rootAa, "B", new LayoutPlan { Width = 64, Height = 32 })
                }
            };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔═══════════════════════════════
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 32), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 64, 32), rootAa.Layout.Bounds);

        Assert.True(root.Layout.HadOverflow);
        Assert.False(rootA.Layout.HadOverflow);
        Assert.False(rootAa.Layout.HadOverflow);
    }

    [Fact]
    public void nested_overflowing_element_in_constraint_container()
    {
        var root =
            new TestNode(new LayoutPlan { Width = 32, Height = 16 })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { Width = 32, Height = 16 })
                {
                    new TestNode(out var rootAa, "B", new LayoutPlan { Width = 64, Height = 32 })
                }
            };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔═══════════════════════════════
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 64, 32), rootAa.Layout.Bounds);

        Assert.True(root.Layout.HadOverflow);
        Assert.True(rootA.Layout.HadOverflow);
        Assert.False(rootAa.Layout.HadOverflow);
    }

    [Fact]
    public void container_wrap_element_size_overflowing_container()
    {
        var root =
            new TestNode(new LayoutPlan { Width = 32, Height = 16 })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { Width = 32 })
                {
                    new TestNode(out var rootAa, "B", new LayoutPlan { Width = 32, Height = 32 })
                }
            };


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
║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 32), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 32), rootAa.Layout.Bounds);

        Assert.True(root.Layout.HadOverflow);
        Assert.False(rootA.Layout.HadOverflow);
        Assert.False(rootAa.Layout.HadOverflow);
    }
}
