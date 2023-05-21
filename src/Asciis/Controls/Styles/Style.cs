using System.Collections;
using System.Dynamic;
using System.Reflection;

namespace No8.Ascii.Controls;


public class Style : DynamicObject, IEnumerable<KeyValuePair<string, object>>
{
    internal Dictionary<string, object> Properties = new(StringComparer.OrdinalIgnoreCase);
    private bool _isDirty;

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
        {
            if (Properties.ContainsKey(key))
            {
                Properties.Remove(key);
                IsDirty = true;
            }
        }
        else
            Add(key, value);
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

    /// <summary>
    ///     Has the planned valued changed since the last layout arrangement
    /// </summary>
    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (_isDirty != value)
            {
                _isDirty = value;

                // set dirty flag bubbles up
                // clear dirty flag propagates down
                if (value)
                    RaiseChanged(this);
            }
        }
    }

    /// <summary>
    ///     Called when the Style first changes
    /// </summary>
    public event EventHandler<Style>? Changed;

    protected virtual void RaiseChanged(Style e) => Changed?.Invoke(this, e);

    public Style OnChanged(EventHandler<Style>? onChanged)
    {
        if (onChanged != null)
            Changed += onChanged;
        return this;
    }

    public void Clear()
    {
        if (Properties.Count > 0)
        {
            Properties.Clear();
            IsDirty = true;
        }
    }

    //********************************************************************

    public Brush? BackgroundBrush
    {
        get => Get<Brush?>(nameof(BackgroundBrush));
        set => Set(nameof(BackgroundBrush), value);
    }
    public Brush? ForegroundBrush
    {
        get => Get<Brush?>(nameof(ForegroundBrush));
        set => Set(nameof(ForegroundBrush), value);
    }
    public float? Opacity
    {
        get => Get<float?>(nameof(Opacity));
        set => Set(nameof(Opacity), value);
    }

    //********************************************************************

    public float? TranslationX
    {
        get => Get<float?>(nameof(TranslationX));
        set => Set(nameof(TranslationX), value);
    }
    public float? TranslationY
    {
        get => Get<float?>(nameof(TranslationY));
        set => Set(nameof(TranslationY), value);
    }
    public float? ScaleX
    {
        get => Get<float?>(nameof(ScaleX));
        set => Set(nameof(ScaleX), value);
    }
    public float? ScaleY
    {
        get => Get<float?>(nameof(ScaleY));
        set => Set(nameof(ScaleY), value);
    }


    //********************************************************************

    public void Add(KeyValuePair<string, object> property)
    {
        Add(property.Key, property.Value);
    }

    public void Add(string key, object value)
    {
        bool changed = false;

        if (!Properties.ContainsKey(key))
            changed = true;
        else if (value != Properties[key])
            changed = true;

        Properties[key] = value;

        if (changed)
            IsDirty = true;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        => Properties.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    //********************************************************************

    public static Style Combine(Style baseStyle, Style other)
    {
        foreach (var item in other.Properties)
            baseStyle.Add(item);
        return baseStyle;
    }

    public Style MergeFrom(Style other)
    {
        foreach (var item in other.Properties)
            Add(item);
        return this;
    }

    //********************************************************************

    public void ApplyTo(object? obj)
    {
        if (obj == null)
            return;

        var objType    = obj!.GetType();
        var properties = objType.GetProperties( /*BindingFlags.Public | BindingFlags.Instance */).ToList();
        foreach (var (key, value) in Properties)
        {
            if (key.Contains('.'))
            {
                var childObj = obj;
                PropertyInfo? property = null;
                var keys     = key
                              .Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                              .ToList();
                for(int i = 0; i < keys.Count; i++)
                {
                    var childProperties = childObj
                                         .GetType()
                                         .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                         .ToList();
                    property = childProperties
                       .FirstOrDefault(p => p.Name.Equals(keys[i], StringComparison.OrdinalIgnoreCase));
                    if (property == null )
                        break;
                    if (i == keys.Count - 1)    // Don't get the object of the last key as that is what we want to set.
                        break;
                    childObj = property.CanRead ? property.GetValue(childObj) : null;
                    if (childObj == null)
                        break;
                }

                if (childObj is not null &&
                    property is not null &&
                    property.CanWrite)
                {
                    property.SetValue(childObj, value);
                }
            }
            else
            {
                var property =
                properties
                   .FirstOrDefault(p => p.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
                if (property is not null && property.CanWrite)
                    property.SetValue(obj, value);
            }
        }
    }
    
    //********************************************************************
}
