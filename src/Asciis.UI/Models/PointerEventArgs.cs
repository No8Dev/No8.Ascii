namespace Asciis.UI;

public record PointerEventArgs(
    long TimeStamp,
    PointerEventType PointerEventType,
    int X,
    int Y,
    int ButtonId = -1,
    int Value = -1,
    bool Handled = false
    )
{
    public override string ToString()
    {
        return ButtonId > 0
                   ? $"Pointer: {PointerEventType} [{ButtonId}] ({X},{Y})"
                   : $"Pointer: {PointerEventType} ({X},{Y})";
    }
}

public record ScrollWheelEventArgs(
    long TimeStamp,
    int Value,
    bool Handled = false
    )
{
    public override string ToString() =>
        $"ScrollWheel: {Value}";
}

public enum PointerEventType
{
    Unknown,
    Enter,
    Move,
    Pressed,
    Released,
    Click,
    DoubleClick,
    Wheel,
    HorizontalWheel,
    Leave
}

public enum PointerType
{
    Mouse,
    Pen,
    Touch
}

[Flags]
public enum PointerButton
{
    None = 0,
    Primary = 1,
    Secondary = 1 << 1,
    Auxiliary = 1 << 2,
    Forth = 1 << 3,
    Fifth = 1 << 4
}

[Flags]
public enum PointerFlags
{
    Unknown,
    Pressed,
    Released,
    Clicked,
    DoubleClicked,
    TrippleClicked
}

public record PointerState(
    int Id,
    PointerType PointerType,
    PointerFlags PointerFlags,
    Vec Position,
    int Count,
    PointerButton Buttons = PointerButton.None,
    float Pressure = 0f)
{
    public override string ToString()
    {
        return $"{PointerType}:{Id} {Position} {Count} {Buttons}";
    }
}
