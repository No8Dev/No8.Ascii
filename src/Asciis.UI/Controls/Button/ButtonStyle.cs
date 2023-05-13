using System.Drawing;

namespace Asciis.UI.Controls;

public class ButtonStyle : Style
{
    public Edges? Border
    {
        get => Get<Edges?>(nameof(Border));
        set => Set(nameof(Border), value);
    }
    public Color? BorderColor
    {
        get => Get<Color?>(nameof(BorderColor));
        set => Set(nameof(BorderColor), value);
    }
    public LineSet? LineSet
    {
        get => Get<LineSet?>(nameof(LineSet));
        set => Set(nameof(LineSet), value);
    }
}
