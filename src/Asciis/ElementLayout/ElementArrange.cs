using System.Diagnostics;
using System.Runtime.CompilerServices;
using No8.Ascii.ElementLayout;
using No8.Ascii.Controls;

namespace No8.Ascii.ElementLayout;

using static NumberExtensions;

internal static class ElementArrange
{
    private static int _currentGenerationCount;

    public static void Calculate(
        IElement element,
        float containerWidth = Number.ValueUndefined,
        float containerHeight = Number.ValueUndefined,
        float pointScaleFactor = 1f)
    {
        float width,
              height;
        MeasureMode widthMeasureMode,
                    heightMeasureMode;

        // Increment the generation count. This will force the recursive routine to
        // visit all dirty elements at least once. Subsequent visits will be skipped if
        // the input parameters don't change.
        _currentGenerationCount++;

        ResolveDimension(element);

        if (IsLayoutDirectionInPlan(element, LayoutDirection.Horz, containerWidth))
        {
            width = element.Layout.ResolvedWidth.Resolve(containerWidth)
                    + GetMarginForDirection(element, LayoutDirection.Horz, containerWidth);
            widthMeasureMode = MeasureMode.Exactly;
        }
        else if (element.Plan.MaxWidth.Resolve(containerWidth).HasValue())
        {
            width = element.Plan.MaxWidth.Resolve(containerWidth);
            widthMeasureMode = MeasureMode.AtMost;
        }
        else
        {
            width = containerWidth;
            widthMeasureMode = width.IsUndefined()
                ? MeasureMode.Undefined
                : MeasureMode.Exactly;
        }

        if (IsLayoutDirectionInPlan(element, LayoutDirection.Vert, containerHeight))
        {
            height = element.Layout.ResolvedHeight.Resolve(containerHeight)
                     + GetMarginForDirection(element, LayoutDirection.Vert, containerWidth);
            heightMeasureMode = MeasureMode.Exactly;
        }
        else if (element.Plan.MaxHeight.Resolve(containerHeight).HasValue())
        {
            height = element.Plan.MaxHeight.Resolve(containerHeight);
            heightMeasureMode = MeasureMode.AtMost;
        }
        else
        {
            height = containerHeight;
            heightMeasureMode = height.IsUndefined()
                ? MeasureMode.Undefined
                : MeasureMode.Exactly;
        }

        if (LayoutElement(
                element,
                width,
                height,
                widthMeasureMode,
                heightMeasureMode,
                containerWidth,
                containerHeight,
                true,
                LayoutPassReason.Initial,
                0, // tree root
                _currentGenerationCount))
        {
            SetPosition(
                element,
                containerWidth,
                containerHeight,
                containerWidth);
            RoundToPixelGrid(element, pointScaleFactor, 0f, 0f);

#if DEBUG
            // Logger.Log(LogLevel.Verbose, ElementPrint.Format(element));
#endif
        }
    }

    /// <summary>
    /// This is a wrapper around the LayoutImplementation function. It determines whether
    /// the layout request is redundant and can be skipped.
    /// </summary>
    private static bool LayoutElement(
        IElement element,
        float availableWidth,
        float availableHeight,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        float containerWidth,
        float containerHeight,
        bool performLayout,
        LayoutPassReason reason,
        int depth,
        int generationCount)
    {
        var layout = element.Layout;
        CachedMeasurement? cachedResults = null;

        depth++;

        var needToVisitElement =
            element.IsDirty && layout.GenerationCount != generationCount;

        if (needToVisitElement)
        {
            // Invalidate the cached results.
            layout.NextCachedMeasurementsIndex = 0;
            layout.CachedLayout.WidthMeasureMode = MeasureMode.Undefined;
            layout.CachedLayout.HeightMeasureMode = MeasureMode.Undefined;
            layout.CachedLayout.ComputedWidth = -1;
            layout.CachedLayout.ComputedHeight = -1;
        }

        // Determine whether the results are already cached. We maintain a separate
        // cache for layouts and measurements. A layout operation modifies the
        // positions and dimensions for elements in the subtree. The algorithm assumes
        // that each element gets laid out a maximum of one time per tree layout, but
        // multiple measurements may be required to resolve all of the flex
        // dimensions. We handle elements with measure functions specially here because
        // they are the most expensive to measure, so it's worth avoiding redundant
        // measurements if at all possible.
        if (element.MeasureFunc != null)
        {
            var marginRow = GetMarginForDirection(element, LayoutDirection.Horz, containerWidth);
            var marginColumn = GetMarginForDirection(element, LayoutDirection.Vert, containerWidth);

            // First, try to use the layout cache.
            if (CanUseCachedMeasurement(
                    widthMeasureMode,
                    availableWidth,
                    heightMeasureMode,
                    availableHeight,
                    layout.CachedLayout.WidthMeasureMode,
                    layout.CachedLayout.AvailableWidth,
                    layout.CachedLayout.HeightMeasureMode,
                    layout.CachedLayout.AvailableHeight,
                    layout.CachedLayout.ComputedWidth,
                    layout.CachedLayout.ComputedHeight,
                    marginRow,
                    marginColumn))
                cachedResults = layout.CachedLayout;
            else
            {
                // Try to use the measurement cache.
                for (var i = 0; i < layout.NextCachedMeasurementsIndex; i++)
                {
                    if (CanUseCachedMeasurement(
                            widthMeasureMode,
                            availableWidth,
                            heightMeasureMode,
                            availableHeight,
                            layout.CachedMeasurements[i].WidthMeasureMode,
                            layout.CachedMeasurements[i].AvailableWidth,
                            layout.CachedMeasurements[i].HeightMeasureMode,
                            layout.CachedMeasurements[i].AvailableHeight,
                            layout.CachedMeasurements[i].ComputedWidth,
                            layout.CachedMeasurements[i].ComputedHeight,
                            marginRow,
                            marginColumn))
                    {
                        cachedResults = layout.CachedMeasurements[i];

                        break;
                    }
                }
            }
        }
        else if (performLayout)
        {
            if (FloatsEqual(layout.CachedLayout.AvailableWidth, availableWidth)
                && FloatsEqual(layout.CachedLayout.AvailableHeight, availableHeight)
                && layout.CachedLayout.WidthMeasureMode == widthMeasureMode
                && layout.CachedLayout.HeightMeasureMode == heightMeasureMode) cachedResults = layout.CachedLayout;
        }
        else
        {
            for (var i = 0; i < layout.NextCachedMeasurementsIndex; i++)
            {
                if (FloatsEqual(
                        layout.CachedMeasurements[i].AvailableWidth,
                        availableWidth)
                    && FloatsEqual(
                        layout.CachedMeasurements[i].AvailableHeight,
                        availableHeight)
                    && layout.CachedMeasurements[i].WidthMeasureMode == widthMeasureMode
                    && layout.CachedMeasurements[i].HeightMeasureMode == heightMeasureMode)
                {
                    cachedResults = layout.CachedMeasurements[i];

                    break;
                }
            }
        }

        if (!needToVisitElement && cachedResults != null)
        {
            layout.MeasuredDimensions.Width = cachedResults.ComputedWidth;
            layout.MeasuredDimensions.Height = cachedResults.ComputedHeight;

            //if (PrintChanges && PrintSkips)
            //{
            //    Logger.Log(LogLevel.Verbose, $"{Spacer(depth)}{depth}.([skipped] ");
            //    Logger.Log(LogLevel.Verbose, new ElementPrint(element).ToString());
            //    Logger.Log(LogLevel.Verbose,
            //               $"wm: {MeasureModeName(widthMeasureMode, performLayout)}, hm: {MeasureModeName(heightMeasureMode, performLayout)}, aw: {availableWidth} ah: {availableHeight} => d: ({cachedResults.ComputedWidth}, {cachedResults.ComputedHeight}) {reason}\n");
            //}
        }
        else
        {
            //if (PrintChanges)
            //{
            //    Logger.Log(LogLevel.Verbose, $"{Spacer(depth)}{depth}.({(needToVisitElement ? "*" : "")}");
            //    Logger.Log(LogLevel.Verbose, new ElementPrint(element).ToString());
            //    Logger.Log(LogLevel.Verbose,
            //               $"wm: {MeasureModeName(widthMeasureMode, performLayout)}, hm: {MeasureModeName(heightMeasureMode, performLayout)}, aw: {availableWidth} ah: {availableHeight} {reason}\n");
            //}

            LayoutImplementation(
                element,
                availableWidth,
                availableHeight,
                widthMeasureMode,
                heightMeasureMode,
                containerWidth,
                containerHeight,
                performLayout,
                depth,
                generationCount);

            //if (PrintChanges)
            //{
            //    Logger.Log(LogLevel.Verbose, $"{Spacer(depth)}{depth}.){(needToVisitElement ? "*" : "")}");
            //    Logger.Log(LogLevel.Verbose, new ElementPrint(element).ToString());
            //    Logger.Log(LogLevel.Verbose,
            //               $"wm: {MeasureModeName(widthMeasureMode, performLayout)}, hm: {MeasureModeName(heightMeasureMode, performLayout)}, d: ({layout.MeasuredDimensions.Width}, {layout.MeasuredDimensions.Height}) {reason}\n");
            //}

            if (cachedResults == null)
            {
                if (layout.NextCachedMeasurementsIndex >= CachedMeasurement.MaxCachedResults)
                {
                    //if (PrintChanges) 
                    //    Logger.Log(LogLevel.Verbose, "Out of cache entries!\n");

                    layout.NextCachedMeasurementsIndex = 0;
                }

                CachedMeasurement newCacheEntry;
                if (performLayout)
                {
                    // Use the single layout cache entry.
                    newCacheEntry = layout.CachedLayout;
                }
                else
                {
                    // Allocate a new measurement cache entry.
                    newCacheEntry = layout.CachedMeasurements[layout.NextCachedMeasurementsIndex];
                    layout.NextCachedMeasurementsIndex++;
                }

                newCacheEntry.AvailableWidth = availableWidth;
                newCacheEntry.AvailableHeight = availableHeight;
                newCacheEntry.WidthMeasureMode = widthMeasureMode;
                newCacheEntry.HeightMeasureMode = heightMeasureMode;
                newCacheEntry.ComputedWidth = layout.MeasuredDimensions.Width;
                newCacheEntry.ComputedHeight = layout.MeasuredDimensions.Height;
            }
        }

        if (performLayout)
        {
            element.Layout.Width = element.Layout.MeasuredDimensions.Width;
            element.Layout.Height = element.Layout.MeasuredDimensions.Height;
            element.IsDirty = false;
        }

        layout.GenerationCount = generationCount;

        return needToVisitElement || cachedResults == null;
    }

