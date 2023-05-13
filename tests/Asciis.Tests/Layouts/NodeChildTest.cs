using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class NodeElementTest
{
    [Fact]
    public void reset_layout_when_element_removed()
    {
        var root = new TestNode()
           .Add(new TestNode(out var rootA, "A", new LayoutPlan { Width = 32, Height = 16 }));

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);

        root.Remove(rootA);

        Assert.Equal(0, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.True(rootA.Layout.Width.IsUndefined());
        Assert.True(rootA.Layout.Height.IsUndefined());
    }
}
