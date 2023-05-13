
using static NodeLayout.NumberExtensions;

namespace NodeLayout;

public class Number
{
    public const float ValueUndefined = float.NaN;

    public static readonly Number Auto = new Number(0f, UoM.Auto);
    public static readonly Number Undefined = new Number(0f, UoM.Undefined);
    public static readonly Number Zero = new Number(0f, UoM.Point);

    public readonly float Value;
    public readonly UoM Unit;

    public Number(float value, UoM unit)
    {
        Unit = unit;
        Value = (unit == UoM.Auto || unit == UoM.Undefined) ? 0f : value;
    }

    public bool IsAuto => Unit == UoM.Auto;

    public bool IsUndefined =>
        Unit == UoM.Undefined ||
        Value.IsUndefined() && (Unit == UoM.Point || Unit == UoM.Percent);

    public bool HasValue => !IsUndefined;

    public bool Equals(Number other)
    {
        if (Unit != other.Unit)
            return false;

        switch (Unit)
        {
            case UoM.Undefined:
            case UoM.Auto:
                return true;
            case UoM.Point:
            case UoM.Percent:
                return
                    Value.Equals(other.Value);
        }

        return false;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        switch (Unit)
        {
            case UoM.Auto:
                return "auto";
            case UoM.Percent:
                return $"{Value}%";
            case UoM.Point:
                return $"{Value}pt";
            case UoM.Undefined:
            default:
                return string.Empty;
        }
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (ReferenceEquals(null, obj)) return false;
        return obj is Number other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            return (Value.GetHashCode() * 397) ^ (int)Unit;
        }
    }

    public static bool operator ==(Number left, Number right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (ReferenceEquals(null, left) || ReferenceEquals(null, right)) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Number left, Number right) { return !(left == right); }

    public static Number operator -(Number left)
    {
        return new Number(-left.Value, left.Unit);
    }

    public static implicit operator Number(int i)
    {
        return new Number(i, UoM.Point);
    }

    public static implicit operator Number(short s)
    {
        return new Number(s, UoM.Point);
    }

    public static implicit operator Number(float f)
    {
        return f.IsUndefined()
                   ? Undefined
                   : new Number(f, UoM.Point);
    }

    public static implicit operator Number(double d)
    {
        return new Number((float)d, UoM.Point);
    }

    public static Number Percent(float f)
    {
        return new Number(f, UoM.Percent);
    }

    public static Number Point(float f)
    {
        return new Number(f, UoM.Point);
    }

    public static Number Sanitized(float value, UoM unit)
    {
        if (value.IsUndefined() && unit != UoM.Auto)
            return new Number(0f, UoM.Undefined);
        return new Number(value, unit);
    }

    public float Resolve(float value)
    {
        switch (Unit)
        {
            case UoM.Point:
                return Value;
            case UoM.Percent:
                return (Value * value * 0.01f);
            default:
                return float.NaN;
        }
    }

    public float ResolveValueMargin(float value)
    {
        return IsAuto ? 0f : Resolve(value);
    }

    public static float RoundValueToPixelGrid(float value, float pointScaleFactor, bool forceCeil, bool forceFloor)
    {
        var scaledValue = value * pointScaleFactor;
        // We want to calculate `fractal` such that `floor(scaledValue) = scaledValue - fractal`.
        // float f = 0.0000019f;
        var fractal = FloatMod(scaledValue, 1.0f);
        if (fractal < 0)
        {
            // This branch is for handling negative numbers for `value`.
            //
            // Regarding `floor` and `ceil`. Note that for a number x, `floor(x) <= x <= ceil(x)`
            // even for negative numbers. Here are a couple of examples:
            //   - x =  2.2: floor( 2.2) =  2, ceil( 2.2) =  3
            //   - x = -2.2: floor(-2.2) = -3, ceil(-2.2) = -2
            //
            // Regarding `fmodf`. For fractional negative numbers, `fmodf` returns a
            // negative number. For example, `fmodf(-2.2) = -0.2`. However, we want
            // `fractal` to be the number such that subtracting it from `value` will
            // give us `floor(value)`. In the case of negative numbers, adding 1 to
            // `fmodf(value)` gives us this. Let's continue the example from above:
            //   - fractal = fmodf(-2.2) = -0.2
            //   - Add 1 to the fraction: fractal2 = fractal + 1 = -0.2 + 1 = 0.8
            //   - Finding the `floor`: -2.2 - fractal2 = -2.2 - 0.8 = -3
            ++fractal;
        }

        if (FloatsEqual(fractal, 0))
        {
            // First we check if the value is already rounded
            scaledValue = scaledValue - fractal;
        }
        else if (FloatsEqual(fractal, 1.0f))
        {
            scaledValue = (scaledValue - fractal) + 1.0f;
        }
        else if (forceCeil)
        {
            // var d = Math.Ceiling(scaledValue);
            // Next we check if we need to use forced rounding
            scaledValue = (scaledValue - fractal) + 1.0f;
        }
        else if (forceFloor)
        {
            scaledValue = scaledValue - fractal;
        }
        else
        {
            // Finally we just round the value
            scaledValue = (scaledValue - fractal) +
                (fractal.HasValue() &&
                    (fractal > 0.5f || FloatsEqual(fractal, 0.5f))
                        ? 1.0f
                        : 0.0f);
        }

        return scaledValue.IsUndefined() || pointScaleFactor.IsUndefined()
                ? Number.ValueUndefined
                : scaledValue / pointScaleFactor;
    }

    /// <summary>
    /// Unit of Measure
    /// </summary>
    public enum UoM
    {
        Undefined,
        Point,
        Percent,
        Auto
    }

}
