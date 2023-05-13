using Asciis.App.Controls;
using Asciis.App.ElementLayout;
using Xunit;

namespace Asciis.App.Tests.Layouts;

[TestClass]
public class ZeroOutLayoutRecursivelyTest
{
    [Fact]
    public void zero_out_layout()
    {
        var root =
            new TestNode(new LayoutPlan { Width = 200, Height = 200, ElementsDirection = LayoutDirection.Horz })
            {
                    new TestNode( out var rootA,
                         new LayoutPlan { Width = 100, Height = 100, Margin = new Sides( top: 10 ), Padding = new Sides( top: 10 ) } )
            };

        ElementArrange.Calculate(root, 100, 100);

        Assert.Equal(10, rootA.Layout.Margin.Top);
        Assert.Equal(10, rootA.Layout.Padding.Top);

        rootA.Plan.Atomic = true;

        ElementArrange.Calculate(root, 100, 100);

        Assert.Equal(0, rootA.Layout.Margin.Top);
        Assert.Equal(0, rootA.Layout.Padding.Top);
    }
}
