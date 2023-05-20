using System.Diagnostics;
using No8.Ascii.ElementLayout;

namespace No8.Ascii.Controls;

public class ControlPlan
{
    public LayoutPlan LayoutPlan { get; }

    internal ControlPlan(LayoutPlan? plan = null)
    {
        LayoutPlan = plan ?? new LayoutPlan();
    }

    //********************************************************************

    /// <summary>
    ///     Element Margin
    /// </summary>
    public Sides Margin
    {
        get => LayoutPlan.Margin;
        set => LayoutPlan.Margin = value;
    }

    /// <summary>
    ///     Element Padding
    /// </summary>
    public Sides Padding
    {
        get => LayoutPlan.Padding;
        set => LayoutPlan.Padding = value;
    }

    /// <summary>
    ///     Position is set to either Relative, or absolute
    /// </summary>
    /// <remarks>Default value is: Relative</remarks>
    public PositionType PositionType
    {
        get => LayoutPlan.PositionType;
        set => LayoutPlan.PositionType = value;
    }

    /// <summary>
    ///     Element Position
    /// </summary>
    public Sides Position
    {
        get => LayoutPlan.Position;
        set => LayoutPlan.Position = value;
    }

    //********************************************************************

    /// <summary>
    ///     Planned Left
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Left
    {
        get => Position[Side.Left];
        set => Position[Side.Left] = value;
    }

    /// <summary>
    ///     Planned Top
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Top
    {
        get => Position[Side.Top];
        set => Position[Side.Top] = value;
    }

    /// <summary>
    ///     Planned Right
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Right
    {
        get => Position[Side.Right];
        set => Position[Side.Right] = value;
    }

    /// <summary>
    ///     Planned Bottom
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Bottom
    {
        get => Position[Side.Bottom];
        set => Position[Side.Bottom] = value;
    }

    /// <summary>
    ///     Planned Start
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Start
    {
        get => Position[Side.Start];
        set => Position[Side.Start] = value;
    }

    /// <summary>
    ///     Planned End
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number End
    {
        get => Position[Side.End];
        set => Position[Side.End] = value;
    }

    /* Why? I don't know
    /// <summary>
    ///     Planned Horizontal
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Horizontal
    {
        get => Position[Side.Horizontal];
        set => Position[Side.Horizontal] = value;
    }

    /// <summary>
    ///     Planned Vertical
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Number Vertical
    {
        get => Position[Side.Vertical];
        set => Position[Side.Vertical] = value;
    }
    */

    //********************************************************************
        
    /// <summary>
    ///     Requested Width
    /// </summary>
    /// <remarks>Default value is: auto</remarks>
    public Number Width
    {
        get => LayoutPlan.Width;
        set => LayoutPlan.Width = value;
    }

    /// <summary>
    ///     Requested Height
    /// </summary>
    /// <remarks>Default value is: auto</remarks>
    public Number Height
    {
        get => LayoutPlan.Height;
        set => LayoutPlan.Height = value;
    }
        
    /// <summary>
    ///     Minimum Width for node
    /// </summary>
    public Number MinWidth
    {
        get => LayoutPlan.MinWidth;
        set => LayoutPlan.MinWidth = value;
    }

    /// <summary>
    ///     Minimum Height for node
    /// </summary>
    public Number MinHeight
    {
        get => LayoutPlan.MinHeight;
        set => LayoutPlan.MinHeight = value;
    }

    /// <summary>
    ///     Maximum Width for node
    /// </summary>
    public Number MaxWidth
    {
        get => LayoutPlan.MaxWidth;
        set => LayoutPlan.MaxWidth = value;
    }

    /// <summary>
    ///     Maximum Height for node
    /// </summary>
    public Number MaxHeight
    {
        get => LayoutPlan.MaxHeight;
        set => LayoutPlan.MaxHeight = value;
    }

    /// <summary>
    ///     Aspect ratio for node
    /// </summary>
    public float AspectRatio
    {
        get => LayoutPlan.AspectRatio;
        set => LayoutPlan.AspectRatio = value;
    }

    //********************************************************************

