using System.Text;

namespace Asciis.UI.Controls;

public class Stack : Control
{
    static Stack()
    {
        Composer.Register(false, typeof(Stack), typeof(StackComposer));
    }

    private Direction? _direction;

    public Direction? Direction
    {
        get => _direction;
        set => ChangeDirtiesLayout(ref _direction, value);
    }

    public Stack(Direction? direction = null, string? name = null) : base(name)
    {
        Direction = direction;
    }

    public Stack(out Stack stack, Direction? direction = null, string? name = null) : this(direction, name)
    {
        stack = this;
    }

    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        if (Direction != null && Direction != Controls.Direction.Unknown)
            sb.Append(" Direction=" + Direction);
    }
}

public enum Direction
{
    Unknown,
    Vertical,
    Horizontal
}
