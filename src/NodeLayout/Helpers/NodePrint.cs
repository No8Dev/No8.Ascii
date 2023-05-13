using System.Diagnostics;
using System.Text;

using static NodeLayout.NumberExtensions;

namespace NodeLayout;

public class NodePrint
{
    private readonly StringBuilder _sb;
    private readonly PrintOptions _options;

    public static LayoutPlan DefaultPlan { get; } = new LayoutPlan();

    public NodePrint(INode node, PrintOptions printOptions = PrintOptions.All, StringBuilder sb = null)
    {
        _options = printOptions;
        _sb = sb ?? new StringBuilder();
        if (node != null)
            Output(node);
    }

    private void Indent(int level)
    {
        for (var i = 0; i < level; ++i)
        {
            AppendString("  ");
        }
    }

    private bool AreFourValuesEqual(Sides four) =>
        four[Side.Left] == four[Side.Top] && four[Side.Left] == four[Side.Right] && four[Side.Left] == four[Side.Bottom];

    private void AppendString(string str)
    {
        _sb.Append(str);
    }

    private void AppendFloatOptionalIfDefined(string key,
                                               float num)
    {
        if (num.HasValue())
            AppendString($"{key}:\"{num:G}\", ");
    }

    private void AppendNumberIfNotUndefined(string key, Number number)
    {
        if (number.Unit == Number.UoM.Undefined)
            return;

        if (number.Unit == Number.UoM.Auto)
            AppendString($"{key}:\"auto\", ");
        else
            AppendString($"{key}:\"{number}\", ");
    }

    private void AppendNumberIfNotAuto(string key,
                                        Number number)
    {
        if (number.Unit != Number.UoM.Auto)
            AppendNumberIfNotUndefined(key, number);
    }

    private void AppendNumberIfNotZero(string str,
                                        Number number)
    {
        if (number.Unit == Number.UoM.Auto)
            AppendString(str + ":\"auto\", ");
        else if (!FloatsEqual(number.Value, 0))
            AppendNumberIfNotUndefined(str, number);
    }

    private void AppendEdges(string key, Sides edges)
    {
        if (AreFourValuesEqual(edges))
            AppendNumberIfNotZero(key, edges[Side.Left]);
        else
        {
            for (var edge = Side.Left; edge != Side.All; ++edge)
            {
                var str = key + "-" + edge.ToString().ToLower();
                AppendNumberIfNotZero(str, edges[edge]);
            }
        }
    }

    private void AppendEdgeIfNotUndefined(string str, Sides edges, Side edge) { AppendNumberIfNotUndefined(str, edges.ComputedEdgeValue(edge, Number.Undefined)); }

