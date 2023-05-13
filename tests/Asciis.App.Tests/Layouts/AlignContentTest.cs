using Asciis.App.Controls;
using Asciis.App.ElementLayout;
using Xunit;
using Xunit.Abstractions;
// ReSharper disable StringLiteralTypo

namespace Asciis.App.Tests.Layouts;

[TestClass]
public class AlignContentTest : BaseTests
{
    public AlignContentTest(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void align_content_start()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           ElementsDirection = LayoutDirection.Horz,
                           ElementsWrap = LayoutWrap.Wrap,
                           Width = 32,
                           Height = 16
                       })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 10, Height = 4 }),
                       new TestNode(out var rootB, "B", new LayoutPlan { Width = 10, Height = 4 }),
                       new TestNode(out var rootC, "C", new LayoutPlan { Width = 10, Height = 4 }),
                       new TestNode(out var rootD, "D", new LayoutPlan { Width = 10, Height = 4 }),
                       new TestNode(out var rootE, "E", new LayoutPlan { Width = 10, Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        // Row wraps

        Draw(root);
        Assert.Equal(
            @"╔════════╗╔════════╗╔════════╗═╗
║AAAAAAAA║║BBBBBBBB║║CCCCCCCC║░║
║AAAAAAAA║║BBBBBBBB║║CCCCCCCC║░║
╚════════╝╚════════╝╚════════╝░║
╔════════╗╔════════╗░░░░░░░░░░░║
║DDDDDDDD║║EEEEEEEE║░░░░░░░░░░░║
║DDDDDDDD║║EEEEEEEE║░░░░░░░░░░░║
╚════════╝╚════════╝░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_start_without_height_on_elements()
    {
        var root = new TestNode(
            new LayoutPlan
            {
                ElementsDirection = LayoutDirection.Horz,
                ElementsWrap = LayoutWrap.Wrap,
                Width = 32,
                Height = 16
            })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 10 }),
                       new TestNode(out var rootB, "B", new LayoutPlan { Width = 10, Height = 4 }),
                       new TestNode(out var rootC, "C", new LayoutPlan { Width = 10 }),
                       new TestNode(out var rootD, "D", new LayoutPlan { Width = 10, Height = 6 }),
                       new TestNode(out var rootE, "E", new LayoutPlan { Width = 10 })
                   };

        ElementArrange.Calculate(root);

        // Each row height is determined by one element.
        // Each element in row stretches to row height

        Draw(root);
        Assert.Equal(
            @"╔════════╗╔════════╗╔════════╗═╗
║AAAAAAAA║║BBBBBBBB║║CCCCCCCC║░║
║AAAAAAAA║║BBBBBBBB║║CCCCCCCC║░║
╚════════╝╚════════╝╚════════╝░║
╔════════╗╔════════╗░░░░░░░░░░░║
║DDDDDDDD║║EEEEEEEE║░░░░░░░░░░░║
║DDDDDDDD║║EEEEEEEE║░░░░░░░░░░░║
║DDDDDDDD║║EEEEEEEE║░░░░░░░░░░░║
║DDDDDDDD║║EEEEEEEE║░░░░░░░░░░░║
╚════════╝╚════════╝░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_start_with_flex()
    {
        var root = new TestNode(
                    new LayoutPlan 
                    {
                        ElementsDirection = LayoutDirection.Horz,
                        ElementsWrap = LayoutWrap.Wrap, 
                        Width = 64, 
                        Height = 8 
                    })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { FlexGrow = 1, Width = 12, Height = 6 }),
                       new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 0.Percent() }),
                       new TestNode(out var rootC, "C", new LayoutPlan { Width = 12 }),
                       new TestNode(out var rootD, "D", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 0.Percent(), Width = 12 }),
                       new TestNode(out var rootE, "E", new LayoutPlan { Width = 12 })
                   };

        ElementArrange.Calculate(root);

        // All elements take the height of the tallest element
        // C & E are fixed with
        // A, B, D will grow to take remaining
        // A starts at 12 width + share of remaining
        // B, D start at 0% + share of remaining
        // ChildLayoutDirectionLength overrides Width when flexing

        Draw(root);
        Assert.Equal(
@"╔═══════════════════╗╔════════╗╔══════════╗╔═══════╗╔══════════╗
║AAAAAAAAAAAAAAAAAAA║║BBBBBBBB║║CCCCCCCCCC║║DDDDDDD║║EEEEEEEEEE║
║AAAAAAAAAAAAAAAAAAA║║BBBBBBBB║║CCCCCCCCCC║║DDDDDDD║║EEEEEEEEEE║
║AAAAAAAAAAAAAAAAAAA║║BBBBBBBB║║CCCCCCCCCC║║DDDDDDD║║EEEEEEEEEE║
║AAAAAAAAAAAAAAAAAAA║║BBBBBBBB║║CCCCCCCCCC║║DDDDDDD║║EEEEEEEEEE║
╚═══════════════════╝╚════════╝╚══════════╝╚═══════╝╚══════════╝
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════════════════════════════════════╝",
            Canvas!.ToString());
    }

    //[Ignore("Exactly the same results as the C++ library")]
    [Fact]
    public void align_content_end()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_Cross = Alignment_Cross.End,
                           ElementsWrap = LayoutWrap.Wrap,
                           Width = 32,
                           Height = 16
                       })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 6, Height = 8 }),
                       new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Height = 7 }),
                       new TestNode(out var rootC, "C", new LayoutPlan { Width = 10, Height = 6 }),
                       new TestNode(out var rootD, "D", new LayoutPlan { Width = 12, Height = 5 }),
                       new TestNode(out var rootE, "E", new LayoutPlan { Width = 14, Height = 4 })
                   };

        ElementArrange.Calculate(root);

        // Layout in rows
        // Each row is height of tallest element
        // Entire content is Alignment_Cross.End (bottom)

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔════╗╔══════╗╔════════╗░░░░░░░║
║AAAA║║BBBBBB║║CCCCCCCC║░░░░░░░║
║AAAA║║BBBBBB║║CCCCCCCC║░░░░░░░║
║AAAA║║BBBBBB║║CCCCCCCC║░░░░░░░║
║AAAA║║BBBBBB║║CCCCCCCC║░░░░░░░║
║AAAA║║BBBBBB║╚════════╝░░░░░░░║
║AAAA║╚══════╝░░░░░░░░░░░░░░░░░║
╚════╝░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════╗╔════════════╗░░░░░║
║DDDDDDDDDD║║EEEEEEEEEEEE║░░░░░║
║DDDDDDDDDD║║EEEEEEEEEEEE║░░░░░║
║DDDDDDDDDD║╚════════════╝░░░░░║
╚══════════╝═══════════════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           AlignElements_Cross = Alignment_Cross.Stretch,
                           ElementsWrap = LayoutWrap.Wrap,
                           Width = 32,
                           Height = 16
                       })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 6, Height = 4 }),
                       new TestNode(out var rootB, "B", new LayoutPlan { Width = 7, Height = 4 }),
                       new TestNode(out var rootC, "C", new LayoutPlan {            Height = 4 }),
                       new TestNode(out var rootD, "D", new LayoutPlan { Width = 9, Height = 4 }),
                       new TestNode(out var rootE, "E", new LayoutPlan { Width = 10, Height = 4 }),
                       new TestNode(out var rootF, "F", new LayoutPlan {             Height = 4 }),
                   };

        ElementArrange.Calculate(root);

        // Layout is Vert (columns)
        // Each column is stretched in cross direction (horz)
        // C, F element width is not specified so is streched

        Draw(root);
        Assert.Equal(
@"╔════╗══════════╔════════╗═════╗
║AAAA║░░░░░░░░░░║EEEEEEEE║░░░░░║
║AAAA║░░░░░░░░░░║EEEEEEEE║░░░░░║
╚════╝░░░░░░░░░░╚════════╝░░░░░║
╔═════╗░░░░░░░░░╔══════════════╗
║BBBBB║░░░░░░░░░║FFFFFFFFFFFFFF║
║BBBBB║░░░░░░░░░║FFFFFFFFFFFFFF║
╚═════╝░░░░░░░░░╚══════════════╝
╔══════════════╗░░░░░░░░░░░░░░░║
║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
║CCCCCCCCCCCCCC║░░░░░░░░░░░░░░░║
╚══════════════╝░░░░░░░░░░░░░░░║
╔═══════╗░░░░░░░░░░░░░░░░░░░░░░║
║DDDDDDD║░░░░░░░░░░░░░░░░░░░░░░║
║DDDDDDD║░░░░░░░░░░░░░░░░░░░░░░║
╚═══════╝══════════════════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_spacebetween()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_Cross = Alignment_Cross.SpaceBetween,
                           ElementsWrap = LayoutWrap.Wrap,
                           Width = 32,
                           Height = 16
                       })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 12, Height = 4 }),
                       new TestNode(out var rootB, "B", new LayoutPlan { Width = 12, Height = 4 }),
                       new TestNode(out var rootC, "C", new LayoutPlan { Width = 12, Height = 4 }),
                       new TestNode(out var rootD, "D", new LayoutPlan { Width = 12, Height = 4 }),
                       new TestNode(out var rootE, "E", new LayoutPlan { Width = 12, Height = 4 })
                   };


        ElementArrange.Calculate(root);

        // Layout is Horz (rows)
        // Each row is spaced out with even amounts between each row

        Draw(root);
        Assert.Equal(
            @"╔══════════╗╔══════════╗═══════╗
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░║
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░║
╚══════════╝╚══════════╝░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════╗╔══════════╗░░░░░░░║
║CCCCCCCCCC║║DDDDDDDDDD║░░░░░░░║
║CCCCCCCCCC║║DDDDDDDDDD║░░░░░░░║
╚══════════╝╚══════════╝░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════╗░░░░░░░░░░░░░░░░░░░║
║EEEEEEEEEE║░░░░░░░░░░░░░░░░░░░║
║EEEEEEEEEE║░░░░░░░░░░░░░░░░░░░║
╚══════════╝═══════════════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_spacearound()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_Cross = Alignment_Cross.SpaceAround,
                           ElementsWrap = LayoutWrap.Wrap,
                           Width = 32,
                           Height = 16
                       })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 12, Height = 4 }),
                       new TestNode(out var rootB, "B", new LayoutPlan { Width = 12, Height = 4 }),
                       new TestNode(out var rootC, "C", new LayoutPlan { Width = 12, Height = 4 }),
                       new TestNode(out var rootD, "D", new LayoutPlan { Width = 12, Height = 4 }),
                       new TestNode(out var rootE, "E", new LayoutPlan { Width = 12, Height = 4 })
                   };

        ElementArrange.Calculate(root);

        // Layout is Horz (rows)
        // Each row is spaced out with even amounts before and after each row

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
╔══════════╗╔══════════╗░░░░░░░║
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░║
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░║
╚══════════╝╚══════════╝░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════╗╔══════════╗░░░░░░░║
║CCCCCCCCCC║║DDDDDDDDDD║░░░░░░░║
║CCCCCCCCCC║║DDDDDDDDDD║░░░░░░░║
╚══════════╝╚══════════╝░░░░░░░║
║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
╔══════════╗░░░░░░░░░░░░░░░░░░░║
║EEEEEEEEEE║░░░░░░░░░░░░░░░░░░░║
║EEEEEEEEEE║░░░░░░░░░░░░░░░░░░░║
╚══════════╝░░░░░░░░░░░░░░░░░░░║
╚══════════════════════════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch_row()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_Cross = Alignment_Cross.Stretch,
                           ElementsWrap = LayoutWrap.Wrap,
                           Width = 36,
                           Height = 12
                       })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 12 }),
                       new TestNode(out var rootB, "B", new LayoutPlan { Width = 12 }),
                       new TestNode(out var rootC, "C", new LayoutPlan { Width = 12 }),
                       new TestNode(out var rootD, "D", new LayoutPlan { Width = 12 }),
                       new TestNode(out var rootE, "E", new LayoutPlan { Width = 12 })
                   };
        ElementArrange.Calculate(root);

        // Layout is Horz (rows)
        // Each row has alignment cross set to stretch so all take up all height evenly

        Draw(root);
        Assert.Equal(
            @"╔══════════╗╔══════════╗╔══════════╗
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
╚══════════╝╚══════════╝╚══════════╝
╔══════════╗╔══════════╗░░░░░░░░░░░║
║DDDDDDDDDD║║EEEEEEEEEE║░░░░░░░░░░░║
║DDDDDDDDDD║║EEEEEEEEEE║░░░░░░░░░░░║
║DDDDDDDDDD║║EEEEEEEEEE║░░░░░░░░░░░║
║DDDDDDDDDD║║EEEEEEEEEE║░░░░░░░░░░░║
╚══════════╝╚══════════╝═══════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch_row_with_elements()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_Cross = Alignment_Cross.Stretch,
                           ElementsWrap = LayoutWrap.Wrap,
                           Width = 36,
                           Height = 12
                       })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 12 })
                       {
                           new TestNode(out var rootAa, "b", new LayoutPlan { FlexGrow = 1 })
                       },
                       new TestNode(out var rootB, "C", new LayoutPlan { Width = 12 }),
                       new TestNode(out var rootC, "D", new LayoutPlan { Width = 12 }),
                       new TestNode(out var rootD, "E", new LayoutPlan { Width = 12 }),
                       new TestNode(out var rootE, "F", new LayoutPlan { Width = 12 }),
                   };

        ElementArrange.Calculate(root);

        // Layout is Horz (rows)
        // Each row has alignment cross set to stretch so all take up all height evenly
        // Element 'b' will grow to take up all available space

        Draw(root);
        Assert.Equal(
            @"╔══════════╗╔══════════╗╔══════════╗
║bbbbbbbbbb║║CCCCCCCCCC║║DDDDDDDDDD║
║bbbbbbbbbb║║CCCCCCCCCC║║DDDDDDDDDD║
║bbbbbbbbbb║║CCCCCCCCCC║║DDDDDDDDDD║
║bbbbbbbbbb║║CCCCCCCCCC║║DDDDDDDDDD║
╚══════════╝╚══════════╝╚══════════╝
╔══════════╗╔══════════╗░░░░░░░░░░░║
║EEEEEEEEEE║║FFFFFFFFFF║░░░░░░░░░░░║
║EEEEEEEEEE║║FFFFFFFFFF║░░░░░░░░░░░║
║EEEEEEEEEE║║FFFFFFFFFF║░░░░░░░░░░░║
║EEEEEEEEEE║║FFFFFFFFFF║░░░░░░░░░░░║
╚══════════╝╚══════════╝═══════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch_row_with_flex()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           ElementsDirection = LayoutDirection.Horz,
                           AlignElements_Cross = Alignment_Cross.Stretch,
                           ElementsWrap = LayoutWrap.Wrap,
                           Width = 64,
                           Height = 12
                       })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Width = 12 }),
                       new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, Width = 12 }),
                       new TestNode(out var rootC, "C", new LayoutPlan { Width = 12 }),
                       new TestNode(out var rootD, "D", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 0.Percent(), Width = 12 }),
                       new TestNode(out var rootE, "E", new LayoutPlan { Width = 12 })
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
@"╔══════════╗╔══════════════════╗╔══════════╗╔══════╗╔══════════╗
║AAAAAAAAAA║║BBBBBBBBBBBBBBBBBB║║CCCCCCCCCC║║DDDDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBBBBBBBBBBBBBBBB║║CCCCCCCCCC║║DDDDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBBBBBBBBBBBBBBBB║║CCCCCCCCCC║║DDDDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBBBBBBBBBBBBBBBB║║CCCCCCCCCC║║DDDDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBBBBBBBBBBBBBBBB║║CCCCCCCCCC║║DDDDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBBBBBBBBBBBBBBBB║║CCCCCCCCCC║║DDDDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBBBBBBBBBBBBBBBB║║CCCCCCCCCC║║DDDDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBBBBBBBBBBBBBBBB║║CCCCCCCCCC║║DDDDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBBBBBBBBBBBBBBBB║║CCCCCCCCCC║║DDDDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBBBBBBBBBBBBBBBB║║CCCCCCCCCC║║DDDDDD║║EEEEEEEEEE║
╚══════════╝╚══════════════════╝╚══════════╝╚══════╝╚══════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch_row_with_flex_no_shrink()
    {
        var root =
            new TestNode(
                new LayoutPlan
                {
                    ElementsDirection = LayoutDirection.Horz,
                    AlignElements_Cross = Alignment_Cross.Stretch,
                    ElementsWrap = LayoutWrap.Wrap,
                    Width = 48,
                    Height = 12
                })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootB, "B", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 0.Percent(), Width = 12 }),
                new TestNode(out var rootC, "C", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootD, "D", new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 0.Percent(), Width = 12 }),
                new TestNode(out var rootE, "E", new LayoutPlan { Width = 12 })
            };

        ElementArrange.Calculate(root);
 
        Draw(root);
        Assert.Equal(
@"╔══════════╗╔════╗╔══════════╗╔════╗╔══════════╗
║AAAAAAAAAA║║BBBB║║CCCCCCCCCC║║DDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBB║║CCCCCCCCCC║║DDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBB║║CCCCCCCCCC║║DDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBB║║CCCCCCCCCC║║DDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBB║║CCCCCCCCCC║║DDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBB║║CCCCCCCCCC║║DDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBB║║CCCCCCCCCC║║DDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBB║║CCCCCCCCCC║║DDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBB║║CCCCCCCCCC║║DDDD║║EEEEEEEEEE║
║AAAAAAAAAA║║BBBB║║CCCCCCCCCC║║DDDD║║EEEEEEEEEE║
╚══════════╝╚════╝╚══════════╝╚════╝╚══════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch_row_with_margin()
    {
        var root =
            new TestNode(
                new LayoutPlan
                {
                    ElementsDirection = LayoutDirection.Horz,
                    AlignElements_Cross = Alignment_Cross.Stretch,
                    ElementsWrap = LayoutWrap.Wrap,
                    Width = 36,
                    Height = 12
                })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootB, "B", new LayoutPlan { Width = 8, Margin = 2 }),
                new TestNode(out var rootC, "C", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootD, "D", new LayoutPlan { Width = 8, Margin = 2 }),
                new TestNode(out var rootE, "E", new LayoutPlan { Width = 12 })
            };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
