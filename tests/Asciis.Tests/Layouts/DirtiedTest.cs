using No8.Ascii.Console;
using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class DirtiedTest
{
    private static void _dirtied(IElement node)
    {
        var dirtiedCount = (int)(node.Context ?? 0);
        dirtiedCount++;
        node.Context = dirtiedCount;
    }

    [Fact]
    public void Dirtied()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 100, Height = 100 });

        ElementArrange.Calculate(root);

        //int dirtiedCount = 0;
        root.Context = 0;
        root.Dirtied += (o, e) => _dirtied(root);

        Assert.Equal(0, (int)root.Context);

        // `_dirtied` MUST be called in case of explicit dirtying.
        root.IsDirty = true;
        Assert.Equal(1, (int)root.Context);

        // `_dirtied` MUST be called ONCE.
        root.IsDirty = true;
        Assert.Equal(1, (int)root.Context);
    }

    [Fact]
    public void dirtied_propagation()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 100, Height = 100 })
                  .Add(new TestNode(out var rootA, new LayoutPlan { Width = 50, Height = 20 }))
                  .Add(new TestNode(new LayoutPlan { Width = 50, Height = 20 }));

        ElementArrange.Calculate(root);

        root.Context = 0;
        root.Dirtied += (sender, node) => _dirtied(root);

        Assert.Equal(0, (int)root.Context);

        // `_dirtied` MUST be called for the first time.
        rootA.MarkDirty();
        Assert.Equal(1, (int)root.Context);

        // `_dirtied` must NOT be called for the second time.
        rootA.MarkDirty();
        Assert.Equal(1, (int)root.Context);
    }

    [Fact]
    public void dirtied_hierarchy()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 100, Height = 100 })
                  .Add(new TestNode(out var rootA, new LayoutPlan { Width = 50, Height = 20 }))
                  .Add(new TestNode(out var rootB, new LayoutPlan { Width = 50, Height = 20 }));

        ElementArrange.Calculate(root);

        rootA.Context = 0;
        rootA.Dirtied += (s, e) => _dirtied(rootA);

        Assert.Equal(0, (int)rootA.Context);

        // `_dirtied` must NOT be called for descendants.
        root.MarkDirty();
        Assert.Equal(0, (int)rootA.Context);

        // `_dirtied` must NOT be called for the sibling node.
        rootB.MarkDirty();
        Assert.Equal(0, (int)rootA.Context);

        // `_dirtied` MUST be called in case of explicit dirtying.
        rootA.MarkDirty();
        Assert.Equal(1, (int)rootA.Context);
    }
}
