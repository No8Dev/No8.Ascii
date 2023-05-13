using System;
using System.Collections.Generic;

namespace NodeLayout;

public class Sides
{
    public static Sides Zero =>
        new Sides(Number.Zero);

    public static Sides Create(Number value) =>
        new Sides(value);

    public static Sides Create(Number left = null,
                                Number top = null,
                                Number right = null,
                                Number bottom = null,
                                Number start = null,
                                Number end = null,
                                Number horizontal = null,
                                Number vertical = null,
                                Number all = null) =>
        new Sides(left, top, right, bottom, start, end, horizontal, vertical, all);

    public event EventHandler Changed;
    private bool _isDirty;
    private Number _all = Number.Undefined;
    private Number _bottom = Number.Undefined;
    private Number _end = Number.Undefined;
    private Number _left = Number.Undefined;
    private Number _right = Number.Undefined;
    private Number _start = Number.Undefined;
    private Number _top = Number.Undefined;

    /// <summary>
    /// All sides have the same value
    /// </summary>
    public Sides(Number value) { _left = _top = _right = _bottom = _start = End = All = value; }

    public Sides(Number left = null,
                  Number top = null,
                  Number right = null,
                  Number bottom = null,
                  Number start = null,
                  Number end = null,
                  Number horizontal = null,
                  Number vertical = null,
                  Number all = null)
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
        set => CheckChanged(ref _all, value ?? Number.Undefined);
    }

    public Number Bottom
    {
        get => _bottom;
        set => CheckChanged(ref _bottom, value ?? Number.Undefined);
    }

    public Number End
    {
        get => _end;
        set => CheckChanged(ref _end, value ?? Number.Undefined);
    }

    public Number Left
    {
        get => _left;
        set => CheckChanged(ref _left, value ?? Number.Undefined);
    }

    public Number Right
    {
        get => _right;
        set => CheckChanged(ref _right, value ?? Number.Undefined);
    }

    public Number Start
    {
        get => _start;
        set => CheckChanged(ref _start, value ?? Number.Undefined);
    }

    public Number Top
    {
        get => _top;
        set => CheckChanged(ref _top, value ?? Number.Undefined);
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

    public Number ComputedEdgeValue(Side side, Number defaultValue = null)
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

    public override bool Equals(object obj)
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
            var hashCode = All != null
                               ? All.GetHashCode()
                               : 0;
            hashCode = (hashCode * 397)
                     ^ (Bottom != null
                             ? Bottom.GetHashCode()
                             : 0);
            hashCode = (hashCode * 397)
                     ^ (End != null
                             ? End.GetHashCode()
                             : 0);
            hashCode = (hashCode * 397)
                     ^ (Left != null
                             ? Left.GetHashCode()
                             : 0);
            hashCode = (hashCode * 397)
                     ^ (Right != null
                             ? Right.GetHashCode()
                             : 0);
            hashCode = (hashCode * 397)
                     ^ (Start != null
                             ? Start.GetHashCode()
                             : 0);
            hashCode = (hashCode * 397)
                     ^ (Top != null
                             ? Top.GetHashCode()
                             : 0);

            return hashCode;
        }
    }

    public static bool operator ==(Sides left, Sides right) =>
        Equals(left, right);

    public static bool operator !=(Sides left, Sides right) =>
        !Equals(left, right);

    public static implicit operator Sides(int value) =>
        Create(value);

    public static implicit operator Sides(Number value) =>
        Create(value);

    protected virtual void RaiseChanged() { Changed?.Invoke(this, null); }

    public Sides SetChanged(EventHandler eventHandler)
    {
        Changed += eventHandler;

        return this;
    }
}
