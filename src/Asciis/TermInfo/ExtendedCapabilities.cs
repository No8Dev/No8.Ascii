namespace No8.Ascii.TermInfo;

public sealed class ExtendedCapabilities
{
    private readonly Dictionary<string, bool?> _booleans;
    private readonly Dictionary<string, int?> _nums;
    private readonly Dictionary<string, string?> _strings;

    /// <summary>
    ///     Terminfo capability kinds
    /// </summary>
    public enum TermInfoCapsKind
    {
        Boolean = 0,
        Num = 1,
        String = 2,
    }
    
    public int Count { get; }

    internal ExtendedCapabilities()
    {
        _booleans = new Dictionary<string, bool?>();
        _nums = new Dictionary<string, int?>();
        _strings = new Dictionary<string, string?>();
    }

    internal ExtendedCapabilities(
        IEnumerable<bool?> booleans, 
        IEnumerable<int?> nums, 
        IEnumerable<string?> strings,
        
        IEnumerable<string> booleanNames, 
        IEnumerable<string> numNames, 
        IEnumerable<string> stringNames)
    {
        _booleans = booleanNames
            .Zip(booleans)
            .ToDictionarySafe(x => x.first, x => x.second);
        _nums = numNames
            .Zip(nums)
            .ToDictionarySafe(x => x.first, x => x.second);
        _strings = stringNames
            .Zip(strings)
            .ToDictionarySafe(x => x.first, x => x.second);

        Count = _booleans.Count + _nums.Count + _strings.Count;
    }

    public bool Exist(string key)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        return _booleans.ContainsKey(key)
               || _nums.ContainsKey(key)
               || _strings.ContainsKey(key);
    }

    public List<string> GetNames(TermInfoCapsKind kind)
    {
        return kind switch
        {
            TermInfoCapsKind.Boolean => new List<string>(_booleans.Keys),
            TermInfoCapsKind.Num => new List<string>(_nums.Keys),
            TermInfoCapsKind.String => new List<string>(_strings.Keys),
            _ => throw new NotSupportedException($"Unknown capability type '{kind}'"),
        };
    }

    public bool? GetBoolean(string key)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        _booleans.TryGetValue(key, out var value);
        return value;
    }

    public int? GetNum(string key)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        _nums.TryGetValue(key, out var value);
        return value;
    }

    public string? GetString(string key)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        _strings.TryGetValue(key, out var value);
        return value;
    }
}