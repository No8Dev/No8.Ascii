namespace Asciis.UI;

/// <summary>
/// 2-dimensional fix array
/// </summary>
public class Array2D<T> where T : class
{
    /// The number of elements in a row of the array.
    public int Width => Bounds.Width;

    /// The number of elements in a column of the array.
    public int Height => Bounds.Height;

    private readonly T[] _elements;

    /// Creates a new array with [width], [height] elements initialised to [value]
    /// (or `null` if [value] is omitted).
    public Array2D(int width, int height, T? value = null)
    {
        Bounds = new Rect(0, 0, width, height);
        _elements = new T[width * height];
        if (value != null) Fill(value);
    }

    public Array2D<T> Generated(int width, int height, Func<Vec, T> generator)
    {
        var arr = new Array2D<T>(width, height);
        arr.Generate(generator);
        return arr;
    }

    public Array2D<T> Generated(int width, int height, Func<int, int, T> generator)
    {
        var arr = new Array2D<T>(width, height);
        arr.Generate(generator);
        return arr;
    }

    public Array2D<T> Clone()
    {
        return Generated(Width, Height, (x, y) => this[y, x]);
    }

    /// Gets the element at [pos].
    public T this[Vec pos]
    {
        get => _elements[pos.Y * Width + pos.X];
        set => _elements[pos.Y * Width + pos.X] = value;
    }

    public T this[int y, int x]
    {
        get => _elements[y * Width + x];
        set => _elements[y * Width + x] = value;
    }

    /// A [Rect] whose bounds cover the full range of valid element indexes.
    public Rect Bounds { get; }
    // Store the bounds rect instead of simply the width and height because this
    // is accessed very frequently and avoids allocating a new Rect each time.

    /// The size of the array.
    public Vec Size => Bounds.Size;

    /// Gets the element in the array at [x], [y].
    public T Get(int x, int y) => _elements[y.Clamp(0, Height - 1) * Width + x.Clamp(0, Width - 1)];

    /// Sets the element in the array at [x], [y] to [value].
    public void Set(int x, int y, T value) { _elements[y.Clamp(0, Height - 1) * Width + x.Clamp(0, Width - 1)] = value; }

    /// Sets every element to [value].
    public void Fill(T value)
    {
        for (int i = 0; i < _elements.Length; i++) _elements[i] = value;
    }

    void Generate(Func<Vec, T> generator)
    {
        for (int y = Bounds.Top; y <= Bounds.Bottom; y++)
        {
            for (int x = Bounds.Left; x <= Bounds.Right; x++)
                this[y, x] = generator(new(x, y));
        }
    }

    void Generate(Func<int, int, T> generator)
    {
        for (int y = Bounds.Top; y <= Bounds.Bottom; y++)
        {
            for (int x = Bounds.Left; x <= Bounds.Right; x++)
                this[y, x] = generator(x, y);
        }
    }
}
