using Asciis.UI.Controls;

namespace Asciis.UI;

public interface INode
{
    string?          Name        { get; }
    Control?         Parent      { get; }
    IReadOnlyList<Control> Children    { get; }
    LayoutPlan       Plan        { get; }
    LayoutActual     Layout      { get; set; }
    object?          Context     { get; set; }
    bool             IsDirty     { get; set; }
    MeasureFunc?     MeasureNode { get; set; }
}


public delegate float BaselineFunc(
    INode node,
    float width,
    float height);

public delegate VecF MeasureFunc(
    INode       node,
    float       width,
    MeasureMode widthMode,
    float       height,
    MeasureMode heightMode);