using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;
using Xunit.Abstractions;

namespace No8.Ascii.Tests.Canvas
{
    public class BaseCanvasTests
    {
        protected No8.Ascii.Canvas? _canvas;
        public ITestOutputHelper TestContext { get; set; }

        public BaseCanvasTests(ITestOutputHelper context)
        {
            TestContext = context;
        }
        
        protected Frame LittleBox() => new() { new FrameLayoutPlan { Width = 3, Height = 2 } };
        protected Frame MediumBox() => new() { new FrameLayoutPlan { Width = 6, Height = 3 } };
        protected Frame LargeBox() => new() { new FrameLayoutPlan { Width = 9, Height = 4 } };
   
        protected void Draw(Control root, int? width = 32, int? height = 16)
        {
            width ??= (int)(root.Layout.Bounds.Right);
            height ??= (int)(root.Layout.Bounds.Bottom);
            if (width == 0) width = 32;
            if (height == 0) height = 16;

            _canvas = new No8.Ascii.Canvas(width.Value, height.Value);
            ElementArrange.Calculate(root, width.Value, height.Value);

            root.OnDraw(_canvas, null);

            TestContext.WriteLine(root.ToString(new StringBuilder(), true, false));

            TestContext.WriteLine(_canvas!.ToString());

            TestContext.WriteLine(root.ToString(new StringBuilder(), false, true));
        }
    }
}