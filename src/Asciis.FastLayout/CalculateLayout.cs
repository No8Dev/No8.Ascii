using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Asciis.FastLayout;

using static NumberExtensions;

public class CalculateLayout
{
    private static int   _currentGenerationCount;
    private const  float PointScaleFactor = 1.0f;

    public static void Calculate(
        INode node,
        float containerWidth = Number.ValueUndefined,
        float containerHeight = Number.ValueUndefined)
    {
        new CalculateLayout()
           .CalcLayout(node, containerWidth, containerHeight);
    }

    void CalcLayout(
        INode node,
        float containerWidth = Number.ValueUndefined,
        float containerHeight = Number.ValueUndefined)
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
            width = node.Layout
                        .ResolvedWidth
                        .Resolve(containerWidth);
            widthMeasureMode = MeasureMode.Exactly;
        }
        else if (node.Plan.MaxWidth.Resolve(containerWidth).HasValue())
        {
            width = node.Plan
                        .MaxWidth
                        .Resolve(containerWidth);
            widthMeasureMode = MeasureMode.AtMost;
        }
        else
        {
            width = containerWidth;
            widthMeasureMode = width.IsUndefined()
                ? MeasureMode.Undefined
                : MeasureMode.Exactly;
        }

        if (IsLayoutDirectionInPlan(node, LayoutDirection.Col, containerHeight))
        {
            height            = node.Layout
                                    .ResolvedHeight
                                    .Resolve(containerHeight);
            heightMeasureMode = MeasureMode.Exactly;
        }
        else if (node.Plan.MaxHeight.Resolve(containerHeight).HasValue())
        {
            height = node.Plan
                         .MaxHeight
                         .Resolve(containerHeight);
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
                containerWidth,
                containerHeight,
                containerWidth);
            RoundToPixelGrid(node, PointScaleFactor, 0f, 0f);

#if DEBUG
            // Logger.Log(LogLevel.Verbose, NodePrint.Format(node));