@"╔══════════╗════════════╔══════════╗
║AAAAAAAAAA║░░░░░░░░░░░░║CCCCCCCCCC║
║AAAAAAAAAA║░░╔══════╗░░║CCCCCCCCCC║
║AAAAAAAAAA║░░╚══════╝░░║CCCCCCCCCC║
║AAAAAAAAAA║░░░░░░░░░░░░║CCCCCCCCCC║
╚══════════╝░░░░░░░░░░░░╚══════════╝
║░░░░░░░░░░░╔══════════╗░░░░░░░░░░░║
║░░░░░░░░░░░║EEEEEEEEEE║░░░░░░░░░░░║
║░╔══════╗░░║EEEEEEEEEE║░░░░░░░░░░░║
║░╚══════╝░░║EEEEEEEEEE║░░░░░░░░░░░║
║░░░░░░░░░░░║EEEEEEEEEE║░░░░░░░░░░░║
╚═══════════╚══════════╝═══════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch_row_with_padding()
    {
        var root =
            new TestNode(
                new LayoutPlan
                {
                    ElementsDirection = LayoutDirection.Horz,
                    AlignElements_Cross = Alignment_Cross.Stretch,
                    ElementsWrap = LayoutWrap.Wrap,
                    Width = 36,
                    Height = 12
                })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootB, "B", new LayoutPlan { Width = 12, Padding = 2 }),
                new TestNode(out var rootC, "C", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootD, "D", new LayoutPlan { Width = 12, Padding = 2 }),
                new TestNode(out var rootE, "E", new LayoutPlan { Width = 12 })
            };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════╗┌──────────┐╔══════════╗
