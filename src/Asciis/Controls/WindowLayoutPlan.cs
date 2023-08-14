using No8.Ascii.ElementLayout;

namespace No8.Ascii.Controls;

public class WindowLayoutPlan : ControlPlan
{
    public WindowLayoutPlan(LayoutPlan? plan = null) : base(plan)
    {
        LayoutPlan.Width  = 100.Percent();
        LayoutPlan.Height = 100.Percent();
    }

}