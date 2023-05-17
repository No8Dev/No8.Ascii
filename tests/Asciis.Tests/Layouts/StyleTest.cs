using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class PlanTests
{
    [Fact]
    public void copy_style_same()
    {
        var node0 = new TestNode();
        var node1 = new TestNode();
        Assert.False(node0.IsDirty);

        node1.CopyPlan(node0);
        Assert.False(node0.IsDirty);
    }

    [Fact]
    public void copy_style_modified()
    {
        var node0 = new TestNode();
        Assert.False(node0.IsDirty);
        Assert.Equal(LayoutDirection.Vert, node0.Plan.ElementsDirection);
        Assert.False(node0.Plan.MaxHeight.Unit != Number.UoM.Undefined);

        var node1 = new TestNode { Plan = { ElementsDirection = LayoutDirection.Horz, MaxHeight = 10 } };

        node0.CopyPlan(node1);
        Assert.True(node0.IsDirty);
        Assert.Equal(LayoutDirection.Horz, node0.Plan.ElementsDirection);
        Assert.Equal(10, node0.Plan.MaxHeight.Value);
    }

    [Fact]
    public void copy_style_modified_same()
    {
        var node0 = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, MaxHeight = 10 });

        ElementArrange.Calculate(node0);

        Assert.False(node0.IsDirty);

        var node1 = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, MaxHeight = 10 });

        node1.CopyPlan(node0);
        Assert.False(node0.IsDirty);
    }

    [Fact]
    public void initialise_flexShrink_flexGrow()
    {
        var node0 = new TestNode(new LayoutPlan { FlexShrink = 1 });
        Assert.Equal(1f, node0.Plan.FlexShrink);

        node0.Plan.FlexShrink = Number.ValueUndefined;
        node0.Plan.FlexGrow = 3;
        Assert.Equal(0, node0.Plan.FlexShrink); // Default value is Zero, if flex shrink is not defined
        Assert.Equal(3, node0.Plan.FlexGrow);

        node0.Plan.FlexGrow = Number.ValueUndefined;
        node0.Plan.FlexShrink = 3;
        Assert.Equal(0, node0.Plan.FlexGrow); // Default value is Zero, if flex grow is not defined
        Assert.Equal(3, node0.Plan.FlexShrink);
    }
}
