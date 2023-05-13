namespace No8.Ascii.Controls.Animation;

public enum ZDirection
{
    Away, Closer, Steady
}


public enum CircleDirection
{
    TopLeft, TopRight, BottomRight, BottomLeft
}

public class CircleInAnimation : AnimationDefinition
{
    public CircleDirection FromDirection { get; set; }
    public ZDirection FromZDirection { get; set; }
    public float Distance { get; set; }
    private float DistanceX { get; set; }
    private float DistanceY { get; set; }


    public CircleInAnimation()
    {
        DurationMS = 400;
        OpacityFromZero = true;
        FromDirection = CircleDirection.BottomLeft;
        FromZDirection = ZDirection.Away;
        Distance = 4f;
        DistanceX = 0;
        DistanceY = 0;
    }

    public override Animation CreateAnimation(IAnimatable element)
    {
        var translation = GetTranslation(element);
        var animation = new Animation();

        if (FromZDirection != ZDirection.Steady)
        {
            animation.WithConcurrent(
                f => element.Scale = f,
                FromZDirection == ZDirection.Away ? 0.3f : 2.0f, 1,
                Easings.BackOut, 0, 1);
        };
        animation.WithConcurrent(
            (f) => element.Opacity = f,
            0, 1,
            null,
            0, 0.25f);

        switch (FromDirection)
        {
            case CircleDirection.TopLeft:
                DistanceX = Distance * -0.5f;
                DistanceY = -Distance;
                break;
            case CircleDirection.TopRight:
                DistanceX = Distance * 0.5f;
                DistanceY = -Distance;
                break;
            case CircleDirection.BottomLeft:
                DistanceX = Distance * -0.5f;
                DistanceY = Distance;
                break;
            case CircleDirection.BottomRight:
                DistanceX = Distance * 0.5f;
                DistanceY = Distance;
                break;
        }

        animation.WithConcurrent(
            (f) => element.TranslationX = f,
            element.TranslationX + DistanceX, element.TranslationX,
            Easings.QuinticOut,
            0, 0.7f);

        animation.WithConcurrent(
            (f) => element.TranslationY = f,
            element.TranslationY + DistanceY, element.TranslationY,
            Easings.BackOut,
            0, 1);

        return animation;
    }
}

public class BounceInAnimation : AnimationDefinition
{
    public ZDirection FromDirection { get; set; }
    public float DistanceX { get; set; }
    public float DistanceY { get; set; }

    public BounceInAnimation()
    {
        DurationMS = 8;
        OpacityFromZero = true;
        FromDirection = ZDirection.Away;
        DistanceX = 0;
        DistanceY = 0;
    }

    public override Animation CreateAnimation(IAnimatable element)
    {
        var translation = GetTranslation(element);
        var animation = new Animation();

        if (FromDirection != ZDirection.Steady)
        {
            animation.WithConcurrent(
                f => element.Scale = f,
                FromDirection == ZDirection.Away ? 0.3f : 2.0f, 1,
                Easings.BackOut, 0, 1);
        };
        animation.WithConcurrent(
            (f) => element.Opacity = f,
            0, 1,
            null,
            0, 0.25f);

        if (Math.Abs(DistanceX) > 0)
        {
            animation.WithConcurrent(
                (f) => element.TranslationX = f,
                element.TranslationX + DistanceX, element.TranslationX,
                Easings.BackOut,
                0, 1);
        }
        if (Math.Abs(DistanceY) > 0)
        {
            animation.WithConcurrent(
                (f) => element.TranslationY = f,
                element.TranslationY + DistanceY, element.TranslationY,
                Easings.BackOut,
                0, 1);
        }

        return animation;
    }
}

public class BounceInUpAnimation : BounceInAnimation
{
    public float Distance
    {
        get { return DistanceY; }
        set { DistanceY = value; }
    }

    public BounceInUpAnimation()
    {
        Distance = 10;
    }
}

public class BounceInDownAnimation : BounceInAnimation
{
    public float Distance
    {
        get { return DistanceY; }
        set { DistanceY = value; }
    }

    public BounceInDownAnimation()
    {
        Distance = -10;
    }
}

public class BounceInLeftAnimation : BounceInAnimation
{
    public float Distance
    {
        get { return DistanceX; }
        set { DistanceX = value; }
    }

    public BounceInLeftAnimation()
    {
        Distance = 10;
    }
}

public class BounceInRightAnimation : BounceInAnimation
{
    public float Distance
    {
        get { return DistanceX; }
        set { DistanceX = value; }
    }

    public BounceInRightAnimation()
    {
        Distance = -10;
    }
}

public class BounceOutAnimation : AnimationDefinition
{
    public ZDirection ToDirection { get; set; }
    public float Amplitude { get; set; }
    public float DistanceX { get; set; }
    public float DistanceY { get; set; }

    public BounceOutAnimation()
    {
        DurationMS = 8;
        ToDirection = ZDirection.Away;
        Amplitude = 0.4f;
        DistanceX = 0;
        DistanceY = 0;
    }

    public override Animation CreateAnimation(IAnimatable element)
    {
        var translation = GetTranslation(element);
        var animation = new Animation();

        element.Opacity = 1;
        animation.WithConcurrent(
            (f) => element.Opacity = f,
            1, 0,
            null,
            0.5f, 1);

        if (ToDirection != ZDirection.Steady)
        {
            animation.WithConcurrent(
                (f) => element.Scale = f,
                1, ToDirection == ZDirection.Away ? 0.3f : 2.0f,
                Easings.BackIn, 0, 1);
        }
        if (Math.Abs(DistanceX) > 0)
        {
            animation.WithConcurrent(
                (f) => element.TranslationX = f,
                element.TranslationX, element.TranslationX + DistanceX,
                Easings.BackIn,
                0, 1);
        }
        if (Math.Abs(DistanceY) > 0)
        {
            animation.WithConcurrent(
                (f) => element.TranslationX = f,
                element.TranslationY, element.TranslationY + DistanceY,
                Easings.BackIn,
                0, 1);
        }

        return animation;
    }
}

public class BounceOutUpAnimation : BounceOutAnimation
{
    public float Distance
    {
        get { return DistanceY; }
        set { DistanceY = value; }
    }

    public BounceOutUpAnimation()
    {
        Distance = -10;
    }
}

public class BounceOutDownAnimation : BounceOutAnimation
{
    public float Distance
    {
        get { return DistanceY; }
        set { DistanceY = value; }
    }

    public BounceOutDownAnimation()
    {
        Distance = 10;
    }
}

public class BounceOutLeftAnimation : BounceOutAnimation
{
    public float Distance
    {
        get { return DistanceX; }
        set { DistanceX = value; }
    }

    public BounceOutLeftAnimation()
    {
        Distance = -10;
    }
}

public class BounceOutRightAnimation : BounceOutAnimation
{
    public float Distance
    {
        get { return DistanceX; }
        set { DistanceX = value; }
    }

    public BounceOutRightAnimation()
    {
        Distance = 10;
    }
}