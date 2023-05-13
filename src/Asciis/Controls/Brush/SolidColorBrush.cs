﻿namespace No8.Ascii.Controls;

public sealed class SolidColorBrush : Brush, IEquatable<SolidColorBrush>
{
    public Color Color { get; init; } = Color.Transparent;
    public SolidColorBrush() { }
    public SolidColorBrush(Color color) { Color = color; }

    public override Color GetColorAt(float offset) => Color;

    public bool Equals(SolidColorBrush? other)
    {
        return Color.Equals(other?.Color);
    }

    public static   Lazy<SolidColorBrush> Transparent              => new(() => new SolidColorBrush(Color.Transparent));
    public static   Lazy<SolidColorBrush> AliceBlue                => new(() => new SolidColorBrush(Color.AliceBlue));
    public static   Lazy<SolidColorBrush> AntiqueWhite             => new(() => new SolidColorBrush(Color.AntiqueWhite));
    public static   Lazy<SolidColorBrush> Aqua                     => new(() => new SolidColorBrush(Color.Aqua));
    public static   Lazy<SolidColorBrush> Aquamarine               => new(() => new SolidColorBrush(Color.Aquamarine));
    public static   Lazy<SolidColorBrush> Azure                    => new(() => new SolidColorBrush(Color.Azure));
    public static   Lazy<SolidColorBrush> Beige                    => new(() => new SolidColorBrush(Color.Beige));
    public static   Lazy<SolidColorBrush> Bisque                   => new(() => new SolidColorBrush(Color.Bisque));
    public static   Lazy<SolidColorBrush> Black                    => new(() => new SolidColorBrush(Color.Black));
    public static   Lazy<SolidColorBrush> BlanchedAlmond           => new(() => new SolidColorBrush(Color.BlanchedAlmond));
    public static   Lazy<SolidColorBrush> Blue                     => new(() => new SolidColorBrush(Color.Blue));
    public static   Lazy<SolidColorBrush> BlueViolet               => new(() => new SolidColorBrush(Color.BlueViolet));
    public static   Lazy<SolidColorBrush> Brown                    => new(() => new SolidColorBrush(Color.Brown));
    public static   Lazy<SolidColorBrush> BurlyWood                => new(() => new SolidColorBrush(Color.BurlyWood));
    public static   Lazy<SolidColorBrush> CadetBlue                => new(() => new SolidColorBrush(Color.CadetBlue));
    public static   Lazy<SolidColorBrush> Chartreuse               => new(() => new SolidColorBrush(Color.Chartreuse));
    public static   Lazy<SolidColorBrush> Chocolate                => new(() => new SolidColorBrush(Color.Chocolate));
    public static   Lazy<SolidColorBrush> Coral                    => new(() => new SolidColorBrush(Color.Coral));
    public static   Lazy<SolidColorBrush> CornflowerBlue           => new(() => new SolidColorBrush(Color.CornflowerBlue));
    public static   Lazy<SolidColorBrush> Cornsilk                 => new(() => new SolidColorBrush(Color.Cornsilk));
    public static   Lazy<SolidColorBrush> Crimson                  => new(() => new SolidColorBrush(Color.Crimson));
    public static   Lazy<SolidColorBrush> Cyan                     => new(() => new SolidColorBrush(Color.Cyan));
    public static   Lazy<SolidColorBrush> DarkBlue                 => new(() => new SolidColorBrush(Color.DarkBlue));
    public static   Lazy<SolidColorBrush> DarkCyan                 => new(() => new SolidColorBrush(Color.DarkCyan));
    public static   Lazy<SolidColorBrush> DarkGoldenrod            => new(() => new SolidColorBrush(Color.DarkGoldenrod));
    public static   Lazy<SolidColorBrush> DarkGray                 => new(() => new SolidColorBrush(Color.DarkGray));
    public static   Lazy<SolidColorBrush> DarkGreen                => new(() => new SolidColorBrush(Color.DarkGreen));
    public static   Lazy<SolidColorBrush> DarkKhaki                => new(() => new SolidColorBrush(Color.DarkKhaki));
    public static   Lazy<SolidColorBrush> DarkMagenta              => new(() => new SolidColorBrush(Color.DarkMagenta));
    public static   Lazy<SolidColorBrush> DarkOliveGreen           => new(() => new SolidColorBrush(Color.DarkOliveGreen));
    public static   Lazy<SolidColorBrush> DarkOrange               => new(() => new SolidColorBrush(Color.DarkOrange));
    public static   Lazy<SolidColorBrush> DarkOrchid               => new(() => new SolidColorBrush(Color.DarkOrchid));
    public static   Lazy<SolidColorBrush> DarkRed                  => new(() => new SolidColorBrush(Color.DarkRed));
    public static   Lazy<SolidColorBrush> DarkSalmon               => new(() => new SolidColorBrush(Color.DarkSalmon));
    public static   Lazy<SolidColorBrush> DarkSeaGreen             => new(() => new SolidColorBrush(Color.DarkSeaGreen));
    public static   Lazy<SolidColorBrush> DarkSlateBlue            => new(() => new SolidColorBrush(Color.DarkSlateBlue));
    public static   Lazy<SolidColorBrush> DarkSlateGray            => new(() => new SolidColorBrush(Color.DarkSlateGray));
    public static   Lazy<SolidColorBrush> DarkTurquoise            => new(() => new SolidColorBrush(Color.DarkTurquoise));
    public static   Lazy<SolidColorBrush> DarkViolet               => new(() => new SolidColorBrush(Color.DarkViolet));
    public static   Lazy<SolidColorBrush> DeepPink                 => new(() => new SolidColorBrush(Color.DeepPink));
    public static   Lazy<SolidColorBrush> DeepSkyBlue              => new(() => new SolidColorBrush(Color.DeepSkyBlue));
    public static   Lazy<SolidColorBrush> DimGray                  => new(() => new SolidColorBrush(Color.DimGray));
    public static   Lazy<SolidColorBrush> DodgerBlue               => new(() => new SolidColorBrush(Color.DodgerBlue));
    public static   Lazy<SolidColorBrush> Firebrick                => new(() => new SolidColorBrush(Color.Firebrick));
    public static   Lazy<SolidColorBrush> FloralWhite              => new(() => new SolidColorBrush(Color.FloralWhite));
    public static   Lazy<SolidColorBrush> ForestGreen              => new(() => new SolidColorBrush(Color.ForestGreen));
    public static   Lazy<SolidColorBrush> Fuchsia                  => new(() => new SolidColorBrush(Color.Fuchsia));
    public static   Lazy<SolidColorBrush> Gainsboro                => new(() => new SolidColorBrush(Color.Gainsboro));
    public static   Lazy<SolidColorBrush> GhostWhite               => new(() => new SolidColorBrush(Color.GhostWhite));
    public static   Lazy<SolidColorBrush> Gold                     => new(() => new SolidColorBrush(Color.Gold));
    public static   Lazy<SolidColorBrush> Goldenrod                => new(() => new SolidColorBrush(Color.Goldenrod));
    public static   Lazy<SolidColorBrush> Gray                     => new(() => new SolidColorBrush(Color.Gray));
    public static   Lazy<SolidColorBrush> Green                    => new(() => new SolidColorBrush(Color.Green));
    public static   Lazy<SolidColorBrush> GreenYellow              => new(() => new SolidColorBrush(Color.GreenYellow));
    public static   Lazy<SolidColorBrush> Honeydew                 => new(() => new SolidColorBrush(Color.Honeydew));
    public static   Lazy<SolidColorBrush> HotPink                  => new(() => new SolidColorBrush(Color.HotPink));
    public static   Lazy<SolidColorBrush> IndianRed                => new(() => new SolidColorBrush(Color.IndianRed));
    public static   Lazy<SolidColorBrush> Indigo                   => new(() => new SolidColorBrush(Color.Indigo));
    public static   Lazy<SolidColorBrush> Ivory                    => new(() => new SolidColorBrush(Color.Ivory));
    public static   Lazy<SolidColorBrush> Khaki                    => new(() => new SolidColorBrush(Color.Khaki));
    public static   Lazy<SolidColorBrush> Lavender                 => new(() => new SolidColorBrush(Color.Lavender));
    public static   Lazy<SolidColorBrush> LavenderBlush            => new(() => new SolidColorBrush(Color.LavenderBlush));
    public static   Lazy<SolidColorBrush> LawnGreen                => new(() => new SolidColorBrush(Color.LawnGreen));
    public static   Lazy<SolidColorBrush> LemonChiffon             => new(() => new SolidColorBrush(Color.LemonChiffon));
    public static   Lazy<SolidColorBrush> LightBlue                => new(() => new SolidColorBrush(Color.LightBlue));
    public static   Lazy<SolidColorBrush> LightCoral               => new(() => new SolidColorBrush(Color.LightCoral));
    public static   Lazy<SolidColorBrush> LightCyan                => new(() => new SolidColorBrush(Color.LightCyan));
    public static   Lazy<SolidColorBrush> LightGoldenrodYellow     => new(() => new SolidColorBrush(Color.LightGoldenrodYellow));
    public static   Lazy<SolidColorBrush> LightGreen               => new(() => new SolidColorBrush(Color.LightGreen));
    public static   Lazy<SolidColorBrush> LightGray                => new(() => new SolidColorBrush(Color.LightGray));
    public static   Lazy<SolidColorBrush> LightPink                => new(() => new SolidColorBrush(Color.LightPink));
    public static   Lazy<SolidColorBrush> LightSalmon              => new(() => new SolidColorBrush(Color.LightSalmon));
    public static   Lazy<SolidColorBrush> LightSeaGreen            => new(() => new SolidColorBrush(Color.LightSeaGreen));
    public static   Lazy<SolidColorBrush> LightSkyBlue             => new(() => new SolidColorBrush(Color.LightSkyBlue));
    public static   Lazy<SolidColorBrush> LightSlateGray           => new(() => new SolidColorBrush(Color.LightSlateGray));
    public static   Lazy<SolidColorBrush> LightSteelBlue           => new(() => new SolidColorBrush(Color.LightSteelBlue));
    public static   Lazy<SolidColorBrush> LightYellow              => new(() => new SolidColorBrush(Color.LightYellow));
    public static   Lazy<SolidColorBrush> Lime                     => new(() => new SolidColorBrush(Color.Lime));
    public static   Lazy<SolidColorBrush> LimeGreen                => new(() => new SolidColorBrush(Color.LimeGreen));
    public static   Lazy<SolidColorBrush> Linen                    => new(() => new SolidColorBrush(Color.Linen));
    public static   Lazy<SolidColorBrush> Magenta                  => new(() => new SolidColorBrush(Color.Magenta));
    public static   Lazy<SolidColorBrush> Maroon                   => new(() => new SolidColorBrush(Color.Maroon));
    public static   Lazy<SolidColorBrush> MediumAquamarine         => new(() => new SolidColorBrush(Color.MediumAquamarine));
    public static   Lazy<SolidColorBrush> MediumBlue               => new(() => new SolidColorBrush(Color.MediumBlue));
    public static   Lazy<SolidColorBrush> MediumOrchid             => new(() => new SolidColorBrush(Color.MediumOrchid));
    public static   Lazy<SolidColorBrush> MediumPurple             => new(() => new SolidColorBrush(Color.MediumPurple));
    public static   Lazy<SolidColorBrush> MediumSeaGreen           => new(() => new SolidColorBrush(Color.MediumSeaGreen));
    public static   Lazy<SolidColorBrush> MediumSlateBlue          => new(() => new SolidColorBrush(Color.MediumSlateBlue));
    public static   Lazy<SolidColorBrush> MediumSpringGreen        => new(() => new SolidColorBrush(Color.MediumSpringGreen));
    public static   Lazy<SolidColorBrush> MediumTurquoise          => new(() => new SolidColorBrush(Color.MediumTurquoise));
    public static   Lazy<SolidColorBrush> MediumVioletRed          => new(() => new SolidColorBrush(Color.MediumVioletRed));
    public static   Lazy<SolidColorBrush> MidnightBlue             => new(() => new SolidColorBrush(Color.MidnightBlue));
    public static   Lazy<SolidColorBrush> MintCream                => new(() => new SolidColorBrush(Color.MintCream));
    public static   Lazy<SolidColorBrush> MistyRose                => new(() => new SolidColorBrush(Color.MistyRose));
    public static   Lazy<SolidColorBrush> Moccasin                 => new(() => new SolidColorBrush(Color.Moccasin));
    public static   Lazy<SolidColorBrush> NavajoWhite              => new(() => new SolidColorBrush(Color.NavajoWhite));
    public static   Lazy<SolidColorBrush> Navy                     => new(() => new SolidColorBrush(Color.Navy));
    public static   Lazy<SolidColorBrush> OldLace                  => new(() => new SolidColorBrush(Color.OldLace));
    public static   Lazy<SolidColorBrush> Olive                    => new(() => new SolidColorBrush(Color.Olive));
    public static   Lazy<SolidColorBrush> OliveDrab                => new(() => new SolidColorBrush(Color.OliveDrab));
    public static   Lazy<SolidColorBrush> Orange                   => new(() => new SolidColorBrush(Color.Orange));
    public static   Lazy<SolidColorBrush> OrangeRed                => new(() => new SolidColorBrush(Color.OrangeRed));
    public static   Lazy<SolidColorBrush> Orchid                   => new(() => new SolidColorBrush(Color.Orchid));
    public static   Lazy<SolidColorBrush> PaleGoldenrod            => new(() => new SolidColorBrush(Color.PaleGoldenrod));
    public static   Lazy<SolidColorBrush> PaleGreen                => new(() => new SolidColorBrush(Color.PaleGreen));
    public static   Lazy<SolidColorBrush> PaleTurquoise            => new(() => new SolidColorBrush(Color.PaleTurquoise));
    public static   Lazy<SolidColorBrush> PaleVioletRed            => new(() => new SolidColorBrush(Color.PaleVioletRed));
    public static   Lazy<SolidColorBrush> PapayaWhip               => new(() => new SolidColorBrush(Color.PapayaWhip));
    public static   Lazy<SolidColorBrush> PeachPuff                => new(() => new SolidColorBrush(Color.PeachPuff));
    public static   Lazy<SolidColorBrush> Peru                     => new(() => new SolidColorBrush(Color.Peru));
    public static   Lazy<SolidColorBrush> Pink                     => new(() => new SolidColorBrush(Color.Pink));
    public static   Lazy<SolidColorBrush> Plum                     => new(() => new SolidColorBrush(Color.Plum));
    public static   Lazy<SolidColorBrush> PowderBlue               => new(() => new SolidColorBrush(Color.PowderBlue));
    public static   Lazy<SolidColorBrush> Purple                   => new(() => new SolidColorBrush(Color.Purple));
    public static   Lazy<SolidColorBrush> RebeccaPurple            => new(() => new SolidColorBrush(Color.RebeccaPurple));
    public static   Lazy<SolidColorBrush> Red                      => new(() => new SolidColorBrush(Color.Red));
    public static   Lazy<SolidColorBrush> RosyBrown                => new(() => new SolidColorBrush(Color.RosyBrown));
    public static   Lazy<SolidColorBrush> RoyalBlue                => new(() => new SolidColorBrush(Color.RoyalBlue));
    public static   Lazy<SolidColorBrush> SaddleBrown              => new(() => new SolidColorBrush(Color.SaddleBrown));
    public static   Lazy<SolidColorBrush> Salmon                   => new(() => new SolidColorBrush(Color.Salmon));
    public static   Lazy<SolidColorBrush> SandyBrown               => new(() => new SolidColorBrush(Color.SandyBrown));
    public static   Lazy<SolidColorBrush> SeaGreen                 => new(() => new SolidColorBrush(Color.SeaGreen));
    public static   Lazy<SolidColorBrush> SeaShell                 => new(() => new SolidColorBrush(Color.SeaShell));
    public static   Lazy<SolidColorBrush> Sienna                   => new(() => new SolidColorBrush(Color.Sienna));
    public static   Lazy<SolidColorBrush> Silver                   => new(() => new SolidColorBrush(Color.Silver));
    public static   Lazy<SolidColorBrush> SkyBlue                  => new(() => new SolidColorBrush(Color.SkyBlue));
    public static   Lazy<SolidColorBrush> SlateBlue                => new(() => new SolidColorBrush(Color.SlateBlue));
    public static   Lazy<SolidColorBrush> SlateGray                => new(() => new SolidColorBrush(Color.SlateGray));
    public static   Lazy<SolidColorBrush> Snow                     => new(() => new SolidColorBrush(Color.Snow));
    public static   Lazy<SolidColorBrush> SpringGreen              => new(() => new SolidColorBrush(Color.SpringGreen));
    public static   Lazy<SolidColorBrush> SteelBlue                => new(() => new SolidColorBrush(Color.SteelBlue));
    public static   Lazy<SolidColorBrush> Tan                      => new(() => new SolidColorBrush(Color.Tan));
    public static   Lazy<SolidColorBrush> Teal                     => new(() => new SolidColorBrush(Color.Teal));
    public static   Lazy<SolidColorBrush> Thistle                  => new(() => new SolidColorBrush(Color.Thistle));
    public static   Lazy<SolidColorBrush> Tomato                   => new(() => new SolidColorBrush(Color.Tomato));
    public static   Lazy<SolidColorBrush> Turquoise                => new(() => new SolidColorBrush(Color.Turquoise));
    public static   Lazy<SolidColorBrush> Violet                   => new(() => new SolidColorBrush(Color.Violet));
    public static   Lazy<SolidColorBrush> Wheat                    => new(() => new SolidColorBrush(Color.Wheat));
    public static   Lazy<SolidColorBrush> White                    => new(() => new SolidColorBrush(Color.White));
    public static   Lazy<SolidColorBrush> WhiteSmoke               => new(() => new SolidColorBrush(Color.WhiteSmoke));
    public static   Lazy<SolidColorBrush> Yellow                   => new(() => new SolidColorBrush(Color.Yellow));
    public static   Lazy<SolidColorBrush> YellowGreen              => new(() => new SolidColorBrush(Color.YellowGreen));
}
