using System.Drawing;

namespace Asciis.UI.Controls;

public class GridStyle : Style
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
    public LineSet? SpacingLineSet
    {
        get => Get<LineSet?>(nameof(SpacingLineSet));
        set => Set(nameof(SpacingLineSet), value);
    }
    public int? RowSpacing
    {
        get => Get<int?>(nameof(RowSpacing));
        set => Set(nameof(RowSpacing), value);
    }
    public int? ColSpacing
    {
        get => Get<int?>(nameof(ColSpacing));
        set => Set(nameof(ColSpacing), value);
    }
}