    /// <summary>
    /// Partial implementation of FlexBox https://www.w3.org/TR/CSS3-flexbox/.
    ///
    /// Limitations
    ///  * Display property is always assumed to be 'flex' except for Text elements, which are assumed to be 'inline-flex'.
    ///  * The 'zIndex' property (or any form of z ordering) is not supported. Elements are stacked in document order.
    ///  * The 'order' property is not supported. The order of flex items is always defined by document order.
    ///  * The 'visibility' property is always assumed to be 'visible'. Values of 'collapse' and 'hidden' are not supported.
    ///  * There is no support for forced breaks.
    ///  * It does not support vertical inline directions (top-to-bottom or bottom-to-top text).
    ///
    /// Deviations 
    ///  * Section 4.5 of the spec indicates that all flex items have a default minimum layout-direction size. For text blocks,
    ///    for example, this is the width of the widest word. Calculating the minimum width is expensive, so we assume a default minimum layout-direction size of 0.
    ///  * Min/Max sizes in the layout-direction direction are not honored when resolving flexible lengths.
    ///  * The spec indicates that the default value for 'flexDirection' is 'row', but the algorithm below assumes a
    ///    default of 'column'.
    ///
    /// Details
    ///    This routine is called recursively to lay out subtrees of flexbox
    ///    elements. It uses the information in element.style, which is treated as a
    ///    read-only input. It is responsible for setting the layout.direction and
    ///    layout.measuredDimensions fields for the input element as well as the
    ///    layout.position and layout.lineIndex fields for its elements. The
    ///    layout.measuredDimensions field includes any padding for the
    ///    element but does not include margins.
    ///
    ///    The spec describes four different layout modes: "fill available", "max
    ///    content", "min content", and "fit content". Of these, we don't use "min
    ///    content" because we don't support default minimum layout-direction sizes (see above
    ///    for details). Each of our measure modes maps to a layout mode from the
    ///    spec (https://www.w3.org/TR/CSS3-sizing/#terms):
    ///      - MeasureMode.Undefined: max content
    ///      - MeasureMode.Exactly: fill available
    ///      - MeasureMode.AtMost: fit content
    ///
    ///    When calling LayoutImplementation and LayoutElement, if the caller
    ///    passes an available size of undefined then it must also pass a measure
    ///    mode of MeasureMode.Undefined in that dimension.
    ///
    /// </summary>
    /// <param name="element">current element to be sized and laid out</param>
    /// <param name="availableWidth">available width to be used for sizing</param>
    /// <param name="availableHeight">available width to be used for sizing</param>
    /// <param name="widthMeasureMode">indicates the sizing rules for the width</param>
    /// <param name="heightMeasureMode">indicates the sizing rules for the height</param>
    /// <param name="containerWidth"></param>
    /// <param name="containerHeight"></param>
    /// <param name="performLayout">specifies whether the caller is interested in just the dimensions of the element or it requires the entire element and its subtree to be laid out (with final positions)</param>
    /// <param name="depth"></param>
    /// <param name="generationCount"></param>
    private static void LayoutImplementation(
        IElement element,
        float availableWidth,
        float availableHeight,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        float containerWidth,
        float containerHeight,
        bool performLayout,
        int depth,
        int generationCount)
    {
        Debug.Assert(
            availableWidth.HasValue() || widthMeasureMode == MeasureMode.Undefined,
            "availableWidth is indefinite so widthMeasureMode must be MeasureMode.Undefined");
        Debug.Assert(
            availableHeight.HasValue() || heightMeasureMode == MeasureMode.Undefined,
            "availableHeight is indefinite so heightMeasureMode must be MeasureMode.Undefined");

        // Set the resolved resolution in the element's layout.
        var flexRowDirection = LayoutDirection.Horz;
        var flexColumnDirection = LayoutDirection.Vert;

        var startEdge = Side.Left;
        var endEdge = Side.Right;

        SetLayoutMargin(element, GetLeadingMargin(element, flexRowDirection, containerWidth), startEdge);
        SetLayoutMargin(element, GetTrailingMargin(element, flexRowDirection, containerWidth), endEdge);
        SetLayoutMargin(element, GetLeadingMargin(element, flexColumnDirection, containerWidth), Side.Top);
        SetLayoutMargin(element, GetTrailingMargin(element, flexColumnDirection, containerWidth), Side.Bottom);

        SetLayoutPadding(element, GetLeadingPadding(element, flexRowDirection, containerWidth), startEdge);
        SetLayoutPadding(element, GetTrailingPadding(element, flexRowDirection, containerWidth), endEdge);
        SetLayoutPadding(element, GetLeadingPadding(element, flexColumnDirection, containerWidth), Side.Top);
        SetLayoutPadding(element, GetTrailingPadding(element, flexColumnDirection, containerWidth), Side.Bottom);

        if (element.MeasureFunc != null)
        {
            SetMeasuredDimensions_MeasureFunc(
                element,
                availableWidth,
                availableHeight,
                widthMeasureMode,
                heightMeasureMode,
                containerWidth,
                containerHeight);

            return;
        }

        var elementCount = element.Children.Count;
        if (elementCount == 0)
        {
            SetMeasuredDimensions_EmptyContainer(
                element,
                availableWidth,
                availableHeight,
                widthMeasureMode,
                heightMeasureMode,
                containerWidth,
                containerHeight);

            return;
        }

        // If we're not being asked to perform a full layout we can skip the algorithm
        // if we already know the size
        if (!performLayout
            && SetMeasuredDimensions_FixedSize(
                element,
                availableWidth,
                availableHeight,
                widthMeasureMode,
                heightMeasureMode,
                containerWidth,
                containerHeight)) return;

        // At this point we know we're going to perform work. Ensure that each subElement
        // has a mutable copy.
        //element.cloneElementsIfNeeded();

        // Reset layout flags, as they could have changed.
        element.Layout.HadOverflow = false;

        // STEP 1: CALCULATE VALUES FOR REMAINDER OF ALGORITHM
        var layoutDirection = element.Plan.ElementsDirection;
        var crossDirection = ResolveCrossDirection(layoutDirection);
        var isLayoutDirectionRow = IsHorz(layoutDirection);
        var isLayoutWrap = element.Plan.ElementsWrap != LayoutWrap.NoWrap;

        var layoutDirectionContainerSize = isLayoutDirectionRow
            ? containerWidth
            : containerHeight;
        var crossDirectionContainerSize = isLayoutDirectionRow
            ? containerHeight
            : containerWidth;

        var leadingPaddingCross = GetLeadingPadding(element, crossDirection, containerWidth);
        var paddingLayoutDirection = PaddingForDirection(element, layoutDirection, containerWidth);
        var paddingCross = PaddingForDirection(element, crossDirection, containerWidth);

        var measureModeLayoutDirectionDim = isLayoutDirectionRow
            ? widthMeasureMode
            : heightMeasureMode;
        var measureModeCrossDim = isLayoutDirectionRow
            ? heightMeasureMode
            : widthMeasureMode;

        var paddingRow = isLayoutDirectionRow
            ? paddingLayoutDirection
            : paddingCross;
        var paddingColumn = isLayoutDirectionRow
            ? paddingCross
            : paddingLayoutDirection;

        var marginRow = GetMarginForDirection(element, LayoutDirection.Horz, containerWidth);
        var marginColumn = GetMarginForDirection(element, LayoutDirection.Vert, containerWidth);

        var minInnerWidth = element.Plan.MinWidth.Resolve(containerWidth) - paddingRow;
        var maxInnerWidth = element.Plan.MaxWidth.Resolve(containerWidth) - paddingRow;
        var minInnerHeight = element.Plan.MinHeight.Resolve(containerHeight) - paddingColumn;
        var maxInnerHeight = element.Plan.MaxHeight.Resolve(containerHeight) - paddingColumn;

        var minInnerLayoutDirectionDim = isLayoutDirectionRow
            ? minInnerWidth
            : minInnerHeight;
        var maxInnerLayoutDirectionDim = isLayoutDirectionRow
            ? maxInnerWidth
            : maxInnerHeight;

        // STEP 2: DETERMINE AVAILABLE SIZE IN LAYOUT-DIRECTION AND CROSS-DIRECTION

        var availableInnerWidth = CalculateAvailableInnerDim(
            element,
            LayoutDirection.Horz,
            availableWidth,
            containerWidth);
        var availableInnerHeight = CalculateAvailableInnerDim(
            element,
            LayoutDirection.Vert,
            availableHeight,
            containerHeight);

        var availableInnerLayoutDirectionDim = isLayoutDirectionRow
            ? availableInnerWidth
            : availableInnerHeight;
        var availableInnerCrossDim = isLayoutDirectionRow
            ? availableInnerHeight
            : availableInnerWidth;

        // STEP 3: DETERMINE LAYOUT-DIRECTION LENGTH FOR EACH ITEM

        var totalOuterLayoutDirectionLength = ComputeLayoutDirectionLengthForElements(
            element,
            availableInnerWidth,
            availableInnerHeight,
            widthMeasureMode,
            heightMeasureMode,
            layoutDirection,
            performLayout,
            depth,
            generationCount);

        var layoutDirectionLengthOverflows =
            measureModeLayoutDirectionDim != MeasureMode.Undefined && totalOuterLayoutDirectionLength > availableInnerLayoutDirectionDim;
        if (isLayoutWrap && layoutDirectionLengthOverflows && measureModeLayoutDirectionDim == MeasureMode.AtMost)
            measureModeLayoutDirectionDim = MeasureMode.Exactly;

        // STEP 4: COLLECT FLEX ITEMS INTO FLEX LINES

        // Indexes of elements that represent the first and last items in the line.
        var startOfLineIndex = 0;
        var endOfLineIndex = 0;

        // Number of lines.
        var lineCount = 0;

        // Accumulated cross dimensions of all lines so far.
        float totalLineCrossDim = 0;

        // Max layout-direction dimension of all the lines.
        float maxLineLayoutDirectionDim = 0;
        for (;
             endOfLineIndex < elementCount;
             lineCount++, startOfLineIndex = endOfLineIndex)
        {
            var collectedFlexItemsValues = CalculateCollectFlexItemsRowValues(
                element,
                layoutDirectionContainerSize,
                availableInnerWidth,
                availableInnerLayoutDirectionDim,
                startOfLineIndex,
                lineCount);
            endOfLineIndex = collectedFlexItemsValues.EndOfLineIndex;

            // If we don't need to measure the cross direction, we can skip the entire flex step.
            var canSkipFlex =
                !performLayout && measureModeCrossDim == MeasureMode.Exactly;

            // STEP 5: RESOLVING FLEXIBLE LENGTHS ON LAYOUT-DIRECTION DIRECTION
            // Calculate the remaining available space that needs to be allocated. If
            // the layout-direction dimension size isn't known, it is computed based on the line
            // length, so there's no more space left to distribute.

            var sizeBasedOnContent = false;
            // If we don't measure with exact layout-direction dimension we want to ensure we don't
            // violate min and max
            if (measureModeLayoutDirectionDim != MeasureMode.Exactly)
            {
                if (minInnerLayoutDirectionDim.HasValue()
                    && collectedFlexItemsValues.SizeConsumedOnCurrentLine < minInnerLayoutDirectionDim)
                    availableInnerLayoutDirectionDim = minInnerLayoutDirectionDim;
                else if (
                    maxInnerLayoutDirectionDim.HasValue()
                    && collectedFlexItemsValues.SizeConsumedOnCurrentLine > maxInnerLayoutDirectionDim)
                    availableInnerLayoutDirectionDim = maxInnerLayoutDirectionDim;
                else
                {
                    if (collectedFlexItemsValues.TotalFlexGrowFactors.IsUndefined()
                        && collectedFlexItemsValues.TotalFlexGrowFactors.IsZero()
                        || ResolveFlexGrow(element).IsUndefined() && ResolveFlexGrow(element).IsZero())
                    {
                        // If we don't have any elements to flex or we can't flex the element
                        // itself, space we've used is all space we need. Root element also
                        // should be shrunk to minimum
                        availableInnerLayoutDirectionDim = collectedFlexItemsValues.SizeConsumedOnCurrentLine;
                    }

                    sizeBasedOnContent = true;
                }
            }

            if (!sizeBasedOnContent && availableInnerLayoutDirectionDim.HasValue())
            {
                collectedFlexItemsValues.RemainingFreeSpace =
                    availableInnerLayoutDirectionDim - collectedFlexItemsValues.SizeConsumedOnCurrentLine;
            }
            else if (collectedFlexItemsValues.SizeConsumedOnCurrentLine < 0)
            {
                // availableInnerLayoutDirectionDim is indefinite which means the element is being sized
                // based on its content. sizeConsumedOnCurrentLine is negative which means
                // the element will allocate 0 points for its content. Consequently,
                // remainingFreeSpace is 0 - sizeConsumedOnCurrentLine.
                collectedFlexItemsValues.RemainingFreeSpace = -collectedFlexItemsValues.SizeConsumedOnCurrentLine;
            }

            if (!canSkipFlex)
            {
                ResolveFlexibleLength(
                    element,
                    collectedFlexItemsValues,
                    layoutDirection,
                    crossDirection,
                    layoutDirectionContainerSize,
                    availableInnerLayoutDirectionDim,
                    availableInnerCrossDim,
                    availableInnerWidth,
                    availableInnerHeight,
                    layoutDirectionLengthOverflows,
                    measureModeCrossDim,
                    performLayout,
                    depth,
                    generationCount);
            }

            element.Layout.HadOverflow = element.Layout.HadOverflow | (collectedFlexItemsValues.RemainingFreeSpace < 0);

            // STEP 6: LAYOUT-DIRECTION JUSTIFICATION & CROSS-DIRECTION SIZE DETERMINATION

            // At this point, all the elements have their dimensions set in the layout-direction.
            // Their dimensions are also set in the cross direction with the exception
            // of items that are aligned "stretch". We need to compute these stretch
            // values and set the final positions.

            JustifyLayoutDirection(
                element,
                collectedFlexItemsValues,
                startOfLineIndex,
                layoutDirection,
                crossDirection,
                measureModeLayoutDirectionDim,
                measureModeCrossDim,
                layoutDirectionContainerSize,
                containerWidth,
                availableInnerLayoutDirectionDim,
                availableInnerCrossDim,
                availableInnerWidth,
                performLayout);

            var containerCrossDirection = availableInnerCrossDim;
            if (measureModeCrossDim == MeasureMode.Undefined || measureModeCrossDim == MeasureMode.AtMost)
            {
                // Compute the cross direction from the max cross dimension of the elements
                containerCrossDirection =
                    BoundDirection(
                        element,
                        crossDirection,
                        collectedFlexItemsValues.CrossDim + paddingCross,
                        crossDirectionContainerSize,
                        containerWidth)
                    - paddingCross;
            }

            // If there's no flex wrap, the cross dimension is defined by the container.
            if (!isLayoutWrap && measureModeCrossDim == MeasureMode.Exactly)
                collectedFlexItemsValues.CrossDim = availableInnerCrossDim;

            // Clamp to the min/max size specified on the container.
            collectedFlexItemsValues.CrossDim =
                BoundDirection(
                    element,
                    crossDirection,
                    collectedFlexItemsValues.CrossDim + paddingCross,
                    crossDirectionContainerSize,
                    containerWidth)
                - paddingCross;

            // STEP 7: CROSS-DIRECTION ALIGNMENT
            // We can skip subElement alignment if we're just measuring the container.
            if (performLayout)
            {
                for (var i = startOfLineIndex; i < endOfLineIndex; i++)
                {
                    var subElement = (IElement)element.Children[i];

                    if (subElement.Plan.Atomic)
                        continue;

                    if (subElement.Plan.PositionType == PositionType.Absolute)
                    {
                        // If the subElement is absolutely positioned and has a
                        // top/left/bottom/right set, override all the previously computed
                        // positions to set it correctly.
                        var isElementLeadingPosDefined = IsLeadingPositionDefined(subElement, crossDirection);
                        if (isElementLeadingPosDefined)
                        {
                            subElement.Layout.Position[crossDirection.ToPosition()] =
                                GetLeadingPosition(subElement, crossDirection, availableInnerCrossDim)
                                + GetLeadingMargin(subElement, crossDirection, availableInnerWidth);
                        }

                        // If leading position is not defined or calculations result in Nan,
                        // default to margin
                        if (!isElementLeadingPosDefined
                            || subElement.Layout.Position[crossDirection.ToPosition()]
                                      .IsUndefined())
                        {
                            subElement.Layout.Position[crossDirection.ToPosition()] = GetLeadingMargin(subElement, crossDirection, availableInnerWidth);
                        }
                    }
                    else
                    {
                        var leadingCrossDim = leadingPaddingCross;

                        // For a relative elements, we're either using alignElements (container) or
                        // alignSelf (subElement) in order to determine the position in the cross direction
                        var alignItem = AlignElement(element, subElement);

                        // If the subElement uses align stretch, we need to lay it out one more
                        // time, this time forcing the cross-direction size to be the computed
                        // cross size for the current line.
                        if (alignItem == AlignmentLine_Cross.Stretch
                            && MarginLeadingValue(subElement, crossDirection).Unit != Number.UoM.Auto
                            && MarginTrailingValue(subElement, crossDirection).Unit != Number.UoM.Auto)
                        {
                            // If the subElement defines a definite size for its cross direction, there's
                            // no need to stretch.
                            if (!IsLayoutDirectionInPlan(subElement, crossDirection, availableInnerCrossDim))
                            {
                                var elementLayoutDirectionLength = subElement.Layout.MeasuredDimensions[layoutDirection.ToDimension()];
                                var elementCrossSize = subElement.Plan.AspectRatio.HasValue()
                                    ? GetMarginForDirection(subElement, crossDirection, availableInnerWidth)
                                      + (isLayoutDirectionRow
                                          ? elementLayoutDirectionLength / subElement.Plan.AspectRatio
                                          : elementLayoutDirectionLength * subElement.Plan.AspectRatio)
                                    : collectedFlexItemsValues.CrossDim;

                                elementLayoutDirectionLength += GetMarginForDirection(subElement, layoutDirection, availableInnerWidth);

                                var elementLayoutDirectionMeasureMode = MeasureMode.Exactly;
                                var elementCrossMeasureMode = MeasureMode.Exactly;
                                ConstrainMaxSizeForMode(
                                    subElement,
                                    layoutDirection,
                                    availableInnerLayoutDirectionDim,
                                    availableInnerWidth,
                                    ref elementLayoutDirectionMeasureMode,
                                    ref elementLayoutDirectionLength);
                                ConstrainMaxSizeForMode(
                                    subElement,
                                    crossDirection,
                                    availableInnerCrossDim,
                                    availableInnerWidth,
                                    ref elementCrossMeasureMode,
                                    ref elementCrossSize);

                                var elementWidth = isLayoutDirectionRow
                                    ? elementLayoutDirectionLength
                                    : elementCrossSize;
                                var elementHeight = !isLayoutDirectionRow
                                    ? elementLayoutDirectionLength
                                    : elementCrossSize;

                                var crossDirectionDoesNotGrow = element.Plan.AlignElements_Cross != Alignment_Cross.Stretch && isLayoutWrap;
                                var elementWidthMeasureMode =
                                    elementWidth.IsUndefined() || !isLayoutDirectionRow && crossDirectionDoesNotGrow
                                        ? MeasureMode.Undefined
                                        : MeasureMode.Exactly;
                                var elementHeightMeasureMode =
                                    elementHeight.IsUndefined() || isLayoutDirectionRow && crossDirectionDoesNotGrow
                                        ? MeasureMode.Undefined
                                        : MeasureMode.Exactly;

                                LayoutElement(
                                    subElement,
                                    elementWidth,
                                    elementHeight,
                                    elementWidthMeasureMode,
                                    elementHeightMeasureMode,
                                    availableInnerWidth,
                                    availableInnerHeight,
                                    true,
                                    LayoutPassReason.Stretch,
                                    depth,
                                    generationCount);
                            }
                        }
                        else
                        {
                            var remainingCrossDim = containerCrossDirection - DimWithMargin(
                                subElement,
                                crossDirection,
                                availableInnerWidth);

                            if (MarginLeadingValue(subElement, crossDirection).Unit == Number.UoM.Auto &&
                                MarginTrailingValue(subElement, crossDirection).Unit == Number.UoM.Auto)
                                leadingCrossDim += FloatMax(0.0f, remainingCrossDim / 2);
                            else if (MarginTrailingValue(subElement, crossDirection).Unit == Number.UoM.Auto)
                            {
                                // No-Op
                            }
                            else if (MarginLeadingValue(subElement, crossDirection).Unit == Number.UoM.Auto)
                                leadingCrossDim += FloatMax(0.0f, remainingCrossDim);
                            else if (alignItem == AlignmentLine_Cross.Start)
                            {
                                // No-Op
                            }
                            else if (alignItem == AlignmentLine_Cross.Center)
                                leadingCrossDim += remainingCrossDim / 2;
                            else
                                leadingCrossDim += remainingCrossDim;
                        }

                        // And we apply the position
                        subElement.Layout.Position[crossDirection.ToPosition()] = subElement.Layout.Position[crossDirection.ToPosition()]
                                                                       + totalLineCrossDim
                                                                       + leadingCrossDim;
                    }
                }
            }

            totalLineCrossDim += collectedFlexItemsValues.CrossDim;
            maxLineLayoutDirectionDim = FloatMax(maxLineLayoutDirectionDim, collectedFlexItemsValues.LayoutDirectionDim);
        }

        // STEP 8: MULTI-LINE CONTENT ALIGNMENT
        // currentLead stores the size of the cross dim
        if (performLayout && isLayoutWrap)
        {
            float crossDimLead = 0;
            var currentLead = leadingPaddingCross;
            if (availableInnerCrossDim.HasValue())
            {
                var remainingAlignContentDim = availableInnerCrossDim - totalLineCrossDim;
                switch (element.Plan.AlignElements_Cross)
                {
                    case Alignment_Cross.End:
                        currentLead += remainingAlignContentDim;

                        break;
                    case Alignment_Cross.Center:
                        currentLead += remainingAlignContentDim / 2;

                        break;
                    case Alignment_Cross.Stretch:
                        if (availableInnerCrossDim > totalLineCrossDim)
                            crossDimLead = remainingAlignContentDim / lineCount;

                        break;
                    case Alignment_Cross.SpaceAround:
                        if (availableInnerCrossDim > totalLineCrossDim)
                        {
                            currentLead += remainingAlignContentDim / (2 * lineCount);
                            if (lineCount > 1)
                                crossDimLead = remainingAlignContentDim / lineCount;
                        }
                        else
                            currentLead += remainingAlignContentDim / 2;

                        break;
                    case Alignment_Cross.SpaceBetween:
                        if (availableInnerCrossDim > totalLineCrossDim && lineCount > 1)
                            crossDimLead = remainingAlignContentDim / (lineCount - 1);

                        break;
                    case Alignment_Cross.Auto:
                    case Alignment_Cross.Start:
                        break;
                }
            }

            var endIndex = 0;
            for (var i = 0; i < lineCount; i++)
            {
                var startIndex = endIndex;
                int ii;

                // compute the line's height and find the endIndex
                float lineHeight = 0;
                for (ii = startIndex; ii < elementCount; ii++)
                {
                    var subElement = (IElement)element.Children[ii];

                    if (subElement.Plan.Atomic)
                        continue;

                    if (subElement.Plan.PositionType == PositionType.Relative)
                    {
                        if (subElement.Layout.LineIndex != i)
                            break;

                        if (IsLayoutDimDefined(subElement, crossDirection))
                        {
                            lineHeight = FloatMax(
                                lineHeight,
                                subElement.Layout.MeasuredDimensions[crossDirection.ToDimension()]
                                + GetMarginForDirection(subElement, crossDirection, availableInnerWidth));
                        }

                    }
                }

                endIndex = ii;
                lineHeight += crossDimLead;

                if (performLayout)
                {
                    for (ii = startIndex; ii < endIndex; ii++)
                    {
                        var subElement = (IElement)element.Children[ii];

                        if (subElement.Plan.Atomic)
                            continue;

                        if (subElement.Plan.PositionType == PositionType.Relative)
                        {
                            switch (AlignElement(element, subElement))
                            {
                                case AlignmentLine_Cross.Start:
                                    {
                                        subElement.Layout.Position[crossDirection.ToPosition()] =
                                            currentLead + GetLeadingMargin(subElement, crossDirection, availableInnerWidth);

                                        break;
                                    }
                                case AlignmentLine_Cross.End:
                                    {
                                        subElement.Layout.Position[crossDirection.ToPosition()] =
                                            currentLead
                                            + lineHeight
                                            - GetTrailingMargin(subElement, crossDirection, availableInnerWidth)
                                            - subElement.Layout.MeasuredDimensions[crossDirection.ToDimension()];

                                        break;
                                    }
                                case AlignmentLine_Cross.Center:
                                    {
                                        var elementHeight = subElement.Layout.MeasuredDimensions[crossDirection.ToDimension()];

                                        subElement.Layout.Position[crossDirection.ToPosition()] =
                                            currentLead + (lineHeight - elementHeight) / 2;

                                        break;
                                    }
                                case AlignmentLine_Cross.Stretch:
                                    {
                                        subElement.Layout.Position[crossDirection.ToPosition()] =
                                            currentLead + GetLeadingMargin(subElement, crossDirection, availableInnerWidth);

                                        // Remeasure subElement with the line height as it as been only
                                        // measured with the containers height yet.
                                        if (!IsLayoutDirectionInPlan(subElement, crossDirection, availableInnerCrossDim))
                                        {
                                            var elementWidth = isLayoutDirectionRow
                                                ? subElement.Layout.MeasuredDimensions.Width + GetMarginForDirection(
                                                    subElement,
                                                    layoutDirection,
                                                    availableInnerWidth)
                                                : lineHeight;

                                            var elementHeight = !isLayoutDirectionRow
                                                ? subElement.Layout.MeasuredDimensions.Height + GetMarginForDirection(
                                                    subElement,
                                                    crossDirection,
                                                    availableInnerWidth)
                                                : lineHeight;

                                            if (!(FloatsEqual(elementWidth, subElement.Layout.MeasuredDimensions.Width)
                                                  && FloatsEqual(elementHeight, subElement.Layout.MeasuredDimensions.Height)))
                                            {
                                                LayoutElement(
                                                    subElement,
                                                    elementWidth,
                                                    elementHeight,
                                                    MeasureMode.Exactly,
                                                    MeasureMode.Exactly,
                                                    availableInnerWidth,
                                                    availableInnerHeight,
                                                    true,
                                                    LayoutPassReason.MultilineStretch,
                                                    depth,
                                                    generationCount);
                                            }
                                        }

                                        break;
                                    }
                                case AlignmentLine_Cross.Auto:
                                    break;
                            }
                        }
                    }
                }

                currentLead += lineHeight;
            }
        }

        // STEP 9: COMPUTING FINAL DIMENSIONS

        SetLayoutMeasuredDimension(
            element,
            BoundDirection(
                element,
                LayoutDirection.Horz,
                availableWidth - marginRow,
                containerWidth,
                containerWidth),
            (int)Dimension.Width);

        SetLayoutMeasuredDimension(
            element,
            BoundDirection(
                element,
                LayoutDirection.Vert,
                availableHeight - marginColumn,
                containerHeight,
                containerWidth),
            Dimension.Height);

        // If the user didn't specify a width or height for the element, set the dimensions based on the elements
        if (measureModeLayoutDirectionDim == MeasureMode.Undefined
            || element.Plan.Overflow != Overflow.Scroll && measureModeLayoutDirectionDim == MeasureMode.AtMost)
        {
            // Clamp the size to the min/max size, if specified, and make sure it
            // doesn't go below the padding amount.
            SetLayoutMeasuredDimension(
                element,
                BoundDirection(
                    element,
                    layoutDirection,
                    maxLineLayoutDirectionDim,
                    layoutDirectionContainerSize,
                    containerWidth),
                layoutDirection.ToDimension());
        }
        else if (measureModeLayoutDirectionDim == MeasureMode.AtMost && element.Plan.Overflow == Overflow.Scroll)
        {
            SetLayoutMeasuredDimension(
                element,
                FloatMax(
                    FloatMin(
                        availableInnerLayoutDirectionDim + paddingLayoutDirection,
                        BoundDirectionWithinMinAndMax(
                            element,
                            layoutDirection,
                            maxLineLayoutDirectionDim,
                            layoutDirectionContainerSize)
                    ),
                    paddingLayoutDirection),
                layoutDirection.ToDimension());
        }

        if (measureModeCrossDim == MeasureMode.Undefined
            || element.Plan.Overflow != Overflow.Scroll && measureModeCrossDim == MeasureMode.AtMost)
        {
            // Clamp the size to the min/max size, if specified, and make sure it
            // doesn't go below the padding amount.
            SetLayoutMeasuredDimension(
                element,
                BoundDirection(
                    element,
                    crossDirection,
                    totalLineCrossDim + paddingCross,
                    crossDirectionContainerSize,
                    containerWidth),
                crossDirection.ToDimension());
        }
        else if (measureModeCrossDim == MeasureMode.AtMost && element.Plan.Overflow == Overflow.Scroll)
        {
            SetLayoutMeasuredDimension(
                element,
                FloatMax(
                    FloatMin(
                        availableInnerCrossDim + paddingCross,
                        BoundDirectionWithinMinAndMax(
                            element,
                            crossDirection,
                            totalLineCrossDim + paddingCross,
                            crossDirectionContainerSize)
                    ),
                    paddingCross),
                crossDirection.ToDimension());
        }

        if (performLayout)
        {
            // STEP 10: SIZING AND POSITIONING ABSOLUTE ELEMENTS
            foreach (IElement subElement in element.Children)
            {
                if (subElement.Plan.PositionType != PositionType.Absolute) continue;

                AbsoluteLayoutElement(
                    element,
                    subElement,
                    availableInnerWidth,
                    isLayoutDirectionRow
                        ? measureModeLayoutDirectionDim
                        : measureModeCrossDim,
                    availableInnerHeight,
                    depth,
                    generationCount);
            }
        }
    }

