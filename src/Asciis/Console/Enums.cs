namespace No8.Ascii.Console;

/// <summary>
///     Align elements for Layout-Direction
/// </summary>
public enum AlignmentElements_LayoutDirection
{
    Unknown = 0,
    Auto = 1,
    Start = 2,
    Center = 3,
    End = 4,
    Stretch = 5,
    SpaceBetween = 7,
    SpaceAround = 8,
    SpaceEvenly = 9
}

/// <summary>
///     Align elements for Cross-Direction
/// </summary>
public enum AlignmentLine_Cross
{
    Unknown = 0,
    Auto = 1,
    Start = 2,
    Center = 3,
    End = 4,
    Stretch = 5,
}

/// <summary>
///     Align content for Cross-Direction
/// </summary>
public enum Alignment_Cross
{
    Unknown = 0,
    Auto = 1,
    Start = 2,
    Center = 3,
    End = 4,
    Stretch = 5,
    SpaceBetween = 7,
    SpaceAround = 8,
    SpaceEvenly = 9
}

public enum Dimension
{
    Width,
    Height
}

public enum LayoutDirection
{
    Vert,   // Vertical
    Horz,   // Horizontal
}

public enum LayoutPosition
{
    Start,
    Center,
    End,
    Stretch
}

public enum LayoutWrap
{
    NoWrap,
    Wrap
}

/// <summary>
///     Used in custom measurement to indicate the measurement mode.
/// </summary>
public enum MeasureMode
{
    Undefined = -1,
    Exactly = 0,
    AtMost = 1
}

/// <summary>
///     What to do the the case of the content being bigger than the container
/// </summary>
public enum Overflow
{
    Visible,
    Hidden,
    Scroll
}

/// <summary>
///     Absolute or relative positioning of elements
/// </summary>
public enum PositionType
{
    Relative,
    Absolute
}

public enum Side
{
    Left,
    Top,
    Right,
    Bottom,
    Start,
    End,
    Horizontal,
    Vertical,
    All
}

public enum Wrap
{
    None,
    WordWrap,
    Truncate,
    TruncateWithWord
}

