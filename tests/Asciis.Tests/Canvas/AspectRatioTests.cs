using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Canvas;

[TestClass]
public class AspectRatioTests : BaseCanvasTests
{
    public AspectRatioTests(ITestOutputHelper context) : base(context)
    {
    }

    [Fact]
    public void ControlLayout_AspectRatio_Width()
    {
        var root = new Frame
        {
            new Frame
            {
                new FrameLayoutPlan
                {
                    Width = 10,
                    Height = Number.Auto,
                    AspectRatio = 2f
                }
            }
            
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌────────┐                    │
            ││        │                    │
            ││        │                    │
            ││        │                    │
            │└────────┘                    │
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
    
    [Fact]
    public void ControlLayout_AspectRatio_Height()
    {
        var root = new Frame
        {
            new Frame
            {
                new FrameLayoutPlan
                {
                    Height = 5,
                    Width = Number.Auto,
                    AspectRatio = 2f
                }
            }
            
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌────────┐                    │
            ││        │                    │
            ││        │                    │
            ││        │                    │
            │└────────┘                    │
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
    
    [Fact]
    public void ControlLayout_AspectRatio_Multiple_Shrink()
    {
        var root = new Frame
        {
            new Frame( new FrameLayoutPlan { Height = 100.Percent(), Width = Number.Auto, FlexShrink = 1, AspectRatio = 2 } ),
            new Frame( new FrameLayoutPlan { Height = 100.Percent(), Width = Number.Auto, FlexShrink = 2, AspectRatio = 2 } )
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌─────────────────┐           │
            ││                 │           │
            ││                 │           │
            ││                 │           │
            ││                 │           │
            ││                 │           │
            ││                 │           │
            ││                 │           │
            │└─────────────────┘           │
            │┌───────┐                     │
            ││       │                     │
            ││       │                     │
            ││       │                     │
            │└───────┘                     │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void ControlLayout_AspectRatio_AbsolutePosition()
    {
        var root = new Frame
        {
            new Frame
            {
                new FrameLayoutPlan
                {
                    PositionType = PositionType.Absolute,
                    Left = 10,
                    Top = 2,
                    Width = 10,
                    Height = Number.Auto,
                    AspectRatio = 2f
                }
            }
            
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                              │
            │         ┌────────┐           │
            │         │        │           │
            │         │        │           │
            │         │        │           │
            │         └────────┘           │
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
    
    [Fact]
    public void ControlLayout_AspectRatio_MaxWidth()
    {
        var root = new Frame
        {
            new Frame
            {
                new FrameLayoutPlan
                {
                    Height = 10,
                    Width = Number.Auto,
                    MaxWidth = 10,
                    AspectRatio = 2f
                }
            }
            
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌────────┐                    │
            ││        │                    │
            ││        │                    │
            ││        │                    │
            ││        │                    │
            ││        │                    │
            ││        │                    │
            ││        │                    │
            ││        │                    │
            │└────────┘                    │
            │                              │
            │                              │
            │                              │
            │                              │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void ControlLayout_AspectRatio_MinWidth()
    {
        var root = new Frame
        {
            new Frame
            {
                new FrameLayoutPlan
                {
                    Height = 4,
                    Width = Number.Auto,
                    MinWidth = 10,
                    AspectRatio = 2f
                }
            }
            
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌────────┐                    │
            ││        │                    │
            ││        │                    │
            │└────────┘                    │
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
    
    [Fact]
    public void ControlLayout_AspectRatio_FlexGrow()
    {
        var root = new Frame
        {
            new Frame
            {
                new FrameLayoutPlan
                {
                    Height = 5,
                    Width = 10,
                    FlexGrow = 1,
                    AspectRatio = 1
                }
            }
            
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌────────────┐                │
            ││            │                │
            ││            │                │
            ││            │                │
            ││            │                │
            ││            │                │
            ││            │                │
            ││            │                │
            ││            │                │
            ││            │                │
            ││            │                │
            ││            │                │
            ││            │                │
            │└────────────┘                │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void ControlLayout_AspectRatio_FlexGrow_Multiple()
    {
        var root = new Frame
        {
            new Frame { new FrameLayoutPlan { Height = 6, FlexGrow = 1, AspectRatio = 2 } },
            new Frame { new FrameLayoutPlan { Height = 8, FlexGrow = 1, AspectRatio = 2 } }
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌──────────┐                  │
            ││          │                  │
            ││          │                  │
            ││          │                  │
            ││          │                  │
            │└──────────┘                  │
            │┌──────────────┐              │
            ││              │              │
            ││              │              │
            ││              │              │
            ││              │              │
            ││              │              │
            ││              │              │
            │└──────────────┘              │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void ControlLayout_AspectRatio_AbsolutePosition_LeftTopRight()
    {
        var root = new Frame
        {
            new Frame
            {
                new FrameLayoutPlan
                {
                    PositionType = PositionType.Absolute,
                    Left = 10,
                    Top = 2,
                    Right = 10,
                    Height = Number.Auto,
                    Width = Number.Auto,
                    AspectRatio = 2f
                }
            }
            
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                              │
            │         ┌──────────┐         │
            │         │          │         │
            │         │          │         │
            │         │          │         │
            │         │          │         │
            │         └──────────┘         │
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
    public void ControlLayout_AspectRatio_AbsolutePosition_LeftRightBottom()
    {
        var root = new Frame
        {
            new Frame
            {
                new FrameLayoutPlan
                {
                    PositionType = PositionType.Absolute,
                    Left = 4,
                    Right = 4,
                    Bottom = 2,
                    Height = Number.Auto,
                    Width = Number.Auto,
                    AspectRatio = 2f
                }
            }
            
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                              │
            │   ┌──────────────────────┐   │
            │   │                      │   │
            │   │                      │   │
            │   │                      │   │
            │   │                      │   │
            │   │                      │   │
            │   │                      │   │
            │   │                      │   │
            │   │                      │   │
            │   │                      │   │
            │   │                      │   │
            │   └──────────────────────┘   │
            │                              │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
}