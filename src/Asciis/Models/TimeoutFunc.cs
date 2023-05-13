namespace No8.Ascii;

public class TimeoutFunc
{
    public TimeSpan         Span     { get; }
    public Func<IApp, bool> Callback { get; }

    public TimeoutFunc(TimeSpan span, Func<IApp, bool> callback)
    {
        Span     = span;
        Callback = callback;
    }
}