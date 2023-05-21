using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Platforms;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Canvas;

[TestClass]
public class ComplexLayoutManyTests : BaseCanvasTests
{
    public ComplexLayoutManyTests(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void Layout0Wrap_Many_Start_Start()
    {
        var root = new Frame
        {
            LittleBox(), LittleBox(), LittleBox(),
            LittleBox(), LittleBox(), LittleBox(),
            LittleBox(), LittleBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌─┐                           │
            │└─┘                           │
            │┌─┐                           │
            │└─┘                           │
            │┌─┐                           │
            │└─┘                           │
            │┌─┐                           │
            │└─┘                           │
            │┌─┐                           │
            │└─┘                           │
            │┌─┐                           │
            │└─┘                           │
            │┌─┐                           │
            │└─┘                           │
            └┬─┬───────────────────────────┘
            """,
            _canvas.ToString());
    }

    [Fact]
    public void LayoutWrap_Many_Start_Start()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                ElementsWrap = LayoutWrap.Wrap
            },
            MediumBox(), MediumBox(), LittleBox(),
            MediumBox(), MediumBox(), LittleBox(),
            MediumBox(), MediumBox(), LittleBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌────┐┌─┐                     │
            ││    │└─┘                     │
            │└────┘┌────┐                  │
            │┌────┐│    │                  │
            ││    │└────┘                  │
            │└────┘┌────┐                  │
            │┌─┐   │    │                  │
            │└─┘   └────┘                  │
            │┌────┐┌─┐                     │
            ││    │└─┘                     │
            │└────┘                        │
            │┌────┐                        │
            ││    │                        │
            │└────┘                        │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }

    [Fact]
    public void LayoutWrap_Many_Start_Center()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                ElementsWrap = LayoutWrap.Wrap,
                HorzAlign = ControlAlign.Center
            },
            MediumBox(), MediumBox(), LittleBox(),
            MediumBox(), MediumBox(), LittleBox(),
            MediumBox(), MediumBox(), LittleBox()

        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │         ┌────┐  ┌─┐          │
            │         │    │  └─┘          │
            │         └────┘┌────┐         │
            │         ┌────┐│    │         │
            │         │    │└────┘         │
            │         └────┘┌────┐         │
            │           ┌─┐ │    │         │
            │           └─┘ └────┘         │
            │         ┌────┐  ┌─┐          │
            │         │    │  └─┘          │
            │         └────┘               │
            │         ┌────┐               │
            │         │    │               │
            │         └────┘               │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void LayoutWrap_Many_Start_End()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                ElementsWrap = LayoutWrap.Wrap,
                HorzAlign = ControlAlign.End
            },
            MediumBox(), MediumBox(), LittleBox(),
            MediumBox(), MediumBox(), LittleBox(),
            MediumBox(), MediumBox(), LittleBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                  ┌────┐   ┌─┐│
            │                  │    │   └─┘│
            │                  └────┘┌────┐│
            │                  ┌────┐│    ││
            │                  │    │└────┘│
            │                  └────┘┌────┐│
            │                     ┌─┐│    ││
            │                     └─┘└────┘│
            │                  ┌────┐   ┌─┐│
            │                  │    │   └─┘│
            │                  └────┘      │
            │                  ┌────┐      │
            │                  │    │      │
            │                  └────┘      │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void LayoutWrap_Many_Start_Stretch()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                ElementsWrap = LayoutWrap.Wrap,
                HorzAlign = ControlAlign.Stretch
            },
            MediumBox(), MediumBox(), LittleBox(),
            MediumBox(), MediumBox(), LittleBox(),
            MediumBox(), MediumBox(), LittleBox(),
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌────┐         ┌─┐            │
            ││    │         └─┘            │
            │└────┘         ┌────┐         │
            │┌────┐         │    │         │
            ││    │         └────┘         │
            │└────┘         ┌────┐         │
            │┌─┐            │    │         │
            │└─┘            └────┘         │
            │┌────┐         ┌─┐            │
            ││    │         └─┘            │
            │└────┘                        │
            │┌────┐                        │
            ││    │                        │
            │└────┘                        │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
}