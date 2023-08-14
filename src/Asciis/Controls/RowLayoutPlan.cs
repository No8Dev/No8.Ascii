using No8.Ascii.ElementLayout;

namespace No8.Ascii.Controls
{
    public class RowLayoutPlan : ControlPlan
    {
        public LayoutWrap ElementsWrap
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

        public RowLayoutPlan(LayoutPlan? plan = null) : base(plan)
        {
            // Default values for Row
            //LayoutPlan.Padding = 1;
            LayoutPlan.ElementsDirection = LayoutDirection.Horz;
            HorzAlign = ControlAlign.Stretch;
            VertAlign = ControlAlign.Start;
            //LayoutPlan.Width = 100.Percent();
            //LayoutPlan.Height = 100.Percent();

        }

    }
}