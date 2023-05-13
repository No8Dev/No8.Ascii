namespace Asciis.UI.Controls;

public class GridComposer : Composer<Grid, GridLayout>
{
    public GridComposer(Grid grid) : base(grid)
    {
    }

    public void CalculateRowsAndCols(Rect area)
    {
        area = area - Layout.Edges;

        int remainingWidth = area.Width;
        int remainingHeight = area.Height;

        if (Control.RowSpacing > 0 && Control.Rows?.Count > 0)
            remainingHeight -= (Control.Rows.Count + 1) * (Control.RowSpacing ?? 0);
        if (Control.ColSpacing > 0 && Control.Cols?.Count > 0)
            remainingWidth -= (Control.Cols.Count + 1) * (Control.ColSpacing ?? 0);

        float totalPercent = 0f;
        int percentageCount = 0;
        if (Control.Rows?.Count > 0)
        {
            foreach (var row in Control.Rows)
            {
                int rowHeight = 0;
                switch (row.Height.Unit)
                {
                    case Number.UoM.Point:
                        rowHeight = (int)row.Height.Value;
                        break;
                    case Number.UoM.Auto:
                        totalPercent += 1;
                        percentageCount++;
                        break;
                    case Number.UoM.Percent:
                        totalPercent += row.Height.Value;
                        percentageCount++;
                        break;
                    default:
                        break;
                }

                remainingHeight -= rowHeight;
                Layout.Rows.Add(new GridLayout.Row(0, rowHeight));
            }
            if (totalPercent > 0f)
            {
                var variableHeight = remainingHeight;
                for (int i = 0; i < Control.Rows.Count; i++)
                {
                    var row = Control.Rows[i];
                    int rowHeight = 0;
                    switch (row.Height.Unit)
                    {
                        case Number.UoM.Point:
                            break;
                        case Number.UoM.Auto:
                        case Number.UoM.Percent:
                            {
                                if (percentageCount == 1)
                                {
                                    rowHeight = remainingHeight;
                                }
                                else
                                {
                                    float value = (float)row.Height.Value;
                                    if (row.Height.Unit == Number.UoM.Auto)
                                        value = 1f;
                                    var relative = value / totalPercent;
                                    rowHeight = (int)(variableHeight * relative);
                                    if (rowHeight == 0)
                                        rowHeight = 1;
                                }
                                Layout.Rows[i] = new GridLayout.Row(0, rowHeight);
                                percentageCount--;
                            }
                            break;
                        default:
                            break;
                    }

                    remainingHeight -= rowHeight;
                }
                if (remainingHeight > 0)
                    throw new System.Exception("Mistake with calculation");
            }

            var y = area.Y + (Control.RowSpacing ?? 0);
            for (int i = 0; i < Layout.Rows.Count; i++)
            {
                var row = Layout.Rows[i];
                Layout.Rows[i] = new GridLayout.Row(y, row.Height);
                y += row.Height + (Control.RowSpacing ?? 0);
            }
        }
        else
        {
            Layout.Rows.Add(new GridLayout.Row(area.Top + (Control.RowSpacing ?? 0), remainingHeight));
        }

        totalPercent = 0f;
        percentageCount = 0;
        if (Control.Cols?.Count > 0)
        {
            foreach (var col in Control.Cols)
            {
                int colWidth = 0;
                switch (col.Width.Unit)
                {
                    case Number.UoM.Point:
                        colWidth = (int)col.Width.Value;
                        break;
                    case Number.UoM.Auto:
                        totalPercent += 1;
                        percentageCount++;
                        break;
                    case Number.UoM.Percent:
                        totalPercent += col.Width.Value;
                        percentageCount++;
                        break;
                    default:
                        break;
                }

                remainingWidth -= colWidth;
                Layout.Cols.Add(new GridLayout.Col(0, colWidth));
            }
            if (totalPercent > 0f)
            {
                var variableWidth = remainingWidth;
                for (int i = 0; i < Control.Cols.Count; i++)
                {
                    var col = Control.Cols[i];
                    int colWidth = 0;
                    switch (col.Width.Unit)
                    {
                        case Number.UoM.Point:
                            break;
                        case Number.UoM.Auto:
                        case Number.UoM.Percent:
                            {
                                if (percentageCount == 1)
                                {
                                    colWidth = remainingWidth;
                                }
                                else
                                {
                                    float value = (float)col.Width.Value;
                                    if (col.Width.Unit == Number.UoM.Auto)
                                        value = 1;
                                    var relative = value / totalPercent;
                                    colWidth = (int)(variableWidth * relative);
                                    if (colWidth == 0)
                                        colWidth = 1;
                                }
                                Layout.Cols[i] = new GridLayout.Col(0, colWidth);
                                percentageCount--;
                            }
                            break;
                        default:
                            break;
                    }

                    remainingWidth -= colWidth;
                }
                if (remainingWidth > 0)
                    throw new System.Exception("Mistake with calculation");
            }

            var x = area.X + (Control.ColSpacing ?? 0);
            for (int i = 0; i < Layout.Cols.Count; i++)
            {
                var col = Layout.Cols[i];
                Layout.Cols[i] = new GridLayout.Col(x, col.Width);
                x += col.Width + (Control.ColSpacing ?? 0);
            }
        }
        else
        {
            Layout.Cols.Add(new GridLayout.Col(area.Left + (Control.ColSpacing ?? 0), remainingWidth));
        }

        BuildZones();
    }

