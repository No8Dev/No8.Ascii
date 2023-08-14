namespace No8.Ascii.TermInfo;

/// <summary>
///     Terminfo description
/// </summary>
public sealed partial class TermInfoDesc
{
    private readonly bool?[] _booleans;
    private readonly int?[] _nums;
    private readonly string?[] _strings;

    public string[] Names { get; }

    public ExtendedCapabilities Extended { get; }

    internal TermInfoDesc(
        string[] names, 
        bool?[] booleans, 
        int?[] nums,
        string?[] strings, 
        ExtendedCapabilities? extended = null)
    {
        Names = names;
        _booleans = booleans;
        _nums = nums;
        _strings = strings;

        Extended = extended ?? new ExtendedCapabilities();
    }

    public bool? GetBoolean(TermInfoCaps.Boolean value)
    {
        var index = (int)value;
        return index >= _booleans.Length ? null : _booleans[index];
    }

    public int? GetNum(TermInfoCaps.Num value)
    {
        var index = (int)value;
        if (index >= _nums.Length)
            return null;

        var result = _nums[index];
        return result is null or -1 ? null : result;
    }

    public string? GetString(TermInfoCaps.String value)
    {
        var index = (int)value;
        return index >= _strings.Length ? null : _strings[index];
    }
    
    /****************************************************************************/
    
    /// <summary>
    ///     Loads the default terminfo description for the current terminal
    /// </summary>
    public static bool TryLoad(out TermInfoDesc? result)
    {
        try
        {
            result = TermInfoLoader.Load();
            return result != null;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    ///     Loads the specified terminfo description for the current terminal
    /// </summary>
    /// <param name="name">The terminfo name to load.</param>
    public static bool TryLoad(string name, out TermInfoDesc? result)
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name));

        try
        {
            result = TermInfoLoader.Load(name);
            return result != null;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    ///     Loads a terminfo description from a stream
    /// </summary>
    public static bool TryLoad(Stream stream, out TermInfoDesc? result)
    {
        try
        {
            result = TermInfoLoader.Load(stream);
            return result != null;
        }
        catch
        {
            result = null;
            return false;
        }
    }

}