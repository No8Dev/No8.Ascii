using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests;

[TestClass]
public class ConsoleCaptureTests
{

    [Fact]
    public void ConsoleCapture_Disposable()
    {
        using var console = new ConsoleCapture();

        System.Console.Write("Simple Test");

        Assert.Equal("Simple Test", console.StringBuilder.ToString());
    }

    [Fact]
    public void ConsoleCapture_Next()
    {
        using var console = new ConsoleCapture();

        System.Console.Write("First");
        var first = console.NextString();

        System.Console.Write("Second");
        var second = console.NextString();

        Assert.Equal("First", first);
        Assert.Equal("Second", second);
    }

}
