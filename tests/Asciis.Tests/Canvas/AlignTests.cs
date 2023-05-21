using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Canvas;

[TestClass]
public class AlignTests : BaseCanvasTests
{
    public AlignTests(ITestOutputHelper context) : base(context)
    {
    }
    
    [Fact]
    public void ControlLayout_Align_Wrap()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                LayoutPlan = { ElementsDirection = LayoutDirection.Horz },
                ElementsWrap = LayoutWrap.Wrap,
            },
            LargeBox(), LargeBox(), LargeBox(),
            LargeBox(), LargeBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌───────┐┌───────┐┌───────┐   │
            ││       ││       ││       │   │
            ││       ││       ││       │   │
            │└───────┘└───────┘└───────┘   │
            │┌───────┐┌───────┐            │
            ││       ││       │            │
            ││       ││       │            │
            │└───────┘└───────┘            │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }    
    
    [Fact]
    public void ControlLayout_Align_Multiple()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                LayoutPlan = { ElementsDirection = LayoutDirection.Horz },
                ElementsWrap = LayoutWrap.Wrap,
            },
            LittleBox(), MediumBox(), LargeBox(),
            LargeBox(), MediumBox(), LittleBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌─┐┌────┐┌───────┐┌───────┐   │
            │└─┘│    ││       ││       │   │
            │   └────┘│       ││       │   │
            │         └───────┘└───────┘   │
            │┌────┐┌─┐                     │
            ││    │└─┘                     │
            │└────┘                        │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void ControlLayout_Align_Flex()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                LayoutPlan = { ElementsDirection = LayoutDirection.Horz },
                ElementsWrap = LayoutWrap.Wrap,
            },
            new Frame { new FrameLayoutPlan { Width = 4, Height = 4, FlexGrow = 1 } },
            new Frame { new FrameLayoutPlan { Width = 4, Height = 4 } },
            new Frame { new FrameLayoutPlan { Width = 4, Height = 4 } },
            new Frame { new FrameLayoutPlan { Width = 4, Height = 4, FlexGrow = 1 } },
            new Frame { new FrameLayoutPlan { Width = 4, Height = 4 } },
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌───────┐┌──┐┌──┐┌───────┐┌──┐│
            ││       ││  ││  ││       ││  ││
            ││       ││  ││  ││       ││  ││
            │└───────┘└──┘└──┘└───────┘└──┘│
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
}