    private void BuildZones()
    {
        for (int y = 0; y < Layout.Rows.Count; y++)
        {
            for (int x = 0; x < Layout.Cols.Count; x++)
            {
                var zoneDef = Control.Zones?.FirstOrDefault(z => z.Col == x && z.Row == y);

                var row = Layout.Rows[y];
                var col = Layout.Cols[x];

                if (zoneDef != null)
                {
                    int width = col.Width;
                    int height = row.Height;
                    if (zoneDef.ColSpan > 1)
                    {
                        for (int xx = x + 1; xx < x + zoneDef.ColSpan && xx < Layout.Cols.Count; xx++)
                            width += Layout.Cols[xx].Width + (Control.ColSpacing ?? 0);
                    }
                    if (zoneDef.RowSpan > 1)
                    {
                        for (int yy = y + 1; yy < y + zoneDef.RowSpan && yy < Layout.Rows.Count; yy++)
                            height += Layout.Rows[yy].Height + (Control.RowSpacing ?? 0);
                    }
                    var zone = new GridLayout.Zone(zoneDef.Name, col.X, row.Y, width, height);
                    Layout.Zones.Add(zone);
                    x += zoneDef.ColSpan - 1;
                }
                else
                {
                    zoneDef = FindZone(x, y);
                    if (zoneDef == null)
                    {
                        var name = string.IsNullOrWhiteSpace(Control.Name)
                            ? $"{x}-{y}"
                            : $"{Control.Name}-{x}-{y}";
                        var zone = new GridLayout.Zone(name, col.X, row.Y, col.Width, row.Height);
                        Layout.Zones.Add(zone);
                    }
                }
            }
        }
    }

    public override void DoMeasure(RectF area)
    {
        CalculateRowsAndCols(area);

        base.DoMeasure(area);
    }

    protected override (float maxWidth, float maxHeight) MeasureChildren(in RectF area)
    {
        foreach (var child in Control.Children)
        {
            var composer = CreateComposerForChild(child);

            var width = area.Width;
            var height = area.Height;
            var zone = Layout.FindZone(child.Name);
            if (zone != null)
            {
                width = zone.Width;
                height = zone.Height;
            }

            composer.DoMeasure(RectF.Create(0, 0, width, height));
            var childLayout = composer.Layout;

            if (childLayout == null) throw new NullReferenceException("Failed to calculate layout");

            float x = 0;
            float y = 0;

            switch (child.HorzPosition)
            {
                case LayoutPosition.Start:
                    x = 0;
                    break;
                case LayoutPosition.Center:
                    x = (width / 2) - (childLayout.Width / 2);
                    break;
                case LayoutPosition.End:
                    x = width - childLayout.Width;
                    break;
                case LayoutPosition.Stretch:
                    x = 0;
                    childLayout.Width = width;
                    break;
            }
            switch (child.VertPosition)
            {
                case LayoutPosition.Start:
                    y = 0;
                    break;
                case LayoutPosition.Center:
                    y = (height / 2) - (childLayout.Height / 2);
                    break;
                case LayoutPosition.End:
                    y = height - childLayout.Height;
                    break;
                case LayoutPosition.Stretch:
                    y = 0;
                    childLayout.Height = height;
                    break;
            }

            if (zone != null)
            {
                // offset from this 
                childLayout.X = zone.X + x;
                childLayout.Y = zone.Y + y;
            }
            else
            {
                // offset from this 
                childLayout.X = Layout.X + x;
                childLayout.Y = Layout.Y + y;
            }
        }

        return (area.Width, area.Height);
    }

