using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Platforms;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.Canvas;

[TestClass]
public class RowLayoutSomeTests
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

    private Frame LittleBox() => new() { new FrameLayoutPlan { Width = 3, Height = 2 } };
    private Frame MediumBox() => new() { new FrameLayoutPlan { Width = 6, Height = 3 } };

    [Fact]
    public void Layout_Some_Start_Start()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.Start,
            },
            new Row
            {
                new RowLayoutPlan { Padding = 1 },
                new RowStyle { Border = 1 },
                LittleBox(), LittleBox(), LittleBox()
            }
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌─────────┐                   │
            ││┌─┐┌─┐┌─┐│                   │
            ││└─┘└─┘└─┘│                   │
            │└─────────┘                   │
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
    public void Layout_Some_Center_Start()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.Center
            },
            new Row 
            {
                new RowLayoutPlan { Padding = 1 },
                new RowStyle { Border = 1 },
                LittleBox(), LittleBox(), LittleBox()
            }
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │          ┌─────────┐         │
            │          │┌─┐┌─┐┌─┐│         │
            │          │└─┘└─┘└─┘│         │
            │          └─────────┘         │
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
    public void Layout_Some_End_Start()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.End
            },
            new Row
            {
                new RowLayoutPlan { Padding = 1 },
                new RowStyle { Border = 1 },
                LittleBox(), LittleBox(), LittleBox()
            }
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │                   ┌─────────┐│
            │                   │┌─┐┌─┐┌─┐││
            │                   │└─┘└─┘└─┘││
            │                   └─────────┘│
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
    public void Layout_Some_Start_Center()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.Start,
                VertAlign = ControlAlign.Center
            },
            new Row
            {
                new RowLayoutPlan { Padding = 1 },
                new RowStyle { Border = 1 },
                LittleBox(), LittleBox(), LittleBox()
            }
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
            │┌─────────┐                   │
            ││┌─┐┌─┐┌─┐│                   │
            ││└─┘└─┘└─┘│                   │
            │└─────────┘                   │
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
    public void Layout_Some_Center_Center()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.Center,
                VertAlign = ControlAlign.Center
            },
            new Row
            {
                new RowLayoutPlan { Padding = 1 },
                new RowStyle { Border = 1 },
                LittleBox(), LittleBox(), LittleBox()
            }

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
            │          ┌─────────┐         │
            │          │┌─┐┌─┐┌─┐│         │
            │          │└─┘└─┘└─┘│         │
            │          └─────────┘         │
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
    public void Layout_Some_End_Center()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.End,
                VertAlign = ControlAlign.Center
            },
            new Row
            {
                new RowLayoutPlan { Padding = 1 },
                new RowStyle { Border = 1 },
                LittleBox(), LittleBox(), LittleBox()
            }

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
            │                   ┌─────────┐│
            │                   │┌─┐┌─┐┌─┐││
            │                   │└─┘└─┘└─┘││
            │                   └─────────┘│
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
    public void Layout_Some_Start_End()
    {
        var root = new Frame
        {
            new FrameLayoutPlan
            {
                HorzAlign = ControlAlign.Start,
                VertAlign = ControlAlign.End
            },
            new Row
            {
                new RowLayoutPlan { Padding = 1 },
                new RowStyle { Border = 1 },
                LittleBox(), LittleBox(), LittleBox()
            }

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
            │                              │
            │                              │
            │┌─────────┐                   │
            ││┌─┐┌─┐┌─┐│                   │
            ││└─┘└─┘└─┘│                   │
            │└─────────┘                   │
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
            new Row
            {
                new RowLayoutPlan { Padding = 1 },
                new RowStyle { Border = 1 },
                LittleBox(), LittleBox(), LittleBox()
            }

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
            │                              │
            │                              │
            │          ┌─────────┐         │
            │          │┌─┐┌─┐┌─┐│         │
            │          │└─┘└─┘└─┘│         │
            │          └─────────┘         │
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
            new Row
            {
                new RowLayoutPlan { Padding = 1 },
                new RowStyle { Border = 1 },
                LittleBox(), LittleBox(), LittleBox()
            }
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
            │                              │
            │                              │
            │                   ┌─────────┐│
            │                   │┌─┐┌─┐┌─┐││
            │                   │└─┘└─┘└─┘││
            │                   └─────────┘│
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
    
    [Fact]
    public void LayoutWrap_Many_Start_Start()
    {
        var root = new Frame
        {
            new Row
            {
                new RowLayoutPlan
                {
                    Padding = 1,
                    ElementsWrap = LayoutWrap.Wrap
                },
                new RowStyle
                {
                    Border  = 1,
                },
                MediumBox(), MediumBox(), LittleBox(),
                MediumBox(), MediumBox(), LittleBox(),
                MediumBox(), MediumBox(), LittleBox()
            }
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌────────────────────────────┐│
            ││┌────┐┌────┐┌─┐┌────┐┌────┐ ││
            │││    ││    │└─┘│    ││    │ ││
            ││└────┘└────┘   └────┘└────┘ ││
            ││┌─┐┌────┐┌────┐┌─┐          ││
            ││└─┘│    ││    │└─┘          ││
            ││   └────┘└────┘             ││
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
    }    [Fact]
    public void LayoutWrapPad0_Many_Start_Start()
    {
        var root = new Frame
        {
            new Row
            {
                new RowLayoutPlan { ElementsWrap = LayoutWrap.Wrap },
                new RowStyle { Border  = 1, },
                MediumBox(), MediumBox(), LittleBox(),
                MediumBox(), MediumBox(), LittleBox(),
                MediumBox(), MediumBox(), LittleBox()
            }
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌────┬┬────┬┬─┬┬────┬┬────┬┬─┐│
            ││    ││    │└─┘│    ││    │└─┤│
            │├────┘└────┘   └────┘└────┘  ││
            │├────┐┌────┐┌─┐              ││
            ││    ││    │└─┘              ││
            │└────┴┴────┴─────────────────┘│
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