    private static bool IsHorz(LayoutDirection layoutDirection) => layoutDirection == LayoutDirection.Horz;
    private static bool IsVert(LayoutDirection layoutDirection) => layoutDirection == LayoutDirection.Vert;

    private static LayoutDirection ResolveCrossDirection(LayoutDirection layoutDirection) =>
        IsVert(layoutDirection)
            ? LayoutDirection.Horz
            : LayoutDirection.Vert;

    private static float ComputeLayoutDirectionLengthForElements(
        IElement element,
        float availableInnerWidth,
        float availableInnerHeight,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        LayoutDirection layoutDirection,
        bool performLayout,
        int depth,
        int generationCount)
    {
        var totalOuterLayoutDirectionLength = 0.0f;
        IElement? singleFlexElement = null;
        var subElements = new List<IElement>(element.Children);
        var measureModeLayoutDirectionDim = IsHorz(layoutDirection)
            ? widthMeasureMode
            : heightMeasureMode;

        // If there is only one subElement with flexGrow + flexShrink it means we can set
        // the computedLayoutDirectionLength to 0 instead of measuring and shrinking / flexing the
        // subElement to exactly match the remaining space
        if (measureModeLayoutDirectionDim == MeasureMode.Exactly)
        {
            foreach (var subElement in subElements)
            {
                if (IsElementFlexible(subElement))
                {
                    if (singleFlexElement != null
                        || FloatsEqual(ResolveFlexGrow(subElement), 0.0f)
                        || FloatsEqual(ResolveFlexShrink(subElement), 0.0f))
                    {
                        // There is already a flexible subElement, or this flexible subElement doesn't
                        // have flexGrow and flexShrink, abort
                        singleFlexElement = null;

                        break;
                    }

                    singleFlexElement = subElement;
                }
            }
        }

        foreach (var subElement in subElements)
        {
            ResolveDimension(subElement);
            if (subElement.Plan.Atomic)
            {
                ZeroOutLayoutRecursively(subElement);
                subElement.IsDirty = false;

                continue;
            }

            if (performLayout)
            {
                // Set the initial position (relative to the container).
                var layoutDirectionDim = IsHorz(layoutDirection)
                    ? availableInnerWidth
                    : availableInnerHeight;
                var crossDim = IsHorz(layoutDirection)
                    ? availableInnerHeight
                    : availableInnerWidth;
                SetPosition(
                    subElement,
                    layoutDirectionDim,
                    crossDim,
                    availableInnerWidth);
            }

            if (subElement.Plan.PositionType == PositionType.Absolute) continue;

            if (subElement == singleFlexElement)
            {
                subElement.Layout.ComputedLayoutDirectionLengthGeneration = generationCount;
                subElement.Layout.ComputedLayoutDirectionLength = 0f;
            }
            else
            {
                ComputeLayoutDirectionLengthForElement(
                    element,
                    subElement,
                    availableInnerWidth,
                    widthMeasureMode,
                    availableInnerHeight,
                    availableInnerWidth,
                    availableInnerHeight,
                    heightMeasureMode,
                    depth,
                    generationCount);
            }

            totalOuterLayoutDirectionLength +=
                subElement.Layout.ComputedLayoutDirectionLength + GetMarginForDirection(subElement, layoutDirection, availableInnerWidth);
        }

        return totalOuterLayoutDirectionLength;
    }

