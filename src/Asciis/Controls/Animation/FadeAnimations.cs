namespace No8.Ascii.Controls.Animation;

public class FadeInAnimation : AnimationDefinition
{
    public float DistanceX { get; set; }
    public float DistanceY { get; set; }

    public FadeInAnimation()
    {
        DurationMS = 400;
        OpacityFromZero = true;
    }

    public override Animation CreateAnimation(IAnimatable element)
    {
        var animation = new Animation();

        animation.WithConcurrent((f) => element.Opacity = f, 0, 1, Easings.CubicOut);

        if (Math.Abs(DistanceX) > 0)
        {
            animation.WithConcurrent(
                (f) => element.TranslationX = f,
                element.TranslationX + DistanceX, element.TranslationX,
                Easings.CubicOut, 0, 1);
        }
        if (Math.Abs(DistanceY) > 0)
        {
            animation.WithConcurrent(
                (f) => element.TranslationY = f,
                element.TranslationY + DistanceY, element.TranslationY,
                Easings.CubicOut, 0, 1);
        }

        return animation;
    }
}

public class FadeOutAnimation : AnimationDefinition
{
    public float DistanceX { get; set; }
    public float DistanceY { get; set; }

    public FadeOutAnimation()
    {
        DurationMS = 400;
    }

    public override Animation CreateAnimation(IAnimatable element)
    {
        var animation = new Animation();

        animation.WithConcurrent(
            (f) => element.Opacity = f,
            1, 0);

        if (Math.Abs(DistanceX) > 0)
        {
            animation.WithConcurrent(
                (f) => element.TranslationX = f,
                element.TranslationX, element.TranslationX + DistanceX);
        }
        if (Math.Abs(DistanceY) > 0)
        {
            animation.WithConcurrent(
                (f) => element.TranslationY = f,
                element.TranslationY, element.TranslationY + DistanceY);
        }

        return animation;
    }
}


public class FadeInUpAnimation : FadeInAnimation
{
    public float Distance
    {
        get { return DistanceY; }
        set { DistanceY = value; }
    }

    public FadeInUpAnimation()
    {
        Distance = 2;
    }
}


public class FadeInDownAnimation : FadeInAnimation
{
    public float Distance
    {
        get { return DistanceY; }
        set { DistanceY = value; }
    }

    public FadeInDownAnimation()
    {
        Distance = -2;
    }
}


public class FadeInLeftAnimation : FadeInAnimation
{
    public float Distance
    {
        get { return DistanceX; }
        set { DistanceX = value; }
    }

    public FadeInLeftAnimation()
    {
        Distance = 2;
    }
}


public class FadeInRightAnimation : FadeInAnimation
{
    public float Distance
    {
        get { return DistanceX; }
        set { DistanceX = value; }
    }

    public FadeInRightAnimation()
    {
        Distance = -2;
    }
}



public class FadeOutUpAnimation : FadeOutAnimation
{
    public float Distance
    {
        get { return DistanceY; }
        set { DistanceY = value; }
    }

    public FadeOutUpAnimation()
    {
        Distance = -2;
    }
}


public class FadeOutDownAnimation : FadeOutAnimation
{
    public float Distance
    {
        get { return DistanceY; }
        set { DistanceY = value; }
    }

    public FadeOutDownAnimation()
    {
        Distance = 2;
    }
}


public class FadeOutLeftAnimation : FadeOutAnimation
{
    public float Distance
    {
        get { return DistanceX; }
        set { DistanceX = value; }
    }

    public FadeOutLeftAnimation()
    {
        Distance = -2;
    }
}


public class FadeOutRightAnimation : FadeOutAnimation
{
    public float Distance
    {
        get { return DistanceY; }
        set { DistanceY = value; }
    }

    public FadeOutRightAnimation()
    {
        Distance = 2;
    }
}