using System.ComponentModel;
using No8.Ascii.ElementLayout;

namespace No8.Ascii.GridLayout;

public class GridLayout
{
    private RectF? _bounds;
    
    public bool HasLayout => _bounds != null;
    
    public bool HasOverflow { get; internal set; }
    
    /// <summary>
    ///     The location and size of the control.  Does not include any translations
    /// </summary>
    public RectF Bounds
    {
        get => _bounds ?? throw new WarningException("Layout not calculated");
        internal set => _bounds = value;
    }
    
    /// <summary>
    ///     The location and size of the contents of a control.  Does not include and translations
    /// </summary>
    public virtual RectF GetContentBounds() =>
        new(
            Bounds.X + Padding.Left,
            Bounds.Y + Padding.Top,
            Width - (Padding.Left + Padding.Right),
            Height - (Padding.Top + Padding.Bottom)
        );
    
    // The margin is outside the control
    public readonly Edges Margin  = new();
    public readonly Edges Padding = new();
    
    public float this[Side side]
    {
        get => Position[side];
        set => Position[side] = value;
    }

    public float Width  { get; internal set; } = float.NaN;
    public float Height { get; internal set; } = float.NaN;
    
    public int ColumnIndex { get; internal set; }
    public int RowIndex { get; internal set; }
    
    public float Left => Position.Left;
    public float Top => Position.Top;
    
    public float Right
    {
        get
        {
            if (Position.Right.IsZero() && Width.IsNotZero())
                Position.Right = Position.Left + Width - 1;
            return Position.Right;
        }
    }

    public float Bottom
    {
        get
        {
            if (Position.Bottom.IsZero() && Height.IsNotZero())
                Position.Bottom = Position.Top + Height - 1;
            return Position.Bottom;
        }
    }

    /***************************************************/
    
    public readonly Edges Position = Edges.Create(0);
}

