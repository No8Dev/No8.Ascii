namespace Asciis.Terminal.Core;

/// TextDirection  [H] = Horizontal  [V] = Vertical
/// =============
/// LeftRight_TopBottom [H] Normal
/// TopBottom_LeftRight [V] Normal
/// 
/// RightLeft_TopBottom [H] Invert Text
/// TopBottom_RightLeft [V] Invert Lines
/// 
/// LeftRight_BottomTop [H] Invert Lines
/// BottomTop_LeftRight [V] Invert Text
/// 
/// RightLeft_BottomTop [H] Invert Text + Invert Lines
/// BottomTop_RightLeft [V] Invert Text + Invert Lines
///
/// <summary>
/// Text direction enumeration, controls how text is displayed.
/// </summary>
public enum TextDirection
{
    /// <summary>
    /// Normal horizontal direction.
    /// <code>HELLO<br/>WORLD</code>
    /// </summary>
    LeftRight_TopBottom,

    /// <summary>
    /// Normal vertical direction.
    /// <code>H W<br/>E O<br/>L R<br/>L L<br/>O D</code>
    /// </summary>
    TopBottom_LeftRight,

    /// <summary>
    /// This is a horizontal direction. <br/> RTL
    /// <code>OLLEH<br/>DLROW</code>
    /// </summary>
    RightLeft_TopBottom,

    /// <summary>
    /// This is a vertical direction.
    /// <code>W H<br/>O E<br/>R L<br/>L L<br/>D O</code>
    /// </summary>
    TopBottom_RightLeft,

    /// <summary>
    /// This is a horizontal direction.
    /// <code>WORLD<br/>HELLO</code>
    /// </summary>
    LeftRight_BottomTop,

    /// <summary>
    /// This is a vertical direction.
    /// <code>O D<br/>L L<br/>L R<br/>E O<br/>H W</code>
    /// </summary>
    BottomTop_LeftRight,

    /// <summary>
    /// This is a horizontal direction.
    /// <code>DLROW<br/>OLLEH</code>
    /// </summary>
    RightLeft_BottomTop,

    /// <summary>
    /// This is a vertical direction.
    /// <code>D O<br/>L L<br/>R L<br/>O E<br/>W H</code>
    /// </summary>
    BottomTop_RightLeft
}
