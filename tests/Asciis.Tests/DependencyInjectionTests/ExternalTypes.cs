namespace No8.Ascii.Tests.DependencyInjectionTests;

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