using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;

namespace Asciis.UI.Controls;

public class Grid : Control, IEnumerable<ZoneDef>
{
    static Grid()
    {
        Composer.Register(false, typeof(Grid), typeof(GridComposer));
    }

    private Edges? _border;
    private Color? _borderColor;
    private LineSet? _spacingLineSet;
    private LineSet? _lineSet;
    private GridRows? _rows;
    private GridCols? _cols;
    private int? _rowSpacing;
    private int? _colSpacing;

    public Edges? Border { get => _border; set => ChangeDirtiesLayout(ref _border, value); }
    public Color? BorderColor { get => _borderColor; set => ChangeDirtiesPainting(ref _borderColor, value); }
    public LineSet? LineSet { get => _lineSet; set => ChangeDirtiesPainting(ref _lineSet, value); }
    public LineSet? SpacingLineSet { get => _spacingLineSet; set => ChangeDirtiesPainting(ref _spacingLineSet, value); }
    public GridRows? Rows
    {
        get => _rows;
        set => ChangeDirtiesLayout(ref _rows, value);
    }
    public GridCols? Cols
    {
        get => _cols;
        set => ChangeDirtiesLayout(ref _cols, value);
    }
    public int? RowSpacing
    {
        get => _rowSpacing;
        set => ChangeDirtiesLayout(ref _rowSpacing, value);
    }
    public int? ColSpacing
    {
        get => _colSpacing;
        set => ChangeDirtiesLayout(ref _colSpacing, value);
    }

    private readonly List<ZoneDef> _zones = new();
    public ReadOnlyCollection<ZoneDef> Zones => _zones.AsReadOnly();

    public IEnumerable<ZoneDef> ZoneDefs
    {
        init
        {
            foreach (var item in value)
                Add(item);
        }
    }

    public Grid(GridRows? rows = null, GridCols? cols = null, string? name = null) : base(name)
    {
        Rows = rows ?? "*";
        Cols = cols ?? "*";
    }
    public Grid(out Grid grid, GridRows? rows = null, GridCols? cols = null, string? name = null) : base(name)
    {
        Rows = rows ?? "*";
        Cols = cols ?? "*";
        grid = this;
    }

    public record RowDef
        (
            Number Height,
            int Min = 0,
            int Max = 0
        )
    {
        public override string ToString()
        {
            var str = Height != 1.Percent() ? Height.ToString() : "*";
            if (Min != 0 || Max != 0)
                return $"{str} >{Min} <{Max}";
            return str;
        }
    }

    public record ColDef
        (
            Number Width,
            int Min = 0,
            int Max = 0
        )
    {
        public override string ToString()
        {
            var str = Width != 1.Percent() ? Width.ToString() : "*";
            if (Min != 0 || Max != 0)
                return $"{str} >{Min} <{Max}";
            return str;
        }
    }

    public class GridRows : List<RowDef>
    {
        public GridRows(List<Number> lists)
            : base(lists.Select(x => new RowDef(x)))
        { }

        public static implicit operator GridRows(string str)
            => new GridRows(GridHelper.Parse(str));
    }
    public class GridCols : List<ColDef>
    {
        public GridCols(List<Number> lists)
            : base(lists.Select(x => new ColDef(x)))
        {
        }

        public static implicit operator GridCols(string str)
            => new GridCols(GridHelper.Parse(str));
    }


    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        if (_border != null) sb.Append(" Border=" + _border);
        if (_borderColor != null) sb.Append(" BorderColor=" + _borderColor);
        if (_lineSet != null) sb.Append(" LineSet=" + _lineSet);
        if (_spacingLineSet != null) sb.Append(" SpacingLineSet=" + _spacingLineSet);
        if (_rows != null)
            sb.Append(" Rows=" + string.Join(',', _rows.Select(row => row.ToString())));
        if (_rowSpacing > 0)
            sb.Append(" RowSpacing=" + _rowSpacing);
        if (_cols != null)
            sb.Append(" Cols=" + string.Join(',', _cols.Select(col => col.ToString())));
        if (_colSpacing > 0)
            sb.Append(" ColSpacing=" + _colSpacing);

    }


    public void Add(ZoneDef zone)
    {
        if (string.IsNullOrWhiteSpace(zone.Name))
        {
            var name = string.IsNullOrWhiteSpace(Name)
                                ? $"{zone.Col}-{zone.Row}"
                                : $"{Name}-{zone.Col}-{zone.Row}";
            zone = zone with { Name = name };
        }
        _zones.Add(zone);
        SetNeedsLayout();
    }

    public new IEnumerator<ZoneDef> GetEnumerator() =>
        Zones.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();
}

public record ZoneDef
(
    [NotNull] string Name,
    int Col = 0,
    int Row = 0,
    int ColSpan = 1,
    int RowSpan = 1);

internal static class GridHelper
{
    internal static List<Number> Parse(string str)
    {
        var result = new List<Number>();
        if (string.IsNullOrWhiteSpace(str))
            return result;

        if (str.Length != 0)
        {
            var tokens = str.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var token in tokens)
            {
                try
                {
                    var num = Number.Parse(token);
                    result.Add(num);
                }
                catch (FormatException ex)
                {
                    throw new FormatException($"Unable to parse input string {str}", ex);
                }
            }
        }

        return result;
    }
}
