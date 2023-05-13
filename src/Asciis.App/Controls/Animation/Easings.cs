namespace Asciis.App.Controls.Animation;


public class Easing
{
    readonly Func<float, float> _easingFunc;

    public Easing(Func<float, float> easingFunc)
    {
        if (easingFunc == null)
            throw new ArgumentNullException("easingFunc");

        _easingFunc = easingFunc;
    }

    public float Ease(float v)
    {
        return _easingFunc(v);
    }

    public static implicit operator Easing(Func<float, float> func)
    {
        return new Easing(func);
    }
}


public static class Easings
{
    public static readonly Easing Linear = new Easing(x => x);

    public static readonly Easing QuadraticIn = QuadraticEasing.In;
    public static readonly Easing QuadraticOut = QuadraticEasing.Out;
    public static readonly Easing QuadraticInOut = QuadraticEasing.InOut;

    public static readonly Easing CubicIn = CubicEasing.In;
    public static readonly Easing CubicOut = CubicEasing.Out;
    public static readonly Easing CubicInOut = CubicEasing.InOut;

    public static readonly Easing QuarticIn = QuarticEasing.In;
    public static readonly Easing QuarticOut = QuarticEasing.Out;
    public static readonly Easing QuarticInOut = QuarticEasing.InOut;

    public static readonly Easing QuinticIn = QuinticEasing.In;
    public static readonly Easing QuinticOut = QuinticEasing.Out;
    public static readonly Easing QuinticInOut = QuinticEasing.InOut;

    public static readonly Easing SinusoidalIn = SinusoidalEasing.In;
    public static readonly Easing SinusoidalOut = SinusoidalEasing.Out;
    public static readonly Easing SinusoidalInOut = SinusoidalEasing.InOut;

    public static readonly Easing ExponentialIn = ExponentialEasing.In;
    public static readonly Easing ExponentialOut = ExponentialEasing.Out;
    public static readonly Easing ExponentialInOut = ExponentialEasing.InOut;

    public static readonly Easing CircularIn = CircularEasing.In;
    public static readonly Easing CircularOut = CircularEasing.Out;
    public static readonly Easing CircularInOut = CircularEasing.InOut;

    public static readonly Easing ElasticIn = ElasticEasing.In;
    public static readonly Easing ElasticOut = ElasticEasing.Out;
    public static readonly Easing ElasticInOut = ElasticEasing.InOut;

    public static readonly Easing BackIn = BackEasing.In;
    public static readonly Easing BackOut = BackEasing.Out;
    public static readonly Easing BackInOut = BackEasing.InOut;

    public static readonly Easing BounceIn = BounceEasing.In;
    public static readonly Easing BounceOut = BounceEasing.Out;
    public static readonly Easing BounceInOut = BounceEasing.InOut;

}

/// <summary>
/// Easing: t^2
/// </summary>
internal class QuadraticEasing
{
    public static Easing In = new Easing(x => x * x);
    public static Easing Out = new Easing(x => x * (2 - x));
    public static Easing InOut = new Easing(x =>
    {
        if ((x *= 2f) < 1f) return 0.5f * x * x;
        return -0.5f * (--x * (x - 2f) - 1f);
    });
}

/// <summary>
/// Easing: t^3
/// </summary>
internal class CubicEasing
{
    public static Easing In = new Easing(x => x * x * x);
    public static Easing Out = new Easing(x => --x * x * x + 1);
    public static Easing InOut = new Easing(x =>
    {
        if ((x *= 2f) < 1f) return 0.5f * x * x * x;
        return 0.5f * ((x -= 2f) * x * x + 2f);
    });
}

/// <summary>
/// Easing: t^4
/// </summary>
internal class QuarticEasing
{
    public static Easing In = new Easing(x => x * x * x * x);
    public static Easing Out = new Easing(x => 1 - (--x * x * x * x));
    public static Easing InOut = new Easing(x =>
    {
        if ((x *= 2f) < 1f) return 0.5f * x * x * x * x;
        return -0.5f * ((x -= 2f) * x * x * x - 2f);
    });
}

/// <summary>
/// Easing: t^5
/// </summary>
internal class QuinticEasing
{
    public static Easing In = new Easing(x => x * x * x * x * x);
    public static Easing Out = new Easing(x => --x * x * x * x * x + 1);
    public static Easing InOut = new Easing(x =>
    {
        if ((x *= 2f) < 1f) return 0.5f * x * x * x * x * x;
        return 0.5f * ((x -= 2f) * x * x * x * x + 2f);
    });
}


/// <summary>
/// Easing: Sinusoidal
/// </summary>
internal class SinusoidalEasing
{
    public static Easing In = new Easing(x =>
    {
        if (x == 0f) return 0f;
        if (x == 1f) return 1f;
        return 1f - MathF.Cos(x * MathF.PI / 2f);
    });
    public static Easing Out = new Easing(x =>
    {
        if (x == 0f) return 0f;
        if (x == 1f) return 1f;
        return MathF.Sin(x * MathF.PI / 2f);
    });
    public static Easing InOut = new Easing(x =>
    {
        if (x == 0) return 0;
        if (x == 1) return 1;
        return 0.5f * (1 - MathF.Cos(MathF.PI * x));
    });
}

