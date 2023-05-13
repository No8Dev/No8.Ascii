using Asciis.App.Controls;
using Asciis.App.ElementLayout;
using Xunit;

namespace Asciis.App.Tests.Layouts;

[TestClass]
public class RelayoutTest
{
    [Fact]
    public void recalculate_resolvedDimension_onchange()
    {
        var root = new TestNode()
           .Add(new TestNode(out var rootA, new LayoutPlan { MinHeight = 10, MaxHeight = 10 }));

        ElementArrange.Calculate(root);
        Assert.Equal(10, rootA.Layout.Height);

        rootA.Plan.MinHeight = Number.ValueUndefined;
        ElementArrange.Calculate(root);

        Assert.Equal(0, rootA.Layout.Height);
    }
}
