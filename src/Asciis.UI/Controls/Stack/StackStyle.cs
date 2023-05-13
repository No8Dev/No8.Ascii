namespace Asciis.UI.Controls;

public class StackStyle : Style
{
    public Direction? Direction
    {
        get => Get<Direction?>(nameof(Direction));
        set => Set(nameof(Direction), value);
    }
}
