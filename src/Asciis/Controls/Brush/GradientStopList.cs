using System.Collections;

namespace No8.Ascii.Controls;

public sealed class GradientStopList : IReadOnlyList<GradientStop>
{
    private readonly List<GradientStop> _stops = new ();

    public int Count => _stops.Count;

    public GradientStopList() { }

    public GradientStopList(IEnumerable<GradientStop> source)
    {
        _stops.AddRange(source);
    }

    public void Add(GradientStop stop) => _stops.Add(stop);

    public IEnumerator<GradientStop> GetEnumerator() => _stops.GetEnumerator();
    IEnumerator IEnumerable.         GetEnumerator() => GetEnumerator();
    public GradientStop this[int index] => _stops[index];
}