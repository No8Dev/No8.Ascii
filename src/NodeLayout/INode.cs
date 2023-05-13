using System;
using System.Collections.Generic;

namespace NodeLayout;

public interface INode
{
    string Name { get; }
    INode Container { get; }
    IReadOnlyList<INode> Elements { get; }
    LayoutPlan Plan { get; }
    Layout Layout { get; set; }
    object Context { get; set; }
    bool IsDirty { get; set; }
    MeasureFunc MeasureNode { get; set; }
}

public delegate VecF MeasureFunc(INode node,
                                 float width,
                                 MeasureMode widthMode,
                                 float height,
                                 MeasureMode heightMode);