    public override void DoArrange(RectF contentArea)
    {
        if (Layout.Width.IsUndefined())
            Layout.Width = contentArea.Width;
        if (Layout.Height.IsUndefined())
            Layout.Height = contentArea.Height;

        switch (Control.HorzPosition)
        {
            case LayoutPosition.Start:
                Layout.X = contentArea.X;
                break;
            case LayoutPosition.Center:
                Layout.X = contentArea.X + ((contentArea.Width / 2) - (Layout.Width / 2));
                break;
            case LayoutPosition.End:
                Layout.X = contentArea.X + (contentArea.Width - Layout.Width);
                break;
            case LayoutPosition.Stretch:
                Layout.X = contentArea.X;
                break;
        }
        switch (Control.VertPosition)
        {
            case LayoutPosition.Start:
                Layout.Y = contentArea.Y;
                break;
            case LayoutPosition.Center:
                Layout.Y = contentArea.Y + ((contentArea.Height / 2) - (Layout.Height / 2));
                break;
            case LayoutPosition.End:
                Layout.Y = contentArea.Y + (contentArea.Height - Layout.Height);
                break;
            case LayoutPosition.Stretch:
                Layout.Y = contentArea.Y;
                break;
        }

        ArrangeChildren();
    }

    protected override void ArrangeChildren()
    {
        foreach (var composer in ChildComposers)
        {
            var childControl = composer.Control;
            var childLayout = composer.Layout;
            var zone = Layout.FindZone(childControl.Name);
            if (zone != null)
            {
            }

            // For a Grid layout, the Child elements should already be arranged,
            // but their children will still need arranging
            foreach (var childrensChildComposer in composer.ChildComposers)
            {
                childrensChildComposer.DoArrange(childLayout.ContentArea);
            }
        }
    }

    private ZoneDef? FindZone(int col, int row)
    {
        return Control.Zones.FirstOrDefault(
            z =>
            col >= z.Col && col < z.Col + z.ColSpan &&
            row >= z.Row && row < z.Row + z.RowSpan);
    }

    public override void Draw(Canvas canvas, RectF? clip)
    {
        clip = (clip == null) ? Layout.Bounds : Rect.Intersect(clip, Layout.Bounds);
        canvas.PushClip(clip);


        if (Layout.LineSet != LineSet.None)
        {
            if (Layout.ColSpacing > 0 && Layout.RowSpacing > 0)
            {
                foreach (var zone in Layout.Zones)
                {
                    var left = Math.Max(zone.X - 1, 0);
                    var top = Math.Max(zone.Y - 1, 0);
                    var width = zone.Width + 2;
                    var height = zone.Height + 2;
                    var right = left + zone.Width + 2;
                    var bottom = top + zone.Height + 2;
                    if (right > Layout.Width)
                        width = zone.Width + 1;
                    if (bottom > Layout.Height)
                        height = zone.Height + 1;

                    var rect = Rect.Create(left, top, width, height);
                    canvas.DrawRect(rect, Layout.SpacingLineSet, Layout.BorderColor);
                }
            }
            else if (Layout.ColSpacing > 0)
            {
                foreach (var col in Layout.Cols)
                {
                    var left = Math.Max(Layout.Bounds.X + col.X - 1f, 0f);
                    var width = col.Width + 2f;
                    var right = left + col.Width + 2;
                    if (right > Layout.Width)
                        width = col.Width + 1;

                    var rect = RectF.Create(left, Layout.Bounds.Y, width, Layout.Bounds.Height);
                    canvas.DrawRect(rect, Layout.SpacingLineSet, Layout.BorderColor);
                }
            }
            else if (Layout.RowSpacing > 0)
            {
                foreach (var row in Layout.Rows)
                {
                    var top = Math.Max(row.Y - 1, 0);
                    var height = row.Height + 2;
                    var bottom = top + row.Height + 2;
                    if (bottom > Layout.Height)
                        height = row.Height + 1;

                    var rect = RectF.Create(Layout.Bounds.X, top, Layout.Bounds.Width, height);
                    canvas.DrawRect(rect, Layout.SpacingLineSet, Layout.BorderColor);
                }
            }

            canvas.DrawRect(Layout.Bounds, Layout.LineSet, Layout.BorderColor);
        }

        base.Draw(canvas, clip);
        canvas.PopClip();
    }
}
