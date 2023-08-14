public event EventHandler<LayoutPlan>? Changed;

/// <summary>
/// Direction that elements are added to the container. This defines the Layout-Direction as Vert or Horz.
/// The Cross-Direction would be the other.  Horz or Vert
/// Vert   Horz
///   1    1 2 3
///   2    
///   3    
/// </summary>
/// <remarks>Default value is: Vert</remarks>
public LayoutDirection ElementsDirection

public enum LayoutDirection
{
Vert,   // Vertical
Horz,   // Horizontal
}


/// <summary>
/// Do you want the elements to wrap when the layout direction (horz/vert) is full
/// NoWrap:      12345678
/// Wrap:        1 2 3 4 5
///              6 7 8
/// </summary>
/// <remarks>Default value is: NoWrap</remarks>
[ public LayoutWrap ElementsWrap ]

public enum LayoutWrap
{
NoWrap,
Wrap
}


/// <summary>
/// Determines how the elements are laid out in the Layout Direction
/// Flex Start:    [1 2 3          ]
/// Flex End:      [          1 2 3]
/// Centre:        [     1 2 3     ]
/// Space Between: [1      2      3]
/// Space Around:  [  1    2    3  ]
/// Space Evenly:  [   1   2   3   ]
/// </summary>
/// <remarks>Default value is: Start</remarks>
/// <remarks>A good mental modal for this how a single line of elements are aligned and spaced</remarks>
public AlignmentElements_LayoutDirection AlignElements_LayoutDirection

public enum AlignmentElements_LayoutDirection
{
Unknown = 0,
Auto = 1,
Start = 2,
Center = 3,
End = 4,
Stretch = 5,
SpaceBetween = 7,
SpaceAround = 8,
SpaceEvenly = 9
}


/// <summary>
/// Alignment of lines of elements in the Cross-Direction
///            Col
///  A B C D E F G
/// [1     D   1   ]  A = Start
/// [2     D 1     ]  B = End
/// [3     D     1 ]  C = Centre
/// [    1 D       ]  D = Stretch
/// [    2 D 2 2 2 ]  E = Space Around
/// [    3 D       ]  F = Space Between
/// [  1   D     3 ]  G = Space Evenly (not implemented)
/// [  2   D 3     ]
/// [  3   D   3   ]
/// </summary>
/// <remarks>Default value is: Start</remarks>
public Alignment_Cross AlignElements_Cross

/// <summary>
///     Align content for Cross-Direction
/// </summary>
public enum Alignment_Cross
{
Unknown = 0,
Auto = 1,
Start = 2,
Center = 3,
End = 4,
Stretch = 5,
SpaceBetween = 7,
SpaceAround = 8,
SpaceEvenly = 9
}

/// <summary>
/// Determines how each element is laid out in the Cross Direction within a single line.
/// [A     D ]  A = Start
/// [A   C D ]  B = End
/// [A B C D ]  C = Centre
/// [  B C D ]  D = Stretch
/// [  B   D ]
/// </summary>
/// <remarks>Default value is: Stretch</remarks>
public AlignmentLine_Cross Align_Cross

/// <summary>
///     Align elements for Cross-Direction
/// </summary>
public enum AlignmentLine_Cross
{
Unknown = 0,
Auto = 1,
Start = 2,
Center = 3,
End = 4,
Stretch = 5,
}



/// <summary>
/// Align Element Cross is normally assigned by the container (Align_Cross).  AlignSelf_Cross will overwrite the containers wishes.
/// e.g. The container want all elements aligned to start, but this one element should align center.
/// </summary>
/// <remarks>Default value is: Auto</remarks>
public AlignmentLine_Cross AlignSelf_Cross

/// <summary>
/// When elements are too big for the container.  Visible, Hidden or Scroll
/// </summary>
/// <remarks>Default value is: Visible</remarks>
public Overflow Overflow