    protected ControlAlign ChildrenHorzAlign
    {
        get
        {
            if (LayoutPlan.ElementsDirection == LayoutDirection.Horz)
                return LayoutPlan.AlignElements_LayoutDirection.ToControlAlign();
            return LayoutPlan.AlignElements_Cross.ToControlAlign();
        }
        set
        {
            if (LayoutPlan.ElementsDirection == LayoutDirection.Horz)
                LayoutPlan.AlignElements_LayoutDirection = value.ToAlignmentElements_LayoutDirection();
            else
            {
                LayoutPlan.AlignElements_Cross = value.ToAlignment_Cross();
                LayoutPlan.Align_Cross = value.ToAlignmentLine_Cross();
            }
        }
    }
    protected ControlAlign ChildrenVertAlign
    {
        get
        {
            if (LayoutPlan.ElementsDirection == LayoutDirection.Vert)
                return LayoutPlan.AlignElements_LayoutDirection.ToControlAlign();
            return LayoutPlan.AlignElements_Cross.ToControlAlign();
        }
        set
        {
            if (LayoutPlan.ElementsDirection == LayoutDirection.Vert)
                LayoutPlan.AlignElements_LayoutDirection = value.ToAlignmentElements_LayoutDirection();
            else
            {
                LayoutPlan.AlignElements_Cross = value.ToAlignment_Cross();
                LayoutPlan.Align_Cross = value.ToAlignmentLine_Cross();
            }
        }
    }
}

public enum ControlAlign
{
    Unknown = 0,
    Auto = 1,
    Start = 2,
    Center = 3,
    End = 4,
    Stretch = 5,
}

public static class ControlAlignHelpers
{
    public static ControlAlign ToControlAlign(this AlignmentElements_LayoutDirection other) =>
        other switch
        {
            AlignmentElements_LayoutDirection.Auto => ControlAlign.Auto,
            AlignmentElements_LayoutDirection.Start => ControlAlign.Start,
            AlignmentElements_LayoutDirection.Center => ControlAlign.Center,
            AlignmentElements_LayoutDirection.End => ControlAlign.End,
            AlignmentElements_LayoutDirection.Stretch => ControlAlign.Stretch,
            _ => ControlAlign.Start
        };

    public static ControlAlign ToControlAlign(this Alignment_Cross other) =>
        other switch
        {
            Alignment_Cross.Auto => ControlAlign.Auto,
            Alignment_Cross.Start => ControlAlign.Start,
            Alignment_Cross.Center => ControlAlign.Center,
            Alignment_Cross.End => ControlAlign.End,
            Alignment_Cross.Stretch => ControlAlign.Stretch,
            _ => ControlAlign.Start
        };
    
    public static ControlAlign ToControlAlign(this AlignmentLine_Cross other) =>
        other switch
        {
            AlignmentLine_Cross.Auto => ControlAlign.Auto,
            AlignmentLine_Cross.Start => ControlAlign.Start,
            AlignmentLine_Cross.Center => ControlAlign.Center,
            AlignmentLine_Cross.End => ControlAlign.End,
            AlignmentLine_Cross.Stretch => ControlAlign.Stretch,
            _ => ControlAlign.Start
        };

    public static AlignmentElements_LayoutDirection ToAlignmentElements_LayoutDirection(this ControlAlign other) =>
        other switch
        {
            ControlAlign.Auto => AlignmentElements_LayoutDirection.Auto,
            ControlAlign.Start => AlignmentElements_LayoutDirection.Start,
            ControlAlign.Center => AlignmentElements_LayoutDirection.Center,
            ControlAlign.End => AlignmentElements_LayoutDirection.End,
            ControlAlign.Stretch => AlignmentElements_LayoutDirection.Stretch,
            _ => AlignmentElements_LayoutDirection.Start
        };

    public static Alignment_Cross ToAlignment_Cross(this ControlAlign other) =>
        other switch
        {
            ControlAlign.Auto => Alignment_Cross.Auto,
            ControlAlign.Start => Alignment_Cross.Start,
            ControlAlign.Center => Alignment_Cross.Center,
            ControlAlign.End => Alignment_Cross.End,
            ControlAlign.Stretch => Alignment_Cross.Stretch,
            _ => Alignment_Cross.Start
        };
    
    public static AlignmentLine_Cross ToAlignmentLine_Cross(this ControlAlign other) =>
        other switch
        {
            ControlAlign.Auto => AlignmentLine_Cross.Auto,
            ControlAlign.Start => AlignmentLine_Cross.Start,
            ControlAlign.Center => AlignmentLine_Cross.Center,
            ControlAlign.End => AlignmentLine_Cross.End,
            ControlAlign.Stretch => AlignmentLine_Cross.Stretch,
            _ => AlignmentLine_Cross.Start
        };
}