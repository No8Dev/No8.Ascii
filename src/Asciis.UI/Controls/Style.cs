using System.Collections;
using System.Drawing;
using System.Dynamic;

namespace Asciis.UI.Controls;

public class Style : DynamicObject, IEnumerable<KeyValuePair<string, object>>
{
    public Style? BasedOn { get; init; } = null;

    internal Dictionary<string, object> Properties = new(StringComparer.OrdinalIgnoreCase);

    //********************************************************************

    public Style() { }

    public Style(Style baseStyle)
    {
        foreach (var item in baseStyle)
            Add(item);
    }

    //********************************************************************

    public object? this[string key]
    {
        get => Get<object?>(key);
        set => Set(key, value);
    }

    public T? Get<T>(string key)
    {
        if (Properties.ContainsKey(key))
            return (T)Properties[key];
        return default;
    }

    public void Set<T>(string key, T? value)
    {
        if (value == null)
            Properties.Remove(key);
        else
            Properties[key] = value;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        var key = binder.Name;
        if (Properties.ContainsKey(key))
        {
            result = Properties[key];
            return true;
        }
        result = null;
        return false;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        Set(binder.Name, value);
        return true;
    }

    //********************************************************************

    public LayoutPosition? HorzPosition
    {
        get => Get<LayoutPosition?>(nameof(HorzPosition));
        set => Set(nameof(HorzPosition), value);
    }
    public LayoutPosition? VertPosition
    {
        get => Get<LayoutPosition?>(nameof(VertPosition));
        set => Set(nameof(VertPosition), value);
    }

    public Edges? Padding
    {
        get => Get<Edges?>(nameof(Padding));
        set => Set(nameof(Padding), value);
    }

    public int? MinWidth
    {
        get => Get<int?>(nameof(MinWidth));
        set => Set(nameof(MinWidth), value);
    }
    public int? MaxWidth
    {
        get => Get<int?>(nameof(MaxWidth));
        set => Set(nameof(MaxWidth), value);
    }
    public int? MinHeight
    {
        get => Get<int?>(nameof(MinHeight));
        set => Set(nameof(MinHeight), value);
    }
    public int? MaxHeight
    {
        get => Get<int?>(nameof(MaxHeight));
        set => Set(nameof(MaxHeight), value);
    }

    public Color? Background
    {
        get => Get<Color?>(nameof(Background));
        set => Set(nameof(Background), value);
    }
    public Color? Foreground
    {
        get => Get<Color?>(nameof(Foreground));
        set => Set(nameof(Foreground), value);
    }
    public float? Opacity
    {
        get => Get<float?>(nameof(Opacity));
        set => Set(nameof(Opacity), value);
    }

    //********************************************************************

    public virtual Style CombineWith(Style style)
    {
        var newStyle = new Style(style);
        foreach (var item in Properties)
            newStyle.Add(item);
        return newStyle;
    }

    public T? OfType<T>() where T : Style
    {
        if (this is T t)
            return t;
        return BasedOn?.OfType<T>();
    }

    //********************************************************************

    public void Add(KeyValuePair<string, object> property)
    {
        Add(property.Key, property.Value);
    }

    public void Add(string key, object value)
    {
        Properties[key] = value;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        => Properties.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    //********************************************************************

}
