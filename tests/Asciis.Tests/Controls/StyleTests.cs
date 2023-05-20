using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.Controls;

[TestClass]
public class StyleTests
{
    [Fact]
    public void Test_StyleCtor()
    {
        var style = new Style
                    {
                        BackgroundBrush = Color.Purple
                    };

        Assert.NotNull(style);
    }

    [Fact]
    public void Test_StyleCtor_DictionaryInitialiser()
    {
        var style = new Style
                    {
                        { "Blobby", "string value" },
                        { nameof(Control.BackgroundBrush), "" }
                    };
        style.BackgroundBrush = Color.MediumPurple;

        Assert.NotNull(style);
    }

    [Fact]
    public void Test_StyleCtor_DynamicStyle()
    {
        dynamic style = new Style
                        {
                            { "Blobby", "string value" },
                            { nameof(Control.BackgroundBrush), "" }
                        };
        style.Background = Color.MediumPurple;

        Assert.NotNull(style);
    }

    [Fact]
    public void Test_StyleCtor_PropertyAndIndexInitialiser()
    {
        dynamic style = new Style
        {
            BackgroundBrush = new SolidColorBrush(Color.MediumPurple),
            ["Blobby"] = "string value",
            ["foregroundBrush"] = (SolidColorBrush)Color.GreenYellow,
            [nameof(Control.Name)] = "yeehaar"
        };

        Assert.NotNull(style);
        Assert.Equal(new SolidColorBrush(Color.MediumPurple), style.BackgroundBrush);
        Assert.Equal(new SolidColorBrush(Color.GreenYellow), style.ForegroundBrush);
        Assert.Equal("yeehaar", style.name);
        Assert.Equal("yeehaar", style.Name);
        Assert.Equal("yeehaar", style["name"]);
        Assert.Equal("yeehaar", style["Name"]);
    }

    [Fact]
    public void Test_Style_ApplyTo()
    {
        var style = new Style
        {
            BackgroundBrush = new SolidColorBrush(Color.MediumPurple),
            ["Blobby"] = "string value",
            ["foregroundbrush"] = (SolidColorBrush)Color.GreenYellow,
            [nameof(Control.Name)] = "yeehaar"
        };
        var ctrl = new TestControl();
        style.ApplyTo(ctrl);

        Assert.Equal("yeehaar", ctrl.Name);
        Assert.Equal(new SolidColorBrush(Color.MediumPurple), ctrl.BackgroundBrush);
        Assert.Equal(new SolidColorBrush(Color.GreenYellow), ctrl.ForegroundBrush);
    }

    [Fact]
    public void Test_Style_ApplyTo_DeepProperties()
    {
        var style = new Style
                    {
                        [nameof(Control.Plan) + "." + nameof(LayoutPlan.Width)] = 100.Point(),
                        ["Plan.Height"] = 200.Point()
                    };
        var ctrl = new TestControl();
        style.ApplyTo(ctrl);

        Assert.Equal(100, ctrl.Plan.Width);
        Assert.Equal(200, ctrl.Plan.Height);

    }

    [Fact]
    public void Test_Style_ApplyTo_Explicit()
    {
        var style = new TestStyle
                    {
                        Text = "Hello, World"
                    };
        var ctrl = new TestControl();
        style.ApplyTo(ctrl);

        Assert.Equal("Hello, World", ctrl.Text);
    }

    [Fact]
    public void Test_Style_ApplyTo_Dynamic()
    {
        dynamic style = new Style();
        style.Text = "Hello, World";

        var ctrl = new TestControl();
        style.ApplyTo(ctrl);

        Assert.Equal("Hello, World", ctrl.Text);
    }
}