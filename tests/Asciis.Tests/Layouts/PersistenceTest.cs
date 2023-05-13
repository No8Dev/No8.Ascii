using No8.Ascii.Tests.Helpers;

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class PersistenceTest
{
    /*
    [Fact]
    public void cloning_shared_root()
    {
        var root = new TestNode(new LayoutPlan { Width = 100, Height = 100 })
                       {
                           new TestNode( out var rootA, new LayoutPlan { FlexGrow = 1, ChildMainLength = 50 } ), 
                           new TestNode( out var rootB, new LayoutPlan { FlexGrow = 1 } )
                       };

        NodeArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(100, root.Layout.Width);
        Assert.Equal(100, root.Layout.Height);

        Assert.Equal(0, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.Equal(100, rootA.Layout.Width);
        Assert.Equal(75, rootA.Layout.Height);

        Assert.Equal(0, rootB.Layout.Left);
        Assert.Equal(75, rootB.Layout.Top);
        Assert.Equal(100, rootB.Layout.Width);
        Assert.Equal(25, rootB.Layout.Height);

        var root2 = new TestNode(root) { Plan = { Width = 100 } };

        Assert.Equal(2, root.Children.Count);
        // The elements should have referential equality at this point.
        Assert.Equal(rootA, root2.Children[0]);
        Assert.Equal(rootB, root2.Children[1]);

        NodeArrange.Calculate(root2);

        Assert.Equal(2, root.Children.Count);
        // Re-layout with no changed input should result in referential equality.
        Assert.Equal(rootA, root2.Children[0]);
        Assert.Equal(rootB, root2.Children[1]);

        root2.Plan.Width = 150;
        root2.Plan.Height = 200;
        NodeArrange.Calculate(root2);

        Assert.Equal(2, root2.Children.Count);
        // Re-layout with changed input should result in cloned elements
        var root_2_0 = root2.Children[0];
        var root_2_1 = root2.Children[1];

        //Subtle issue with AreNotEqual and IEnumerable
        //Assert.AreNotEqual(rootA, root_2_0);
        //Assert.AreNotEqual(rootB, root_2_1);
        Assert.True(rootA != root_2_0);
        Assert.True(rootB != root_2_1);

        // Everything in the root should remain unchanged.
        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(100, root.Layout.Width);
        Assert.Equal(100, root.Layout.Height);

        Assert.Equal(0, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.Equal(100, rootA.Layout.Width);
        Assert.Equal(75, rootA.Layout.Height);

        Assert.Equal(0, rootB.Layout.Left);
        Assert.Equal(75, rootB.Layout.Top);
        Assert.Equal(100, rootB.Layout.Width);
        Assert.Equal(25, rootB.Layout.Height);

        // The new root now has new layout.
        Assert.Equal(0, root2.Layout.Left);
        Assert.Equal(0, root2.Layout.Top);
        Assert.Equal(150, root2.Layout.Width);
        Assert.Equal(200, root2.Layout.Height);

        Assert.Equal(0, root_2_0.Layout.Left);
        Assert.Equal(0, root_2_0.Layout.Top);
        Assert.Equal(150, root_2_0.Layout.Width);
        Assert.Equal(125, root_2_0.Layout.Height);

        Assert.Equal(0, root_2_1.Layout.Left);
        Assert.Equal(125, root_2_1.Layout.Top);
        Assert.Equal(150, root_2_1.Layout.Width);
        Assert.Equal(75, root_2_1.Layout.Height);
    }
    */

    /*
    [Fact]
    public void mutating_elements_of_a_clone_clones()
    {
        var root = new TestNode();
        Assert.Equal(0, root.Children.Count);

        var root2 = new TestNode(root);
        Assert.Equal(0, root.Children.Count);

        var root2_0 = new TestNode();
        root2.Add(root2_0);

        Assert.Equal(0, root.Children.Count);
        Assert.Equal(1, root2.Children.Count);

        var root3 = new TestNode(root2);
        Assert.Equal(1, root2.Children.Count);
        Assert.Equal(1, root3.Children.Count);
        Assert.Equal(root2.Children[0], root3.Children[0]);

        var root3_1 = new TestNode();
        root3.Add(root3_1);
        Assert.Equal(1, root2.Children.Count);
        Assert.Equal(2, root3.Children.Count);
        Assert.Equal(root3_1, root3.Children[1]);
        Assert.False(ReferenceEquals(root2.Children[0], root3.Children[1]));

        var root4 = new TestNode(root3);
        Assert.Equal(root3_1, root4.Children[1]);

        // We are now creating new instances immediately, so this test is a little warped
        root4.Remove(root3_1);
        Assert.Equal(2, root3.Children.Count);
        //Assert.Equal(1, root4.Children.Count);
        //Assert.False(ReferenceEquals(root3.Children[0], root4.Children[0]));
    }
    */

    /*
    [Fact]
    public void cloning_two_levels()
    {
        new TestNode(out var root, new LayoutPlan { Width = 100, Height = 100 })
            {
                new TestNode( out var rootA, new LayoutPlan { FlexGrow = 1, ChildMainLength = 15 } ),
                new TestNode( out var rootB, new LayoutPlan { FlexGrow = 1 } )
                {
                    new TestNode( out var rootBa, new LayoutPlan { ChildMainLength = 10, FlexGrow = 1 } ), new TestNode( out var rootBb, new LayoutPlan { ChildMainLength = 25 } )
                }
            };


        NodeArrange.Calculate(root);

        Assert.Equal(40, rootA.Layout.Height);
        Assert.Equal(60, rootB.Layout.Height);
        Assert.Equal(35, rootBa.Layout.Height);
        Assert.Equal(25, rootBb.Layout.Height);

        var root2_0 = new TestNode(rootA);
        var root2_1 = new TestNode(rootB);
        var root2 = new TestNode(root);

        root2_0.Plan.FlexGrow = 0;
        root2_0.Plan.ChildMainLength = 40;

        root2.Clear();
        root2.Add(root2_0);
        root2.Add(root2_1);
        Assert.Equal(2, root.Children.Count);

        NodeArrange.Calculate(root2);

        // Original root is unchanged
        Assert.Equal(40, rootA.Layout.Height);
        Assert.Equal(60, rootB.Layout.Height);
        Assert.Equal(35, rootBa.Layout.Height);
        Assert.Equal(25, rootBb.Layout.Height);

        // New root has new layout at the top
        Assert.Equal(40, root2_0.Layout.Height);
        Assert.Equal(60, root2_1.Layout.Height);

        // The deeper elements are untouched.
        Assert.Equal(rootBa, root2_1.Children[0]);
        Assert.Equal(rootBb, root2_1.Children[1]);
    }
    */
}
