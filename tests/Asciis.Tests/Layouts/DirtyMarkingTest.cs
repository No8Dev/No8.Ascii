using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class DirtyMarkingTest
{
    [Fact]
    public void dirty_propagation()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 100, Height = 100 })
                       {
                           new TestNode( out var rootA, new LayoutPlan { Width = 50, Height = 20 } ), new TestNode( out var rootB, new LayoutPlan { Width = 50, Height = 20 } )
                       };

        ElementArrange.Calculate(root);

        rootA.Plan.Width = 20;

        Assert.True(rootA.IsDirty);
        Assert.False(rootB.IsDirty);
        Assert.True(root.IsDirty);

        ElementArrange.Calculate(root);

        Assert.False(rootA.IsDirty);
        Assert.False(rootB.IsDirty);
        Assert.False(root.IsDirty);
    }

    [Fact]
    public void dirty_propagation_only_if_prop_changed()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 100, Height = 100 })
                       {
                           new TestNode( out var rootA, new LayoutPlan { Width = 50, Height = 20 } ), new TestNode( out var rootB, new LayoutPlan { Width = 50, Height = 20 } )
                       };

        ElementArrange.Calculate(root);

        rootA.Plan.Width = 50;

        Assert.False(rootA.IsDirty);
        Assert.False(rootB.IsDirty);
        Assert.False(root.IsDirty);
    }

    [Fact]
    public void dirty_mark_all_elements_as_dirty_when_display_changes()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Height = 100 })
                       {
                           new TestNode( out var rootA, new LayoutPlan { FlexGrow = 1 } ),
                           new TestNode( out var rootB, new LayoutPlan { FlexGrow = 1, Atomic = true } )
                           {
                               new TestNode { new TestNode( out var root_1_0_0, new LayoutPlan { Width = 8, Height = 16 } ) }
                           }
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(0, root_1_0_0.Layout.Width);
        Assert.Equal(0, root_1_0_0.Layout.Height);


        rootA.Plan.Atomic = true;
        rootB.Plan.Atomic = false;

        ElementArrange.Calculate(root);

        Assert.Equal(8, root_1_0_0.Layout.Width);
        Assert.Equal(16, root_1_0_0.Layout.Height);

        rootA.Plan.Atomic = false;
        rootB.Plan.Atomic = true;

        ElementArrange.Calculate(root);

        Assert.Equal(0, root_1_0_0.Layout.Width);
        Assert.Equal(0, root_1_0_0.Layout.Height);

        rootA.Plan.Atomic = true;
        rootB.Plan.Atomic = false;

        ElementArrange.Calculate(root);

        Assert.Equal(8, root_1_0_0.Layout.Width);
        Assert.Equal(16, root_1_0_0.Layout.Height);
    }

    [Fact]
    public void dirty_node_only_if_elements_are_actually_removed()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 50, Height = 50 })
                       {
                           new TestNode( out var root0, new LayoutPlan { Width = 50, Height = 25 } )
                       };

        ElementArrange.Calculate(root);

        var root1 = new TestNode();
        root.Remove(root1);

        Assert.False(root.IsDirty);

        root.Remove(root0);
        Assert.True(root.IsDirty);
    }

    [Fact]
    public void dirty_node_only_if_undefined_values_gets_set_to_undefined()
    {
        var root = new TestNode(new LayoutPlan { Width = 50, Height = 50, MinWidth = Number.ValueUndefined });

        ElementArrange.Calculate(root);

        Assert.False(root.IsDirty);

        root.Plan.MinWidth = Number.ValueUndefined;

        Assert.False(root.IsDirty);
    }
}
