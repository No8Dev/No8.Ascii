using Xunit;

namespace Asciis.App.Tests.Canvas;

[TestClass]
public class DrawLineTests
{
    private readonly App.Canvas _canvas;

    public DrawLineTests()
    {
        _canvas = new App.Canvas(10, 5);
    }

    [Fact]
    public void DrawLineHorz()
    {
        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Single ); // Left to right
        _canvas.DrawLine( 0, 2, 9, 2, LineSet.Double );
        _canvas.DrawLine( 9, 3, 0, 3, LineSet.Single ); // Right to Left
        _canvas.DrawLine( 9, 4, 0, 4, LineSet.Double );

        var str = _canvas.ToString();
        var box = @"
╶────────╴
╺════════╸
╶────────╴
╺════════╸";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineVert()
    {
        _canvas.DrawLine( 0, 1, 0, 4, LineSet.Single ); // Top to Bottom
        _canvas.DrawLine( 1, 1, 1, 4, LineSet.Double );
        _canvas.DrawLine( 2, 4, 2, 1, LineSet.Single ); // Bottom to Top
        _canvas.DrawLine( 3, 4, 3, 1, LineSet.Double );

        var str = _canvas.ToString();
        var box = @"
╷╥╷╥
│║│║
│║│║
╵╨╵╨";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineSingleCross()
    {
        _canvas.DrawLine( 0, 2, 9, 2, LineSet.Single );
        _canvas.DrawLine( 2, 1, 2, 4, LineSet.Single );

        var str = _canvas.ToString();
        var box = @"
  ╷
╶─┼──────╴
  │
  ╵";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineDoubleCross()
    {
        _canvas.DrawLine( 0, 2, 9, 2, LineSet.Double );
        _canvas.DrawLine( 2, 1, 2, 4, LineSet.Double );

        var str = _canvas.ToString();
        var box = @"
  ╥
╺═╬══════╸
  ║
  ╨";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineBothCrossDoubleTop()
    {
        _canvas.DrawLine( 0, 2, 9, 2, LineSet.Single );
        _canvas.DrawLine( 2, 1, 2, 4, LineSet.Double );

        var str = _canvas.ToString();
        var box = @"
  ╥
╶─╫──────╴
  ║
  ╨";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineBothCrossSingleTop()
    {
        _canvas.DrawLine( 0, 2, 9, 2, LineSet.Double );
        _canvas.DrawLine( 2, 1, 2, 4, LineSet.Single );

        var str = _canvas.ToString();
        var box = @"
  ╷
╺═╪══════╸
  │
  ╵";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLinesSingle()
    {
        _canvas.DrawLine( 0, 1, 0, 4, LineSet.Single );
        _canvas.DrawLine( 6, 1, 6, 4, LineSet.Single );
        _canvas.DrawLine( 9, 1, 9, 4, LineSet.Single );

        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Single );
        _canvas.DrawLine( 0, 2, 9, 2, LineSet.Single );
        _canvas.DrawLine( 0, 4, 9, 4, LineSet.Single );

        var str = _canvas.ToString();
        var box = @"
┌─────┬──┐
├─────┼──┤
│     │  │
└─────┴──┘";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLinesDouble()
    {
        _canvas.DrawLine( 0, 1, 0, 4, LineSet.Double );
        _canvas.DrawLine( 6, 1, 6, 4, LineSet.Double );
        _canvas.DrawLine( 9, 1, 9, 4, LineSet.Double );

        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Double );
        _canvas.DrawLine( 0, 2, 9, 2, LineSet.Double );
        _canvas.DrawLine( 0, 4, 9, 4, LineSet.Double );

        var str = _canvas.ToString();
        var box = @"
╔═════╦══╗
╠═════╬══╣
║     ║  ║
╚═════╩══╝";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLinesBothTopDouble()
    {
        _canvas.DrawLine( 0, 1, 0, 4, LineSet.Single );
        _canvas.DrawLine( 6, 1, 6, 4, LineSet.Single );
        _canvas.DrawLine( 9, 1, 9, 4, LineSet.Single );

        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Double );
        _canvas.DrawLine( 0, 2, 9, 2, LineSet.Double );
        _canvas.DrawLine( 0, 4, 9, 4, LineSet.Double );

        var str = _canvas.ToString();
        var box = @"
╒═════╤══╕
╞═════╪══╡
│     │  │
╘═════╧══╛";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLinesBothTopSingle()
    {
        _canvas.DrawLine( 0, 1, 0, 4, LineSet.Double );
        _canvas.DrawLine( 6, 1, 6, 4, LineSet.Double );
        _canvas.DrawLine( 9, 1, 9, 4, LineSet.Double );

        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Single );
        _canvas.DrawLine( 0, 2, 9, 2, LineSet.Single );
        _canvas.DrawLine( 0, 4, 9, 4, LineSet.Single );

        var str = _canvas.ToString();
        var box = @"
╓─────╥──╖
╟─────╫──╢
║     ║  ║
╙─────╨──╜";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineHorzHiag()
    {
        _canvas.DrawLine( 0, 1, 9, 2, LineSet.Single );
        _canvas.DrawLine( 0, 2, 9, 4, LineSet.Double );

        var str = _canvas.ToString();
        var box = @"
╶────┐
╺═══╗└───╴
    ╚══╗
       ╚═╸";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineVertDiag()
    {
        _canvas.DrawLine( 0, 1, 2, 4, LineSet.Single );
        _canvas.DrawLine( 3, 1, 6, 4, LineSet.Double );
        _canvas.DrawLine( 5, 1, 9, 4, LineSet.Single );

        var str = _canvas.ToString();
        var box = @"
╷  ╥ ╶─┐
│  ╚╗  └┐
└┐  ╚╗  └┐
 └┐  ╚╗  └";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineSingleDiagCross()
    {
        _canvas.DrawLine( 0, 1, 9, 4, LineSet.Single ); // Down Right
        _canvas.DrawLine( 0, 4, 9, 1, LineSet.Single ); // Up Right

        var str = _canvas.ToString();
        var box = @"
╶──┐   ┌─╴
   └─┬─┘
   ┌─┴─┐
╶──┘   └─╴";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineDoubleDiagCross()
    {
        _canvas.DrawLine( 9, 1, 0, 4, LineSet.Double ); // Down Left
        _canvas.DrawLine( 9, 4, 0, 1, LineSet.Double ); // Up Left

        var str = _canvas.ToString();
        var box = @"
╺══╗   ╔═╸
   ╚═╦═╝
   ╔═╩═╗
╺══╝   ╚═╸";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineBothDiagCrossTopDouble()
    {
        _canvas.DrawLine( 0, 1, 9, 4, LineSet.Single ); // Down Right
        _canvas.DrawLine( 0, 4, 9, 1, LineSet.Double ); // Up Right

        var str = _canvas.ToString();
        var box = @"
╶──┐   ╔═╸
   └─╔═╝
   ╔═╝─┐
╺══╝   └─╴";
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineBothDiagCrossTopSingle()
    {
        _canvas.DrawLine( 0, 1, 9, 4, LineSet.Double ); // Down Right
        _canvas.DrawLine( 9, 1, 0, 4, LineSet.Single ); // Down Left

        var str = _canvas.ToString();
        var box = @"
╺══╗   ┌─╴
   ╚═┌─┘
   ┌─┘═╗
╶──┘   ╚═╸";
        Assert.Equal( box, str );
    }
}