namespace Asciis.FastLayout;

public static class NumberExtensions
{
    public static int Clamp(this int value, int min, int max)
    {
        if (value < min) value = min;
        if (value > max) value = max;

        return value;
    }
    public static int Clamp(this int value, int? min, int? max)
    {
        if (min.HasValue && value < min) value = min.Value;
        if (max.HasValue && value > max) value = max.Value;

        return value;
    }

    public static float Clamp(this float value, float min, float max)
    {
        if (!float.IsNaN(value))
        {
            if (!float.IsNaN(max))
            {
                if (value > max)
                    return max;
            }
            if (!float.IsNaN(min))
            {
                if (value < min)
                    return min;
            }
        }

        return value;
    }

    public static double Clamp(this double value, double min, double max)
    {
        if (!double.IsNaN(value))
        {
            if (!double.IsNaN(max))
            {
                if (value > max)
                    return max;
            }
            if (!double.IsNaN(min))
            {
                if (value < min)
                    return min;
            }
        }

        return value;
    }

    public static bool HasValue(this float value) => !float.IsNaN(value) && !float.IsInfinity(value);
    public static bool HasValue(this double value) => !double.IsNaN(value) && !double.IsInfinity(value);

    public static bool HasValue(this float? value) => value.HasValue && !float.IsNaN(value.Value) && !float.IsInfinity(value.Value);
    public static bool HasValue(this double? value) => value.HasValue && !double.IsNaN(value.Value) && !double.IsInfinity(value.Value);

    public static bool IsUndefined(this float value) => float.IsNaN(value) || float.IsInfinity(value);
    public static bool IsUndefined(this double value) => double.IsNaN(value) || double.IsInfinity(value);
    public static bool IsUndefined(this float? value) => !value.HasValue || float.IsNaN(value.Value) || float.IsInfinity(value.Value);
    public static bool IsUndefined(this double? value) => !value.HasValue || double.IsNaN(value.Value) || double.IsInfinity(value.Value);

    public static bool IsZero(this float value) => MathF.Abs(value) < 0.0001f;
    public static bool IsZero(this double value) => Math.Abs(value) < 0.0001;

    public static bool IsNotZero(this float value) => MathF.Abs(value) > 0.0001f;
    public static bool IsNotZero(this double value) => Math.Abs(value) > 0.0001;

    public static bool Is(this float value, float other) =>
        FloatsEqual(value, other);
    public static bool IsNot(this float value, float other) =>
        !FloatsEqual(value, other);

    public static bool Is(this double value, double other) =>
        DoublesEqual(value, other);
    public static bool IsNot(this double value, double other) =>
        !DoublesEqual(value, other);

    public static bool Within(this float value, float begin, float end) => value >= begin && value < end;
    public static bool Within(this double value, double begin, double end) => value >= begin && value < end;

    // This custom float equality function returns true if either absolute
    // difference between two floats is less than 0.0001f or both are undefined.
    public static bool FloatsEqual(float a, float b)
    {
        if (a.HasValue() && b.HasValue())
            return MathF.Abs(a - b) < 0.0001f;

        return a.IsUndefined() && b.IsUndefined();
    }

    // This custom float equality function returns true if either absolute
    // difference between two floats is less than 0.0001f or both are undefined.
    public static bool DoublesEqual(double a, double b)
    {
        if (a.HasValue() && b.HasValue())
            return Math.Abs(a - b) < 0.0001;

        return a.IsUndefined() && b.IsUndefined();
    }

    public static float FloatMax(float a, float b)
    {
        if (a.HasValue() && b.HasValue())
            return MathF.Max(a, b);

        return a.IsUndefined() ? b : a;
    }
    public static double DoubleMax(double a, double b)
    {
        if (a.HasValue() && b.HasValue())
            return Math.Max(a, b);

        return a.IsUndefined() ? b : a;
    }

    public static float FloatMod(float x, float y) => MathF.IEEERemainder(x, y);

    public static float FloatMin(float a, float b)
    {
        if (a.HasValue() && b.HasValue())
            return MathF.Min(a, b);

        return a.IsUndefined() ? b : a;
    }

    public static Number Point(this float value) => new (value, Number.UoM.Point);
    public static Number Percent(this float value) => new (value, Number.UoM.Percent);
    public static Number Point(this int value) => new (value, Number.UoM.Point);
    public static Number Percent(this int value) => new (value, Number.UoM.Percent);





    public static bool IsUndefined(this Number? value)
    {
        if (value == null) return true;
        return value.Unit == Number.UoM.Undefined ||
        value.Value.IsUndefined() && (value.Unit == Number.UoM.Point || value.Unit == Number.UoM.Percent);
    }

    public static bool HasValue(this Number? value) => !IsUndefined(value);

    public static float Resolve(this Number? number, float value)
    {
        if (number == null)
            return float.NaN;
        switch (number.Unit)
        {
            case Number.UoM.Point:
                return number.Value;
            case Number.UoM.Percent:
                return (number.Value * value * 0.01f);
            default:
                return float.NaN;
        }
    }

    public static float ResolveValueMargin(this Number number, float value)
    {
        return number.IsAuto ? 0f : Resolve(number, value);
    }

}
