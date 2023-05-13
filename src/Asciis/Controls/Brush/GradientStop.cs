namespace No8.Ascii.Controls;

public class GradientStop
{
    private readonly float _offset;

    public Color Color { get; init; } = Color.Transparent;
    public float Offset
    {
        get => _offset;
        init => _offset = Math.Clamp(value, 0.0f, 1.0f);
    }

    public GradientStop() { }
    public GradientStop(Color color, float offset)
    {
        Color  = color;
        Offset = Math.Clamp(offset, 0.0f, 1.0f);
    }

    public override string ToString() => $"{Color}:{Offset}";
}