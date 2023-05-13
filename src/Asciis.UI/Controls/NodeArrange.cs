using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Asciis.UI.Controls;

using static NumberExtensions;

public class NodeArrange
{
    internal static readonly Side[]      Leading = { Side.Top, Side.Bottom, Side.Left, Side.Right };
    internal static readonly Side[]      Trailing = { Side.Bottom, Side.Top, Side.Right, Side.Left };
    internal static readonly Side[]      Pos = { Side.Top, Side.Bottom, Side.Left, Side.Right };
    internal static readonly Dimension[] Dim = { Dimension.Height, Dimension.Height, Dimension.Width, Dimension.Width };

    private static int _currentGenerationCount;

    public static void Calculate(
        INode       node,
        float       containerWidth       = Number.ValueUndefined,
        float       containerHeight      = Number.ValueUndefined,
        UIDirection containerUIDirection = UIDirection.Ltr,
        float       pointScaleFactor     = 1f,
        bool        printTree            = false)
    {
        new NodeArrange()
           .CalculateLayout(
                node,
                containerWidth,
                containerHeight,
                containerUIDirection,
                pointScaleFactor,
                printTree);
    }

    private void CalculateLayout(
        INode    node,
        float       containerWidth,
        float       containerHeight,
        UIDirection containerUIDirection,
        float       pointScaleFactor,
        bool        printTree)
    {
        float width,
              height;
        MeasureMode widthMeasureMode,
                    heightMeasureMode;

        // Increment the generation count. This will force the recursive routine to
        // visit all dirty nodes at least once. Subsequent visits will be skipped if
        // the input parameters don't change.
        _currentGenerationCount++;

        ResolveDimension(node);

        if (IsLayoutDirectionInPlan(node, LayoutDirection.Row, containerWidth))
        {
            width = node.Layout.GetResolvedDimension(Dim[(int)LayoutDirection.Row]).Resolve(containerWidth)
                    + GetMarginForAxis(node, LayoutDirection.Row, containerWidth);
            widthMeasureMode = MeasureMode.Exactly;
        }
        else if (node.Plan.MaxWidth.Resolve(containerWidth).HasValue())
        {
            width            = node.Plan.MaxWidth.Resolve(containerWidth);
            widthMeasureMode = MeasureMode.AtMost;
        }
        else
        {
            width = containerWidth;
            widthMeasureMode = width.IsUndefined()
                ? MeasureMode.Undefined
                : MeasureMode.Exactly;
        }

        if (IsLayoutDirectionInPlan(node, LayoutDirection.Column, containerHeight))
        {
            height = node.Layout.GetResolvedDimension(Dim[(int)LayoutDirection.Column]).Resolve(containerHeight)
                     + GetMarginForAxis(node, LayoutDirection.Column, containerWidth);
            heightMeasureMode = MeasureMode.Exactly;
        }
        else if (node.Plan.MaxHeight.Resolve(containerHeight).HasValue())
        {
            height            = node.Plan.MaxHeight.Resolve(containerHeight);
            heightMeasureMode = MeasureMode.AtMost;
        }
        else
        {
            height = containerHeight;
            heightMeasureMode = height.IsUndefined()
                ? MeasureMode.Undefined
                : MeasureMode.Exactly;
        }

        if (LayoutNode(
                node,
                width,
                height,
                containerUIDirection,
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
                node,
                node.Layout.UIDirection,
                containerWidth,
                containerHeight,
                containerWidth);
            RoundToPixelGrid(node, pointScaleFactor, 0f, 0f);

        #if DEBUG
            //if (printTree)
            //    Logger.Log(LogLevel.Verbose, NodePrint.Format(node));
        #endif
        }
    }

