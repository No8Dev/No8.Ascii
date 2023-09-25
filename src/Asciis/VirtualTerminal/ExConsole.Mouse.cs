namespace No8.Ascii.VirtualTerminal;

using static TerminalSeq;

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

public partial class ExConsole : ExConsole.IExConsoleMouse
{
    private int _mouseX;
    private int _mouseY;
    public IExConsoleMouse Mouse => this;
    
    public interface IExConsoleMouse
    {
        void HighlightEnable();
        void HighlightDisable();
        void TrackingStart();
        void TrackingStop();
        
        int X { get; }
        int Y { get; }
    }
    
    void IExConsoleMouse.HighlightEnable()
    {
        Write(ControlSeq.SetResourceValue(1));   // This should be the default anyway
        Write(Mode.MouseTrackingHilite);
    }

    void IExConsoleMouse.HighlightDisable()
    {
        Write(Mode.StopMouseTrackingHilite);
        Write(ControlSeq.SetResourceValue(1));
    }

    void IExConsoleMouse.TrackingStart()
    {
        Write(ControlSeq.PrivateModeSetDec(
            1003,   // Use All Motion Mouse Tracking
            1006   // SGR ext mouse mode
        ));
        if (TermInfo.Extended.Exist("XF"))
        {
            Write(ControlSeq.PrivateModeSetDec(
                1004   // Send focus in/out events
            ));
        }
    }
    void IExConsoleMouse.TrackingStop()
    {
        Write(ControlSeq.PrivateResetDec(
            1003,   // Use All Motion Mouse Tracking
            1006   // SGR ext mouse mode
        ));
        if (TermInfo.Extended.Exist("XF"))
        {
            Write(ControlSeq.PrivateResetDec(
                1004   // Send focus in/out events
            ));
        }
    }

    int IExConsoleMouse.X => _mouseX;

    int IExConsoleMouse.Y => _mouseY;
}