using System.Collections.Generic;
using System.Linq;
using Asciis.App.Controls;
using Xunit;

namespace Asciis.App.Tests.Layouts;

[TestClass]
public class TraversalTest
{
    [Fact]
    public void pre_order_traversal()
    {
        var root = new TestNode(name: "root")
            {
                new TestNode( out var root0, name: "rootA" )
                {
                    new TestNode( out var root00, name: "rootAa" )
                },
                new TestNode( out var root1, name: "rootB" )
            }
        ;

        List<Control> visited = new();
        Control.Traverse(root, node =>
        {
            visited.Add(node);
            return false;
        });

        List<Control> expected = new()
                                 {
                                     root,
                                     root0,
                                     root00,
                                     root1
                                 };
        Assert.True(expected.SequenceEqual(visited));
    }
}
