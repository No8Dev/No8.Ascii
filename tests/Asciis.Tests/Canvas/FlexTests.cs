using No8.Ascii.Controls;
using Xunit;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Canvas;

public class FlexTests : BaseCanvasTests
{
    public FlexTests(ITestOutputHelper context) : base(context)
    {
    }
    
    [Fact]
    public void ControlLayout_Flex()
    {
        var root = new Frame
        {
            new Frame { new FrameLayoutPlan { FlexShrink = 1, MaxHeight = 3 } },
            new Frame { new FrameLayoutPlan { FlexShrink = 1, FlexGrow = 1, Height = Number.Auto } },
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌────────────────────────────┐│
            ││                            ││
            │└────────────────────────────┘│
            │┌────────────────────────────┐│
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            │└────────────────────────────┘│
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void ControlLayout_Flex_LookLikeABed()
    {
        var root = new Frame
        {
            new Row
            {
                new RowLayoutPlan { FlexShrink = 1, MaxHeight = 4 },
                new Frame { new FrameLayoutPlan { FlexShrink = 1, MinHeight = 3 }}, 
                new Frame { new FrameLayoutPlan { FlexShrink = 1 }},
            },
            new Frame { new FrameLayoutPlan { FlexShrink = 1, FlexGrow = 1, Height = Number.Auto } },
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌─────────────┐┌─────────────┐│
            ││             ││             ││
            │└─────────────┘└─────────────┘│
            │┌────────────────────────────┐│
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            │└────────────────────────────┘│
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void ControlLayout_Flex_Header()
    {
        var root = new Frame
        {
            new Row
            {
                new RowLayoutPlan { FlexShrink = 1, MaxHeight = 4 },
                new Frame { new FrameLayoutPlan { Height = 3, MaxWidth = 10 }}, 
                new Frame { new FrameLayoutPlan { FlexGrow = 1, Width = Number.Auto }},
            },
            new Frame { new FrameLayoutPlan { FlexShrink = 1, FlexGrow = 1, Height = Number.Auto } },
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌────────┐┌──────────────────┐│
            ││        ││                  ││
            │└────────┘└──────────────────┘│
            │┌────────────────────────────┐│
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            ││                            ││
            │└────────────────────────────┘│
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
}