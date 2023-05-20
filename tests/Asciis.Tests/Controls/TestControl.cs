using No8.Ascii.Controls;
using No8.Ascii.ElementLayout;

namespace No8.Ascii.Tests.Controls
{
    internal class TestControl : Control
    {
        public TestControl(ControlPlan? plan = null)
            : base(plan)
        {

        }

        public TestControl(out Control node, ControlPlan? plan = null)
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
