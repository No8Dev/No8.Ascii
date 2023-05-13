using System.Diagnostics.CodeAnalysis;

namespace Asciis.App.DependencyInjection;

public class SafeDictionary<TKey, TValue> : IDisposable 
    where TKey : notnull
{
    private readonly ReaderWriterLockSlim     _padlock    = new();
    private readonly Dictionary<TKey, TValue> _dictionary = new();

    public TValue this[TKey key]
    {
        set
        {
            _padlock.EnterWriteLock();

            try
            {
                if (_dictionary.TryGetValue(key, out var current))
                {
                    if (current is IDisposable disposable)
                        disposable.Dispose();
                }

                _dictionary[key] = value;
            }
            finally
            {
                _padlock.ExitWriteLock();
            }
        }
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        _padlock.EnterReadLock();
        try
        {
            return _dictionary.TryGetValue(key, out value);
        }
        finally
        {
            _padlock.ExitReadLock();
        }
    }

    public bool Remove(TKey key)
    {
        _padlock.EnterWriteLock();
        try
        {
            return _dictionary.Remove(key);
        }
        finally
        {
            _padlock.ExitWriteLock();
        }
    }

    public void Clear()
    {
        _padlock.EnterWriteLock();
        try
        {
            _dictionary.Clear();
        }
        finally
        {
            _padlock.ExitWriteLock();
        }
    }

    public IEnumerable<TKey> Keys
    {
        get
        {
            _padlock.EnterReadLock();
            try
            {
                return new List<TKey>(_dictionary.Keys);
            }
            finally
            {
                _padlock.ExitReadLock();
            }
        }
    }

        

    public void Dispose()
    {
        _padlock.EnterWriteLock();

        try
        {
            var disposableItems = from item in _dictionary.Values
                where item is IDisposable
                select item as IDisposable;

            foreach (var item in disposableItems)
            {
                item.Dispose();
            }
        }
        finally
        {
            _padlock.ExitWriteLock();
        }

        GC.SuppressFinalize(this);
    }

}

/// <summary>
/// Name/Value pairs for specifying "user" parameters when resolving
/// </summary>
public sealed class NamedParameterOverloads : Dictionary<string, object>
{
    public static NamedParameterOverloads FromIDictionary(IDictionary<string, object> data)
    {
        return data as NamedParameterOverloads ?? new NamedParameterOverloads(data);
    }

    public NamedParameterOverloads()
    {
    }

    public NamedParameterOverloads(IDictionary<string, object> data)
        : base(data)
    {
    }

    public static NamedParameterOverloads Default { get; } = new();
}

public enum UnregisteredResolutionActions
{
    /// <summary>
    /// Attempt to resolve type, even if the type isn't registered.
    /// 
    /// Registered types/options will always take precedence.
    /// </summary>
    AttemptResolve,

    /// <summary>
    /// Fail resolution if type not explicitly registered
    /// </summary>
    Fail,

    /// <summary>
    /// Attempt to resolve unregistered type if requested type is generic
    /// and no registration exists for the specific generic parameters used.
    /// 
    /// Registered types/options will always take precedence.
    /// </summary>
    GenericsOnly
}


public enum NamedResolutionFailureActions
{
    AttemptUnnamedResolution,
    Fail
}

public enum DuplicateImplementationActions
{
    RegisterSingle,
    RegisterMultiple,
    Fail
}

/// <summary>
/// Resolution settings
/// </summary>
public sealed class ResolveOptions
{
    public UnregisteredResolutionActions UnregisteredResolutionAction { get; set; } = UnregisteredResolutionActions.AttemptResolve;
    public NamedResolutionFailureActions NamedResolutionFailureAction { get; set; } = NamedResolutionFailureActions.Fail;

    /// <summary>
    /// Gets the default options (attempt resolution of unregistered types, fail on named resolution if name not found)
    /// </summary>
    public static ResolveOptions Default { get; } = new();

    /// <summary>
    /// Preconfigured option for attempting resolution of unregistered types and failing on named resolution if name not found
    /// </summary>
    public static ResolveOptions FailNameNotFoundOnly { get; } = new() { NamedResolutionFailureAction = NamedResolutionFailureActions.Fail, UnregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve };

    /// <summary>
    /// Preconfigured option for failing on resolving unregistered types and on named resolution if name not found
    /// </summary>
    public static ResolveOptions FailUnregisteredAndNameNotFound { get; } = new() { NamedResolutionFailureAction = NamedResolutionFailureActions.Fail, UnregisteredResolutionAction = UnregisteredResolutionActions.Fail };

    /// <summary>
    /// Preconfigured option for failing on resolving unregistered types, but attempting unnamed resolution if name not found
    /// </summary>
    public static ResolveOptions FailUnregisteredOnly { get; } = new() { NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution, UnregisteredResolutionAction = UnregisteredResolutionActions.Fail };
}

[AttributeUsage(AttributeTargets.Constructor)]
public sealed class DICConstructorAttribute : Attribute
{
}

