using System.Collections;
using System.Drawing;
using System.Text;

namespace Asciis.UI.Controls;

public abstract class Layout : IEnumerable<Layout>
{
    public Layout? Parent { get; internal set; } 

    public string? Name { get; internal set; }
    /// <summary>
    /// Relative to parent : X
    /// </summary>
    public float X { get; internal set; }
    /// <summary>
    /// Relative to parent : Y
    /// </summary>
    public float Y { get; internal set; }
    public float Width { get; internal set; }
    public float Height { get; internal set; }

    public float ChildrenMaxWidth { get; internal set; }
    public float ChildrenMaxHeight { get; internal set; }

    public Color Foreground { get; set; }
    public Color Background { get; set; }

    /// <summary>
    /// Absolute : X
    /// </summary>
    public float LayoutX
    {
        get
        {
            if (Parent != null)
                return Parent.LayoutX + X;

            return X;
        }
    }

    /// <summary>
    /// Absolute : Y
    /// </summary>
    public float LayoutY
    {
        get
        {
            if (Parent != null)
                return Parent.LayoutY + Y;

            return Y;
        }
    }

    public virtual RectF Bounds => new RectF(LayoutX, LayoutY, Width, Height);

    public virtual Edges Edges =>
        Padding;

    public RectF ContentArea
    {
        get
        {
            var width = Width;
            var height = Height;
            if (width.IsUndefined() && Parent != null)
                width = Parent.ContentArea.Width;
            if (height.IsUndefined() && Parent != null)
                height = Parent.ContentArea.Height;

            return new RectF(
                               Edges.Left,
                               Edges.Top,
                               (width - (Edges.Left + Edges.Right)),
                               (height - (Edges.Top + Edges.Bottom)));
        }
    }

    public Edges Padding { get; set; } = Edges.Zero;

    private readonly List<Layout> _children = new();

    //**********************************************

    public Layout Add(Layout layout)
    {
        if (layout.Parent != null)
            layout.Parent.Remove(layout);
        layout.Parent = this;
        _children.Add(layout);
        return this;
    }

    public Layout Add(IEnumerable<Layout> layouts)
    {
        foreach (var child in layouts)
            Add(child);
        return this;
    }

    public Layout Remove(Layout layout)
    {
        layout.Parent = null;
        _children.Remove(layout);
        return this;
    }

    public Layout Clear()
    {
        foreach (var child in _children)
            Remove(child);
        return this;
    }

    public Layout SetChildren(IEnumerable<Layout> layouts)
    {
        Clear();
        Add(layouts);
        return this;
    }

    public IReadOnlyList<Layout> Children() => _children.AsReadOnly();

    public IEnumerable<T> Children<T>() where T : Layout
    {
        return Enumerable.OfType<T>(_children.AsReadOnly());
    }

    public IEnumerator<Layout> GetEnumerator()
    {
        return _children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //**********************************************

    public bool Contains(Vec? pos)
    {
        if (pos == null || pos.Equals(Vec.Unknown)) 
            return false;

        if (pos.X < LayoutX) return false;
        if (pos.X >= LayoutX + Width) return false;
        if (pos.Y < LayoutY) return false;
        if (pos.Y >= LayoutY + Height) return false;

        return true;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        AppendProperties(sb);
        if (_children.Count > 0)
        {
            sb.AppendLine();
            AppendChildren(sb);
        }

        return sb.ToString();
    }

    protected void AppendChildren(StringBuilder sb)
    {
        foreach (var child in Children())
        {
            child.AppendProperties(sb);
            sb.AppendLine();
            child.AppendChildren(sb);
        }
    }

    protected virtual void AppendProperties(StringBuilder sb)
    {
        sb.Append($"{GetType().Name}");
        if (Name != null) sb.Append($" Name={Name}");
        sb.Append($" ({X},{Y} {Width}/{Height})");
        sb.Append($" ({LayoutX},{LayoutY})");
        if (!Padding.Equals(Edges.Zero)) 
            sb.Append($" Padding={Padding}");
        sb.Append($" Foreground={Foreground}");
        sb.Append($" Background={Background}");
    }
}

public abstract class Layout<TControl> : Layout
    where TControl : Control
{
    protected Layout(TControl control)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        UpdateValues(control);
    }

    public virtual void UpdateValues(TControl control)
    {
        Name = control.Name;
        Foreground = control.Foreground ?? Color.Black;
        Background = control.Background ?? Color.White;
        Padding = control.Padding ?? Edges.Zero;
    }
}
