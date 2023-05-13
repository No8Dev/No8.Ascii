using System.Diagnostics;
using Asciis.App.Controls;
using Asciis.App.ElementLayout;

namespace Asciis.App;

using static NumberExtensions;

[Flags]
public enum PrintOptions
{
    Layout   = 1,
    Plan     = 2,
    Elements = 4,
    All      = Elements | Plan | Layout
}

public class ElementPrint
{
    private readonly StringBuilder _sb;
    private readonly PrintOptions _options;

    public static LayoutPlan DefaultPlan { get; } = new LayoutPlan();

    public ElementPrint(IElement? node, PrintOptions printOptions = PrintOptions.All, StringBuilder? sb = null)
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

    public ElementPrint Output(IElement node, int level = 0)
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
            AppendString($"{node.Layout.Bounds},  {node.Layout.Position}");
            if (!node.Layout.Padding.IsZero)
                AppendString($"padding:\"{node.Layout.Padding}\" ");
            if (!node.Layout.Margin.IsZero)
                AppendString($"margin:\"{node.Layout.Margin}\" ");
            if (node.MeasureFunc != null)
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

            if (node.Plan.ElementsDirection != DefaultPlan.ElementsDirection)
                AppendString($"elements-direction:\"{node.Plan.ElementsDirection.ToString().ToLower()}\" ");

            if (node.Plan.AlignElements_LayoutDirection != DefaultPlan.AlignElements_LayoutDirection)
                AppendString($"align-content-main:\"{node.Plan.AlignElements_LayoutDirection.ToString().ToLower()}\" ");

            if (node.Plan.Align_Cross != DefaultPlan.Align_Cross)
                AppendString($"align-elements-cross:\"{node.Plan.Align_Cross.ToString().ToLower()}\" ");

            if (node.Plan.AlignElements_Cross != DefaultPlan.AlignElements_Cross)
                AppendString($"align-content-cross:\"{node.Plan.AlignElements_Cross.ToString().ToLower()}\" ");

            if (node.Plan.AlignSelf_Cross != DefaultPlan.AlignSelf_Cross)
                AppendString($"align-self:\"{node.Plan.AlignSelf_Cross.ToString().ToLower()}\" ");

            AppendNumberIfNotZero("flex-grow", node.Plan.FlexGrow);
            AppendNumberIfNotZero("flex-shrink", node.Plan.FlexShrink);
            AppendNumberIfNotAuto("main-length", node.Plan.ChildLayoutDirectionLength);
            AppendFloatOptionalIfDefined("flex", node.Plan.Flex);

            if (node.Plan.ElementsWrap != DefaultPlan.ElementsWrap)
                AppendString($"elements-wrap:\"{node.Plan.ElementsWrap.ToString().ToLower()}\" ");

            if (node.Plan.Overflow != DefaultPlan.Overflow)
                AppendString($"Overflow=\"{node.Plan.Overflow.ToString().ToLower()}\" ");

            if (node.Plan.Atomic != DefaultPlan.Atomic)
                AppendString($"display:\"{node.Plan.Atomic.ToString().ToLower()}\" ");

            AppendEdges("margin", node.Plan.Margin);
            AppendEdges("padding", node.Plan.Padding);

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

        var elementCount = node.Children.Count;
        if (_options.HasFlag(PrintOptions.Elements) && elementCount > 0)
        {
            for (var i = 0; i < elementCount; i++)
            {
                AppendString("\n");
                Output(node.Children[i], level + 1);
            }

            AppendString("\n");
            Indent(level);
        }

        AppendString("}");

        return this;
    }

    [Conditional("DEBUG")]
    public static void Output(string message, IElement node, PrintOptions options = PrintOptions.All)
    {
        var nodePrint = new ElementPrint(node, options);
        //Logger.Log(LogLevel.Debug, "\n" + message);
        //Logger.Log(LogLevel.Debug, nodePrint.ToString());
    }

    public static string Format(IElement node) =>
        new ElementPrint(node) + "\n";



    /// <inheritdoc />
    public override string ToString() =>
        _sb.ToString();
}
