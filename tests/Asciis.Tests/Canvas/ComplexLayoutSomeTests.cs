using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Platforms;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.Canvas;

[TestClass]
public class ComplexLayoutSomeTests
{
    private No8.Ascii.Canvas? _canvas;

    private void Draw( Control root, int width = 32, int height = 16 )
    {
        _canvas = new No8.Ascii.Canvas(width, height);
        ElementArrange.Calculate(root, width, height);

        root.OnDraw(_canvas, null);

        System.Console.WriteLine(root);
        System.Console.WriteLine(root.Layout);
    }

    private Frame LittleBox()
    {
        return new Frame
        {
            new FrameLayoutPlan
            {
                Width = 3, Height = 2
            }
        };
    }

    [Fact]
    public void Layout_Some_Start_Start()
    {
        var root = new Frame
        {
            LittleBox(), LittleBox(), LittleBox()
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
    public void Layout_Some_Center_Start()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.Center
            },
            LittleBox(), LittleBox(), LittleBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │              ┌─┐             │
            │              └─┘             │
            │              ┌─┐             │
            │              └─┘             │
            │              ┌─┐             │
            │              └─┘             │
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
    public void Layout_Some_End_Start()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.End
            },
            LittleBox(), LittleBox(), LittleBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                           ┌─┐│
            │                           └─┘│
            │                           ┌─┐│
            │                           └─┘│
            │                           ┌─┐│
            │                           └─┘│
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
    public void Layout_Some_Start_Center()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                VertAlign = ControlAlign.Center
            },
            LittleBox(), LittleBox(), LittleBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                              │
            │                              │
            │                              │
            │                              │
            │┌─┐                           │
            │└─┘                           │
            │┌─┐                           │
            │└─┘                           │
            │┌─┐                           │
            │└─┘                           │
            │                              │
            │                              │
            │                              │
            │                              │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void Layout_Some_Center_Center()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.Center,
                VertAlign = ControlAlign.Center
            },
            LittleBox(), LittleBox(), LittleBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                              │
            │                              │
            │                              │
            │                              │
            │              ┌─┐             │
            │              └─┘             │
            │              ┌─┐             │
            │              └─┘             │
            │              ┌─┐             │
            │              └─┘             │
            │                              │
            │                              │
            │                              │
            │                              │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void Layout_Some_End_Center()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.End,
                VertAlign = ControlAlign.Center
            },
            LittleBox(), LittleBox(), LittleBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                              │
            │                              │
            │                              │
            │                              │
            │                           ┌─┐│
            │                           └─┘│
            │                           ┌─┐│
            │                           └─┘│
            │                           ┌─┐│
            │                           └─┘│
            │                              │
            │                              │
            │                              │
            │                              │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void Layout_Some_Start_End()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                VertAlign = ControlAlign.End
            },
            LittleBox(), LittleBox(), LittleBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │┌─┐                           │
            │└─┘                           │
            │┌─┐                           │
            │└─┘                           │
            │┌─┐                           │
            │└─┘                           │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void Layout_Some_Center_End()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.Center,
                VertAlign = ControlAlign.End
            },
            LittleBox(), LittleBox(), LittleBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │              ┌─┐             │
            │              └─┘             │
            │              ┌─┐             │
            │              └─┘             │
            │              ┌─┐             │
            │              └─┘             │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void Layout_Some_End_End()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.End,
                VertAlign = ControlAlign.End
            },
            LittleBox(), LittleBox(), LittleBox()
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                           ┌─┐│
            │                           └─┘│
            │                           ┌─┐│
            │                           └─┘│
            │                           ┌─┐│
            │                           └─┘│
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }

    [Fact]
    public void Layout_Some_Stretch_Start()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.Stretch
            },
            new Frame { new FrameLayoutPlan { Height = 2 } },
            new Frame { new FrameLayoutPlan { Height = 2 } },
            new Frame { new FrameLayoutPlan { Height = 4 } }
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌────────────────────────────┐│
            │└────────────────────────────┘│
            │┌────────────────────────────┐│
            │└────────────────────────────┘│
            │┌────────────────────────────┐│
            ││                            ││
            ││                            ││
            │└────────────────────────────┘│
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
    public void Layout_Some_Stretch_Center()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.Stretch,
                VertAlign = ControlAlign.Center
            },
            new Frame { new FrameLayoutPlan { Height = 2 } },
            new Frame { new FrameLayoutPlan { Height = 2 } },
            new Frame { new FrameLayoutPlan { Height = 4 } }
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                              │
            │                              │
            │                              │
            │┌────────────────────────────┐│
            │└────────────────────────────┘│
            │┌────────────────────────────┐│
            │└────────────────────────────┘│
            │┌────────────────────────────┐│
            ││                            ││
            ││                            ││
            │└────────────────────────────┘│
            │                              │
            │                              │
            │                              │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }

    [Fact]
    public void Layout_Some_Stretch_End()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.Stretch,
                VertAlign = ControlAlign.End
            },
            new Frame { new FrameLayoutPlan { Height = 2 } },
            new Frame { new FrameLayoutPlan { Height = 2 } },
            new Frame { new FrameLayoutPlan { Height = 4 } }
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │                              │
            │┌────────────────────────────┐│
            │└────────────────────────────┘│
            │┌────────────────────────────┐│
            │└────────────────────────────┘│
            │┌────────────────────────────┐│
            ││                            ││
            ││                            ││
            │└────────────────────────────┘│
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
}