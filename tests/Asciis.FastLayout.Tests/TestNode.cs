using System;

namespace Asciis.FastLayout.Tests;

public class TestNode : INode
{
    public Number? Width { get; set; }
    public Number? Height { get; set; }
    public Number? MinWidth { get; set; }
    public Number? MaxWidth { get; set; }
    public Number? MinHeight { get; set; }
    public Number? MaxHeight { get; set; }
    public Sides Padding { get; set; } = new();

    public DirectionLayout? HorzLayout { get; set; }
    public DirectionLayout? VertLayout { get; set; }
    public Align? HorzAlign { get; set; }
    public Align? VertAlign { get; set; }

    public Space Space { get; set; }
    public float GrowFactor { get; set; }
    public float ShrinkFactor { get; set; }


    public INode?        Parent { get; set; }
    public LayoutPlan?   Plan   { get; set; }
    public RenderLayout? Layout { get; set; }

    public static INode CreateBlock(
        Number? width        = null,
        Number? height       = null,
        Number? minWidth     = null,
        Number? maxWidth     = null,
        Number? minHeight    = null,
        Number? maxHeight    = null,
        Align?  horzAlign    = null,
        Align?  vertAlign    = null,
        Sides?  padding      = null,
        float   growFactor   = 0f,
        float   shrinkFactor = 1f
    )
    {
        return new TestNode
               {
                   ContentsLayout = ContentsPlan.Block,
                   HorzAlign      = horzAlign,
                   VertAlign      = vertAlign,
                   Width          = width,
                   Height         = height,
                   MinWidth       = minWidth,
                   MaxWidth       = maxWidth,
                   MinHeight      = minHeight,
                   MaxHeight      = maxHeight,
                   Padding        = padding ?? Sides.Zero,
                   GrowFactor     = growFactor,
                   ShrinkFactor   = shrinkFactor
               };
    }

    public static INode CreateAbsolute(
        int     x,
        int     y,
        Number? width        = null,
        Number? height       = null,
        Number? minWidth     = null,
        Number? maxWidth     = null,
        Number? minHeight    = null,
        Number? maxHeight    = null,
        Sides?  padding      = null,
        float   growFactor   = 0f,
        float   shrinkFactor = 1f
        )
    {
        return new TestNode
        {
            ContentsLayout = ContentsPlan.Absolute,
            X              = x,
            Y              = y,
            Width          = width,
            Height         = height,
            MinWidth       = minWidth,
            MaxWidth       = maxWidth,
            MinHeight      = minHeight,
            MaxHeight      = maxHeight,
            Padding        = padding ?? Sides.Zero,
            GrowFactor     = growFactor,
            ShrinkFactor   = shrinkFactor
        };
    }

    public static INode CreateSingle(
        Align   horzAlign,
        Align   vertAlign,
        Number? width        = null,
        Number? height       = null,
        Number? minWidth     = null,
        Number? maxWidth     = null,
        Number? minHeight    = null,
        Number? maxHeight    = null,
        Sides?  padding      = null,
        float   growFactor   = 0f,
        float   shrinkFactor = 1f
    )
    {
        return new TestNode
        {
            ContentsLayout = ContentsPlan.Single,
            HorzAlign = horzAlign,
            VertAlign = vertAlign,
            Width = width,
            Height = height,
            MinWidth = minWidth,
            MaxWidth = maxWidth,
            MinHeight = minHeight,
            MaxHeight = maxHeight,
            Padding = padding ?? Sides.Zero,
            GrowFactor = growFactor,
            ShrinkFactor = shrinkFactor
        };
    }

    public static INode CreateHorz(
        DirectionLayout horzLayout,
        Align vertAlign,
        Number? width = null,
        Number? height = null,
        Number? minWidth = null,
        Number? maxWidth = null,
        Number? minHeight = null,
        Number? maxHeight = null,
        Sides? padding = null,
        Space space = Space.None,
        float growFactor = 0f,
        float shrinkFactor = 1f
    )
    {
        return new TestNode
        {
            ContentsLayout = ContentsPlan.Single,
            HorzLayout = horzLayout,
            VertAlign = vertAlign,
            Width = width,
            Height = height,
            MinWidth = minWidth,
            MaxWidth = maxWidth,
            MinHeight = minHeight,
            MaxHeight = maxHeight,
            Padding = padding ?? Sides.Zero,
            Space = space,
            GrowFactor = growFactor,
            ShrinkFactor = shrinkFactor
        };
    }

    public static INode CreateVert(
        Align horzAlign,
        DirectionLayout vertLayout,
        Number? width = null,
        Number? height = null,
        Number? minWidth = null,
        Number? maxWidth = null,
        Number? minHeight = null,
        Number? maxHeight = null,
        Sides? padding = null,
        Space space = Space.None,
        float growFactor = 0f,
        float shrinkFactor = 1f
    )
    {
        return new TestNode
        {
            ContentsLayout = ContentsPlan.Single,
            HorzAlign = horzAlign,
            VertLayout = vertLayout,
            Width = width,
            Height = height,
            MinWidth = minWidth,
            MaxWidth = maxWidth,
            MinHeight = minHeight,
            MaxHeight = maxHeight,
            Padding = padding ?? Sides.Zero,
            Space = space,
            GrowFactor = growFactor,
            ShrinkFactor = shrinkFactor
        };
    }

    public Vec DoLayout(float containerWidth = Number.ValueUndefined, float containerHeight = Number.ValueUndefined)
    {
        switch (ContentsPlan)
        {
        case ContentsPlan.Block:      break;
        case ContentsPlan.Absolute:   break;
        case ContentsPlan.Single:     break;
        case ContentsPlan.Horizontal: break;
        case ContentsPlan.Vertical:   break;
        default:                        throw new ArgumentOutOfRangeException();
        }

        return Vec.Zero;
    }

}
