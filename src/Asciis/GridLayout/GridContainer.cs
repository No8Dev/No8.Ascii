namespace No8.Ascii.GridLayout;

/// <summary>
/// 
/// </summary>
public class GridContainer
{
    public readonly List<GridTrackDefinition> RowDefinitions = new();
    public readonly List<GridTrackDefinition> ColDefinitions = new();
    public GridTrackDefinition Auto { get; set; }

    public readonly List<GridArea> Areas = new();
    
    public int ColGap { get; set; }
    public int RowGap { get; set; }
    
    public GridItemAlign ItemHorzAlign { get; set; }
    public GridItemAlign ItemVertAlign { get; set; }
    public GridAlign     GridHorzAlign { get; set; }
    public GridAlign     GridVertAlign { get; set; }
    
    public GridFlow GridFlow { get; set; }

    public List<GridItem> Items { get; } = new();
}

public class GridTrackDefinition
{
    public string Name { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
    public Number Size { get; set; }
}

public class GridArea
{
    public string Name { get; set; }
    public Sides Location { get; set; }
}


public class GridItem
{
    // Location can be Requested
    // - Area name
    // - Line Index (0 ...)  Left, Top, Right, Bottom
    // - Track Index (0 ...) Left, Top, Right, Bottom
    // - Track Name Left, Top, Right, Bottom
    // Left and Top are inclusive
    // Right and Bottom are exclusive
    public string? Area { get; set; }
    public LTRB<int>? LineIndexLocation { get; set; }
    public LTRB<int>? TrackIndexLocation { get; set; }
    public LTRB<string>? TrackNameLocation { get; set; }
   
    public GridItemAlign SelfHorzAlign { get; set; }
    public GridItemAlign SelfVertAlign { get; set; }
}

/// <summary>
///     Horizontal justification or Vertical Alignment
///     For an item within a cell
/// </summary>
/// Start:         [1    ]
/// Centre:        [  1  ]
/// End:           [    1]
/// Stretch:       [11111]
public enum GridItemAlign
{
    Unknown,
    Start,
    Center,
    End,
    Stretch
}

/// Start:         [1 2 3          ]
/// Centre:        [     1 2 3     ]
/// End:           [          1 2 3]
/// Stretch:       [111112222233333]
/// Space Between: [1      2      3]
/// Space Around:  [  1    2    3  ]
/// Space Evenly:  [   1   2   3   ]
public enum GridAlign
{
    Unknown,
    Start,
    Center,
    End,
    Stretch,
    SpaceAround,
    SpaceBetween,
    SpaceEvenly
}

public enum GridFlow
{
    Unknown, 
    Horz,       // Fill horizontal before filling next row 
    Vert,       // Fill vertical before filling next col
    Dense       // Fill and available free gaps
}
