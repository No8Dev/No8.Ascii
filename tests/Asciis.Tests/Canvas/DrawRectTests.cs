﻿using No8.Ascii.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Canvas;

[TestClass]
public class DrawRectTests : BaseCanvasTests
{
    public DrawRectTests(ITestOutputHelper context) : base(context)
    {
        _canvas = new No8.Ascii.Canvas(10, 5);
    }

    [Fact]
    public void DrawRect_Single()
    {
        _canvas.DrawRect(Rect.Create( 0,0,10,4 ), LineSet.Single);

        var str = _canvas.ToString();
        var box = 
            """
            ┌────────┐
            │        │
            │        │
            └────────┘
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_Double()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 10, 4), LineSet.Double);

        var str = _canvas.ToString();
        var box = 
            """
            ╔════════╗
            ║        ║
            ║        ║
            ╚════════╝
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_Rounded()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 10, 4), LineSet.Rounded);

        var str = _canvas.ToString();
        var box = """
            ╭────────╮
            │        │
            │        │
            ╰────────╯
            """;
        Assert.Equal(box, str);
    }



    [Fact]
    public void DrawRect_DoubleUnder()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 10, 4), LineSet.DoubleUnder);

        var str = _canvas.ToString();
        var box = """
            ┌────────┐
            │        │
            │        │
            ╘════════╛
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_DoubleOver()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 10, 4), LineSet.DoubleOver);

        var str = _canvas.ToString();
        var box = """
            ╒════════╕
            │        │
            │        │
            └────────┘
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_DoubleRaised()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 10, 4), LineSet.DoubleRaised);

        var str = _canvas.ToString();
        var box = """
            ┌────────╖
            │        ║
            │        ║
            ╘════════╝
            """;
        Assert.Equal(box, str);
    }
    [Fact]
    public void DrawRect_DoublePressed()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 10, 4), LineSet.DoublePressed);

        var str = _canvas.ToString();
        var box = """
            ╔════════╕
            ║        │
            ║        │
            ╙────────┘
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_BothHorizontal()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 5, 4), LineSet.Single);
        _canvas.DrawRect(Rect.Create(5, 0, 5, 4), LineSet.Double);

        var str = _canvas.ToString();
        var box = """
            ┌───┐╔═══╗
            │   │║   ║
            │   │║   ║
            └───┘╚═══╝
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_BothVertical()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 10, 2), LineSet.Single);
        _canvas.DrawRect(Rect.Create(0, 2, 10, 2), LineSet.Double);

        var str = _canvas.ToString();
        var box = """
            ┌────────┐
            └────────┘
            ╔════════╗
            ╚════════╝
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_SingleCross()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 7, 3), LineSet.Single);
        _canvas.DrawRect(Rect.Create(3, 1, 7, 3), LineSet.Rounded);

        var str = _canvas.ToString();
        var box = """
            ┌─────┐
            │  ╭──┼──╮
            └──┼──┘  │
               ╰─────╯
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_DoubleCross()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 7, 3), LineSet.Double);
        _canvas.DrawRect(Rect.Create(3, 1, 7, 3), LineSet.Double);

        var str = _canvas.ToString();
        var box = """
            ╔═════╗
            ║  ╔══╬══╗
            ╚══╬══╝  ║
               ╚═════╝
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_BothCrossDoubleTop()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 7, 3), LineSet.Single);
        _canvas.DrawRect(Rect.Create(3, 1, 7, 3), LineSet.Double);

        var str = _canvas.ToString();
        var box = """
            ┌─────┐
            │  ╔══╪══╗
            └──╫──┘  ║
               ╚═════╝
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_BothCrossSingleTop()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 7, 3), LineSet.Double);
        _canvas.DrawRect(Rect.Create(3, 1, 7, 3), LineSet.Single);

        var str = _canvas.ToString();
        var box = """
            ╔═════╗
            ║  ┌──╫──┐
            ╚══╪══╝  │
               └─────┘
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_SingleSideBySide()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 5, 3), LineSet.Single);
        _canvas.DrawRect(Rect.Create(4, 0, 6, 4), LineSet.Single);

        var str = _canvas.ToString();
        var box = """
            ┌───┬────┐
            │   │    │
            └───┤    │
                └────┘
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_RoundedSideBySide()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 5, 3), LineSet.Rounded);
        _canvas.DrawRect(Rect.Create(4, 0, 6, 4), LineSet.Rounded);

        var str = _canvas.ToString();
        var box = """
            ╭───┬────╮
            │   │    │
            ╰───┤    │
                ╰────╯
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_DoubleSideBySide()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 5, 3), LineSet.Double);
        _canvas.DrawRect(Rect.Create(4, 0, 6, 4), LineSet.Double);

        var str = _canvas.ToString();
        var box = """
            ╔═══╦════╗
            ║   ║    ║
            ╚═══╣    ║
                ╚════╝
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_BothSideBySideDoubleTop()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 5, 3), LineSet.Single);
        _canvas.DrawRect(Rect.Create(4, 0, 6, 4), LineSet.Double);

        var str = _canvas.ToString();
        var box = """
            ┌───╔════╗
            │   ║    ║
            └───╢    ║
                ╚════╝
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_BothSideBySideSingleTop()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 5, 3), LineSet.Double);
        _canvas.DrawRect(Rect.Create(4, 0, 6, 4), LineSet.Single);

        var str = _canvas.ToString();
        var box = """
            ╔═══┌────┐
            ║   │    │
            ╚═══╡    │
                └────┘
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_BothSingleInside()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 10, 3), LineSet.Double);
        _canvas.DrawRect(Rect.Create(2, 0, 5, 4), LineSet.Single);

        var str = _canvas.ToString();
        var box = """
            ╔═┌───┐══╗
            ║ │   │  ║
            ╚═╪═══╪══╝
              └───┘
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_BothDoubleInside()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 10, 3), LineSet.Single);
        _canvas.DrawRect(Rect.Create(2, 0, 5, 4), LineSet.Double);

        var str = _canvas.ToString();
        var box = """
            ┌─╔═══╗──┐
            │ ║   ║  │
            └─╫───╫──┘
              ╚═══╝
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawRect_BothRoundedDoubleInside()
    {
        _canvas.DrawRect(Rect.Create(0, 0, 10, 3), LineSet.Rounded);
        _canvas.DrawRect(Rect.Create(2, 0, 5,  4), LineSet.Double);

        var str = _canvas.ToString();
        var box = """
            ╭─╔═══╗──╮
            │ ║   ║  │
            ╰─╫───╫──╯
              ╚═══╝
            """;
        Assert.Equal(box, str);
    }
}