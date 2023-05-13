using System.Text;

namespace Asciis.UI.Controls;

public class StackLayout : Layout<Stack>
{
    public Direction Direction { get; set; }

    public StackLayout(Stack stack) : base(stack)
    {
    }

    public override void UpdateValues(Stack stack)
    {
        base.UpdateValues(stack);

        Direction = stack.Direction ?? Direction.Vertical;
    }

    protected override void AppendProperties(StringBuilder sb)
    {
        base.AppendProperties(sb);
        sb.Append($" Direction={Direction}");
    }
}
