
namespace Asciis.App;

using static NumberExtensions;

/// <summary>
/// Represents the layout definition as a fixed value, percentage, or auto
/// </summary>
public record Number(float Value, Number.UoM Unit)
{
    public const float ValueUndefined = float.NaN;

    public static readonly Number Auto = new(0f, UoM.Auto);
    public static readonly Number Undefined = new(0f, UoM.Undefined);
    public static readonly Number Zero = new(0f, UoM.Point);

    public bool IsAuto => Unit == UoM.Auto;
    
    public bool IsUndefined =>
        Unit == UoM.Undefined ||
        Value.IsUndefined() && (Unit == UoM.Point || Unit == UoM.Percent);

    public bool HasValue => !IsUndefined;

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
                return $"{Value}";
            case UoM.Undefined:
            default:
                return string.Empty;
        }
    }

    public static Number operator -(Number left) => new(-left.Value, left.Unit);

    public static implicit operator Number(int i) => new(i, UoM.Point);

    public static implicit operator Number(short s) => new(s, UoM.Point);

    public static implicit operator Number(float f) =>
        f.IsUndefined()
            ? Undefined
            : new(f, UoM.Point);

    public static implicit operator Number(double d) => new((float)d, UoM.Point);

    public static Number Percent(float f) => new(f, UoM.Percent);

    public static Number Point(float f) => new(f, UoM.Point);

    public static Number Sanitized(float value, UoM unit)
    {
        if (value.IsUndefined() && unit != UoM.Auto)
            return new(0f, UoM.Undefined);
        return new(value, unit);
    }

    public static float RoundValue(float value, float pointScaleFactor, bool forceCeil, bool forceFloor)
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

    public static Number Parse(string? str)
    {
        if (str == null) 
            return 1.Percent();
        if (str == "auto") 
            return Auto;
        if (str.Contains('%') || str.Contains('*'))
        {
            if (str == "*")
                str = "1*";
            return Convert.ToSingle(str.Replace('%', ' ')
                                       .Replace('*', ' '))
                          .Percent();
        }
        return Convert.ToSingle(str);
    }

    public virtual bool Equals(Number? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return FloatsEqual(Value, other.Value) && Unit == other.Unit;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, (int)Unit);
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
