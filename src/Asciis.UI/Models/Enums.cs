namespace Asciis.UI;

/// <summary>
///     Align elements for Main-Direction
/// </summary>
public enum AlignmentMain
{
    Unknown      = 0,
    Auto         = 1,
    Start        = 2,
    Center       = 3,
    End          = 4,
    Stretch      = 5,
    SpaceBetween = 7,
    SpaceAround  = 8,
    SpaceEvenly  = 9
}

/// <summary>
///     Align elements for Cross-Direction
/// </summary>
public enum AlignmentCross
{
    Unknown      = 0,
    Auto         = 1,
    Start        = 2,
    Center       = 3,
    End          = 4,
    Stretch      = 5,
    Baseline     = 6,
    SpaceBetween = 7,
    SpaceAround  = 8,
    SpaceEvenly  = 9
}

public enum Dimension
{
    Width,
    Height
}

public enum LayoutDirection
{
    Column,
    ColumnReverse,
    Row,
    RowReverse
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
    Wrap,
    WrapReverse
}

/// <summary>
///     Used in custom measurement to indicate the measurement mode.
/// </summary>
public enum MeasureMode
{
    Undefined = -1,
    Exactly   = 0,
    AtMost    = 1
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

public enum UIDirection
{
    Unknown = -1,

    /// <summary>Inherit from container</summary>
    Inherit = 0,

    /// <summary>Left to right</summary>
    Ltr = 1,

    /// <summary>Right to left</summary>
    Rtl = 2
}

public enum Wrap
{
    None,
    WordWrap,
    Truncate,
    TruncateWithWord
}

