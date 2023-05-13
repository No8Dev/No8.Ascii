namespace Asciis.UI.Controls;

public class StackComposer : Composer<Stack, StackLayout>
{
    public StackComposer(Stack stack) : base(stack)
    {
    }

    protected override (float maxWidth, float maxHeight) MeasureChildren(in RectF area)
    {
        return Layout.Direction == Direction.Horizontal
                   ? MeasureChildrenHorizontal(area)
                   : MeasureChildrenVertical(area);
    }

    private (float maxWidth, float maxHeight) MeasureChildrenVertical(in Rect area)
    {
        float maxWidth = 0;
        float maxHeight = 0;

        var parentHorzPosition = LayoutPosition.Stretch;

        if (Parent?.Control != null)
        {
            var parent = Parent.Control;
            parentHorzPosition = parent.HorzPosition ?? LayoutPosition.Stretch;
        }
        if (parentHorzPosition == LayoutPosition.Stretch)
        {
            maxWidth = area.Width;
        }

        foreach (var child in Control.Children)
        {
            var composer = CreateComposerForChild(child);

            composer.DoMeasure(area);

            var childLayout = composer.Layout;

            if (!childLayout.Width.IsUndefined() && childLayout.Width > maxWidth && parentHorzPosition != LayoutPosition.Stretch)
                maxWidth = childLayout.Width;

            maxHeight += childLayout.Height;
        }

        return (maxWidth, maxHeight);
    }

    private (float maxWidth, float maxHeight) MeasureChildrenHorizontal(in RectF area)
    {
        float maxWidth = 0;
        float maxHeight = 0;

        var parentVertPosition = LayoutPosition.Stretch;


        if (Parent?.Control != null)
        {
            var parent = Parent.Control;
            parentVertPosition = parent.VertPosition ?? LayoutPosition.Stretch;
        }
        if (parentVertPosition == LayoutPosition.Stretch)
        {
            maxWidth = area.Width;
        }

        foreach (var child in Control.Children)
        {
            var composer = CreateComposerForChild(child);

            composer.DoMeasure(area);

            var childLayout = composer.Layout;

            if (!childLayout.Height.IsUndefined() &&
                childLayout.Height > maxHeight &&
                parentVertPosition != LayoutPosition.Stretch)
                maxHeight = childLayout.Height;

            maxWidth += childLayout.Width;
        }

        return (maxWidth, maxHeight);
    }

    protected override void ArrangeChildren()
    {
        var area = Layout.ContentArea;
        float startX = area.X;
        float startY = area.Y;
        float remainingWidth = area.Width;
        float remainingHeight = area.Height;

        foreach (var composer in ChildComposers)
        {
            var childLayout = composer.Layout;

            if (childLayout.Width.IsUndefined())
                childLayout.Width = area.Width;
            if (childLayout.Height.IsUndefined())
                childLayout.Height = area.Height;

            if (Layout.Direction == Direction.Horizontal)
            {
                switch (Control.HorzPosition)
                {
                    case LayoutPosition.Start:
                    case LayoutPosition.Center:
                    case LayoutPosition.End:
                        childLayout.X = startX;
                        break;
                    case LayoutPosition.Stretch:
                        childLayout.X = startX;
                        childLayout.Width = remainingWidth;
                        break;
                }
                switch (Control.VertPosition)
                {
                    case LayoutPosition.Start:
                        childLayout.Y = startY;
                        break;
                    case LayoutPosition.Center:
                        childLayout.Y = startY + ((remainingHeight / 2) - (childLayout.Height / 2));
                        break;
                    case LayoutPosition.End:
                        childLayout.Y = startY + (remainingHeight - childLayout.Height);
                        break;
                    case LayoutPosition.Stretch:
                        childLayout.Y = startY;
                        childLayout.Height = remainingHeight;
                        break;
                }

                startX += childLayout.Width;
            }
            else if (Layout.Direction == Direction.Vertical)
            {
                switch (Control.HorzPosition)
                {
                    case LayoutPosition.Start:
                        childLayout.X = startX;
                        break;
                    case LayoutPosition.Center:
                        childLayout.X = startX + ((remainingWidth / 2) - (childLayout.Width / 2));
                        break;
                    case LayoutPosition.End:
                        childLayout.X = startX + (remainingWidth - childLayout.Width);
                        break;
                    case LayoutPosition.Stretch:
                        childLayout.X = startX;
                        childLayout.Width = remainingWidth;
                        break;
                }
                switch (Control.VertPosition)
                {
                    case LayoutPosition.Start:
                    case LayoutPosition.Center:
                    case LayoutPosition.End:
                        childLayout.Y = startY;
                        break;
                    case LayoutPosition.Stretch:
                        childLayout.Y = startY;
                        childLayout.Height = remainingHeight;
                        break;
                }

                startY += childLayout.Height;
            }
        }
    }
}