    private static void ComputeLayoutDirectionLengthForElement(
        IElement element,
        IElement subElement,
        float width,
        MeasureMode widthMode,
        float height,
        float containerWidth,
        float containerHeight,
        MeasureMode heightMode,
        int depth,
        int generationCount)
    {
        var layoutDirection = element.Plan.ElementsDirection;
        var isLayoutDirectionRow = IsHorz(layoutDirection);
        var layoutDirectionLength = isLayoutDirectionRow
            ? width
            : height;
        var layoutDirectionContainerLength = isLayoutDirectionRow
            ? containerWidth
            : containerHeight;

        var resolvedLayoutDirectionLength = ResolveLayoutDirectionLength(subElement.Plan).Resolve(layoutDirectionContainerLength);
        var isRowStyleDimDefined = IsLayoutDirectionInPlan(subElement, LayoutDirection.Horz, containerWidth);
        var isColumnStyleDimDefined = IsLayoutDirectionInPlan(subElement, LayoutDirection.Vert, containerHeight);

        if (resolvedLayoutDirectionLength.HasValue() && layoutDirectionLength.HasValue())
        {
            if (subElement.Layout.ComputedLayoutDirectionLength.IsUndefined()
                && subElement.Layout.ComputedLayoutDirectionLengthGeneration != generationCount)
            {
                var padding = PaddingForDirection(subElement, layoutDirection, containerWidth);
                subElement.Layout.ComputedLayoutDirectionLength = Math.Max(resolvedLayoutDirectionLength, padding);
            }
        }
        else if (isLayoutDirectionRow && isRowStyleDimDefined)
        {
            // The width is definite, so use that as the layout-direction length.
            var padding = PaddingForDirection(subElement, LayoutDirection.Horz, containerWidth);

            subElement.Layout.ComputedLayoutDirectionLength = Math.Max(
                subElement.Layout.ResolvedWidth.Resolve(containerWidth),
                padding);
        }
        else if (!isLayoutDirectionRow && isColumnStyleDimDefined)
        {
            // The height is definite, so use that as the layout-direction length.
            var padding = PaddingForDirection(subElement, LayoutDirection.Vert, containerWidth);
            subElement.Layout.ComputedLayoutDirectionLength = Math.Max(
                subElement.Layout.ResolvedHeight.Resolve(containerHeight),
                padding);
        }
        else
        {
            // Compute the layout-direction length and hypothetical layout direction length (i.e. the clamped layout directionlength).
            var elementWidth = Number.ValueUndefined;
            var elementHeight = Number.ValueUndefined;
            var elementWidthMeasureMode = MeasureMode.Undefined;
            var elementHeightMeasureMode = MeasureMode.Undefined;

            var marginRow = GetMarginForDirection(subElement, LayoutDirection.Horz, containerWidth);
            var marginColumn = GetMarginForDirection(subElement, LayoutDirection.Vert, containerWidth);

            if (isRowStyleDimDefined)
            {
                elementWidth = subElement.Layout.ResolvedWidth.Resolve(containerWidth) + marginRow;
                elementWidthMeasureMode = MeasureMode.Exactly;
            }

            if (isColumnStyleDimDefined)
            {
                elementHeight = subElement.Layout.ResolvedHeight.Resolve(containerHeight) +
                                marginColumn;
                elementHeightMeasureMode = MeasureMode.Exactly;
            }

            // The W3C spec doesn't say anything about the 'overflow' property, but all
            // major browsers appear to implement the following logic.
            if (!isLayoutDirectionRow && element.Plan.Overflow == Overflow.Scroll || element.Plan.Overflow != Overflow.Scroll)
            {
                if (elementWidth.IsUndefined() && width.HasValue())
                {
                    elementWidth = width;
                    elementWidthMeasureMode = MeasureMode.AtMost;
                }
            }

            if (isLayoutDirectionRow && element.Plan.Overflow == Overflow.Scroll || element.Plan.Overflow != Overflow.Scroll)
            {
                if (elementHeight.IsUndefined() && height.HasValue())
                {
                    elementHeight = height;
                    elementHeightMeasureMode = MeasureMode.AtMost;
                }
            }

            if (subElement.Plan.AspectRatio.HasValue())
            {
                if (!isLayoutDirectionRow && elementWidthMeasureMode == MeasureMode.Exactly)
                {
                    elementHeight = marginColumn + (elementWidth - marginRow) / subElement.Plan.AspectRatio;
                    elementHeightMeasureMode = MeasureMode.Exactly;
                }
                else if (isLayoutDirectionRow && elementHeightMeasureMode == MeasureMode.Exactly)
                {
                    elementWidth = marginRow + (elementHeight - marginColumn) * subElement.Plan.AspectRatio;
                    elementWidthMeasureMode = MeasureMode.Exactly;
                }
            }

            // If the subElement has no defined size in the cross direction and is set to stretch, set
            // the cross direction to be measured exactly with the available inner width

            var hasExactWidth = width.HasValue() && widthMode == MeasureMode.Exactly;
            var elementWidthStretch = AlignElement(element, subElement) == AlignmentLine_Cross.Stretch
                                      && elementWidthMeasureMode != MeasureMode.Exactly;
            if (!isLayoutDirectionRow && !isRowStyleDimDefined && hasExactWidth && elementWidthStretch)
            {
                elementWidth = width;
                elementWidthMeasureMode = MeasureMode.Exactly;
                if (subElement.Plan.AspectRatio.HasValue())
                {
                    elementHeight = (elementWidth - marginRow) / subElement.Plan.AspectRatio;
                    elementHeightMeasureMode = MeasureMode.Exactly;
                }
            }

            var hasExactHeight = height.HasValue() && heightMode == MeasureMode.Exactly;
            var elementHeightStretch = AlignElement(element, subElement) == AlignmentLine_Cross.Stretch
                                       && elementHeightMeasureMode != MeasureMode.Exactly;
            if (isLayoutDirectionRow && !isColumnStyleDimDefined && hasExactHeight && elementHeightStretch)
            {
                elementHeight = height;
                elementHeightMeasureMode = MeasureMode.Exactly;

                if (subElement.Plan.AspectRatio.HasValue())
                {
                    elementWidth = (elementHeight - marginColumn) * subElement.Plan.AspectRatio;
                    elementWidthMeasureMode = MeasureMode.Exactly;
                }
            }

            ConstrainMaxSizeForMode(
                subElement,
                LayoutDirection.Horz,
                containerWidth,
                containerWidth,
                ref elementWidthMeasureMode,
                ref elementWidth);
            ConstrainMaxSizeForMode(
                subElement,
                LayoutDirection.Vert,
                containerHeight,
                containerWidth,
                ref elementHeightMeasureMode,
                ref elementHeight);

            // Measure the subElement
            LayoutElement(
                subElement,
                elementWidth,
                elementHeight,
                elementWidthMeasureMode,
                elementHeightMeasureMode,
                containerWidth,
                containerHeight,
                false,
                LayoutPassReason.MeasureElement,
                depth,
                generationCount);

            subElement.Layout.ComputedLayoutDirectionLength = FloatMax(
                subElement.Layout.MeasuredDimensions[layoutDirection.ToDimension()],
                PaddingForDirection(subElement, layoutDirection, containerWidth));
        }

        subElement.Layout.ComputedLayoutDirectionLengthGeneration = generationCount;
    }

    private static Number ResolveLayoutDirectionLength(LayoutPlan plan)
    {
        var layoutDirectionLength = plan.ChildLayoutDirectionLength;

        if (layoutDirectionLength.Unit != Number.UoM.Auto && layoutDirectionLength.Unit != Number.UoM.Undefined)
            return layoutDirectionLength;

        if (plan.Flex.HasValue() && plan.Flex > 0.0f)
            return Number.Zero;

        return Number.Auto;
    }

    private static void AbsoluteLayoutElement(
        IElement container,
        IElement subElement,
        float width,
        MeasureMode widthMode,
        float height,
        int depth,
        int generationCount)
    {
        var layoutDirection = container.Plan.ElementsDirection;
        var crossDirection = ResolveCrossDirection(layoutDirection);
        var isLayoutDirectionRow = IsHorz(layoutDirection);

        var elementWidth = Number.ValueUndefined;
        var elementHeight = Number.ValueUndefined;

        var marginRow = GetMarginForDirection(subElement, LayoutDirection.Horz, width);
        var marginColumn = GetMarginForDirection(subElement, LayoutDirection.Vert, width);

        if (IsLayoutDirectionInPlan(subElement, LayoutDirection.Horz, width))
            elementWidth = subElement.Layout.ResolvedWidth.Resolve(width) + marginRow;
        else
        {
            // If the subElement doesn't have a specified width, compute the width based on
            // the left/right offsets if they're defined.
            if (IsLeadingPositionDefined(subElement, LayoutDirection.Horz)
                && IsTrailingPosDefined(subElement, LayoutDirection.Horz))
            {
                elementWidth = container.Layout.MeasuredDimensions.Width
                               - (GetLeadingPosition(subElement, LayoutDirection.Horz, width) + GetTrailingPosition(
                                   subElement,
                                   LayoutDirection.Horz,
                                   width));
                elementWidth = BoundDirection(subElement, LayoutDirection.Horz, elementWidth, width, width);
            }
        }

        if (IsLayoutDirectionInPlan(subElement, LayoutDirection.Vert, height))
            elementHeight = subElement.Layout.ResolvedHeight.Resolve(height) + marginColumn;
        else
        {
            // If the subElement doesn't have a specified height, compute the height based on
            // the top/bottom offsets if they're defined.
            if (IsLeadingPositionDefined(subElement, LayoutDirection.Vert)
                && IsTrailingPosDefined(subElement, LayoutDirection.Vert))
            {
                elementHeight = container.Layout.MeasuredDimensions.Height
                                - (GetLeadingPosition(subElement, LayoutDirection.Vert, height) + GetTrailingPosition(
                                    subElement,
                                    LayoutDirection.Vert,
                                    height));
                elementHeight = BoundDirection(
                    subElement,
                    LayoutDirection.Vert,
                    elementHeight,
                    height,
                    width);
            }
        }

        // Exactly one dimension needs to be defined for us to be able to do aspect
        // ratio calculation. One dimension being the anchor and the other being
        // flexible.
        if (elementWidth.IsUndefined() ^ elementHeight.IsUndefined())
        {
            if (subElement.Plan.AspectRatio.HasValue())
            {
                if (elementWidth.IsUndefined())
                    elementWidth = marginRow + (elementHeight - marginColumn) * subElement.Plan.AspectRatio;
                else if (elementHeight.IsUndefined())
                    elementHeight = marginColumn + (elementWidth - marginRow) / subElement.Plan.AspectRatio;
            }
        }

        // If we're still missing one or the other dimension, measure the content.
        if (elementWidth.IsUndefined() || elementHeight.IsUndefined())
        {
            var elementWidthMeasureMode = elementWidth.IsUndefined()
                ? MeasureMode.Undefined
                : MeasureMode.Exactly;
            var elementHeightMeasureMode = elementHeight.IsUndefined()
                ? MeasureMode.Undefined
                : MeasureMode.Exactly;

            // If the size of the container is defined then try to constrain the absolute
            // subElement to that size as well. This allows text within the absolute subElement to
            // wrap to the size of its container. This is the same behavior as many browsers
            // implement.
            if (!isLayoutDirectionRow
                && elementWidth.IsUndefined()
                && widthMode != MeasureMode.Undefined
                && width.HasValue()
                && width > 0)
            {
                elementWidth = width;
                elementWidthMeasureMode = MeasureMode.AtMost;
            }

            LayoutElement(
                subElement,
                elementWidth,
                elementHeight,
                elementWidthMeasureMode,
                elementHeightMeasureMode,
                elementWidth,
                elementHeight,
                false,
                LayoutPassReason.AbsMeasureElement,
                depth,
                generationCount);
            elementWidth = subElement.Layout.MeasuredDimensions.Width +
                           GetMarginForDirection(subElement, LayoutDirection.Horz, width);
            elementHeight = subElement.Layout.MeasuredDimensions.Height
                            + GetMarginForDirection(subElement, LayoutDirection.Vert, width);
        }

        LayoutElement(
            subElement,
            elementWidth,
            elementHeight,
            MeasureMode.Exactly,
            MeasureMode.Exactly,
            elementWidth,
            elementHeight,
            true,
            LayoutPassReason.AbsLayout,
            depth,
            generationCount);

        if (IsTrailingPosDefined(subElement, layoutDirection) && !IsLeadingPositionDefined(subElement, layoutDirection))
        {
            subElement.Layout.Position[layoutDirection.ToLeadingSide()] =
                container.Layout.MeasuredDimensions[layoutDirection.ToDimension()]
                - subElement.Layout.MeasuredDimensions[layoutDirection.ToDimension()]
                - GetTrailingMargin(subElement, layoutDirection, width)
                - GetTrailingPosition(
                    subElement,
                    layoutDirection,
                    isLayoutDirectionRow
                        ? width
                        : height);
        }
        else if (
            !IsLeadingPositionDefined(subElement, layoutDirection) && container.Plan.AlignElements_LayoutDirection == AlignmentElements_LayoutDirection.Center)
        {
            subElement.Layout.Position[layoutDirection.ToLeadingSide()] =
                (container.Layout.MeasuredDimensions[layoutDirection.ToDimension()]
                 - subElement.Layout.MeasuredDimensions[layoutDirection.ToDimension()])
                / 2.0f;
        }
        else if (!IsLeadingPositionDefined(subElement, layoutDirection) && container.Plan.AlignElements_LayoutDirection == AlignmentElements_LayoutDirection.End)
        {
            subElement.Layout.Position[layoutDirection.ToLeadingSide()] =
                container.Layout.MeasuredDimensions[layoutDirection.ToDimension()]
                - subElement.Layout.MeasuredDimensions[layoutDirection.ToDimension()];
        }

        if (IsTrailingPosDefined(subElement, crossDirection) && !IsLeadingPositionDefined(subElement, crossDirection))
        {
            subElement.Layout.Position[crossDirection.ToLeadingSide()] =
                container.Layout.MeasuredDimensions[crossDirection.ToDimension()]
                - subElement.Layout.MeasuredDimensions[crossDirection.ToDimension()]
                - GetTrailingMargin(subElement, crossDirection, width)
                - GetTrailingPosition(
                    subElement,
                    crossDirection,
                    isLayoutDirectionRow
                        ? height
                        : width);
        }
        else if (!IsLeadingPositionDefined(subElement, crossDirection) &&
                 AlignElement(container, subElement) == AlignmentLine_Cross.Center)
        {
            subElement.Layout.Position[crossDirection.ToLeadingSide()] =
                (container.Layout.MeasuredDimensions[crossDirection.ToDimension()]
                 - subElement.Layout.MeasuredDimensions[crossDirection.ToDimension()])
                / 2.0f;
        }
        else if (!IsLeadingPositionDefined(subElement, crossDirection) && AlignElement(container, subElement) == AlignmentLine_Cross.End)
        {
            subElement.Layout.Position[crossDirection.ToLeadingSide()] =
                container.Layout.MeasuredDimensions[crossDirection.ToDimension()]
                - subElement.Layout.MeasuredDimensions[crossDirection.ToDimension()];
        }
    }

