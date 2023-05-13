namespace Asciis.FastLayout;

public class RenderLayout
{
    public Vec   Pos             { get; internal set; } = new(0,0);
    public Vec?  DisplayLocation { get; internal set; }

    public Sides Position        { get; internal set; } = new();

    public Vec   Size            { get; internal set; } = new(0,0);
    
    public int     Width          => Size.X;
    public int     Height         => Size.Y;

    public Sides Padding { get; internal set; } = new();

    public Rect Bounds => new(Pos, Size);

    public Rect ContentRect =>
        new(
            Pos.X + (int)(Padding.Start.Value),
            Pos.Y + (int)(Padding.Top.Value),
            Size.X - (int)(Padding.Start.Value + Padding.End.Value),
            Size.Y - (int)(Padding.Top.Value + Padding.Bottom.Value)
        );


    public bool  HadOverflow        { get; internal set; }
    public int   LineIndex          { get; internal set; }


    // INTERNAL VARIABLES **************************************

    // Instead of recomputing the entire layout every single time, we cache some
    // information to break early when nothing changed
    internal int     GenerationCount              { get; set; }
    internal Number? ResolvedWidth                { get; set; }
    internal Number? ResolvedHeight               { get; set; }
    internal float   MeasuredWidth                { get; set; }
    internal float   MeasuredHeight               { get; set; }
    internal int     ComputedMainLengthGeneration { get; set; }
    internal float   ComputedMainLength           { get; set; } = float.NaN;

    internal Number? GetResolvedDimension(Dimension dimension)
    {
        switch (dimension)
        {
        case Dimension.Width:  return ResolvedWidth;
        case Dimension.Height: return ResolvedHeight;
        default:
            return Number.Undefined;
        }
    }

    public float GetMeasuredDimension(Dimension dim)
    {
        switch (dim)
        {
        case Dimension.Width:  return MeasuredWidth;
        case Dimension.Height: return MeasuredHeight;
        default:               throw new ArgumentOutOfRangeException(nameof(dim), dim, null);
        }
    }

    public void SetMeasuredDimension(Dimension dim, float value)
    {
        switch (dim)
        {
        case Dimension.Width:  
            MeasuredWidth = value;
            break;
        case Dimension.Height:
            MeasuredHeight = value;
            break;
        default:               throw new ArgumentOutOfRangeException(nameof(dim), dim, null);
        }
    }
}

