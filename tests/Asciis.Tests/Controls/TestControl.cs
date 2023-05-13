using No8.Ascii.Controls;

namespace No8.Ascii.Tests.Controls
{
    internal class TestControl : Control
    {
        public TestControl(LayoutPlan? plan = null)
            : base(plan)
        {

        }

        public TestControl(out Control node, LayoutPlan? plan = null)
            : this(plan)
        {
            node = this;
        }

        public string Text { get; set; } = string.Empty;

    }

    internal class TestStyle : Style
    {
        public string? Text
        {
            get => Get<string?>(nameof(Text));
            set => Set(nameof(Text), value);
        }

    }
}
