namespace No8.Ascii.Tests.DependencyInjectionTests.TestData.NestedDependencies;

internal interface IService1
{
}

internal interface IService2
{
    IService3 Service3 { get; }
}

internal interface IService3
{
}

internal interface IService4
{
    IService2 Service2 { get; }
}

internal interface IService5
{
    IService4 Service4 { get; }
}

internal class Service1 : IService1
{
}

internal class Service2 : IService2
{
    public IService3 Service3 { get; private set; }

    public Service2(IService3 service3)
    {
        Service3 = service3;
    }
}

internal class Service3 : IService3
{
}

internal class Service4 : IService4
{
    public IService2 Service2 { get; private set; }

    public Service4(IService2 service1)
    {
        Service2 = service1;
    }
}

internal class Service5 : IService5
{
    public Service5(IService4 service4)
    {
        Service4 = service4;
    }

    public IService4 Service4 { get; private set; }
}


internal interface IRoot
{

}

internal class RootClass : IRoot
{
    private IService1 _service1;
    private IService2 _service2;

    public string StringProperty { get; set; }
    public int    IntProperty    { get; set; }

    public RootClass(IService1 service1, IService2 service2) : this(service1, service2, "DEFAULT", 1976)
    {
    }

    public RootClass(IService1 service1, IService2 service2, string stringProperty, int intProperty)
    {
        _service1  = service1;
        _service2  = service2;
        StringProperty = stringProperty;
        IntProperty    = intProperty;
    }
}