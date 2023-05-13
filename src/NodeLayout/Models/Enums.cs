using System;

namespace NodeLayout;

public enum Dimension
{
    Width,
    Height
}

public enum UIDirection
{
    Unknown = -1,

    /// <summary>Inherit from container</summary>
    Inherit = 0,

    /// <summary>Left to right</summary>
    LTR = 1,

    /// <summary>Right to left</summary>
    RTL = 2
}

public enum LayoutDirection
{
    Column,
    ColumnReverse,
    Row,
    RowReverse
}

public enum LayoutWrap
{
    NoWrap,
    Wrap,
    WrapReverse
}

/// <summary>
///     Align elements for Main-Direction
/// </summary>
public enum AlignmentMain
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
public enum AlignmentCross
{
    Unknown = 0,
    Auto = 1,
    Start = 2,
    Center = 3,
    End = 4,
    Stretch = 5,
    Baseline = 6,
    SpaceBetween = 7,
    SpaceAround = 8,
    SpaceEvenly = 9
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

[Flags]
public enum PrintOptions
{
    Layout = 1,
    Plan = 2,
    Elements = 4,
    All = Elements | Plan | Layout
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
