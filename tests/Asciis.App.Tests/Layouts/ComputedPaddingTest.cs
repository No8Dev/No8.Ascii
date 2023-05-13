using Asciis.App.Controls;
using Asciis.App.ElementLayout;
using Xunit;
using Xunit.Abstractions;

namespace Asciis.App.Tests.Layouts;

[TestClass]
public class ComputedPaddingTest : BaseTests
{
    public ComputedPaddingTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void computed_layout_padding()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16, Padding = new Sides(start: 3) });

        ElementArrange.Calculate(root, 32, 16);

        Draw(root);
        Assert.Equal(
            @"┌──╔═══════════════════════════╗
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
│░░║░░░░░░░░░░░░░░░░░░░░░░░░░░░║
└──╚═══════════════════════════╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(3, 0, 29, 16), root.Layout.ContentBounds);
    }
}
