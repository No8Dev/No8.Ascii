using Asciis.App.Controls;
using Asciis.App.ElementLayout;
using Xunit;
using Xunit.Abstractions;
// ReSharper disable StringLiteralTypo

namespace Asciis.App.Tests.Layouts;

[TestClass]
public class AndroidNewsFeed : BaseTests
{
    public AndroidNewsFeed(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void android_news_feed()
    {
        var root = new TestNode(new LayoutPlan { AlignElements_Cross = Alignment_Cross.Stretch, Width = 64 })
                       {
                           new TestNode( out var rootA , "A")
                           {
                               new TestNode( out var rootAa, "B", new LayoutPlan { AlignElements_Cross = Alignment_Cross.Stretch } )
                               {
                                   new TestNode( out var rootAaa, "C", new LayoutPlan { AlignElements_Cross = Alignment_Cross.Stretch } )
                                   {
                                       new TestNode( out var rootAaaa, "D",
                                                new LayoutPlan
                                                {
                                                    ElementsDirection   = LayoutDirection.Horz,
                                                    AlignElements_Cross = Alignment_Cross.Stretch,
                                                    Align_Cross     = AlignmentLine_Cross.Start,
                                                    Margin            = new Sides( start: 16, top: 8 )
                                                } )
                                       {
                                           new TestNode( out var rootAaaaa, "E", new LayoutPlan
                                               {
                                                   ElementsDirection = LayoutDirection.Horz, AlignElements_Cross = Alignment_Cross.Stretch
                                               } )
                                           {
                                               new TestNode( out var rootAaaaaa, "F", new LayoutPlan
                                                   {
                                                       AlignElements_Cross = Alignment_Cross.Stretch, Width = 12, Height = 12
                                                   } )
                                           },
                                           new TestNode( out var rootAaaab, "G",
                                                    new LayoutPlan
                                                    {
                                                        AlignElements_Cross = Alignment_Cross.Stretch,
                                                        FlexShrink        = 1,
                                                        Margin            = new Sides( right: 4 ),
                                                        Padding           = new Sides( 12, 2, 4, 2 )
                                                    } )
                                           {
                                               new TestNode( out var rootAaaaba, "H",
                                                        new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_Cross = Alignment_Cross.Stretch, FlexShrink = 1 } ),
                                               new TestNode( out var rootAaaabb, "I", new LayoutPlan { AlignElements_Cross = Alignment_Cross.Stretch, FlexShrink = 1 } )
                                           }
                                       }
                                   },
                                   new TestNode( out var rootAab, "J", new LayoutPlan { AlignElements_Cross = Alignment_Cross.Stretch } )
                                   {
                                       new TestNode( out var rootAaba, "K",
                                                new LayoutPlan
                                                {
                                                    ElementsDirection  = LayoutDirection.Horz,
                                                    AlignElements_Cross  = Alignment_Cross.Stretch,
                                                    Align_Cross = AlignmentLine_Cross.Start,
                                                    Margin             = new Sides( start: 16, top: 2 )
                                                } )
                                       {
                                           new TestNode( out var rootAabaa, "L", new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_Cross = Alignment_Cross.Stretch } )
                                           {
                                               new TestNode( out var rootAabaaa, "M", new LayoutPlan { AlignElements_Cross = Alignment_Cross.Stretch, Width = 7, Height = 7 } )
                                           },
                                           new TestNode( out var rootAabab, "N",
                                                    new LayoutPlan
                                                    {
                                                        AlignElements_Cross = Alignment_Cross.Stretch,
                                                        FlexShrink        = 1,
                                                        Margin            = new Sides( right: 4 ),
                                                        Padding           = new Sides( 4, 2, 4, 2 )
                                                    } )
                                           {
                                               new TestNode( out var rootAababa, "O",
                                                        new LayoutPlan { ElementsDirection = LayoutDirection.Horz, AlignElements_Cross = Alignment_Cross.Stretch, FlexShrink = 1 } ),
                                               new TestNode( out var rootAababb, "P", new LayoutPlan { AlignElements_Cross = Alignment_Cross.Stretch, FlexShrink = 1 } )
                                           }
                                       }
                                   }
                               }
                           }
                       };


