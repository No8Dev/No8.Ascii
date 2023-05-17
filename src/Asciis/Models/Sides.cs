using No8.Ascii.ElementLayout;

namespace No8.Ascii;

public class Sides
{
    public static Sides Zero =>
        new Sides(Number.Zero);

    public static Sides Create(Number value) =>
        new Sides(value);

    public static Sides Create(Number? left       = null,
                                Number? top        = null,
                                Number? right      = null,
                                Number? bottom     = null,
                                Number? start      = null,
                                Number? end        = null,
                                Number? horizontal = null,
                                Number? vertical   = null,
                                Number? all        = null) =>
        new Sides(left, top, right, bottom, start, end, horizontal, vertical, all);

    public event EventHandler? Changed;
    private bool               _isDirty;
    private Number             _all    = Number.Undefined;
    private Number             _bottom = Number.Undefined;
    private Number             _end    = Number.Undefined;
    private Number             _left   = Number.Undefined;
    private Number             _right  = Number.Undefined;
    private Number             _start  = Number.Undefined;
    private Number             _top    = Number.Undefined;

    /// <summary>
    /// All sides have the same value
    /// </summary>
    public Sides(Number value) { _left = _top = _right = _bottom = _start = End = All = value; }

    public Sides(Number? left = null,
                  Number? top = null,
                  Number? right = null,
                  Number? bottom = null,
                  Number? start = null,
                  Number? end = null,
                  Number? horizontal = null,
                  Number? vertical = null,
                  Number? all = null)
    {
        if (all != null) _all = all;
        if (horizontal != null) _left = _right = horizontal;
        if (vertical != null) _top = _bottom = vertical;
        if (start != null) _start = start;
        if (end != null) _end = end;
        if (left != null) _left = left;
        if (top != null) _top = top;
        if (right != null) _right = right;
        if (bottom != null) _bottom = bottom;
    }

    public Sides(Sides other)
    {
        _all = other.All;
        _left = other.Left;
        _top = other.Top;
        _right = other.Right;
        _bottom = other.Bottom;
        _start = other.Start;
        _end = other.End;
    }

    public Number All
    {
        get => _all;
        set => CheckChanged(ref _all, value);
    }

    public Number Bottom
    {
        get => _bottom;
        set => CheckChanged(ref _bottom, value);
    }

    public Number End
    {
        get => _end;
        set => CheckChanged(ref _end, value);
    }

    public Number Left
    {
        get => _left;
        set => CheckChanged(ref _left, value);
    }

    public Number Right
    {
        get => _right;
        set => CheckChanged(ref _right, value);
    }

    public Number Start
    {
        get => _start;
        set => CheckChanged(ref _start, value);
    }

    public Number Top
    {
        get => _top;
        set => CheckChanged(ref _top, value);
    }

    /// <summary>
    ///     Return the Side value
    /// </summary>
    public Number this[Side side]
    {
        get
        {
            switch (side)
            {
                case Side.Left: return Left;
                case Side.Top: return Top;
                case Side.Right: return Right;
                case Side.Bottom: return Bottom;
                case Side.Horizontal:
                    return Left == Right
                               ? Left
                               : Number.Undefined;
                case Side.Vertical:
                    return Top == Bottom
                               ? Top
                               : Number.Undefined;
                case Side.All: return All;
                case Side.Start: return Start;
                case Side.End: return End;
                default: throw new ArgumentException("Unknown side", nameof(side));
            }
        }
        set
        {
            switch (side)
            {
                case Side.Left:
                    Left = value;

                    break;
                case Side.Top:
                    Top = value;

                    break;
                case Side.Right:
                    Right = value;

                    break;
                case Side.Bottom:
                    Bottom = value;

                    break;
                case Side.Horizontal:
                    Left = Right = value;

                    break;
                case Side.Vertical:
                    Top = Bottom = value;

                    break;
                case Side.All:
                    All = value;

                    break;
                case Side.Start:
                    Start = value;

                    break;
                case Side.End:
                    End = value;

                    break;
                default: throw new ArgumentException("Unknown side", nameof(side));
            }
        }
    }

    public Number ComputedEdgeValue(Side side, Number? defaultValue = null)
    {
        if (this[side].HasValue)
            return this[side];

        if ((side == Side.Top || side == Side.Bottom) && this[Side.Vertical].HasValue)
            return this[Side.Vertical];

        if ((side == Side.Left || side == Side.Right || side == Side.Start || side == Side.End)
         && this[Side.Horizontal].HasValue)
            return this[Side.Horizontal];

        if (!this[Side.All].IsUndefined)
            return this[Side.All];

        if (side == Side.Start || side == Side.End)
            return Number.Undefined;

        return defaultValue ?? Number.Undefined;
    }

    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (_isDirty != value)
            {
                _isDirty = value;
                if (value)
                    RaiseChanged();
            }
        }
    }

    public bool IsUndefined =>
        _all.IsUndefined &&
        _start.IsUndefined && _end.IsUndefined &&
        _left.IsUndefined && _top.IsUndefined && _right.IsUndefined && _bottom.IsUndefined;

    public void ClearDirtyFlag() =>
        _isDirty = false;

    private void CheckChanged<T>(ref T field, T value)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            IsDirty = true;
        }
    }

    public Edges ToEdges(float width, float height)
    {
        var left   = ComputedEdgeValue(Side.Left, 0f).Resolve(width);
        var top   = ComputedEdgeValue(Side.Top, 0f).Resolve(height);
        var right   = ComputedEdgeValue(Side.Right, 0f).Resolve(width);
        var bottom   = ComputedEdgeValue(Side.Bottom, 0f).Resolve(height);
        return new Edges(left, top, right, bottom);
    }

    public override string ToString()
    {
        if (All.HasValue)
            return $"(all:{All})";
        if (Start.HasValue || End.HasValue)
            return $"({Start}:{End} - {Left},{Top},{Right},{Bottom})";

        return $"({Left},{Top},{Right},{Bottom})";
    }

    protected bool Equals(Sides other) =>
        Equals(All, other.All)
     && Equals(Bottom, other.Bottom)
     && Equals(End, other.End)
     && Equals(Left, other.Left)
     && Equals(Right, other.Right)
     && Equals(Start, other.Start)
     && Equals(Top, other.Top);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;

        return Equals((Sides)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = All.GetHashCode();
            hashCode = (hashCode * 397) ^ (Bottom.GetHashCode());
            hashCode = (hashCode * 397) ^ (End.GetHashCode());
            hashCode = (hashCode * 397) ^ (Left.GetHashCode());
            hashCode = (hashCode * 397) ^ (Right.GetHashCode());
            hashCode = (hashCode * 397) ^ (Start.GetHashCode());
            hashCode = (hashCode * 397) ^ (Top.GetHashCode());

            return hashCode;
        }
    }

    public static                   bool operator ==(Sides? left, Sides? right) => Equals(left, right);
    public static                   bool operator !=(Sides? left, Sides? right) => !Equals(left, right);
    public static implicit operator Sides(int               value) => Create(value);
    public static implicit operator Sides(Number            value) => Create(value);

    protected virtual void RaiseChanged() { Changed?.Invoke(this, EventArgs.Empty); }

    public Sides SetChanged(EventHandler? eventHandler)
    {
        if (eventHandler != null)
            Changed += eventHandler;
        return this;
    }
}
