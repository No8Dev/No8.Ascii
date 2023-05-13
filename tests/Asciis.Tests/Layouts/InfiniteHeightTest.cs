using No8.Ascii.Console;
using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable StringLiteralTypo

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class InfiniteHeightTest : BaseTests
{
    public InfiniteHeightTest(ITestOutputHelper context) : base(context)
    {
    }

    // This test isn't correct from the Flexbox standard standpoint,
    // because percentages are calculated with container constraints.
    // However, we need to make sure we fail gracefully in this case, not returning NaN
    [Fact]
    public void percent_absolute_position_infinite_height()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 32 })
                  .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 30, Height = 16 }))
                  .Add(new TestNode(out var rootB, "B", new LayoutPlan
                  {
                      PositionType = PositionType.Absolute,
                      Left = 25.Percent(),
                      Top = 25.Percent(),
                      Width = 25.Percent(),
                      Height = 25.Percent()
                  }));

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔════════════════════════════╗═╗
║AAAAAAAAAAAAAAAAAAAAAAAAAAAA║░║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAA║░║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAA║░║
║AAAAAAA╔══════╗AAAAAAAAAAAAA║░║
║AAAAAAA║BBBBBB║AAAAAAAAAAAAA║░║
║AAAAAAA║BBBBBB║AAAAAAAAAAAAA║░║
║AAAAAAA╚══════╝AAAAAAAAAAAAA║░║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAA║░║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAA║░║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAA║░║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAA║░║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAA║░║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAA║░║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAA║░║
╚════════════════════════════╝═╝",
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 30, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(8, 4, 8, 4), rootB.Layout.Bounds);

    }
}