    private static void SetMeasuredDimensions_MeasureFunc(
        IElement element,
        float availableWidth,
        float availableHeight,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        float containerWidth,
        float containerHeight)
    {
        Debug.Assert(
            element.MeasureFunc != null,
            "Expected element to have custom measure function");

        var paddingRow = PaddingForDirection(element, LayoutDirection.Horz, availableWidth);
        var paddingColumn = PaddingForDirection(element, LayoutDirection.Vert, availableWidth);
        var marginRow = GetMarginForDirection(element, LayoutDirection.Horz, availableWidth);
        var marginColumn = GetMarginForDirection(element, LayoutDirection.Vert, availableWidth);

        // We want to make sure we don't call measure with negative size
        var innerWidth = availableWidth.IsUndefined()
            ? availableWidth
            : FloatMax(0, availableWidth - marginRow - paddingRow);
        var innerHeight = availableHeight.IsUndefined()
            ? availableHeight
            : FloatMax(0, availableHeight - marginColumn - paddingColumn);

        if (widthMeasureMode == MeasureMode.Exactly && heightMeasureMode == MeasureMode.Exactly)
        {
            // Don't bother sizing the text if both dimensions are already defined.
            SetLayoutMeasuredDimension(
                element,
                BoundDirection(
                    element,
                    LayoutDirection.Horz,
                    availableWidth - marginRow,
                    containerWidth,
                    containerWidth),
                Dimension.Width);
            SetLayoutMeasuredDimension(
                element,
                BoundDirection(
                    element,
                    LayoutDirection.Vert,
                    availableHeight - marginColumn,
                    containerHeight,
                    containerWidth),
                Dimension.Height);
        }
        else
        {
            // Measure the text under the current constraints.
            var measuredSize = element.MeasureFunc?.Invoke(
                                   element,
                                   innerWidth,
                                   widthMeasureMode,
                                   innerHeight,
                                   heightMeasureMode)
                               ?? VecF.Zero;

            SetLayoutMeasuredDimension(
                element,
                BoundDirection(
                    element,
                    LayoutDirection.Horz,
                    widthMeasureMode == MeasureMode.Undefined || widthMeasureMode == MeasureMode.AtMost
                        ? measuredSize.X + paddingRow
                        : availableWidth - marginRow,
                    containerWidth,
                    containerWidth),
                Dimension.Width);

            SetLayoutMeasuredDimension(
                element,
                BoundDirection(
                    element,
                    LayoutDirection.Vert,
                    heightMeasureMode == MeasureMode.Undefined || heightMeasureMode == MeasureMode.AtMost
                        ? measuredSize.Y + paddingColumn
                        : availableHeight - marginColumn,
                    containerHeight,
                    containerWidth),
                Dimension.Height);
        }
    }

    // For elements with no elements, use the available values if they were provided,
    // or the minimum size as indicated by the padding sizes.
    private static void SetMeasuredDimensions_EmptyContainer(
        IElement element,
        float availableWidth,
        float availableHeight,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        float containerWidth,
        float containerHeight)
    {
        var paddingRow = PaddingForDirection(element, LayoutDirection.Horz, containerWidth);
        var paddingColumn = PaddingForDirection(element, LayoutDirection.Vert, containerWidth);
        var marginRow = GetMarginForDirection(element, LayoutDirection.Horz, containerWidth);
        var marginColumn = GetMarginForDirection(element, LayoutDirection.Vert, containerWidth);

        SetLayoutMeasuredDimension(
            element,
            BoundDirection(
                element,
                LayoutDirection.Horz,
                widthMeasureMode == MeasureMode.Undefined
                || widthMeasureMode == MeasureMode.AtMost
                    ? paddingRow
                    : availableWidth - marginRow,
                containerWidth,
                containerWidth),
            Dimension.Width);

        SetLayoutMeasuredDimension(
            element,
            BoundDirection(
                element,
                LayoutDirection.Vert,
                heightMeasureMode == MeasureMode.Undefined
                || heightMeasureMode == MeasureMode.AtMost
                    ? paddingColumn
                    : availableHeight - marginColumn,
                containerHeight,
                containerWidth),
            Dimension.Height);
    }

    private static bool SetMeasuredDimensions_FixedSize(
        IElement element,
        float availableWidth,
        float availableHeight,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        float containerWidth,
        float containerHeight)
    {
        if (availableWidth.HasValue() && widthMeasureMode == MeasureMode.AtMost && availableWidth <= 0.0f
            || availableHeight.HasValue() && heightMeasureMode == MeasureMode.AtMost && availableHeight <= 0.0f
            || widthMeasureMode == MeasureMode.Exactly && heightMeasureMode == MeasureMode.Exactly)
        {
            var marginColumn = GetMarginForDirection(element, LayoutDirection.Vert, containerWidth);
            var marginRow = GetMarginForDirection(element, LayoutDirection.Horz, containerWidth);

            SetLayoutMeasuredDimension(
                element,
                BoundDirection(
                    element,
                    LayoutDirection.Horz,
                    availableWidth.IsUndefined()
                    || widthMeasureMode == MeasureMode.AtMost
                    && availableWidth < 0.0f
                        ? 0.0f
                        : availableWidth - marginRow,
                    containerWidth,
                    containerWidth),
                Dimension.Width);

            SetLayoutMeasuredDimension(
                element,
                BoundDirection(
                    element,
                    LayoutDirection.Vert,
                    availableHeight.IsUndefined() || heightMeasureMode == MeasureMode.AtMost && availableHeight < 0.0f
                        ? 0.0f
                        : availableHeight - marginColumn,
                    containerHeight,
                    containerWidth),
                Dimension.Height);

            return true;
        }

        return false;
    }

    private static void ZeroOutLayoutRecursively(IElement container)
    {
        Traverse(
            container,
            n => n.Layout = new LayoutActual { Width = 0f, Height = 0f });
    }

    public static void Traverse(IElement container, Action<IElement> action)
    {
        action(container);
        foreach (var subElement in container.Children.Cast<IElement>())
        {
            Traverse(subElement, action);
        }
    }

    private static float CalculateAvailableInnerDim(
        IElement element,
        LayoutDirection layoutDirection,
        float availableDim,
        float containerDim)
    {
        var direction = IsHorz(layoutDirection)
            ? LayoutDirection.Horz
            : LayoutDirection.Vert;
        var dimension = IsHorz(layoutDirection)
            ? Dimension.Width
            : Dimension.Height;

        var margin = GetMarginForDirection(element, direction, containerDim);
        var padding = PaddingForDirection(element, direction, containerDim);

        var availableInnerDim = availableDim - margin - padding;
        // Max dimension overrides predefined dimension value; Min dimension in turn
        // overrides both of the above
        if (availableInnerDim.HasValue())
        {
            // We want to make sure our available height does not violate min and max
            // constraints
            var minDimensionOptional = element.Plan.MinDimension(dimension).Resolve(containerDim);
            var minInnerDim = minDimensionOptional.IsUndefined()
                ? 0.0f
                : minDimensionOptional - padding;

            var maxDimensionOptional = element.Plan.MaxDimension(dimension).Resolve(containerDim);

            var maxInnerDim = maxDimensionOptional.IsUndefined()
                ? float.MaxValue
                : maxDimensionOptional - padding;
            availableInnerDim = FloatMax(FloatMin(availableInnerDim, maxInnerDim), minInnerDim);
        }

        return availableInnerDim;
    }

    // This function assumes that all the elements of element have their computedLayoutDirectionLength properly computed
    // (To do this use ComputeLayoutDirectionLengthForElements function).
    // This function calculates YGCollectFlexItemsRowMeasurement
    private static CollectFlexItemsRowValues CalculateCollectFlexItemsRowValues(
        IElement element,
        float layoutDirectionContainerSize,
        float availableInnerWidth,
        float availableInnerLayoutDirectionDim,
        int startOfLineIndex,
        int lineCount)
    {
        var flexAlgoRowMeasurement = new CollectFlexItemsRowValues
                { 
                    RelativeElements = new List<IElement>(element.Children.Count) 
                };

        float sizeConsumedOnCurrentLineIncludingMinConstraint = 0;
        var layoutDirection = element.Plan.ElementsDirection;
        var isLayoutWrap = element.Plan.ElementsWrap != LayoutWrap.NoWrap;

        // Add items to the current line until it's full or we run out of items.
        var endOfLineIndex = startOfLineIndex;
        for (; endOfLineIndex < element.Children.Count; endOfLineIndex++)
        {
            var subElement = (IElement)element.Children[endOfLineIndex];

            if (subElement.Plan.Atomic || subElement.Plan.PositionType == PositionType.Absolute)
                continue;

            subElement.Layout.LineIndex = lineCount;
            var elementMarginLayoutDirection = GetMarginForDirection(subElement, layoutDirection, availableInnerWidth);
            var layoutDirectionLengthWithMinAndMaxConstraints = BoundDirectionWithinMinAndMax(
                subElement,
                layoutDirection,
                subElement.Layout.ComputedLayoutDirectionLength,
                layoutDirectionContainerSize);

            // If this is a multi-line flow and this item pushes us over the available
            // size, we've hit the end of the current line. Break out of the loop and
            // lay out the current line.
            if (sizeConsumedOnCurrentLineIncludingMinConstraint
                + layoutDirectionLengthWithMinAndMaxConstraints
                + elementMarginLayoutDirection
                > availableInnerLayoutDirectionDim
                && isLayoutWrap
                && flexAlgoRowMeasurement.ItemsOnLine > 0)
                break;

            sizeConsumedOnCurrentLineIncludingMinConstraint +=
                layoutDirectionLengthWithMinAndMaxConstraints + elementMarginLayoutDirection;
            flexAlgoRowMeasurement.SizeConsumedOnCurrentLine +=
                layoutDirectionLengthWithMinAndMaxConstraints + elementMarginLayoutDirection;
            flexAlgoRowMeasurement.ItemsOnLine++;

            if (IsElementFlexible(subElement))
            {
                flexAlgoRowMeasurement.TotalFlexGrowFactors += ResolveFlexGrow(subElement);

                // Unlike the grow factor, the shrink factor is scaled relative to the subElement dimension.
                flexAlgoRowMeasurement.TotalFlexShrinkScaledFactors +=
                    -ResolveFlexShrink(subElement) * subElement.Layout.ComputedLayoutDirectionLength;
            }

            flexAlgoRowMeasurement.RelativeElements.Add(subElement);
        }

        // The total flex factor needs to be floored to 1.
        if (flexAlgoRowMeasurement.TotalFlexGrowFactors > 0 && flexAlgoRowMeasurement.TotalFlexGrowFactors < 1)
            flexAlgoRowMeasurement.TotalFlexGrowFactors = 1;

        // The total flex shrink factor needs to be floored to 1.
        if (flexAlgoRowMeasurement.TotalFlexShrinkScaledFactors > 0
            && flexAlgoRowMeasurement.TotalFlexShrinkScaledFactors < 1)
            flexAlgoRowMeasurement.TotalFlexShrinkScaledFactors = 1;

        flexAlgoRowMeasurement.EndOfLineIndex = endOfLineIndex;

        return flexAlgoRowMeasurement;
    }

