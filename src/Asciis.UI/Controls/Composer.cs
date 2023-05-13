using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Asciis.UI.Controls;

public abstract class Composer
{
    protected static readonly Dictionary<Type, Type> Composers = new();

    /// <summary>
    /// Overwrite the default composer for a particular control
    /// </summary>
    public static void Register(Type controlType, Type composerType)
    {
        Register(true, controlType, composerType);
    }

    internal static void Register(bool overwrite, Type controlType, Type composerType)
    {
        if (!controlType.IsSubclassOf(typeof(Control)))
            throw new ArgumentException("Must be subclass of Asciis.UI.Controls.Control", nameof(controlType));
        if (!composerType.IsSubclassOf(typeof(Composer)))
            throw new ArgumentException("Must be subclass of Asciis.UI.Composer", nameof(composerType));

        if (overwrite || !Composers.ContainsKey(controlType))
            Composers[controlType] = composerType;
    }

    public static Composer MeasureArrange<TControl>(TControl control, float width, float height)
        where TControl : Control
    {
        var composerType = Composers[control.GetType()];
        var composer = (Composer?)Activator.CreateInstance(composerType, control)
                    ?? throw new KeyNotFoundException();

        var area = new RectF(0, 0, width, height);

        composer.DoMeasure(area);
        composer.DoArrange(area);

        return composer;
    }

    public static Composer CreateComposerFor<TControl>(TControl control)
        where TControl : Control
    {
        var composerType = Composers[control.GetType()];
        return (Composer?)Activator.CreateInstance(composerType, control)
            ?? throw new KeyNotFoundException();
    }

    public static Composer LayoutAndDraw<TControl>(TControl control, Canvas canvas)
        where TControl : Control
    {
        var composer = CreateComposerFor(control);

        var area = new RectF(0, 0, canvas.Width, canvas.Height);

        composer.DoMeasure(area);
        composer.DoArrange(area);
        composer.Draw(canvas, null);

        return composer;
    }

