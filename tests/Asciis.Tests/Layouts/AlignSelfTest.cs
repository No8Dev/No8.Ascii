using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class AlignSelfTest : BaseTests
{
    public AlignSelfTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void align_self_center()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { AlignSelf_Cross = AlignmentLine_Cross.Center, Width = 12, Height = 12 } )
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(10, 0, 12, 12), rootA.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔═════════╔══════════╗═════════╗
            ║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
            ║░░░░░░░░░║AAAAAAAAAA║░░░░░░░░░║
            ║░░░░░░░░░╚══════════╝░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());
    }

    [Fact]
    public void align_self_flex_end()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { AlignSelf_Cross = AlignmentLine_Cross.End, Width = 12, Height = 12 } )
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(20, 0, 12, 12), rootA.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔═══════════════════╔══════════╗
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░╚══════════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());
    }

    [Fact]
    public void align_self_flex_start()
    {
        var root = new TestNode(new LayoutPlan { Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { AlignSelf_Cross = AlignmentLine_Cross.Start, Width = 12, Height = 12 } )
                       };


        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 12, 12), rootA.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════╗═══════════════════╗
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ║AAAAAAAAAA║░░░░░░░░░░░░░░░░░░░║
            ╚══════════╝░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());
    }

    [Fact]
    public void align_self_flex_end_override_flex_start()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 32, Height = 16 })
                       {
                           new TestNode( out var rootA, "A", new LayoutPlan { AlignSelf_Cross = AlignmentLine_Cross.End, Width = 12, Height = 12 } )
                       };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(20, 0, 12, 12), rootA.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            """
            ╔═══════════════════╔══════════╗
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░║AAAAAAAAAA║
            ║░░░░░░░░░░░░░░░░░░░╚══════════╝
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());
    }
}
