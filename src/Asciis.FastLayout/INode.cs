namespace Asciis.FastLayout;

public interface INode
{
    Vec DoLayout(
        float containerWidth  = Number.ValueUndefined,
        float containerHeight = Number.ValueUndefined);


    INode?               Parent   { get; }
    IReadOnlyList<INode> Children { get; }

    LayoutPlan   Plan        { get; }
    RenderLayout Layout      { get; }
    MeasureFunc? MeasureNode { get; set; }
    bool         IsDirty     { get; set; }
}


public delegate VecF MeasureFunc(
    INode       node,
    float       width,
    MeasureMode widthMode,
    float       height,
    MeasureMode heightMode);