║AAAAAAAAAA║│BBBBBBBBBB│║CCCCCCCCCC║
║AAAAAAAAAA║│B╔══════╗B│║CCCCCCCCCC║
║AAAAAAAAAA║│B╚══════╝B│║CCCCCCCCCC║
║AAAAAAAAAA║│BBBBBBBBBB│║CCCCCCCCCC║
╚══════════╝└──────────┘╚══════════╝
┌──────────┐╔══════════╗░░░░░░░░░░░║
│DDDDDDDDDD│║EEEEEEEEEE║░░░░░░░░░░░║
│D╔══════╗D│║EEEEEEEEEE║░░░░░░░░░░░║
│D╚══════╝D│║EEEEEEEEEE║░░░░░░░░░░░║
│DDDDDDDDDD│║EEEEEEEEEE║░░░░░░░░░░░║
└──────────┘╚══════════╝═══════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch_row_with_single_row()
    {
        var root =
            new TestNode(
                new LayoutPlan
                {
                    ElementsDirection = LayoutDirection.Horz,
                    AlignElements_Cross = Alignment_Cross.Stretch,
                    ElementsWrap = LayoutWrap.Wrap,
                    Width = 36,
                    Height = 12
                })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootB, "B", new LayoutPlan { Width = 12 })
            };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════╗╔══════════╗═══════════╗
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░░░░░║
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░░░░░║
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░░░░░║
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░░░░░║
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░░░░░║
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░░░░░║
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░░░░░║
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░░░░░║
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░░░░░║
║AAAAAAAAAA║║BBBBBBBBBB║░░░░░░░░░░░║
╚══════════╝╚══════════╝═══════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch_row_with_fixed_height()
    {
        var root =
            new TestNode(
                new LayoutPlan
                {
                    ElementsDirection = LayoutDirection.Horz,
                    AlignElements_Cross = Alignment_Cross.Stretch,
                    ElementsWrap = LayoutWrap.Wrap,
                    Width = 48,
                    Height = 12
                })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { Width = 14 }),
                new TestNode(out var rootB, "B", new LayoutPlan { Width = 14, Height = 8 }),
                new TestNode(out var rootC, "C", new LayoutPlan { Width = 14 }),
                new TestNode(out var rootD, "D", new LayoutPlan { Width = 14 }),
                new TestNode(out var rootE, "E", new LayoutPlan { Width = 14, Height = 4 })
            };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
