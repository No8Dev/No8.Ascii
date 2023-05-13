namespace Asciis.App.Tests.DependencyInjectionTests;

public interface IExternalTestInterface
{
}

public class ExternalTestClass : IExternalTestInterface
{
}

public interface IExternalTestInterface2
{
}

class ExternalTestClassInternal : IExternalTestInterface2
{
}