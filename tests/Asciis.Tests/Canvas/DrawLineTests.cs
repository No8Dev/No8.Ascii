using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.Canvas;

[TestClass]
public class DrawLineTests
{
    private readonly No8.Ascii.Canvas _canvas;

    public DrawLineTests()
    {
        _canvas = new No8.Ascii.Canvas(10, 5);
    }

    [Fact]
    public void DrawLineHorz()
    {
        _canvas.DrawLine( 0, 0, 9, 0, LineSet.Single ); // Left to right
        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Double );
        _canvas.DrawLine( 9, 2, 0, 2, LineSet.Single ); // Right to Left
        _canvas.DrawLine( 9, 3, 0, 3, LineSet.Double );

        var str = _canvas.ToString();
        Assert.Equal( 
            """
            ╶────────╴
            ╺════════╸
            ╶────────╴
            ╺════════╸
            """, str );
    }

    [Fact]
    public void DrawLineVert()
    {
        _canvas.DrawLine( 0, 0, 0, 3, LineSet.Single ); // Top to Bottom
        _canvas.DrawLine( 1, 0, 1, 3, LineSet.Double );
        _canvas.DrawLine( 2, 3, 2, 0, LineSet.Single ); // Bottom to Top
        _canvas.DrawLine( 3, 3, 3, 0, LineSet.Double );

        var str = _canvas.ToString();
        var box = """
            ╷╥╷╥
            │║│║
            │║│║
            ╵╨╵╨
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineSingleCross()
    {
        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Single );
        _canvas.DrawLine( 2, 0, 2, 3, LineSet.Single );

        var str = _canvas.ToString();
        var box = """
              ╷
            ╶─┼──────╴
              │
              ╵
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineDoubleCross()
    {
        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Double );
        _canvas.DrawLine( 2, 0, 2, 3, LineSet.Double );

        var str = _canvas.ToString();
        var box = """
              ╥
            ╺═╬══════╸
              ║
              ╨
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineBothCrossDoubleTop()
    {
        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Single );
        _canvas.DrawLine( 2, 0, 2, 3, LineSet.Double );

        var str = _canvas.ToString();
        var box = """
              ╥
            ╶─╫──────╴
              ║
              ╨
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineBothCrossSingleTop()
    {
        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Double );
        _canvas.DrawLine( 2, 0, 2, 3, LineSet.Single );

        var str = _canvas.ToString();
        var box = """
              ╷
            ╺═╪══════╸
              │
              ╵
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLinesSingle()
    {
        _canvas.DrawLine( 0, 0, 0, 3, LineSet.Single );
        _canvas.DrawLine( 6, 0, 6, 3, LineSet.Single );
        _canvas.DrawLine( 9, 0, 9, 3, LineSet.Single );

        _canvas.DrawLine( 0, 0, 9, 0, LineSet.Single );
        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Single );
        _canvas.DrawLine( 0, 3, 9, 3, LineSet.Single );

        var str = _canvas.ToString();
        var box = """
            ┌─────┬──┐
            ├─────┼──┤
            │     │  │
            └─────┴──┘
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLinesDouble()
    {
        _canvas.DrawLine( 0, 0, 0, 3, LineSet.Double );
        _canvas.DrawLine( 6, 0, 6, 3, LineSet.Double );
        _canvas.DrawLine( 9, 0, 9, 3, LineSet.Double );

        _canvas.DrawLine( 0, 0, 9, 0, LineSet.Double );
        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Double );
        _canvas.DrawLine( 0, 3, 9, 3, LineSet.Double );

        var str = _canvas.ToString();
        var box = """
            ╔═════╦══╗
            ╠═════╬══╣
            ║     ║  ║
            ╚═════╩══╝
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLinesBothTopDouble()
    {
        _canvas.DrawLine( 0, 0, 0, 3, LineSet.Single );
        _canvas.DrawLine( 6, 0, 6, 3, LineSet.Single );
        _canvas.DrawLine( 9, 0, 9, 3, LineSet.Single );

        _canvas.DrawLine( 0, 0, 9, 0, LineSet.Double );
        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Double );
        _canvas.DrawLine( 0, 3, 9, 3, LineSet.Double );

        var str = _canvas.ToString();
        var box = """
            ╒═════╤══╕
            ╞═════╪══╡
            │     │  │
            ╘═════╧══╛
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLinesBothTopSingle()
    {
        _canvas.DrawLine( 0, 0, 0, 3, LineSet.Double );
        _canvas.DrawLine( 6, 0, 6, 3, LineSet.Double );
        _canvas.DrawLine( 9, 0, 9, 3, LineSet.Double );

        _canvas.DrawLine( 0, 0, 9, 0, LineSet.Single );
        _canvas.DrawLine( 0, 1, 9, 1, LineSet.Single );
        _canvas.DrawLine( 0, 3, 9, 3, LineSet.Single );

        var str = _canvas.ToString();
        var box = """
            ╓─────╥──╖
            ╟─────╫──╢
            ║     ║  ║
            ╙─────╨──╜
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineHorzHiag()
    {
        _canvas.DrawLine( 0, 0, 9, 1, LineSet.Single );
        _canvas.DrawLine( 0, 1, 9, 3, LineSet.Double );

        var str = _canvas.ToString();
        var box = """
            ╶────┐
            ╺═══╗└───╴
                ╚══╗
                   ╚═╸
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineVertDiag()
    {
        _canvas.DrawLine( 0, 0, 2, 3, LineSet.Single );
        _canvas.DrawLine( 3, 0, 6, 3, LineSet.Double );
        _canvas.DrawLine( 5, 0, 9, 3, LineSet.Single );

        var str = _canvas.ToString();
        var box = """
            ╷  ╥ ╶─┐
            │  ╚╗  └┐
            └┐  ╚╗  └┐
             └┐  ╚╗  └
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineSingleDiagCross()
    {
        _canvas.DrawLine( 0, 0, 9, 3, LineSet.Single ); // Down Right
        _canvas.DrawLine( 0, 3, 9, 0, LineSet.Single ); // Up Right

        var str = _canvas.ToString();
        var box = """
            ╶──┐   ┌─╴
               └─┬─┘
               ┌─┴─┐
            ╶──┘   └─╴
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineDoubleDiagCross()
    {
        _canvas.DrawLine( 9, 0, 0, 3, LineSet.Double ); // Down Left
        _canvas.DrawLine( 9, 3, 0, 0, LineSet.Double ); // Up Left

        var str = _canvas.ToString();
        var box = """
            ╺══╗   ╔═╸
               ╚═╦═╝
               ╔═╩═╗
            ╺══╝   ╚═╸
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineBothDiagCrossTopDouble()
    {
        _canvas.DrawLine( 0, 0, 9, 3, LineSet.Single ); // Down Right
        _canvas.DrawLine( 0, 3, 9, 0, LineSet.Double ); // Up Right

        var str = _canvas.ToString();
        var box = """
            ╶──┐   ╔═╸
               └─╔═╝
               ╔═╝─┐
            ╺══╝   └─╴
            """;
        Assert.Equal( box, str );
    }

    [Fact]
    public void DrawLineBothDiagCrossTopSingle()
    {
        _canvas.DrawLine( 0, 0, 9, 3, LineSet.Double ); // Down Right
        _canvas.DrawLine( 9, 0, 0, 3, LineSet.Single ); // Down Left

        var str = _canvas.ToString();
        var box = """
            ╺══╗   ┌─╴
               ╚═┌─┘
               ┌─┘═╗
            ╶──┘   ╚═╸
            """;
        Assert.Equal( box, str );
    }
}