    public virtual bool HandleKeyEvent(AKeyEventArgs keyArgs)
    {
        if (Control.HandleKeyEvent(keyArgs))
            return true;

        if (keyArgs.Handled)
            return true;

        foreach (var child in ChildComposers)
        {
            if (child.HandleKeyEvent(keyArgs))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Override this method to handle accelerator type keys. e.g. Alt-A
    /// Return true to stop processing key event
    /// </summary>
    public virtual bool PreProcessKey(AKeyEventArgs keyArgs)
    {
        if (Control.PreProcessKey(keyArgs))
            return true;

        if (keyArgs.Handled)
            return true;

        foreach (var child in ChildComposers)
        {
            if (child.PreProcessKey(keyArgs))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Override this method to handle keys not handled by other controls.e.g. Enter, Tab
    /// Return true to stop processing key event
    /// </summary>
    public virtual bool PostProcessKey(AKeyEventArgs keyArgs)
    {
        if (Control.PostProcessKey(keyArgs))
            return true;

        if (keyArgs.Handled)
            return true;

        foreach (var child in ChildComposers)
        {
            if (child.PostProcessKey(keyArgs))
                return true;
        }
        return false;
    }

    public virtual bool HandlePointerEvent(PointerEventArgs pointerEvent)
    {
        switch (pointerEvent.PointerEventType)
        {
            case PointerEventType.Enter:
                Control.IsMouseOver = true;
                break;
            case PointerEventType.Leave:
                Control.IsMouseOver = false;
                break;
        }

        return false;
    }

    public Composer? Parent { get; set; }
    public abstract Control Control { get; }
    public abstract Layout Layout { get; }
    public List<Composer> ChildComposers = new();

    protected void Clear()
    {
        foreach (var child in ChildComposers)
            child.Clear();
        ChildComposers.Clear();
    }

    public bool SetFocus() =>
        Control.Host?.FocusManager.SetFocus(this) ?? false;

    public virtual Vec FocusCursor =>
        new((int)(Layout.LayoutX + Layout.Edges.Left), (int)(Layout.LayoutY + Layout.Edges.Top));

    public virtual void DoMeasure(RectF area) { }
    public virtual void DoArrange(RectF contentArea) { }
    public abstract void Draw(Canvas canvas, RectF? clip);
    public abstract void UpdateLayoutValues();
}

public abstract class Composer<TControl, TLayout> : Composer
    where TControl : Control
    where TLayout : Layout<TControl>
{
    // ReSharper disable InconsistentNaming
    protected TLayout _layout;
    protected TControl _control;
    // ReSharper restore InconsistentNaming

    public override TLayout Layout =>
        _layout;
    public override TControl Control =>
        _control;
    protected Composer([NotNull] TControl control)
    {
        _control = control;
        ApplyStyle();

        _layout = (TLayout?)Activator.CreateInstance(typeof(TLayout), _control) ?? throw new Exception();
    }

    public Composer CreateComposerForChild(Control control)
    {
        var composerType = Composers[control.GetType()];

        var composer = (Composer?)Activator.CreateInstance(composerType, control)
            ?? throw new KeyNotFoundException();
        composer.Layout.Parent = Layout;
        composer.Parent = this;

        ChildComposers.Add(composer);
        Layout.Add(composer.Layout);

        return composer;
    }

    /// <summary>
    /// Updates Layout values from Control
    /// </summary>
    public override void UpdateLayoutValues()
    {
        Layout.UpdateValues(Control);
        foreach (var child in ChildComposers)
            child.UpdateLayoutValues();
    }

    public override void DoMeasure(RectF area)
    {
        if (Layout == null)
            throw new NoNullAllowedException("Composer must set a Layout ");

        // Root element *MUST* be the same size as the canvas
        Layout.Width = Layout.Parent == null
                           ? area.Width
                           : area.Width.Clamp(Control.MinWidth, Control.MaxWidth);
        Layout.Height = Layout.Parent == null
                            ? area.Height
                            : area.Height.Clamp(Control.MinHeight, Control.MaxHeight);

        var edges = Layout.Edges;
        var childArea = Layout.ContentArea;

        Clear();
        var (maxWidth, maxHeight) = MeasureChildren(childArea);

        Layout.ChildrenMaxWidth = maxWidth;
        Layout.ChildrenMaxHeight = maxHeight;

        if (Layout.Parent == null)
            return;

        if (maxWidth > 0)
        {
            switch (Control.HorzPosition)
            {
                case LayoutPosition.Start:
                case LayoutPosition.Center:
                case LayoutPosition.End:
                    Layout.Width = maxWidth + edges.Left + edges.Right;

                    break;
                case LayoutPosition.Stretch:
                    Layout.Width = float.NaN;

                    break;
            }
        }
        if (maxHeight > 0)
        {
            switch (Control.VertPosition)
            {
                case LayoutPosition.Start:
                case LayoutPosition.Center:
                case LayoutPosition.End:
                    Layout.Height = maxHeight + edges.Top + edges.Bottom;

                    break;
                case LayoutPosition.Stretch:
                    Layout.Height = float.NaN;

                    break;
            }
        }

        Layout.Width = Layout.Width.Clamp(Control.MinWidth, Control.MaxWidth);
        Layout.Height = Layout.Height.Clamp(Control.MinHeight, Control.MaxHeight);
    }

    protected virtual (float maxWidth, float maxHeight) MeasureChildren(in RectF area)
    {
        float maxWidth = 0;
        float maxHeight = 0;

        foreach (var child in Control.Children)
        {
            var composer = CreateComposerForChild(child);

            composer.DoMeasure(area);

            var childLayout = composer.Layout;

            if (!childLayout.Width.IsUndefined() && childLayout.Width > maxWidth)
                maxWidth = childLayout.Width;
            if (!childLayout.Height.IsUndefined() && childLayout.Height > maxHeight)
                maxHeight = childLayout.Height;
        }

        return (maxWidth, maxHeight);
    }

    public override void DoArrange(RectF contentArea)
    {
        ArrangeChildren();

        if (Layout.Width.IsUndefined())
            Layout.Width = contentArea.Width;
        if (Layout.Height.IsUndefined())
            Layout.Height = contentArea.Height;

        switch (Control.HorzPosition)
        {
            case LayoutPosition.Start:
                Layout.X = contentArea.X;
                break;
            case LayoutPosition.Center:
                Layout.X = contentArea.X + ((contentArea.Width / 2) - (Layout.Width / 2));
                break;
            case LayoutPosition.End:
                Layout.X = contentArea.X + (contentArea.Width - Layout.Width);
                break;
            case LayoutPosition.Stretch:
                Layout.X = contentArea.X;
                break;
        }
        switch (Control.VertPosition)
        {
            case LayoutPosition.Start:
                Layout.Y = contentArea.Y;
                break;
            case LayoutPosition.Center:
                Layout.Y = contentArea.Y + ((contentArea.Height / 2) - (Layout.Height / 2));
                break;
            case LayoutPosition.End:
                Layout.Y = contentArea.Y + (contentArea.Height - Layout.Height);
                break;
            case LayoutPosition.Stretch:
                Layout.Y = contentArea.Y;
                break;
        }
    }

    protected virtual void ArrangeChildren()
    {
        foreach (var composer in ChildComposers)
            composer.DoArrange(Layout.ContentArea);
    }

    public Style ApplyStyle()
    {
        var style = BuildStyle(Control.Style);
        var typeStyle = SkinManager.Instance.Current.GetStyle(Control);
        if (typeStyle != null)
            style = style.CombineWith(typeStyle);

        var controlProperties = Control.GetType().GetProperties(
            BindingFlags.Public |
            BindingFlags.Instance)
            .ToList();
        foreach (var item in style.Properties)
        {
            var property = controlProperties.FirstOrDefault(p => p.Name.Equals(item.Key, StringComparison.OrdinalIgnoreCase));
            if (property == null) continue;

            if (property.CanRead && property.CanWrite)
            {
                // Only overwrite property when it has not been explicitly set
                var value = property.GetValue(Control);
                if (value == null)
                    property.SetValue(Control, item.Value);
            }
        }

        return style;
    }

    public Style BuildStyle(Style? style)
    {
        if (style == null)
            return new Style();

        Style? baseStyle = null;
        if (style.BasedOn != null)
            baseStyle = BuildStyle(style.BasedOn);

        return baseStyle?.CombineWith(style) ?? style;
    }

    public override void Draw(Canvas canvas, RectF? clip)
    {
        clip = (clip == null) ? Layout.Bounds : RectF.Intersect(clip, Layout.Bounds);
        canvas.PushClip(clip);

        foreach (var child in ChildComposers)
        {
            var insideClippingBounds = clip?.ContainsAnyPart(child.Layout.Bounds) ?? true;

            if (insideClippingBounds)
                child.Draw(canvas, clip);
        }
        canvas.PopClip();
    }

}
