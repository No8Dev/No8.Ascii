﻿using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.Canvas;

[TestClass]
public class DrawThingsTests
{
    private readonly No8.Ascii.Canvas _canvas;

    public DrawThingsTests()
    {
        _canvas = new No8.Ascii.Canvas(40, 20);
    }


    [Fact]
    public void DrawTriange()
    {
        _canvas.DrawTriangle(1, 16, 10, 0, 39, 17, Runes.Shapes.SquareSolid);

        var str = _canvas.ToString();
        var box = """
                      ■
                     ■ ■■
                     ■   ■■
                    ■      ■
                    ■       ■■
                   ■          ■■
                   ■            ■■
                  ■               ■
                  ■                ■■
                 ■                   ■■
                ■                      ■
                ■                       ■■
               ■                          ■■
               ■                            ■■
              ■                               ■
              ■                                ■■
             ■■■■■■■■■■■■■■■■■■■                 ■■
                                ■■■■■■■■■■■■■■■■■■■■
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawFillTriange()
    {
        _canvas.FillTriangle(1, 16, 10, 0, 39, 17, Runes.Block.Solid);

        var str = _canvas.ToString();
        var box = """
                      █
                     ████
                     ██████
                    ████████
                    ██████████
                   █████████████
                   ███████████████
                  █████████████████
                 ████████████████████
                 ██████████████████████
                ████████████████████████
                ██████████████████████████
               █████████████████████████████
               ███████████████████████████████
              █████████████████████████████████
              ███████████████████████████████████
             ██████████████████████████████████████
                                ████████████████████
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawCircle()
    {
        _canvas.CircleCompensateRatio = 1.0f;
        _canvas.DrawCircle(7, 7, 7, Runes.Block.DarkShade);

        var str = _canvas.ToString();
        var box = """
                 ▓▓▓▓▓
               ▓▓     ▓▓
              ▓         ▓
             ▓           ▓
             ▓           ▓
            ▓             ▓
            ▓             ▓
            ▓             ▓
            ▓             ▓
            ▓             ▓
             ▓           ▓
             ▓           ▓
              ▓         ▓
               ▓▓     ▓▓
                 ▓▓▓▓▓
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawFillCircle()
    {
        _canvas.CircleCompensateRatio = 1.0f;
        _canvas.FillCircle(7, 7, 7, Runes.Block.LightShade);
        _canvas.FillCircle(7, 7, 6, Runes.Block.MediumShade);
        _canvas.FillCircle(7, 7, 4, Runes.Block.DarkShade);

        var str = _canvas.ToString();
        var box = """
                 ░░░░░
               ░░▒▒▒▒▒░░
              ░░▒▒▒▒▒▒▒░░
             ░░▒▒▒▓▓▓▒▒▒░░
             ░▒▒▓▓▓▓▓▓▓▒▒░
            ░▒▒▒▓▓▓▓▓▓▓▒▒▒░
            ░▒▒▓▓▓▓▓▓▓▓▓▒▒░
            ░▒▒▓▓▓▓▓▓▓▓▓▒▒░
            ░▒▒▓▓▓▓▓▓▓▓▓▒▒░
            ░▒▒▒▓▓▓▓▓▓▓▒▒▒░
             ░▒▒▓▓▓▓▓▓▓▒▒░
             ░░▒▒▒▓▓▓▒▒▒░░
              ░░▒▒▒▒▒▒▒░░
               ░░▒▒▒▒▒░░
                 ░░░░░
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void DrawSprite()
    {
        _canvas.DrawSprite(0, 0, GetSprite());

        var str = _canvas.ToString();
        var box = """
            ╭────────╮
            │╒══════╕│
            ││ ▓  ▓ ││
            │└──────┘│
            │▲▲▲▲▲▲▲ │
            │ ▲▲▲▲▲  │
            │  ▲▲▲   │
            │   ▲    │
            │        │
            ╰────────╯
            """;
        Assert.Equal(box, str);
    }

    [Fact]
    public void GenerateSprite()
    {
        _canvas.Fill(0, 0, 10, 10, (Rune)' ', Color.White, Color.Black);
        _canvas.DrawRect(0, 0, 9, 9, LineSet.Rounded, Color.GreenYellow);
        _canvas.FillTriangle(1, 4, 7, 4, 4, 7, Runes.Shapes.TriangleSolidUp, Color.OrangeRed);
        _canvas.DrawRect(1, 1, 8, 3, LineSet.DoubleOver, Color.Purple);
        _canvas.SetGlyph(3, 2, new Glyph(Runes.Block.DarkShade, Color.DeepSkyBlue));
        _canvas.SetGlyph(6, 2, new Glyph(Runes.Block.DarkShade, Color.DeepSkyBlue));

        var sprite = _canvas.ExportSprite(0, 0, 10, 10);

        var str = _canvas.ToString();
        var box = """
            ╭────────╮
            │╒══════╕│
            ││ ▓  ▓ ││
            │└──────┘│
            │▲▲▲▲▲▲▲ │
            │ ▲▲▲▲▲  │
            │  ▲▲▲   │
            │   ▲    │
            │        │
            ╰────────╯
            """;
        Assert.Equal(box, str);

        str = sprite!.ToString();
        Assert.Equal(SpriteStr, str);
    }

    public static string SpriteStr = """
        10
        10
        ╭────────╮
        │╒══════╕│
        ││ ▓  ▓ ││
        │└──────┘│
        │▲▲▲▲▲▲▲ │
        │ ▲▲▲▲▲  │
        │  ▲▲▲   │
        │   ▲    │
        │        │
        ╰────────╯
        FFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2F
        FFADFF2FFF800080FF800080FF800080FF800080FF800080FF800080FF800080FF800080FFADFF2F
        FFADFF2FFF800080FF800080FF00BFFFFF800080FF800080FF00BFFFFF800080FF800080FFADFF2F
        FFADFF2FFF800080FF800080FF800080FF800080FF800080FF800080FF800080FF800080FFADFF2F
        FFADFF2FFFFF4500FFFF4500FFFF4500FFFF4500FFFF4500FFFF4500FFFF4500FFADFF2FFFADFF2F
        FFADFF2FFFADFF2FFFFF4500FFFF4500FFFF4500FFFF4500FFFF4500FFADFF2FFFADFF2FFFADFF2F
        FFADFF2FFFADFF2FFFADFF2FFFFF4500FFFF4500FFFF4500FFADFF2FFFADFF2FFFADFF2FFFADFF2F
        FFADFF2FFFADFF2FFFADFF2FFFADFF2FFFFF4500FFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2F
        FFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2F
        FFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2FFFADFF2F
        FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000
        FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000
        FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000
        FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000
        FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000
        FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000
        FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000
        FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000
        FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000
        FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000FF000000

        """;
    public static Sprite GetSprite()
    {
        return new Sprite(SpriteStr);
    }
}