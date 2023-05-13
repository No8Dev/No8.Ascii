namespace Asciis.Terminal;

public struct CellAttributes
{
    public static readonly CellAttributes Empty = new CellAttributes();

    public bool Underline { get; } = false;
    public bool Reverse { get; } = false;

    /// <summary>
    /// The foreground color.
    /// </summary>
    public ConsoleColor Foreground { get; }

    /// <summary>
    /// The background color.
    /// </summary>
    public ConsoleColor Background { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CellAttributes"/> struct.
    /// </summary>
    public CellAttributes()
    {
        Foreground = default;
        Background = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CellAttributes"/> struct.
    /// </summary>
    public CellAttributes(ConsoleColor foreground, ConsoleColor background, bool underline = false, bool reverse = false)
    {
        Foreground = foreground;
        Background = background;
        Underline = underline;
        Reverse = reverse;
    }

    public bool Equals(CellAttributes other)
    {
        return Foreground == other.Foreground && Background == other.Background;
    }

    public override bool Equals(object? obj)
    {
        return obj is CellAttributes other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Foreground, (int)Background);
    }

    public static bool operator ==(CellAttributes left, CellAttributes right) { return left.Equals(right); }
    public static bool operator !=(CellAttributes left, CellAttributes right) { return !left.Equals(right); }
}