@"╔════════════╗╔════════════╗╔════════════╗═════╗
║AAAAAAAAAAAA║║BBBBBBBBBBBB║║CCCCCCCCCCCC║░░░░░║
║AAAAAAAAAAAA║║BBBBBBBBBBBB║║CCCCCCCCCCCC║░░░░░║
║AAAAAAAAAAAA║║BBBBBBBBBBBB║║CCCCCCCCCCCC║░░░░░║
║AAAAAAAAAAAA║║BBBBBBBBBBBB║║CCCCCCCCCCCC║░░░░░║
║AAAAAAAAAAAA║║BBBBBBBBBBBB║║CCCCCCCCCCCC║░░░░░║
║AAAAAAAAAAAA║║BBBBBBBBBBBB║║CCCCCCCCCCCC║░░░░░║
╚════════════╝╚════════════╝╚════════════╝░░░░░║
╔════════════╗╔════════════╗░░░░░░░░░░░░░░░░░░░║
║DDDDDDDDDDDD║║EEEEEEEEEEEE║░░░░░░░░░░░░░░░░░░░║
║DDDDDDDDDDDD║║EEEEEEEEEEEE║░░░░░░░░░░░░░░░░░░░║
╚════════════╝╚════════════╝═══════════════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch_row_with_max_height()
    {
        var root =
            new TestNode(
                new LayoutPlan
                {
                    ElementsDirection = LayoutDirection.Horz,
                    AlignElements_Cross = Alignment_Cross.Stretch,
                    ElementsWrap = LayoutWrap.Wrap,
                    Width = 36,
                    Height = 12
                })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootB, "B", new LayoutPlan { Width = 12, MaxHeight = 4 }),
                new TestNode(out var rootC, "C", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootD, "D", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootE, "E", new LayoutPlan { Width = 12 })
            };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════╗╔══════════╗╔══════════╗
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
║AAAAAAAAAA║╚══════════╝║CCCCCCCCCC║
║AAAAAAAAAA║░░░░░░░░░░░░║CCCCCCCCCC║
╚══════════╝░░░░░░░░░░░░╚══════════╝
╔══════════╗╔══════════╗░░░░░░░░░░░║
║DDDDDDDDDD║║EEEEEEEEEE║░░░░░░░░░░░║
║DDDDDDDDDD║║EEEEEEEEEE║░░░░░░░░░░░║
║DDDDDDDDDD║║EEEEEEEEEE║░░░░░░░░░░░║
║DDDDDDDDDD║║EEEEEEEEEE║░░░░░░░░░░░║
╚══════════╝╚══════════╝═══════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch_row_with_min_height()
    {
        var root =
            new TestNode(
                new LayoutPlan
                {
                    ElementsDirection = LayoutDirection.Horz,
                    AlignElements_Cross = Alignment_Cross.Stretch,
                    ElementsWrap = LayoutWrap.Wrap,
                    Width = 36,
                    Height = 12
                })
            {
                new TestNode(out var rootA, "A", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootB, "B", new LayoutPlan { Width = 12, MinHeight = 8 }),
                new TestNode(out var rootC, "C", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootD, "D", new LayoutPlan { Width = 12 }),
                new TestNode(out var rootE, "E", new LayoutPlan { Width = 12 })
            };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════╗╔══════════╗╔══════════╗
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
║AAAAAAAAAA║║BBBBBBBBBB║║CCCCCCCCCC║
╚══════════╝╚══════════╝╚══════════╝
╔══════════╗╔══════════╗░░░░░░░░░░░║
╚══════════╝╚══════════╝═══════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch_column()
    {
        var root = new TestNode(
                       new LayoutPlan
                       {
                           AlignElements_Cross = Alignment_Cross.Stretch,
                           ElementsWrap = LayoutWrap.Wrap,
                           Width = 32,
                           Height = 24
                       })
                   {
                       new TestNode(out var rootA, "A", new LayoutPlan { Height = 8 })
                       {
                           new TestNode(out var rootAa, "b", new LayoutPlan { FlexGrow = 1, FlexShrink = 1, ChildLayoutDirectionLength = 0.Percent() })
                       },
                       new TestNode(out var rootB, "C", new LayoutPlan { FlexGrow = 1, FlexShrink = 1, ChildLayoutDirectionLength = 0.Percent(), Height = 8 }),
                       new TestNode(out var rootC, "D", new LayoutPlan { Height = 8 }),
                       new TestNode(out var rootD, "E", new LayoutPlan { Height = 8 }),
                       new TestNode(out var rootE, "F", new LayoutPlan { Height = 8 })
                   };

        ElementArrange.Calculate(root);

        Draw(root);
        Assert.Equal(
            @"╔══════════════╗╔══════════════╗
║bbbbbbbbbbbbbb║║FFFFFFFFFFFFFF║
║bbbbbbbbbbbbbb║║FFFFFFFFFFFFFF║
║bbbbbbbbbbbbbb║║FFFFFFFFFFFFFF║
║bbbbbbbbbbbbbb║║FFFFFFFFFFFFFF║
║bbbbbbbbbbbbbb║║FFFFFFFFFFFFFF║
║bbbbbbbbbbbbbb║║FFFFFFFFFFFFFF║
╚══════════════╝╚══════════════╝
╔══════════════╗░░░░░░░░░░░░░░░║
║DDDDDDDDDDDDDD║░░░░░░░░░░░░░░░║
║DDDDDDDDDDDDDD║░░░░░░░░░░░░░░░║
║DDDDDDDDDDDDDD║░░░░░░░░░░░░░░░║
║DDDDDDDDDDDDDD║░░░░░░░░░░░░░░░║
║DDDDDDDDDDDDDD║░░░░░░░░░░░░░░░║
║DDDDDDDDDDDDDD║░░░░░░░░░░░░░░░║
╚══════════════╝░░░░░░░░░░░░░░░║
╔══════════════╗░░░░░░░░░░░░░░░║
║EEEEEEEEEEEEEE║░░░░░░░░░░░░░░░║
║EEEEEEEEEEEEEE║░░░░░░░░░░░░░░░║
║EEEEEEEEEEEEEE║░░░░░░░░░░░░░░░║
║EEEEEEEEEEEEEE║░░░░░░░░░░░░░░░║
║EEEEEEEEEEEEEE║░░░░░░░░░░░░░░░║
║EEEEEEEEEEEEEE║░░░░░░░░░░░░░░░║
╚══════════════╝═══════════════╝",
            Canvas!.ToString());
    }

    [Fact]
    public void align_content_stretch_is_not_overriding_align_items()
    {
        new TestNode(out var root, new LayoutPlan { AlignElements_Cross = Alignment_Cross.Stretch })
        {
            new TestNode(out var rootA, "A", new LayoutPlan {
                                                            ElementsDirection  = LayoutDirection.Horz,
                                                            AlignElements_Cross  = Alignment_Cross.Stretch,
                                                            Align_Cross = AlignmentLine_Cross.Center,
                                                            Width              = 32,
                                                            Height             = 16
                                                        })
            {
                new TestNode(out var rootAa, "B", new LayoutPlan { AlignElements_Cross = Alignment_Cross.Stretch, Width = 4, Height = 4 })
            }
        };

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 32, 16), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 0, 32, 16), rootA.Layout.Bounds);
        Assert.Equal(new RectF(0, 6, 4, 4), rootAa.Layout.Bounds);

        Draw(root);
        Assert.Equal(
            @"╔══════════════════════════════╗
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
╔══╗AAAAAAAAAAAAAAAAAAAAAAAAAAA║
║BB║AAAAAAAAAAAAAAAAAAAAAAAAAAA║
║BB║AAAAAAAAAAAAAAAAAAAAAAAAAAA║
╚══╝AAAAAAAAAAAAAAAAAAAAAAAAAAA║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
║AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA║
╚══════════════════════════════╝",
            Canvas!.ToString());
    }
}