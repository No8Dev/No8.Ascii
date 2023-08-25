namespace No8.Ascii.VirtualTerminal;

using static TerminalSeq;

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

public partial class ExConsole : ExConsole.IExConsoleMouse
{
    public IExConsoleMouse Mouse => this;
    
    public interface IExConsoleMouse
    {
        void HighlightEnable();
        void HighlightDisable();
        void TrackingStart();
        void TrackingStop();
    }
    
    void IExConsoleMouse.HighlightEnable()
    {
        CD.Write(ControlSeq.SetResourceValue(1));   // This should be the default anyway
        CD.Write(Mode.MouseTrackingHilite);
    }

    void IExConsoleMouse.HighlightDisable()
    {
        CD.Write(Mode.StopMouseTrackingHilite);
        CD.Write(ControlSeq.SetResourceValue(1));
    }

    void IExConsoleMouse.TrackingStart()
    {
        CD.Write(ControlSeq.PrivateModeSetDec(
            1003,   // Use All Motion Mouse Tracking
            1006   // SGR ext mouse mode
        ));
        if (TermInfo.Extended.Exist("XF"))
        {
            CD.Write(ControlSeq.PrivateModeSetDec(
                1004   // Send focus in/out events
            ));
        }
    }
    void IExConsoleMouse.TrackingStop()
    {
        CD.Write(ControlSeq.PrivateResetDec(
            1003,   // Use All Motion Mouse Tracking
            1006   // SGR ext mouse mode
        ));
        if (TermInfo.Extended.Exist("XF"))
        {
            CD.Write(ControlSeq.PrivateResetDec(
                1004   // Send focus in/out events
            ));
        }
    }
}