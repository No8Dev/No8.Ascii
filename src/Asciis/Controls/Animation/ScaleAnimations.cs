namespace No8.Ascii.Controls.Animation;

public class ScaleInAnimation : AnimationDefinition
{
    public float StartScale = 0.7f;

    public ScaleInAnimation()
    {
        DurationMS = 8;
        OpacityFromZero = true;
    }

    public override Animation CreateAnimation(IAnimatable element)
    {
        var animation = new Animation();

        animation.WithConcurrent((f) => element.Opacity = f, 0, 1, null, 0, 0.5f);
        animation.WithConcurrent((f) => element.Scale = f, StartScale, 1, Easings.CubicOut);

        return animation;
    }

}

public class ScaleOutAnimation : AnimationDefinition
{
    public float EndScale = 0.7f;

    public ScaleOutAnimation()
    {
        DurationMS = 400;
    }

    public override Animation CreateAnimation(IAnimatable element)
    {
        var animation = new Animation();

        animation.WithConcurrent((f) => element.Opacity = f, 1, 0, null, 0.5f, 1);
        animation.WithConcurrent((f) => element.Scale = f, element.Scale, EndScale, Easings.CubicIn);

        return animation;
    }
}

public class ScaleFromElementAnimation : AnimationDefinition
{
    public Control FromElement { get; init; } = null!;

    public ScaleFromElementAnimation()
    {
        OpacityFromZero = true;
        DurationMS = 400;
    }

    public override Animation CreateAnimation(IAnimatable element)
    {
        Control control = (Control)element;
        var toBounds = element.Bounds;
        var fromBounds = FromElement.Bounds;

        control.Layout.Bounds = fromBounds;
        element.ClearTransforms();

        var animation = new Animation();

        animation.WithConcurrent((f) => element.Opacity = f, 0, 1, null, 0, 0.25f);
        animation.WithConcurrent((f) =>
        {
            var newBounds = new RectF(
                            fromBounds.X + (toBounds.X - fromBounds.X) * f,
                            fromBounds.Y + (toBounds.Y - fromBounds.Y) * f,
                            fromBounds.Width + (toBounds.Width - fromBounds.Width) * f,
                            fromBounds.Height + (toBounds.Height - fromBounds.Height) * f);
            control.Layout.Bounds = newBounds;
        });

        return animation;
    }

}