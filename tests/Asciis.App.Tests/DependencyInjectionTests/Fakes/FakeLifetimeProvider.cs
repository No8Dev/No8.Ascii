using Asciis.App.DependencyInjection;

namespace Asciis.App.Tests.DependencyInjectionTests.Fakes;

public class FakeLifetimeProvider : DependencyInjectionContainer.IDICObjectLifetimeProvider
{
    public object? TheObject { get; set; }

    public object? GetObject()
    {
        return TheObject;
    }

    public void SetObject(object value)
    {
        TheObject = value;
    }

    public void ReleaseObject()
    {
        TheObject = null;
    }
}