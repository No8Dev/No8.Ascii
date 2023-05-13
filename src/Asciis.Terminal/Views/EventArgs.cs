namespace Asciis.Terminal.Views;

public abstract class ModalEventArgs : EventArgs
{
    protected ModalEventArgs(Window modal) { Modal = modal; }
    public Window Modal { get; private set; }
}

public class ModalPoppedEventArgs : ModalEventArgs
{
    public ModalPoppedEventArgs(Window modal) : base(modal) { }
}

public class ModalPoppingEventArgs : ModalEventArgs
{
    public ModalPoppingEventArgs(Window modal) : base(modal) { }
    public bool Cancel { get; set; }
}

public class ModalPushedEventArgs : ModalEventArgs
{
    public ModalPushedEventArgs(Window modal) : base(modal) { }
}

public class ModalPushingEventArgs : ModalEventArgs
{
    public ModalPushingEventArgs(Window modal) : base(modal) { }
}