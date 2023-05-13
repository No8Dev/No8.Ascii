using System.Drawing;

namespace Asciis.UI.Controls;

public class CheckboxStyle : Style
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
    public bool? Checked
    {
        get => Get<bool?>(nameof(Checked));
        set => Set(nameof(Checked), value);
    }
    public string? CheckedString
    {
        get => Get<string?>(nameof(CheckedString));
        set => Set(nameof(CheckedString), value);
    }
    public string? UncheckedString
    {
        get => Get<string?>(nameof(UncheckedString));
        set => Set(nameof(UncheckedString), value);
    }
    public string? UnknownString
    {
        get => Get<string?>(nameof(UnknownString));
        set => Set(nameof(UnknownString), value);
    }
}
