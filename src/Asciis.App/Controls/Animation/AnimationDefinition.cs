namespace Asciis.App.Controls.Animation;

public record KeyFrame<T> (float MilliSeconds, T Value, Easing Easing);

public abstract class AnimationDefinition
{
    public uint DurationMS { get; set; }
    public int PauseBeforeMS { get; set; }
    public int PauseAfterMS { get; set; }
    public float SpeedRatio { get; set; }
    public float RepeatCount { get; set; }
    public int RepeatDurationMS { get; set; }
    public int DelayMS { get; set; }
    public bool AutoReverse { get; set; }
    public bool Forever { get; set; }

    public bool OpacityFromZero { get; protected set; }

    protected AnimationDefinition()
    {
        DurationMS = 1000;
    }

    protected (float x, float y) GetTranslation(IAnimatable element)
    {
        return (element.TranslationX, element.TranslationY);
    }

    public abstract Animation CreateAnimation(IAnimatable element);
}