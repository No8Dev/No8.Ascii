﻿
using No8.Ascii.ElementLayout;
using No8.Ascii.Controls;

namespace No8.Ascii.ElementLayout;

public interface IElement
{
    string?          Name        { get; }
    Control?         Parent      { get; }
    IReadOnlyList<Control> Children    { get; }
    LayoutPlan       Plan        { get; }
    LayoutActual     Layout      { get; set; }
    object?          Context     { get; set; }
    bool             IsDirty     { get; set; }
    MeasureFunc?     MeasureFunc { get; set; }
}


public delegate float BaselineFunc(
    IElement element,
    float width,
    float height);

public delegate VecF MeasureFunc(
    IElement    element,
    float       width,
    MeasureMode widthMode,
    float       height,
    MeasureMode heightMode);