using No8.Ascii.ElementLayout;

namespace No8.Ascii.Controls;

public class LabelLayoutPlan : ControlPlan
{
    public LabelLayoutPlan(LayoutPlan? plan = null) : base(plan)
    {
        LayoutPlan.IsText    = true;
        LayoutPlan.MinHeight = 1;
        LayoutPlan.MinWidth  = 1;
    }
}