    // It distributes the free space to the flexible items.For those flexible items
    // whose min and max constraints are triggered, those flex item's clamped size
    // is removed from the remaining free space.
    private static void DistributeFreeSpace_FirstPass(
        CollectFlexItemsRowValues collectedFlexItemsValues,
        LayoutDirection layoutDirection,
        float layoutDirectionContainerLength,
        float availableInnerLayoutDirectionDim,
        float availableInnerWidth)
    {
        float deltaFreeSpace = 0;

        foreach (var currentRelativeElement in collectedFlexItemsValues.RelativeElements.Cast<IElement>())
        {
            var elementLayoutDirectionLength =
                BoundDirectionWithinMinAndMax(
                    currentRelativeElement,
                    layoutDirection,
                    currentRelativeElement.Layout.ComputedLayoutDirectionLength,
                    layoutDirectionContainerLength);

            float baseLayoutDirectionLength;
            float boundLayoutDirectionLength;
            if (collectedFlexItemsValues.RemainingFreeSpace < 0)
            {
                var flexShrinkScaledFactor = -ResolveFlexShrink(currentRelativeElement) * elementLayoutDirectionLength;

                // Is this subElement able to shrink?
                if (flexShrinkScaledFactor.HasValue() && flexShrinkScaledFactor.IsNotZero())
                {
                    baseLayoutDirectionLength = elementLayoutDirectionLength
                                   + collectedFlexItemsValues.RemainingFreeSpace
                                   / collectedFlexItemsValues.TotalFlexShrinkScaledFactors
                                   * flexShrinkScaledFactor;
                    boundLayoutDirectionLength = BoundDirection(
                        currentRelativeElement,
                        layoutDirection,
                        baseLayoutDirectionLength,
                        availableInnerLayoutDirectionDim,
                        availableInnerWidth);
                    if (baseLayoutDirectionLength.HasValue()
                        && boundLayoutDirectionLength.HasValue()
                        && !FloatsEqual(baseLayoutDirectionLength, boundLayoutDirectionLength))
                    {
                        // By excluding this item's size and flex factor from remaining, this
                        // item's min/max constraints should also trigger in the second pass
                        // resulting in the item's size calculation being identical in the
                        // first and second passes.
                        deltaFreeSpace += boundLayoutDirectionLength - elementLayoutDirectionLength;
                        collectedFlexItemsValues.TotalFlexShrinkScaledFactors -= flexShrinkScaledFactor;
                    }
                }
            }
            else if (collectedFlexItemsValues.RemainingFreeSpace.HasValue()
                     && collectedFlexItemsValues.RemainingFreeSpace > 0)
            {
                var flexGrowFactor = ResolveFlexGrow(currentRelativeElement);

                // Is this subElement able to grow?
                if (flexGrowFactor.HasValue() && flexGrowFactor.IsNotZero())
                {
                    baseLayoutDirectionLength = elementLayoutDirectionLength
                                   + collectedFlexItemsValues.RemainingFreeSpace
                                   / collectedFlexItemsValues.TotalFlexGrowFactors
                                   * flexGrowFactor;
                    boundLayoutDirectionLength = BoundDirection(
                        currentRelativeElement,
                        layoutDirection,
                        baseLayoutDirectionLength,
                        availableInnerLayoutDirectionDim,
                        availableInnerWidth);

                    if (baseLayoutDirectionLength.HasValue()
                        && boundLayoutDirectionLength.HasValue()
                        && !FloatsEqual(baseLayoutDirectionLength, boundLayoutDirectionLength))
                    {
                        // By excluding this item's size and flex factor from remaining, this
                        // item's min/max constraints should also trigger in the second pass
                        // resulting in the item's size calculation being identical in the
                        // first and second passes.
                        deltaFreeSpace += boundLayoutDirectionLength - elementLayoutDirectionLength;
                        collectedFlexItemsValues.TotalFlexGrowFactors -= flexGrowFactor;
                    }
                }
            }
        }

        collectedFlexItemsValues.RemainingFreeSpace -= deltaFreeSpace;
    }

    // It distributes the free space to the flexible items and ensures that the size
    // of the flex items abide the min and max constraints. At the end of this
    // function the elements would have proper size. Prior using this function
    // please ensure that DistributeFreeSpace_FirstPass is called.
    private static float DistributeFreeSpace_SecondPass(
        CollectFlexItemsRowValues collectedFlexItemsValues,
        IElement element,
        LayoutDirection layoutDirection,
        LayoutDirection crossDirection,
        float layoutDirectionContainerLength,
        float availableInnerLayoutDirectionDim,
        float availableInnerCrossDim,
        float availableInnerWidth,
        float availableInnerHeight,
        bool layoutDirectionLengthOverflows,
        MeasureMode measureModeCrossDim,
        bool performLayout,
        int depth,
        int generationCount)
    {
        float deltaFreeSpace = 0;
        var isLayoutDirectionRow = IsHorz(layoutDirection);
        var isLayoutWrap = element.Plan.ElementsWrap != LayoutWrap.NoWrap;

        foreach (var currentRelativeElement in collectedFlexItemsValues.RelativeElements.Cast<IElement>())
        {
            var elementLayoutDirectionLength = BoundDirectionWithinMinAndMax(
                currentRelativeElement,
                layoutDirection,
                currentRelativeElement.Layout.ComputedLayoutDirectionLength,
                layoutDirectionContainerLength);
            var updatedLayoutDirectionLength = elementLayoutDirectionLength;

            if (collectedFlexItemsValues.RemainingFreeSpace.HasValue()
                && collectedFlexItemsValues.RemainingFreeSpace < 0)
            {
                var flexShrinkScaledFactor = -ResolveFlexShrink(currentRelativeElement) * elementLayoutDirectionLength;
                // Is this subElement able to shrink?
                if (flexShrinkScaledFactor.IsNotZero())
                {
                    float elementSize;

                    if (collectedFlexItemsValues.TotalFlexShrinkScaledFactors.HasValue()
                        && collectedFlexItemsValues.TotalFlexShrinkScaledFactors.IsZero())
                        elementSize = elementLayoutDirectionLength + flexShrinkScaledFactor;
                    else
                    {
                        elementSize = elementLayoutDirectionLength
                                      + collectedFlexItemsValues.RemainingFreeSpace
                                      / collectedFlexItemsValues.TotalFlexShrinkScaledFactors
                                      * flexShrinkScaledFactor;
                    }

                    updatedLayoutDirectionLength = BoundDirection(
                        currentRelativeElement,
                        layoutDirection,
                        elementSize,
                        availableInnerLayoutDirectionDim,
                        availableInnerWidth);
                }
            }
            else if (collectedFlexItemsValues.RemainingFreeSpace.HasValue()
                     && collectedFlexItemsValues.RemainingFreeSpace > 0)
            {
                var flexGrowFactor = ResolveFlexGrow(currentRelativeElement);

                // Is this subElement able to grow?
                if (flexGrowFactor.HasValue() && flexGrowFactor.IsNotZero())
                {
                    updatedLayoutDirectionLength = BoundDirection(
                        currentRelativeElement,
                        layoutDirection,
                        elementLayoutDirectionLength
                        + collectedFlexItemsValues.RemainingFreeSpace
                        / collectedFlexItemsValues.TotalFlexGrowFactors
                        * flexGrowFactor,
                        availableInnerLayoutDirectionDim,
                        availableInnerWidth);
                }
            }

            deltaFreeSpace += updatedLayoutDirectionLength - elementLayoutDirectionLength;

            var marginLayoutDirection = GetMarginForDirection(currentRelativeElement, layoutDirection, availableInnerWidth);
            var marginCross = GetMarginForDirection(currentRelativeElement, crossDirection, availableInnerWidth);

            float elementCrossSize;
            var elementLayouutDirectionLength = updatedLayoutDirectionLength + marginLayoutDirection;
            var elementLayoutDirectionMeasureMode = MeasureMode.Exactly;
            MeasureMode elementCrossMeasureMode;

            if (currentRelativeElement.Plan.AspectRatio.HasValue())
            {
                elementCrossSize = isLayoutDirectionRow
                    ? (elementLayouutDirectionLength - marginLayoutDirection) / currentRelativeElement.Plan.AspectRatio
                    : (elementLayouutDirectionLength - marginLayoutDirection) * currentRelativeElement.Plan.AspectRatio;
                elementCrossMeasureMode = MeasureMode.Exactly;

                elementCrossSize += marginCross;
            }
            else if (
                availableInnerCrossDim.HasValue()
                && !IsLayoutDirectionInPlan(currentRelativeElement, crossDirection, availableInnerCrossDim)
                && measureModeCrossDim == MeasureMode.Exactly
                && !(isLayoutWrap && layoutDirectionLengthOverflows)
                && AlignElement(element, currentRelativeElement) == AlignmentLine_Cross.Stretch
                && MarginLeadingValue(currentRelativeElement, crossDirection).Unit != Number.UoM.Auto
                && MarginTrailingValue(currentRelativeElement, crossDirection).Unit != Number.UoM.Auto)
            {
                elementCrossSize = availableInnerCrossDim;
                elementCrossMeasureMode = MeasureMode.Exactly;
            }
            else if (!IsLayoutDirectionInPlan(currentRelativeElement, crossDirection, availableInnerCrossDim))
            {
                elementCrossSize = availableInnerCrossDim;
                elementCrossMeasureMode = elementCrossSize.IsUndefined()
                    ? MeasureMode.Undefined
                    : MeasureMode.AtMost;
            }
            else
            {
                elementCrossSize = currentRelativeElement.Layout
                                                         .GetResolvedDimension(crossDirection.ToDimension())
                                                         .Resolve(availableInnerCrossDim)
                                   + marginCross;
                var isLoosePercentageMeasurement =
                    currentRelativeElement.Layout.GetResolvedDimension(crossDirection.ToDimension()).Unit == Number.UoM.Percent
                    && measureModeCrossDim != MeasureMode.Exactly;
                elementCrossMeasureMode = elementCrossSize.IsUndefined() || isLoosePercentageMeasurement
                    ? MeasureMode.Undefined
                    : MeasureMode.Exactly;
            }

            ConstrainMaxSizeForMode(
                currentRelativeElement,
                layoutDirection,
                availableInnerLayoutDirectionDim,
                availableInnerWidth,
                ref elementLayoutDirectionMeasureMode,
                ref elementLayouutDirectionLength);
            ConstrainMaxSizeForMode(
                currentRelativeElement,
                crossDirection,
                availableInnerCrossDim,
                availableInnerWidth,
                ref elementCrossMeasureMode,
                ref elementCrossSize);

            var requiresStretchLayout = !IsLayoutDirectionInPlan(
                                            currentRelativeElement,
                                            crossDirection,
                                            availableInnerCrossDim)
                                        && AlignElement(element, currentRelativeElement) == AlignmentLine_Cross.Stretch
                                        && MarginLeadingValue(currentRelativeElement, crossDirection).Unit != Number.UoM.Auto
                                        && MarginTrailingValue(currentRelativeElement, crossDirection).Unit !=
                                        Number.UoM.Auto;

            var elementWidth = isLayoutDirectionRow
                ? elementLayouutDirectionLength
                : elementCrossSize;
            var elementHeight = !isLayoutDirectionRow
                ? elementLayouutDirectionLength
                : elementCrossSize;

            var elementWidthMeasureMode = isLayoutDirectionRow
                ? elementLayoutDirectionMeasureMode
                : elementCrossMeasureMode;
            var elementHeightMeasureMode = !isLayoutDirectionRow
                ? elementLayoutDirectionMeasureMode
                : elementCrossMeasureMode;

            var isLayoutPass = performLayout && !requiresStretchLayout;
            // Recursively call the layout algorithm for this subElement with the updated layout-direction length
            LayoutElement(
                currentRelativeElement,
                elementWidth,
                elementHeight,
                elementWidthMeasureMode,
                elementHeightMeasureMode,
                availableInnerWidth,
                availableInnerHeight,
                isLayoutPass,
                isLayoutPass
                    ? LayoutPassReason.FlexLayout
                    : LayoutPassReason.FlexMeasure,
                depth,
                generationCount);
            element.Layout.HadOverflow = element.Layout.HadOverflow | currentRelativeElement.Layout.HadOverflow;
        }

        return deltaFreeSpace;
    }

    // Do two passes over the flex items to figure out how to distribute the
    // remaining space.
    //
    // The first pass finds the items whose min/max constraints trigger, freezes
    // them at those sizes, and excludes those sizes from the remaining space.
    //
    // The second pass sets the size of each flexible item. It distributes the
    // remaining space amongst the items whose min/max constraints didn't trigger in
    // the first pass. For the other items, it sets their sizes by forcing their
    // min/max constraints to trigger again.
    //
    // This two pass approach for resolving min/max constraints deviates from the
    // spec. The spec
    // (https://www.w3.org/TR/CSS-flexbox-1/#resolve-flexible-lengths) describes a
    // process that needs to be repeated a variable number of times. The algorithm
    // implemented here won't handle all cases but it was simpler to implement and
    // it mitigates performance concerns because we know exactly how many passes
    // it'll do.
    //
    // At the end of this function the elements would have the proper size assigned to them.
    private static void ResolveFlexibleLength(
        IElement element,
        CollectFlexItemsRowValues collectedFlexItemsValues,
        LayoutDirection layoutDirection,
        LayoutDirection crossDirection,
        float layoutDirectionContainerLength,
        float availableInnerLayoutDirectionDim,
        float availableInnerCrossDim,
        float availableInnerWidth,
        float availableInnerHeight,
        bool layoutDirectionLengthOverflows,
        MeasureMode measureModeCrossDim,
        bool performLayout,
        int depth,
        int generationCount)
    {
        var originalFreeSpace = collectedFlexItemsValues.RemainingFreeSpace;
        // First pass: detect the flex items whose min/max constraints trigger
        DistributeFreeSpace_FirstPass(
            collectedFlexItemsValues,
            layoutDirection,
            layoutDirectionContainerLength,
            availableInnerLayoutDirectionDim,
            availableInnerWidth);

        // Second pass: resolve the sizes of the flexible items
        var distributedFreeSpace = DistributeFreeSpace_SecondPass(
            collectedFlexItemsValues,
            element,
            layoutDirection,
            crossDirection,
            layoutDirectionContainerLength,
            availableInnerLayoutDirectionDim,
            availableInnerCrossDim,
            availableInnerWidth,
            availableInnerHeight,
            layoutDirectionLengthOverflows,
            measureModeCrossDim,
            performLayout,
            depth,
            generationCount);

        collectedFlexItemsValues.RemainingFreeSpace = originalFreeSpace - distributedFreeSpace;
    }