#endif
        }
    }

    /// <summary>
    /// This is a wrapper around the LayoutImplementation function. It determines whether
    /// the layout request is redundant and can be skipped.
    /// </summary>
    private bool LayoutNode(
        INode node,
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
        var layout = node.Layout;

        depth++;

        var needToVisitNode =
            node.IsDirty && layout.GenerationCount != generationCount;

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

        if (performLayout)
        {
            node.Layout.Size = new ((int)node.Layout.MeasuredWidth, (int)node.Layout.MeasuredHeight);
            node.IsDirty = false;
        }

        layout.GenerationCount = generationCount;

        return needToVisitNode;
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
    ///    for example, this is the width of the widest word. Calculating the minimum width is expensive, so we assume a default minimum main size of 0.
    ///  * Min/Max sizes in the main direction are not honored when resolving flexible lengths.
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
    /// <param name="widthMeasureMode">indicates the sizing rules for the width</param>
    /// <param name="heightMeasureMode">indicates the sizing rules for the height</param>
    /// <param name="containerWidth"></param>
    /// <param name="containerHeight"></param>
    /// <param name="performLayout">specifies whether the caller is interested in just the dimensions of the node or it requires the entire node and its subtree to be laid out (with final positions)</param>
    /// <param name="depth"></param>
    /// <param name="generationCount"></param>
    private void LayoutImplementation(
        INode node,
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

        // Set the resolved resolution in the node's layout.
        var flexRowDirection = LayoutDirection.Row;
        var flexColumnDirection = LayoutDirection.Col;

        var startEdge = Side.Start;
        var endEdge = Side.End;

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
        var mainDirection      = node.Plan.ContentsPlan.ToDirection();
        var crossDirection     = ResolveCrossDirection(mainDirection);
        var isMainDirectionRow = IsRow(mainDirection);
        var isLayoutWrap       = node.Plan.ContentsLayout == DirectionLayout.Wrap;

        var mainDirectionContainerSize = isMainDirectionRow
            ? containerWidth
            : containerHeight;
        var crossDirectionContainerSize = isMainDirectionRow
            ? containerHeight
            : containerWidth;

        var leadingPaddingAndBorderCross = GetLeadingPaddingAndBorder(node, crossDirection, containerWidth);
        var paddingAndBorderMain = PaddingAndBorderForDirection(node, mainDirection, containerWidth);
        var paddingAndBorderCross = PaddingAndBorderForDirection(node, crossDirection, containerWidth);

        var measureModeMainDim = isMainDirectionRow
            ? widthMeasureMode
            : heightMeasureMode;
        var measureModeCrossDim = isMainDirectionRow
            ? heightMeasureMode
            : widthMeasureMode;

        var paddingAndBorderRow = isMainDirectionRow
            ? paddingAndBorderMain
            : paddingAndBorderCross;
        var paddingAndBorderColumn = isMainDirectionRow
            ? paddingAndBorderCross
            : paddingAndBorderMain;

        var minInnerWidth = node.Plan.MinWidth.Resolve(containerWidth) - paddingAndBorderRow;
        var maxInnerWidth = node.Plan.MaxWidth.Resolve(containerWidth) - paddingAndBorderRow;
        var minInnerHeight = node.Plan.MinHeight.Resolve(containerHeight) - paddingAndBorderColumn;
        var maxInnerHeight = node.Plan.MaxHeight.Resolve(containerHeight) - paddingAndBorderColumn;

        var minInnerMainDim = isMainDirectionRow
            ? minInnerWidth
            : minInnerHeight;
        var maxInnerMainDim = isMainDirectionRow
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
            LayoutDirection.Col,
            availableHeight,
            containerHeight);

        var availableInnerMainDim = isMainDirectionRow
            ? availableInnerWidth
            : availableInnerHeight;
        var availableInnerCrossDim = isMainDirectionRow
            ? availableInnerHeight
            : availableInnerWidth;

        // STEP 3: DETERMINE MAIN LENGTH FOR EACH ITEM

        var totalOuterMainLength = ComputeMainLengthForElements(
            node,
            availableInnerWidth,
            availableInnerHeight,
            widthMeasureMode,
            heightMeasureMode,
            mainDirection,
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
        var endOfLineIndex = 0;

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
                mainDirectionContainerSize,
                availableInnerWidth,
                availableInnerMainDim,
                startOfLineIndex,
                lineCount);
            endOfLineIndex = collectedFlexItemsValues.EndOfLineIndex;

            // If we don't need to measure the cross direction, we can skip the entire flex step.
            var canSkipFlex =
                !performLayout && measureModeCrossDim == MeasureMode.Exactly;

            // STEP 5: RESOLVING FLEXIBLE LENGTHS ON MAIN DIRECTION
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
                    mainDirection,
                    crossDirection,
                    mainDirectionContainerSize,
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

            // STEP 6: MAIN-DIRECTION JUSTIFICATION & CROSS-DIRECTION SIZE DETERMINATION

            // At this point, all the elements have their dimensions set in the main
            // direction. Their dimensions are also set in the cross direction with the exception
            // of items that are aligned "stretch". We need to compute these stretch
            // values and set the final positions.

            JustifyMainDirection(
                node,
                collectedFlexItemsValues,
                startOfLineIndex,
                mainDirection,
                crossDirection,
                measureModeMainDim,
                measureModeCrossDim,
                mainDirectionContainerSize,
                containerWidth,
                availableInnerMainDim,
                availableInnerCrossDim,
                availableInnerWidth,
                performLayout);

            var containerCrossDirection = availableInnerCrossDim;
            if (measureModeCrossDim == MeasureMode.Undefined || measureModeCrossDim == MeasureMode.AtMost)
            {
                // Compute the cross direction from the max cross dimension of the elements
                containerCrossDirection =
                    BoundDirection(
                        node,
                        crossDirection,
                        collectedFlexItemsValues.CrossDim + paddingAndBorderCross,
                        crossDirectionContainerSize,
                        containerWidth)
                    - paddingAndBorderCross;
            }

            // If there's no flex wrap, the cross dimension is defined by the container.
            if (!isLayoutWrap && measureModeCrossDim == MeasureMode.Exactly)
                collectedFlexItemsValues.CrossDim = availableInnerCrossDim;

            // Clamp to the min/max size specified on the container.
            collectedFlexItemsValues.CrossDim =
                BoundDirection(
                    node,
                    crossDirection,
                    collectedFlexItemsValues.CrossDim + paddingAndBorderCross,
                    crossDirectionContainerSize,
                    containerWidth)
                - paddingAndBorderCross;

            // STEP 7: CROSS-DIRECTION ALIGNMENT
            // We can skip element alignment if we're just measuring the container.
            if (performLayout)
            {
                for (var i = startOfLineIndex; i < endOfLineIndex; i++)
                {
                    var element = (INode)node.Children[i];

                    //if (element.Plan.Atomic)
                    //    continue;

                    if (element.Plan.PositionType == PositionType.Absolute)
                    {
                        // If the element is absolutely positioned and has a
                        // top/left/bottom/right set, override all the previously computed
                        // positions to set it correctly.
                        var isElementLeadingPosDefined = IsLeadingPositionDefined(element, crossDirection);
                        if (isElementLeadingPosDefined)
                        {
                            element.Layout.Position[crossDirection.ToPosition()] =
                                GetLeadingPosition(element, crossDirection, availableInnerCrossDim);
                        }

                        // If leading position is not defined or calculations result in Nan,
                        // default to border + margin
                        if (!isElementLeadingPosDefined
                            || element.Layout.Position[crossDirection.ToPosition()]
                                      .IsUndefined())
                        {
                            element.Layout.Position[crossDirection.ToPosition()] = 0f;
                        }
                    }
                    else
                    {
                        var leadingCrossDim = leadingPaddingAndBorderCross;

                        // For a relative elements, we're either using alignElements (container) or
                        // alignSelf (element) in order to determine the position in the cross direction
                        var alignItem = AlignElement(node, element);

                        // If the element uses align stretch, we need to lay it out one more
                        // time, this time forcing the cross-direction size to be the computed
                        // cross size for the current line.
                        if (alignItem == Align.Stretch)
                        {
                            // If the element defines a definite size for its cross direction, there's
                            // no need to stretch.
                            if (!IsLayoutDirectionInPlan(element, crossDirection, availableInnerCrossDim))
                            {
                                var elementMainSize = element.Layout.GetMeasuredDimension(mainDirection.ToDimension());
                                var elementCrossSize = element.Plan.AspectRatio.HasValue()
                                    ? (isMainDirectionRow
                                          ? elementMainSize / element.Plan.AspectRatio!.Value
                                          : elementMainSize * element.Plan.AspectRatio!.Value)
                                    : collectedFlexItemsValues.CrossDim;

                                var elementMainMeasureMode = MeasureMode.Exactly;
                                var elementCrossMeasureMode = MeasureMode.Exactly;
                                ConstrainMaxSizeForMode(
                                    element,
                                    mainDirection,
                                    availableInnerMainDim,
                                    availableInnerWidth,
                                    ref elementMainMeasureMode,
                                    ref elementMainSize);
                                ConstrainMaxSizeForMode(
                                    element,
                                    crossDirection,
                                    availableInnerCrossDim,
                                    availableInnerWidth,
                                    ref elementCrossMeasureMode,
                                    ref elementCrossSize);

                                var elementWidth = isMainDirectionRow
                                    ? elementMainSize
                                    : elementCrossSize;
                                var elementHeight = !isMainDirectionRow
                                    ? elementMainSize
                                    : elementCrossSize;

                                var alignContentCross = node.Plan.Align;
                                var crossDirectionDoesNotGrow = alignContentCross != Align.Stretch && isLayoutWrap;
                                var elementWidthMeasureMode =
                                    elementWidth.IsUndefined() || !isMainDirectionRow && crossDirectionDoesNotGrow
                                        ? MeasureMode.Undefined
                                        : MeasureMode.Exactly;
                                var elementHeightMeasureMode =
                                    elementHeight.IsUndefined() || isMainDirectionRow && crossDirectionDoesNotGrow
                                        ? MeasureMode.Undefined
                                        : MeasureMode.Exactly;

                                LayoutNode(
                                    element,
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
                                element,
                                crossDirection,
                                availableInnerWidth);

                            if (alignItem == Align.Start)
                            {
                                // No-Op
                            }
                            else if (alignItem == Align.Center)
                                leadingCrossDim += remainingCrossDim / 2;
                            else
                                leadingCrossDim += remainingCrossDim;
                        }

                        // And we apply the position
                        element.Layout.Position[crossDirection.ToPosition()] = element.Layout.Position[crossDirection.ToPosition()].Value
                                                                       + totalLineCrossDim
                                                                       + leadingCrossDim;
                    }
                }
            }

            totalLineCrossDim += collectedFlexItemsValues.CrossDim;
            maxLineMainDim = FloatMax(maxLineMainDim, collectedFlexItemsValues.MainDim);
        }

        // STEP 8: MULTI-LINE CONTENT ALIGNMENT
        // currentLead stores the size of the cross dim
        if (performLayout && isLayoutWrap)
        {
            float crossDimLead = 0;
            var currentLead = leadingPaddingAndBorderCross;
            if (availableInnerCrossDim.HasValue())
            {
                var remainingAlignContentDim = availableInnerCrossDim - totalLineCrossDim;
                switch (node.Plan.Align)
                {
                    case Align.Start:
                        break;
                    case Align.End:
                        currentLead += remainingAlignContentDim;
                        break;
                    case Align.Center:
                        currentLead += remainingAlignContentDim / 2;
                        break;
                    case Align.Stretch:
                        if (availableInnerCrossDim > totalLineCrossDim)
                            crossDimLead = remainingAlignContentDim / lineCount;
                        break;
                    //case Align.SpaceAround:
                    //    if (availableInnerCrossDim > totalLineCrossDim)
                    //    {
                    //        currentLead += remainingAlignContentDim / (2 * lineCount);
                    //        if (lineCount > 1)
                    //            crossDimLead = remainingAlignContentDim / lineCount;
                    //    }
                    //    else
                    //        currentLead += remainingAlignContentDim / 2;

                    //    break;
                    //case AlignmentCross.SpaceBetween:
                    //    if (availableInnerCrossDim > totalLineCrossDim && lineCount > 1)
                    //        crossDimLead = remainingAlignContentDim / (lineCount - 1);

                    //    break;
                }
            }

            var endIndex = 0;
            for (var i = 0; i < lineCount; i++)
            {
                var startIndex = endIndex;
                int ii;

                // compute the line's height and find the endIndex
                float lineHeight = 0;
                float maxAscentForCurrentLine = 0;
                float maxDescentForCurrentLine = 0;
                for (ii = startIndex; ii < elementCount; ii++)
                {
                    var element = (INode)node.Children[ii];

                    //if (element.Plan.Atomic)
                    //    continue;

                    if (element.Plan.PositionType == PositionType.Relative)
                    {
                        if (element.Layout.LineIndex != i)
                            break;

                        if (IsLayoutDimDefined(element, crossDirection))
                        {
                            lineHeight = FloatMax(
                                lineHeight,
                                element.Layout.GetMeasuredDimension(crossDirection.ToDimension()));
                        }
                    }
                }

                endIndex = ii;
                lineHeight += crossDimLead;

                if (performLayout)
                {
                    for (ii = startIndex; ii < endIndex; ii++)
                    {
                        var element = (INode)node.Children[ii];

                        //if (element.Plan.Atomic)
                        //    continue;

                        if (element.Plan.PositionType == PositionType.Relative)
                        {
                            switch (AlignElement(node, element))
                            {
                                case Align.Start:
                                    {
                                        element.Layout.Position[crossDirection.ToPosition()] = currentLead;
                                        break;
                                    }
                                case Align.End:
                                    {
                                        element.Layout.Position[crossDirection.ToPosition()] =
                                            currentLead
                                            + lineHeight
                                            - element.Layout.GetMeasuredDimension(crossDirection.ToDimension());

                                        break;
                                    }
                                case Align.Center:
                                    {
                                        var elementHeight = element.Layout.GetMeasuredDimension(crossDirection.ToDimension());

                                        element.Layout.Position[crossDirection.ToPosition()] =
                                            currentLead + (lineHeight - elementHeight) / 2;

                                        break;
                                    }
                                case Align.Stretch:
                                    {
                                        element.Layout.Position[crossDirection.ToPosition()] = currentLead;

                                        // Remeasure element with the line height as it as been only
                                        // measured with the containers height yet.
                                        if (!IsLayoutDirectionInPlan(element, crossDirection, availableInnerCrossDim))
                                        {
                                            var elementWidth = isMainDirectionRow
                                                ? element.Layout.MeasuredWidth
                                                : lineHeight;

                                            var elementHeight = !isMainDirectionRow
                                                ? element.Layout.MeasuredHeight
                                                : lineHeight;

                                            if (!(FloatsEqual(elementWidth, element.Layout.MeasuredWidth)
                                                  && FloatsEqual(elementHeight, element.Layout.MeasuredHeight)))
                                            {
                                                LayoutNode(
                                                    element,
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
            BoundDirection(
                node,
                LayoutDirection.Row,
                availableWidth,
                containerWidth,
                containerWidth),
            (int)Dimension.Width);

        SetLayoutMeasuredDimension(
            node,
            BoundDirection(
                node,
                LayoutDirection.Col,
                availableHeight,
                containerHeight,
                containerWidth),
            Dimension.Height);

        // If the user didn't specify a width or height for the node, set the dimensions based on the elements
        if (measureModeMainDim == MeasureMode.Undefined
            || node.Plan.ContentsLayout != DirectionLayout.Scroll && measureModeMainDim == MeasureMode.AtMost)
        {
            // Clamp the size to the min/max size, if specified, and make sure it
            // doesn't go below the padding and border amount.
            SetLayoutMeasuredDimension(
                node,
                BoundDirection(
                    node,
                    mainDirection,
                    maxLineMainDim,
                    mainDirectionContainerSize,
                    containerWidth),
                mainDirection.ToDimension());
        }
        else if (measureModeMainDim == MeasureMode.AtMost && node.Plan.ContentsLayout == DirectionLayout.Scroll)
        {
            SetLayoutMeasuredDimension(
                node,
                FloatMax(
                    FloatMin(
                        availableInnerMainDim + paddingAndBorderMain,
                        BoundDirectionWithinMinAndMax(
                            node,
                            mainDirection,
                            maxLineMainDim,
                            mainDirectionContainerSize)
                    ),
                    paddingAndBorderMain),
                mainDirection.ToDimension());
        }

        if (measureModeCrossDim == MeasureMode.Undefined
            || node.Plan.ContentsLayout != DirectionLayout.Scroll && measureModeCrossDim == MeasureMode.AtMost)
        {
            // Clamp the size to the min/max size, if specified, and make sure it
            // doesn't go below the padding and border amount.
            SetLayoutMeasuredDimension(
                node,
                BoundDirection(
                    node,
                    crossDirection,
                    totalLineCrossDim + paddingAndBorderCross,
                    crossDirectionContainerSize,
                    containerWidth),
                crossDirection.ToDimension());
        }
        else if (measureModeCrossDim == MeasureMode.AtMost && node.Plan.ContentsLayout == DirectionLayout.Scroll)
        {
            SetLayoutMeasuredDimension(
                node,
                FloatMax(
                    FloatMin(
                        availableInnerCrossDim + paddingAndBorderCross,
                        BoundDirectionWithinMinAndMax(
                            node,
                            crossDirection,
                            totalLineCrossDim + paddingAndBorderCross,
                            crossDirectionContainerSize)
                    ),
                    paddingAndBorderCross),
                crossDirection.ToDimension());
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
                    isMainDirectionRow
                        ? measureModeMainDim
                        : measureModeCrossDim,
                    availableInnerHeight,
                    depth,
                    generationCount);
            }
        }
    }

    //**********************************************

    private bool IsRow(LayoutDirection layoutDirection) => layoutDirection == LayoutDirection.Row;

    private bool IsColumn(LayoutDirection layoutDirection) => layoutDirection == LayoutDirection.Col;

    private LayoutDirection ResolveCrossDirection(LayoutDirection layoutDirection) =>
        IsColumn(layoutDirection)
            ? LayoutDirection.Row
            : LayoutDirection.Col;

    private float ComputeMainLengthForElements(
        INode node,
        float availableInnerWidth,
        float availableInnerHeight,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        LayoutDirection mainDirection,
        bool performLayout,
        int depth,
        int generationCount)
    {
        var totalOuterMainLength = 0.0f;
        INode? singleFlexElement = null;
        var elements = new List<INode>(node.Children);
        var measureModeMainDim = IsRow(mainDirection)
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
            //if (element.Plan.Atomic)
            //{
            //    ZeroOutLayoutRecursively(element);
            //    element.IsDirty = false;

            //    continue;
            //}

            if (performLayout)
            {
                // Set the initial position (relative to the container).
                var mainDim = IsRow(mainDirection)
                    ? availableInnerWidth
                    : availableInnerHeight;
                var crossDim = IsRow(mainDirection)
                    ? availableInnerHeight
                    : availableInnerWidth;
                SetPosition(
                    element,
                    mainDim,
                    crossDim,
                    availableInnerWidth);
            }

            if (element.Plan.PositionType == PositionType.Absolute) continue;

            if (element == singleFlexElement)
            {
                element.Layout.ComputedMainLengthGeneration = generationCount;
                element.Layout.ComputedMainLength = 0f;
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
                    depth,
                    generationCount);
            }

            totalOuterMainLength +=
                element.Layout.ComputedMainLength;
        }

        return totalOuterMainLength;
    }

    private void ComputeMainLengthForElement(
        INode node,
        INode element,
        float width,
        MeasureMode widthMode,
        float height,
        float containerWidth,
        float containerHeight,
        MeasureMode heightMode,
        int depth,
        int generationCount)
    {
        var mainDirection      = node.Plan.ContentsPlan.ToDirection();
        var isMainDirectionRow = IsRow(mainDirection);
        var mainDirectionSize = isMainDirectionRow
            ? width
            : height;
        var mainDirectionContainerSize = isMainDirectionRow
            ? containerWidth
            : containerHeight;

        var resolvedMainLength = ResolveMainLength(element.Plan).Resolve(mainDirectionContainerSize);
        var isRowStyleDimDefined = IsLayoutDirectionInPlan(element, LayoutDirection.Row, containerWidth);
        var isColumnStyleDimDefined = IsLayoutDirectionInPlan(element, LayoutDirection.Col, containerHeight);

        if (resolvedMainLength.HasValue() && mainDirectionSize.HasValue())
        {
            if (element.Layout.ComputedMainLength.IsUndefined()
                && element.Layout.ComputedMainLengthGeneration != generationCount)
            {
                var paddingAndBorder = PaddingAndBorderForDirection(element, mainDirection, containerWidth);
                element.Layout.ComputedMainLength = Math.Max(resolvedMainLength, paddingAndBorder);
            }
        }
        else if (isMainDirectionRow && isRowStyleDimDefined)
        {
            // The width is definite, so use that as the main length.
            var paddingAndBorder = PaddingAndBorderForDirection(element, LayoutDirection.Row, containerWidth);

            element.Layout.ComputedMainLength = Math.Max(
                element.Layout.ResolvedWidth.Resolve(containerWidth),
                paddingAndBorder);
        }
        else if (!isMainDirectionRow && isColumnStyleDimDefined)
        {
            // The height is definite, so use that as the main length.
            var paddingAndBorder = PaddingAndBorderForDirection(element, LayoutDirection.Col, containerWidth);
            element.Layout.ComputedMainLength = Math.Max(
                element.Layout.ResolvedHeight.Resolve(containerHeight),
                paddingAndBorder);
        }
        else
        {
            // Compute the main length and hypothetical main size (i.e. the clamped main length).
            var elementWidth = Number.ValueUndefined;
            var elementHeight = Number.ValueUndefined;
            var elementWidthMeasureMode = MeasureMode.Undefined;
            var elementHeightMeasureMode = MeasureMode.Undefined;

            if (isRowStyleDimDefined)
            {
                elementWidth = element.Layout.ResolvedWidth.Resolve(containerWidth);
                elementWidthMeasureMode = MeasureMode.Exactly;
            }

            if (isColumnStyleDimDefined)
            {
                elementHeight = element.Layout.ResolvedHeight.Resolve(containerHeight);
                elementHeightMeasureMode = MeasureMode.Exactly;
            }

            // The W3C spec doesn't say anything about the 'overflow' property, but all
            // major browsers appear to implement the following logic.
            if (!isMainDirectionRow && node.Plan.ContentsLayout == DirectionLayout.Scroll || 
                node.Plan.ContentsLayout != DirectionLayout.Scroll)
            {
                if (elementWidth.IsUndefined() && width.HasValue())
                {
                    elementWidth = width;
                    elementWidthMeasureMode = MeasureMode.AtMost;
                }
            }

            if (isMainDirectionRow && node.Plan.ContentsLayout == DirectionLayout.Scroll || node.Plan.ContentsLayout != DirectionLayout.Scroll)
            {
                if (elementHeight.IsUndefined() && height.HasValue())
                {
                    elementHeight = height;
                    elementHeightMeasureMode = MeasureMode.AtMost;
                }
            }

            if (element.Plan.AspectRatio.HasValue())
            {
                if (!isMainDirectionRow && elementWidthMeasureMode == MeasureMode.Exactly)
                {
                    elementHeight = elementWidth / (element.Plan.AspectRatio ?? 1f);
                    elementHeightMeasureMode = MeasureMode.Exactly;
                }
                else if (isMainDirectionRow && elementHeightMeasureMode == MeasureMode.Exactly)
                {
                    elementWidth = elementHeight * (element.Plan.AspectRatio ?? 1f);
                    elementWidthMeasureMode = MeasureMode.Exactly;
                }
            }

            // If the element has no defined size in the cross direction and is set to stretch, set
            // the cross direction to be measured exactly with the available inner width

            var hasExactWidth = width.HasValue() && widthMode == MeasureMode.Exactly;
            var elementWidthStretch = AlignElement(node, element) == Align.Stretch
                                      && elementWidthMeasureMode != MeasureMode.Exactly;
            if (!isMainDirectionRow && !isRowStyleDimDefined && hasExactWidth && elementWidthStretch)
            {
                elementWidth = width;
                elementWidthMeasureMode = MeasureMode.Exactly;
                if (element.Plan.AspectRatio.HasValue())
                {
                    elementHeight = elementWidth / (element.Plan.AspectRatio ?? 1f);
                    elementHeightMeasureMode = MeasureMode.Exactly;
                }
            }

            var hasExactHeight = height.HasValue() && heightMode == MeasureMode.Exactly;
            var elementHeightStretch = AlignElement(node, element) == Align.Stretch
                                       && elementHeightMeasureMode != MeasureMode.Exactly;
            if (isMainDirectionRow && !isColumnStyleDimDefined && hasExactHeight && elementHeightStretch)
            {
                elementHeight = height;
                elementHeightMeasureMode = MeasureMode.Exactly;

                if (element.Plan.AspectRatio.HasValue())
                {
                    elementWidth = elementHeight * (element.Plan.AspectRatio ?? 1f);
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
                LayoutDirection.Col,
                containerHeight,
                containerWidth,
                ref elementHeightMeasureMode,
                ref elementHeight);

            // Measure the element
            LayoutNode(
                element,
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

            element.Layout.ComputedMainLength = FloatMax(
                element.Layout.GetMeasuredDimension(mainDirection.ToDimension()),
                PaddingAndBorderForDirection(element, mainDirection, containerWidth));
        }

        element.Layout.ComputedMainLengthGeneration = generationCount;
    }

    private Number ResolveMainLength(LayoutPlan plan)
    {
        var mainLength = plan.ChildMainLength;

        if (mainLength.Unit != Number.UoM.Auto && 
            mainLength.Unit != Number.UoM.Undefined)
            return mainLength;

        return Number.Auto;
    }

    private void AbsoluteLayoutElement(
        INode container,
        INode element,
        float width,
        MeasureMode widthMode,
        float height,
        int depth,
        int generationCount)
    {
        var mainDirection = container.Plan.ContentsPlan.ToDirection();
        var crossDirection = ResolveCrossDirection(mainDirection);
        var isMainDirectionRow = IsRow(mainDirection);

        var elementWidth = Number.ValueUndefined;
        var elementHeight = Number.ValueUndefined;

        if (IsLayoutDirectionInPlan(element, LayoutDirection.Row, width))
            elementWidth = element.Layout.ResolvedWidth.Resolve(width);
        else
        {
            // If the element doesn't have a specified width, compute the width based on
            // the left/right offsets if they're defined.
            if (IsLeadingPositionDefined(element, LayoutDirection.Row)
                && IsTrailingPosDefined(element, LayoutDirection.Row))
            {
                elementWidth = container.Layout.MeasuredWidth
                               - (GetLeadingPosition(element, LayoutDirection.Row, width) + GetTrailingPosition(
                                   element,
                                   LayoutDirection.Row,
                                   width));
                elementWidth = BoundDirection(element, LayoutDirection.Row, elementWidth, width, width);
            }
        }

        if (IsLayoutDirectionInPlan(element, LayoutDirection.Col, height))
            elementHeight = element.Layout.ResolvedHeight.Resolve(height);
        else
        {
            // If the element doesn't have a specified height, compute the height based on
            // the top/bottom offsets if they're defined.
            if (IsLeadingPositionDefined(element, LayoutDirection.Col)
                && IsTrailingPosDefined(element, LayoutDirection.Col))
            {
                elementHeight = container.Layout.MeasuredHeight
                                - (GetLeadingPosition(element, LayoutDirection.Col, height) + GetTrailingPosition(
                                    element,
                                    LayoutDirection.Col,
                                    height));
                elementHeight = BoundDirection(
                    element,
                    LayoutDirection.Col,
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
                    elementWidth = elementHeight * (element.Plan.AspectRatio ?? 1f);
                else if (elementHeight.IsUndefined())
                    elementHeight = elementWidth / (element.Plan.AspectRatio ?? 1f);
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
            if (!isMainDirectionRow
                && elementWidth.IsUndefined()
                && widthMode != MeasureMode.Undefined
                && width.HasValue()
                && width > 0)
            {
                elementWidth = width;
                elementWidthMeasureMode = MeasureMode.AtMost;
            }

            LayoutNode(
                element,
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
            elementWidth = element.Layout.MeasuredWidth;
            elementHeight = element.Layout.MeasuredHeight;
        }

        LayoutNode(
            element,
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

        if (IsTrailingPosDefined(element, mainDirection) && !IsLeadingPositionDefined(element, mainDirection))
        {
            element.Layout.Position[mainDirection.ToLeadingSide()] =
                container.Layout.GetMeasuredDimension(mainDirection.ToDimension())
                - element.Layout.GetMeasuredDimension(mainDirection.ToDimension())
                - GetTrailingPosition(
                    element,
                    mainDirection,
                    isMainDirectionRow
                        ? width
                        : height);
        }
        else if (
            !IsLeadingPositionDefined(element, mainDirection) && container.Plan.DirectionAlign == Align.Center)
        {
            element.Layout.Position[mainDirection.ToLeadingSide()] =
                (container.Layout.GetMeasuredDimension(mainDirection.ToDimension())
                 - element.Layout.GetMeasuredDimension(mainDirection.ToDimension()))
                / 2.0f;
        }
        else if (!IsLeadingPositionDefined(element, mainDirection) && container.Plan.DirectionAlign == Align.End)
        {
            element.Layout.Position[mainDirection.ToLeadingSide()] =
                container.Layout.GetMeasuredDimension(mainDirection.ToDimension())
                - element.Layout.GetMeasuredDimension(mainDirection.ToDimension());
        }

        if (IsTrailingPosDefined(element, crossDirection) && !IsLeadingPositionDefined(element, crossDirection))
        {
            element.Layout.Position[crossDirection.ToLeadingSide()] =
                container.Layout.GetMeasuredDimension(crossDirection.ToDimension())
                - element.Layout.GetMeasuredDimension(crossDirection.ToDimension())
                - GetTrailingPosition(
                    element,
                    crossDirection,
                    isMainDirectionRow
                        ? height
                        : width);
        }
        else if (!IsLeadingPositionDefined(element, crossDirection) &&
                 AlignElement(container, element) == Align.Center)
        {
            element.Layout.Position[crossDirection.ToLeadingSide()] =
                (container.Layout.GetMeasuredDimension(crossDirection.ToDimension())
                 - element.Layout.GetMeasuredDimension(crossDirection.ToDimension()))
                / 2.0f;
        }
        else if (!IsLeadingPositionDefined(element, crossDirection)
                 && (AlignElement(container, element) == Align.End) ^ container.Plan.ContentsLayout == DirectionLayout.Wrap)
        {
            element.Layout.Position[crossDirection.ToLeadingSide()] =
                container.Layout.GetMeasuredDimension(crossDirection.ToDimension())
                - element.Layout.GetMeasuredDimension(crossDirection.ToDimension());
        }
    }

    private void SetMeasuredDimensions_MeasureFunc(
        INode node,
        float availableWidth,
        float availableHeight,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        float containerWidth,
        float containerHeight)
    {
        Debug.Assert(
            node.MeasureNode != null,
            "Expected node to have custom measure function");

        var paddingAndBorderRow = PaddingAndBorderForDirection(node, LayoutDirection.Row, availableWidth);
        var paddingAndBorderColumn = PaddingAndBorderForDirection(node, LayoutDirection.Col, availableWidth);

        // We want to make sure we don't call measure with negative size
        var innerWidth = availableWidth.IsUndefined()
            ? availableWidth
            : FloatMax(0, availableWidth - paddingAndBorderRow);
        var innerHeight = availableHeight.IsUndefined()
            ? availableHeight
            : FloatMax(0, availableHeight - paddingAndBorderColumn);

        if (widthMeasureMode == MeasureMode.Exactly && heightMeasureMode == MeasureMode.Exactly)
        {
            // Don't bother sizing the text if both dimensions are already defined.
            SetLayoutMeasuredDimension(
                node,
                BoundDirection(
                    node,
                    LayoutDirection.Row,
                    availableWidth,
                    containerWidth,
                    containerWidth),
                Dimension.Width);
            SetLayoutMeasuredDimension(
                node,
                BoundDirection(
                    node,
                    LayoutDirection.Col,
                    availableHeight,
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
                BoundDirection(
                    node,
                    LayoutDirection.Row,
                    widthMeasureMode == MeasureMode.Undefined || widthMeasureMode == MeasureMode.AtMost
                        ? measuredSize.X + paddingAndBorderRow
                        : availableWidth,
                    containerWidth,
                    containerWidth),
                Dimension.Width);

            SetLayoutMeasuredDimension(
                node,
                BoundDirection(
                    node,
                    LayoutDirection.Col,
                    heightMeasureMode == MeasureMode.Undefined || heightMeasureMode == MeasureMode.AtMost
                        ? measuredSize.Y + paddingAndBorderColumn
                        : availableHeight,
                    containerHeight,
                    containerWidth),
                Dimension.Height);
        }
    }

    // For nodes with no elements, use the available values if they were provided,
    // or the minimum size as indicated by the padding and border sizes.
    private void SetMeasuredDimensions_EmptyContainer(
        INode node,
        float availableWidth,
        float availableHeight,
        MeasureMode widthMeasureMode,
        MeasureMode heightMeasureMode,
        float containerWidth,
        float containerHeight)
    {
        var paddingAndBorderRow = PaddingAndBorderForDirection(node, LayoutDirection.Row, containerWidth);
        var paddingAndBorderColumn = PaddingAndBorderForDirection(node, LayoutDirection.Col, containerWidth);

        SetLayoutMeasuredDimension(
            node,
            BoundDirection(
                node,
                LayoutDirection.Row,
                widthMeasureMode == MeasureMode.Undefined
                || widthMeasureMode == MeasureMode.AtMost
                    ? paddingAndBorderRow
                    : availableWidth,
                containerWidth,
                containerWidth),
            Dimension.Width);

        SetLayoutMeasuredDimension(
            node,
            BoundDirection(
                node,
                LayoutDirection.Col,
                heightMeasureMode == MeasureMode.Undefined
                || heightMeasureMode == MeasureMode.AtMost
                    ? paddingAndBorderColumn
                    : availableHeight,
                containerHeight,
                containerWidth),
            Dimension.Height);
    }

    private bool SetMeasuredDimensions_FixedSize(
        INode node,
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
            SetLayoutMeasuredDimension(
                node,
                BoundDirection(
                    node,
                    LayoutDirection.Row,
                    availableWidth.IsUndefined()
                    || widthMeasureMode == MeasureMode.AtMost
                    && availableWidth < 0.0f
                        ? 0.0f
                        : availableWidth,
                    containerWidth,
                    containerWidth),
                Dimension.Width);

            SetLayoutMeasuredDimension(
                node,
                BoundDirection(
                    node,
                    LayoutDirection.Col,
                    availableHeight.IsUndefined() || heightMeasureMode == MeasureMode.AtMost && availableHeight < 0.0f
                        ? 0.0f
                        : availableHeight,
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
        INode node,
        LayoutDirection layoutDirection,
        float availableDim,
        float containerDim)
    {
        var direction = IsRow(layoutDirection)
            ? LayoutDirection.Row
            : LayoutDirection.Col;
        var dimension = IsRow(layoutDirection)
            ? Dimension.Width
            : Dimension.Height;

        var margin = GetMarginForDirection(node, direction, containerDim);
        var paddingAndBorder = PaddingAndBorderForDirection(node, direction, containerDim);

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
        INode node,
        float mainDirectionContainerSize,
        float availableInnerWidth,
        float availableInnerMainDim,
        int startOfLineIndex,
        int lineCount)
    {
        var flexAlgoRowMeasurement = new CollectFlexItemsRowValues
        { RelativeElements = new List<INode>(node.Children.Count) };

        float sizeConsumedOnCurrentLineIncludingMinConstraint = 0;
        var mainDirection = node.Plan.LayoutDirection;
        var isLayoutWrap = node.Plan.ChildrenWrap != LayoutWrap.NoWrap;

        // Add items to the current line until it's full or we run out of items.
        var endOfLineIndex = startOfLineIndex;
        for (; endOfLineIndex < node.Children.Count; endOfLineIndex++)
        {
            var element = (INode)node.Children[endOfLineIndex];

            if (element.Plan.Atomic || element.Plan.PositionType == PositionType.Absolute)
                continue;

            element.Layout.LineIndex = lineCount;
            var elementMarginMainDirection = GetMarginForDirection(element, mainDirection, availableInnerWidth);
            var mainLengthWithMinAndMaxConstraints = BoundDirectionWithinMinAndMax(
                element,
                mainDirection,
                element.Layout.ComputedMainLength,
                mainDirectionContainerSize);

            // If this is a multi-line flow and this item pushes us over the available
            // size, we've hit the end of the current line. Break out of the loop and
            // lay out the current line.
            if (sizeConsumedOnCurrentLineIncludingMinConstraint
                + mainLengthWithMinAndMaxConstraints
                + elementMarginMainDirection
                > availableInnerMainDim
                && isLayoutWrap
                && flexAlgoRowMeasurement.ItemsOnLine > 0)
                break;

            sizeConsumedOnCurrentLineIncludingMinConstraint +=
                mainLengthWithMinAndMaxConstraints + elementMarginMainDirection;
            flexAlgoRowMeasurement.SizeConsumedOnCurrentLine +=
                mainLengthWithMinAndMaxConstraints + elementMarginMainDirection;
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
    // is removed from the remaining free space.
    private void DistributeFreeSpace_FirstPass(
        CollectFlexItemsRowValues collectedFlexItemsValues,
        LayoutDirection mainDirection,
        float mainDirectionContainerSize,
        float availableInnerMainDim,
        float availableInnerWidth)
    {
        float deltaFreeSpace = 0;

        foreach (var currentRelativeElement in collectedFlexItemsValues.RelativeElements.Cast<INode>())
        {
            var elementMainLength =
                BoundDirectionWithinMinAndMax(
                    currentRelativeElement,
                    mainDirection,
                    currentRelativeElement.Layout.ComputedMainLength,
                    mainDirectionContainerSize);

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
                    boundMainSize = BoundDirection(
                        currentRelativeElement,
                        mainDirection,
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
                        deltaFreeSpace += boundMainSize - elementMainLength;
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
                    boundMainSize = BoundDirection(
                        currentRelativeElement,
                        mainDirection,
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
                        deltaFreeSpace += boundMainSize - elementMainLength;
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
        INode node,
        LayoutDirection mainDirection,
        LayoutDirection crossDirection,
        float mainDirectionContainerSize,
        float availableInnerMainDim,
        float availableInnerCrossDim,
        float availableInnerWidth,
        float availableInnerHeight,
        bool mainLengthOverflows,
        MeasureMode measureModeCrossDim,
        bool performLayout,
        int depth,
        int generationCount)
    {
        float deltaFreeSpace = 0;
        var isMainDirectionRow = IsRow(mainDirection);
        var isLayoutWrap = node.Plan.ChildrenWrap != LayoutWrap.NoWrap;

        foreach (var currentRelativeElement in collectedFlexItemsValues.RelativeElements.Cast<INode>())
        {
            var elementMainLength = BoundDirectionWithinMinAndMax(
                currentRelativeElement,
                mainDirection,
                currentRelativeElement.Layout.ComputedMainLength,
                mainDirectionContainerSize);
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

                    updatedMainSize = BoundDirection(
                        currentRelativeElement,
                        mainDirection,
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
                    updatedMainSize = BoundDirection(
                        currentRelativeElement,
                        mainDirection,
                        elementMainLength
                        + collectedFlexItemsValues.RemainingFreeSpace
                        / collectedFlexItemsValues.TotalFlexGrowFactors
                        * flexGrowFactor,
                        availableInnerMainDim,
                        availableInnerWidth);
                }
            }

            deltaFreeSpace += updatedMainSize - elementMainLength;

            var marginMain = GetMarginForDirection(currentRelativeElement, mainDirection, availableInnerWidth);
            var marginCross = GetMarginForDirection(currentRelativeElement, crossDirection, availableInnerWidth);

            float elementCrossSize;
            var elementMainSize = updatedMainSize + marginMain;
            var elementMainMeasureMode = MeasureMode.Exactly;
            MeasureMode elementCrossMeasureMode;

            if (currentRelativeElement.Plan.AspectRatio.HasValue())
            {
                elementCrossSize = isMainDirectionRow
                    ? (elementMainSize - marginMain) / currentRelativeElement.Plan.AspectRatio
                    : (elementMainSize - marginMain) * currentRelativeElement.Plan.AspectRatio;
                elementCrossMeasureMode = MeasureMode.Exactly;

                elementCrossSize += marginCross;
            }
            else if (
                availableInnerCrossDim.HasValue()
                && !IsLayoutDirectionInPlan(currentRelativeElement, crossDirection, availableInnerCrossDim)
                && measureModeCrossDim == MeasureMode.Exactly
                && !(isLayoutWrap && mainLengthOverflows)
                && AlignElement(node, currentRelativeElement) == AlignmentCross.Stretch
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
                mainDirection,
                availableInnerMainDim,
                availableInnerWidth,
                ref elementMainMeasureMode,
                ref elementMainSize);
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
                                        && AlignElement(node, currentRelativeElement) == AlignmentCross.Stretch
                                        && MarginLeadingValue(currentRelativeElement, crossDirection).Unit != Number.UoM.Auto
                                        && MarginTrailingValue(currentRelativeElement, crossDirection).Unit !=
                                        Number.UoM.Auto;

            var elementWidth = isMainDirectionRow
                ? elementMainSize
                : elementCrossSize;
            var elementHeight = !isMainDirectionRow
                ? elementMainSize
                : elementCrossSize;

            var elementWidthMeasureMode = isMainDirectionRow
                ? elementMainMeasureMode
                : elementCrossMeasureMode;
            var elementHeightMeasureMode = !isMainDirectionRow
                ? elementMainMeasureMode
                : elementCrossMeasureMode;

            var isLayoutPass = performLayout && !requiresStretchLayout;
            // Recursively call the layout algorithm for this element with the updated main size.
            LayoutNode(
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
        INode node,
        CollectFlexItemsRowValues collectedFlexItemsValues,
        LayoutDirection mainDirection,
        LayoutDirection crossDirection,
        float mainDirectionContainerSize,
        float availableInnerMainDim,
        float availableInnerCrossDim,
        float availableInnerWidth,
        float availableInnerHeight,
        bool mainLengthOverflows,
        MeasureMode measureModeCrossDim,
        bool performLayout,
        int depth,
        int generationCount)
    {
        var originalFreeSpace = collectedFlexItemsValues.RemainingFreeSpace;
        // First pass: detect the flex items whose min/max constraints trigger
        DistributeFreeSpace_FirstPass(
            collectedFlexItemsValues,
            mainDirection,
            mainDirectionContainerSize,
            availableInnerMainDim,
            availableInnerWidth);

        // Second pass: resolve the sizes of the flexible items
        var distributedFreeSpace = DistributeFreeSpace_SecondPass(
            collectedFlexItemsValues,
            node,
            mainDirection,
            crossDirection,
            mainDirectionContainerSize,
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

    private void JustifyMainDirection(
        INode node,
        CollectFlexItemsRowValues collectedFlexItemsValues,
        int startOfLineIndex,
        LayoutDirection mainDirection,
        LayoutDirection crossDirection,
        MeasureMode measureModeMainDim,
        MeasureMode measureModeCrossDim,
        float mainDirectionContainerSize,
        float containerWidth,
        float availableInnerMainDim,
        float availableInnerCrossDim,
        float availableInnerWidth,
        bool performLayout)
    {
        var leadingPaddingAndBorderMain = GetLeadingPaddingAndBorder(node, mainDirection, containerWidth);
        var trailingPaddingAndBorderMain = GetTrailingPaddingAndBorder(node, mainDirection, containerWidth);
        // If we are using "at most" rules in the main direction, make sure that
        // remainingFreeSpace is 0 when min main dimension is not given
        if (measureModeMainDim == MeasureMode.AtMost && collectedFlexItemsValues.RemainingFreeSpace > 0)
        {
            if (node.Plan.MinDimension(mainDirection.ToDimension()).HasValue
                && node.Plan.MinDimension(mainDirection.ToDimension()).Resolve(mainDirectionContainerSize).HasValue())
            {
                // This condition makes sure that if the size of main dimension(after
                // considering elements main dim, leading and trailing padding etc)
                // falls below min dimension, then the remainingFreeSpace is reassigned
                // considering the min dimension

                // `minAvailableMainDim` denotes minimum available space in which element
                // can be laid out, it will exclude space consumed by padding and border.
                var minAvailableMainDim = node.Plan.MinDimension(mainDirection.ToDimension()).Resolve(mainDirectionContainerSize)
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
                if (MarginLeadingValue(element, mainDirection).Unit == Number.UoM.Auto)
                    numberOfAutoMarginsOnCurrentLine++;

                if (MarginTrailingValue(element, mainDirection).Unit == Number.UoM.Auto)
                    numberOfAutoMarginsOnCurrentLine++;
            }
        }

        // In order to position the elements in the main direction, we have two controls.
        // The space between the beginning and the first element and the space between
        // each two elements.
        float leadingMainDim = 0;
        float betweenMainDim = 0;
        var justifyContent = node.Plan.AlignChildrenMain;

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

        collectedFlexItemsValues.MainDim = leadingPaddingAndBorderMain + leadingMainDim;
        collectedFlexItemsValues.CrossDim = 0;

        float maxAscentForCurrentLine = 0;
        float maxDescentForCurrentLine = 0;
        var isNodeBaselineLayout = IsBaselineLayout(node);
        for (var i = startOfLineIndex; i < collectedFlexItemsValues.EndOfLineIndex; i++)
        {
            var element = (INode)node.Children[i];

            if (element.Plan.Atomic)
                continue;

            if (element.Plan.PositionType == PositionType.Absolute && IsLeadingPositionDefined(element, mainDirection))
            {
                if (performLayout)
                {
                    // In case the element is position absolute and has left/top being
                    // defined, we override the position to whatever the user said (and
                    // margin/border).
                    element.Layout.Position[mainDirection.ToPosition()] =
                        GetLeadingPosition(element, mainDirection, availableInnerMainDim)
                        + GetLeadingBorder(node, mainDirection)
                        + GetLeadingMargin(element, mainDirection, availableInnerWidth);
                }
            }
            else
            {
                // Now that we placed the element, we need to update the variables.
                // We need to do that only for relative elements. Absolute elements do not
                // take part in that phase.
                if (element.Plan.PositionType == PositionType.Relative)
                {
                    if (MarginLeadingValue(element, mainDirection).Unit == Number.UoM.Auto)
                        collectedFlexItemsValues.MainDim += collectedFlexItemsValues.RemainingFreeSpace /
                                                            numberOfAutoMarginsOnCurrentLine;

                    if (performLayout)
                    {
                        element.Layout.Position[mainDirection.ToPosition()] = element.Layout.Position[mainDirection.ToPosition()]
                                                                      + collectedFlexItemsValues.MainDim;
                    }

                    if (MarginTrailingValue(element, mainDirection).Unit == Number.UoM.Auto)
                        collectedFlexItemsValues.MainDim += collectedFlexItemsValues.RemainingFreeSpace /
                                                            numberOfAutoMarginsOnCurrentLine;

                    var canSkipFlex = !performLayout && measureModeCrossDim == MeasureMode.Exactly;
                    if (canSkipFlex)
                    {
                        // If we skipped the flex step, then we can't rely on the measuredDims
                        // because they weren't computed. This means we can't call DimWithMargin.
                        collectedFlexItemsValues.MainDim += betweenMainDim
                                                            + GetMarginForDirection(element, mainDirection, availableInnerWidth)
                                                            + element.Layout.ComputedMainLength;
                        collectedFlexItemsValues.CrossDim = availableInnerCrossDim;
                    }
                    else
                    {
                        // The main dimension is the sum of all the elements dimension plus the spacing.
                        collectedFlexItemsValues.MainDim += betweenMainDim + DimWithMargin(
                            element,
                            mainDirection,
                            availableInnerWidth);

                        if (isNodeBaselineLayout)
                        {
                            // If the element is baseline aligned then the cross dimension is
                            // calculated by adding maxAscent and maxDescent from the baseline.
                            var ascent = Baseline(element)
                                         + GetLeadingMargin(element, LayoutDirection.Col, availableInnerWidth);
                            var descent =
                                element.Layout.MeasuredDimensions.Height
                                + GetMarginForDirection(element, LayoutDirection.Col, availableInnerWidth)
                                - ascent;

                            maxAscentForCurrentLine = FloatMax(maxAscentForCurrentLine, ascent);
                            maxDescentForCurrentLine = FloatMax(maxDescentForCurrentLine, descent);
                        }
                        else
                        {
                            // The cross dimension is the max of the elements dimension since
                            // there can only be one element in that cross dimension in the case
                            // when the items are not baseline aligned
                            collectedFlexItemsValues.CrossDim = FloatMax(
                                collectedFlexItemsValues.CrossDim,
                                DimWithMargin(element, crossDirection, availableInnerWidth));
                        }
                    }
                }
                else if (performLayout)
                {
                    element.Layout.Position[mainDirection.ToPosition()] +=
                        GetLeadingBorder(node, mainDirection) + leadingMainDim;
                }
            }
        }

        collectedFlexItemsValues.MainDim += trailingPaddingAndBorderMain;

        if (isNodeBaselineLayout)
            collectedFlexItemsValues.CrossDim = maxAscentForCurrentLine + maxDescentForCurrentLine;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MeasureMode_SizeIsExactAndMatchesOldMeasuredSize(
        MeasureMode sizeMode,
        float size,
        float lastComputedSize) =>
        sizeMode == MeasureMode.Exactly && FloatsEqual(size, lastComputedSize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MeasureMode_OldSizeIsUnspecifiedAndStillFits(
        MeasureMode sizeMode,
        float size,
        MeasureMode lastSizeMode,
        float lastComputedSize) =>
        sizeMode == MeasureMode.AtMost
        && lastSizeMode == MeasureMode.Undefined
        && (size >= lastComputedSize || FloatsEqual(size, lastComputedSize));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool MeasureMode_NewMeasureSizeIsStricterAndStillValid(
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

    private bool CanUseCachedMeasurement(
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

    private void RoundToPixelGrid(INode node, float pointScaleFactor, float absoluteLeft, float absoluteTop)
    {
        if (pointScaleFactor.IsZero()) return;

        var nodeLeft = node.Layout.Left;
        var nodeTop = node.Layout.Top;

        var nodeWidth = node.Layout.Width;
        var nodeHeight = node.Layout.Height;

        var absoluteNodeLeft = absoluteLeft + nodeLeft;
        var absoluteNodeTop = absoluteTop + nodeTop;

        var absoluteNodeRight = absoluteNodeLeft + nodeWidth;
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
                node.Children[i],
                pointScaleFactor,
                absoluteNodeLeft,
                absoluteNodeTop);
        }
    }

    //**********************************************

    private void ResolveDimension(INode node)
    {
        if (node.Plan.MaxDimension(Dimension.Width)?.HasValue == true &&
            node.Plan.MaxDimension(Dimension.Width) == node.Plan.MinDimension(Dimension.Width))
            node.Layout.ResolvedWidth = node.Plan.MaxDimension(Dimension.Width);
        else
            node.Layout.ResolvedWidth = node.Plan.Dimension(Dimension.Width);

        if (node.Plan.MaxDimension(Dimension.Height)?.HasValue == true &&
            node.Plan.MaxDimension(Dimension.Height) == node.Plan.MinDimension(Dimension.Height))
            node.Layout.ResolvedHeight = node.Plan.MaxDimension(Dimension.Height);
        else
            node.Layout.ResolvedHeight = node.Plan.Dimension(Dimension.Height);
    }

    private bool IsLayoutDirectionInPlan(INode node, LayoutDirection direction, float containerSize)
    {
        var number = node.Layout.GetResolvedDimension(direction.ToDimension());
        if (number == null) 
            return false;

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

        if (node.Plan.GrowFactor.HasValue())
            return node.Plan.GrowFactor;

        return LayoutPlan.DefaultGrowFactor;
    }

    public float ResolveFlexShrink(INode node)
    {
        if (node.Parent == null)
            return 0.0f;

        if (node.Plan.ShrinkFactor.HasValue())
            return node.Plan.ShrinkFactor;

        return LayoutPlan.DefaultShrinkFactor;
    }

    public bool IsNodeFlexible(INode node) =>
        node.Plan.PositionType == PositionType.Relative
        && (ResolveFlexGrow(node).IsNotZero() || ResolveFlexShrink(node).IsNotZero());

    //**********************************************

    public float GetLeadingPadding(INode node, LayoutDirection direction, float widthSize)
    {
        var resolvedValue = node.Plan.Padding
                               ?.ComputedEdgeValue(direction.ToLeadingSide(), Number.Zero)
                                .Resolve(widthSize) ?? 0f;
        return Math.Max(resolvedValue, 0f);
    }

    public float GetTrailingPadding(INode node, LayoutDirection direction, float widthSize)
    {
        var resolvedValue = node.Plan.Padding
                               ?.ComputedEdgeValue(direction.ToTrailingSide(), Number.Zero)
                                .Resolve(widthSize) ?? 0f;

        return Math.Max(resolvedValue, 0f);
    }

    public float GetLeadingPaddingAndBorder(INode node, LayoutDirection direction, float widthSize) =>
        GetLeadingPadding(node, direction, widthSize);

    public float GetTrailingPaddingAndBorder(INode node, LayoutDirection direction, float widthSize) =>
        GetTrailingPadding(node, direction, widthSize);

    public float PaddingAndBorderForDirection(INode node, LayoutDirection direction, float widthSize) =>
        GetLeadingPaddingAndBorder(node, direction, widthSize) + GetTrailingPaddingAndBorder(node, direction, widthSize);

    public float DimWithMargin(INode node, LayoutDirection direction, float widthSize) => 
        node.Layout.GetMeasuredDimension(direction.ToDimension());

    //**********************************************

    public void SetPosition(
        INode node,
        float mainSize,
        float crossSize,
        float containerWidth)
    {
        var mainDirection  = node.Plan.ContentsPlan.ToDirection();
        var crossDirection = ResolveCrossDirection(mainDirection);

        var relativePositionMain = RelativePosition(node, mainDirection, mainSize);
        var relativePositionCross = RelativePosition(node, crossDirection, crossSize);

        node.Layout.Position[mainDirection.ToLeadingSide()] = relativePositionMain;
        node.Layout.Position[mainDirection.ToTrailingSide()] = relativePositionMain;
        node.Layout.Position[crossDirection.ToLeadingSide()] = relativePositionCross;
        node.Layout.Position[crossDirection.ToTrailingSide()] = relativePositionCross;
    }

    // If both left and right are defined, then use left. Otherwise return +left or
    // -right depending on which is defined.
    private float RelativePosition(INode node, LayoutDirection direction, float directionSize)
    {
        if (IsLeadingPositionDefined(node, direction))
            return GetLeadingPosition(node, direction, directionSize);

        var trailingPosition = GetTrailingPosition(node, direction, directionSize);
        if (trailingPosition.HasValue())
            trailingPosition = -1f * trailingPosition;

        return trailingPosition;
    }

    //**********************************************

    public bool IsLeadingPositionDefined(INode node, LayoutDirection direction) =>
        node.Plan.Position.ComputedEdgeValue(direction.ToLeadingSide()).HasValue;

    public bool IsTrailingPosDefined(INode node, LayoutDirection direction) =>
        node.Plan.Position.ComputedEdgeValue(direction.ToTrailingSide()).HasValue;

    public float GetLeadingPosition(INode node, LayoutDirection direction, float directionSize)
    {
        var leadingPosition = node.Plan.Position.ComputedEdgeValue(direction.ToLeadingSide());
        return leadingPosition.HasValue ? leadingPosition.Resolve(directionSize) : 0f;
    }

    public float GetTrailingPosition(INode node, LayoutDirection direction, float directionSize)
    {
        var trailingPosition = node.Plan.Position.ComputedEdgeValue(direction.ToTrailingSide());
        return trailingPosition.HasValue ? trailingPosition.Resolve(directionSize) : 0f;
    }

    //**********************************************

    public float BoundDirectionWithinMinAndMax(INode node, LayoutDirection direction, float value, float directionSize)
    {
        var min = float.NaN;
        var max = float.NaN;

        if (IsColumn(direction))
        {
            min = node.Plan.MinHeight.Resolve(directionSize);
            max = node.Plan.MaxHeight.Resolve(directionSize);
        }
        else if (IsRow(direction))
        {
            min = node.Plan.MinWidth.Resolve(directionSize);
            max = node.Plan.MaxWidth.Resolve(directionSize);
        }

        if (max >= 0f && value > max) return max;

        if (min >= 0f && value < min) return min;

        return value;
    }

    // Like BoundDirectionWithinMinAndMax but also ensures that the value doesn't go below the padding and border amount.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float BoundDirection(INode node, LayoutDirection direction, float value, float directionSize, float widthSize) =>
        FloatMax(
            BoundDirectionWithinMinAndMax(node, direction, value, directionSize),
            GetLeadingPaddingAndBorder(node, direction, widthSize));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Align AlignElement(INode container, INode element)
    {
        var align = container.Plan.Align;
        return align;
    }

    public void SetElementTrailingPosition(INode container, INode element, LayoutDirection direction)
    {
        var size = element.Layout.GetMeasuredDimension(direction.ToDimension());
        element.Layout.Position[direction.ToTrailingSide()] =
            container.Layout.GetMeasuredDimension(direction.ToDimension())
            - size
            - element.Layout.Position[direction.ToPosition()].Value;
    }

    //**********************************************

    public void ConstrainMaxSizeForMode(
        INode node,
        LayoutDirection direction,
        float containerDirectionSize,
        float containerWidth,
        ref MeasureMode mode,
        ref float size)
    {
        var maxSize = node.Plan
                          .MaxDimension(direction.ToDimension())
                          .Resolve(containerDirectionSize);
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

    public void SetLayoutMeasuredDimension(INode node, float measuredDimension, Dimension dim) =>
        node.Layout.SetMeasuredDimension(dim, measuredDimension);

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
            if (!IsLayoutTreeEqualToNode(node.Children[i], other.Children[i]))
                return false;
        }

        return true;
    }

    //*********************************************************

    public bool IsLayoutDimDefined(INode node, LayoutDirection direction)
    {
        var value = node.Layout.GetMeasuredDimension(direction.ToDimension());

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
    
    //**********************************************

}