/// <summary>
/// Easing: Exponential
/// </summary>
internal class ExponentialEasing
{
    public static Easing In = new Easing(x => x == 0 ? 0 : MathF.Pow(1024, x - 1));
    public static Easing Out = new Easing(x => x == 1 ? 1 : 1 - MathF.Pow(2, -10 * x));
    public static Easing InOut = new Easing(x =>
    {
        if (x == 0) return 0;
        if (x == 1) return 1;
        if ((x *= 2) < 1) return 0.5f * MathF.Pow(1024, x - 1);
        return 0.5f * (-MathF.Pow(2, -10 * (x - 1)) + 2);
    });
}


/// <summary>
/// Easing : Circular
/// </summary>
internal class CircularEasing
{
    public static Easing In = new Easing(x => 1 - MathF.Sqrt(1 - x * x));
    public static Easing Out = new Easing(x => MathF.Sqrt(1 - (--x * x)));
    public static Easing InOut = new Easing(x =>
    {
        if ((x *= 2) < 1) return -0.5f * (MathF.Sqrt(1 - x * x) - 1);
        return 0.5f * (MathF.Sqrt(1 - (x -= 2) * x) + 1);
    });
}

/// <summary>
/// Easing : Elastic
/// </summary>
internal class ElasticEasing
{
    public static Easing In = new Easing(x =>
    {
        float s = 0.0f, a = 0.1f, p = 0.4f;
        if (x == 0) return 0;
        if (x == 1) return 1;
        //if (!a || a < 1) { a = 1; s = p / 4; }
        if (a < 1) { a = 1; s = p / 4; }
        else s = p * MathF.Asin(1f / a) / (2 * MathF.PI);
        return -(a * MathF.Pow(2, 10 * (x -= 1)) * MathF.Sin((x - s) * (2 * MathF.PI) / p));
    });
    public static Easing Out = new Easing(x =>
    {
        float s = 0.0f, a = 0.1f, p = 0.4f;
        if (x == 0) return 0;
        if (x == 1) return 1;
        //if (!a || a < 1) { a = 1; s = p / 4; }
        if (a < 1) { a = 1; s = p / 4; }
        else s = p * MathF.Asin(1 / a) / (2 * MathF.PI);
        return (a * MathF.Pow(2, -10 * x) * MathF.Sin((x - s) * (2 * MathF.PI) / p) + 1);
    });
    public static Easing InOut = new Easing(x =>
    {
        float s = 0.0f, a = 0.1f, p = 0.4f;
        if (x == 0) return 0;
        if (x == 1) return 1;
        //if (!a || a < 1) { a = 1; s = p / 4; }
        if (a < 1) 
        { 
            a = 1; 
            s = p / 4; 
        }
        else 
            s = p * MathF.Asin(1 / a) / (2 * MathF.PI);
        if ((x *= 2) < 1) 
            return -0.5f * (a * MathF.Pow(2, 10 * (x -= 1)) * MathF.Sin((x - s) * (2f * MathF.PI) / p));
        return a * MathF.Pow(2, -10f * (x -= 1)) * MathF.Sin((x - s) * (2 * MathF.PI) / p) * 0.5f + 1;
    });
}


internal class BackEasing
{
    public static Easing In = new Easing(x => {
        var s = 1.70158f;
        return x * x * ((s + 1) * x - s);
    });
    public static Easing Out = new Easing(x => {
        var s = 1.70158f;
        return --x * x * ((s + 1) * x + s) + 1;
    });
    public static Easing InOut = new Easing(x => {
        var s = 1.70158f * 1.525f;
        if ((x *= 2) < 1) return 0.5f * (x * x * ((s + 1) * x - s));
        return 0.5f * ((x -= 2) * x * ((s + 1) * x + s) + 2);
    });
}

internal class BounceEasing
{
    private static Func<float, float> Bounce = x =>
    {
        if (x < (1f / 2.75f))
            return 7.5625f * x * x;

        if (x < (2f / 2.75f))
            return 7.5625f * (x -= (1.5f / 2.75f)) * x + 0.75f;

        if (x < (2.5f / 2.75f))
            return 7.5625f * (x -= (2.25f / 2.75f)) * x + 0.9375f;

        return 7.5625f * (x -= (2.625f / 2.75f)) * x + 0.984375f;
    };

    public static Easing In = new Easing(x => 1 - Bounce(1 - x));
    public static Easing Out = new Easing(x => Bounce(x));
    public static Easing InOut = new Easing(x => {
        if (x < 0.5f)
            return 0.5f - (Bounce(1 - (x * 2)) * 0.5f);

        return Bounce(x * 2 - 1) * 0.5f + 0.5f;
    });
}

public class PowerEasing
{
    private static Func<float, float, float> P = (x, p) => MathF.Pow(x, p);

    private float _power;
    public PowerEasing(float power)
    {
        _power = power;
    }

    public Func<float, float> In
    {
        get { return (x) => MathF.Pow(x, _power); }
    }
    public Func<float, float> Out
    {
        get { return (x) => 1 - MathF.Pow(--x, _power); }
    }
    public Func<float, float> InOut
    {
        get
        {
            return (x) =>
            {
                if ((x *= 2) < 1) 
                    return 0.5f * (MathF.Pow(x, _power));
                //return 0.5 + (0.5 * MathF.Pow(--x, _power));     // I havn't found the formula yet
                //return 1 - MathF.Pow(--x, _power);
                return 0.5f;
            };
        }
    }


}