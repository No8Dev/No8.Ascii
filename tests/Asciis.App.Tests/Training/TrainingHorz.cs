using Asciis.App.Controls;
using Asciis.App.ElementLayout;
using Asciis.App.Tests.Layouts;
using Xunit;
using Xunit.Abstractions;

namespace Asciis.App.Tests.Training;

public class TrainingHorz : BaseTests
{
    public TrainingHorz(ITestOutputHelper context) : base(context) { }


    [Fact]
    public void Training_horizontal()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 8,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsDirection = LayoutDirection.Horz,
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 4, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 32, 9);

        Draw(root, 32, 9);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔══╗╔══╗╔══╗╔══╗╔══╗═════════╗│
│║AA║║BB║║CC║║DD║║EE║░░░░░░░░░║│
│║AA║║BB║║CC║║DD║║EE║░░░░░░░░░║│
│╚══╝╚══╝╚══╝╚══╝╚══╝░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_horizontal_relative_position()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 8,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsDirection = LayoutDirection.Horz,
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 4, Height = 4, Start = 1 }),
                       new TestNode("B", new LayoutPlan { Width = 4, Height = 4, Top = 1 }),
                       new TestNode("C", new LayoutPlan { Width = 4, Height = 4, End = 1 }),
                       new TestNode("D", new LayoutPlan { Width = 4, Height = 4, Bottom = 1 }),
                       new TestNode("E", new LayoutPlan { Width = 4, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 32, 9);

        Draw(root, 32, 9);
        Assert.Equal(
            @"
┌────────────╔══╗──────────────┐
│╔╔══╗══╔══╗═║DD║╔══╗═════════╗│
│║║AA╔══║CC║░║DD║║EE║░░░░░░░░░║│
│║║AA║BB║CC║░╚══╝║EE║░░░░░░░░░║│
│║╚══║BB╚══╝░░░░░╚══╝░░░░░░░░░║│
│║░░░╚══╝░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_horizontal_absolute_position()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsDirection = LayoutDirection.Horz,
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 4, Height = 4, PositionType = PositionType.Absolute, Start = 14, Top = 6 }),
                       new TestNode("B", new LayoutPlan { Width = 4, Height = 4, PositionType = PositionType.Absolute, Top = 1, End = 1 }),
                       new TestNode("C", new LayoutPlan { Width = 4, Height = 4, PositionType = PositionType.Absolute, End = 1, Bottom = 1 }),
                       new TestNode("D", new LayoutPlan { Width = 4, Height = 4, PositionType = PositionType.Absolute, Start = 1, Bottom = 1 }),
                       new TestNode("E", new LayoutPlan { Width = 4, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 32, 17);

        Draw(root, 32, 17);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔══╗══════════════════════╔══╗│
│║EE║░░░░░░░░░░░░░░░░░░░░░░║BB║│
│║EE║░░░░░░░░░░░░░░░░░░░░░░║BB║│
│╚══╝░░░░░░░░░░░░░░░░░░░░░░╚══╝│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░╔══╗░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║AA║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║AA║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░╚══╝░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╔══╗░░░░░░░░░░░░░░░░░░░░░░╔══╗│
│║DD║░░░░░░░░░░░░░░░░░░░░░░║CC║│
│║DD║░░░░░░░░░░░░░░░░░░░░░░║CC║│
│╚══╝══════════════════════╚══╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }


    [Fact]
    public void Training_items_align_center()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 8,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 4, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 32, 9);

        Draw(root, 32, 9);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔════╔══╗╔══╗╔══╗╔══╗╔══╗════╗│
│║░░░░║AA║║BB║║CC║║DD║║EE║░░░░║│
│║░░░░║AA║║BB║║CC║║DD║║EE║░░░░║│
│║░░░░╚══╝╚══╝╚══╝╚══╝╚══╝░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_items_align_end()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 8,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.End
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 4, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 32, 9);

        Draw(root, 32, 9);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔═════════╔══╗╔══╗╔══╗╔══╗╔══╗│
│║░░░░░░░░░║AA║║BB║║CC║║DD║║EE║│
│║░░░░░░░░░║AA║║BB║║CC║║DD║║EE║│
│║░░░░░░░░░╚══╝╚══╝╚══╝╚══╝╚══╝│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_items_align_spacearound()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 8,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceAround
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 4, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 32, 9);

        Draw(root, 32, 9);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔╔══╗══╔══╗══╔══╗══╔══╗══╔══╗╗│
