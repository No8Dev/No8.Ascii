using System.Collections.Generic;
using System.Linq;
using Asciis.App.Controls;
using Asciis.App.ElementLayout;
using Xunit;

namespace Asciis.App.Tests.Layouts;

[TestClass]
public class TreeMutationTest
{
    [Fact]
    public void set_elements_adds_elements_to_container()
    {
        var root =
            new TestNode { new TestNode(out var rootA), new TestNode(out var rootB) };

        var elements = root.Children;
        var expectedElements = new List<Control> { rootA, rootB };
        Assert.True(elements.SequenceEqual(expectedElements));

        var containers = new List<IElement> { rootA.Parent!, rootB.Parent! };
        var expectedContainers = new List<IElement> { root, root };
        Assert.True(containers.SequenceEqual(expectedContainers));
    }

    [Fact]
    public void set_elements_to_empty_removes_old_elements()
    {
        var root =
            new TestNode { new TestNode(out var rootA), new TestNode(out var rootB) };
        root.SetChildren(new TestNode[] { });

        var elements = root.Children;
        var expectedElements = new TestNode[] { };
        Assert.True(elements.SequenceEqual(expectedElements));

        var containers = new[] { rootA.Parent, rootB.Parent };
        var expectedContainers = new IElement[] { null!, null! };
        Assert.True(containers.SequenceEqual(expectedContainers));
    }

    [Fact]
    public void set_elements_replaces_non_common_elements()
    {
        var root = new TestNode();
        var rootA = new TestNode(name: "rootA") { Context = 0 };
        var rootB = new TestNode(name: "rootB") { Context = 1 };
        var rootC = new TestNode(name: "rootC") { Context = 2 };
        var rootD = new TestNode(name: "rootD") { Context = 3 };

        root.SetChildren(new[] { rootA, rootB });
        root.SetChildren(new[] { rootC, rootD });

        var elements = root.Children;
        var expectedElements = new[] { rootC, rootD };
        Assert.True(elements.SequenceEqual(expectedElements));


        var containers = new[] { rootA.Parent, rootB.Parent };
        var expectedContainers = new TestNode[] { null!, null! };
        Assert.True(containers.SequenceEqual(expectedContainers));
    }

    [Fact]
    public void set_elements_keeps_and_reorders_common_elements()
    {
        var root = new TestNode();
        var rootA = new TestNode(name: "rootA") { Context = 0 };
        var rootB = new TestNode(name: "rootB") { Context = 1 };
        var rootC = new TestNode(name: "rootC") { Context = 2 };
        var rootD = new TestNode(name: "rootD") { Context = 3 };

        root.SetChildren(new[] { rootA, rootB, rootC });
        root.SetChildren(new[] { rootC, rootB, rootD });

        var elements = root.Children;
        var expectedElements = new[] { rootC, rootB, rootD };
        Assert.True(elements.SequenceEqual(expectedElements));

        var containers = new List<IElement> { rootA.Parent!, rootB.Parent!, rootC.Parent!, rootD.Parent! };
        var expectedContainers = new[] { null, root, root, root };
        Assert.True(containers.SequenceEqual(expectedContainers));
    }
}
