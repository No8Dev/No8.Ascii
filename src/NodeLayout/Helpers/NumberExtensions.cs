using System;

namespace NodeLayout;

public static class NumberExtensions
{
    public static int Clamp(this int value, int min, int max)
    {
        if (value < min) value = min;
        if (value > max) value = max;

        return value;
    }

    public static float Clamp(this float value, float min, float max)
    {
        if (value < min) value = min;
        if (value > max) value = max;

        return value;
    }

    public static double Clamp(this double value, double min, double max)
    {
        if (value < min) value = min;
        if (value > max) value = max;

        return value;
    }

    public static bool HasValue(this float value) => !float.IsNaN(value) && !float.IsInfinity(value);

    public static bool IsUndefined(this float value) => float.IsNaN(value) || float.IsInfinity(value);

    public static bool IsZero(this float value) => Math.Abs(value) < 0.0001f;

    public static bool IsNotZero(this float value) => Math.Abs(value) > 0.0001f;

    public static bool Is(this float value, float other) =>
        FloatsEqual(value, other);

    // This custom float equality function returns true if either absolute
    // difference between two floats is less than 0.0001f or both are undefined.
    public static bool FloatsEqual(float a, float b)
    {
        if (a.HasValue() && b.HasValue())
            return Math.Abs(a - b) < 0.0001f;

        return a.IsUndefined() && b.IsUndefined();
    }

    public static float FloatMax(float a, float b)
    {
        if (a.HasValue() && b.HasValue())
            return Math.Max(a, b);

        return a.IsUndefined() ? b : a;
    }

    public static float FloatMod(float x, float y) => (float)Math.IEEERemainder(x, y);

    public static float FloatMin(float a, float b)
    {
        if (a.HasValue() && b.HasValue())
            return Math.Min(a, b);

        return a.IsUndefined() ? b : a;
    }

    public static Number Point(this float value) => new Number(value, Number.UoM.Point);
    public static Number Percent(this float value) => new Number(value, Number.UoM.Percent);
    public static Number Point(this int value) => new Number(value, Number.UoM.Point);
    public static Number Percent(this int value) => new Number(value, Number.UoM.Percent);

}
