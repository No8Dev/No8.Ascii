using Xunit;

namespace Asciis.App.Tests.Layouts;

[TestClass]
public class DefaultValuesTest
{
    [Fact]
    public void assert_default_values()
    {
        var root = new TestNode();

        Assert.Equal(0, root.Children.Count);

        Assert.Equal(LayoutDirection.Vert, root.Plan.ElementsDirection);
        Assert.Equal(AlignmentElements_LayoutDirection.Start, root.Plan.AlignElements_LayoutDirection);
        Assert.Equal(Alignment_Cross.Start, root.Plan.AlignElements_Cross);
        Assert.Equal(AlignmentLine_Cross.Stretch, root.Plan.Align_Cross);
        Assert.Equal(AlignmentLine_Cross.Auto, root.Plan.AlignSelf_Cross);
        Assert.Equal(PositionType.Relative, root.Plan.PositionType);
        Assert.Equal(LayoutWrap.NoWrap, root.Plan.ElementsWrap);
        Assert.Equal(Overflow.Visible, root.Plan.Overflow);
        Assert.Equal(0f, root.Plan.FlexGrow);
        Assert.Equal(0f, root.Plan.FlexShrink);
        Assert.Equal(Number.UoM.Auto, root.Plan.ChildLayoutDirectionLength.Unit);

        Assert.True(root.Plan.Position.Left.IsUndefined);
        Assert.True(root.Plan.Position.Top.IsUndefined);
        Assert.True(root.Plan.Position.Right.IsUndefined);
        Assert.True(root.Plan.Position.Bottom.IsUndefined);
        Assert.True(root.Plan.Position.Start.IsUndefined);
        Assert.True(root.Plan.Position.End.IsUndefined);

        Assert.True(root.Plan.Margin.Left.IsUndefined);
        Assert.True(root.Plan.Margin.Top.IsUndefined);
        Assert.True(root.Plan.Margin.Right.IsUndefined);
        Assert.True(root.Plan.Margin.Bottom.IsUndefined);
        Assert.True(root.Plan.Margin.Start.IsUndefined);
        Assert.True(root.Plan.Margin.End.IsUndefined);

        Assert.True(root.Plan.Padding.Left.IsUndefined);
        Assert.True(root.Plan.Padding.Top.IsUndefined);
        Assert.True(root.Plan.Padding.Right.IsUndefined);
        Assert.True(root.Plan.Padding.Bottom.IsUndefined);
        Assert.True(root.Plan.Padding.Start.IsUndefined);
        Assert.True(root.Plan.Padding.End.IsUndefined);

        Assert.Equal(Number.UoM.Auto, root.Plan.Width.Unit);
        Assert.Equal(Number.UoM.Auto, root.Plan.Height.Unit);
        Assert.Equal(Number.UoM.Undefined, root.Plan.MinWidth.Unit);
        Assert.Equal(Number.UoM.Undefined, root.Plan.MinHeight.Unit);
        Assert.Equal(Number.UoM.Undefined, root.Plan.MaxWidth.Unit);
        Assert.Equal(Number.UoM.Undefined, root.Plan.MaxHeight.Unit);

        Assert.Equal(0, root.Layout.Left);
        Assert.Equal(0, root.Layout.Top);
        Assert.Equal(0, root.Layout.Right);
        Assert.Equal(0, root.Layout.Bottom);

        Assert.Equal(0, root.Layout.Margin.Left);
        Assert.Equal(0, root.Layout.Margin.Top);
        Assert.Equal(0, root.Layout.Margin.Right);
        Assert.Equal(0, root.Layout.Margin.Bottom);

        Assert.Equal(0, root.Layout.Padding.Left);
        Assert.Equal(0, root.Layout.Padding.Top);
        Assert.Equal(0, root.Layout.Padding.Right);
        Assert.Equal(0, root.Layout.Padding.Bottom);

        Assert.True(root.Layout.Width.IsUndefined());
        Assert.True(root.Layout.Height.IsUndefined());
    }
}
