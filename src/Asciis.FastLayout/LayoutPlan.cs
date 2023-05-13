using System.Runtime.InteropServices;

namespace Asciis.FastLayout;

public class LayoutPlan
{
    public const float DefaultGrowFactor = 0.0f;
    public const float DefaultShrinkFactor = 1.0f;

    //-- Position or Alignment, but not both
    public Sides        Position     { get; set; } = new();
    public PositionType PositionType { get; set; } = PositionType.Relative;
    //-- 
    public Align? HorzAlign      { get; set; }
    public Align? VertAlign      { get; set; }
    public Align? DirectionAlign => ContentsPlan.ToDirection() == LayoutDirection.Row ? HorzAlign : VertAlign;

    //-- Size
    public Number? Width       { get; private set; }
    public Number? Height      { get; private set; }
    public Number? MinWidth    { get; private set; }
    public Number? MaxWidth    { get; private set; }
    public Number? MinHeight   { get; private set; }
    public Number? MaxHeight   { get; private set; }
    public float?  AspectRatio { get; set; }

    public Sides?  Padding   { get; private set; }


    #region Flexible Child

    /// <summary>
    ///     Flex factor for child controls inside a Flex DirectionLayout. 
    ///     When child controls total size is less that parent size
    ///     Default value is 0.0
    /// </summary>
    public float GrowFactor { get; private set; } = DefaultGrowFactor;

    /// <summary>
    ///     Flex factor for child controls inside a Flex DirectionLayout.
    ///     When child controls total size exceed parent size
    ///     Default value is 1.0
    /// </summary>
    public float ShrinkFactor { get; private set; } = DefaultShrinkFactor;
    #endregion


    #region Contents layout

    public ContentsPlan ContentsPlan { get; private set; } = ContentsPlan.FreeForm;

    /// <summary>
    ///     Horizontal or Vertical layout
    /// </summary>
    internal DirectionLayout ContentsLayout { get; set; }

    /// <summary>
    ///     Vertical or Horizontal alignment
    ///     If ContentsPlan is Vertical, Align applies to Horizontal alignment
    ///     If ContentsPlan is Horizontal, Align applies to Vertical alignment
    /// </summary>
    internal Align Align { get; set; } = Align.Start;

    /// <summary>
    ///     Space around Horizontal or Vertical contents
    /// </summary>
    internal Space Space { get; set; } = Space.None;

    #endregion



    //-- Position facades

    public int X
    {
        get => (int)Position.ComputedEdgeValue(Side.Start).Value;
        set => Position.Start = value;
    }

    public int Y
    {
        get => (int)Position.ComputedEdgeValue(Side.Top).Value;
        set => Position.Top = value;
    }

    //********************************************************************

    /// <summary>
    ///     Get the Planned width or height
    /// </summary>
    public Number? Dimension(Dimension dim)
    {
        switch (dim)
        {
        case FastLayout.Dimension.Width:
            return Width;
        case FastLayout.Dimension.Height:
            return Height;
        default:
            throw new ArgumentException("Unknown dimension", nameof(dim));
        }
    }

    /// <summary>
    ///     Get the minimum Planned width or height
    /// </summary>
    public Number? MinDimension(Dimension dim)
    {
        switch (dim)
        {
        case FastLayout.Dimension.Width:
            return MinWidth;
        case FastLayout.Dimension.Height:
            return MinHeight;
        default:
            throw new ArgumentException("Unknown dimension", nameof(dim));
        }
    }

    /// <summary>
    ///     Get the maximum Planned width or height
    /// </summary>
    public Number? MaxDimension(Dimension dim)
    {
        switch (dim)
        {
        case FastLayout.Dimension.Width:
            return MaxWidth;
        case FastLayout.Dimension.Height:
            return MaxHeight;
        default:
            throw new ArgumentException("Unknown dimension", nameof(dim));
        }
    }

    //********************************************************************

    public LayoutPlan PositionNode(int x, int y)
    {
        Position  = new(x, y);
        HorzAlign = null;
        VertAlign = null;

        return this;
    }

    public LayoutPlan SetAlign(
        Align? horzAlign = null,
        Align? vertAlign = null)
    {
        HorzAlign = horzAlign;
        VertAlign = vertAlign;
        Position = new();
        return this;
    }

    public LayoutPlan SetSize(
        Number? width     = null,
        Number? height    = null,
        Number? minWidth  = null,
        Number? maxWidth  = null,
        Number? minHeight = null,
        Number? maxHeight = null)
    {
        Width     = width;
        Height    = height;
        MinWidth  = minWidth;
        MaxWidth  = maxWidth;
        MinHeight = minHeight;
        MaxHeight = maxHeight;
        return this;
    }

    public LayoutPlan SetPadding(Sides padding)
    {
        Padding = padding;
        return this;
    }

    public LayoutPlan SetFlexible(
        float growFactor = 0f,
        float shrinkFactor = 1f)
    {
        GrowFactor   = growFactor;
        ShrinkFactor = shrinkFactor;

        return this;
    }


    //--- Properties about the contents

    /// <summary>
    ///     Contents are not stacked.  Useful for single content node or absolute positioning.
    /// </summary>
    public LayoutPlan WithContentsFreeForm()
    {
        ContentsPlan = ContentsPlan.FreeForm;
        return this;
    }

    /// <summary>
    ///     All Contents are stacked horizontally
    /// </summary>
    public LayoutPlan WithContentsHorizontal(
        DirectionLayout horzLayout,
        Align vertAlign,
        Space space = Space.None)
    {
        ContentsPlan   = ContentsPlan.Horizontal;
        ContentsLayout = horzLayout;
        Align          = vertAlign;
        Space          = space;
        return this;
    }

    /// <summary>
    ///     All contents are stacked Vertically
    /// </summary>
    public LayoutPlan WithContentsVertical(
        Align horzAlign,
        DirectionLayout vertLayout,
        Space space = Space.None)
    {
        ContentsPlan   = ContentsPlan.Vertical;
        ContentsLayout = vertLayout;
        Align          = horzAlign;
        Space          = space;
        return this;
    }
}