    private static void JustifyLayoutDirection(
        IElement element,
        CollectFlexItemsRowValues collectedFlexItemsValues,
        int startOfLineIndex,
        LayoutDirection layoutDirection,
        LayoutDirection crossDirection,
        MeasureMode measureModeLayoutDirectionDim,
        MeasureMode measureModeCrossDim,
        float layoutDirectionContainerLength,
        float containerWidth,
        float availableInnerLayoutDirectionDim,
        float availableInnerCrossDim,
        float availableInnerWidth,
        bool performLayout)
    {
        var leadingPaddingLayoutDirection = GetLeadingPadding(element, layoutDirection, containerWidth);
        var trailingPaddingLayoutDirection = GetTrailingPadding(element, layoutDirection, containerWidth);
        // If we are using "at most" rules in the layout-direction, make sure that
        // remainingFreeSpace is 0 when min layout-direction dimension is not given
        if (measureModeLayoutDirectionDim == MeasureMode.AtMost && collectedFlexItemsValues.RemainingFreeSpace > 0)
        {
            if (element.Plan.MinDimension(layoutDirection.ToDimension()).HasValue
                && element.Plan.MinDimension(layoutDirection.ToDimension()).Resolve(layoutDirectionContainerLength).HasValue())
            {
                // This condition makes sure that if the size of layout-direction dimension (after
                // considering elements layout-direction dim, leading and trailing padding etc)
                // falls below min dimension, then the remainingFreeSpace is reassigned
                // considering the min dimension

                // `minAvailableLayoutDirectionDim` denotes minimum available space in which subElement
                // can be laid out, it will exclude space consumed by padding.
                var minAvailableLayoutDirectionDim = element.Plan.MinDimension(layoutDirection.ToDimension()).Resolve(layoutDirectionContainerLength)
                                          - leadingPaddingLayoutDirection
                                          - trailingPaddingLayoutDirection;
                var occupiedSpaceByElements = availableInnerLayoutDirectionDim - collectedFlexItemsValues.RemainingFreeSpace;
                collectedFlexItemsValues.RemainingFreeSpace = FloatMax(
                    0,
                    minAvailableLayoutDirectionDim - occupiedSpaceByElements);
            }
            else
                collectedFlexItemsValues.RemainingFreeSpace = 0;
        }

        var numberOfAutoMarginsOnCurrentLine = 0;
        for (var i = startOfLineIndex; i < collectedFlexItemsValues.EndOfLineIndex; i++)
        {
            var subElement = (IElement)element.Children[i];
            if (subElement.Plan.PositionType == PositionType.Relative)
            {
                if (MarginLeadingValue(subElement, layoutDirection).Unit == Number.UoM.Auto)
                    numberOfAutoMarginsOnCurrentLine++;

                if (MarginTrailingValue(subElement, layoutDirection).Unit == Number.UoM.Auto)
                    numberOfAutoMarginsOnCurrentLine++;
            }
        }

        // In order to position the elements in the layout-direction, we have two controls.
        // The space between the beginning and the first subElement and the space between
        // each two elements.
        float leadingLayoutDirectionDim = 0;
        float betweenLayoutDirectionDim = 0;
        var justifyContent = element.Plan.AlignElements_LayoutDirection;

        if (numberOfAutoMarginsOnCurrentLine == 0)
        {
            switch (justifyContent)
            {
                case AlignmentElements_LayoutDirection.Center:
                    leadingLayoutDirectionDim = collectedFlexItemsValues.RemainingFreeSpace / 2;

                    break;
                case AlignmentElements_LayoutDirection.End:
                    leadingLayoutDirectionDim = collectedFlexItemsValues.RemainingFreeSpace;

                    break;
                case AlignmentElements_LayoutDirection.SpaceBetween:
                    if (collectedFlexItemsValues.ItemsOnLine > 1)
                    {
                        betweenLayoutDirectionDim =
                            FloatMax(collectedFlexItemsValues.RemainingFreeSpace, 0)
                            / (collectedFlexItemsValues.ItemsOnLine - 1);
                    }
                    else
                        betweenLayoutDirectionDim = 0;

                    break;
                case AlignmentElements_LayoutDirection.SpaceEvenly:
                    // Space is distributed evenly across all elements
                    betweenLayoutDirectionDim = collectedFlexItemsValues.RemainingFreeSpace /
                                     (collectedFlexItemsValues.ItemsOnLine + 1);
                    leadingLayoutDirectionDim = betweenLayoutDirectionDim;

                    break;
                case AlignmentElements_LayoutDirection.SpaceAround:
                    // Space on the edges is half of the space between elements
                    betweenLayoutDirectionDim = collectedFlexItemsValues.RemainingFreeSpace / collectedFlexItemsValues.ItemsOnLine;
                    leadingLayoutDirectionDim = betweenLayoutDirectionDim / 2;

                    break;
                case AlignmentElements_LayoutDirection.Start:
                    break;
            }
        }

        collectedFlexItemsValues.LayoutDirectionDim = leadingPaddingLayoutDirection + leadingLayoutDirectionDim;
        collectedFlexItemsValues.CrossDim = 0;

        for (var i = startOfLineIndex; i < collectedFlexItemsValues.EndOfLineIndex; i++)
        {
            var subElement = (IElement)element.Children[i];

            if (subElement.Plan.Atomic)
                continue;

            if (subElement.Plan.PositionType == PositionType.Absolute && IsLeadingPositionDefined(subElement, layoutDirection))
            {
                if (performLayout)
                {
                    // In case the subElement is position absolute and has left/top being
                    // defined, we override the position to whatever the user said (and
                    // margin).
                    subElement.Layout.Position[layoutDirection.ToPosition()] =
                        GetLeadingPosition(subElement, layoutDirection, availableInnerLayoutDirectionDim)
                        + GetLeadingMargin(subElement, layoutDirection, availableInnerWidth);
                }
            }
            else
            {
                // Now that we placed the subElement, we need to update the variables.
                // We need to do that only for relative elements. Absolute elements do not
                // take part in that phase.
                if (subElement.Plan.PositionType == PositionType.Relative)
                {
                    if (MarginLeadingValue(subElement, layoutDirection).Unit == Number.UoM.Auto)
                        collectedFlexItemsValues.LayoutDirectionDim += collectedFlexItemsValues.RemainingFreeSpace /
                                                            numberOfAutoMarginsOnCurrentLine;

                    if (performLayout)
                    {
                        subElement.Layout.Position[layoutDirection.ToPosition()] = subElement.Layout.Position[layoutDirection.ToPosition()]
                                                                      + collectedFlexItemsValues.LayoutDirectionDim;
                    }

                    if (MarginTrailingValue(subElement, layoutDirection).Unit == Number.UoM.Auto)
                        collectedFlexItemsValues.LayoutDirectionDim += collectedFlexItemsValues.RemainingFreeSpace /
                                                            numberOfAutoMarginsOnCurrentLine;

                    var canSkipFlex = !performLayout && measureModeCrossDim == MeasureMode.Exactly;
                    if (canSkipFlex)
                    {
                        // If we skipped the flex step, then we can't rely on the measuredDims
                        // because they weren't computed. This means we can't call DimWithMargin.
                        collectedFlexItemsValues.LayoutDirectionDim += betweenLayoutDirectionDim
                                                            + GetMarginForDirection(subElement, layoutDirection, availableInnerWidth)
                                                            + subElement.Layout.ComputedLayoutDirectionLength;
                        collectedFlexItemsValues.CrossDim = availableInnerCrossDim;
                    }
                    else
                    {
                        // The layout-direction dimension is the sum of all the elements dimension plus the spacing.
                        collectedFlexItemsValues.LayoutDirectionDim += betweenLayoutDirectionDim + DimWithMargin(
                            subElement,
                            layoutDirection,
                            availableInnerWidth);

                        // The cross dimension is the max of the elements dimension since
                        // there can only be one subElement in that cross dimension in the case
                        // when the items are not baseline aligned
                        collectedFlexItemsValues.CrossDim = FloatMax(
                            collectedFlexItemsValues.CrossDim,
                            DimWithMargin(subElement, crossDirection, availableInnerWidth));
                    }
                }
            }
        }

        collectedFlexItemsValues.LayoutDirectionDim += trailingPaddingLayoutDirection;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MeasureMode_SizeIsExactAndMatchesOldMeasuredSize(
        MeasureMode sizeMode,
        float size,
        float lastComputedSize) =>
        sizeMode == MeasureMode.Exactly && FloatsEqual(size, lastComputedSize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MeasureMode_OldSizeIsUnspecifiedAndStillFits(
        MeasureMode sizeMode,
        float size,
        MeasureMode lastSizeMode,
        float lastComputedSize) =>
        sizeMode == MeasureMode.AtMost
        && lastSizeMode == MeasureMode.Undefined
        && (size >= lastComputedSize || FloatsEqual(size, lastComputedSize));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool MeasureMode_NewMeasureSizeIsStricterAndStillValid(
        MeasureMode sizeMode,
        float size,
        MeasureMode lastSizeMode,
        float lastSize,
        float lastComputedSize) =>
        lastSizeMode == MeasureMode.AtMost
        && sizeMode == MeasureMode.AtMost
        && lastSize.HasValue()
        && size.HasValue()
        && lastComputedSize.HasValue()
        && lastSize > size
        && (lastComputedSize <= size || FloatsEqual(size, lastComputedSize));

    private static bool CanUseCachedMeasurement(
        MeasureMode widthMode,
        float width,
        MeasureMode heightMode,
        float height,
        MeasureMode lastWidthMode,
        float lastWidth,
        MeasureMode lastHeightMode,
        float lastHeight,
        float lastComputedWidth,
        float lastComputedHeight,
        float marginRow,
        float marginColumn)
    {
        if (lastComputedHeight.HasValue() && lastComputedHeight < 0
            || lastComputedWidth.HasValue() && lastComputedWidth < 0)
            return false;

        var effectiveWidth = width;
        var effectiveHeight = height;
        var effectiveLastWidth = lastWidth;
        var effectiveLastHeight = lastHeight;

        var hasSameWidthSpec = lastWidthMode == widthMode && FloatsEqual(effectiveLastWidth, effectiveWidth);
        var hasSameHeightSpec = lastHeightMode == heightMode && FloatsEqual(effectiveLastHeight, effectiveHeight);

        var widthIsCompatible = hasSameWidthSpec
                                || MeasureMode_SizeIsExactAndMatchesOldMeasuredSize(
                                    widthMode,
                                    width - marginRow,
                                    lastComputedWidth)
                                || MeasureMode_OldSizeIsUnspecifiedAndStillFits(
                                    widthMode,
                                    width - marginRow,
                                    lastWidthMode,
                                    lastComputedWidth)
                                || MeasureMode_NewMeasureSizeIsStricterAndStillValid(
                                    widthMode,
                                    width - marginRow,
                                    lastWidthMode,
                                    lastWidth,
                                    lastComputedWidth);

        var heightIsCompatible = hasSameHeightSpec
                                 || MeasureMode_SizeIsExactAndMatchesOldMeasuredSize(
                                     heightMode,
                                     height - marginColumn,
                                     lastComputedHeight)
                                 || MeasureMode_OldSizeIsUnspecifiedAndStillFits(
                                     heightMode,
                                     height - marginColumn,
                                     lastHeightMode,
                                     lastComputedHeight)
                                 || MeasureMode_NewMeasureSizeIsStricterAndStillValid(
                                     heightMode,
                                     height - marginColumn,
                                     lastHeightMode,
                                     lastHeight,
                                     lastComputedHeight);

        return widthIsCompatible && heightIsCompatible;
    }

    private static void RoundToPixelGrid(IElement element, float pointScaleFactor, float absoluteLeft, float absoluteTop)
    {
        if (pointScaleFactor.IsZero()) return;

        var elementLeft = element.Layout.Left;
        var elementTop = element.Layout.Top;

        var elementWidth = element.Layout.Width;
        var elementHeight = element.Layout.Height;

        var absoluteElementLeft = absoluteLeft + elementLeft;
        var absoluteElementTop = absoluteTop + elementTop;

        var absoluteElementRight = absoluteElementLeft + elementWidth;
        var absoluteElementBottom = absoluteElementTop + elementHeight;

        // If a element has a custom measure function we never want to round down its
        // size as this could lead to unwanted text truncation.
        var textRounding = element.Plan.IsText;

        SetLayoutPosition(
            element,
            Number.RoundValue(elementLeft, pointScaleFactor, false, textRounding),
            Side.Left);

        SetLayoutPosition(
            element,
            Number.RoundValue(elementTop, pointScaleFactor, false, textRounding),
            Side.Top);

        // We multiply dimension by scale factor and if the result is close to the
        // whole number, we don't have any fraction To verify if the result is close
        // to whole number we want to check both floor and ceil numbers
        var hasFractionalWidth =
            !FloatsEqual(FloatMod(elementWidth * pointScaleFactor, 1.0f), 0f)
            && !FloatsEqual(FloatMod(elementWidth * pointScaleFactor, 1.0f), 1f);
        var hasFractionalHeight = !FloatsEqual(FloatMod(elementHeight * pointScaleFactor, 1.0f), 0f)
                                  && !FloatsEqual(FloatMod(elementHeight * pointScaleFactor, 1.0f), 1f);

        element.Layout.Width =
            Number.RoundValue(
                absoluteElementRight,
                pointScaleFactor,
                textRounding && hasFractionalWidth,
                textRounding && !hasFractionalWidth)
            - Number.RoundValue(
                absoluteElementLeft,
                pointScaleFactor,
                false,
                textRounding);

        element.Layout.Height =
            Number.RoundValue(
                absoluteElementBottom,
                pointScaleFactor,
                textRounding && hasFractionalHeight,
                textRounding && !hasFractionalHeight)
            - Number.RoundValue(
                absoluteElementTop,
                pointScaleFactor,
                false,
                textRounding);

        if (element.Parent != null)
        {
            element.Layout.Bounds =
                new RectF(
                    element.Parent.Layout.Bounds.Left + element.Layout.Left,
                    element.Parent.Layout.Bounds.Top + element.Layout.Top,
                    element.Layout.Width,
                    element.Layout.Height);
        }
        else
        {
            element.Layout.Bounds = new RectF(element.Layout.Left, element.Layout.Top, element.Layout.Width, element.Layout.Height);
        }

        var elementCount = element.Children.Count;
        for (var i = 0; i < elementCount; i++)
        {
            RoundToPixelGrid(
                element.Children[i],
                pointScaleFactor,
                absoluteElementLeft,
                absoluteElementTop);
        }
    }

    //**********************************************

    private static void ResolveDimension(IElement element)
    {
        if (!element.Plan.MaxDimension(Dimension.Width).IsUndefined &&
            element.Plan.MaxDimension(Dimension.Width) == element.Plan.MinDimension(Dimension.Width))
            element.Layout.ResolvedWidth = element.Plan.MaxDimension(Dimension.Width);
        else
            element.Layout.ResolvedWidth = element.Plan.Dimension(Dimension.Width);

        if (!element.Plan.MaxDimension(Dimension.Height).IsUndefined &&
            element.Plan.MaxDimension(Dimension.Height) == element.Plan.MinDimension(Dimension.Height))
            element.Layout.ResolvedHeight = element.Plan.MaxDimension(Dimension.Height);
        else
            element.Layout.ResolvedHeight = element.Plan.Dimension(Dimension.Height);
    }

    private static bool IsLayoutDirectionInPlan(IElement element, LayoutDirection direction, float containerSize)
    {
        var number = element.Layout.GetResolvedDimension(direction.ToDimension());

        return !(number.Unit == Number.UoM.Auto
                 || number.Unit == Number.UoM.Undefined
                 || number.Unit == Number.UoM.Point && !number.IsUndefined && number.Value < 0.0f
                 || number.Unit == Number.UoM.Percent && !number.IsUndefined &&
                 (number.Value < 0.0f || containerSize.IsUndefined()));
    }

    //**********************************************

    public static float ResolveFlexGrow(IElement element)
    {
        // Root elements flexGrow should always be 0
        if (element.Parent == null)
            return 0.0f;

        if (element.Plan.FlexGrow.HasValue())
            return element.Plan.FlexGrow;

        if (element.Plan.Flex.HasValue() && element.Plan.Flex > 0.0f)
            return element.Plan.Flex;

        return LayoutPlan.DefaultFlexGrow;
    }

    public static float ResolveFlexShrink(IElement element)
    {
        if (element.Parent == null)
            return 0.0f;

        if (element.Plan.FlexShrink.HasValue())
            return element.Plan.FlexShrink;

        if (element.Plan.Flex.HasValue() && element.Plan.Flex < 0.0f)
            return -element.Plan.Flex;

        return LayoutPlan.DefaultFlexShrink;
    }

    public static bool IsElementFlexible(IElement element) =>
        element.Plan.PositionType == PositionType.Relative
        && (ResolveFlexGrow(element).IsNotZero() || ResolveFlexShrink(element).IsNotZero());

    //**********************************************

    public static float GetLeadingMargin(IElement element, LayoutDirection direction, float widthSize)
    {
        if (IsHorz(direction) && !element.Plan.Margin[Side.Start].IsUndefined)
        {
            return element.Plan.Margin[Side.Start].ResolveValueMargin(widthSize);
        }

        return element.Plan.Margin.ComputedEdgeValue(direction.ToLeadingSide(), Number.Zero).ResolveValueMargin(widthSize);
    }

    public static float GetTrailingMargin(IElement element, LayoutDirection direction, float widthSize)
    {
        if (IsHorz(direction)
            && !element.Plan.Margin[Side.End].IsUndefined)
        {
            return element.Plan.Margin[Side.End].ResolveValueMargin(widthSize);
        }

        return element.Plan.Margin.ComputedEdgeValue(direction.ToTrailingSide(), Number.Zero).ResolveValueMargin(widthSize);
    }

    public static float GetLeadingPadding(IElement element, LayoutDirection direction, float widthSize)
    {
        var paddingEdgeStart = element.Plan.Padding[Side.Start]
                                   .Resolve(widthSize);

        if (IsHorz(direction)
            && !element.Plan.Padding[Side.Start].IsUndefined
            && paddingEdgeStart.HasValue()
            && paddingEdgeStart >= 0.0f)
            return paddingEdgeStart;

        var resolvedValue = element.Plan.Padding.ComputedEdgeValue(direction.ToLeadingSide(), Number.Zero)
                                .Resolve(widthSize);

        return Math.Max(resolvedValue, 0f);
    }

    public static float GetTrailingPadding(IElement element, LayoutDirection direction, float widthSize)
    {
        var paddingEdgeEnd = element.Plan.Padding[Side.End]
                                 .Resolve(widthSize);

        if (IsHorz(direction) && paddingEdgeEnd >= 0f)
            return paddingEdgeEnd;

        var resolvedValue = element.Plan.Padding.ComputedEdgeValue(direction.ToTrailingSide(), Number.Zero)
                                .Resolve(widthSize);

        return Math.Max(resolvedValue, 0f);
    }

    public static float PaddingForDirection(IElement element, LayoutDirection direction, float widthSize) =>
        GetLeadingPadding(element, direction, widthSize) + GetTrailingPadding(element, direction, widthSize);

    public static float DimWithMargin(IElement element, LayoutDirection direction, float widthSize) =>
        element.Layout.MeasuredDimensions[direction.ToDimension()]
        + GetLeadingMargin(element, direction, widthSize)
        + GetTrailingMargin(element, direction, widthSize);

    //**********************************************

    public static void SetPosition(
        IElement element,
        float layoutDirectionlength,
        float crossLength,
        float containerWidth)
    {
        var layoutDirection = element.Plan.ElementsDirection;
        var crossDirection = ResolveCrossDirection(layoutDirection);

        var relativePositionLayoutDirection = RelativePosition(element, layoutDirection, layoutDirectionlength);
        var relativePositionCross = RelativePosition(element, crossDirection, crossLength);

        element.Layout.Position[layoutDirection.ToLeadingSide()] = GetLeadingMargin(element, layoutDirection, containerWidth) + relativePositionLayoutDirection;
        element.Layout.Position[layoutDirection.ToTrailingSide()] = GetTrailingMargin(element, layoutDirection, containerWidth) + relativePositionLayoutDirection;
        element.Layout.Position[crossDirection.ToLeadingSide()] = GetLeadingMargin(element, crossDirection, containerWidth) + relativePositionCross;
        element.Layout.Position[crossDirection.ToTrailingSide()] = GetTrailingMargin(element, crossDirection, containerWidth) + relativePositionCross;
    }

    // If both left and right are defined, then use left. Otherwise return +left or
    // -right depending on which is defined.
    private static float RelativePosition(IElement element, LayoutDirection direction, float directionSize)
    {
        if (IsLeadingPositionDefined(element, direction))
            return GetLeadingPosition(element, direction, directionSize);

        var trailingPosition = GetTrailingPosition(element, direction, directionSize);
        if (trailingPosition.HasValue())
            trailingPosition = -1f * trailingPosition;

        return trailingPosition;
    }

    //**********************************************

    public static bool IsLeadingPositionDefined(IElement element, LayoutDirection direction) =>
        IsHorz(direction)
        && element.Plan.Position.ComputedEdgeValue(Side.Start).HasValue
        || element.Plan.Position.ComputedEdgeValue(direction.ToLeadingSide()).HasValue;

    public static bool IsTrailingPosDefined(IElement element, LayoutDirection direction) =>
        IsHorz(direction)
        && !element.Plan.Position.ComputedEdgeValue(Side.End).IsUndefined
        || !element.Plan.Position.ComputedEdgeValue(direction.ToTrailingSide()).IsUndefined;

    public static float GetLeadingPosition(IElement element, LayoutDirection direction, float directionSize)
    {
        if (IsHorz(direction))
        {
            var lp = element.Plan.Position.ComputedEdgeValue(Side.Start);

            if (!lp.IsUndefined)
                return lp.Resolve(directionSize);
        }

        var leadingPosition = element.Plan.Position.ComputedEdgeValue(direction.ToLeadingSide());

        return leadingPosition.IsUndefined
            ? 0f
            : leadingPosition.Resolve(directionSize);
    }

    public static float GetTrailingPosition(IElement element, LayoutDirection direction, float directionSize)
    {
        if (IsHorz(direction))
        {
            var tp = element.Plan.Position.ComputedEdgeValue(Side.End);

            if (!tp.IsUndefined)
                return tp.Resolve(directionSize);
        }

        var trailingPosition = element.Plan.Position.ComputedEdgeValue(direction.ToTrailingSide());
        return trailingPosition.IsUndefined
            ? 0f
            : trailingPosition.Resolve(directionSize);
    }

    //**********************************************

    public static float BoundDirectionWithinMinAndMax(IElement element, LayoutDirection direction, float value, float directionSize)
    {
        var min = float.NaN;
        var max = float.NaN;

        if (IsVert(direction))
        {
            min = element.Plan.MinHeight.Resolve(directionSize);
            max = element.Plan.MaxHeight.Resolve(directionSize);
        }
        else if (IsHorz(direction))
        {
            min = element.Plan.MinWidth.Resolve(directionSize);
            max = element.Plan.MaxWidth.Resolve(directionSize);
        }

        if (max >= 0f && value > max) return max;

        if (min >= 0f && value < min) return min;

        return value;
    }

    // Like BoundDirectionWithinMinAndMax but also ensures that the value doesn't go below the padding amount.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float BoundDirection(IElement element, LayoutDirection direction, float value, float directionSize, float widthSize) =>
        FloatMax(
            BoundDirectionWithinMinAndMax(element, direction, value, directionSize),
            GetLeadingPadding(element, direction, widthSize));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AlignmentLine_Cross AlignElement(IElement container, IElement subElement)
    {
        var align = subElement.Plan.AlignSelf_Cross == AlignmentLine_Cross.Auto
            ? container.Plan.Align_Cross
            : subElement.Plan.AlignSelf_Cross;

        return align;
    }

    public static void SetElementTrailingPosition(IElement container, IElement subElement, LayoutDirection direction)
    {
        var size = subElement.Layout.MeasuredDimensions[direction.ToDimension()];
        subElement.Layout.Position[direction.ToTrailingSide()] =
            container.Layout.MeasuredDimensions[direction.ToDimension()]
            - size
            - subElement.Layout.Position[direction.ToPosition()];
    }

    //**********************************************

    public static void ConstrainMaxSizeForMode(
        IElement element,
        LayoutDirection direction,
        float containerDirectionSize,
        float containerWidth,
        ref MeasureMode mode,
        ref float size)
    {
        var maxSize = element.Plan.MaxDimension(direction.ToDimension()).Resolve(containerDirectionSize)
                      + GetMarginForDirection(element, direction, containerWidth);
        switch (mode)
        {
            case MeasureMode.Exactly:
            case MeasureMode.AtMost:
                size = maxSize.IsUndefined() || size < maxSize
                    ? size
                    : maxSize;

                break;
            case MeasureMode.Undefined:
                if (maxSize.HasValue())
                {
                    mode = MeasureMode.AtMost;
                    size = maxSize;
                }

                break;
        }
    }

    public static float GetMarginForDirection(IElement element, LayoutDirection direction, float widthSize) =>
        GetLeadingMargin(element, direction, widthSize) + GetTrailingMargin(element, direction, widthSize);

    public static void SetLayoutMeasuredDimension(IElement element, float measuredDimension, Dimension index) =>
        element.Layout.MeasuredDimensions[index] = measuredDimension;

    public static void SetLayoutMargin(IElement element, float margin, Side edge) => element.Layout.Margin[edge] = margin;

    public static void SetLayoutPadding(IElement element, float padding, Side edge) => element.Layout.Padding[edge] = padding;

    public static void SetLayoutPosition(IElement element, float position, Side side) => element.Layout.Position[side] = position;

    //*********************************************************

    public static bool IsLayoutTreeEqualToElement(IElement element, IElement other)
    {
        if (element.Children.Count != other.Children.Count) return false;

        if (element.Layout != other.Layout) return false;

        if (element.Children.Count == 0) return true;

        for (var i = 0; i < element.Children.Count; ++i)
        {
            if (!IsLayoutTreeEqualToElement(element.Children[i], other.Children[i]))
                return false;
        }

        return true;
    }

    //*********************************************************

    public static Number MarginLeadingValue(IElement element, LayoutDirection direction)
    {
        if (IsHorz(direction) && !element.Plan.Margin[Side.Start].IsUndefined)
            return element.Plan.Margin[Side.Start];

        return element.Plan.Margin[direction.ToLeadingSide()];
    }

    public static Number MarginTrailingValue(IElement element, LayoutDirection direction)
    {
        if (IsHorz(direction) && !element.Plan.Margin[Side.End].IsUndefined)
            return element.Plan.Margin[Side.End];

        return element.Plan.Margin[direction.ToTrailingSide()];
    }

    //*********************************************************

    public static bool IsLayoutDimDefined(IElement element, LayoutDirection direction)
    {
        var value = element.Layout.MeasuredDimensions[direction.ToDimension()];

        return value.HasValue() && value >= 0.0f;
    }

    //**********************************************

    /// <summary>
    ///     Used to indicate which layout process is being run
    /// </summary>
    internal enum LayoutPassReason
    {
        Initial = 0,
        AbsLayout = 1,
        Stretch = 2,
        MultilineStretch = 3,
        FlexLayout = 4,
        MeasureElement = 5,
        AbsMeasureElement = 6,
        FlexMeasure = 7
    }
}