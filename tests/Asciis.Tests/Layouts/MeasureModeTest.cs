using No8.Ascii.Console;
using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.Layouts;

[TestClass]
public class MeasureModeTest
{
    private class MeasureConstraint
    {
        public float Width;
        public MeasureMode WidthMode;
        public float Height;
        public MeasureMode HeightMode;
    };

    private class MeasureConstraintList : List<MeasureConstraint> { };

    private static readonly MeasureFunc Measure = (node, width, widthMode, height, heightMode) =>
                                          {
                                              var constraintList = (MeasureConstraintList)(node.Context ?? new MeasureConstraintList());

                                              constraintList.Add(
                                                                 new MeasureConstraint { Width = width, WidthMode = widthMode, Height = height, HeightMode = heightMode });

                                              return new VecF(
                                                              widthMode == MeasureMode.Undefined ? 10 : width,
                                                              heightMode == MeasureMode.Undefined ? 10 : width);
                                          };

    [Fact]
    public void exactly_measure_stretched_element_column()
    {
        var constraintList = new MeasureConstraintList();

        var root = new TestNode(new LayoutPlan { Width = 100, Height = 100 })
           .Add(new TestNode (out var rootA) { MeasureFunc = Measure });
        rootA.Context = constraintList;

        ElementArrange.Calculate(root);
         
        Assert.Single(constraintList);

        Assert.Equal(100, constraintList[0].Width);
        Assert.Equal(MeasureMode.Exactly, constraintList[0].WidthMode);
    }

    [Fact]
    public void exactly_measure_stretched_element_row()
    {
        var constraintList = new MeasureConstraintList();

        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 100, Height = 100 })
           .Add(new TestNode (out var rootA) { MeasureFunc = Measure });
        rootA.Context = constraintList;

        ElementArrange.Calculate(root);

        Assert.Single(constraintList);

        Assert.Equal(100, constraintList[0].Height);
        Assert.Equal(MeasureMode.Exactly, constraintList[0].HeightMode);
    }

    [Fact]
    public void at_most_main_axis_column()
    {
        var constraintList = new MeasureConstraintList();

        var root = new TestNode(new LayoutPlan { Width = 100, Height = 100 })
           .Add(new TestNode (out var rootA) { MeasureFunc = Measure });
        rootA.Context = constraintList;

        ElementArrange.Calculate(root);

        Assert.Single(constraintList);

        Assert.Equal(100, constraintList[0].Height);
        Assert.Equal(MeasureMode.AtMost, constraintList[0].HeightMode);
    }

    [Fact]
    public void at_most_cross_axis_column()
    {
        var constraintList = new MeasureConstraintList();

        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Width = 100, Height = 100 })
           .Add(new TestNode (out var rootA) { MeasureFunc = Measure });
        rootA.Context = constraintList;

        ElementArrange.Calculate(root);

        Assert.Single(constraintList);

        Assert.Equal(100, constraintList[0].Width);
        Assert.Equal(MeasureMode.AtMost, constraintList[0].WidthMode);
    }

    [Fact]
    public void at_most_main_axis_row()
    {
        var constraintList = new MeasureConstraintList();

        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Width = 100, Height = 100 })
           .Add(new TestNode (out var rootA) { MeasureFunc = Measure });
        rootA.Context = constraintList;

        ElementArrange.Calculate(root);

        Assert.Single(constraintList);

        Assert.Equal(100, constraintList[0].Width);
        Assert.Equal(MeasureMode.AtMost, constraintList[0].WidthMode);
    }

    [Fact]
    public void at_most_cross_axis_row()
    {
        var constraintList = new MeasureConstraintList();

        var root = new TestNode(new LayoutPlan { ElementsDirection = LayoutDirection.Horz, Align_Cross = AlignmentLine_Cross.Start, Width = 100, Height = 100 })
           .Add(new TestNode (out var rootA) { MeasureFunc = Measure });
        rootA.Context = constraintList;

        ElementArrange.Calculate(root);

        Assert.Single(constraintList);

        Assert.Equal(100, constraintList[0].Height);
        Assert.Equal(MeasureMode.AtMost, constraintList[0].HeightMode);
    }

    [Fact]
    public void flex_element()
    {
        var constraintList = new MeasureConstraintList();

        var root = new TestNode(new LayoutPlan { Height = 100 })
           .Add(new TestNode(out var rootA, new LayoutPlan { FlexGrow = 1 }) { MeasureFunc = Measure });
        rootA.Context = constraintList;

        ElementArrange.Calculate(root);

        Assert.Equal(2, constraintList.Count);

        Assert.Equal(100, constraintList[0].Height);
        Assert.Equal(MeasureMode.AtMost, constraintList[0].HeightMode);

        Assert.Equal(100, constraintList[1].Height);
        Assert.Equal(MeasureMode.Exactly, constraintList[1].HeightMode);
    }

    [Fact]
    public void flex_element_with_main_length()
    {
        var constraintList = new MeasureConstraintList();

        var root = new TestNode(new LayoutPlan { Height = 100 })
           .Add(new TestNode(out var rootA, new LayoutPlan { FlexGrow = 1, ChildLayoutDirectionLength = 0 }) { MeasureFunc = Measure });
        rootA.Context = constraintList;

        ElementArrange.Calculate(root);

        Assert.Single(constraintList);

        Assert.Equal(100, constraintList[0].Height);
        Assert.Equal(MeasureMode.Exactly, constraintList[0].HeightMode);
    }

    [Fact]
    public void overflow_scroll_column()
    {
        var constraintList = new MeasureConstraintList();

        var root = new TestNode(new LayoutPlan { Align_Cross = AlignmentLine_Cross.Start, Overflow = Overflow.Scroll, Width = 100, Height = 100 })
           .Add(new TestNode (out var rootA) { MeasureFunc = Measure });
        rootA.Context = constraintList;

        ElementArrange.Calculate(root);

        Assert.Single(constraintList);

        Assert.Equal(100, constraintList[0].Width);
        Assert.Equal(MeasureMode.AtMost, constraintList[0].WidthMode);

        Assert.True(constraintList[0].Height.IsUndefined());
        Assert.Equal(MeasureMode.Undefined, constraintList[0].HeightMode);
    }

    [Fact]
    public void overflow_scroll_row()
    {
        var constraintList = new MeasureConstraintList();

        var root = new TestNode(new LayoutPlan
        {
            Align_Cross = AlignmentLine_Cross.Start,
            ElementsDirection = LayoutDirection.Horz,
            Overflow = Overflow.Scroll,
            Width = 100,
            Height = 100
        })
           .Add(new TestNode (out var rootA) { MeasureFunc = Measure });
        rootA.Context = constraintList;

        ElementArrange.Calculate(root);

        Assert.Single(constraintList);

        Assert.True(constraintList[0].Width.IsUndefined());
        Assert.Equal(MeasureMode.Undefined, constraintList[0].WidthMode);

        Assert.Equal(100, constraintList[0].Height);
        Assert.Equal(MeasureMode.AtMost, constraintList[0].HeightMode);
    }
}
