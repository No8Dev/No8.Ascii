using No8.Ascii.ElementLayout;

namespace No8.Ascii.Controls
{
    public class FrameLayoutPlan : ControlPlan
    {
        public LayoutWrap Wrap
        {
            get => LayoutPlan.ElementsWrap;
            set => LayoutPlan.ElementsWrap = value;
        }

        public float FlexShrink
        {
            get => LayoutPlan.FlexShrink;
            set => LayoutPlan.FlexShrink = value;
        }
        public float FlexGrow
        {
            get => LayoutPlan.FlexGrow;
            set => LayoutPlan.FlexGrow = value;
        }
    

        public FrameLayoutPlan(LayoutPlan? plan = null) : base(plan)
        {
            // Default values for Frame
            LayoutPlan.Padding = 1;
            LayoutPlan.Width = 100.Percent();
            LayoutPlan.Height = 100.Percent();
        }
    }
}