    /// <summary>
    /// This is a wrapper around the LayoutImplementation function. It determines whether
    /// the layout request is redundant and can be skipped.
    /// </summary>
    private bool LayoutNode(
        INode            node,
        float            availableWidth,
        float            availableHeight,
        UIDirection      containerUIDirection,
        MeasureMode      widthMeasureMode,
        MeasureMode      heightMeasureMode,
        float            containerWidth,
        float            containerHeight,
        bool             performLayout,
        LayoutPassReason reason,
        int              depth,
        int              generationCount)
    {
        var                layout        = node.Layout;
        CachedMeasurement? cachedResults = null;

        depth++;

        var needToVisitNode =
            node.IsDirty && layout.GenerationCount != generationCount
            || layout.LastContainerUIDirection != containerUIDirection;

        if (needToVisitNode)
        {
            // Invalidate the cached results.
            layout.NextCachedMeasurementsIndex    = 0;
            layout.CachedLayout.WidthMeasureMode  = MeasureMode.Undefined;
            layout.CachedLayout.HeightMeasureMode = MeasureMode.Undefined;
            layout.CachedLayout.ComputedWidth     = -1;
            layout.CachedLayout.ComputedHeight    = -1;
        }

        // Determine whether the results are already cached. We maintain a separate
        // cache for layouts and measurements. A layout operation modifies the
        // positions and dimensions for nodes in the subtree. The algorithm assumes
        // that each node gets laid out a maximum of one time per tree layout, but
        // multiple measurements may be required to resolve all of the flex
        // dimensions. We handle nodes with measure functions specially here because
        // they are the most expensive to measure, so it's worth avoiding redundant
        // measurements if at all possible.
        if (node.MeasureNode != null)
        {
            var marginAxisRow    = GetMarginForAxis(node, LayoutDirection.Row, containerWidth);
            var marginAxisColumn = GetMarginForAxis(node, LayoutDirection.Column, containerWidth);

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
                    marginAxisRow,
                    marginAxisColumn))
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
                            marginAxisRow,
                            marginAxisColumn))
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

        if (!needToVisitNode && cachedResults != null)
        {
            layout.MeasuredDimensions.Width  = cachedResults.ComputedWidth;
            layout.MeasuredDimensions.Height = cachedResults.ComputedHeight;

            //if (PrintChanges && PrintSkips)
            //{
            //    Logger.Log(LogLevel.Verbose, $"{Spacer(depth)}{depth}.([skipped] ");
            //    Logger.Log(LogLevel.Verbose, new NodePrint(node).ToString());
            //    Logger.Log(LogLevel.Verbose,
            //               $"wm: {MeasureModeName(widthMeasureMode, performLayout)}, hm: {MeasureModeName(heightMeasureMode, performLayout)}, aw: {availableWidth} ah: {availableHeight} => d: ({cachedResults.ComputedWidth}, {cachedResults.ComputedHeight}) {reason}\n");
            //}
        }
        else
        {
            //if (PrintChanges)
            //{
            //    Logger.Log(LogLevel.Verbose, $"{Spacer(depth)}{depth}.({(needToVisitNode ? "*" : "")}");
            //    Logger.Log(LogLevel.Verbose, new NodePrint(node).ToString());
            //    Logger.Log(LogLevel.Verbose,
            //               $"wm: {MeasureModeName(widthMeasureMode, performLayout)}, hm: {MeasureModeName(heightMeasureMode, performLayout)}, aw: {availableWidth} ah: {availableHeight} {reason}\n");
            //}

            LayoutImplementation(
                node,
                availableWidth,
                availableHeight,
                containerUIDirection,
                widthMeasureMode,
                heightMeasureMode,
                containerWidth,
                containerHeight,
                performLayout,
                depth,
                generationCount);

            //if (PrintChanges)
            //{
            //    Logger.Log(LogLevel.Verbose, $"{Spacer(depth)}{depth}.){(needToVisitNode ? "*" : "")}");
            //    Logger.Log(LogLevel.Verbose, new NodePrint(node).ToString());
            //    Logger.Log(LogLevel.Verbose,
            //               $"wm: {MeasureModeName(widthMeasureMode, performLayout)}, hm: {MeasureModeName(heightMeasureMode, performLayout)}, d: ({layout.MeasuredDimensions.Width}, {layout.MeasuredDimensions.Height}) {reason}\n");
            //}

            layout.LastContainerUIDirection = containerUIDirection;

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

                newCacheEntry.AvailableWidth    = availableWidth;
                newCacheEntry.AvailableHeight   = availableHeight;
                newCacheEntry.WidthMeasureMode  = widthMeasureMode;
                newCacheEntry.HeightMeasureMode = heightMeasureMode;
                newCacheEntry.ComputedWidth     = layout.MeasuredDimensions.Width;
                newCacheEntry.ComputedHeight    = layout.MeasuredDimensions.Height;
            }
        }

        if (performLayout)
        {
            node.Layout.Width  = node.Layout.MeasuredDimensions.Width;
            node.Layout.Height = node.Layout.MeasuredDimensions.Height;
            node.IsDirty       = false;
        }

        layout.GenerationCount = generationCount;

        return needToVisitNode || cachedResults == null;
    }

    /// <summary>
    /// Partial implementation of FlexBox https://www.w3.org/TR/CSS3-flexbox/.
    ///
    /// Limitations
    ///  * Display property is always assumed to be 'flex' except for Text nodes, which are assumed to be 'inline-flex'.
    ///  * The 'zIndex' property (or any form of z ordering) is not supported. Nodes are stacked in document order.
    ///  * The 'order' property is not supported. The order of flex items is always defined by document order.
    ///  * The 'visibility' property is always assumed to be 'visible'. Values of 'collapse' and 'hidden' are not supported.
    ///  * There is no support for forced breaks.
    ///  * It does not support vertical inline directions (top-to-bottom or bottom-to-top text).
    ///
    /// Deviations 
    ///  * Section 4.5 of the spec indicates that all flex items have a default minimum main size. For text blocks,
    ///    for example, this is the width of the widest word. Calculating the minimum width is expensive, so we forego it
    ///    and assume a default minimum main size of 0.
    ///  * Min/Max sizes in the main axis are not honoured when resolving flexible lengths.
    ///  * The spec indicates that the default value for 'flexDirection' is 'row', but the algorithm below assumes a
    ///    default of 'column'.
    ///
    /// Details
    ///    This routine is called recursively to lay out subtrees of flexbox
    ///    elements. It uses the information in node.style, which is treated as a
    ///    read-only input. It is responsible for setting the layout.direction and
    ///    layout.measuredDimensions fields for the input node as well as the
    ///    layout.position and layout.lineIndex fields for its elements. The
    ///    layout.measuredDimensions field includes any border or padding for the
    ///    node but does not include margins.
    ///
    ///    The spec describes four different layout modes: "fill available", "max
    ///    content", "min content", and "fit content". Of these, we don't use "min
    ///    content" because we don't support default minimum main sizes (see above
    ///    for details). Each of our measure modes maps to a layout mode from the
    ///    spec (https://www.w3.org/TR/CSS3-sizing/#terms):
    ///      - MeasureMode.Undefined: max content
    ///      - MeasureMode.Exactly: fill available
    ///      - MeasureMode.AtMost: fit content
    ///
    ///    When calling LayoutImplementation and LayoutNode, if the caller
    ///    passes an available size of undefined then it must also pass a measure
    ///    mode of MeasureMode.Undefined in that dimension.
    ///
    /// </summary>
    /// <param name="node">current node to be sized and laid out</param>
    /// <param name="availableWidth">available width to be used for sizing</param>
    /// <param name="availableHeight">available width to be used for sizing</param>
    /// <param name="containerUIDirection">the inline (text) direction within the container (left-to-right or right-to-left)</param>
    /// <param name="widthMeasureMode">indicates the sizing rules for the width</param>
    /// <param name="heightMeasureMode">indicates the sizing rules for the height</param>
    /// <param name="containerWidth"></param>
    /// <param name="containerHeight"></param>
    /// <param name="performLayout">specifies whether the caller is interested in just the dimensions of the node or it requires the entire node and its subtree to be laid out (with final positions)</param>
    /// <param name="depth"></param>
    /// <param name="generationCount"></param>
    private void LayoutImplementation(
        INode       node,
        float       availableWidth,
        float       availableHeight,
        UIDirection containerUIDirection,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        float       containerWidth,
        float       containerHeight,
        bool        performLayout,
        int         depth,
        int         generationCount)
    {
        Debug.Assert(
            availableWidth.HasValue() || widthMeasureMode == MeasureMode.Undefined,
            "availableWidth is indefinite so widthMeasureMode must be MeasureMode.Undefined");
        Debug.Assert(
            availableHeight.HasValue() || heightMeasureMode == MeasureMode.Undefined,
            "availableHeight is indefinite so heightMeasureMode must be MeasureMode.Undefined");

        // Set the resolved resolution in the node's layout.
        var uiDirection = ResolveUIDirection(node, containerUIDirection);
        node.Layout.UIDirection = uiDirection;

        var flexRowDirection    = ResolveLayoutDirection(LayoutDirection.Row, uiDirection);
        var flexColumnDirection = ResolveLayoutDirection(LayoutDirection.Column, uiDirection);

        var startEdge = uiDirection == UIDirection.Ltr
            ? Side.Left
            : Side.Right;
        var endEdge = uiDirection == UIDirection.Ltr
            ? Side.Right
            : Side.Left;

        SetLayoutMargin(node, GetLeadingMargin(node, flexRowDirection, containerWidth), startEdge);
        SetLayoutMargin(node, GetTrailingMargin(node, flexRowDirection, containerWidth), endEdge);
        SetLayoutMargin(node, GetLeadingMargin(node, flexColumnDirection, containerWidth), Side.Top);
        SetLayoutMargin(node, GetTrailingMargin(node, flexColumnDirection, containerWidth), Side.Bottom);

        SetLayoutBorder(node, GetLeadingBorder(node, flexRowDirection), startEdge);
        SetLayoutBorder(node, GetTrailingBorder(node, flexRowDirection), endEdge);
        SetLayoutBorder(node, GetLeadingBorder(node, flexColumnDirection), Side.Top);
        SetLayoutBorder(node, GetTrailingBorder(node, flexColumnDirection), Side.Bottom);

        SetLayoutPadding(node, GetLeadingPadding(node, flexRowDirection, containerWidth), startEdge);
        SetLayoutPadding(node, GetTrailingPadding(node, flexRowDirection, containerWidth), endEdge);
        SetLayoutPadding(node, GetLeadingPadding(node, flexColumnDirection, containerWidth), Side.Top);
        SetLayoutPadding(node, GetTrailingPadding(node, flexColumnDirection, containerWidth), Side.Bottom);

        if (node.MeasureNode != null)
        {
            SetMeasuredDimensions_MeasureFunc(
                node,
                availableWidth,
                availableHeight,
                widthMeasureMode,
                heightMeasureMode,
                containerWidth,
                containerHeight);

            return;
        }

        var elementCount = node.Children.Count;
        if (elementCount == 0)
        {
            SetMeasuredDimensions_EmptyContainer(
                node,
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
                node,
                availableWidth,
                availableHeight,
                widthMeasureMode,
                heightMeasureMode,
                containerWidth,
                containerHeight)) return;

        // At this point we know we're going to perform work. Ensure that each element
        // has a mutable copy.
        //node.cloneElementsIfNeeded();

        // Reset layout flags, as they could have changed.
        node.Layout.HadOverflow = false;

        // STEP 1: CALCULATE VALUES FOR REMAINDER OF ALGORITHM
        var mainAxis      = ResolveLayoutDirection(node.Plan.LayoutDirection, uiDirection);
        var crossAxis     = ResolveCrossAxisLayoutDirection(mainAxis, uiDirection);
        var isMainAxisRow = IsRow(mainAxis);
        var isLayoutWrap  = node.Plan.LayoutWrap != LayoutWrap.NoWrap;

        var mainAxisContainerSize = isMainAxisRow
            ? containerWidth
            : containerHeight;
        var crossAxisContainerSize = isMainAxisRow
            ? containerHeight
            : containerWidth;

        var leadingPaddingAndBorderCross = GetLeadingPaddingAndBorder(node, crossAxis, containerWidth);
        var paddingAndBorderAxisMain     = PaddingAndBorderForAxis(node, mainAxis, containerWidth);
        var paddingAndBorderAxisCross    = PaddingAndBorderForAxis(node, crossAxis, containerWidth);

        var measureModeMainDim = isMainAxisRow
            ? widthMeasureMode
            : heightMeasureMode;
        var measureModeCrossDim = isMainAxisRow
            ? heightMeasureMode
            : widthMeasureMode;

        var paddingAndBorderAxisRow = isMainAxisRow
            ? paddingAndBorderAxisMain
            : paddingAndBorderAxisCross;
        var paddingAndBorderAxisColumn = isMainAxisRow
            ? paddingAndBorderAxisCross
            : paddingAndBorderAxisMain;

        var marginAxisRow    = GetMarginForAxis(node, LayoutDirection.Row, containerWidth);
        var marginAxisColumn = GetMarginForAxis(node, LayoutDirection.Column, containerWidth);

        var minInnerWidth  = node.Plan.MinWidth.Resolve(containerWidth) - paddingAndBorderAxisRow;
        var maxInnerWidth  = node.Plan.MaxWidth.Resolve(containerWidth) - paddingAndBorderAxisRow;
        var minInnerHeight = node.Plan.MinHeight.Resolve(containerHeight) - paddingAndBorderAxisColumn;
        var maxInnerHeight = node.Plan.MaxHeight.Resolve(containerHeight) - paddingAndBorderAxisColumn;

        var minInnerMainDim = isMainAxisRow
            ? minInnerWidth
            : minInnerHeight;
        var maxInnerMainDim = isMainAxisRow
            ? maxInnerWidth
            : maxInnerHeight;

        // STEP 2: DETERMINE AVAILABLE SIZE IN MAIN AND CROSS DIRECTIONS

        var availableInnerWidth = CalculateAvailableInnerDim(
            node,
            LayoutDirection.Row,
            availableWidth,
            containerWidth);
        var availableInnerHeight = CalculateAvailableInnerDim(
            node,
            LayoutDirection.Column,
            availableHeight,
            containerHeight);

        var availableInnerMainDim = isMainAxisRow
            ? availableInnerWidth
            : availableInnerHeight;
        var availableInnerCrossDim = isMainAxisRow
            ? availableInnerHeight
            : availableInnerWidth;

        // STEP 3: DETERMINE MAIN LENGTH FOR EACH ITEM

        var totalOuterMainLength = ComputeMainLengthForElements(
            node,
            availableInnerWidth,
            availableInnerHeight,
            widthMeasureMode,
            heightMeasureMode,
            uiDirection,
            mainAxis,
            performLayout,
            depth,
            generationCount);

        var mainLengthOverflows =
            measureModeMainDim != MeasureMode.Undefined && totalOuterMainLength > availableInnerMainDim;
        if (isLayoutWrap && mainLengthOverflows && measureModeMainDim == MeasureMode.AtMost)
            measureModeMainDim = MeasureMode.Exactly;

        // STEP 4: COLLECT FLEX ITEMS INTO FLEX LINES

        // Indexes of elements that represent the first and last items in the line.
        var startOfLineIndex = 0;
        var endOfLineIndex   = 0;

        // Number of lines.
        var lineCount = 0;

        // Accumulated cross dimensions of all lines so far.
        float totalLineCrossDim = 0;

        // Max main dimension of all the lines.
        float maxLineMainDim = 0;
        for (;
             endOfLineIndex < elementCount;
             lineCount++, startOfLineIndex = endOfLineIndex)
        {
            var collectedFlexItemsValues = CalculateCollectFlexItemsRowValues(
                node,
                containerUIDirection,
                mainAxisContainerSize,
                availableInnerWidth,
                availableInnerMainDim,
                startOfLineIndex,
                lineCount);
            endOfLineIndex = collectedFlexItemsValues.EndOfLineIndex;

            // If we don't need to measure the cross axis, we can skip the entire flex
            // step.
            var canSkipFlex =
                !performLayout && measureModeCrossDim == MeasureMode.Exactly;

            // STEP 5: RESOLVING FLEXIBLE LENGTHS ON MAIN AXIS
            // Calculate the remaining available space that needs to be allocated. If
            // the main dimension size isn't known, it is computed based on the line
            // length, so there's no more space left to distribute.

            var sizeBasedOnContent = false;
            // If we don't measure with exact main dimension we want to ensure we don't
            // violate min and max
            if (measureModeMainDim != MeasureMode.Exactly)
            {
                if (minInnerMainDim.HasValue()
                    && collectedFlexItemsValues.SizeConsumedOnCurrentLine < minInnerMainDim)
                    availableInnerMainDim = minInnerMainDim;
                else if (
                    maxInnerMainDim.HasValue()
                    && collectedFlexItemsValues.SizeConsumedOnCurrentLine > maxInnerMainDim)
                    availableInnerMainDim = maxInnerMainDim;
                else
                {
                    if (collectedFlexItemsValues.TotalFlexGrowFactors.IsUndefined()
                        && collectedFlexItemsValues.TotalFlexGrowFactors.IsZero()
                        || ResolveFlexGrow(node).IsUndefined() && ResolveFlexGrow(node).IsZero())
                    {
                        // If we don't have any elements to flex or we can't flex the node
                        // itself, space we've used is all space we need. Root node also
                        // should be shrunk to minimum
                        availableInnerMainDim = collectedFlexItemsValues.SizeConsumedOnCurrentLine;
                    }

                    sizeBasedOnContent = true;
                }
            }

            if (!sizeBasedOnContent && availableInnerMainDim.HasValue())
            {
                collectedFlexItemsValues.RemainingFreeSpace =
                    availableInnerMainDim - collectedFlexItemsValues.SizeConsumedOnCurrentLine;
            }
            else if (collectedFlexItemsValues.SizeConsumedOnCurrentLine < 0)
            {
                // availableInnerMainDim is indefinite which means the node is being sized
                // based on its content. sizeConsumedOnCurrentLine is negative which means
                // the node will allocate 0 points for its content. Consequently,
                // remainingFreeSpace is 0 - sizeConsumedOnCurrentLine.
                collectedFlexItemsValues.RemainingFreeSpace = -collectedFlexItemsValues.SizeConsumedOnCurrentLine;
            }

            if (!canSkipFlex)
            {
                ResolveFlexibleLength(
                    node,
                    collectedFlexItemsValues,
                    mainAxis,
                    crossAxis,
                    mainAxisContainerSize,
                    availableInnerMainDim,
                    availableInnerCrossDim,
                    availableInnerWidth,
                    availableInnerHeight,
                    mainLengthOverflows,
                    measureModeCrossDim,
                    performLayout,
                    depth,
                    generationCount);
            }

            node.Layout.HadOverflow = node.Layout.HadOverflow | (collectedFlexItemsValues.RemainingFreeSpace < 0);

            // STEP 6: MAIN-AXIS JUSTIFICATION & CROSS-AXIS SIZE DETERMINATION

            // At this point, all the elements have their dimensions set in the main
            // axis. Their dimensions are also set in the cross axis with the exception
            // of items that are aligned "stretch". We need to compute these stretch
            // values and set the final positions.

            JustifyMainAxis(
                node,
                collectedFlexItemsValues,
                startOfLineIndex,
                mainAxis,
                crossAxis,
                measureModeMainDim,
                measureModeCrossDim,
                mainAxisContainerSize,
                containerWidth,
                availableInnerMainDim,
                availableInnerCrossDim,
                availableInnerWidth,
                performLayout);

            var containerCrossAxis = availableInnerCrossDim;
            if (measureModeCrossDim == MeasureMode.Undefined || measureModeCrossDim == MeasureMode.AtMost)
            {
                // Compute the cross axis from the max cross dimension of the elements
                containerCrossAxis =
                    BoundAxis(
                        node,
                        crossAxis,
                        collectedFlexItemsValues.CrossDim + paddingAndBorderAxisCross,
                        crossAxisContainerSize,
                        containerWidth)
                    - paddingAndBorderAxisCross;
            }

            // If there's no flex wrap, the cross dimension is defined by the container.
            if (!isLayoutWrap && measureModeCrossDim == MeasureMode.Exactly)
                collectedFlexItemsValues.CrossDim = availableInnerCrossDim;

            // Clamp to the min/max size specified on the container.
            collectedFlexItemsValues.CrossDim =
                BoundAxis(
                    node,
                    crossAxis,
                    collectedFlexItemsValues.CrossDim + paddingAndBorderAxisCross,
                    crossAxisContainerSize,
                    containerWidth)
                - paddingAndBorderAxisCross;

            // STEP 7: CROSS-AXIS ALIGNMENT
            // We can skip element alignment if we're just measuring the container.
            if (performLayout)
            {
                for (var i = startOfLineIndex; i < endOfLineIndex; i++)
                {
                    var element = (INode)node.Children[i];

                    if (element.Plan.Atomic)
                        continue;

                    if (element.Plan.PositionType == PositionType.Absolute)
                    {
                        // If the element is absolutely positioned and has a
                        // top/left/bottom/right set, override all the previously computed
                        // positions to set it correctly.
                        var isElementLeadingPosDefined = IsLeadingPositionDefined(element, crossAxis);
                        if (isElementLeadingPosDefined)
                        {
                            element.Layout.Position[Pos[(int)crossAxis]] =
                                GetLeadingPosition(element, crossAxis, availableInnerCrossDim)
                                + GetLeadingBorder(node, crossAxis)
                                + GetLeadingMargin(element, crossAxis, availableInnerWidth);
                        }

                        // If leading position is not defined or calculations result in Nan,
                        // default to border + margin
                        if (!isElementLeadingPosDefined
                            || element.Layout.Position[Pos[(int)crossAxis]]
                                      .IsUndefined())
                        {
                            element.Layout.Position[Pos[(int)crossAxis]] =
                                GetLeadingBorder(node, crossAxis)
                                + GetLeadingMargin(element, crossAxis, availableInnerWidth);
                        }
                    }
                    else
                    {
                        var leadingCrossDim = leadingPaddingAndBorderCross;

                        // For a relative elements, we're either using alignElements (container) or
                        // alignSelf (element) in order to determine the position in the cross axis
                        var alignItem = AlignElement(node, element);

                        // If the element uses align stretch, we need to lay it out one more
                        // time, this time forcing the cross-axis size to be the computed
                        // cross size for the current line.
                        if (alignItem == AlignmentCross.Stretch
                            && MarginLeadingValue(element, crossAxis).Unit != Number.UoM.Auto
                            && MarginTrailingValue(element, crossAxis).Unit != Number.UoM.Auto)
                        {
                            // If the element defines a definite size for its cross axis, there's
                            // no need to stretch.
                            if (!IsLayoutDirectionInPlan(element, crossAxis, availableInnerCrossDim))
                            {
                                var elementMainSize = element.Layout.MeasuredDimensions[Dim[(int)mainAxis]];
                                var elementCrossSize = element.Plan.AspectRatio.HasValue()
                                    ? GetMarginForAxis(element, crossAxis, availableInnerWidth)
                                      + (isMainAxisRow
                                          ? elementMainSize / element.Plan.AspectRatio
                                          : elementMainSize * element.Plan.AspectRatio)
                                    : collectedFlexItemsValues.CrossDim;

                                elementMainSize += GetMarginForAxis(element, mainAxis, availableInnerWidth);

                                var elementMainMeasureMode  = MeasureMode.Exactly;
                                var elementCrossMeasureMode = MeasureMode.Exactly;
                                ConstrainMaxSizeForMode(
                                    element,
                                    mainAxis,
                                    availableInnerMainDim,
                                    availableInnerWidth,
                                    ref elementMainMeasureMode,
                                    ref elementMainSize);
                                ConstrainMaxSizeForMode(
                                    element,
                                    crossAxis,
                                    availableInnerCrossDim,
                                    availableInnerWidth,
                                    ref elementCrossMeasureMode,
                                    ref elementCrossSize);

                                var elementWidth = isMainAxisRow
                                    ? elementMainSize
                                    : elementCrossSize;
                                var elementHeight = !isMainAxisRow
                                    ? elementMainSize
                                    : elementCrossSize;

                                var alignContentCross    = node.Plan.AlignContentCross;
                                var crossAxisDoesNotGrow = alignContentCross != AlignmentCross.Stretch && isLayoutWrap;
                                var elementWidthMeasureMode =
                                    elementWidth.IsUndefined() || !isMainAxisRow && crossAxisDoesNotGrow
                                        ? MeasureMode.Undefined
                                        : MeasureMode.Exactly;
                                var elementHeightMeasureMode =
                                    elementHeight.IsUndefined() || isMainAxisRow && crossAxisDoesNotGrow
                                        ? MeasureMode.Undefined
                                        : MeasureMode.Exactly;

                                LayoutNode(
                                    element,
                                    elementWidth,
                                    elementHeight,
                                    uiDirection,
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
                            var remainingCrossDim = containerCrossAxis - DimWithMargin(
                                element,
                                crossAxis,
                                availableInnerWidth);

                            if (MarginLeadingValue(element, crossAxis).Unit == Number.UoM.Auto &&
                                MarginTrailingValue(element, crossAxis).Unit == Number.UoM.Auto)
                                leadingCrossDim += FloatMax(0.0f, remainingCrossDim / 2);
                            else if (MarginTrailingValue(element, crossAxis).Unit == Number.UoM.Auto)
                            {
                                // No-Op
                            }
                            else if (MarginLeadingValue(element, crossAxis).Unit == Number.UoM.Auto)
                                leadingCrossDim += FloatMax(0.0f, remainingCrossDim);
                            else if (alignItem == AlignmentCross.Start)
                            {
                                // No-Op
                            }
                            else if (alignItem == AlignmentCross.Center)
                                leadingCrossDim += remainingCrossDim / 2;
                            else
                                leadingCrossDim += remainingCrossDim;
                        }

                        // And we apply the position
                        element.Layout.Position[Pos[(int)crossAxis]] = element.Layout.Position[Pos[(int)crossAxis]]
                                                                       + totalLineCrossDim
                                                                       + leadingCrossDim;
                    }
                }
            }

            totalLineCrossDim += collectedFlexItemsValues.CrossDim;
            maxLineMainDim    =  FloatMax(maxLineMainDim, collectedFlexItemsValues.MainDim);
        }

        // STEP 8: MULTI-LINE CONTENT ALIGNMENT
        // currentLead stores the size of the cross dim
        if (performLayout && (isLayoutWrap || IsBaselineLayout(node)))
        {
            float crossDimLead = 0;
            var   currentLead  = leadingPaddingAndBorderCross;
            if (availableInnerCrossDim.HasValue())
            {
                var remainingAlignContentDim = availableInnerCrossDim - totalLineCrossDim;
                switch (node.Plan.AlignContentCross)
                {
                case AlignmentCross.End:
                    currentLead += remainingAlignContentDim;

                    break;
                case AlignmentCross.Center:
                    currentLead += remainingAlignContentDim / 2;

                    break;
                case AlignmentCross.Stretch:
                    if (availableInnerCrossDim > totalLineCrossDim)
                        crossDimLead = remainingAlignContentDim / lineCount;

                    break;
                case AlignmentCross.SpaceAround:
                    if (availableInnerCrossDim > totalLineCrossDim)
                    {
                        currentLead += remainingAlignContentDim / (2 * lineCount);
                        if (lineCount > 1)
                            crossDimLead = remainingAlignContentDim / lineCount;
                    }
                    else
                        currentLead += remainingAlignContentDim / 2;

                    break;
                case AlignmentCross.SpaceBetween:
                    if (availableInnerCrossDim > totalLineCrossDim && lineCount > 1)
                        crossDimLead = remainingAlignContentDim / (lineCount - 1);

                    break;
                case AlignmentCross.Auto:
                case AlignmentCross.Start:
                case AlignmentCross.Baseline:
                    break;
                }
            }

            var endIndex = 0;
            for (var i = 0; i < lineCount; i++)
            {
                var startIndex = endIndex;
                int ii;

                // compute the line's height and find the endIndex
                float lineHeight               = 0;
                float maxAscentForCurrentLine  = 0;
                float maxDescentForCurrentLine = 0;
                for (ii = startIndex; ii < elementCount; ii++)
                {
                    var element = (INode)node.Children[ii];

                    if (element.Plan.Atomic)
                        continue;

                    if (element.Plan.PositionType == PositionType.Relative)
                    {
                        if (element.Layout.LineIndex != i)
                            break;

                        if (IsLayoutDimDefined(element, crossAxis))
                        {
                            lineHeight = FloatMax(
                                lineHeight,
                                element.Layout.MeasuredDimensions[Dim[(int)crossAxis]]
                                + GetMarginForAxis(element, crossAxis, availableInnerWidth));
                        }

                        if (AlignElement(node, element) == AlignmentCross.Baseline)
                        {
                            var ascent = Baseline(element)
                                         + GetLeadingMargin(element, LayoutDirection.Column, availableInnerWidth);
                            var descent = element.Layout.MeasuredDimensions.Height
                                          + GetMarginForAxis(element, LayoutDirection.Column, availableInnerWidth)
                                          - ascent;
                            maxAscentForCurrentLine = FloatMax(maxAscentForCurrentLine, ascent);
                            maxDescentForCurrentLine = FloatMax(maxDescentForCurrentLine, descent);
                            lineHeight = FloatMax(lineHeight, maxAscentForCurrentLine + maxDescentForCurrentLine);
                        }
                    }
                }

                endIndex   =  ii;
                lineHeight += crossDimLead;

                if (performLayout)
                {
                    for (ii = startIndex; ii < endIndex; ii++)
                    {
                        var element = (INode)node.Children[ii];

                        if (element.Plan.Atomic)
                            continue;

                        if (element.Plan.PositionType == PositionType.Relative)
                        {
                            switch (AlignElement(node, element))
                            {
                            case AlignmentCross.Start:
                            {
                                element.Layout.Position[Pos[(int)crossAxis]] =
                                    currentLead + GetLeadingMargin(element, crossAxis, availableInnerWidth);

                                break;
                            }
                            case AlignmentCross.End:
                            {
                                element.Layout.Position[Pos[(int)crossAxis]] =
                                    currentLead
                                    + lineHeight
                                    - GetTrailingMargin(element, crossAxis, availableInnerWidth)
                                    - element.Layout.MeasuredDimensions[Dim[(int)crossAxis]];

                                break;
                            }
                            case AlignmentCross.Center:
                            {
                                var elementHeight = element.Layout.MeasuredDimensions[Dim[(int)crossAxis]];

                                element.Layout.Position[Pos[(int)crossAxis]] =
                                    currentLead + (lineHeight - elementHeight) / 2;

                                break;
                            }
                            case AlignmentCross.Stretch:
                            {
                                element.Layout.Position[Pos[(int)crossAxis]] =
                                    currentLead + GetLeadingMargin(element, crossAxis, availableInnerWidth);

                                // Remeasure element with the line height as it as been only
                                // measured with the containers height yet.
                                if (!IsLayoutDirectionInPlan(element, crossAxis, availableInnerCrossDim))
                                {
                                    var elementWidth = isMainAxisRow
                                        ? element.Layout.MeasuredDimensions.Width + GetMarginForAxis(
                                            element,
                                            mainAxis,
                                            availableInnerWidth)
                                        : lineHeight;

                                    var elementHeight = !isMainAxisRow
                                        ? element.Layout.MeasuredDimensions.Height + GetMarginForAxis(
                                            element,
                                            crossAxis,
                                            availableInnerWidth)
                                        : lineHeight;

                                    if (!(FloatsEqual(elementWidth, element.Layout.MeasuredDimensions.Width)
                                          && FloatsEqual(elementHeight, element.Layout.MeasuredDimensions.Height)))
                                    {
                                        LayoutNode(
                                            element,
                                            elementWidth,
                                            elementHeight,
                                            uiDirection,
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
                            case AlignmentCross.Baseline:
                            {
                                element.Layout.Position[Side.Top] =
                                    currentLead
                                    + maxAscentForCurrentLine
                                    - Baseline(element)
                                    + GetLeadingPosition(element, LayoutDirection.Column, availableInnerCrossDim);

                                break;
                            }
                            case AlignmentCross.Auto:
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
            node,
            BoundAxis(
                node,
                LayoutDirection.Row,
                availableWidth - marginAxisRow,
                containerWidth,
                containerWidth),
            (int)Dimension.Width);

        SetLayoutMeasuredDimension(
            node,
            BoundAxis(
                node,
                LayoutDirection.Column,
                availableHeight - marginAxisColumn,
                containerHeight,
                containerWidth),
            Dimension.Height);

        // If the user didn't specify a width or height for the node, set the dimensions based on the elements
        if (measureModeMainDim == MeasureMode.Undefined
            || node.Plan.Overflow != Overflow.Scroll && measureModeMainDim == MeasureMode.AtMost)
        {
            // Clamp the size to the min/max size, if specified, and make sure it
            // doesn't go below the padding and border amount.
            SetLayoutMeasuredDimension(
                node,
                BoundAxis(
                    node,
                    mainAxis,
                    maxLineMainDim,
                    mainAxisContainerSize,
                    containerWidth),
                Dim[(int)mainAxis]);
        }
        else if (measureModeMainDim == MeasureMode.AtMost && node.Plan.Overflow == Overflow.Scroll)
        {
            SetLayoutMeasuredDimension(
                node,
                FloatMax(
                    FloatMin(
                        availableInnerMainDim + paddingAndBorderAxisMain,
                        BoundAxisWithinMinAndMax(
                            node,
                            mainAxis,
                            maxLineMainDim,
                            mainAxisContainerSize)
                    ),
                    paddingAndBorderAxisMain),
                Dim[(int)mainAxis]);
        }

        if (measureModeCrossDim == MeasureMode.Undefined
            || node.Plan.Overflow != Overflow.Scroll && measureModeCrossDim == MeasureMode.AtMost)
        {
            // Clamp the size to the min/max size, if specified, and make sure it
            // doesn't go below the padding and border amount.
            SetLayoutMeasuredDimension(
                node,
                BoundAxis(
                    node,
                    crossAxis,
                    totalLineCrossDim + paddingAndBorderAxisCross,
                    crossAxisContainerSize,
                    containerWidth),
                Dim[(int)crossAxis]);
        }
        else if (measureModeCrossDim == MeasureMode.AtMost && node.Plan.Overflow == Overflow.Scroll)
        {
            SetLayoutMeasuredDimension(
                node,
                FloatMax(
                    FloatMin(
                        availableInnerCrossDim + paddingAndBorderAxisCross,
                        BoundAxisWithinMinAndMax(
                            node,
                            crossAxis,
                            totalLineCrossDim + paddingAndBorderAxisCross,
                            crossAxisContainerSize)
                    ),
                    paddingAndBorderAxisCross),
                Dim[(int)crossAxis]);
        }

        // As we only wrapped in normal direction yet, we need to reverse the
        // positions on wrap-reverse.
        if (performLayout && node.Plan.LayoutWrap == LayoutWrap.WrapReverse)
        {
            for (var i = 0; i < elementCount; i++)
            {
                var element = (INode)node.Children[i];
                if (element.Plan.PositionType == PositionType.Relative)
                {
                    element.Layout.Position[Pos[(int)crossAxis]] =
                        node.Layout.MeasuredDimensions[Dim[(int)crossAxis]]
                        - element.Layout.Position[Pos[(int)crossAxis]]
                        - element.Layout.MeasuredDimensions[Dim[(int)crossAxis]];
                }
            }
        }

        if (performLayout)
        {
            // STEP 10: SIZING AND POSITIONING ABSOLUTE ELEMENTS
            foreach (INode element in node.Children)
            {
                if (element.Plan.PositionType != PositionType.Absolute) continue;

                AbsoluteLayoutElement(
                    node,
                    element,
                    availableInnerWidth,
                    isMainAxisRow
                        ? measureModeMainDim
                        : measureModeCrossDim,
                    availableInnerHeight,
                    uiDirection,
                    depth,
                    generationCount);
            }

            // STEP 11: SETTING TRAILING POSITIONS FOR ELEMENTS
            var needsMainTrailingPos =
                mainAxis == LayoutDirection.RowReverse || mainAxis == LayoutDirection.ColumnReverse;
            var needsCrossTrailingPos =
                crossAxis == LayoutDirection.RowReverse || crossAxis == LayoutDirection.ColumnReverse;

            // Set trailing position if necessary.
            if (needsMainTrailingPos || needsCrossTrailingPos)
            {
                for (var i = 0; i < elementCount; i++)
                {
                    var element = (INode)node.Children[i];

                    if (element.Plan.Atomic)
                        continue;

                    if (needsMainTrailingPos)
                        SetElementTrailingPosition(node, element, mainAxis);

                    if (needsCrossTrailingPos)
                        SetElementTrailingPosition(node, element, crossAxis);
                }
            }
        }
    }

    private bool IsRow(LayoutDirection layoutDirection) =>
        layoutDirection == LayoutDirection.Row || layoutDirection == LayoutDirection.RowReverse;

    private bool IsColumn(LayoutDirection layoutDirection) =>
        layoutDirection == LayoutDirection.Column || layoutDirection == LayoutDirection.ColumnReverse;

    private LayoutDirection ResolveCrossAxisLayoutDirection(LayoutDirection layoutDirection, UIDirection uiDirection) =>
        IsColumn(layoutDirection)
            ? ResolveLayoutDirection(LayoutDirection.Row, uiDirection)
            : LayoutDirection.Column;

    private LayoutDirection ResolveLayoutDirection(LayoutDirection layoutDirection, UIDirection uiDirection)
    {
        if (uiDirection == UIDirection.Rtl)
        {
            if (layoutDirection == LayoutDirection.Row)
                return LayoutDirection.RowReverse;

            if (layoutDirection == LayoutDirection.RowReverse)
                return LayoutDirection.Row;
        }

        return layoutDirection;
    }

    private float ComputeMainLengthForElements(
        INode           node,
        float           availableInnerWidth,
        float           availableInnerHeight,
        MeasureMode     widthMeasureMode,
        MeasureMode     heightMeasureMode,
        UIDirection     uiDirection,
        LayoutDirection mainAxis,
        bool            performLayout,
        int             depth,
        int             generationCount)
    {
        var       totalOuterMainLength = 0.0f;
        INode?    singleFlexElement    = null;
        var       elements             = new List<INode>(node.Children.Cast<INode>());
        var measureModeMainDim = IsRow(mainAxis)
            ? widthMeasureMode
            : heightMeasureMode;

        // If there is only one element with flexGrow + flexShrink it means we can set
        // the computedMainLength to 0 instead of measuring and shrinking / flexing the
        // element to exactly match the remaining space
        if (measureModeMainDim == MeasureMode.Exactly)
        {
            foreach (var element in elements)
            {
                if (IsNodeFlexible(element))
                {
                    if (singleFlexElement != null
                        || FloatsEqual(ResolveFlexGrow(element), 0.0f)
                        || FloatsEqual(ResolveFlexShrink(element), 0.0f))
                    {
                        // There is already a flexible element, or this flexible element doesn't
                        // have flexGrow and flexShrink, abort
                        singleFlexElement = null;

                        break;
                    }

                    singleFlexElement = element;
                }
            }
        }

        foreach (var element in elements)
        {
            ResolveDimension(element);
            if (element.Plan.Atomic)
            {
                ZeroOutLayoutRecursively(element);
                element.IsDirty = false;

                continue;
            }

            if (performLayout)
            {
                // Set the initial position (relative to the container).
                var elementUIDirection = ResolveUIDirection(element, uiDirection);
                var mainDim = IsRow(mainAxis)
                    ? availableInnerWidth
                    : availableInnerHeight;
                var crossDim = IsRow(mainAxis)
                    ? availableInnerHeight
                    : availableInnerWidth;
                SetPosition(
                    element,
                    elementUIDirection,
                    mainDim,
                    crossDim,
                    availableInnerWidth);
            }

            if (element.Plan.PositionType == PositionType.Absolute) continue;

            if (element == singleFlexElement)
            {
                element.Layout.ComputedMainLengthGeneration = generationCount;
                element.Layout.ComputedMainLength           = 0f;
            }
            else
            {
                ComputeMainLengthForElement(
                    node,
                    element,
                    availableInnerWidth,
                    widthMeasureMode,
                    availableInnerHeight,
                    availableInnerWidth,
                    availableInnerHeight,
                    heightMeasureMode,
                    uiDirection,
                    depth,
                    generationCount);
            }

            totalOuterMainLength +=
                element.Layout.ComputedMainLength + GetMarginForAxis(element, mainAxis, availableInnerWidth);
        }

        return totalOuterMainLength;
    }

    private void ComputeMainLengthForElement(
        INode       node,
        INode       element,
        float       width,
        MeasureMode widthMode,
        float       height,
        float       containerWidth,
        float       containerHeight,
        MeasureMode heightMode,
        UIDirection uiDirection,
        int         depth,
        int         generationCount)
    {
        var mainAxis      = ResolveLayoutDirection(node.Plan.LayoutDirection, uiDirection);
        var isMainAxisRow = IsRow(mainAxis);
        var mainAxisSize = isMainAxisRow
            ? width
            : height;
        var mainAxisContainerSize = isMainAxisRow
            ? containerWidth
            : containerHeight;

        var resolvedMainLength      = ResolveMainLength(element.Plan).Resolve(mainAxisContainerSize);
        var isRowStyleDimDefined    = IsLayoutDirectionInPlan(element, LayoutDirection.Row, containerWidth);
        var isColumnStyleDimDefined = IsLayoutDirectionInPlan(element, LayoutDirection.Column, containerHeight);

        if (resolvedMainLength.HasValue() && mainAxisSize.HasValue())
        {
            if (element.Layout.ComputedMainLength.IsUndefined()
                && element.Layout.ComputedMainLengthGeneration != generationCount)
            {
                var paddingAndBorder = PaddingAndBorderForAxis(element, mainAxis, containerWidth);
                element.Layout.ComputedMainLength = Math.Max(resolvedMainLength, paddingAndBorder);
            }
        }
        else if (isMainAxisRow && isRowStyleDimDefined)
        {
            // The width is definite, so use that as the main length.
            var paddingAndBorder = PaddingAndBorderForAxis(element, LayoutDirection.Row, containerWidth);

            element.Layout.ComputedMainLength = Math.Max(
                element.Layout.GetResolvedDimension(Dimension.Width).Resolve(containerWidth),
                paddingAndBorder);
        }
        else if (!isMainAxisRow && isColumnStyleDimDefined)
        {
            // The height is definite, so use that as the main length.
            var paddingAndBorder = PaddingAndBorderForAxis(element, LayoutDirection.Column, containerWidth);
            element.Layout.ComputedMainLength = Math.Max(
                element.Layout.GetResolvedDimension(Dimension.Height).Resolve(containerHeight),
                paddingAndBorder);
        }
        else
        {
            // Compute the main length and hypothetical main size (i.e. the clamped main length).
            var elementWidth             = Number.ValueUndefined;
            var elementHeight            = Number.ValueUndefined;
            var elementWidthMeasureMode  = MeasureMode.Undefined;
            var elementHeightMeasureMode = MeasureMode.Undefined;

            var marginRow    = GetMarginForAxis(element, LayoutDirection.Row, containerWidth);
            var marginColumn = GetMarginForAxis(element, LayoutDirection.Column, containerWidth);

            if (isRowStyleDimDefined)
            {
                elementWidth = element.Layout.GetResolvedDimension(Dimension.Width).Resolve(containerWidth) + marginRow;
                elementWidthMeasureMode = MeasureMode.Exactly;
            }

            if (isColumnStyleDimDefined)
            {
                elementHeight = element.Layout.GetResolvedDimension(Dimension.Height).Resolve(containerHeight) +
                                marginColumn;
                elementHeightMeasureMode = MeasureMode.Exactly;
            }

            // The W3C spec doesn't say anything about the 'overflow' property, but all
            // major browsers appear to implement the following logic.
            if (!isMainAxisRow && node.Plan.Overflow == Overflow.Scroll || node.Plan.Overflow != Overflow.Scroll)
            {
                if (elementWidth.IsUndefined() && width.HasValue())
                {
                    elementWidth            = width;
                    elementWidthMeasureMode = MeasureMode.AtMost;
                }
            }

            if (isMainAxisRow && node.Plan.Overflow == Overflow.Scroll || node.Plan.Overflow != Overflow.Scroll)
            {
                if (elementHeight.IsUndefined() && height.HasValue())
                {
                    elementHeight            = height;
                    elementHeightMeasureMode = MeasureMode.AtMost;
                }
            }

            if (element.Plan.AspectRatio.HasValue())
            {
                if (!isMainAxisRow && elementWidthMeasureMode == MeasureMode.Exactly)
                {
                    elementHeight            = marginColumn + (elementWidth - marginRow) / element.Plan.AspectRatio;
                    elementHeightMeasureMode = MeasureMode.Exactly;
                }
                else if (isMainAxisRow && elementHeightMeasureMode == MeasureMode.Exactly)
                {
                    elementWidth            = marginRow + (elementHeight - marginColumn) * element.Plan.AspectRatio;
                    elementWidthMeasureMode = MeasureMode.Exactly;
                }
            }

            // If the element has no defined size in the cross axis and is set to stretch, set
            // the cross axis to be measured exactly with the available inner width

            var hasExactWidth = width.HasValue() && widthMode == MeasureMode.Exactly;
            var elementWidthStretch = AlignElement(node, element) == AlignmentCross.Stretch
                                      && elementWidthMeasureMode != MeasureMode.Exactly;
            if (!isMainAxisRow && !isRowStyleDimDefined && hasExactWidth && elementWidthStretch)
            {
                elementWidth            = width;
                elementWidthMeasureMode = MeasureMode.Exactly;
                if (element.Plan.AspectRatio.HasValue())
                {
                    elementHeight            = (elementWidth - marginRow) / element.Plan.AspectRatio;
                    elementHeightMeasureMode = MeasureMode.Exactly;
                }
            }

            var hasExactHeight = height.HasValue() && heightMode == MeasureMode.Exactly;
            var elementHeightStretch = AlignElement(node, element) == AlignmentCross.Stretch
                                       && elementHeightMeasureMode != MeasureMode.Exactly;
            if (isMainAxisRow && !isColumnStyleDimDefined && hasExactHeight && elementHeightStretch)
            {
                elementHeight            = height;
                elementHeightMeasureMode = MeasureMode.Exactly;

                if (element.Plan.AspectRatio.HasValue())
                {
                    elementWidth            = (elementHeight - marginColumn) * element.Plan.AspectRatio;
                    elementWidthMeasureMode = MeasureMode.Exactly;
                }
            }

            ConstrainMaxSizeForMode(
                element,
                LayoutDirection.Row,
                containerWidth,
                containerWidth,
                ref elementWidthMeasureMode,
                ref elementWidth);
            ConstrainMaxSizeForMode(
                element,
                LayoutDirection.Column,
                containerHeight,
                containerWidth,
                ref elementHeightMeasureMode,
                ref elementHeight);

            // Measure the element
            LayoutNode(
                element,
                elementWidth,
                elementHeight,
                uiDirection,
                elementWidthMeasureMode,
                elementHeightMeasureMode,
                containerWidth,
                containerHeight,
                false,
                LayoutPassReason.MeasureElement,
                depth,
                generationCount);

            element.Layout.ComputedMainLength = FloatMax(
                element.Layout.MeasuredDimensions[Dim[(int)mainAxis]],
                PaddingAndBorderForAxis(element, mainAxis, containerWidth));
        }

        element.Layout.ComputedMainLengthGeneration = generationCount;
    }

    private Number ResolveMainLength(LayoutPlan plan)
    {
        var mainLength = plan.MainLength;

        if (mainLength.Unit != Number.UoM.Auto && mainLength.Unit != Number.UoM.Undefined)
            return mainLength;

        if (plan.Flex.HasValue() && plan.Flex > 0.0f)
            return Number.Zero;

        return Number.Auto;
    }

    private void AbsoluteLayoutElement(
        INode       container,
        INode       element,
        float       width,
        MeasureMode widthMode,
        float       height,
        UIDirection uiDirection,
        int         depth,
        int         generationCount)
    {
        var mainAxis      = ResolveLayoutDirection(container.Plan.LayoutDirection, uiDirection);
        var crossAxis     = ResolveCrossAxisLayoutDirection(mainAxis, uiDirection);
        var isMainAxisRow = IsRow(mainAxis);

        var elementWidth  = Number.ValueUndefined;
        var elementHeight = Number.ValueUndefined;

        var marginRow    = GetMarginForAxis(element, LayoutDirection.Row, width);
        var marginColumn = GetMarginForAxis(element, LayoutDirection.Column, width);

        if (IsLayoutDirectionInPlan(element, LayoutDirection.Row, width))
            elementWidth = element.Layout.GetResolvedDimension(Dimension.Width).Resolve(width) + marginRow;
        else
        {
            // If the element doesn't have a specified width, compute the width based on
            // the left/right offsets if they're defined.
            if (IsLeadingPositionDefined(element, LayoutDirection.Row)
                && IsTrailingPosDefined(element, LayoutDirection.Row))
            {
                elementWidth = container.Layout.MeasuredDimensions.Width
                               - (GetLeadingBorder(container, LayoutDirection.Row) +
                                  GetTrailingBorder(container, LayoutDirection.Row))
                               - (GetLeadingPosition(element, LayoutDirection.Row, width) + GetTrailingPosition(
                                   element,
                                   LayoutDirection.Row,
                                   width));
                elementWidth = BoundAxis(element, LayoutDirection.Row, elementWidth, width, width);
            }
        }

        if (IsLayoutDirectionInPlan(element, LayoutDirection.Column, height))
            elementHeight = element.Layout.GetResolvedDimension(Dimension.Height).Resolve(height) + marginColumn;
        else
        {
            // If the element doesn't have a specified height, compute the height based on
            // the top/bottom offsets if they're defined.
            if (IsLeadingPositionDefined(element, LayoutDirection.Column)
                && IsTrailingPosDefined(element, LayoutDirection.Column))
            {
                elementHeight = container.Layout.MeasuredDimensions.Height
                                - (GetLeadingBorder(container, LayoutDirection.Column) +
                                   GetTrailingBorder(container, LayoutDirection.Column))
                                - (GetLeadingPosition(element, LayoutDirection.Column, height) + GetTrailingPosition(
                                    element,
                                    LayoutDirection.Column,
                                    height));
                elementHeight = BoundAxis(
                    element,
                    LayoutDirection.Column,
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
            if (element.Plan.AspectRatio.HasValue())
            {
                if (elementWidth.IsUndefined())
                    elementWidth = marginRow + (elementHeight - marginColumn) * element.Plan.AspectRatio;
                else if (elementHeight.IsUndefined())
                    elementHeight = marginColumn + (elementWidth - marginRow) / element.Plan.AspectRatio;
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
            // element to that size as well. This allows text within the absolute element to
            // wrap to the size of its container. This is the same behavior as many browsers
            // implement.
            if (!isMainAxisRow
                && elementWidth.IsUndefined()
                && widthMode != MeasureMode.Undefined
                && width.HasValue()
                && width > 0)
            {
                elementWidth            = width;
                elementWidthMeasureMode = MeasureMode.AtMost;
            }

            LayoutNode(
                element,
                elementWidth,
                elementHeight,
                uiDirection,
                elementWidthMeasureMode,
                elementHeightMeasureMode,
                elementWidth,
                elementHeight,
                false,
                LayoutPassReason.AbsMeasureElement,
                depth,
                generationCount);
            elementWidth = element.Layout.MeasuredDimensions.Width +
                           GetMarginForAxis(element, LayoutDirection.Row, width);
            elementHeight = element.Layout.MeasuredDimensions.Height
                            + GetMarginForAxis(element, LayoutDirection.Column, width);
        }

        LayoutNode(
            element,
            elementWidth,
            elementHeight,
            uiDirection,
            MeasureMode.Exactly,
            MeasureMode.Exactly,
            elementWidth,
            elementHeight,
            true,
            LayoutPassReason.AbsLayout,
            depth,
            generationCount);

        if (IsTrailingPosDefined(element, mainAxis) && !IsLeadingPositionDefined(element, mainAxis))
        {
            element.Layout.Position[Leading[(int)mainAxis]] =
                container.Layout.MeasuredDimensions[Dim[(int)mainAxis]]
                - element.Layout.MeasuredDimensions[Dim[(int)mainAxis]]
                - GetTrailingBorder(container, mainAxis)
                - GetTrailingMargin(element, mainAxis, width)
                - GetTrailingPosition(
                    element,
                    mainAxis,
                    isMainAxisRow
                        ? width
                        : height);
        }
        else if (
            !IsLeadingPositionDefined(element, mainAxis) && container.Plan.AlignContentMain == AlignmentMain.Center)
        {
            element.Layout.Position[Leading[(int)mainAxis]] =
                (container.Layout.MeasuredDimensions[Dim[(int)mainAxis]]
                 - element.Layout.MeasuredDimensions[Dim[(int)mainAxis]])
                / 2.0f;
        }
        else if (!IsLeadingPositionDefined(element, mainAxis) && container.Plan.AlignContentMain == AlignmentMain.End)
        {
            element.Layout.Position[Leading[(int)mainAxis]] =
                container.Layout.MeasuredDimensions[Dim[(int)mainAxis]]
                - element.Layout.MeasuredDimensions[Dim[(int)mainAxis]];
        }

        if (IsTrailingPosDefined(element, crossAxis) && !IsLeadingPositionDefined(element, crossAxis))
        {
            element.Layout.Position[Leading[(int)crossAxis]] =
                container.Layout.MeasuredDimensions[Dim[(int)crossAxis]]
                - element.Layout.MeasuredDimensions[Dim[(int)crossAxis]]
                - GetTrailingBorder(container, crossAxis)
                - GetTrailingMargin(element, crossAxis, width)
                - GetTrailingPosition(
                    element,
                    crossAxis,
                    isMainAxisRow
                        ? height
                        : width);
        }
        else if (!IsLeadingPositionDefined(element, crossAxis) &&
                 AlignElement(container, element) == AlignmentCross.Center)
        {
            element.Layout.Position[Leading[(int)crossAxis]] =
                (container.Layout.MeasuredDimensions[Dim[(int)crossAxis]]
                 - element.Layout.MeasuredDimensions[Dim[(int)crossAxis]])
                / 2.0f;
        }
        else if (!IsLeadingPositionDefined(element, crossAxis)
                 && (AlignElement(container, element) == AlignmentCross.End) ^
                 (container.Plan.LayoutWrap == LayoutWrap.WrapReverse))
        {
            element.Layout.Position[Leading[(int)crossAxis]] =
                container.Layout.MeasuredDimensions[Dim[(int)crossAxis]]
                - element.Layout.MeasuredDimensions[Dim[(int)crossAxis]];
        }
    }

    private void SetMeasuredDimensions_MeasureFunc(
        INode       node,
        float       availableWidth,
        float       availableHeight,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        float       containerWidth,
        float       containerHeight)
    {
        Debug.Assert(
            node.MeasureNode != null,
            "Expected node to have custom measure function");

        var paddingAndBorderAxisRow    = PaddingAndBorderForAxis(node, LayoutDirection.Row, availableWidth);
        var paddingAndBorderAxisColumn = PaddingAndBorderForAxis(node, LayoutDirection.Column, availableWidth);
        var marginAxisRow              = GetMarginForAxis(node, LayoutDirection.Row, availableWidth);
        var marginAxisColumn           = GetMarginForAxis(node, LayoutDirection.Column, availableWidth);

        // We want to make sure we don't call measure with negative size
        var innerWidth = availableWidth.IsUndefined()
            ? availableWidth
            : FloatMax(0, availableWidth - marginAxisRow - paddingAndBorderAxisRow);
        var innerHeight = availableHeight.IsUndefined()
            ? availableHeight
            : FloatMax(0, availableHeight - marginAxisColumn - paddingAndBorderAxisColumn);

        if (widthMeasureMode == MeasureMode.Exactly && heightMeasureMode == MeasureMode.Exactly)
        {
            // Don't bother sizing the text if both dimensions are already defined.
            SetLayoutMeasuredDimension(
                node,
                BoundAxis(
                    node,
                    LayoutDirection.Row,
                    availableWidth - marginAxisRow,
                    containerWidth,
                    containerWidth),
                Dimension.Width);
            SetLayoutMeasuredDimension(
                node,
                BoundAxis(
                    node,
                    LayoutDirection.Column,
                    availableHeight - marginAxisColumn,
                    containerHeight,
                    containerWidth),
                Dimension.Height);
        }
        else
        {
            // Measure the text under the current constraints.
            var measuredSize = node.MeasureNode?.Invoke(
                                   node,
                                   innerWidth,
                                   widthMeasureMode,
                                   innerHeight,
                                   heightMeasureMode)
                               ?? VecF.Zero;

            SetLayoutMeasuredDimension(
                node,
                BoundAxis(
                    node,
                    LayoutDirection.Row,
                    widthMeasureMode == MeasureMode.Undefined || widthMeasureMode == MeasureMode.AtMost
                        ? measuredSize.X + paddingAndBorderAxisRow
                        : availableWidth - marginAxisRow,
                    containerWidth,
                    containerWidth),
                Dimension.Width);

            SetLayoutMeasuredDimension(
                node,
                BoundAxis(
                    node,
                    LayoutDirection.Column,
                    heightMeasureMode == MeasureMode.Undefined || heightMeasureMode == MeasureMode.AtMost
                        ? measuredSize.Y + paddingAndBorderAxisColumn
                        : availableHeight - marginAxisColumn,
                    containerHeight,
                    containerWidth),
                Dimension.Height);
        }
    }

    // For nodes with no elements, use the available values if they were provided,
    // or the minimum size as indicated by the padding and border sizes.
    private void SetMeasuredDimensions_EmptyContainer(
        INode       node,
        float       availableWidth,
        float       availableHeight,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        float       containerWidth,
        float       containerHeight)
    {
        var paddingAndBorderAxisRow    = PaddingAndBorderForAxis(node, LayoutDirection.Row, containerWidth);
        var paddingAndBorderAxisColumn = PaddingAndBorderForAxis(node, LayoutDirection.Column, containerWidth);
        var marginAxisRow              = GetMarginForAxis(node, LayoutDirection.Row, containerWidth);
        var marginAxisColumn           = GetMarginForAxis(node, LayoutDirection.Column, containerWidth);

        SetLayoutMeasuredDimension(
            node,
            BoundAxis(
                node,
                LayoutDirection.Row,
                widthMeasureMode == MeasureMode.Undefined
                || widthMeasureMode == MeasureMode.AtMost
                    ? paddingAndBorderAxisRow
                    : availableWidth - marginAxisRow,
                containerWidth,
                containerWidth),
            Dimension.Width);

        SetLayoutMeasuredDimension(
            node,
            BoundAxis(
                node,
                LayoutDirection.Column,
                heightMeasureMode == MeasureMode.Undefined
                || heightMeasureMode == MeasureMode.AtMost
                    ? paddingAndBorderAxisColumn
                    : availableHeight - marginAxisColumn,
                containerHeight,
                containerWidth),
            Dimension.Height);
    }

    private bool SetMeasuredDimensions_FixedSize(
        INode       node,
        float       availableWidth,
        float       availableHeight,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        float       containerWidth,
        float       containerHeight)
    {
        if (availableWidth.HasValue() && widthMeasureMode == MeasureMode.AtMost && availableWidth <= 0.0f
            || availableHeight.HasValue() && heightMeasureMode == MeasureMode.AtMost && availableHeight <= 0.0f
            || widthMeasureMode == MeasureMode.Exactly && heightMeasureMode == MeasureMode.Exactly)
        {
            var marginAxisColumn = GetMarginForAxis(node, LayoutDirection.Column, containerWidth);
            var marginAxisRow    = GetMarginForAxis(node, LayoutDirection.Row, containerWidth);

            SetLayoutMeasuredDimension(
                node,
                BoundAxis(
                    node,
                    LayoutDirection.Row,
                    availableWidth.IsUndefined()
                    || widthMeasureMode == MeasureMode.AtMost
                    && availableWidth < 0.0f
                        ? 0.0f
                        : availableWidth - marginAxisRow,
                    containerWidth,
                    containerWidth),
                Dimension.Width);

            SetLayoutMeasuredDimension(
                node,
                BoundAxis(
                    node,
                    LayoutDirection.Column,
                    availableHeight.IsUndefined() || heightMeasureMode == MeasureMode.AtMost && availableHeight < 0.0f
                        ? 0.0f
                        : availableHeight - marginAxisColumn,
                    containerHeight,
                    containerWidth),
                Dimension.Height);

            return true;
        }

        return false;
    }

    private void ZeroOutLayoutRecursively(INode container)
    {
        Traverse(
            container,
            n => n.Layout = new LayoutActual { Width = 0f, Height = 0f });
    }

    public void Traverse(INode container, Action<INode> action)
    {
        action(container);
        foreach (var element in container.Children.Cast<INode>())
        {
            Traverse(element, action);
        }
    }

    private float CalculateAvailableInnerDim(
        INode           node,
        LayoutDirection axis,
        float           availableDim,
        float           containerDim)
    {
        var direction = IsRow(axis)
            ? LayoutDirection.Row
            : LayoutDirection.Column;
        var dimension = IsRow(axis)
            ? Dimension.Width
            : Dimension.Height;

        var margin           = GetMarginForAxis(node, direction, containerDim);
        var paddingAndBorder = PaddingAndBorderForAxis(node, direction, containerDim);

        var availableInnerDim = availableDim - margin - paddingAndBorder;
        // Max dimension overrides predefined dimension value; Min dimension in turn
        // overrides both of the above
        if (availableInnerDim.HasValue())
        {
            // We want to make sure our available height does not violate min and max
            // constraints
            var minDimensionOptional = node.Plan.MinDimension(dimension).Resolve(containerDim);
            var minInnerDim = minDimensionOptional.IsUndefined()
                ? 0.0f
                : minDimensionOptional - paddingAndBorder;

            var maxDimensionOptional = node.Plan.MaxDimension(dimension).Resolve(containerDim);

            var maxInnerDim = maxDimensionOptional.IsUndefined()
                ? float.MaxValue
                : maxDimensionOptional - paddingAndBorder;
            availableInnerDim = FloatMax(FloatMin(availableInnerDim, maxInnerDim), minInnerDim);
        }

        return availableInnerDim;
    }

    // This function assumes that all the elements of node have their computedMainLength properly computed
    // (To do this use ComputeMainLengthForElements function).
    // This function calculates YGCollectFlexItemsRowMeasurement
    private CollectFlexItemsRowValues CalculateCollectFlexItemsRowValues(
        INode       node,
        UIDirection containerUIDirection,
        float       mainAxisContainerSize,
        float       availableInnerWidth,
        float       availableInnerMainDim,
        int         startOfLineIndex,
        int         lineCount)
    {
        var flexAlgoRowMeasurement = new CollectFlexItemsRowValues
                                     { RelativeElements = new List<INode>(node.Children.Count) };

        float sizeConsumedOnCurrentLineIncludingMinConstraint = 0;
        var mainAxis = ResolveLayoutDirection(
            node.Plan.LayoutDirection,
            ResolveUIDirection(node, containerUIDirection));
        var isLayoutWrap = node.Plan.LayoutWrap != LayoutWrap.NoWrap;

        // Add items to the current line until it's full or we run out of items.
        var endOfLineIndex = startOfLineIndex;
        for (; endOfLineIndex < node.Children.Count; endOfLineIndex++)
        {
            var element = (INode)node.Children[endOfLineIndex];

            if (element.Plan.Atomic || element.Plan.PositionType == PositionType.Absolute)
                continue;

            element.Layout.LineIndex = lineCount;
            var elementMarginMainAxis = GetMarginForAxis(element, mainAxis, availableInnerWidth);
            var mainLengthWithMinAndMaxConstraints = BoundAxisWithinMinAndMax(
                element,
                mainAxis,
                element.Layout.ComputedMainLength,
                mainAxisContainerSize);

            // If this is a multi-line flow and this item pushes us over the available
            // size, we've hit the end of the current line. Break out of the loop and
            // lay out the current line.
            if (sizeConsumedOnCurrentLineIncludingMinConstraint
                + mainLengthWithMinAndMaxConstraints
                + elementMarginMainAxis
                > availableInnerMainDim
                && isLayoutWrap
                && flexAlgoRowMeasurement.ItemsOnLine > 0)
                break;

            sizeConsumedOnCurrentLineIncludingMinConstraint +=
                mainLengthWithMinAndMaxConstraints + elementMarginMainAxis;
            flexAlgoRowMeasurement.SizeConsumedOnCurrentLine +=
                mainLengthWithMinAndMaxConstraints + elementMarginMainAxis;
            flexAlgoRowMeasurement.ItemsOnLine++;

            if (IsNodeFlexible(element))
            {
                flexAlgoRowMeasurement.TotalFlexGrowFactors += ResolveFlexGrow(element);

                // Unlike the grow factor, the shrink factor is scaled relative to the element dimension.
                flexAlgoRowMeasurement.TotalFlexShrinkScaledFactors +=
                    -ResolveFlexShrink(element) * element.Layout.ComputedMainLength;
            }

            flexAlgoRowMeasurement.RelativeElements.Add(element);
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
    // is removed from the remaingfreespace.
    private void DistributeFreeSpace_FirstPass(
        CollectFlexItemsRowValues collectedFlexItemsValues,
        LayoutDirection           mainAxis,
        float                     mainAxisContainerSize,
        float                     availableInnerMainDim,
        float                     availableInnerWidth)
    {
        float deltaFreeSpace = 0;

        foreach (var currentRelativeElement in collectedFlexItemsValues.RelativeElements.Cast<INode>())
        {
            var elementMainLength =
                BoundAxisWithinMinAndMax(
                    currentRelativeElement,
                    mainAxis,
                    currentRelativeElement.Layout.ComputedMainLength,
                    mainAxisContainerSize);

            float baseMainSize;
            float boundMainSize;
            if (collectedFlexItemsValues.RemainingFreeSpace < 0)
            {
                var flexShrinkScaledFactor = -ResolveFlexShrink(currentRelativeElement) * elementMainLength;

                // Is this element able to shrink?
                if (flexShrinkScaledFactor.HasValue() && flexShrinkScaledFactor.IsNotZero())
                {
                    baseMainSize = elementMainLength
                                   + collectedFlexItemsValues.RemainingFreeSpace
                                   / collectedFlexItemsValues.TotalFlexShrinkScaledFactors
                                   * flexShrinkScaledFactor;
                    boundMainSize = BoundAxis(
                        currentRelativeElement,
                        mainAxis,
                        baseMainSize,
                        availableInnerMainDim,
                        availableInnerWidth);
                    if (baseMainSize.HasValue()
                        && boundMainSize.HasValue()
                        && !FloatsEqual(baseMainSize, boundMainSize))
                    {
                        // By excluding this item's size and flex factor from remaining, this
                        // item's min/max constraints should also trigger in the second pass
                        // resulting in the item's size calculation being identical in the
                        // first and second passes.
                        deltaFreeSpace                                        += boundMainSize - elementMainLength;
                        collectedFlexItemsValues.TotalFlexShrinkScaledFactors -= flexShrinkScaledFactor;
                    }
                }
            }
            else if (collectedFlexItemsValues.RemainingFreeSpace.HasValue()
                     && collectedFlexItemsValues.RemainingFreeSpace > 0)
            {
                var flexGrowFactor = ResolveFlexGrow(currentRelativeElement);

                // Is this element able to grow?
                if (flexGrowFactor.HasValue() && flexGrowFactor.IsNotZero())
                {
                    baseMainSize = elementMainLength
                                   + collectedFlexItemsValues.RemainingFreeSpace
                                   / collectedFlexItemsValues.TotalFlexGrowFactors
                                   * flexGrowFactor;
                    boundMainSize = BoundAxis(
                        currentRelativeElement,
                        mainAxis,
                        baseMainSize,
                        availableInnerMainDim,
                        availableInnerWidth);

                    if (baseMainSize.HasValue()
                        && boundMainSize.HasValue()
                        && !FloatsEqual(baseMainSize, boundMainSize))
                    {
                        // By excluding this item's size and flex factor from remaining, this
                        // item's min/max constraints should also trigger in the second pass
                        // resulting in the item's size calculation being identical in the
                        // first and second passes.
                        deltaFreeSpace                                += boundMainSize - elementMainLength;
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
    private float DistributeFreeSpace_SecondPass(
        CollectFlexItemsRowValues collectedFlexItemsValues,
        INode                  node,
        LayoutDirection           mainAxis,
        LayoutDirection           crossAxis,
        float                     mainAxisContainerSize,
        float                     availableInnerMainDim,
        float                     availableInnerCrossDim,
        float                     availableInnerWidth,
        float                     availableInnerHeight,
        bool                      mainLengthOverflows,
        MeasureMode               measureModeCrossDim,
        bool                      performLayout,
        int                       depth,
        int                       generationCount)
    {
        float deltaFreeSpace = 0;
        var   isMainAxisRow  = IsRow(mainAxis);
        var   isLayoutWrap   = node.Plan.LayoutWrap != LayoutWrap.NoWrap;

        foreach (var currentRelativeElement in collectedFlexItemsValues.RelativeElements.Cast<INode>())
        {
            var elementMainLength = BoundAxisWithinMinAndMax(
                currentRelativeElement,
                mainAxis,
                currentRelativeElement.Layout.ComputedMainLength,
                mainAxisContainerSize);
            var updatedMainSize = elementMainLength;

            if (collectedFlexItemsValues.RemainingFreeSpace.HasValue()
                && collectedFlexItemsValues.RemainingFreeSpace < 0)
            {
                var flexShrinkScaledFactor = -ResolveFlexShrink(currentRelativeElement) * elementMainLength;
                // Is this element able to shrink?
                if (flexShrinkScaledFactor.IsNotZero())
                {
                    float elementSize;

                    if (collectedFlexItemsValues.TotalFlexShrinkScaledFactors.HasValue()
                        && collectedFlexItemsValues.TotalFlexShrinkScaledFactors.IsZero())
                        elementSize = elementMainLength + flexShrinkScaledFactor;
                    else
                    {
                        elementSize = elementMainLength
                                      + collectedFlexItemsValues.RemainingFreeSpace
                                      / collectedFlexItemsValues.TotalFlexShrinkScaledFactors
                                      * flexShrinkScaledFactor;
                    }

                    updatedMainSize = BoundAxis(
                        currentRelativeElement,
                        mainAxis,
                        elementSize,
                        availableInnerMainDim,
                        availableInnerWidth);
                }
            }
            else if (collectedFlexItemsValues.RemainingFreeSpace.HasValue()
                     && collectedFlexItemsValues.RemainingFreeSpace > 0)
            {
                var flexGrowFactor = ResolveFlexGrow(currentRelativeElement);

                // Is this element able to grow?
                if (flexGrowFactor.HasValue() && flexGrowFactor.IsNotZero())
                {
                    updatedMainSize = BoundAxis(
                        currentRelativeElement,
                        mainAxis,
                        elementMainLength
                        + collectedFlexItemsValues.RemainingFreeSpace
                        / collectedFlexItemsValues.TotalFlexGrowFactors
                        * flexGrowFactor,
                        availableInnerMainDim,
                        availableInnerWidth);
                }
            }

            deltaFreeSpace += updatedMainSize - elementMainLength;

            var marginMain  = GetMarginForAxis(currentRelativeElement, mainAxis, availableInnerWidth);
            var marginCross = GetMarginForAxis(currentRelativeElement, crossAxis, availableInnerWidth);

            float elementCrossSize;
            var   elementMainSize         = updatedMainSize + marginMain;
            var   elementCrossMeasureMode = MeasureMode.Undefined;
            var   elementMainMeasureMode  = MeasureMode.Exactly;

            if (currentRelativeElement.Plan.AspectRatio.HasValue())
            {
                elementCrossSize = isMainAxisRow
                    ? (elementMainSize - marginMain) / currentRelativeElement.Plan.AspectRatio
                    : (elementMainSize - marginMain) * currentRelativeElement.Plan.AspectRatio;
                elementCrossMeasureMode = MeasureMode.Exactly;

                elementCrossSize += marginCross;
            }
            else if (
                availableInnerCrossDim.HasValue()
                && !IsLayoutDirectionInPlan(currentRelativeElement, crossAxis, availableInnerCrossDim)
                && measureModeCrossDim == MeasureMode.Exactly
                && !(isLayoutWrap && mainLengthOverflows)
                && AlignElement(node, currentRelativeElement) == AlignmentCross.Stretch
                && MarginLeadingValue(currentRelativeElement, crossAxis).Unit != Number.UoM.Auto
                && MarginTrailingValue(currentRelativeElement, crossAxis).Unit != Number.UoM.Auto)
            {
                elementCrossSize        = availableInnerCrossDim;
                elementCrossMeasureMode = MeasureMode.Exactly;
            }
            else if (!IsLayoutDirectionInPlan(currentRelativeElement, crossAxis, availableInnerCrossDim))
            {
                elementCrossSize = availableInnerCrossDim;
                elementCrossMeasureMode = elementCrossSize.IsUndefined()
                    ? MeasureMode.Undefined
                    : MeasureMode.AtMost;
            }
            else
            {
                elementCrossSize = currentRelativeElement.Layout
                                                         .GetResolvedDimension(Dim[(int)crossAxis])
                                                         .Resolve(availableInnerCrossDim)
                                   + marginCross;
                var isLoosePercentageMeasurement =
                    currentRelativeElement.Layout.GetResolvedDimension(Dim[(int)crossAxis]).Unit == Number.UoM.Percent
                    && measureModeCrossDim != MeasureMode.Exactly;
                elementCrossMeasureMode = elementCrossSize.IsUndefined() || isLoosePercentageMeasurement
                    ? MeasureMode.Undefined
                    : MeasureMode.Exactly;
            }

            ConstrainMaxSizeForMode(
                currentRelativeElement,
                mainAxis,
                availableInnerMainDim,
                availableInnerWidth,
                ref elementMainMeasureMode,
                ref elementMainSize);
            ConstrainMaxSizeForMode(
                currentRelativeElement,
                crossAxis,
                availableInnerCrossDim,
                availableInnerWidth,
                ref elementCrossMeasureMode,
                ref elementCrossSize);

            var requiresStretchLayout = !IsLayoutDirectionInPlan(
                                            currentRelativeElement,
                                            crossAxis,
                                            availableInnerCrossDim)
                                        && AlignElement(node, currentRelativeElement) == AlignmentCross.Stretch
                                        && MarginLeadingValue(currentRelativeElement, crossAxis).Unit != Number.UoM.Auto
                                        && MarginTrailingValue(currentRelativeElement, crossAxis).Unit !=
                                        Number.UoM.Auto;

            var elementWidth = isMainAxisRow
                ? elementMainSize
                : elementCrossSize;
            var elementHeight = !isMainAxisRow
                ? elementMainSize
                : elementCrossSize;

            var elementWidthMeasureMode = isMainAxisRow
                ? elementMainMeasureMode
                : elementCrossMeasureMode;
            var elementHeightMeasureMode = !isMainAxisRow
                ? elementMainMeasureMode
                : elementCrossMeasureMode;

            var isLayoutPass = performLayout && !requiresStretchLayout;
            // Recursively call the layout algorithm for this element with the updated main size.
            LayoutNode(
                currentRelativeElement,
                elementWidth,
                elementHeight,
                node.Layout.UIDirection,
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
            node.Layout.HadOverflow = node.Layout.HadOverflow | currentRelativeElement.Layout.HadOverflow;
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
    private void ResolveFlexibleLength(
        INode                  node,
        CollectFlexItemsRowValues collectedFlexItemsValues,
        LayoutDirection           mainAxis,
        LayoutDirection           crossAxis,
        float                     mainAxisContainerSize,
        float                     availableInnerMainDim,
        float                     availableInnerCrossDim,
        float                     availableInnerWidth,
        float                     availableInnerHeight,
        bool                      mainLengthOverflows,
        MeasureMode               measureModeCrossDim,
        bool                      performLayout,
        int                       depth,
        int                       generationCount)
    {
        var originalFreeSpace = collectedFlexItemsValues.RemainingFreeSpace;
        // First pass: detect the flex items whose min/max constraints trigger
        DistributeFreeSpace_FirstPass(
            collectedFlexItemsValues,
            mainAxis,
            mainAxisContainerSize,
            availableInnerMainDim,
            availableInnerWidth);

        // Second pass: resolve the sizes of the flexible items
        var distributedFreeSpace = DistributeFreeSpace_SecondPass(
            collectedFlexItemsValues,
            node,
            mainAxis,
            crossAxis,
            mainAxisContainerSize,
            availableInnerMainDim,
            availableInnerCrossDim,
            availableInnerWidth,
            availableInnerHeight,
            mainLengthOverflows,
            measureModeCrossDim,
            performLayout,
            depth,
            generationCount);

        collectedFlexItemsValues.RemainingFreeSpace = originalFreeSpace - distributedFreeSpace;
    }

    private void JustifyMainAxis(
        INode                  node,
        CollectFlexItemsRowValues collectedFlexItemsValues,
        int                       startOfLineIndex,
        LayoutDirection           mainAxis,
        LayoutDirection           crossAxis,
        MeasureMode               measureModeMainDim,
        MeasureMode               measureModeCrossDim,
        float                     mainAxisContainerSize,
        float                     containerWidth,
        float                     availableInnerMainDim,
        float                     availableInnerCrossDim,
        float                     availableInnerWidth,
        bool                      performLayout)
    {
        var leadingPaddingAndBorderMain  = GetLeadingPaddingAndBorder(node, mainAxis, containerWidth);
        var trailingPaddingAndBorderMain = GetTrailingPaddingAndBorder(node, mainAxis, containerWidth);
        // If we are using "at most" rules in the main axis, make sure that
        // remainingFreeSpace is 0 when min main dimension is not given
        if (measureModeMainDim == MeasureMode.AtMost && collectedFlexItemsValues.RemainingFreeSpace > 0)
        {
            if (node.Plan.MinDimension(Dim[(int)mainAxis]).HasValue
                && node.Plan.MinDimension(Dim[(int)mainAxis]).Resolve(mainAxisContainerSize).HasValue())
            {
                // This condition makes sure that if the size of main dimension(after
                // considering elements main dim, leading and trailing padding etc)
                // falls below min dimension, then the remainingFreeSpace is reassigned
                // considering the min dimension

                // `minAvailableMainDim` denotes minimum available space in which element
                // can be laid out, it will exclude space consumed by padding and border.
                var minAvailableMainDim = node.Plan.MinDimension(Dim[(int)mainAxis]).Resolve(mainAxisContainerSize)
                                          - leadingPaddingAndBorderMain
                                          - trailingPaddingAndBorderMain;
                var occupiedSpaceByElements = availableInnerMainDim - collectedFlexItemsValues.RemainingFreeSpace;
                collectedFlexItemsValues.RemainingFreeSpace = FloatMax(
                    0,
                    minAvailableMainDim - occupiedSpaceByElements);
            }
            else
                collectedFlexItemsValues.RemainingFreeSpace = 0;
        }

        var numberOfAutoMarginsOnCurrentLine = 0;
        for (var i = startOfLineIndex; i < collectedFlexItemsValues.EndOfLineIndex; i++)
        {
            var element = (INode)node.Children[i];
            if (element.Plan.PositionType == PositionType.Relative)
            {
                if (MarginLeadingValue(element, mainAxis).Unit == Number.UoM.Auto)
                    numberOfAutoMarginsOnCurrentLine++;

                if (MarginTrailingValue(element, mainAxis).Unit == Number.UoM.Auto)
                    numberOfAutoMarginsOnCurrentLine++;
            }
        }

        // In order to position the elements in the main axis, we have two controls.
        // The space between the beginning and the first element and the space between
        // each two elements.
        float leadingMainDim = 0;
        float betweenMainDim = 0;
        var   justifyContent = node.Plan.AlignContentMain;

        if (numberOfAutoMarginsOnCurrentLine == 0)
        {
            switch (justifyContent)
            {
            case AlignmentMain.Center:
                leadingMainDim = collectedFlexItemsValues.RemainingFreeSpace / 2;

                break;
            case AlignmentMain.End:
                leadingMainDim = collectedFlexItemsValues.RemainingFreeSpace;

                break;
            case AlignmentMain.SpaceBetween:
                if (collectedFlexItemsValues.ItemsOnLine > 1)
                {
                    betweenMainDim =
                        FloatMax(collectedFlexItemsValues.RemainingFreeSpace, 0)
                        / (collectedFlexItemsValues.ItemsOnLine - 1);
                }
                else
                    betweenMainDim = 0;

                break;
            case AlignmentMain.SpaceEvenly:
                // Space is distributed evenly across all elements
                betweenMainDim = collectedFlexItemsValues.RemainingFreeSpace /
                                 (collectedFlexItemsValues.ItemsOnLine + 1);
                leadingMainDim = betweenMainDim;

                break;
            case AlignmentMain.SpaceAround:
                // Space on the edges is half of the space between elements
                betweenMainDim = collectedFlexItemsValues.RemainingFreeSpace / collectedFlexItemsValues.ItemsOnLine;
                leadingMainDim = betweenMainDim / 2;

                break;
            case AlignmentMain.Start:
                break;
            }
        }

        collectedFlexItemsValues.MainDim  = leadingPaddingAndBorderMain + leadingMainDim;
        collectedFlexItemsValues.CrossDim = 0;

        float maxAscentForCurrentLine  = 0;
        float maxDescentForCurrentLine = 0;
        var   isNodeBaselineLayout     = IsBaselineLayout(node);
        for (var i = startOfLineIndex; i < collectedFlexItemsValues.EndOfLineIndex; i++)
        {
            var element = (INode)node.Children[i];

            if (element.Plan.Atomic)
                continue;

            if (element.Plan.PositionType == PositionType.Absolute && IsLeadingPositionDefined(element, mainAxis))
            {
                if (performLayout)
                {
                    // In case the element is position absolute and has left/top being
                    // defined, we override the position to whatever the user said (and
                    // margin/border).
                    element.Layout.Position[Pos[(int)mainAxis]] =
                        GetLeadingPosition(element, mainAxis, availableInnerMainDim)
                        + GetLeadingBorder(node, mainAxis)
                        + GetLeadingMargin(element, mainAxis, availableInnerWidth);
                }
            }
            else
            {
                // Now that we placed the element, we need to update the variables.
                // We need to do that only for relative elements. Absolute elements do not
                // take part in that phase.
                if (element.Plan.PositionType == PositionType.Relative)
                {
                    if (MarginLeadingValue(element, mainAxis).Unit == Number.UoM.Auto)
                        collectedFlexItemsValues.MainDim += collectedFlexItemsValues.RemainingFreeSpace /
                                                            numberOfAutoMarginsOnCurrentLine;

                    if (performLayout)
                    {
                        element.Layout.Position[Pos[(int)mainAxis]] = element.Layout.Position[Pos[(int)mainAxis]]
                                                                      + collectedFlexItemsValues.MainDim;
                    }

                    if (MarginTrailingValue(element, mainAxis).Unit == Number.UoM.Auto)
                        collectedFlexItemsValues.MainDim += collectedFlexItemsValues.RemainingFreeSpace /
                                                            numberOfAutoMarginsOnCurrentLine;

                    var canSkipFlex = !performLayout && measureModeCrossDim == MeasureMode.Exactly;
                    if (canSkipFlex)
                    {
                        // If we skipped the flex step, then we can't rely on the measuredDims
                        // because they weren't computed. This means we can't call DimWithMargin.
                        collectedFlexItemsValues.MainDim += betweenMainDim
                                                            + GetMarginForAxis(element, mainAxis, availableInnerWidth)
                                                            + element.Layout.ComputedMainLength;
                        collectedFlexItemsValues.CrossDim = availableInnerCrossDim;
                    }
                    else
                    {
                        // The main dimension is the sum of all the elements dimension plus the spacing.
                        collectedFlexItemsValues.MainDim += betweenMainDim + DimWithMargin(
                            element,
                            mainAxis,
                            availableInnerWidth);

                        if (isNodeBaselineLayout)
                        {
                            // If the element is baseline aligned then the cross dimension is
                            // calculated by adding maxAscent and maxDescent from the baseline.
                            var ascent = Baseline(element)
                                         + GetLeadingMargin(element, LayoutDirection.Column, availableInnerWidth);
                            var descent =
                                element.Layout.MeasuredDimensions.Height
                                + GetMarginForAxis(element, LayoutDirection.Column, availableInnerWidth)
                                - ascent;

                            maxAscentForCurrentLine  = FloatMax(maxAscentForCurrentLine, ascent);
                            maxDescentForCurrentLine = FloatMax(maxDescentForCurrentLine, descent);
                        }
                        else
                        {
                            // The cross dimension is the max of the elements dimension since
                            // there can only be one element in that cross dimension in the case
                            // when the items are not baseline aligned
                            collectedFlexItemsValues.CrossDim = FloatMax(
                                collectedFlexItemsValues.CrossDim,
                                DimWithMargin(element, crossAxis, availableInnerWidth));
                        }
                    }
                }
                else if (performLayout)
                {
                    element.Layout.Position[Pos[(int)mainAxis]] =
                        element.Layout.Position[Pos[(int)mainAxis]]
                        + GetLeadingBorder(node, mainAxis)
                        + leadingMainDim;
                }
            }
        }

        collectedFlexItemsValues.MainDim += trailingPaddingAndBorderMain;

        if (isNodeBaselineLayout)
            collectedFlexItemsValues.CrossDim = maxAscentForCurrentLine + maxDescentForCurrentLine;
    }

    //#if DEBUG
    //    public static bool PrintChanges = false;
    //    public static bool PrintSkips = false;
    //#else
    //        static bool PrintChanges = false;
    //        static bool PrintSkips = false;
    //#endif

    private static readonly string spacer = "                                                            ";

    private static string Spacer(int level)
    {
        var spacerLen = spacer.Length;

        if (level > spacerLen) return spacer;

        return spacer.Substring(spacerLen - level);
    }

    private static readonly Dictionary<MeasureMode, string> MeasureModeNames = new Dictionary<MeasureMode, string>
                                                                               {
                                                                                   {
                                                                                       MeasureMode.Undefined,
                                                                                       "UNDEFINED"
                                                                                   },
                                                                                   { MeasureMode.Exactly, "EXACTLY" },
                                                                                   { MeasureMode.AtMost, "AT_MOST" }
                                                                               };

    private static readonly Dictionary<MeasureMode, string> LayoutModeNames = new Dictionary<MeasureMode, string>
                                                                              {
                                                                                  {
                                                                                      MeasureMode.Undefined,
                                                                                      "LAY_UNDEFINED"
                                                                                  },
                                                                                  {
                                                                                      MeasureMode.Exactly, "LAY_EXACTLY"
                                                                                  },
                                                                                  { MeasureMode.AtMost, "LAY_AT_MOST" }
                                                                              };

    private string MeasureModeName(MeasureMode mode, bool performLayout)
    {
        if (mode >= MeasureMode.AtMost)
            return "";

        return performLayout
            ? LayoutModeNames[mode]
            : MeasureModeNames[mode];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MeasureMode_SizeIsExactAndMatchesOldMeasuredSize(
        MeasureMode sizeMode,
        float       size,
        float       lastComputedSize) =>
        sizeMode == MeasureMode.Exactly && FloatsEqual(size, lastComputedSize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MeasureMode_OldSizeIsUnspecifiedAndStillFits(
        MeasureMode sizeMode,
        float       size,
        MeasureMode lastSizeMode,
        float       lastComputedSize) =>
        sizeMode == MeasureMode.AtMost
        && lastSizeMode == MeasureMode.Undefined
        && (size >= lastComputedSize || FloatsEqual(size, lastComputedSize));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MeasureMode_NewMeasureSizeIsStricterAndStillValid(
        MeasureMode sizeMode,
        float       size,
        MeasureMode lastSizeMode,
        float       lastSize,
        float       lastComputedSize) =>
        lastSizeMode == MeasureMode.AtMost
        && sizeMode == MeasureMode.AtMost
        && lastSize.HasValue()
        && size.HasValue()
        && lastComputedSize.HasValue()
        && lastSize > size
        && (lastComputedSize <= size || FloatsEqual(size, lastComputedSize));

    private bool CanUseCachedMeasurement(
        MeasureMode widthMode,
        float       width,
        MeasureMode heightMode,
        float       height,
        MeasureMode lastWidthMode,
        float       lastWidth,
        MeasureMode lastHeightMode,
        float       lastHeight,
        float       lastComputedWidth,
        float       lastComputedHeight,
        float       marginRow,
        float       marginColumn)
    {
        if (lastComputedHeight.HasValue() && lastComputedHeight < 0
            || lastComputedWidth.HasValue() && lastComputedWidth < 0)
            return false;

        var effectiveWidth      = width;
        var effectiveHeight     = height;
        var effectiveLastWidth  = lastWidth;
        var effectiveLastHeight = lastHeight;

        var hasSameWidthSpec  = lastWidthMode == widthMode && FloatsEqual(effectiveLastWidth, effectiveWidth);
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

    private void RoundToPixelGrid(INode node, float pointScaleFactor, float absoluteLeft, float absoluteTop)
    {
        if (pointScaleFactor.IsZero()) return;

        var nodeLeft = node.Layout.Left;
        var nodeTop  = node.Layout.Top;

        var nodeWidth  = node.Layout.Width;
        var nodeHeight = node.Layout.Height;

        var absoluteNodeLeft = absoluteLeft + nodeLeft;
        var absoluteNodeTop  = absoluteTop + nodeTop;

        var absoluteNodeRight  = absoluteNodeLeft + nodeWidth;
        var absoluteNodeBottom = absoluteNodeTop + nodeHeight;

        // If a node has a custom measure function we never want to round down its
        // size as this could lead to unwanted text truncation.
        var textRounding = node.Plan.IsText;

        SetLayoutPosition(
            node,
            Number.RoundValue(nodeLeft, pointScaleFactor, false, textRounding),
            Side.Left);

        SetLayoutPosition(
            node,
            Number.RoundValue(nodeTop, pointScaleFactor, false, textRounding),
            Side.Top);

        // We multiply dimension by scale factor and if the result is close to the
        // whole number, we don't have any fraction To verify if the result is close
        // to whole number we want to check both floor and ceil numbers
        var hasFractionalWidth =
            !FloatsEqual(FloatMod(nodeWidth * pointScaleFactor, 1.0f), 0f)
            && !FloatsEqual(FloatMod(nodeWidth * pointScaleFactor, 1.0f), 1f);
        var hasFractionalHeight = !FloatsEqual(FloatMod(nodeHeight * pointScaleFactor, 1.0f), 0f)
                                  && !FloatsEqual(FloatMod(nodeHeight * pointScaleFactor, 1.0f), 1f);

        node.Layout.Width =
            Number.RoundValue(
                absoluteNodeRight,
                pointScaleFactor,
                textRounding && hasFractionalWidth,
                textRounding && !hasFractionalWidth)
            - Number.RoundValue(
                absoluteNodeLeft,
                pointScaleFactor,
                false,
                textRounding);

        node.Layout.Height =
            Number.RoundValue(
                absoluteNodeBottom,
                pointScaleFactor,
                textRounding && hasFractionalHeight,
                textRounding && !hasFractionalHeight)
            - Number.RoundValue(
                absoluteNodeTop,
                pointScaleFactor,
                false,
                textRounding);

        if (node.Parent != null)
        {
            node.Layout.DisplayLocation =
                new VecF(
                    node.Parent.Layout.DisplayLocation.X + node.Layout.Left,
                    node.Parent.Layout.DisplayLocation.Y + node.Layout.Top);
        }
        else
        {
            node.Layout.DisplayLocation = new VecF(node.Layout.Left, node.Layout.Top);
        }

        var elementCount = node.Children.Count;
        for (var i = 0; i < elementCount; i++)
        {
            RoundToPixelGrid(
                (INode)node.Children[i],
                pointScaleFactor,
                absoluteNodeLeft,
                absoluteNodeTop);
        }
    }

    //**********************************************

    private void ResolveDimension(INode node)
    {
        if (!node.Plan.MaxDimension(Dimension.Width).IsUndefined &&
            node.Plan.MaxDimension(Dimension.Width) == node.Plan.MinDimension(Dimension.Width))
            node.Layout.ResolvedWidth = node.Plan.MaxDimension(Dimension.Width);
        else
            node.Layout.ResolvedWidth = node.Plan.Dimension(Dimension.Width);

        if (!node.Plan.MaxDimension(Dimension.Height).IsUndefined &&
            node.Plan.MaxDimension(Dimension.Height) == node.Plan.MinDimension(Dimension.Height))
            node.Layout.ResolvedHeight = node.Plan.MaxDimension(Dimension.Height);
        else
            node.Layout.ResolvedHeight = node.Plan.Dimension(Dimension.Height);
    }

    private bool IsLayoutDirectionInPlan(INode node, LayoutDirection axis, float containerSize)
    {
        var number = node.Layout.GetResolvedDimension(Dim[(int)axis]);

        return !(number.Unit == Number.UoM.Auto
                 || number.Unit == Number.UoM.Undefined
                 || number.Unit == Number.UoM.Point && !number.IsUndefined && number.Value < 0.0f
                 || number.Unit == Number.UoM.Percent && !number.IsUndefined &&
                 (number.Value < 0.0f || containerSize.IsUndefined()));
    }

    //**********************************************

    public float ResolveFlexGrow(INode node)
    {
        // Root nodes flexGrow should always be 0
        if (node.Parent == null)
            return 0.0f;

        if (node.Plan.FlexGrow.HasValue())
            return node.Plan.FlexGrow;

        if (node.Plan.Flex.HasValue() && node.Plan.Flex > 0.0f)
            return node.Plan.Flex;

        return LayoutPlan.DefaultFlexGrow;
    }

    public float ResolveFlexShrink(INode node)
    {
        if (node.Parent == null)
            return 0.0f;

        if (node.Plan.FlexShrink.HasValue())
            return node.Plan.FlexShrink;

        if (node.Plan.Flex.HasValue() && node.Plan.Flex < 0.0f)
            return -node.Plan.Flex;

        return LayoutPlan.DefaultFlexShrink;
    }

    public bool IsNodeFlexible(INode node) =>
        node.Plan.PositionType == PositionType.Relative
        && (ResolveFlexGrow(node).IsNotZero() || ResolveFlexShrink(node).IsNotZero());

    //**********************************************

    public float GetLeadingMargin(INode node, LayoutDirection axis, float widthSize)
    {
        if (IsRow(axis) && !node.Plan.Margin[Side.Start].IsUndefined)
        {
            return node.Plan.Margin[Side.Start].ResolveValueMargin(widthSize);
        }

        return node.Plan.Margin.ComputedEdgeValue(Leading[(int)axis], Number.Zero).ResolveValueMargin(widthSize);
    }

    public float GetTrailingMargin(INode node, LayoutDirection axis, float widthSize)
    {
        if (IsRow(axis)
            && !node.Plan.Margin[Side.End].IsUndefined)
        {
            return node.Plan.Margin[Side.End].ResolveValueMargin(widthSize);
        }

        return node.Plan.Margin.ComputedEdgeValue(Trailing[(int)axis], Number.Zero).ResolveValueMargin(widthSize);
    }

    public float GetLeadingBorder(INode node, LayoutDirection axis)
    {
        Number leadingBorder;
        if (IsRow(axis)
            && !node.Plan.Border[Side.Start].IsUndefined)
        {
            leadingBorder = node.Plan.Border[Side.Start];

            if (leadingBorder.Value >= 0)
                return leadingBorder.Value;
        }

        leadingBorder = node.Plan.Border.ComputedEdgeValue(Leading[(int)axis], Number.Zero);

        return FloatMax(leadingBorder.Value, 0.0f);
    }

    public float GetTrailingBorder(INode node, LayoutDirection layoutDirection)
    {
        Number trailingBorder;
        if (IsRow(layoutDirection) && !node.Plan.Border[Side.End].IsUndefined)
        {
            trailingBorder = node.Plan.Border[Side.End];

            if (trailingBorder.Value >= 0.0f)
                return trailingBorder.Value;
        }

        trailingBorder = node.Plan.Border.ComputedEdgeValue(Trailing[(int)layoutDirection], Number.Zero);

        return FloatMax(trailingBorder.Value, 0.0f);
    }

    public float GetLeadingPadding(INode node, LayoutDirection axis, float widthSize)
    {
        var paddingEdgeStart = node.Plan.Padding[Side.Start]
                                   .Resolve(widthSize);

        if (IsRow(axis)
            && !node.Plan.Padding[Side.Start].IsUndefined
            && paddingEdgeStart.HasValue()
            && paddingEdgeStart >= 0.0f)
            return paddingEdgeStart;

        var resolvedValue = node.Plan.Padding.ComputedEdgeValue(Leading[(int)axis], Number.Zero)
                                .Resolve(widthSize);

        return Math.Max(resolvedValue, 0f);
    }

    public float GetTrailingPadding(INode node, LayoutDirection axis, float widthSize)
    {
        var paddingEdgeEnd = node.Plan.Padding[Side.End]
                                 .Resolve(widthSize);

        if (IsRow(axis) && paddingEdgeEnd >= 0f)
            return paddingEdgeEnd;

        var resolvedValue = node.Plan.Padding.ComputedEdgeValue(Trailing[(int)axis], Number.Zero)
                                .Resolve(widthSize);

        return Math.Max(resolvedValue, 0f);
    }

    public float GetLeadingPaddingAndBorder(INode node, LayoutDirection axis, float widthSize) =>
        GetLeadingPadding(node, axis, widthSize) + GetLeadingBorder(node, axis);

    public float GetTrailingPaddingAndBorder(INode node, LayoutDirection axis, float widthSize) =>
        GetTrailingPadding(node, axis, widthSize) + GetTrailingBorder(node, axis);

    public float PaddingAndBorderForAxis(INode node, LayoutDirection axis, float widthSize) =>
        GetLeadingPaddingAndBorder(node, axis, widthSize) + GetTrailingPaddingAndBorder(node, axis, widthSize);

    public float DimWithMargin(INode node, LayoutDirection axis, float widthSize) =>
        node.Layout.MeasuredDimensions[Dim[(int)axis]]
        + GetLeadingMargin(node, axis, widthSize)
        + GetTrailingMargin(node, axis, widthSize);

    //**********************************************

    public void SetPosition(
        INode    node,
        UIDirection uiDirection,
        float       mainSize,
        float       crossSize,
        float       containerWidth)
    {
        // Root nodes should be always layed out as LTR, so we don't return negative values.
        var uiDirectionRespectingRoot = node.Parent != null
            ? uiDirection
            : UIDirection.Ltr;
        var mainAxis  = ResolveLayoutDirection(node.Plan.LayoutDirection, uiDirectionRespectingRoot);
        var crossAxis = ResolveCrossAxisLayoutDirection(mainAxis, uiDirectionRespectingRoot);

        var relativePositionMain  = RelativePosition(node, mainAxis, mainSize);
        var relativePositionCross = RelativePosition(node, crossAxis, crossSize);

        node.Layout.Position[Leading[(int)mainAxis]] =
            GetLeadingMargin(node, mainAxis, containerWidth) + relativePositionMain;
        node.Layout.Position[Trailing[(int)mainAxis]] =
            GetTrailingMargin(node, mainAxis, containerWidth) + relativePositionMain;
        node.Layout.Position[Leading[(int)crossAxis]] =
            GetLeadingMargin(node, crossAxis, containerWidth) + relativePositionCross;
        node.Layout.Position[Trailing[(int)crossAxis]] =
            GetTrailingMargin(node, crossAxis, containerWidth) + relativePositionCross;
    }

    // If both left and right are defined, then use left. Otherwise return +left or
    // -right depending on which is defined.
    private float RelativePosition(INode node, LayoutDirection axis, float axisSize)
    {
        if (IsLeadingPositionDefined(node, axis))
            return GetLeadingPosition(node, axis, axisSize);

        var trailingPosition = GetTrailingPosition(node, axis, axisSize);
        if (trailingPosition.HasValue())
            trailingPosition = -1f * trailingPosition;

        return trailingPosition;
    }

    //**********************************************

    public bool IsLeadingPositionDefined(INode node, LayoutDirection axis) =>
        IsRow(axis)
        && !node.Plan.Position.ComputedEdgeValue(Side.Start).IsUndefined
        || !node.Plan.Position.ComputedEdgeValue(Leading[(int)axis]).IsUndefined;

    public bool IsTrailingPosDefined(INode node, LayoutDirection axis) =>
        IsRow(axis)
        && !node.Plan.Position.ComputedEdgeValue(Side.End).IsUndefined
        || !node.Plan.Position.ComputedEdgeValue(Trailing[(int)axis]).IsUndefined;

    public float GetLeadingPosition(INode node, LayoutDirection axis, float axisSize)
    {
        if (IsRow(axis))
        {
            var lp = node.Plan.Position.ComputedEdgeValue(Side.Start);

            if (!lp.IsUndefined)
                return lp.Resolve(axisSize);
        }

        var leadingPosition = node.Plan.Position.ComputedEdgeValue(Leading[(int)axis]);

        return leadingPosition.IsUndefined
            ? 0f
            : leadingPosition.Resolve(axisSize);
    }

    public float GetTrailingPosition(INode node, LayoutDirection axis, float axisSize)
    {
        if (IsRow(axis))
        {
            var tp = node.Plan.Position.ComputedEdgeValue(Side.End);

            if (!tp.IsUndefined)
                return tp.Resolve(axisSize);
        }

        var trailingPosition = node.Plan.Position.ComputedEdgeValue(Trailing[(int)axis]);

        return trailingPosition.IsUndefined
            ? 0f
            : trailingPosition.Resolve(axisSize);
    }

    //**********************************************

    public float BoundAxisWithinMinAndMax(INode node, LayoutDirection axis, float value, float axisSize)
    {
        var min = float.NaN;
        var max = float.NaN;

        if (IsColumn(axis))
        {
            min = node.Plan.MinHeight.Resolve(axisSize);
            max = node.Plan.MaxHeight.Resolve(axisSize);
        }
        else if (IsRow(axis))
        {
            min = node.Plan.MinWidth.Resolve(axisSize);
            max = node.Plan.MaxWidth.Resolve(axisSize);
        }

        if (max >= 0f && value > max) return max;

        if (min >= 0f && value < min) return min;

        return value;
    }

    // Like YGNodeBoundAxisWithinMinAndMax but also ensures that the value doesn't go below the padding and border amount.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float BoundAxis(INode node, LayoutDirection axis, float value, float axisSize, float widthSize) =>
        FloatMax(
            BoundAxisWithinMinAndMax(node, axis, value, axisSize),
            GetLeadingPaddingAndBorder(node, axis, widthSize));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AlignmentCross AlignElement(INode container, INode element)
    {
        var align = element.Plan.AlignSelf == AlignmentCross.Auto
            ? container.Plan.AlignElements
            : element.Plan.AlignSelf;

        if (align == AlignmentCross.Baseline && IsColumn(container.Plan.LayoutDirection))
            return AlignmentCross.Start;

        return align;
    }

    public void SetElementTrailingPosition(INode container, INode element, LayoutDirection axis)
    {
        var size = element.Layout.MeasuredDimensions[Dim[(int)axis]];
        element.Layout.Position[Trailing[(int)axis]] =
            container.Layout.MeasuredDimensions[Dim[(int)axis]]
            - size
            - element.Layout.Position[Pos[(int)axis]];
    }

    //**********************************************

    public void ConstrainMaxSizeForMode(
        INode        node,
        LayoutDirection axis,
        float           containerAxisSize,
        float           containerWidth,
        ref MeasureMode mode,
        ref float       size)
    {
        var maxSize = node.Plan.MaxDimension(Dim[(int)axis]).Resolve(containerAxisSize)
                      + GetMarginForAxis(node, axis, containerWidth);
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

    public float GetMarginForAxis(INode node, LayoutDirection axis, float widthSize) =>
        GetLeadingMargin(node, axis, widthSize) + GetTrailingMargin(node, axis, widthSize);

    public void SetLayoutMeasuredDimension(INode node, float measuredDimension, Dimension index) =>
        node.Layout.MeasuredDimensions[index] = measuredDimension;

    public void SetLayoutMargin(INode node, float margin, Side edge) => node.Layout.Margin[edge] = margin;

    public void SetLayoutBorder(INode node, float border, Side edge) => node.Layout.Border[edge] = border;

    public void SetLayoutPadding(INode node, float padding, Side edge) => node.Layout.Padding[edge] = padding;

    public void SetLayoutPosition(INode node, float position, Side side) => node.Layout.Position[side] = position;

    //*********************************************************

    public static bool IsLayoutTreeEqualToNode(INode node, INode other)
    {
        if (node.Children.Count != other.Children.Count) return false;

        if (node.Layout != other.Layout) return false;

        if (node.Children.Count == 0) return true;

        for (var i = 0; i < node.Children.Count; ++i)
        {
            if (!IsLayoutTreeEqualToNode((INode)node.Children[i], (INode)other.Children[i]))
                return false;
        }

        return true;
    }

    //*********************************************************

    public Number MarginLeadingValue(INode node, LayoutDirection axis)
    {
        if (IsRow(axis) && !node.Plan.Margin[Side.Start].IsUndefined)
            return node.Plan.Margin[Side.Start];

        return node.Plan.Margin[Leading[(int)axis]];
    }

    public Number MarginTrailingValue(INode node, LayoutDirection axis)
    {
        if (IsRow(axis) && !node.Plan.Margin[Side.End].IsUndefined)
            return node.Plan.Margin[Side.End];

        return node.Plan.Margin[Trailing[(int)axis]];
    }

    public UIDirection ResolveUIDirection(INode node, UIDirection containerUIDirection)
    {
        if (node.Plan.UIDirection == UIDirection.Inherit)
        {
            return containerUIDirection > UIDirection.Inherit
                ? containerUIDirection
                : UIDirection.Ltr;
        }

        return node.Plan.UIDirection;
    }

    //*********************************************************

    public bool IsLayoutDimDefined(INode node, LayoutDirection axis)
    {
        var value = node.Layout.MeasuredDimensions[Dim[(int)axis]];

        return value.HasValue() && value >= 0.0f;
    }

    //**********************************************

    public float Baseline(INode node)
    {
        if (node.Plan.BaselineFunc != null)
        {
            var layoutBaseline = node.Plan.BaselineFunc?.Invoke(
                                     node,
                                     node.Layout.MeasuredDimensions.Width,
                                     node.Layout.MeasuredDimensions.Height)
                                 ?? 0f;

            Debug.Assert(layoutBaseline.HasValue(), "Expect custom baseline function to not return NaN");

            return layoutBaseline;
        }

        INode? baselineElement = null;
        var    elementCount    = node.Children.Count;
        
        for (var i = 0; i < elementCount; i++)
        {
            var element = (INode)node.Children[i];

            if (element.Layout.LineIndex > 0)
                break;

            if (element.Plan.PositionType == PositionType.Absolute)
                continue;

            if (AlignElement(node, element) == AlignmentCross.Baseline || element.Plan.IsReferenceBaseline)
            {
                baselineElement = element;

                break;
            }

            if (baselineElement == null)
                baselineElement = element;
        }

        if (baselineElement == null)
            return node.Layout.MeasuredDimensions.Height;

        var baseline = Baseline(baselineElement);

        return baseline + baselineElement.Layout.Top;
    }

    public bool IsBaselineLayout(INode node)
    {
        if (IsColumn(node.Plan.LayoutDirection)) return false;

        if (node.Plan.AlignElements == AlignmentCross.Baseline) return true;

        var elementCount = node.Children.Count;
        for (var i = 0; i < elementCount; i++)
        {
            var element = (INode)node.Children[i];

            if (element.Plan.PositionType == PositionType.Relative && element.Plan.AlignSelf == AlignmentCross.Baseline)
                return true;
        }

        return false;
    }

    //**********************************************

    /// <summary>
    ///     Used to indicate which layout process is being run
    /// </summary>
    internal enum LayoutPassReason
    {
        Initial           = 0,
        AbsLayout         = 1,
        Stretch           = 2,
        MultilineStretch  = 3,
        FlexLayout        = 4,
        MeasureElement    = 5,
        AbsMeasureElement = 6,
        FlexMeasure       = 7
    }
}