using No8.Ascii.Controls;
using Xunit;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Canvas;

public class MarginTests : BaseCanvasTests
{
    public MarginTests(ITestOutputHelper context) : base(context)
    {
    }
    
    [Fact]
    public void ControlLayout_Margin_Percent()
    {
        var root = new Frame
        {
            new Row
            {
                new RowLayoutPlan { Padding = 1, Margin = new Sides(start: 10.Percent())},
                new RowStyle { Border = 1 },
                MediumBox()
            }
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │   ┌─────────────────────────┐│
            │   │┌────┐                   ││
            │   ││    │                   ││
            │   │└────┘                   ││
            │   └─────────────────────────┘│
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
    public void ControlLayout_Padding_Percent()
    {
        var root = new Frame
        {
            new Row
            {
                new RowLayoutPlan { Padding = new Sides(left: 10.Percent())},
                new RowStyle { Border = 1 },
                MediumBox()
            }
        };

        Draw(root);

        Assert.Equal(
            """
            ┌──────────────────────────────┐
            │┌──┬────┬────────────────────┐│
            ││  │    │                    ││
            │└──┴────┴────────────────────┘│
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
            │                              │
            └──────────────────────────────┘
            """,
            _canvas.ToString());
    }
}