using No8.Ascii.Console;
using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class MeasureTest
{
    private static MeasureFunc _measure = (node, width, widthMode, height, heightMode) =>
                                          {
                                              var nodeContext = node.Context;

                                              var measureCount = (int?)nodeContext ?? 0;
                                              measureCount++;
                                              node.Context = measureCount;

                                              return new VecF(10, 10);
                                          };

    private static MeasureFunc _simulateWrappingText = (node, width, widthMode, height, heightMode) =>
                                                       {
                                                           if (widthMode == MeasureMode.Undefined || width >= 68)
                                                               return new VecF(68, 16);

                                                           return new VecF(50, 32);
                                                       };

    private static MeasureFunc _measureAssertNegative = (node, width, widthMode, height, heightMode) =>
                                                        {
                                                            Assert.True(width >= 0);
                                                            Assert.True(height >= 0);

                                                            return new VecF(0, 0);
                                                        };
    private static MeasureFunc _measureMax = (node, width, widthMode, height, heightMode) =>
    {
        var measureCount = (int)(node.Context ?? 0);
        measureCount++;
        node.Context = measureCount;

        return new VecF(
                        widthMode == MeasureMode.Undefined
                            ? 10
                            : width,
                        heightMode == MeasureMode.Undefined
                            ? 10
                            : height);
    };

    private static readonly MeasureFunc MeasureMin = (node, width, widthMode, height, heightMode) =>
    {
        var measureCount = (int)(node.Context ?? 0);
        measureCount = measureCount + 1;
        node.Context = measureCount;

        return new VecF(
                        widthMode == MeasureMode.Undefined || widthMode == MeasureMode.AtMost && width > 10
                            ? 10
                            : width,
                        heightMode == MeasureMode.Undefined || heightMode == MeasureMode.AtMost && height > 10
                            ? 10
                            : height);
    };

    private static readonly MeasureFunc Measure8449 = (node, width, widthMode, height, heightMode) =>
    {
        var measureCount = (int)(node.Context ?? 0);
        measureCount++;
        node.Context = measureCount;

        return new VecF(84f, 49f);
    };


    [Fact]
    public void do_not_measure_single_grow_shrink_element()
    {
        var root = new TestNode(new LayoutPlan { Width = 100, Height = 100 })
           .Add(new TestNode(out var rootA, new LayoutPlan { FlexGrow = 1, FlexShrink = 1 }) { MeasureFunc = _measure });
        rootA.Context = 0;

        ElementArrange.Calculate(root);

        Assert.Equal(0, (int)rootA.Context);
    }

    [Fact]
    public void measure_absolute_element_with_no_constraints()
    {
        var root = new TestNode()
           .Add(new TestNode()
                   .Add(new TestNode(out var rootAa, new LayoutPlan { PositionType = PositionType.Absolute }) { MeasureFunc = _measure })
               );
        rootAa.Context = 0;

        ElementArrange.Calculate(root);

        Assert.Equal(1, (int)rootAa.Context);
    }

    [Fact]
    public void do_not_measure_when_min_equals_max()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 100, Height = 100 })
           .Add(new TestNode(out var rootA, new LayoutPlan
           {
               MinWidth = 10,
               MaxWidth = 10,
               MinHeight = 10,
               MaxHeight = 10
           })
           { MeasureFunc = _measure });
        rootA.Context = 0;

        ElementArrange.Calculate(root);

        Assert.Equal(0, (int)rootA.Context);
        Assert.Equal(0, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.Equal(10, rootA.Layout.Width);
        Assert.Equal(10, rootA.Layout.Height);
    }

    [Fact]
    public void do_not_measure_when_min_equals_max_percentages()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 100, Height = 100 })
           .Add(new TestNode(out var rootA, new LayoutPlan
           {
               MinWidth = 10,
               MaxWidth = 10,
               MinHeight = 10,
               MaxHeight = 10
           })
           { MeasureFunc = _measure });
        rootA.Context = 0;

        ElementArrange.Calculate(root);

        Assert.Equal(0, (int)rootA.Context);
        Assert.Equal(0, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.Equal(10, rootA.Layout.Width);
        Assert.Equal(10, rootA.Layout.Height);
    }

    [Fact]
    public void measure_nodes_with_margin_auto_and_stretch()
    {
        var root = new TestNode(new LayoutPlan { Width = 500, Height = 500 })
           .Add(new TestNode(out var rootA, new LayoutPlan { Margin = new Sides(left: Number.Auto) }) { MeasureFunc = _measure });

        ElementArrange.Calculate(root);

        Assert.Equal(490, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.Equal(10, rootA.Layout.Width);
        Assert.Equal(10, rootA.Layout.Height);
    }

    [Fact]
    public void do_not_measure_when_min_equals_max_mixed_width_percent()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 100, Height = 100 })
           .Add(new TestNode(out var rootA, new LayoutPlan
           {
               MinWidth = 10.Percent(),
               MaxWidth = 10.Percent(),
               MinHeight = 10,
               MaxHeight = 10
           })
           { MeasureFunc = _measure });
        rootA.Context = 0;

        ElementArrange.Calculate(root);

        Assert.Equal(0, (int)rootA.Context);
        Assert.Equal(0, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.Equal(10, rootA.Layout.Width);
        Assert.Equal(10, rootA.Layout.Height);
    }

    [Fact]
    public void do_not_measure_when_min_equals_max_mixed_height_percent()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 100, Height = 100 })
           .Add(new TestNode(out var rootA, new LayoutPlan
           {
               MinWidth = 10,
               MaxWidth = 10,
               MinHeight = 10.Percent(),
               MaxHeight = 10.Percent()
           })
           { MeasureFunc = _measure });
        rootA.Context = 0;

        ElementArrange.Calculate(root);

        Assert.Equal(0, (int)rootA.Context);
        Assert.Equal(0, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.Equal(10, rootA.Layout.Width);
        Assert.Equal(10, rootA.Layout.Height);
    }

    [Fact]
    public void measure_enough_size_should_be_in_single_line()
    {
        var root = new TestNode(new LayoutPlan { Width = 100 })
           .Add(new TestNode(out var rootA, new LayoutPlan { AlignSelf_Cross = AlignmentLine_Cross.Start }) { MeasureFunc = _simulateWrappingText });

        ElementArrange.Calculate(root);

        Assert.Equal(68, rootA.Layout.Width);
        Assert.Equal(16, rootA.Layout.Height);
    }

    [Fact]
    public void measure_not_enough_size_should_wrap()
    {
        var root = new TestNode(new LayoutPlan { Width = 55 })
           .Add(new TestNode(out var rootA, new LayoutPlan { AlignSelf_Cross = AlignmentLine_Cross.Start }) { MeasureFunc = _simulateWrappingText });

        ElementArrange.Calculate(root);

        Assert.Equal(50, rootA.Layout.Width);
        Assert.Equal(32, rootA.Layout.Height);
    }

    [Fact]
    public void measure_zero_space_should_grow()
    {
        var root = new TestNode(new LayoutPlan { Height = 200, ElementsDirection = LayoutDirection.Vert, FlexGrow = 0 })
           .Add(new TestNode(out var rootA, new LayoutPlan { ElementsDirection = LayoutDirection.Vert, Padding = 100 }) { MeasureFunc = _measure });
        rootA.Context = 0;

        ElementArrange.Calculate(root, 282, Number.ValueUndefined);

        Assert.Equal(282, rootA.Layout.Width);
        Assert.Equal(0, rootA.Layout.Top);
    }

    [Fact]
    public void measure_flex_direction_row_and_padding()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Padding = 25, Width = 50, Height = 50 })
                  .Add(new TestNode (out var rootA) { MeasureFunc = _simulateWrappingText })
                  .Add(new TestNode(out var rootB, new LayoutPlan { Width = 5, Height = 5 }));

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(50, root.Layout.Width);
        Assert.Equal(50, root.Layout.Height);

        Assert.Equal(25, rootA.Layout.Left);
        Assert.Equal(25, rootA.Layout.Top);
        Assert.Equal(50, rootA.Layout.Width);
        Assert.Equal(0, rootA.Layout.Height);

        Assert.Equal(75, rootB.Layout.Left);
        Assert.Equal(25, rootB.Layout.Top);
        Assert.Equal(5, rootB.Layout.Width);
        Assert.Equal(5, rootB.Layout.Height);
    }

    [Fact]
    public void measure_flex_direction_column_and_padding()
    {
        var root = new TestNode(new LayoutPlan { Margin = new Sides(top: 20), Padding = 25, Width = 50, Height = 50 })
                  .Add(new TestNode (out var rootA) { MeasureFunc = _simulateWrappingText })
                  .Add(new TestNode(out var rootB, new LayoutPlan { Width = 5, Height = 5 }));

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(20, root.Layout.Top);
        Assert.Equal(50, root.Layout.Width);
        Assert.Equal(50, root.Layout.Height);

        Assert.Equal(25, rootA.Layout.Left);
        Assert.Equal(25, rootA.Layout.Top);
        Assert.Equal(0, rootA.Layout.Width);
        Assert.Equal(32, rootA.Layout.Height);

        Assert.Equal(25, rootB.Layout.Left);
        Assert.Equal(57, rootB.Layout.Top);
        Assert.Equal(5, rootB.Layout.Width);
        Assert.Equal(5, rootB.Layout.Height);
    }

    [Fact]
    public void measure_flex_direction_row_no_padding()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Margin = new Sides(top: 20), Width = 50, Height = 50 })
                  .Add(new TestNode (out var rootA) { MeasureFunc = _simulateWrappingText })
                  .Add(new TestNode(out var rootB, new LayoutPlan { Width = 5, Height = 5 }));

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(20, root.Layout.Top);
        Assert.Equal(50, root.Layout.Width);
        Assert.Equal(50, root.Layout.Height);

        Assert.Equal(0, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.Equal(50, rootA.Layout.Width);
        Assert.Equal(50, rootA.Layout.Height);

        Assert.Equal(50, rootB.Layout.Left);
        Assert.Equal(0, rootB.Layout.Top);
        Assert.Equal(5, rootB.Layout.Width);
        Assert.Equal(5, rootB.Layout.Height);
    }

    [Fact]
    public void measure_flex_direction_row_no_padding_align_items_flexstart()
    {
        var root = new TestNode(new LayoutPlan
        {
            ElementsDirection = LayoutDirection.Horz,
            Margin = new Sides(top: 20),
            Width = 50,
            Height = 50,
            Align_Cross = AlignmentLine_Cross.Start
        })
                  .Add(new TestNode (out var rootA) { MeasureFunc = _simulateWrappingText })
                  .Add(new TestNode(out var rootB, new LayoutPlan { Width = 5, Height = 5 }));

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(20, root.Layout.Top);
        Assert.Equal(50, root.Layout.Width);
        Assert.Equal(50, root.Layout.Height);

        Assert.Equal(0, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.Equal(50, rootA.Layout.Width);
        Assert.Equal(32, rootA.Layout.Height);

        Assert.Equal(50, rootB.Layout.Left);
        Assert.Equal(0, rootB.Layout.Top);
        Assert.Equal(5, rootB.Layout.Width);
        Assert.Equal(5, rootB.Layout.Height);
    }

    [Fact]
    public void measure_with_fixed_size()
    {
        var root = new TestNode(new LayoutPlan { Margin = new Sides(top: 20), Padding = 25, Width = 50, Height = 50 })
                  .Add(new TestNode(out var rootA, new LayoutPlan { Width = 10, Height = 10 }) { MeasureFunc = _simulateWrappingText })
                  .Add(new TestNode(out var rootB, new LayoutPlan { Width = 5, Height = 5 }));

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(20, root.Layout.Top);
        Assert.Equal(50, root.Layout.Width);
        Assert.Equal(50, root.Layout.Height);

        Assert.Equal(25, rootA.Layout.Left);
        Assert.Equal(25, rootA.Layout.Top);
        Assert.Equal(10, rootA.Layout.Width);
        Assert.Equal(10, rootA.Layout.Height);

        Assert.Equal(25, rootB.Layout.Left);
        Assert.Equal(35, rootB.Layout.Top);
        Assert.Equal(5, rootB.Layout.Width);
        Assert.Equal(5, rootB.Layout.Height);
    }

    [Fact]
    public void measure_with_flex_shrink()
    {
        var root = new TestNode(new LayoutPlan { Margin = new Sides(top: 20), Padding = 25, Width = 50, Height = 50 })
                  .Add(new TestNode(out var rootA, new LayoutPlan { FlexShrink = 1 }) { MeasureFunc = _simulateWrappingText })
                  .Add(new TestNode(out var rootB, new LayoutPlan { Width = 5, Height = 5 }));

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(20, root.Layout.Top);
        Assert.Equal(50, root.Layout.Width);
        Assert.Equal(50, root.Layout.Height);

        Assert.Equal(25, rootA.Layout.Left);
        Assert.Equal(25, rootA.Layout.Top);
        Assert.Equal(0, rootA.Layout.Width);
        Assert.Equal(0, rootA.Layout.Height);

        Assert.Equal(25, rootB.Layout.Left);
        Assert.Equal(25, rootB.Layout.Top);
        Assert.Equal(5, rootB.Layout.Width);
        Assert.Equal(5, rootB.Layout.Height);
    }

    [Fact]
    public void measure_no_padding()
    {
        var root = new TestNode(new LayoutPlan { Margin = new Sides(top: 20), Width = 50, Height = 50 })
                  .Add(new TestNode(out var rootA, new LayoutPlan { FlexShrink = 1 }) { MeasureFunc = _simulateWrappingText })
                  .Add(new TestNode(out var rootB, new LayoutPlan { Width = 5, Height = 5 }));

        ElementArrange.Calculate(root);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(20, root.Layout.Top);
        Assert.Equal(50, root.Layout.Width);
        Assert.Equal(50, root.Layout.Height);

        Assert.Equal(0, rootA.Layout.Left);
        Assert.Equal(0, rootA.Layout.Top);
        Assert.Equal(50, rootA.Layout.Width);
        Assert.Equal(32, rootA.Layout.Height);

        Assert.Equal(0, rootB.Layout.Left);
        Assert.Equal(32, rootB.Layout.Top);
        Assert.Equal(5, rootB.Layout.Width);
        Assert.Equal(5, rootB.Layout.Height);
    }

    [Fact]
    public void can_nullify_measure_func_on_any_node()
    {
        var root = new TestNode()
           .Add(new TestNode());

        Assert.True(root.MeasureFunc == null);
    }

    [Fact]
    public void cant_call_negative_measure()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Vert, Width = 50, Height = 10 })
           .Add(new TestNode(new LayoutPlan { Margin = new Sides(top: 20) }) { MeasureFunc = _measureAssertNegative });

        ElementArrange.Calculate(root);
    }

    [Fact]
    public void cant_call_negative_measure_horizontal()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 10, Height = 20 })
           .Add(new TestNode(new LayoutPlan { Margin = new Sides(start: 20) }) { MeasureFunc = _measureAssertNegative });

        ElementArrange.Calculate(root);
    }

    private static MeasureFunc _measure9010 = (node, width, widthMode, height, heightMode) => new VecF(90, 10);

    [Fact]
    public void percent_with_text_node()
    {
        var root = new TestNode(new LayoutPlan
        {
            ElementsDirection = LayoutDirection.Horz,
            AlignElements_LayoutDirection = AlignmentElements_LayoutDirection.SpaceBetween,
            Align_Cross = AlignmentLine_Cross.Center,
            Width = 100,
            Height = 80
        })
                  .Add(new TestNode(out var rootA))
                  .Add(new TestNode(out var rootB,
                                  new LayoutPlan
                                  {
                                      MaxWidth = 50.Percent(),
                                      Padding = new Sides(top: 50.Percent())
                                  })
                  {
                      MeasureFunc = _measure9010
                  });

        ElementArrange.Calculate(root);

        Assert.Equal(new RectF(0, 0, 100, 80), root.Layout.Bounds);
        Assert.Equal(new RectF(0, 40, 0, 0), rootA.Layout.Bounds);
        Assert.Equal(new RectF(50, 15, 50, 50), rootB.Layout.Bounds);
    }

    [Fact]
    public void measure_once_single_flexible_element()
    {
        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Align_Cross = AlignmentLine_Cross.Start, Width = 100, Height = 100 })
           .Add(new TestNode(out var rootA, new LayoutPlan { FlexGrow = 1 }) { MeasureFunc = _measureMax });
        rootA.Context = 0;

        ElementArrange.Calculate(root);

        Assert.Equal(1, (int)rootA.Context);
    }

    [Fact]
    public void remeasure_with_same_exact_width_larger_than_needed_height()
    {
        var root = new TestNode(new LayoutPlan { Width = 100, Height = 100 }) { new TestNode(out var rootA) { MeasureFunc = MeasureMin } };
        rootA.Context = 0;

        ElementArrange.Calculate(root, 100, 100);
        ElementArrange.Calculate(root, 100, 50);

        Assert.Equal(1, (int)rootA.Context);
    }

    [Fact]
    public void remeasure_with_same_at_most_width_larger_than_needed_height()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start })
           .Add(new TestNode(out var rootA) { MeasureFunc = MeasureMin });
        rootA.Context = 0;

        ElementArrange.Calculate(root, 100, 100);
        ElementArrange.Calculate(root, 100, 50);

        Assert.Equal(1, (int)rootA.Context);
    }

    [Fact]
    public void remeasure_with_computed_width_larger_than_needed_height()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start })
           .Add(new TestNode(out var rootA) { MeasureFunc = MeasureMin });
        rootA.Context = 0;

        ElementArrange.Calculate(root, 100, 100);
        root.Plan.Align_Cross = AlignmentLine_Cross.Stretch;

        ElementArrange.Calculate(root, 10, 50);

        Assert.Equal(1, (int)rootA.Context);
    }

    [Fact]
    public void remeasure_with_at_most_computed_width_undefined_height()
    {
        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start })
           .Add(new TestNode(out var rootA) { MeasureFunc = MeasureMin });
        rootA.Context = 0;

        ElementArrange.Calculate(root, 100);
        ElementArrange.Calculate(root, 10);

        Assert.Equal(1, (int)rootA.Context);
    }

    [Fact]
    public void remeasure_with_already_measured_value_smaller_but_still_float_equal()
    {
        var root = new TestNode(new LayoutPlan { Width = 288, Height = 288, ElementsDirection = LayoutDirection.Horz })
           .Add(new TestNode(new LayoutPlan { Padding = new Sides(2.88f), ElementsDirection = LayoutDirection.Horz })
                   .Add(new TestNode(out var rootAa) { MeasureFunc = Measure8449 })
               );
        rootAa.Context = 0;

        ElementArrange.Calculate(root);

        Assert.Equal(1, (int)rootAa.Context);
    }
}
