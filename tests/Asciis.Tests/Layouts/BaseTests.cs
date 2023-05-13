using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Layouts;

public abstract class BaseTests
{
    protected No8.Ascii.Canvas? Canvas;

    public ITestOutputHelper TestContext { get; set; }

    protected BaseTests(ITestOutputHelper context)
    {
        TestContext = context;
    }

    protected void Draw(Control root, int? width = null, int? height = null)
    {
        width ??= (int)(root.Layout.Bounds.Right);
        height ??= (int)(root.Layout.Bounds.Bottom);
        if (width == 0) width = 32;
        if (height == 0) height = 16;

        Canvas = new No8.Ascii.Canvas(width.Value, height.Value);
        ElementArrange.Calculate(root, width.Value, height.Value);

        root.OnDraw(Canvas, null);

        TestContext.WriteLine(root.ToString(new StringBuilder(), true, false));

        TestContext.WriteLine(Canvas!.ToString());

        TestContext.WriteLine(root.ToString(new StringBuilder(), false, true));
    }

}