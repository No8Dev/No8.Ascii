using Xunit;

namespace Asciis.App.Tests.Layouts;

[TestClass]
public class RoundingFunctionTest
{
    [Fact]
    public void rounding_value()
    {
        // Test that whole numbers are rounded to whole despite ceil/floor flags
        Assert.Equal(6.0f, Number.RoundValue(6.000001f, 2.0f, false, false));
        Assert.Equal(6.0f, Number.RoundValue(6.000001f, 2.0f, true, false));
        Assert.Equal(6.0f, Number.RoundValue(6.000001f, 2.0f, false, true));
        Assert.Equal(6.0f, Number.RoundValue(5.999999f, 2.0f, false, false));
        Assert.Equal(6.0f, Number.RoundValue(5.999999f, 2.0f, true, false));
        Assert.Equal(6.0f, Number.RoundValue(5.999999f, 2.0f, false, true));

        // Test that numbers with fraction are rounded correctly accounting for ceil/floor flags
        Assert.Equal(6.0f, Number.RoundValue(6.01f, 2.0f, false, false));
        Assert.Equal(6.5f, Number.RoundValue(6.01f, 2.0f, true, false));
        Assert.Equal(6.0f, Number.RoundValue(6.01f, 2.0f, false, true));
        Assert.Equal(6.0f, Number.RoundValue(5.99f, 2.0f, false, false));
        Assert.Equal(6.0f, Number.RoundValue(5.99f, 2.0f, true, false));
        Assert.Equal(5.5f, Number.RoundValue(5.99f, 2.0f, false, true));
    }
}
