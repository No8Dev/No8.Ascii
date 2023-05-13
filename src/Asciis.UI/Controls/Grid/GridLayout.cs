using System.Drawing;
using System.Text;

namespace Asciis.UI.Controls;

public class GridLayout : Layout<Grid>
{
    public const float DefaultFlexRate = 0f;

    public List<Row> Rows { get; } = new();
    public List<Col> Cols { get; } = new();
    public List<Zone> Zones { get; } = new();

    public int[] RowHeights()
        => Rows.Select(r => r.Height).ToArray();
    public int[] ColWidths()
        => Cols.Select(c => c.Width).ToArray();

    public Color? BorderColor { get; set; }
    public Edges Border { get; set; } = Edges.Zero;
    public LineSet LineSet { get; set; }
    public LineSet SpacingLineSet { get; set; }
    public int RowSpacing { get; set; }
    public int ColSpacing { get; set; }

    public GridLayout(Grid grid) : base(grid)
    {
    }

    public override void UpdateValues(Grid grid)
    {
        base.UpdateValues(grid);
        Border = grid.Border ?? Edges.Zero;
        BorderColor = grid.BorderColor;
        LineSet = grid.LineSet ?? grid.SpacingLineSet ?? LineSet.None;
        SpacingLineSet = grid.SpacingLineSet ?? LineSet.None;
        RowSpacing = grid.RowSpacing ?? 0;
        ColSpacing = grid.ColSpacing ?? 0;
    }

    public override Edges Edges =>
        Padding + Border;

    public record Col(int X, int Width);
    public record Row(int Y, int Height);
    public record Zone(string Name, int X, int Y, int Width, int Height);

    public Zone? FindZone(string? name)
    {
        return Zones
            .FirstOrDefault(
                z => z.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));
    }

    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        if (Rows.Count > 0)
            sb.AppendLine("Rows\n\t" + string.Join("\n\t", Rows));
        if (Cols.Count > 0)
            sb.AppendLine("Cols\n\t" + string.Join("\n\t", Cols));
        if (Zones.Count > 0)
            sb.AppendLine("Zones\n\t" + string.Join("\n\t", Zones));
    }
}
