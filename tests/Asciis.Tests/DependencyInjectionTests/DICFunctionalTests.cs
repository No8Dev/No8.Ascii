using No8.Ascii.DependencyInjection;
using No8.Ascii.Tests.DependencyInjectionTests.PlatformTestSuite;
using No8.Ascii.Tests.DependencyInjectionTests.TestData;
using No8.Ascii.Tests.DependencyInjectionTests.TestData.NestedDependencies;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.DependencyInjectionTests;

[TestClass]
public class DICFunctionalTests
{
    [Fact]
    public void NestedInterfaceDependencies_CorrectlyRegistered_ResolvesRoot()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<IService1, Service1>();
        container.Register<IService2, Service2>();
        container.Register<IService3, Service3>();
        container.Register<RootClass>();

        var result = container.Resolve<RootClass>();

        Assert.IsType<RootClass>(result);
    }

    [Fact]
    public void NestedInterfaceDependencies_MissingIService3Registration_ThrowsExceptionWithDefaultSettings()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<IService1, Service1>();
        container.Register<IService2, Service2>();
        container.Register<RootClass>();

        Assert.Throws<DICResolutionException>(() => container.Resolve<RootClass>());
    }

    [Fact]
    public void NestedClassDependencies_CorrectlyRegistered_ResolvesRoot()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<NestedClassDependencies.Service1>();
        container.Register<NestedClassDependencies.Service2>();
        container.Register<NestedClassDependencies.Service3>();
        container.Register<NestedClassDependencies.RootClass>();

        var result = container.Resolve<NestedClassDependencies.RootClass>();

        Assert.IsType<NestedClassDependencies.RootClass>(result);
    }

    [Fact]
    public void NestedClassDependencies_UsingConstructorFromAnotherType_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();
        var registerOptions = container.Register<NestedClassDependencies.RootClass>();

        Assert.Throws<DICConstructorResolutionException>
            (() => registerOptions.UsingConstructor(() => new RootClass(null!, null!)));
    }

    [Fact]
    public void NestedClassDependencies_MissingService3Registration_ResolvesRootResolutionOn()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<NestedClassDependencies.Service1>();
        container.Register<NestedClassDependencies.Service2>();
        container.Register<NestedClassDependencies.RootClass>();

        var result = container.Resolve<NestedClassDependencies.RootClass>(new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve });

        Assert.IsType<NestedClassDependencies.RootClass>(result);
    }

    [Fact]
    public void NestedClassDependencies_MissingService3RegistrationAndUnRegisteredResolutionOff_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<NestedClassDependencies.Service1>();
        container.Register<NestedClassDependencies.Service2>();
        container.Register<NestedClassDependencies.RootClass>();

        Assert.Throws<DICResolutionException>(() => container.Resolve<NestedClassDependencies.RootClass>(new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.Fail }));
    }

    [Fact]
    public void NestedInterfaceDependencies_JustAutoRegisterCalled_ResolvesRoot()
    {
        var container = UtilityMethods.GetContainer();
        container.AutoRegister(new[] { this.GetType().Assembly });

        var result = container.Resolve<RootClass>();

        Assert.IsType<RootClass>(result);
    }

    [Fact]
    public void Dependency_Hierarchy_With_Named_Factories_Resolves_All_Correctly()
    {
        var container = UtilityMethods.GetContainer();
        var mainView = new MainView();
        container.Register<IViewManager>(mainView);
        container.Register<IView, MainView>(mainView, "MainView");
        container.Register<IView, SplashView>("SplashView").UsingConstructor(() => new SplashView());
        container.Resolve<IView>("MainView");
        container.Register<IStateManager, StateManager>();
        var stateManager = container.Resolve<IStateManager>();

        stateManager.Init();

        Assert.IsType<SplashView>(mainView.LoadedView);
    }

    [Fact]
    public void Dependency_Hierarchy_Resolves_IEnumerable_Correctly()
    {
        var container = UtilityMethods.GetContainer();
        var mainView = new MainView();
        container.Register<IView, MainView>(mainView, "MainView");
        container.Register<IView, SplashView>("SplashView").UsingConstructor(() => new SplashView());
        var viewCollection = container.Resolve<ViewCollection>();
        Assert.Equal(2, viewCollection.Views.Count());
    }

    [Fact]
    public void When_Unable_To_Resolve_Nested_Dependency_Should_Include_That_Type_In_The_Exception()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<IService1, Service1>();
        container.Register<IService2, Service2>();
        container.Register<IRoot, RootClass>();

        DICResolutionException? e = null;
        try
        {
            container.Resolve<IRoot>();
        }
        catch (DICResolutionException ex)
        {
            e = ex;
        }

        Assert.NotNull(e);
        Assert.Contains("IService3", e?.ToString());
    }

    [Fact]
    public void When_Unable_To_Resolve_Non_Nested_Dependency_Should_Include_That_Type_In_The_Exception()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<IService2, Service2>();
        container.Register<IService3, Service3>();
        container.Register<IRoot, RootClass>();

        DICResolutionException? e = null;
        try
        {
            container.Resolve<IRoot>();
        }
        catch (DICResolutionException ex)
        {
            e = ex;
        }

        Assert.NotNull(e);
        Assert.Contains("IService1", e?.ToString());
    }

    [Fact]
    public void Run_Platform_Tests()
    {
        var logger = new StringLogger();
        var platformTests = new PlatformTests(logger);

        platformTests.RunTests(out _, out _, out var failed);

        Assert.Equal(0, failed);
    }

    [Fact]
    public void Resolve_InterfacesAcrossInChildContainer_Resolves()
    {
        var container = UtilityMethods.GetContainer();

        container.Register<IService2, Service2>().AsMultiInstance();

        container.Register<IService4, Service4>().AsMultiInstance();

        container.Register<IService5, Service5>().AsMultiInstance();

        var child = container.GetChildContainer();

        var nestedService = new Service3();
        child.Register<IService3>(nestedService);

        var service5 = child.Resolve<IService5>();

        Assert.NotNull(service5.Service4);

        Assert.Same(nestedService, service5.Service4.Service2.Service3);
    }
}