│║║AA║░░║BB║░░║CC║░░║DD║░░║EE║║│
│║║AA║░░║BB║░░║CC║░░║DD║░░║EE║║│
│║╚══╝░░╚══╝░░╚══╝░░╚══╝░░╚══╝║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_items_align_spacebetween()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 30,
                           Height = 8,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceBetween
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 4, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 30, 9);

        Draw(root, 32, 9);
        Assert.Equal(
            @"
┌────────────────────────────┐
│╔══╗══╔══╗══╔══╗══╔══╗══╔══╗│
│║AA║░░║BB║░░║CC║░░║DD║░░║EE║│
│║AA║░░║BB║░░║CC║░░║DD║░░║EE║│
│╚══╝░░╚══╝░░╚══╝░░╚══╝░░╚══╝│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚══════════════════════════╝│
└────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_items_align_spaceevenly()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 28,
                           Height = 8,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceEvenly
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 4, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 4, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 28, 9);

        Draw(root, 32, 9);
        Assert.Equal(
            @"
┌──────────────────────────┐
│╔╔══╗═╔══╗═╔══╗═╔══╗═╔══╗╗│
│║║AA║░║BB║░║CC║░║DD║░║EE║║│
│║║AA║░║BB║░║CC║░║DD║░║EE║║│
│║╚══╝░╚══╝░╚══╝░╚══╝░╚══╝║│
│║░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════╝│
└──────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           ElementsDirection = LayoutDirection.Horz,
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("B", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("D", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("E", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("F", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("G", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("H", new LayoutPlan { Width = 8, Height = 3 }),
                   };

        ElementArrange.Calculate(root, 32, 17);

        Draw(root, 32, 17);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔══════╗╔══════╗╔══════╗═════╗│
│║AAAAAA║║BBBBBB║║CCCCCC║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│╔══════╗╔══════╗╔══════╗░░░░░║│
│║DDDDDD║║EEEEEE║║FFFFFF║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│╔══════╗╔══════╗░░░░░░░░░░░░░║│
│║GGGGGG║║HHHHHH║░░░░░░░░░░░░░║│
│╚══════╝╚══════╝░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_centre()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 15,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_Cross = Alignment_Cross.Center
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("B", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("D", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("E", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("F", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("G", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("H", new LayoutPlan { Width = 8, Height = 3 }),
                   };

        ElementArrange.Calculate(root, 32, 17);

        Draw(root, 32, 17);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔════════════════════════════╗│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╔══════╗╔══════╗╔══════╗░░░░░║│
│║AAAAAA║║BBBBBB║║CCCCCC║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│╔══════╗╔══════╗╔══════╗░░░░░║│
│║DDDDDD║║EEEEEE║║FFFFFF║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│╔══════╗╔══════╗░░░░░░░░░░░░░║│
│║GGGGGG║║HHHHHH║░░░░░░░░░░░░░║│
│╚══════╝╚══════╝░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_end()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 15,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_Cross = Alignment_Cross.End
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("B", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("D", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("E", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("F", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("G", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("H", new LayoutPlan { Width = 8, Height = 3 }),
                   };

        ElementArrange.Calculate(root, 32, 17);

        Draw(root, 32, 17);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔════════════════════════════╗│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╔══════╗╔══════╗╔══════╗░░░░░║│
│║AAAAAA║║BBBBBB║║CCCCCC║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│╔══════╗╔══════╗╔══════╗░░░░░║│
│║DDDDDD║║EEEEEE║║FFFFFF║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│╔══════╗╔══════╗░░░░░░░░░░░░░║│
│║GGGGGG║║HHHHHH║░░░░░░░░░░░░░║│
│╚══════╝╚══════╝═════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_spacearound()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 17,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_Cross = Alignment_Cross.SpaceAround
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("B", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("D", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("E", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("F", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("G", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("H", new LayoutPlan { Width = 8, Height = 3 }),
                   };

        ElementArrange.Calculate(root, 32, 18);

        Draw(root, 32, 18);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔════════════════════════════╗│
│╔══════╗╔══════╗╔══════╗░░░░░║│
│║AAAAAA║║BBBBBB║║CCCCCC║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╔══════╗╔══════╗╔══════╗░░░░░║│
│║DDDDDD║║EEEEEE║║FFFFFF║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╔══════╗╔══════╗░░░░░░░░░░░░░║│
│║GGGGGG║║HHHHHH║░░░░░░░░░░░░░║│
│╚══════╝╚══════╝░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_spacebetween()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 15,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_Cross = Alignment_Cross.SpaceBetween
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("B", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("D", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("E", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("F", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("G", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("H", new LayoutPlan { Width = 8, Height = 3 }),
                   };

        ElementArrange.Calculate(root, 32, 17);

        Draw(root, 32, 17);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔══════╗╔══════╗╔══════╗═════╗│
│║AAAAAA║║BBBBBB║║CCCCCC║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╔══════╗╔══════╗╔══════╗░░░░░║│
│║DDDDDD║║EEEEEE║║FFFFFF║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╔══════╗╔══════╗░░░░░░░░░░░░░║│
│║GGGGGG║║HHHHHH║░░░░░░░░░░░░░║│
│╚══════╝╚══════╝═════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact(Skip = "AlignContentCross = AlignmentCross.SpaceEvenly doesn't work correctly")]
    public void Training_content_spaceevenly()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 15,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_Cross = Alignment_Cross.SpaceEvenly
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("B", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("D", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("E", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("F", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("G", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("H", new LayoutPlan { Width = 8, Height = 3 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @" This one doesn't work yet
┌──────────────────────────────┐
│╔════════════════════════════╗│
│╔══════╗╔══════╗╔══════╗░░░░░║│
│║AAAAAA║║BBBBBB║║CCCCCC║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╔══════╗╔══════╗╔══════╗░░░░░║│
│║DDDDDD║║EEEEEE║║FFFFFF║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╔══════╗╔══════╗░░░░░░░░░░░░░║│
│║GGGGGG║║HHHHHH║░░░░░░░░░░░░░║│
│╚══════╝╚══════╝░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_children_start()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 10,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           ElementsDirection = LayoutDirection.Horz,
                           Align_Cross = AlignmentLine_Cross.Start
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("B", new LayoutPlan { Width = 8, Height = 6 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 32, 11);

        Draw(root, 32, 11);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔══════╗╔══════╗╔══════╗═════╗│
│║AAAAAA║║BBBBBB║║CCCCCC║░░░░░║│
│╚══════╝║BBBBBB║║CCCCCC║░░░░░║│
│║░░░░░░░║BBBBBB║╚══════╝░░░░░║│
│║░░░░░░░║BBBBBB║░░░░░░░░░░░░░║│
│║░░░░░░░╚══════╝░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_children_centre()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 10,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           ElementsDirection = LayoutDirection.Horz,
                           Align_Cross = AlignmentLine_Cross.Center
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("B", new LayoutPlan { Width = 8, Height = 6 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 32, 11);

        Draw(root, 32, 11);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔═══════╔══════╗═════════════╗│
│║░░░░░░░║BBBBBB║╔══════╗░░░░░║│
│╔══════╗║BBBBBB║║CCCCCC║░░░░░║│
│║AAAAAA║║BBBBBB║║CCCCCC║░░░░░║│
│╚══════╝║BBBBBB║╚══════╝░░░░░║│
│║░░░░░░░╚══════╝░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_children_end()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 10,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           ElementsDirection = LayoutDirection.Horz,
                           Align_Cross = AlignmentLine_Cross.End
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("B", new LayoutPlan { Width = 8, Height = 6 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 32, 11);

        Draw(root, 32, 17);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔═══════╔══════╗═════════════╗│
│║░░░░░░░║BBBBBB║░░░░░░░░░░░░░║│
│║░░░░░░░║BBBBBB║╔══════╗░░░░░║│
│╔══════╗║BBBBBB║║CCCCCC║░░░░░║│
│║AAAAAA║║BBBBBB║║CCCCCC║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_multipline_children_end()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 20,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           ElementsDirection = LayoutDirection.Horz,
                           Align_Cross = AlignmentLine_Cross.End
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("B", new LayoutPlan { Width = 8, Height = 6 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("E", new LayoutPlan { Width = 8, Height = 6 }),
                       new TestNode("F", new LayoutPlan { Width = 8, Height = 4 }),
                       new TestNode("G", new LayoutPlan { Width = 8, Height = 3 }),
                       new TestNode("H", new LayoutPlan { Width = 8, Height = 6 }),
                   };

        ElementArrange.Calculate(root, 32, 21);

        Draw(root, 32, 21);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔═══════╔══════╗═════════════╗│
│║░░░░░░░║BBBBBB║░░░░░░░░░░░░░║│
│║░░░░░░░║BBBBBB║╔══════╗░░░░░║│
│╔══════╗║BBBBBB║║CCCCCC║░░░░░║│
│║AAAAAA║║BBBBBB║║CCCCCC║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│║░░░░░░░╔══════╗░░░░░░░░░░░░░║│
│║░░░░░░░║EEEEEE║░░░░░░░░░░░░░║│
│║░░░░░░░║EEEEEE║╔══════╗░░░░░║│
│╔══════╗║EEEEEE║║FFFFFF║░░░░░║│
│║DDDDDD║║EEEEEE║║FFFFFF║░░░░░║│
│╚══════╝╚══════╝╚══════╝░░░░░║│
│║░░░░░░░╔══════╗░░░░░░░░░░░░░║│
│║░░░░░░░║HHHHHH║░░░░░░░░░░░░░║│
│║░░░░░░░║HHHHHH║░░░░░░░░░░░░░║│
│╔══════╗║HHHHHH║░░░░░░░░░░░░░║│
│║GGGGGG║║HHHHHH║░░░░░░░░░░░░░║│
│╚══════╝╚══════╝═════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }
}