        ElementArrange.Calculate(root);

        Draw(root, 64, 32);

        Assert.Equal(new RectF(0, 0, 64, 32), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 64, 29), rootA.Layout.Bounds);      // A
        Assert.Equal(new RectF(0, 0, 64, 29), rootAa.Layout.Bounds);     // B
        Assert.Equal(new RectF(0, 0, 64, 20), rootAaa.Layout.Bounds);    // C
        Assert.Equal(new RectF(16, 8, 48, 12), rootAaaa.Layout.Bounds);   // D
        Assert.Equal(new RectF(16, 8, 12, 12), rootAaaaa.Layout.Bounds);  // E
        Assert.Equal(new RectF(16, 8, 12, 12), rootAaaaaa.Layout.Bounds); // F
        Assert.Equal(new RectF(28, 8, 16, 4), rootAaaab.Layout.Bounds);   // G
        Assert.Equal(new RectF(40, 10, 0, 0), rootAaaaba.Layout.Bounds);  // H
        Assert.Equal(new RectF(40, 10, 0, 0), rootAaaabb.Layout.Bounds);  // I
        Assert.Equal(new RectF(0, 20, 64, 9), rootAab.Layout.Bounds);   // J
        Assert.Equal(new RectF(16, 22, 48, 7), rootAaba.Layout.Bounds);   // K
        Assert.Equal(new RectF(16, 22, 7, 7), rootAabaa.Layout.Bounds);   // L
        Assert.Equal(new RectF(16, 22, 7, 7), rootAabaaa.Layout.Bounds);   // M
        Assert.Equal(new RectF(23, 22, 8, 4), rootAabab.Layout.Bounds);   // N
        Assert.Equal(new RectF(27, 24, 0, 0), rootAababa.Layout.Bounds);  // O
        Assert.Equal(new RectF(27, 24, 0, 0), rootAababb.Layout.Bounds);  // P

        Assert.Equal(
@"╔══════════════════════════════════════════════════════════════╗
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
║CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC║
║CCCCCCCCCCCCCCC╔══════════╗┌──────────────┐═══════════════════╗
║CCCCCCCCCCCCCCC║FFFFFFFFFF║│GGGGGGGGGGGGGG│DDDDDDDDDDDDDDDDDDD║
║CCCCCCCCCCCCCCC║FFFFFFFFFF║│GGGGGGGGGGGIGG│DDDDDDDDDDDDDDDDDDD║
║CCCCCCCCCCCCCCC║FFFFFFFFFF║└──────────────┘DDDDDDDDDDDDDDDDDDD║
║CCCCCCCCCCCCCCC║FFFFFFFFFF║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
║CCCCCCCCCCCCCCC║FFFFFFFFFF║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
║CCCCCCCCCCCCCCC║FFFFFFFFFF║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
║CCCCCCCCCCCCCCC║FFFFFFFFFF║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
║CCCCCCCCCCCCCCC║FFFFFFFFFF║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
║CCCCCCCCCCCCCCC║FFFFFFFFFF║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
║CCCCCCCCCCCCCCC║FFFFFFFFFF║DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD║
╚═══════════════╚══════════╝═══════════════════════════════════╝
╔══════════════════════════════════════════════════════════════╗
║JJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJJ║
║JJJJJJJJJJJJJJJ╔═════╗┌──────┐════════════════════════════════╗
║JJJJJJJJJJJJJJJ║MMMMM║│NNNNNN│KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK║
║JJJJJJJJJJJJJJJ║MMMMM║│NNNPNN│KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK║
║JJJJJJJJJJJJJJJ║MMMMM║└──────┘KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK║
║JJJJJJJJJJJJJJJ║MMMMM║KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK║
║JJJJJJJJJJJJJJJ║MMMMM║KKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKK║
╚═══════════════╚═════╝════════════════════════════════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════════════════════════════════════╝",
    Canvas!.ToString());

    }
}