/// <summary>
///     What to do the the case of the content being bigger than the container
/// </summary>
public enum Overflow
{
Visible,
Hidden,
Scroll
}


[ public Sides Margin ]
[ public Sides Padding ]

/// <summary>
/// Position is set to either Relative, or absolute
/// </summary>
/// <remarks>Default value is: Relative</remarks>
[ public PositionType PositionType ]

[ public Sides Position ]
[ public Number Left, Top, Right, Bottom ]
[ public Number Start, End ] 
public number Horizontal, Vertical

/// <summary>
///     Combination of FlexGrow and FlexShrink
/// </summary>
public float Flex
/// <summary>
/// Grow item to fill the Layout-Direction
/// </summary>
/// <remarks>Default value is: 0.0</remarks>
public float FlexGrow

/// <summary>
/// Shrink item to fit Layout-Direction
/// </summary>
/// <remarks>Default value is: 0.0</remarks>
public float FlexShrink

/// <summary>
/// The default size of the element in the Layout-Direction, prior to remaining space being distributed
/// </summary>
/// <remarks>Default value is: auto</remarks>
public Number ChildLayoutDirectionLength

/// <summary>
///     Requested Width
/// </summary>
/// <remarks>Default value is: auto</remarks>
[ public Number Width ]

/// <summary>
///     Requested Height
/// </summary>
/// <remarks>Default value is: auto</remarks>
[ public Number Height ]

[ public Number MinWidth, MaxWidth ]
[ public Number MinHeight, MaxHeight ]

/// <summary>
///     Aspect ratio for node
/// </summary>
[ public float AspectRatio ]

/// <summary>
/// Is the Node a Text element. Used to calculate baseline
/// </summary>
[ public bool IsText ]

/// <summary>
///     A element of a container without sub-elements (equivalent to Display: none)
///     Does not calculate elements as part of layout arrangement
/// </summary>
public bool Atomic


public LayoutDirection ElementsDirection
public AlignmentElements_LayoutDirection AlignElements_LayoutDirection
public Alignment_Cross AlignElements_Cross
public AlignmentLine_Cross Align_Cross
public AlignmentLine_Cross AlignSelf_Cross

public Overflow Overflow
public number Horizontal, Vertical
public float Flex
public float FlexGrow
public float FlexShrink
public Number ChildLayoutDirectionLength
public bool Atomic



protected ControlAlign ChildrenHorzAlign        Horz => AlignElements_LayoutDirection,      Vert => AlignElements_Cross + Align_Cross
protected ControlAlign ChildrenVertAlign        Horz => AlignElements_Cross + Align_Cross,  Vert => AlignElements_LayoutDirection

public enum ControlAlign
{
    Unknown = 0,
    Auto = 1,
    Start = 2,
    Center = 3,
    End = 4,
    Stretch = 5,
}

[ public Sides Margin ]
[ public Sides Padding ]
[ public PositionType PositionType ]

[ public Sides Position ]
[ public Number Left, Top, Right, Bottom ]
[ public Number Start, End ] 

[ public Number Width ]
[ public Number Height ]
[ public Number MinWidth, MaxWidth ]
[ public Number MinHeight, MaxHeight ]
[ public float AspectRatio ]
[ public bool IsText ]



Frame
public LayoutWrap Wrap
public float FlexShrink
public float FlexGrow
// Default values for Frame
LayoutPlan.Padding = 1;
LayoutPlan.Width = 100.Percent();
LayoutPlan.Height = 100.Percent();

Row
public LayoutWrap ElementsWrap
public float FlexShrink
public float FlexGrow
// Default values for Row
LayoutPlan.ElementsDirection = LayoutDirection.Horz;
HorzAlign = ControlAlign.Stretch;
VertAlign = ControlAlign.Start;

Window
LayoutPlan.Width  = 100.Percent();
LayoutPlan.Height = 100.Percent();

Label
LayoutPlan.IsText    = true;
LayoutPlan.MinHeight = 1;
LayoutPlan.MinWidth  = 1;
