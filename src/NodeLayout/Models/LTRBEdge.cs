using System;

using static NodeLayout.NumberExtensions;

namespace NodeLayout;

public interface ILTRBEdge
{
    float this[Side i] { get; }

    bool IsZero { get; }
    float Left { get; }
    float Top { get; }
    float Right { get; }
    float Bottom { get; }
}

/// <summary>
/// Left Top Right Bottom edges
/// </summary>
public class LTRBEdge : ILTRBEdge
{
    public LTRBEdge(float defaultValue = default)
    {
        Bottom = Left = Right = Top = defaultValue;
    }

    public LTRBEdge(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public LTRBEdge(LTRBEdge other)
    {
        Bottom = other.Bottom;
        Left = other.Left;
        Right = other.Right;
        Top = other.Top;
    }

    public bool IsZero =>
        Left.IsZero() &&
        FloatsEqual(Left, Top) &&
        FloatsEqual(Left, Right) &&
        FloatsEqual(Left, Bottom);

    public bool IsNotZero =>
        !IsZero;

    public float Left { get; set; }
    public float Top { get; set; }
    public float Right { get; set; }
    public float Bottom { get; set; }

    public float this[Side edge]
    {
        get
        {
            switch (edge)
            {
                case Side.Left: return Left;
                case Side.Top: return Top;
                case Side.Right: return Right;
                case Side.Bottom: return Bottom;
                default:
                    throw new ArgumentException("Unknown edge", nameof(edge));
            }
        }
        set
        {
            switch (edge)
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
                default:
                    throw new ArgumentException("Unknown edge", nameof(edge));
            }
        }
    }

    /// <inheritdoc />
    public override string ToString() => $"({Left}, {Top}, {Right}, {Bottom})";

    protected bool Equals(LTRBEdge other) =>
        Bottom.Equals(other.Bottom) &&
        Left.Equals(other.Left) &&
        Right.Equals(other.Right) &&
        Top.Equals(other.Top);

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((LTRBEdge)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Bottom.GetHashCode();
            hashCode = (hashCode * 397) ^ Left.GetHashCode();
            hashCode = (hashCode * 397) ^ Right.GetHashCode();
            hashCode = (hashCode * 397) ^ Top.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(LTRBEdge left, LTRBEdge right) => Equals(left, right);

    public static bool operator !=(LTRBEdge left, LTRBEdge right) => !Equals(left, right);
}
