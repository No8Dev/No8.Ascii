using No8.Ascii.Console;
using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Layouts;
using Xunit;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Training;

public class TrainingFlex : BaseTests
{
    public TrainingFlex(ITestOutputHelper context) : base(context) { }

    [Fact]
    public void Training_flex()
    {
        var root = new TestNode(new LayoutPlan { Width = 80, Height = 25, Margin = new Sides(0, 1, 0, 0) })
                {
                    new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Height=4 })
                    {
                        new TestNode("A", new LayoutPlan { Width = 15, Height = 4 }),
                        new TestNode("B", new LayoutPlan { Width = 4, Height = 4 }),
                        new TestNode("C", new LayoutPlan { Width = 15, Height = 4 }),
                    },
                    new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz })
                    {
                        new TestNode("A", new LayoutPlan { Width = 15, Height = 4 }),
                        new TestNode("B", new LayoutPlan { Width = 4, Height = 4, FlexGrow = 1 }),
                        new TestNode("C", new LayoutPlan { Width = 15, Height = 4 }),
                    },
                    new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz })
                    {
                        new TestNode("A", new LayoutPlan { Width = 15, Height = 4 }),
                        new TestNode("B", new LayoutPlan { Width = 4, Height = 4, FlexGrow = 1 }),
                        new TestNode("C", new LayoutPlan { Width = 4, Height = 4, FlexGrow = 1 }),
                        new TestNode("D", new LayoutPlan { Width = 15, Height = 4 }),
                    },
                    new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz })
                    {
                        new TestNode("A", new LayoutPlan { Width = 15, Height = 4 }),
                        new TestNode("B", new LayoutPlan { Width = 40, Height = 4, FlexShrink = 1 }),
                        new TestNode("C", new LayoutPlan { Width = 40, Height = 4, FlexShrink = 1 }),
                        new TestNode("D", new LayoutPlan { Width = 15, Height = 4 }),
                    },


                    new TestNode(new LayoutPlan {Width=100.Percent(), Height = 3, Bottom = 0, PositionType = PositionType.Absolute, ElementsDirection=LayoutDirection.Horz } )
                    {
                        new TestNode("A", new LayoutPlan { Width = 10 }),
                        new TestNode(" ", new LayoutPlan { Width = 10, FlexGrow=1 }),
                        new TestNode("C", new LayoutPlan { Width = 10, }),
                    }
                };

        ElementArrange.Calculate(root, 80,26);

        Draw(root, 80, 26);
        Assert.Equal(
            @"
╔═════════════╗╔══╗╔═════════════╗═════════════════════════════════════════════╗
║AAAAAAAAAAAAA║║BB║║CCCCCCCCCCCCC║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║AAAAAAAAAAAAA║║BB║║CCCCCCCCCCCCC║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚═════════════╝╚══╝╚═════════════╝═════════════════════════════════════════════╝
╔═════════════╗╔════════════════════════════════════════════════╗╔═════════════╗
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║║CCCCCCCCCCCCC║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB║║CCCCCCCCCCCCC║
╚═════════════╝╚════════════════════════════════════════════════╝╚═════════════╝
╔═════════════╗╔═══════════════════════╗╔═══════════════════════╗╔═════════════╗
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBBBBBBBBBB║║CCCCCCCCCCCCCCCCCCCCCCC║║DDDDDDDDDDDDD║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBBBBBBBBBB║║CCCCCCCCCCCCCCCCCCCCCCC║║DDDDDDDDDDDDD║
╚═════════════╝╚═══════════════════════╝╚═══════════════════════╝╚═════════════╝
╔═════════════╗╔═══════════════════════╗╔═══════════════════════╗╔═════════════╗
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBBBBBBBBBB║║CCCCCCCCCCCCCCCCCCCCCCC║║DDDDDDDDDDDDD║
║AAAAAAAAAAAAA║║BBBBBBBBBBBBBBBBBBBBBBB║║CCCCCCCCCCCCCCCCCCCCCCC║║DDDDDDDDDDDDD║
╚═════════════╝╚═══════════════════════╝╚═══════════════════════╝╚═════════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔════════╗╔══════════════════════════════════════════════════════════╗╔════════╗
║AAAAAAAA║║                                                          ║║CCCCCCCC║
╚════════╝╚══════════════════════════════════════════════════════════╝╚════════╝",
            Canvas!.ToString());
    }
}
