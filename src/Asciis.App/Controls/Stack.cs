namespace Asciis.App.Controls;

public class Stack : Control
{
    //**********************************************

    public Stack(LayoutPlan? plan = null, Style? style = null)
        : base(plan ?? new LayoutPlan(), style)
    {
    }

    public Stack(out Stack stack, LayoutPlan? plan = null, Style? style = null)
        : this(plan, style)
    {
        stack = this;
    }

    //--

}