    public NodePrint Output(INode node, int level = 0)
    {
        Indent(level);

        AppendString("{ ");
        if (node.IsDirty)
            AppendString("* ");
        if (!string.IsNullOrWhiteSpace(node.Name))
            AppendString(node.Name + " ");
        AppendString(node.GetType().Name + " ");

        if (_options.HasFlag(PrintOptions.Layout))
        {
            AppendString("Layout{ ");
            AppendString($"{node.Layout.DisplayRect},  {node.Layout.Position}");
            if (!node.Layout.Padding.IsZero)
                AppendString($"padding:\"{node.Layout.Padding}\" ");
            if (!node.Layout.Border.IsZero)
                AppendString($"border:\"{node.Layout.Border}\" ");
            if (!node.Layout.Margin.IsZero)
                AppendString($"margin:\"{node.Layout.Margin}\" ");
            if (node.MeasureNode != null)
                AppendString("has-custom-measure:\"true\" ");
            if (node.Layout.LineIndex > 0)
                AppendString($"line-index:{node.Layout.LineIndex} ");

            AppendString("} ");
        }

        if (_options.HasFlag(PrintOptions.Plan))
        {
            if (node.Plan.IsDirty)
                AppendString("*");
            AppendString("Plan{ ");

            if (node.Plan.LayoutDirection != DefaultPlan.LayoutDirection)
                AppendString($"layout-direction:\"{node.Plan.LayoutDirection.ToString().ToLower()}\" ");

            if (node.Plan.AlignContentMain != DefaultPlan.AlignContentMain)
                AppendString($"align-content-main:\"{node.Plan.AlignContentMain.ToString().ToLower()}\" ");

            if (node.Plan.AlignElements != DefaultPlan.AlignElements)
                AppendString($"align-elements:\"{node.Plan.AlignElements.ToString().ToLower()}\" ");

            if (node.Plan.AlignContentCross != DefaultPlan.AlignContentCross)
                AppendString($"align-content-cross:\"{node.Plan.AlignContentCross.ToString().ToLower()}\" ");

            if (node.Plan.AlignSelf != DefaultPlan.AlignSelf)
                AppendString($"align-self:\"{node.Plan.AlignSelf.ToString().ToLower()}\" ");

            AppendNumberIfNotZero("flex-grow", node.Plan.FlexGrow);
            AppendNumberIfNotZero("flex-shrink", node.Plan.FlexShrink);
            AppendNumberIfNotAuto("main-length", node.Plan.MainLength);
            AppendFloatOptionalIfDefined("flex", node.Plan.Flex);

            if (node.Plan.LayoutWrap != DefaultPlan.LayoutWrap)
                AppendString($"layout-wrap:\"{node.Plan.LayoutWrap.ToString().ToLower()}\" ");

            if (node.Plan.Overflow != DefaultPlan.Overflow)
                AppendString($"Overflow=\"{node.Plan.Overflow.ToString().ToLower()}\" ");

            if (node.Plan.Atomic != DefaultPlan.Atomic)
                AppendString($"display:\"{node.Plan.Atomic.ToString().ToLower()}\" ");

            AppendEdges("margin", node.Plan.Margin);
            AppendEdges("padding", node.Plan.Padding);
            AppendEdges("border", node.Plan.Border);

            AppendNumberIfNotAuto("width", node.Plan.Width);
            AppendNumberIfNotAuto("height", node.Plan.Height);
            AppendNumberIfNotAuto("max-width", node.Plan.MaxWidth);
            AppendNumberIfNotAuto("max-height", node.Plan.MaxHeight);
            AppendNumberIfNotAuto("min-width", node.Plan.MinWidth);
            AppendNumberIfNotAuto("min-height", node.Plan.MinHeight);

            if (node.Plan.PositionType != DefaultPlan.PositionType)
                AppendString($"position: \"{node.Plan.PositionType.ToString().ToLower()}\" ");

            AppendEdgeIfNotUndefined("left", node.Plan.Position, Side.Left);
            AppendEdgeIfNotUndefined("right", node.Plan.Position, Side.Right);
            AppendEdgeIfNotUndefined("top", node.Plan.Position, Side.Top);
            AppendEdgeIfNotUndefined("bottom", node.Plan.Position, Side.Bottom);

            AppendString("} ");
        }

        var elementCount = node.Elements.Count;
        if (_options.HasFlag(PrintOptions.Elements) && elementCount > 0)
        {
            for (var i = 0; i < elementCount; i++)
            {
                AppendString("\n");
                Output(node.Elements[i], level + 1);
            }

            AppendString("\n");
            Indent(level);
        }

        AppendString("}");

        return this;
    }

    [Conditional("DEBUG")]
    public static void Output(string message, INode node, PrintOptions options = PrintOptions.All)
    {
        var nodePrint = new NodePrint(node, options);
        //Logger.Log(LogLevel.Debug, "\n" + message);
        //Logger.Log(LogLevel.Debug, nodePrint.ToString());
    }

    public static string Format(INode node) =>
        new NodePrint(node) + "\n";



    /// <inheritdoc />
    public override string ToString() =>
        _sb.ToString();
}
