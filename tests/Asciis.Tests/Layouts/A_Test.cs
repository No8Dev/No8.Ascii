using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using Xunit;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Layouts;


public class A_Test : BaseTests
{
    public A_Test(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void a_stacked_labels()
    {
        var root = new TestNode(name: "Root", new LayoutPlan { Width = 32, Height = 16 })
                   {
                       new TestNode(out var frame, name: "A", new LayoutPlan { Height = 100.Percent() })
                       {
                           new TestNode(out var heading, name: "B", new LayoutPlan { Width = 10, Height = 5 }),
                           new TestNode(out var text, name: "C", new LayoutPlan { Width = 10, Height = 5 })
                       }
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔════════╗═════════════════════╗
            ║BBBBBBBB║AAAAAAAAAAAAAAAAAAAAA║
            ║BBBBBBBB║AAAAAAAAAAAAAAAAAAAAA║
            ║BBBBBBBB║AAAAAAAAAAAAAAAAAAAAA║
            ╚════════╝AAAAAAAAAAAAAAAAAAAAA║
            ╔════════╗AAAAAAAAAAAAAAAAAAAAA║
            ║CCCCCCCC║AAAAAAAAAAAAAAAAAAAAA║
            ║CCCCCCCC║AAAAAAAAAAAAAAAAAAAAA║
            ║CCCCCCCC║AAAAAAAAAAAAAAAAAAAAA║
            ╚════════╝AAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 16), frame.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 10, 5), heading.Layout.Bounds);
        Assert.Equal(new RectF(0, 5, 10, 5), text.Layout.Bounds);
    }


    [Fact]
    public void a_layout_testharness()
    {
        var root = new TestNode(new TestPlan { Width = 32, Height = 16 } )
           .Add(new TestNode(out var frame, "A", new TestPlan { Margin = 1, Padding = 2, ElementsDirection = LayoutDirection.Vert } )
                .Add(new TestNode(out var heading, "B", new TestPlan { Width = 16, Height = 4, AlignSelf_Cross = AlignmentLine_Cross.Center } ))
                .Add(new TestNode(out var text, "C", new TestPlan { Width = 6, Height = 4, AlignSelf_Cross = AlignmentLine_Cross.Start } ))
                );

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║┌────────────────────────────┐║
            ║│AAAAAAAAAAAAAAAAAAAAAAAAAAAA│║
            ║│A╔════╔══════════════╗════╗A│║
            ║│A║AAAA║BBBBBBBBBBBBBB║AAAA║A│║
            ║│A║AAAA║BBBBBBBBBBBBBB║AAAA║A│║
            ║│A║AAAA╚══════════════╝AAAA║A│║
            ║│A╔════╗AAAAAAAAAAAAAAAAAAA║A│║
            ║│A║CCCC║AAAAAAAAAAAAAAAAAAA║A│║
            ║│A║CCCC║AAAAAAAAAAAAAAAAAAA║A│║
            ║│A╚════╝═══════════════════╝A│║
            ║│AAAAAAAAAAAAAAAAAAAAAAAAAAAA│║
            ║└────────────────────────────┘║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(1, 1, 30, 12), frame.Layout.Bounds);
        Assert.Equal(new RectF(8, 3, 16, 4), heading.Layout.Bounds);
        Assert.Equal(new RectF(3, 7, 6, 4), text.Layout.Bounds);
    }

    [Fact]
    public void a_layout_alignment_testharness()
    {
        ElementArrange.Calculate( new TestNode(
                                   "Container",
                                        new TestPlan
                                        {
                                            Width = 100,
                                            Height = 100,
                                            ElementsDirection = LayoutDirection.Horz,
                                            ElementsWrap = LayoutWrap.Wrap,
                                                //AlignContentMain  = AlignmentMain.Stretch,
                                                //AlignContentCross = AlignmentCross.Stretch,
                                                Align_Cross = AlignmentLine_Cross.Center
                                        })
                                   {
                                       new TestNode( "Item1", new TestPlan { Width = 27, Height = 10 } ),
                                       new TestNode( "Item2", new TestPlan { Width = 27, Height = 10 } ),
                                       new TestNode( "Item3", new TestPlan { Width = 27, Height = 10 } ),
                                       new TestNode( "Item4", new TestPlan { Width = 27, Height = 10 } ),
                                       new TestNode( "Item5", new TestPlan { Width = 27, Height = 10 } )
                                   });

        ElementArrange.Calculate(
                              new TestNode(new TestPlan { ElementsDirection = LayoutDirection.Horz, Align_Cross = AlignmentLine_Cross.End, Width = 100, Height = 100 })
                              {
                                      new TestNode( out var rootA, new TestPlan { Width = 50, Height = 60 } ),
                                      new TestNode( out var rootB, new TestPlan { ElementsDirection = LayoutDirection.Horz, ElementsWrap = LayoutWrap.Wrap, Width = 50, Height = 25 } )
                                      {
                                          new TestNode( out var rootBa, new TestPlan { Width = 25, Height = 20 } ),
                                          new TestNode( out var rootBb, new TestPlan { Width = 25, Height = 10 } ),
                                          new TestNode( out var rootBc, new TestPlan { Width = 25, Height = 20 } ),
                                          new TestNode( out var root_1_3, new TestPlan { Width = 25, Height = 10 } )
                                      }
                              });

        Assert.True(true);
    }

    [Fact]
    public void a_margin_test()
    {
        var root = new TestNode {
           new TestNode(out var layer1, "A", new TestPlan { Width = 16, Height = 3, Margin = 1 })
        };

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║╔══════════════╗░░░░░░░░░░░░░░║
            ║║AAAAAAAAAAAAAA║░░░░░░░░░░░░░░║
            ║╚══════════════╝░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(1, 1, 16, 3), layer1.Layout.Bounds);
    }

    [Fact]
    public void a_padding_test()
    {
        var root = new TestNode()
           .Add(new TestNode(out var layer1, "A", new TestPlan { Padding = 1 })
                   .Add(new TestNode(out var layer2, "B", new TestPlan { Width = 5, Height = 3 }))
               );

        ElementArrange.Calculate(root);

        Draw(root,32,16);
        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │╔═══╗════════════════════════╗│
            │║BBB║AAAAAAAAAAAAAAAAAAAAAAAA║│
            │╚═══╝════════════════════════╝│
            └──────────────────────────────┘
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 5), layer1.Layout.Bounds);
        Assert.Equal(new RectF(1, 1, 5, 3), layer2.Layout.Bounds);
    }

    [Fact]
    public void a_layout_test()
    {
        var framePlan = new TestPlan
        {
            AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceBetween,
            Align_Cross = AlignmentLine_Cross.Stretch,
            Flex = 1,
            FlexGrow = 1
        };

        var root = new TestNode(
                new TestPlan
                {
                    AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceBetween,
                    Align_Cross = AlignmentLine_Cross.Stretch,
                    Flex = 1,
                    FlexGrow = 1
                })
                  .Add(new TestNode(out var frame1, "A", new TestPlan
                  {
                      AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceBetween,
                      Align_Cross = AlignmentLine_Cross.Stretch,
                      Flex = 1,
                      FlexGrow = 1,
                  }))
                  .Add(new TestNode(out var frame2, "B", new TestPlan
                  {
                      AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceBetween,
                      Align_Cross = AlignmentLine_Cross.Stretch,
                      Flex = 1,
                      FlexGrow = 1,
                  })
                          .Add(new TestNode(out var text, "C", new TestPlan
                              {
                                      //Height = 1,
                                      IsText = true,
                                  Margin = 1,
                                  Padding = 1,
                              })
                          {
                              MeasureFunc = (n, w, wm, h, hm) => new VecF(10, 1)
                          })
                      );

        ElementArrange.Calculate(root, 20, 20);

        Draw(root,32,16);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
            ╚══════════════════════════════╝
            ╔══════════════════════════════╗
            ║┌────────────────────────────┐║
            ║│╔══════════════════════════╗│║
            ║└────────────────────────────┘║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 8), frame1.Layout.Bounds);
        Assert.Equal(new RectF(0, 8, 32, 8), frame2.Layout.Bounds);
        Assert.Equal(new RectF(1, 9, 30, 3), text.Layout.Bounds);

    }

    [Fact]
    public void a_simple_layout_test()
    {
        var framePlan = new TestPlan
        {
            AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceBetween,
            Align_Cross = AlignmentLine_Cross.Stretch,
            Flex = 1,
            FlexGrow = 1
        };

        var root = new TestNode ( framePlan )
           .Add(new TestNode(out var text, "A",
               new TestPlan
               {
                   Margin = 1,
                   Padding = 1,
                   Width = 10,
                   Height = 5
               }));

        ElementArrange.Calculate(root, 32, 16);

        Draw(root);
        Assert.Equal(
            """
            ╔══════════════════════════════╗
            ║┌────────┐░░░░░░░░░░░░░░░░░░░░║
            ║│╔══════╗│░░░░░░░░░░░░░░░░░░░░║
            ║│║AAAAAA║│░░░░░░░░░░░░░░░░░░░░║
            ║│╚══════╝│░░░░░░░░░░░░░░░░░░░░║
            ║└────────┘░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
            ╚══════════════════════════════╝
            """,
            Canvas!.ToString());

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(1, 1, 10, 5), text.Layout.Bounds);

    }

}
