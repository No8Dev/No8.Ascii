namespace Asciis.App;

public static class ColorExtensions
{
    /// <summary>
    /// Adjust brightness of color by factor.
    /// Factor must be between -1.0 and 1.0
    /// </summary>
    public static Color AdjustBy(this Color color, float factor)
    {
        var red = (float)color.R;
        var green = (float)color.G;
        var blue = (float)color.B;

        if (factor < 0)
        {
            factor = 1 + factor;
            red *= factor;
            green *= factor;
            blue *= factor;
        }
        else
        {
            red = (255 - red) * factor + red;
            green = (255 - green) * factor + green;
            blue = (255 - blue) * factor + blue;
        }

        return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
    }
}
