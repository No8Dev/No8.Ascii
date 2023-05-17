using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Layouts;
using Xunit;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Training;

public class TrainingVert : BaseTests
{
    public TrainingVert(ITestOutputHelper context) : base(context) { }


    [Fact]
    public void Training_vertical()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 16,
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),                           
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 8, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 8, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────┐
│╔══════╗═════╗│
│║AAAAAA║░░░░░║│
│║AAAAAA║░░░░░║│
│╚══════╝░░░░░║│
│╔══════╗░░░░░║│
│║BBBBBB║░░░░░║│
│║BBBBBB║░░░░░║│
│╚══════╝░░░░░║│
│╔══════╗░░░░░║│
│║CCCCCC║░░░░░║│
│║CCCCCC║░░░░░║│
│╚══════╝░░░░░║│
│║░░░░░░░░░░░░║│
│╚════════════╝│
└──────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_vertical_relative_position()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 16,
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),                           
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 3, Start = 2 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 3, Top = 1 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 3, End = 2 }),
                       new TestNode("D", new LayoutPlan { Width = 6, Height = 3, Bottom = 1 }),
                       new TestNode("E", new LayoutPlan { Width = 6, Height = 3 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────┐
│╔═╔════╗═════╗│
│║░║AAAA║░░░░░║│
│║░╚════╝░░░░░║│
│║░░░░░░░░░░░░║│
│╔════╗░░░░░░░║│
│║BBBB║░░░░░░░║│
════╗═╝░░░░░░░║│
CCCC║░░░░░░░░░║│
═╔════╗░░░░░░░║│
│║DDDD║░░░░░░░║│
│╚════╝░░░░░░░║│
│║░░░░░░░░░░░░║│
│╔════╗░░░░░░░║│
│║EEEE║═══════╝│
└╚════╝────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_vertical_absolute_position()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4, PositionType = PositionType.Absolute, Start = 14, Top = 6 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 4, PositionType = PositionType.Absolute, Top = 1, End = 1 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 4, PositionType = PositionType.Absolute, End = 1, Bottom = 1 }),
                       new TestNode("D", new LayoutPlan { Width = 6, Height = 4, PositionType = PositionType.Absolute, Start = 1, Bottom = 1 }),
                       new TestNode("E", new LayoutPlan { Width = 6, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 32, 17);

        Draw(root, 32, 17);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔════╗══════════════════╔════╗│
│║EEEE║░░░░░░░░░░░░░░░░░░║BBBB║│
│║EEEE║░░░░░░░░░░░░░░░░░░║BBBB║│
│╚════╝░░░░░░░░░░░░░░░░░░╚════╝│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│║░░░░░░░░░░░░╔════╗░░░░░░░░░░║│
│║░░░░░░░░░░░░║AAAA║░░░░░░░░░░║│
│║░░░░░░░░░░░░║AAAA║░░░░░░░░░░║│
│║░░░░░░░░░░░░╚════╝░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╔════╗░░░░░░░░░░░░░░░░░░╔════╗│
│║DDDD║░░░░░░░░░░░░░░░░░░║CCCC║│
│║DDDD║░░░░░░░░░░░░░░░░░░║CCCC║│
│╚════╝══════════════════╚════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }


    [Fact]
    public void Training_items_align_center()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 16,
                           Height = 20,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           
                           AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.Center
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────┐
│╔════════════╗│
│║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║│
│╔════╗░░░░░░░║│
│║AAAA║░░░░░░░║│
│║AAAA║░░░░░░░║│
│╚════╝░░░░░░░║│
│╔════╗░░░░░░░║│
│║BBBB║░░░░░░░║│
│║BBBB║░░░░░░░║│
│╚════╝░░░░░░░║│
│╔════╗░░░░░░░║│
│║CCCC║░░░░░░░║│
│║CCCC║░░░░░░░║│
│╚════╝░░░░░░░║│
│║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║│
│╚════════════╝│
└──────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_items_align_end()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 16,
                           Height = 20,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           
                           AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.End
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────┐
│╔════════════╗│
│║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║│
│╔════╗░░░░░░░║│
│║AAAA║░░░░░░░║│
│║AAAA║░░░░░░░║│
│╚════╝░░░░░░░║│
│╔════╗░░░░░░░║│
│║BBBB║░░░░░░░║│
│║BBBB║░░░░░░░║│
│╚════╝░░░░░░░║│
│╔════╗░░░░░░░║│
│║CCCC║░░░░░░░║│
│║CCCC║░░░░░░░║│
│╚════╝═══════╝│
└──────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_items_align_spacearound()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 16,
                           Height = 20,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           
                           AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceAround
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────┐
│╔════════════╗│
│╔════╗░░░░░░░║│
│║AAAA║░░░░░░░║│
│║AAAA║░░░░░░░║│
│╚════╝░░░░░░░║│
│║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║│
│╔════╗░░░░░░░║│
│║BBBB║░░░░░░░║│
│║BBBB║░░░░░░░║│
│╚════╝░░░░░░░║│
│║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║│
│╔════╗░░░░░░░║│
│║CCCC║░░░░░░░║│
│║CCCC║░░░░░░░║│
│╚════╝░░░░░░░║│
│╚════════════╝│
└──────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_items_align_spacebetween()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 16,
                           Height = 20,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceBetween
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────┐
│╔════╗═══════╗│
│║AAAA║░░░░░░░║│
│║AAAA║░░░░░░░║│
│╚════╝░░░░░░░║│
│║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║│
│╔════╗░░░░░░░║│
│║BBBB║░░░░░░░║│
│║BBBB║░░░░░░░║│
│╚════╝░░░░░░░║│
│║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║│
│║░░░░░░░░░░░░║│
│╔════╗░░░░░░░║│
│║CCCC║░░░░░░░║│
│║CCCC║░░░░░░░║│
│╚════╝═══════╝│
└──────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_items_align_spaceevenly()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 16,
                           Height = 18,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           
                           AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceEvenly
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────┐
│╔════════════╗│
│╔════╗░░░░░░░║│
│║AAAA║░░░░░░░║│
│║AAAA║░░░░░░░║│
│╚════╝░░░░░░░║│
│║░░░░░░░░░░░░║│
│╔════╗░░░░░░░║│
│║BBBB║░░░░░░░║│
│║BBBB║░░░░░░░║│
│╚════╝░░░░░░░║│
│║░░░░░░░░░░░░║│
│╔════╗░░░░░░░║│
│║CCCC║░░░░░░░║│
│║CCCC║░░░░░░░║│
│╚════╝░░░░░░░║│
│╚════════════╝│
└──────────────┘",
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
                           
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("F", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("G", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("H", new LayoutPlan { Width = 6, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔════╗╔════╗╔════╗═══════════╗│
│║AAAA║║DDDD║║GGGG║░░░░░░░░░░░║│
│║AAAA║║DDDD║║GGGG║░░░░░░░░░░░║│
│╚════╝╚════╝╚════╝░░░░░░░░░░░║│
│╔════╗╔════╗╔════╗░░░░░░░░░░░║│
│║BBBB║║EEEE║║HHHH║░░░░░░░░░░░║│
│║BBBB║║EEEE║║HHHH║░░░░░░░░░░░║│
│╚════╝╚════╝╚════╝░░░░░░░░░░░║│
│╔════╗╔════╗░░░░░░░░░░░░░░░░░║│
│║CCCC║║FFFF║░░░░░░░░░░░░░░░░░║│
│║CCCC║║FFFF║░░░░░░░░░░░░░░░░░║│
│╚════╝╚════╝░░░░░░░░░░░░░░░░░║│
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
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           
                           AlignElements_Cross = Alignment_Cross.Center
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("F", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("G", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("H", new LayoutPlan { Width = 6, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔═════╔════╗╔════╗╔════╗═════╗│
│║░░░░░║AAAA║║DDDD║║GGGG║░░░░░║│
│║░░░░░║AAAA║║DDDD║║GGGG║░░░░░║│
│║░░░░░╚════╝╚════╝╚════╝░░░░░║│
│║░░░░░╔════╗╔════╗╔════╗░░░░░║│
│║░░░░░║BBBB║║EEEE║║HHHH║░░░░░║│
│║░░░░░║BBBB║║EEEE║║HHHH║░░░░░║│
│║░░░░░╚════╝╚════╝╚════╝░░░░░║│
│║░░░░░╔════╗╔════╗░░░░░░░░░░░║│
│║░░░░░║CCCC║║FFFF║░░░░░░░░░░░║│
│║░░░░░║CCCC║║FFFF║░░░░░░░░░░░║│
│║░░░░░╚════╝╚════╝░░░░░░░░░░░║│
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
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           
                           AlignElements_Cross = Alignment_Cross.End
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("F", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("G", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("H", new LayoutPlan { Width = 6, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────────────────────┐
│╔═══════════╔════╗╔════╗╔════╗│
│║░░░░░░░░░░░║AAAA║║DDDD║║GGGG║│
│║░░░░░░░░░░░║AAAA║║DDDD║║GGGG║│
│║░░░░░░░░░░░╚════╝╚════╝╚════╝│
│║░░░░░░░░░░░╔════╗╔════╗╔════╗│
│║░░░░░░░░░░░║BBBB║║EEEE║║HHHH║│
│║░░░░░░░░░░░║BBBB║║EEEE║║HHHH║│
│║░░░░░░░░░░░╚════╝╚════╝╚════╝│
│║░░░░░░░░░░░╔════╗╔════╗░░░░░║│
│║░░░░░░░░░░░║CCCC║║FFFF║░░░░░║│
│║░░░░░░░░░░░║CCCC║║FFFF║░░░░░║│
│║░░░░░░░░░░░╚════╝╚════╝░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════════╝│
└──────────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_spacearound()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 26,
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           
                           AlignElements_Cross = Alignment_Cross.SpaceAround
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("F", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("G", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("H", new LayoutPlan { Width = 6, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌────────────────────────┐
│╔╔════╗══╔════╗══╔════╗╗│
│║║AAAA║░░║DDDD║░░║GGGG║║│
│║║AAAA║░░║DDDD║░░║GGGG║║│
│║╚════╝░░╚════╝░░╚════╝║│
│║╔════╗░░╔════╗░░╔════╗║│
│║║BBBB║░░║EEEE║░░║HHHH║║│
│║║BBBB║░░║EEEE║░░║HHHH║║│
│║╚════╝░░╚════╝░░╚════╝║│
│║╔════╗░░╔════╗░░░░░░░░║│
│║║CCCC║░░║FFFF║░░░░░░░░║│
│║║CCCC║░░║FFFF║░░░░░░░░║│
│║╚════╝░░╚════╝░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░║│
│╚══════════════════════╝│
└────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_spacebetween()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 28,
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           
                           AlignElements_Cross = Alignment_Cross.SpaceBetween
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("F", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("G", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("H", new LayoutPlan { Width = 6, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────────────────┐
│╔════╗════╔════╗════╔════╗│
│║AAAA║░░░░║DDDD║░░░░║GGGG║│
│║AAAA║░░░░║DDDD║░░░░║GGGG║│
│╚════╝░░░░╚════╝░░░░╚════╝│
│╔════╗░░░░╔════╗░░░░╔════╗│
│║BBBB║░░░░║EEEE║░░░░║HHHH║│
│║BBBB║░░░░║EEEE║░░░░║HHHH║│
│╚════╝░░░░╚════╝░░░░╚════╝│
│╔════╗░░░░╔════╗░░░░░░░░░║│
│║CCCC║░░░░║FFFF║░░░░░░░░░║│
│║CCCC║░░░░║FFFF║░░░░░░░░░║│
│╚════╝░░░░╚════╝░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚════════════════════════╝│
└──────────────────────────┘",
            Canvas!.ToString());
    }

    [Fact(Skip = "AlignmentCross.SpaceEvenly doesn't work")]
    public void Training_content_spaceevenly()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 32,
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           AlignElements_Cross = Alignment_Cross.SpaceEvenly
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("F", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("G", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("H", new LayoutPlan { Width = 6, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"This doesn't work yet",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_children_start()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 16,
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           
                           Align_Cross = AlignmentLine_Cross.Start
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 10, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────┐
│╔════╗═══════╗│
│║AAAA║░░░░░░░║│
│║AAAA║░░░░░░░║│
│╚════╝░░░░░░░║│
│╔════════╗░░░║│
│║BBBBBBBB║░░░║│
│║BBBBBBBB║░░░║│
│╚════════╝░░░║│
│╔══════╗░░░░░║│
│║CCCCCC║░░░░░║│
│║CCCCCC║░░░░░║│
│╚══════╝░░░░░║│
│║░░░░░░░░░░░░║│
│╚════════════╝│
└──────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_children_centre()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 16,
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           
                           Align_Cross = AlignmentLine_Cross.Center
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 10, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌──────────────┐
│╔═╔════╗═════╗│
│║░║AAAA║░░░░░║│
│║░║AAAA║░░░░░║│
│║░╚════╝░░░░░║│
│╔════════╗░░░║│
│║BBBBBBBB║░░░║│
│║BBBBBBBB║░░░║│
│╚════════╝░░░║│
│║╔══════╗░░░░║│
│║║CCCCCC║░░░░║│
│║║CCCCCC║░░░░║│
│║╚══════╝░░░░║│
│║░░░░░░░░░░░░║│
│╚════════════╝│
└──────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_children_end()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 16,
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           
                           Align_Cross = AlignmentLine_Cross.End
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 10, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 4 }),
                   };

        ElementArrange.Calculate(root, 32, 11);

        Draw(root, 32, 17);
        Assert.Equal(
            @"
┌──────────────┐
│╔═══╔════╗═══╗│
│║░░░║AAAA║░░░║│
│║░░░║AAAA║░░░║│
│║░░░╚════╝░░░║│
│╔════════╗░░░║│
│║BBBBBBBB║░░░║│
│║BBBBBBBB║░░░║│
│╚════════╝░░░║│
│║░╔══════╗░░░║│
│║░║CCCCCC║░░░║│
│║░║CCCCCC║░░░║│
│║░╚══════╝░░░║│
│║░░░░░░░░░░░░║│
│╚════════════╝│
└──────────────┘",
            Canvas!.ToString());
    }

    [Fact]
    public void Training_content_multipline_children_end()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           Width = 34,
                           Height = 16,
                           Margin = new Sides(0, 1, 0, 0),
                           Padding = new Sides(1),
                           ElementsWrap = LayoutWrap.Wrap,
                           Align_Cross = AlignmentLine_Cross.End
                       })
                   {
                       new TestNode("A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("B", new LayoutPlan { Width = 10, Height = 4 }),
                       new TestNode("C", new LayoutPlan { Width = 8, Height = 4 }),
                       new TestNode("D", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("E", new LayoutPlan { Width = 10, Height = 4 }),
                       new TestNode("F", new LayoutPlan { Width = 8, Height = 4 }),
                       new TestNode("G", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode("H", new LayoutPlan { Width = 10, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"
┌────────────────────────────────┐
│╔═══╔════╗════╔════╗════╔════╗═╗│
│║░░░║AAAA║░░░░║DDDD║░░░░║GGGG║░║│
│║░░░║AAAA║░░░░║DDDD║░░░░║GGGG║░║│
│║░░░╚════╝░░░░╚════╝░░░░╚════╝░║│
│╔════════╗╔════════╗╔════════╗░║│
│║BBBBBBBB║║EEEEEEEE║║HHHHHHHH║░║│
│║BBBBBBBB║║EEEEEEEE║║HHHHHHHH║░║│
│╚════════╝╚════════╝╚════════╝░║│
│║░╔══════╗░░╔══════╗░░░░░░░░░░░║│
│║░║CCCCCC║░░║FFFFFF║░░░░░░░░░░░║│
│║░║CCCCCC║░░║FFFFFF║░░░░░░░░░░░║│
│║░╚══════╝░░╚══════╝░░░░░░░░░░░║│
│║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║│
│╚══════════════════════════════╝│
└────────────────────────────────┘",
            Canvas!.ToString());
    }
}
