using System.Reflection;
using No8.Ascii.DependencyInjection;
using No8.Ascii.Tests.DependencyInjectionTests.Fakes;
using No8.Ascii.Tests.DependencyInjectionTests.TestData;
using No8.Ascii.Tests.DependencyInjectionTests.TestData.BasicClasses;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.DependencyInjectionTests;

[TestClass]
public class DICTests
{
    [Fact]
    public void Current_Get_ReturnsInstanceOfTinyIoC()
    {
        var container = DependencyInjectionContainer.Current;

        Assert.IsType<DependencyInjectionContainer>(container);
    }

    [Fact]
    public void Current_GetTwice_ReturnsSameInstance()
    {
        var container1 = DependencyInjectionContainer.Current;
        var container2 = DependencyInjectionContainer.Current;

        Assert.Same(container1, container2);
    }

    [Fact]
    public void Register_ImplementationOnly_CanRegister()
    {
        UtilityMethods.GetContainer().Register<TestClassDefaultCtor>();
    }

    [Fact]
    public void Register_InterfaceAndImplementation_CanRegister()
    {
        UtilityMethods.GetContainer().Register<ITestInterface, TestClassDefaultCtor>();
    }

    [Fact]
    public void Resolve_RegisteredTypeWithImplementation_ReturnsInstanceOfCorrectType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        var output = container.Resolve<ITestInterface>();

        Assert.IsType<TestClassDefaultCtor>(output);
    }

    [Fact]
    public void Resolve_RegisteredTypeWithImplementation_ReturnsSingleton()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        var output  = container.Resolve<ITestInterface>();
        var output2 = container.Resolve<ITestInterface>();

        Assert.Same(output, output2);
    }

    [Fact]
    public void Resolve_RegisteredTypeImplementationOnly_ReturnsInstanceOfCorrectType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();

        var output = container.Resolve<TestClassDefaultCtor>();

        Assert.IsType<TestClassDefaultCtor>(output);
    }

    [Fact]
    public void Resolve_RegisteredTypeImplementationOnly_ReturnsMultipleInstances()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();

        var output  = container.Resolve<TestClassDefaultCtor>();
        var output2 = container.Resolve<TestClassDefaultCtor>();

        Assert.False(ReferenceEquals(output, output2));
    }

    [Fact]
    public void Register_WithDelegateFactoryStaticMethod_CanRegister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register((c, p) => TestClassDefaultCtor.CreateNew(c));
    }

    [Fact]
    public void Register_WithDelegateFactoryLambda_CanRegister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface>((c, p) => new TestClassDefaultCtor() { Prop1 = "Testing" });
    }

    [Fact]
    public void Register_WithDelegateFactory_CanRegisterSingleton()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface>((c, p) => new TestClassDefaultCtor() { Prop1 = "Testing" })
                 .AsSingleton();
    }

    [Fact]
    public void Resolve_TypeRegisteredWithDelegateFactoryStaticMethod_ResolvesCorrectlyUsingDelegateFactory()
    {
        var container = UtilityMethods.GetContainer();
        container.Register((c, p) => TestClassDefaultCtor.CreateNew(c));

        var output = container.Resolve<ITestInterface>() as TestClassDefaultCtor;

        Assert.Equal("Testing", output!.Prop1);
    }

    [Fact]
    public void Resolve_TypeRegisteredWithDelegateFactoryLambda_ResolvesCorrectlyUsingDelegateFactory()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface>((c, p) => new TestClassDefaultCtor() { Prop1 = "Testing" });

        TestClassDefaultCtor? output = container.Resolve<ITestInterface>() as TestClassDefaultCtor;

        Assert.Equal("Testing", output?.Prop1);
    }

    [Fact]
    public void Resolve_TypeRegisteredWithDelegateFactory_ResolvesSameInstance()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface>((c, p) => new TestClassDefaultCtor() { Prop1 = "Testing" })
                 .AsSingleton();

        var instance1 = container.Resolve<ITestInterface>();
        var instance2 = container.Resolve<ITestInterface>();

        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Resolve_UnregisteredClassTypeWithDefaultCtor_ResolvesType()
    {
        var container = UtilityMethods.GetContainer();
        var output    = container.Resolve<TestClassDefaultCtor>();

        Assert.IsType<TestClassDefaultCtor>(output);
    }

    [Fact]
    public void Resolve_UnregisteredClassTypeWithDependencies_ResolvesType()
    {
        var container = UtilityMethods.GetContainer();

        var output = container.Resolve<TestClassWithDependency>();

        Assert.IsType<TestClassWithDependency>(output);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_UnregisteredInterface_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();
        Assert.Throws<DICResolutionException>(() => container.Resolve<ITestInterface>());

        //Assert.IsType<>(output, typeof(TestClassDefaultCtor)));
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_UnregisteredClassWithUnregisteredInterfaceDependencies_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();
        Assert.Throws<DICResolutionException>(
            () => container.Resolve<TestClassWithInterfaceDependency>());

        //Assert.IsType<>(output, typeof(TestClassWithInterfaceDependency)));
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_RegisteredClassWithUnregisteredInterfaceDependencies_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithInterfaceDependency>();

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<TestClassWithInterfaceDependency>());

        //Assert.IsType<>(output, typeof(TestClassWithInterfaceDependency)));
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_RegisteredInterfaceWithUnregisteredInterfaceDependencies_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface2, TestClassWithInterfaceDependency>();

        Assert.Throws<DICResolutionException>(() => container.Resolve<ITestInterface2>());

        //Assert.IsType<>(output, typeof(TestClassWithInterfaceDependency)));
    }

    [Fact]
    public void CanResolveType_RegisteredTypeDefaultCtor_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();

        var result = container.CanResolve<TestClassDefaultCtor>();

        Assert.True(result);
    }

    [Fact]
    public void CanResolveType_UnregisteredTypeDefaultCtor_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        var result    = container.CanResolve<TestClassDefaultCtor>();

        Assert.True(result);
    }

    [Fact]
    public void CanResolveType_UnregisteredInterface_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();
        var result    = container.CanResolve<ITestInterface>();

        Assert.False(result);
    }

    [Fact]
    public void CanResolveType_RegisteredInterface_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        var result = container.CanResolve<ITestInterface>();

        Assert.True(result);
    }

    [Fact]
    public void CanResolveType_RegisteredTypeWithRegisteredDependencies_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassWithDependency>();

        var result = container.CanResolve<TestClassWithDependency>();

        Assert.True(result);
    }

    [Fact]
    public void Resolve_RegisteredTypeWithRegisteredDependencies_Resolves()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassWithDependency>();

        var result = container.Resolve<TestClassWithDependency>();

        Assert.IsType<TestClassWithDependency>(result);
    }

    [Fact]
    public void CanResolveType_RegisteredTypeWithRegisteredDependenciesAndParameters_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassWithDependencyAndParameters>();

        var result = container.CanResolve<TestClassWithDependencyAndParameters>(
            new NamedParameterOverloads { { "param1", 12 }, { "param2", "Testing" } });

        Assert.True(result);
    }

    [Fact]
    public void Resolve_RegisteredTypeWithRegisteredDependenciesAndParameters_Resolves()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassWithDependencyAndParameters>();

        var result = container.Resolve<TestClassWithDependencyAndParameters>(
            new NamedParameterOverloads { { "param1", 12 }, { "param2", "Testing" } });

        Assert.IsType<TestClassWithDependencyAndParameters>(result);
    }

    [Fact]
    public void Resolve_RegisteredTypeWithRegisteredDependenciesAndParameters_ResolvesWithCorrectConstructor()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassWithDependencyAndParameters>();

        var result = container.Resolve<TestClassWithDependencyAndParameters>(
            new NamedParameterOverloads { { "param1", 12 }, { "param2", "Testing" } });

        Assert.Equal(12, result.Param1);
        Assert.Equal("Testing", result.Param2);
    }

    [Fact]
    public void CanResolveType_RegisteredTypeWithRegisteredDependenciesAndIncorrectParameters_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassWithDependencyAndParameters>();

        var result = container.CanResolve<TestClassWithDependencyAndParameters>(
            new NamedParameterOverloads { { "wrongparam1", 12 }, { "wrongparam2", "Testing" } });

        Assert.False(result);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_RegisteredTypeWithRegisteredDependenciesAndIncorrectParameters_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassWithDependencyAndParameters>();

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<TestClassWithDependencyAndParameters>(
                new NamedParameterOverloads { { "wrongparam1", 12 }, { "wrongparam2", "Testing" } }));

        //Assert.IsType<>(result, typeof(TestClassWithDependencyAndParameters));
    }

    [Fact]
    public void CanResolveType_FactoryRegisteredType_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register((c, p) => TestClassDefaultCtor.CreateNew(c));

        var result = container.CanResolve<ITestInterface>();

        Assert.True(result);
    }

    [Fact]
    public void CanResolveType_FactoryRegisteredTypeThatThrows_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface>((c, p) => { throw new NotImplementedException(); });

        var result = container.CanResolve<ITestInterface>();

        Assert.True(result);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_FactoryRegisteredTypeThatThrows_ThrowsCorrectException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface>((c, p) => { throw new NotImplementedException(); });

        Assert.Throws<DICResolutionException>(() => container.Resolve<ITestInterface>());

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentNullException))]
    public void Register_NullFactory_ThrowsCorrectException()
    {
        var container = UtilityMethods.GetContainer();
        Func<DependencyInjectionContainer, NamedParameterOverloads, ITestInterface>? factory = null;
        Assert.Throws<ArgumentNullException>(() => container.Register(factory!));

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    public void Resolve_TinyIoC_ReturnsCurrentContainer()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.Resolve<DependencyInjectionContainer>();

        Assert.Same(result, container);
    }

    [Fact]
    public void Resolve_ClassWithTinyIoCDependency_Resolves()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithContainerDependency>();

        var result = container.Resolve<TestClassWithContainerDependency>();

        Assert.IsType<TestClassWithContainerDependency>(result);
    }

    [Fact]
    public void Register_Instance_CanRegister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register(new DisposableTestClassWithInterface());
    }

    [Fact]
    public void Register_InstanceUsingInterface_CanRegister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface>(new DisposableTestClassWithInterface());
    }

    [Fact]
    public void Resolve_RegisteredInstance_SameInstance()
    {
        var container = UtilityMethods.GetContainer();
        var item      = new DisposableTestClassWithInterface();
        container.Register(item);

        var result = container.Resolve<DisposableTestClassWithInterface>();

        Assert.Same(item, result);
    }

    [Fact]
    public void Resolve_RegisteredInstanceWithInterface_SameInstance()
    {
        var container = UtilityMethods.GetContainer();
        var item      = new DisposableTestClassWithInterface();
        container.Register<ITestInterface>(item);

        var result = container.Resolve<ITestInterface>();

        Assert.Same(item, result);
    }

#if MOQ
        [Fact]
        public void Dispose_RegisteredDisposableInstance_CallsDispose()
        {
            var item = new Mock<DisposableTestClassWithInterface>();
            var disposableItem = item.As<IDisposable>();
            disposableItem.Setup(i => i.Dispose());

            var container = UtilityMethods.GetContainer();
            container.Register<DisposableTestClassWithInterface>(item.Object);

            container.Dispose();

            item.VerifyAll();
        }
#endif

#if MOQ
        [Fact]
        public void Dispose_RegisteredDisposableInstanceWithInterface_CallsDispose()
        {
            var item = new Mock<DisposableTestClassWithInterface>();
            var disposableItem = item.As<IDisposable>();
            disposableItem.Setup(i => i.Dispose());

            var container = UtilityMethods.GetContainer();
            container.Register<ITestInterface>(item.Object);

            container.Dispose();

            item.VerifyAll();
        }
#endif

    [Fact]
    public void Resolve_RegisteredTypeWithFluentSingletonCall_ReturnsSingleton()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassNoInterfaceDefaultCtor>().AsSingleton();

        var result  = container.Resolve<TestClassNoInterfaceDefaultCtor>();
        var result2 = container.Resolve<TestClassNoInterfaceDefaultCtor>();

        Assert.Same(result, result2);
    }

    [Fact]
    public void Resolve_RegisteredTypeWithInterfaceWithFluentMultiInstanceCall_ReturnsMultipleInstances()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>().AsMultiInstance();

        var result  = container.Resolve<TestClassNoInterfaceDefaultCtor>();
        var result2 = container.Resolve<TestClassNoInterfaceDefaultCtor>();

        Assert.False(ReferenceEquals(result, result2));
    }

    [Fact]
    public void Resolve_RegisteredInstanceWithFluentMultiInstanceCall_ReturnsMultipleInstance()
    {
        var container = UtilityMethods.GetContainer();
        var input     = new TestClassDefaultCtor();
        container.Register(input).AsMultiInstance();

        var result = container.Resolve<TestClassDefaultCtor>();

        Assert.False(ReferenceEquals(result, input));
    }

    [Fact]
    public void Register_GenericTypeImplementationOnly_CanRegister()
    {
        var container = UtilityMethods.GetContainer();

        container.Register<GenericClassWithInterface<int, string>>();
    }

    [Fact]
    public void Register_GenericTypeWithInterface_CanRegister()
    {
        var container = UtilityMethods.GetContainer();

        container.Register<ITestInterface, GenericClassWithInterface<int, string>>();
    }

    [Fact]
    public void Resolve_RegisteredGenericTypeImplementationOnlyCorrectGenericTypes_Resolves()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<GenericClassWithInterface<int, string>>();

        var result = container.Resolve<GenericClassWithInterface<int, string>>();

        Assert.IsType<GenericClassWithInterface<int, string>>(result);
    }

    [Fact]
    public void Resolve_RegisteredGenericTypeWithInterfaceCorrectGenericTypes_Resolves()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, GenericClassWithInterface<int, string>>();

        var result = container.Resolve<ITestInterface>();

        Assert.IsType<GenericClassWithInterface<int, string>>(result);
    }

    [Fact]
    public void Resolve_RegisteredGenericTypeWithGenericInterfaceCorrectGenericTypes_Resolves()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface<int, string>, GenericClassWithGenericInterface<int, string>>();

        var result = container.Resolve<ITestInterface<int, string>>();

        Assert.IsType<GenericClassWithGenericInterface<int, string>>(result);
    }

    [Fact]
    public void Resolve_RegisteredGenericTypeWithGenericInterfaceCorrectGenericTypesWithParent_Resolves()
    {
        var container = UtilityMethods.GetContainer();
        var child     = container.GetChildContainer();
        container.Register<ITestInterface<int, string>>(new GenericClassWithGenericInterface<int, string>());

        var result = child.Resolve<ITestInterface<int, string>>();

        Assert.IsType<GenericClassWithGenericInterface<int, string>>(result);
    }


    [Fact]
    public void Register_NamedRegistration_CanRegister()
    {
        var container = UtilityMethods.GetContainer();

        container.Register<TestClassDefaultCtor>("TestName");
    }

    [Fact]
    public void Register_NamedInterfaceRegistration_CanRegister()
    {
        var container = UtilityMethods.GetContainer();

        container.Register<ITestInterface, TestClassDefaultCtor>("TestName");
    }

    [Fact]
    public void Register_NamedInstanceRegistration_CanRegister()
    {
        var container = UtilityMethods.GetContainer();
        var item      = new TestClassDefaultCtor();

        container.Register(item, "TestName");
    }

    [Fact]
    public void Register_NamedFactoryRegistration_CanRegister()
    {
        var container = UtilityMethods.GetContainer();

        container!.Register(
            (c, p) => (TestClassDefaultCtor)TestClassDefaultCtor.CreateNew(c),
            "TestName");
    }

    [Fact]
    public void Resolve_NamedRegistrationFollowedByNormal_CanResolveNamed()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>("TestName");
        container.Register<TestClassDefaultCtor>();

        var result = container.Resolve<TestClassDefaultCtor>("TestName");

        Assert.IsType<TestClassDefaultCtor>(result);
    }

    [Fact]
    public void Resolve_NormalRegistrationFollowedByNamed_CanResolveNormal()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassDefaultCtor>("TestName");

        var result = container.Resolve<TestClassDefaultCtor>();

        Assert.IsType<TestClassDefaultCtor>(result);
    }

    [Fact]
    public void Resolve_NamedInterfaceRegistrationFollowedByNormal_CanResolveNamed()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("TestName");
        container.Register<ITestInterface, TestClassDefaultCtor>();

        var result = container.Resolve<ITestInterface>("TestName");

        Assert.IsAssignableFrom<ITestInterface>(result);
    }

    [Fact]
    public void Resolve_NormalInterfaceRegistrationFollowedByNamed_CanResolveNormal()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        container.Register<ITestInterface, TestClassDefaultCtor>("TestName");

        var result = container.Resolve<TestClassDefaultCtor>();

        Assert.IsAssignableFrom<ITestInterface>(result);
    }

    [Fact]
    public void Resolve_NamedInstanceRegistrationFollowedByNormal_CanResolveNamed()
    {
        var container = UtilityMethods.GetContainer();
        var instance1 = new TestClassDefaultCtor();
        var instance2 = new TestClassDefaultCtor();
        container.Register(instance1, "TestName");
        container.Register(instance2);

        var result = container.Resolve<TestClassDefaultCtor>("TestName");

        Assert.Same(instance1, result);
    }

    [Fact]
    public void Resolve_NormalInstanceRegistrationFollowedByNamed_CanResolveNormal()
    {
        var container = UtilityMethods.GetContainer();
        var instance1 = new TestClassDefaultCtor();
        var instance2 = new TestClassDefaultCtor();
        container.Register(instance1);
        container.Register(instance2, "TestName");

        var result = container.Resolve<TestClassDefaultCtor>();

        Assert.Same(instance1, result);
    }


    [Fact]
    public void Resolve_NamedFactoryRegistrationFollowedByNormal_CanResolveNamed()
    {
        var container = UtilityMethods.GetContainer();
        var instance1 = new TestClassDefaultCtor();
        var instance2 = new TestClassDefaultCtor();
        container.Register((c, p) => instance1, "TestName");
        container.Register((c, p) => instance2);

        var result = container.Resolve<TestClassDefaultCtor>("TestName");

        Assert.Same(instance1, result);
    }

    [Fact]
    public void Resolve_FactoryInstanceRegistrationFollowedByNamed_CanResolveNormal()
    {
        var container = UtilityMethods.GetContainer();
        var instance1 = new TestClassDefaultCtor();
        var instance2 = new TestClassDefaultCtor();
        container.Register((c, p) => instance1);
        container.Register((c, p) => instance2, "TestName");

        var result = container.Resolve<TestClassDefaultCtor>();

        Assert.Same(instance1, result);
    }

    [Fact]
    public void Resolve_NoNameButOnlyNamedRegistered_ResolvesWithAttemptResolve()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>("Testing");

        var output = container.Resolve<TestClassDefaultCtor>(
            new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve });

        Assert.IsType<TestClassDefaultCtor>(output);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_NoNameButOnlyNamedRegistered_ThrowsExceptionWithNoAttemptResolve()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>("Testing");

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<TestClassDefaultCtor>(
                new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.Fail }));

    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_NamedButOnlyUnnamedRegistered_ThrowsExceptionWithNoFallback()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<TestClassDefaultCtor>(
                "Testing",
                new ResolveOptions() { NamedResolutionFailureAction = NamedResolutionFailureActions.Fail }));
    }

    [Fact]
    public void Resolve_NamedButOnlyUnnamedRegistered_ResolvesWithFallbackEnabled()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();

        var output = container.Resolve<TestClassDefaultCtor>(
            "Testing",
            new ResolveOptions()
            { NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution });

        Assert.IsType<TestClassDefaultCtor>(output);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_CorrectlyRegisteredSpecifyingMistypedParameters_ThrowsCorrectException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<TestClassWithParameters>(
                new NamedParameterOverloads { { "StringProperty", "Testing" }, { "IntProperty", 12 } }
            ));
    }

    [Fact]
    public void Resolve_CorrectlyRegisteredSpecifyingParameters_Resolves()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        var output = container.Resolve<TestClassWithParameters>(
            new NamedParameterOverloads { { "stringProperty", "Testing" }, { "intProperty", 12 } }
        );

        Assert.IsType<TestClassWithParameters>(output);
    }

    [Fact]
    public void Resolve_CorrectlyRegisteredSpecifyingParametersAndOptions_Resolves()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        var output = container.Resolve<TestClassWithParameters>(
            new NamedParameterOverloads { { "stringProperty", "Testing" }, { "intProperty", 12 } },
            ResolveOptions.Default
        );

        Assert.IsType<TestClassWithParameters>(output);
    }

    [Fact]
    public void CanResolve_UnRegisteredType_TrueWithAttemptResolve()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.CanResolve<TestClassDefaultCtor>(
            new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve });

        Assert.True(result);
    }

    [Fact]
    public void CanResolve_UnRegisteredType_FalseWithAttemptResolveOff()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.CanResolve<TestClassDefaultCtor>(
            new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.Fail });

        Assert.False(result);
    }

    [Fact]
    public void CanResolve_UnRegisteredTypeEithParameters_TrueWithAttemptResolve()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.CanResolve<TestClassWithParameters>(
            new NamedParameterOverloads { { "stringProperty", "Testing" }, { "intProperty", 12 } },
            new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve }
        );

        Assert.True(result);
    }

    [Fact]
    public void CanResolve_UnRegisteredTypeWithParameters_FalseWithAttemptResolveOff()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.CanResolve<TestClassWithParameters>(
            new NamedParameterOverloads { { "stringProperty", "Testing" }, { "intProperty", 12 } },
            new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.Fail }
        );

        Assert.False(result);
    }

    [Fact]
    public void CanResolve_NamedTypeAndNamedRegistered_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>("TestName");

        var result = container.CanResolve<TestClassDefaultCtor>("TestName");

        Assert.True(result);
    }

    [Fact]
    public void CanResolve_NamedTypeAndUnnamedRegistered_ReturnsTrueWithFallback()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();

        var result = container.CanResolve<TestClassDefaultCtor>(
            "TestName",
            new ResolveOptions()
            { NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution });

        Assert.True(result);
    }

    [Fact]
    public void CanResolve_NamedTypeAndUnnamedRegistered_ReturnsFalseWithFallbackOff()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();

        var result = container.CanResolve<TestClassDefaultCtor>(
            "TestName",
            new ResolveOptions() { NamedResolutionFailureAction = NamedResolutionFailureActions.Fail });

        Assert.False(result);
    }

    [Fact]
    public void CanResolve_NamedTypeWithParametersAndNamedRegistered_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>("TestName");

        var result = container.CanResolve<TestClassWithParameters>(
            "TestName",
            new NamedParameterOverloads { { "stringProperty", "Testing" }, { "intProperty", 12 } }
        );

        Assert.True(result);
    }

    [Fact]
    public void CanResolve_NamedTypeWithParametersAndUnnamedRegistered_ReturnsTrueWithFallback()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        var result = container.CanResolve<TestClassWithParameters>(
            "TestName",
            new NamedParameterOverloads { { "stringProperty", "Testing" }, { "intProperty", 12 } },
            new ResolveOptions()
            { NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution }
        );

        Assert.True(result);
    }

    [Fact]
    public void CanResolve_NamedTypeWithParametersAndUnnamedRegistered_ReturnsFalseWithFallbackOff()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        var result = container.CanResolve<TestClassWithParameters>(
            "TestName",
            new NamedParameterOverloads { { "stringProperty", "Testing" }, { "intProperty", 12 } },
            new ResolveOptions() { NamedResolutionFailureAction = NamedResolutionFailureActions.Fail }
        );

        Assert.False(result);
    }

    [Fact]
    public void Resolve_RegisteredTypeWithNameParametersAndOptions_Resolves()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>("TestName");

        var result = container.Resolve<TestClassWithParameters>(
            "TestName",
            new NamedParameterOverloads { { "stringProperty", "Testing" }, { "intProperty", 12 } },
            new ResolveOptions() { NamedResolutionFailureAction = NamedResolutionFailureActions.Fail }
        );

        Assert.IsType<TestClassWithParameters>(result);
    }

    [Fact]
    public void Resolve_RegisteredTypeWithNameAndParameters_Resolves()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>("TestName");

        var result = container.Resolve<TestClassWithParameters>(
            "TestName",
            new NamedParameterOverloads { { "stringProperty", "Testing" }, { "intProperty", 12 } }
        );

        Assert.IsType<TestClassWithParameters>(result);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_ClassWithOnlyPrivateConstructor_ThrowsCorrectException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassPrivateCtor>();

        Assert.Throws<DICResolutionException>(() => container.Resolve<TestClassPrivateCtor>());

        //Assert.IsType<>(result, typeof(TestClassPrivateCtor));
    }

    [Fact]
    public void Resolve_ClassWithOnlyProtectedConstructor_ThrowsCorrectException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassProtectedCtor>();

        Assert.Throws<DICResolutionException>(() => container.Resolve<TestClassPrivateCtor>());
    }

    [Fact]
    public void CanResolve_ClassWithOnlyProtectedConstructor_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassProtectedCtor>();

        bool canResolve = container.CanResolve<TestClassProtectedCtor>();
        Assert.False(canResolve);
    }

    [Fact]
    public void Resolve_SubclassOfClassWithOnlyProtectedConstructor_Succeeds()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestSubclassProtectedCtor>();
    }

    [Fact]
    public void CanResolve_SubclassOfClassWithOnlyProtectedConstructor_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassProtectedCtor>();

        bool canResolve = container.CanResolve<TestSubclassProtectedCtor>();
        Assert.True(canResolve);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_RegisteredSingletonWithParameters_ThrowsCorrectException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<ITestInterface>(
                new NamedParameterOverloads { { "stringProperty", "Testing" }, { "intProperty", 12 } }));

        //Assert.IsType<>(output, typeof(ITestInterface));
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_WithNullParameters_ThrowsCorrectException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        NamedParameterOverloads? parameters = null;

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<TestClassDefaultCtor>(parameters!));

        //Assert.IsType<>(output, typeof(TestClassDefaultCtor));
    }


    [Fact]
    public void Register_MultiInstanceToSingletonFluent_Registers()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>().AsSingleton();
    }

    [Fact]
    public void Register_MultiInstanceToMultiInstance_Registers()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>().AsMultiInstance();
    }

    [Fact]
    //[ExpectedException(typeof(DICRegistrationException))]
    public void Register_MultiInstanceWithStrongReference_Throws()
    {
        var container = UtilityMethods.GetContainer();
        Assert.Throws<DICRegistrationException>(
            () => container.Register<TestClassDefaultCtor>().WithStrongReference());
    }

    [Fact]
    //[ExpectedException(typeof(DICRegistrationException))]
    public void Register_MultiInstanceWithWeakReference_Throws()
    {
        var container = UtilityMethods.GetContainer();
        Assert.Throws<DICRegistrationException>(
            () => container.Register<TestClassDefaultCtor>().WithWeakReference());

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    public void Register_SingletonToSingletonFluent_Registers()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>().AsSingleton();
    }

    [Fact]
    public void Register_SingletonToMultiInstanceFluent_Registers()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>().AsMultiInstance();
    }

    [Fact]
    //[ExpectedException(typeof(DICRegistrationException))]
    public void Register_SingletonWithStrongReference_Throws()
    {
        var container = UtilityMethods.GetContainer();
        Assert.Throws<DICRegistrationException>(
            () => container.Register<ITestInterface, TestClassDefaultCtor>().WithStrongReference());

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    //[ExpectedException(typeof(DICRegistrationException))]
    public void Register_SingletonWithWeakReference_Throws()
    {
        var container = UtilityMethods.GetContainer();
        Assert.Throws<DICRegistrationException>(
            () => container.Register<ITestInterface, TestClassDefaultCtor>().WithWeakReference());

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    //[ExpectedException(typeof(DICRegistrationException))]
    public void Register_FactoryToMultiInstanceFluent_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();
        Assert.Throws<DICRegistrationException>(
            () => container.Register((c, p) => new TestClassDefaultCtor()).AsMultiInstance());

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    public void Register_FactoryWithStrongReference_Registers()
    {
        var container = UtilityMethods.GetContainer();
        container.Register((c, p) => new TestClassDefaultCtor()).WithStrongReference();
    }

    [Fact]
    public void Register_FactoryWithWeakReference_Registers()
    {
        var container = UtilityMethods.GetContainer();
        container.Register((c, p) => new TestClassDefaultCtor()).WithWeakReference();
    }

    [Fact]
    //[ExpectedException(typeof(DICRegistrationException))]
    public void Register_InstanceToSingletonFluent_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();
        Assert.Throws<DICRegistrationException>(
            () => container.Register(new TestClassDefaultCtor()).AsSingleton());
    }

    [Fact]
    public void Register_InstanceToMultiInstance_Registers()
    {
        var container = UtilityMethods.GetContainer();
        container.Register(new TestClassDefaultCtor()).AsMultiInstance();
    }

    [Fact]
    public void Register_InstanceWithStrongReference_Registers()
    {
        var container = UtilityMethods.GetContainer();
        container.Register(new TestClassDefaultCtor()).WithStrongReference();
    }

    [Fact]
    public void Register_InstanceWithWeakReference_Registers()
    {
        var container = UtilityMethods.GetContainer();
        container.Register(new TestClassDefaultCtor()).WithWeakReference();
    }

    // @mbrit - 2012-05-22 - forced GC not supported in WinRT...
#if !NETFX_CORE
    [Fact]
    public void Resolve_OutOfScopeStrongReferencedInstance_ResolvesCorrectly()
    {
        var container = UtilityMethods.GetContainer();
        UtilityMethods.RegisterInstanceStrongRef(container);

        GC.Collect();
        GC.WaitForFullGCComplete(4000);

        var result = container.Resolve<TestClassDefaultCtor>();
        Assert.Equal("Testing", result.Prop1);
    }
#endif

    // @mbrit - 2012-05-22 - forced GC not supported in WinRT...
#if !NETFX_CORE
    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_OutOfScopeWeakReferencedInstance_ThrowsCorrectException()
    {
        var container = UtilityMethods.GetContainer();
        UtilityMethods.RegisterInstanceWeakRef(container);

        GC.Collect();
        GC.WaitForFullGCComplete(4000);

        Assert.Throws<DICResolutionException>(() =>
        {
            var result = container.Resolve<TestClassDefaultCtor>();
            Assert.Equal("Testing", result.Prop1);
        });
    }
#endif

    // @mbrit - 2012-05-22 - forced GC not supported in WinRT...
#if !NETFX_CORE
    [Fact]
    public void Resolve_OutOfScopeStrongReferencedFactory_ResolvesCorrectly()
    {
        var container = UtilityMethods.GetContainer();
        UtilityMethods.RegisterFactoryStrongRef(container);

        GC.Collect();
        GC.WaitForFullGCComplete(4000);

        var result = container.Resolve<TestClassDefaultCtor>();
        Assert.Equal("Testing", result.Prop1);
    }
#endif

    // @mbrit - 2012-05-22 - forced GC not supported in WinRT...
#if !NETFX_CORE
    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_OutOfScopeWeakReferencedFactory_ThrowsCorrectException()
    {
        var container = UtilityMethods.GetContainer();
        UtilityMethods.RegisterFactoryWeakRef(container);

        GC.Collect();
        GC.WaitForFullGCComplete(4000);

        Assert.Throws<DICResolutionException>(() =>
        {
            var result = container.Resolve<TestClassDefaultCtor>();
            Assert.Equal("Testing", result.Prop1);
        });
    }
#endif

    [Fact]
    public void Register_InterfaceAndImplementationWithInstance_Registers()
    {
        var container = UtilityMethods.GetContainer();
        var item      = new TestClassDefaultCtor();
        container.Register<ITestInterface, TestClassDefaultCtor>(item);
    }

    [Fact]
    public void Register_InterfaceAndImplementationNamedWithInstance_Registers()
    {
        var container = UtilityMethods.GetContainer();
        var item      = new TestClassDefaultCtor();
        var item2     = new TestClassDefaultCtor();
        container.Register<ITestInterface, TestClassDefaultCtor>(item, "TestName");
        container.Register<ITestInterface, TestClassDefaultCtor>(item2);
    }

    [Fact]
    public void Resolved_InterfaceAndImplementationWithInstance_ReturnsCorrectInstance()
    {
        var container = UtilityMethods.GetContainer();
        var item      = new TestClassDefaultCtor();
        container.Register<ITestInterface, TestClassDefaultCtor>(item);

        var result = container.Resolve<ITestInterface>();

        Assert.Same(item, result);
    }

    [Fact]
    public void Resolve_InterfaceAndImplementationNamedWithInstance_ReturnsCorrectInstance()
    {
        var container = UtilityMethods.GetContainer();
        var item      = new TestClassDefaultCtor();
        var item2     = new TestClassDefaultCtor();
        container.Register<ITestInterface, TestClassDefaultCtor>(item, "TestName");
        container.Register<ITestInterface, TestClassDefaultCtor>(item2);

        var result = container.Resolve<ITestInterface>("TestName");

        Assert.Same(item, result);
    }

    [Fact]
    public void Resolve_BoundGenericTypeWithoutRegistered_ResolvesWithDefaultOptions()
    {
        var container = UtilityMethods.GetContainer();

        var testing = container.Resolve<GenericClassWithInterface<int, string>>();

        Assert.IsType<GenericClassWithInterface<int, string>>(testing);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_BoundGenericTypeWithoutRegistered_FailsWithUnRegisteredFallbackOff()
    {
        var container = UtilityMethods.GetContainer();

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<GenericClassWithInterface<int, string>>(
                new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.Fail }));
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_NormalUnregisteredType_FailsWithUnregisteredFallbackSetToGenericsOnly()
    {
        var container = UtilityMethods.GetContainer();

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<TestClassDefaultCtor>(
                new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.GenericsOnly }));
    }

    [Fact]
    public void Resolve_BoundGenericTypeWithoutRegistered_ResolvesWithUnRegisteredFallbackSetToGenericsOnly()
    {
        var container = UtilityMethods.GetContainer();

        var testing = container.Resolve<GenericClassWithInterface<int, string>>(
            new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.GenericsOnly });

        Assert.IsType<GenericClassWithInterface<int, string>>(testing);
    }

    [Fact]
    public void CanResolve_BoundGenericTypeWithoutRegistered_ReturnsTrueWithDefaultOptions()
    {
        var container = UtilityMethods.GetContainer();

        var testing = container.CanResolve<GenericClassWithInterface<int, string>>();

        Assert.True(testing);
    }

    [Fact]
    public void CanResolve_BoundGenericTypeWithoutRegistered_ReturnsFalseWithUnRegisteredFallbackOff()
    {
        var container = UtilityMethods.GetContainer();

        var testing = container.CanResolve<GenericClassWithInterface<int, string>>(
            new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.Fail });

        Assert.False(testing);
    }

    [Fact]
    public void CanResolve_BoundGenericTypeWithoutRegistered_ReturnsTrueWithUnRegisteredFallbackSetToGenericsOnly()
    {
        var container = UtilityMethods.GetContainer();

        var testing = container.CanResolve<GenericClassWithInterface<int, string>>(
            new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.GenericsOnly });

        Assert.True(testing);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_UnRegisteredNonGenericType_FailsWithOptionsSetToGenericOnly()
    {
        var container = UtilityMethods.GetContainer();

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<TestClassDefaultCtor>(
                new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.GenericsOnly }));

        //assert.IsInstanceOfType(result, typeof(TestClassDefaultCtor));
    }

    [Fact]
    public void CanResolve_UnRegisteredNonGenericType_ReturnsFalseWithOptionsSetToGenericOnly()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.CanResolve<TestClassDefaultCtor>(
            new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.GenericsOnly });

        Assert.False(result);
    }

    [Fact]
    public void Resolve_BoundGenericTypeWithParametersWithoutRegistered_ResolvesUsingCorrectCtor()
    {
        var container = UtilityMethods.GetContainer();

        var testing = container.Resolve<GenericClassWithInterface<int, string>>(
            new NamedParameterOverloads() { { "prop1", 27 }, { "prop2", "Testing" } });

        Assert.Equal(27, testing.Prop1);
        Assert.Equal("Testing", testing.Prop2);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_BoundGenericTypeWithFailedDependenciesWithoutRegistered_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<GenericClassWithParametersAndDependencies<int, string>>());

        //Assert.IsType<>(testing, typeof(GenericClassWithParametersAndDependencies<int, string>));
    }

    [Fact]
    public void Resolve_BoundGenericTypeWithDependenciesWithoutRegistered_ResolvesUsingCorrectCtor()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface2, TestClass2>();

        var testing = container.Resolve<GenericClassWithParametersAndDependencies<int, string>>();

        Assert.IsType<GenericClassWithParametersAndDependencies<int, string>>(testing);
    }

    [Fact]
    public void Resolve_BoundGenericTypeWithDependenciesAndParametersWithoutRegistered_ResolvesUsingCorrectCtor()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface2, TestClass2>();

        var testing = container.Resolve<GenericClassWithParametersAndDependencies<int, string>>(
            new NamedParameterOverloads() { { "prop1", 27 }, { "prop2", "Testing" } });

        Assert.Equal(27, testing.Prop1);
        Assert.Equal("Testing", testing.Prop2);
    }

    [Fact]
    public void
        Resolve_NamedRegistrationButOnlyUnnamedRegistered_ResolvesCorrectUnnamedRegistrationWithUnnamedFallback()
    {
        var container = UtilityMethods.GetContainer();
        var item      = new TestClassDefaultCtor() { Prop1 = "Testing" };
        container.Register(item);

        var result = container.Resolve<TestClassDefaultCtor>(
            "Testing",
            new ResolveOptions()
            { NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution });

        Assert.Same(item, result);
    }

    [Fact]
    public void Resolve_ClassWithLazyFactoryDependency_Resolves()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.Resolve<TestClassWithLazyFactory>();

        Assert.IsType<TestClassWithLazyFactory>(result);
    }

    [Fact]
    public void CanResolve_ClassWithLazyFactoryDependency_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.CanResolve<TestClassWithLazyFactory>();

        Assert.True(result);
    }

    [Fact]
    public void Resolve_ClassWithNamedLazyFactoryDependency_Resolves()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.Resolve<TestClassWithNamedLazyFactory>();

        Assert.IsType<TestClassWithNamedLazyFactory>(result);
    }

    [Fact]
    public void CanResolve_ClassNamedWithLazyFactoryDependency_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.CanResolve<TestClassWithNamedLazyFactory>();

        Assert.True(result);
    }

    [Fact]
    public void LazyFactory_CalledByDependantClass_ReturnsInstanceOfType()
    {
        var container = UtilityMethods.GetContainer();
        var item      = container.Resolve<TestClassWithLazyFactory>();

        item.Method1();

        Assert.IsType<TestClassDefaultCtor>(item.Prop1);
    }

    [Fact]
    public void NamedLazyFactory_CalledByDependantClass_ReturnsCorrectInstanceOfType()
    {
        var container = UtilityMethods.GetContainer();
        var item1     = new TestClassDefaultCtor();
        var item2     = new TestClassDefaultCtor();
        container.Register(item1, "Testing");
        container.Register(item2);
        container.Register<TestClassWithNamedLazyFactory>();

        var item = container.Resolve<TestClassWithNamedLazyFactory>();

        item.Method1();
        item.Method2();

        Assert.Same(item.Prop1, item1);
        Assert.Same(item.Prop2, item2);
    }

    [Fact]
    public void AutoRegister_NoParameters_ReturnsNoErrors()
    {
        var container = UtilityMethods.GetContainer();
        container.AutoRegister();
    }

    [Fact]
    public void AutoRegister_AssemblySpecified_ReturnsNoErrors()
    {
        var container = UtilityMethods.GetContainer();

        container.AutoRegister(new[] { GetType().Assembly });
    }

    [Fact]
    public void AutoRegister_TestAssembly_CanResolveInterface()
    {
        var container = UtilityMethods.GetContainer();
        container.AutoRegister(new[] { GetType().Assembly });

        var result = container.Resolve<ITestInterface>();

        Assert.IsAssignableFrom<ITestInterface>(result);
    }

    [Fact]
    public void AutoRegister_TestAssembly_CanResolveAbstractBaseClass()
    {
        var container = UtilityMethods.GetContainer();
        container.AutoRegister(new[] { GetType().Assembly });

        var result = container.Resolve<TestClassBase>();

        Assert.IsAssignableFrom<TestClassBase>(result);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void AutoRegister_TinyIoCAssembly_CannotResolveInternalTinyIoCClass()
    {
        var container = UtilityMethods.GetContainer();
        container.AutoRegister(new[] { container.GetType().Assembly });

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<DependencyInjectionContainer.TypeRegistration>(
                new NamedParameterOverloads() { { "type", GetType() } },
                new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.Fail }));

        //Assert.IsType<>(output, typeof(DependencyInjectionContainer.TypeRegistration));
    }

    [Fact]
    //[ExpectedException(typeof(DICAutoRegistrationException))]
    public void AutoRegister_ThisAssemblySpecifiedDuplicateActionFail_ThrowsException()
    {
        Assert.Throws<DICAutoRegistrationException>(() =>
        {
            var container = UtilityMethods.GetContainer();
            container.AutoRegister(new[] { GetType().Assembly }, DuplicateImplementationActions.Fail);
            //Assert.True(false);
        });
    }

    [Fact(Skip = "AsciiApp Assembly contains classes that break this approach")]
    public void AutoRegister_TinyIoCAssemblySpecifiedDuplicateActionFail_NoErrors()
    {
        var container = UtilityMethods.GetContainer();
        container.AutoRegister(
            new[] { typeof(DependencyInjectionContainer).Assembly },
            DuplicateImplementationActions.Fail);
    }

    [Fact]
    public void AutoRegister_SpecifiedDuplicateActionRegisterMultiple_RegistersMultipleImplementations()
    {
        var container = UtilityMethods.GetContainer();
        container.AutoRegister(
            new[] { typeof(TestClassDefaultCtor).Assembly },
            DuplicateImplementationActions.RegisterMultiple);

        var results = container.ResolveAll<ITestInterface>();

        Assert.IsType<TestClassDefaultCtor>(results.First());
    }

    [Fact]
    public void Register_ConstructorSpecifiedForDelegateFactory_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();

        Assert.Throws<DICConstructorResolutionException>(
            () => container.Register((c, p) => new TestClassDefaultCtor())
                           .UsingConstructor(() => new TestClassDefaultCtor()));

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    //[ExpectedException(typeof(DICConstructorResolutionException))]
    public void Register_ConstructorSpecifiedForWeakDelegateFactory_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();

        Assert.Throws<DICConstructorResolutionException>(
            () => container.Register((c, p) => new TestClassDefaultCtor())
                           .WithWeakReference()
                           .UsingConstructor(() => new TestClassDefaultCtor()));

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    //[ExpectedException(typeof(DICConstructorResolutionException))]
    public void Register_ConstructorSpecifiedForInstanceFactory_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();

        Assert.Throws<DICConstructorResolutionException>(
            () => container.Register(new TestClassDefaultCtor())
                           .UsingConstructor(() => new TestClassDefaultCtor()));

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    //[ExpectedException(typeof(DICConstructorResolutionException))]
    public void Register_ConstructorSpecifiedForWeakInstanceFactory_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();

        Assert.Throws<DICConstructorResolutionException>(
            () => container.Register(new TestClassDefaultCtor())
                           .WithWeakReference()
                           .UsingConstructor(() => new TestClassDefaultCtor()));

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    public void Register_ConstructorSpecifiedForMultiInstanceFactory_Registers()
    {
        var container = UtilityMethods.GetContainer();

        container.Register<TestClassDefaultCtor>().UsingConstructor(() => new TestClassDefaultCtor());
    }

    [Fact]
    public void Register_ConstructorSpecifiedForSingletonFactory_Registers()
    {
        var container = UtilityMethods.GetContainer();

        container.Register<ITestInterface, TestClassDefaultCtor>().UsingConstructor(() => new TestClassDefaultCtor());
    }

    [Fact]
    public void Resolve_SingletonFactoryConstructorSpecified_UsesCorrectCtor()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassMultiDepsMultiCtors>()
                 .AsSingleton()
                 .UsingConstructor(() => new TestClassMultiDepsMultiCtors(null!));

        var result = container.Resolve<TestClassMultiDepsMultiCtors>();

        Assert.Equal(1, result.NumberOfDepsResolved);
    }

    [Fact]
    public void Resolve_MultiInstanceFactoryConstructorSpecified_UsesCorrectCtor()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassMultiDepsMultiCtors>()
                 .UsingConstructor(() => new TestClassMultiDepsMultiCtors(null!));

        var result = container.Resolve<TestClassMultiDepsMultiCtors>();

        Assert.Equal(1, result.NumberOfDepsResolved);
    }

    [Fact]
    public void Resolve_SingletonFactoryNoConstructorSpecified_UsesCorrectCtor()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassMultiDepsMultiCtors>().AsSingleton();

        var result = container.Resolve<TestClassMultiDepsMultiCtors>();

        Assert.Equal(2, result.NumberOfDepsResolved);
    }

    [Fact]
    public void Resolve_MultiInstanceFactoryNoConstructorSpecified_UsesCorrectCtor()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassMultiDepsMultiCtors>();

        var result = container.Resolve<TestClassMultiDepsMultiCtors>();

        Assert.Equal(2, result.NumberOfDepsResolved);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_ConstructorSpecifiedThatRequiresParametersButNonePassed_FailsToResolve()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        container.Register<TestClassWithInterfaceDependency>()
                 .UsingConstructor(() => new TestClassWithInterfaceDependency(null!, 27, "Testing"));

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<TestClassWithInterfaceDependency>());

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    public void Resolve_ConstructorSpecifiedWithAttribute_UsesCorrectCtor()
    {
        var container = UtilityMethods.GetContainer();
        var result    = container.Resolve<TestClassWithConstructorAttrib>();

        Assert.True(result.AttributeConstructorUsed);
    }

    [Fact]
    public void Resolve_InternalConstructorSpecifiedWithAttribute_UsesCorrectCtor()
    {
        var container = UtilityMethods.GetContainer();
        var result    = container.Resolve<TestClassWithInternalConstructorAttrib>();

        Assert.True(result.AttributeConstructorUsed);
    }

    [Fact]
    public void Resolve_ManyConstructorsSpecifiedWithAttribute_UsesCorrectCtor()
    {
        var container = UtilityMethods.GetContainer();
        var result    = container.Resolve<TestClassWithManyConstructorAttribs>();

        Assert.True(result.MostGreedyAttribCtorUsed);
    }

    [Fact]
    public void Resolve_AttributeConstructorOverriden_UsesCorrectCtor()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithConstructorAttrib>()
                 .UsingConstructor(() => new TestClassWithConstructorAttrib(null!)); // The non-attribute constructor

        var result = container.Resolve<TestClassWithConstructorAttrib>();

        Assert.False(result.AttributeConstructorUsed);
    }

    [Fact]
    public void CanResolve_ConstructorSpecifiedThatRequiresParametersButNonePassed_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        container.Register<TestClassWithInterfaceDependency>()
                 .UsingConstructor(() => new TestClassWithInterfaceDependency(null!, 27, "Testing"));

        var result = container.CanResolve<TestClassWithInterfaceDependency>();

        Assert.False(result);
    }

    [Fact]
    public void CanResolve_SingletonFactoryConstructorSpecified_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassMultiDepsMultiCtors>()
                 .AsSingleton()
                 .UsingConstructor(() => new TestClassMultiDepsMultiCtors(null!));

        var result = container.CanResolve<TestClassMultiDepsMultiCtors>();

        Assert.True(result);
    }

    [Fact]
    public void CanResolve_MultiInstanceFactoryConstructorSpecified_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();
        container.Register<TestClassMultiDepsMultiCtors>()
                 .UsingConstructor(() => new TestClassMultiDepsMultiCtors(null!));

        var result = container.CanResolve<TestClassMultiDepsMultiCtors>();

        Assert.True(result);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_ConstructorThrowsException_ThrowsTinyIoCException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassConstructorFailure>();

        Assert.Throws<DICResolutionException>(() => container.Resolve<TestClassConstructorFailure>());

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_DelegateFactoryThrowsException_ThrowsTinyIoCException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassConstructorFailure>((c, p) => { throw new NotImplementedException(); });

        Assert.Throws<DICResolutionException>(() => container.Resolve<TestClassConstructorFailure>());

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    //[ExpectedException(typeof(DICResolutionException))]
    public void Resolve_DelegateFactoryResolvedWithUnnamedFallbackThrowsException_ThrowsTinyIoCException()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassConstructorFailure>((c, p) => { throw new NotImplementedException(); });

        Assert.Throws<DICResolutionException>(
            () => container.Resolve<TestClassConstructorFailure>(
                "Testing",
                new ResolveOptions()
                { NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution }));

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    //[ExpectedException(typeof(DICCRegistrationTypeException))]
    public void Register_AbstractClassWithNoImplementation_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();

        Assert.Throws<DICRegistrationTypeException>(() => container.Register<TestClassBase>());

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    //[ExpectedException(typeof(DICCRegistrationTypeException))]
    public void Register_InterfaceWithNoImplementation_ThrowsException()
    {
        var container = UtilityMethods.GetContainer();

        Assert.Throws<DICRegistrationTypeException>(() => container.Register<ITestInterface>());

        // Should have thrown by now
        //Assert.True(false);
    }

    [Fact]
    public void BuildUp_ObjectWithPropertyDependenciesAndDepsRegistered_SetsDependencies()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        container.Register<ITestInterface2, TestClass2>();
        var input = new TestClassPropertyDependencies();

        container.BuildUp(input);

        Assert.NotNull(input.Property1);
        Assert.NotNull(input.Property2);
        Assert.NotNull(input.ConcreteProperty);
    }

    [Fact]
    public void BuildUp_ObjectAndOptionsWithPropertyDependenciesAndDepsRegistered_SetsDependencies()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        container.Register<ITestInterface2, TestClass2>();
        var input = new TestClassPropertyDependencies();

        container.BuildUp(input, new ResolveOptions());

        Assert.NotNull(input.Property1);
        Assert.NotNull(input.Property2);
        Assert.NotNull(input.ConcreteProperty);
    }

    [Fact]
    public void BuildUp_ObjectAndOptionsWithPropertyDependenciesAndDepsRegistered_SetsDependenciesUsingOptions()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        container.Register<ITestInterface2, TestClass2>();
        var input = new TestClassPropertyDependencies();

        container.BuildUp(
            input,
            new ResolveOptions() { UnregisteredResolutionAction = UnregisteredResolutionActions.Fail });

        Assert.NotNull(input.Property1);
        Assert.NotNull(input.Property2);
        Assert.Null(input.ConcreteProperty);
    }

    [Fact]
    public void BuildUp_ObjectWithPropertyDependenciesAndDepsNotRegistered_DoesNotThrow()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        var input = new TestClassPropertyDependencies();

        container.BuildUp(input);

        Assert.NotNull(input.Property1);
        Assert.Null(input.Property2);
    }

    [Fact]
    public void BuildUp_ObjectWithPropertyDependenciesWithSomeSet_SetsOnlyUnsetDependencies()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        container.Register<ITestInterface2, TestClass2>();
        var preset = new TestClassDefaultCtor();
        var input  = new TestClassPropertyDependencies();
        input.Property1 = preset;

        container.BuildUp(input);

        Assert.Same(preset, input.Property1);
        Assert.NotNull(input.Property2);
    }

    [Fact]
    public void BuildUp_ObjectWithPropertyDependenciesAndDepsRegistered_DoesNotSetWriteOnlyProperty()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        container.Register<ITestInterface2, TestClass2>();
        var input = new TestClassPropertyDependencies();

        container.BuildUp(input);

        Assert.Null(input.WriteOnlyProperty);
    }

    [Fact]
    public void Resolve_ClassWithNameAndParamsLazyFactoryDependency_Resolves()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.Resolve<TestClassWithNameAndParamsLazyFactory>();

        Assert.IsType<TestClassWithNameAndParamsLazyFactory>(result);
    }

    [Fact]
    public void CanResolve_ClassWithNameAndParamsLazyFactoryDependency_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.CanResolve<TestClassWithNameAndParamsLazyFactory>();

        Assert.True(result);
    }

    [Fact]
    public void NameAndParamsLazyFactoryInvoke_Params_ResolvesWithParameters()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.Resolve<TestClassWithNameAndParamsLazyFactory>();

        // Values should be set by the ctor of TestClassWithNameAndParamsLazyFactory
        Assert.Equal("Testing", result!.Prop1!.StringProperty);
        Assert.Equal(22, result.Prop1.IntProperty);
    }

    [Fact]
    public void NamedParameterOverloads_ConstructedUsingFromIDictionary_CopiesDictionary()
    {
        var dictionary = new Dictionary<string, object>() { { "Test", "Test" } };

        var output = NamedParameterOverloads.FromIDictionary(dictionary);

        Assert.NotNull(output);
        Assert.Equal("Test", output["Test"]);
    }

    [Fact]
    public void AutoRegister_IEnumerableAssemblies_DoesNotThrow()
    {
        var container = UtilityMethods.GetContainer();
        List<Assembly> assemblies = new List<Assembly>()
                                    { GetType().Assembly, typeof(IExternalTestInterface).Assembly };

        container.AutoRegister(assemblies);
    }

    [Fact]
    public void AutoRegister_IEnumerableAssemblies_TypesFromBothAssembliesResolve()
    {
        var container = UtilityMethods.GetContainer();
        List<Assembly> assemblies = new List<Assembly>()
                                    { GetType().Assembly, typeof(IExternalTestInterface).Assembly };

        container.AutoRegister(assemblies);

        var result1 = container.Resolve<ITestInterface>();
        var result2 = container.Resolve<IExternalTestInterface>();

        Assert.IsAssignableFrom<ITestInterface>(result1);
        Assert.IsAssignableFrom<IExternalTestInterface>(result2);
    }

#if APPDOMAIN_GETASSEMBLIES
        [Fact]
        public void AutoRegister_NoParameters_TypesFromDifferentAssembliesInAppDomainResolve()
        {
            var container = UtilityMethods.GetContainer();
            container.AutoRegister();

            var result1 = container.Resolve<ITestInterface>();
            var result2 = container.Resolve<ExternalTypes.IExternalTestInterface>();

            Assert.IsType<>(result1, typeof(ITestInterface));
            Assert.IsType<>(result2, typeof(ExternalTypes.IExternalTestInterface));
        }
#endif

    [Fact]
    public void GetChildContainer_NoParameters_ReturnsContainerInstance()
    {
        var container = UtilityMethods.GetContainer();

        var child = container.GetChildContainer();

        Assert.IsType<DependencyInjectionContainer>(child);
    }

    [Fact]
    public void GetChildContainer_NoParameters_ContainerReturnedIsNewContainer()
    {
        var container = UtilityMethods.GetContainer();

        var child = container.GetChildContainer();

        Assert.False(ReferenceEquals(child, container));
    }

    [Fact]
    public void ChildContainerResolve_TypeRegisteredWithParent_ResolvesType()
    {
        var container = UtilityMethods.GetContainer();
        var child     = container.GetChildContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        var result = child.Resolve<ITestInterface>();

        Assert.IsType<TestClassDefaultCtor>(result);
    }

    [Fact]
    public void ChildContainerCanResolve_TypeRegisteredWithParent_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        var child     = container.GetChildContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        var result = child.CanResolve<ITestInterface>();

        Assert.True(result);
    }

    [Fact]
    public void ChildContainerResolve_TypeRegisteredWithChild_ResolvesType()
    {
        var container = UtilityMethods.GetContainer();
        var child     = container.GetChildContainer();
        child.Register<ITestInterface, TestClassDefaultCtor>();

        var result = child.Resolve<ITestInterface>();

        Assert.IsType<TestClassDefaultCtor>(result);
    }

    [Fact]
    public void ChildContainerCanResolve_TypeRegisteredWithChild_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        var child     = container.GetChildContainer();
        child.Register<ITestInterface, TestClassDefaultCtor>();

        var result = child.CanResolve<ITestInterface>();

        Assert.True(result);
    }

    [Fact]
    public void ChildContainerResolve_TypeRegisteredWithParentAndChild_ResolvesChildVersion()
    {
        var container         = UtilityMethods.GetContainer();
        var containerInstance = new TestClassDefaultCtor();
        var child             = container.GetChildContainer();
        var childInstance     = new TestClassDefaultCtor();
        container.Register<ITestInterface>(containerInstance);
        child.Register<ITestInterface>(childInstance);

        var result = child.Resolve<ITestInterface>();

        Assert.Same(result, childInstance);
    }

    [Fact]
    public void ChildContainerResolve_NamedOnlyRegisteredWithParent_ResolvesFromParent()
    {
        var container         = UtilityMethods.GetContainer();
        var containerInstance = new TestClassDefaultCtor();
        var child             = container.GetChildContainer();
        var childInstance     = new TestClassDefaultCtor();
        container.Register<ITestInterface>(containerInstance, "Testing");
        child.Register<ITestInterface>(childInstance);

        var result = child.Resolve<ITestInterface>("Testing");

        Assert.Same(result, containerInstance);
    }

    [Fact]
    public void ChildContainerCanResolve_NamedOnlyRegisteredWithParent_ReturnsTrue()
    {
        var container         = UtilityMethods.GetContainer();
        var containerInstance = new TestClassDefaultCtor();
        var child             = container.GetChildContainer();
        var childInstance     = new TestClassDefaultCtor();
        container.Register<ITestInterface>(containerInstance, "Testing");
        child.Register<ITestInterface>(childInstance);

        var result = child.CanResolve<ITestInterface>("Testing");

        Assert.True(result);
    }

    [Fact]
    public void ChildContainerResolve_NamedOnlyRegisteredWithParentUnnamedFallbackOn_ResolvesFromChild()
    {
        var container         = UtilityMethods.GetContainer();
        var containerInstance = new TestClassDefaultCtor();
        var child             = container.GetChildContainer();
        var childInstance     = new TestClassDefaultCtor();
        container.Register<ITestInterface>(containerInstance, "Testing");
        child.Register<ITestInterface>(childInstance);

        var result = child.Resolve<ITestInterface>(
            new ResolveOptions()
            { NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution });

        Assert.Same(result, childInstance);
    }

    [Fact]
    public void
        ChildContainerResolve_NamedOnlyRegisteredWithParentChildNoRegistrationUnnamedFallbackOn_ResolvesFromParent()
    {
        var container         = UtilityMethods.GetContainer();
        var containerInstance = new TestClassDefaultCtor();
        var child             = container.GetChildContainer();
        var childInstance     = new TestClassDefaultCtor();
        container.Register<ITestInterface>(containerInstance, "Testing");
        child.Register<ITestInterface>(childInstance);

        var result = child.Resolve<ITestInterface>(
            "Testing",
            new ResolveOptions()
            { NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution });

        Assert.Same(result, containerInstance);
    }

    [Fact]
    public void TryResolve_ValidResolve_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        var result = container.TryResolve<ITestInterface>(out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolve_ValidResolve_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        container.TryResolve<ITestInterface>(out var output);

        Assert.IsAssignableFrom<ITestInterface>(output);
    }

    [Fact]
    public void TryResolve_InvalidResolve_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve<ITestInterface>(out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithOptions_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        var result = container.TryResolve<ITestInterface>(new ResolveOptions(), out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithOptions_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        var result = container.TryResolve<ITestInterface>(new ResolveOptions(), out var output);

        Assert.IsAssignableFrom<ITestInterface>(output);
    }

    [Fact]
    public void TryResolve_InvalidResolveWithOptions_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve<ITestInterface>(new ResolveOptions(), out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithName_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("Testing");

        var result = container.TryResolve<ITestInterface>("Testing", out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithName_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("Testing");

        container.TryResolve<ITestInterface>("Testing", out var output);

        Assert.IsAssignableFrom<ITestInterface>(output);
    }

    [Fact]
    public void TryResolve_InvalidResolveWithName_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve<ITestInterface>("Testing", out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithNameAndOptions_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("Testing");

        var result = container.TryResolve<ITestInterface>("Testing", new ResolveOptions(), out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithNameAndOptions_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("Testing");

        container.TryResolve<ITestInterface>("Testing", new ResolveOptions(), out var output);

        Assert.IsAssignableFrom<ITestInterface>(output);
    }

    [Fact]
    public void TryResolve_InvalidResolveWithNameAndOptions_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve<ITestInterface>("Testing", new ResolveOptions(), out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithParameters_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        var result = container.TryResolve<TestClassWithParameters>(
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithParameters_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        var result = container.TryResolve<TestClassWithParameters>(
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            out var output);

        Assert.IsType<TestClassWithParameters>(output);
    }

    [Fact]
    public void TryResolve_InvalidResolveWithParameters_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve<TestClassWithParameters>(
            new NamedParameterOverloads() { { "intProperty", 2 } },
            out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithNameAndParameters_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>("Testing");

        var result = container.TryResolve<TestClassWithParameters>(
            "Testing",
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithNameAndParameters_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>("Testing");

        var result = container.TryResolve<TestClassWithParameters>(
            "Testing",
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            out var output);

        Assert.IsType<TestClassWithParameters>(output);
    }

    [Fact]
    public void TryResolve_InvalidResolveWithNameAndParameters_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve<TestClassWithParameters>(
            "Testing",
            new NamedParameterOverloads() { { "intProperty", 2 } },
            out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithParametersAndOptions_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        var result = container.TryResolve<TestClassWithParameters>(
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            new ResolveOptions(),
            out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithParametersAndOptions_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        var result = container.TryResolve<TestClassWithParameters>(
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            new ResolveOptions(),
            out var output);

        Assert.IsType<TestClassWithParameters>(output);
    }

    [Fact]
    public void TryResolve_InvalidResolveWithParametersAndOptions_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve<TestClassWithParameters>(
            new NamedParameterOverloads() { { "intProperty", 2 } },
            new ResolveOptions(),
            out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithNameParametersAndOptions_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>("Testing");

        var result = container.TryResolve<TestClassWithParameters>(
            "Testing",
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            new ResolveOptions(),
            out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolve_ValidResolveWithNameParametersAndOptions_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>("Testing");

        var result = container.TryResolve<TestClassWithParameters>(
            "Testing",
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            new ResolveOptions(),
            out var output);

        Assert.IsType<TestClassWithParameters>(output);
    }

    [Fact]
    public void TryResolve_InvalidResolveWithNameParametersAndOptions_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve<TestClassWithParameters>(
            "Testing",
            new NamedParameterOverloads() { { "intProperty", 2 } },
            new ResolveOptions(),
            out var output);

        Assert.False(result);
    }

    [Fact]
    public void ResolveAll_MultipleTypesRegistered_ReturnsIEnumerableWithCorrectCount()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        container.Register<ITestInterface, TestClassDefaultCtor>("Named1");
        container.Register<ITestInterface, TestClassDefaultCtor>("Named2");

        var result = container.ResolveAll<ITestInterface>();

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void ResolveAll_NamedAndUnnamedRegisteredAndPassedTrue_ReturnsIEnumerableWithNamedAndUnnamedRegistrations()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        container.Register<ITestInterface, TestClassDefaultCtor>("Named1");
        container.Register<ITestInterface, TestClassDefaultCtor>("Named2");

        var result = container.ResolveAll<ITestInterface>(true);

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void ResolveAll_NamedAndUnnamedRegisteredAndPassedFalse_ReturnsIEnumerableWithJustNamedRegistrations()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        container.Register<ITestInterface, TestClassDefaultCtor>("Named1");
        container.Register<ITestInterface, TestClassDefaultCtor>("Named2");

        var result = container.ResolveAll<ITestInterface>(false);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void ResolveAll_NoTypesRegistered_ReturnsIEnumerableWithNoItems()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.ResolveAll<ITestInterface>();

        Assert.Empty(result);
    }

    [Fact]
    public void Resolve_TypeWithIEnumerableOfRegisteredTypeDependency_ResolvesWithIEnumerableOfCorrectCount()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();
        container.Register<ITestInterface, TestClassDefaultCtor>("Named1");
        container.Register<ITestInterface, TestClassDefaultCtor>("Named2");
        container.Register<TestClassEnumerableDependency>();

        var result = container.Resolve<TestClassEnumerableDependency>();

        Assert.Equal(2, result.EnumerableCount);
    }

    [Fact]
    public void Resolve_TypeWithIEnumerableOfNonRegisteredTypeDependency_ResolvesWithIEnumerablewithNoItems()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassEnumerableDependency>();

        var result = container.Resolve<TestClassEnumerableDependency>();

        Assert.Equal(0, result.EnumerableCount);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolve_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        var result = container.TryResolve(typeof(ITestInterface), out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolve_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        container.TryResolve(typeof(ITestInterface), out var output);

        Assert.IsAssignableFrom<ITestInterface>(output);
    }

    [Fact]
    public void TryResolveNonGeneric_InvalidResolve_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve(typeof(ITestInterface), out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithOptions_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        var result = container.TryResolve(typeof(ITestInterface), new ResolveOptions(), out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithOptions_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        var result = container.TryResolve(typeof(ITestInterface), new ResolveOptions(), out var output);

        Assert.IsAssignableFrom<ITestInterface>(output);
    }

    [Fact]
    public void TryResolveNonGeneric_InvalidResolveWithOptions_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve(typeof(ITestInterface), new ResolveOptions(), out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithName_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("Testing");

        var result = container.TryResolve(typeof(ITestInterface), "Testing", out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithName_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("Testing");

        container.TryResolve(typeof(ITestInterface), "Testing", out var output);

        Assert.IsAssignableFrom<ITestInterface>(output);
    }

    [Fact]
    public void TryResolveNonGeneric_InvalidResolveWithName_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve(typeof(ITestInterface), "Testing", out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithNameAndOptions_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("Testing");

        var result = container.TryResolve(typeof(ITestInterface), "Testing", new ResolveOptions(), out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithNameAndOptions_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("Testing");

        container.TryResolve(typeof(ITestInterface), "Testing", new ResolveOptions(), out var output);

        Assert.IsAssignableFrom<ITestInterface>(output);
    }

    [Fact]
    public void TryResolveNonGeneric_InvalidResolveWithNameAndOptions_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve(typeof(ITestInterface), "Testing", new ResolveOptions(), out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithParameters_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        var result = container.TryResolve(
            typeof(TestClassWithParameters),
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithParameters_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        var result = container.TryResolve(
            typeof(TestClassWithParameters),
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            out var output);

        Assert.IsType<TestClassWithParameters>(output);
    }

    [Fact]
    public void TryResolveNonGeneric_InvalidResolveWithParameters_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve(
            typeof(TestClassWithParameters),
            new NamedParameterOverloads() { { "intProperty", 2 } },
            out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithNameAndParameters_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>("Testing");

        var result = container.TryResolve(
            typeof(TestClassWithParameters),
            "Testing",
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithNameAndParameters_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>("Testing");

        var result = container.TryResolve(
            typeof(TestClassWithParameters),
            "Testing",
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            out var output);

        Assert.IsType<TestClassWithParameters>(output);
    }

    [Fact]
    public void TryResolveNonGeneric_InvalidResolveWithNameAndParameters_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve(
            typeof(TestClassWithParameters),
            "Testing",
            new NamedParameterOverloads() { { "intProperty", 2 } },
            out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithParametersAndOptions_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        var result = container.TryResolve(
            typeof(TestClassWithParameters),
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            new ResolveOptions(),
            out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithParametersAndOptions_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>();

        var result = container.TryResolve(
            typeof(TestClassWithParameters),
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            new ResolveOptions(),
            out var output);

        Assert.IsType<TestClassWithParameters>(output);
    }

    [Fact]
    public void TryResolveNonGeneric_InvalidResolveWithParametersAndOptions_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve(
            typeof(TestClassWithParameters),
            new NamedParameterOverloads() { { "intProperty", 2 } },
            new ResolveOptions(),
            out var output);

        Assert.False(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithNameParametersAndOptions_ReturnsTrue()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>("Testing");

        var result = container.TryResolve(
            typeof(TestClassWithParameters),
            "Testing",
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            new ResolveOptions(),
            out var output);

        Assert.True(result);
    }

    [Fact]
    public void TryResolveNonGeneric_ValidResolveWithNameParametersAndOptions_ReturnsType()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassWithParameters>("Testing");

        var result = container.TryResolve(
            typeof(TestClassWithParameters),
            "Testing",
            new NamedParameterOverloads() { { "stringProperty", "test" }, { "intProperty", 2 } },
            new ResolveOptions(),
            out var output);

        Assert.IsType<TestClassWithParameters>(output);
    }

    [Fact]
    public void TryResolveNonGeneric_InvalidResolveWithNameParametersAndOptions_ReturnsFalse()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.TryResolve(
            typeof(TestClassWithParameters),
            "Testing",
            new NamedParameterOverloads() { { "intProperty", 2 } },
            new ResolveOptions(),
            out var output);

        Assert.False(result);
    }

    [Fact]
    public void RegisterNonGeneric_BasicType_RegistersAndCanResolve()
    {
        var container = UtilityMethods.GetContainer();

        container.Register(typeof(TestClassDefaultCtor));
        var result = container.Resolve<TestClassDefaultCtor>(ResolveOptions.FailUnregisteredAndNameNotFound);

        Assert.IsType<TestClassDefaultCtor>(result);
    }

    [Fact]
    public void RegisterNonGeneric_BasicTypeAndName_RegistersAndCanResolve()
    {
        var container = UtilityMethods.GetContainer();

        container.Register(typeof(TestClassDefaultCtor), "TestClass");
        var result = container.Resolve<TestClassDefaultCtor>(
            "TestClass",
            ResolveOptions.FailUnregisteredAndNameNotFound);

        Assert.IsType<TestClassDefaultCtor>(result);
    }

    [Fact]
    public void RegisterNonGeneric_TypeImplementingInterface_RegistersAndCanResolve()
    {
        var container = UtilityMethods.GetContainer();

        container.Register(typeof(ITestInterface), typeof(TestClassDefaultCtor));
        var result = container.Resolve<ITestInterface>(ResolveOptions.FailUnregisteredAndNameNotFound);

        Assert.IsAssignableFrom<ITestInterface>(result);
    }

    [Fact]
    public void RegisterNonGeneric_NamedTypeImplementingInterface_RegistersAndCanResolve()
    {
        var container = UtilityMethods.GetContainer();

        container.Register(typeof(ITestInterface), typeof(TestClassDefaultCtor), "TestClass");
        var result = container.Resolve<ITestInterface>("TestClass", ResolveOptions.FailUnregisteredAndNameNotFound);

        Assert.IsAssignableFrom<ITestInterface>(result);
    }

    [Fact]
    public void RegisterNonGeneric_BasicTypeAndInstance_RegistersAndCanResolve()
    {
        var container = UtilityMethods.GetContainer();
        var instance  = new TestClassDefaultCtor();

        container.Register(typeof(TestClassDefaultCtor), instance);
        var result = container.Resolve<TestClassDefaultCtor>(ResolveOptions.FailUnregisteredAndNameNotFound);

        Assert.Same(instance, result);
    }

    [Fact]
    public void RegisterNonGeneric_BasicTypeInstanceAndName_RegistersAndCanResolve()
    {
        var container = UtilityMethods.GetContainer();
        var instance  = new TestClassDefaultCtor();

        container.Register(typeof(TestClassDefaultCtor), instance, "TestClass");
        var result = container.Resolve<TestClassDefaultCtor>(
            "TestClass",
            ResolveOptions.FailUnregisteredAndNameNotFound);

        Assert.Same(instance, result);
    }

    [Fact]
    public void RegisterNonGeneric_TypeImplementingInterfaceAndInstance_RegistersAndCanResolve()
    {
        var container = UtilityMethods.GetContainer();
        var instance  = new TestClassDefaultCtor();

        container.Register(typeof(ITestInterface), typeof(TestClassDefaultCtor), instance);
        var result = container.Resolve<ITestInterface>(ResolveOptions.FailUnregisteredAndNameNotFound);

        Assert.IsAssignableFrom<ITestInterface>(result);
    }

    [Fact]
    public void RegisterNonGeneric_TypeImplementingInterfaceInstanceAndName_RegistersAndCanResolve()
    {
        var container = UtilityMethods.GetContainer();
        var instance  = new TestClassDefaultCtor();

        container.Register(typeof(ITestInterface), typeof(TestClassDefaultCtor), instance, "TestClass");
        var result = container.Resolve<ITestInterface>("TestClass", ResolveOptions.FailUnregisteredAndNameNotFound);

        Assert.IsAssignableFrom<ITestInterface>(result);
    }

    [Fact]
    public void RegisterMultiple_Null_Throws()
    {
        var container = UtilityMethods.GetContainer();

        try
        {
            container.RegisterMultiple<ITestInterface>(null!);

            Assert.False(true);
        }
        catch (ArgumentNullException) { }
    }

    [Fact]
    public void RegisterMultiple_ATypeThatDoesntImplementTheRegisterType_Throws()
    {
        var container = UtilityMethods.GetContainer();

        try
        {
            container.RegisterMultiple<ITestInterface>(new Type[] { typeof(TestClassDefaultCtor), typeof(TestClass2) });

            Assert.False(true);
        }
        catch (ArgumentException) { }
    }

    [Fact]
    public void RegisterMultiple_ValidTypesButSameTypeMoreThanOnce_Throws()
    {
        var container = UtilityMethods.GetContainer();

        try
        {
            container.RegisterMultiple<ITestInterface>(
                new Type[] { typeof(TestClassDefaultCtor), typeof(TestClassDefaultCtor) });

            Assert.False(true);
        }
        catch (ArgumentException) { }
    }

    [Fact]
    public void RegisterMultiple_TypesThatImplementTheRegisterType_DoesNotThrow()
    {
        var container = UtilityMethods.GetContainer();

        container.RegisterMultiple<ITestInterface>(
            new Type[] { typeof(TestClassDefaultCtor), typeof(DisposableTestClassWithInterface) });
    }

    [Fact]
    public void RegisterMultiple_ValidTypes_ReturnsMultipleRegisterOptions()
    {
        var container = UtilityMethods.GetContainer();

        var result = container.RegisterMultiple<ITestInterface>(
            new Type[] { typeof(TestClassDefaultCtor), typeof(DisposableTestClassWithInterface) });

        Assert.IsType<DependencyInjectionContainer.MultiRegisterOptions>(result);
    }

    [Fact]
    public void RegisterMultiple_ValidTypes_CorrectCountReturnedByResolveAll()
    {
        var container = UtilityMethods.GetContainer();
        container.RegisterMultiple<ITestInterface>(
            new Type[] { typeof(TestClassDefaultCtor), typeof(DisposableTestClassWithInterface) });

        var result = container.ResolveAll<ITestInterface>();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void RegisterMultiple_ValidTypes_InstancesOfCorrectTypesReturnedByResolveAll()
    {
        var container = UtilityMethods.GetContainer();
        container.RegisterMultiple<ITestInterface>(
            new Type[] { typeof(TestClassDefaultCtor), typeof(DisposableTestClassWithInterface) });

        var result = container.ResolveAll<ITestInterface>();

        Assert.NotNull(result.Where(o => o.GetType() == typeof(TestClassDefaultCtor)).FirstOrDefault());
        Assert.NotNull(result.Where(o => o.GetType() == typeof(DisposableTestClassWithInterface)).FirstOrDefault());
    }

    [Fact]
    public void RegisterMultiple_ValidTypesRegisteredAsSingleton_AlwaysReturnsSameInstance()
    {
        var container = UtilityMethods.GetContainer();
        container.RegisterMultiple<ITestInterface>(
                      new Type[] { typeof(TestClassDefaultCtor), typeof(DisposableTestClassWithInterface) })
                 .AsSingleton();

        var result1               = container.ResolveAll<ITestInterface>();
        var result2               = container.ResolveAll<ITestInterface>();
        var result1Class1Instance = result1.Where(o => o.GetType() == typeof(TestClassDefaultCtor)).FirstOrDefault();
        var result2Class1Instance = result2.Where(o => o.GetType() == typeof(TestClassDefaultCtor)).FirstOrDefault();
        var result1Class2Instance =
            result1.Where(o => o.GetType() == typeof(DisposableTestClassWithInterface)).FirstOrDefault();
        var result2Class2Instance =
            result2.Where(o => o.GetType() == typeof(DisposableTestClassWithInterface)).FirstOrDefault();

        Assert.Same(result1Class1Instance, result2Class1Instance);
        Assert.Same(result1Class2Instance, result2Class2Instance);
    }

    [Fact]
    public void RegisterMultiple_ValidTypesRegisteredAsMultiInstance_AlwaysReturnsNewInstance()
    {
        var container = UtilityMethods.GetContainer();
        container.RegisterMultiple<ITestInterface>(
                      new Type[] { typeof(TestClassDefaultCtor), typeof(DisposableTestClassWithInterface) })
                 .AsMultiInstance();

        var result1               = container.ResolveAll<ITestInterface>();
        var result2               = container.ResolveAll<ITestInterface>();
        var result1Class1Instance = result1.Where(o => o.GetType() == typeof(TestClassDefaultCtor)).FirstOrDefault();
        var result2Class1Instance = result2.Where(o => o.GetType() == typeof(TestClassDefaultCtor)).FirstOrDefault();
        var result1Class2Instance =
            result1.Where(o => o.GetType() == typeof(DisposableTestClassWithInterface)).FirstOrDefault();
        var result2Class2Instance =
            result2.Where(o => o.GetType() == typeof(DisposableTestClassWithInterface)).FirstOrDefault();

        Assert.False(ReferenceEquals(result1Class1Instance, result2Class1Instance));
        Assert.False(ReferenceEquals(result1Class2Instance, result2Class2Instance));
    }

    [Fact]
    public void Resolve_MultiInstanceTypeInParentContainerButDependencyInChildContainer_GetsDependencyFromChild()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface2, TestClassWithInterfaceDependency>().AsMultiInstance();
        var parentInstance = new TestClassDefaultCtor();
        container.Register<ITestInterface>(parentInstance);
        var child         = container.GetChildContainer();
        var childInstance = new TestClassDefaultCtor();
        child.Register<ITestInterface>(childInstance);
        container.Resolve<ITestInterface2>();

        var result = child.Resolve<ITestInterface2>() as TestClassWithInterfaceDependency;

        Assert.True(ReferenceEquals(result!.Dependency, childInstance));
    }

    [Fact]
    public void
        Resolve_AlreadyResolvedSingletonTypeInParentContainerButDependencyInChildContainer_GetsDependencyFromParent()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface2, TestClassWithInterfaceDependency>().AsSingleton();
        var parentInstance = new TestClassDefaultCtor();
        container.Register<ITestInterface>(parentInstance);
        var child         = container.GetChildContainer();
        var childInstance = new TestClassDefaultCtor();
        child.Register<ITestInterface>(childInstance);
        container.Resolve<ITestInterface2>();

        var result = child.Resolve<ITestInterface2>() as TestClassWithInterfaceDependency;

        Assert.True(ReferenceEquals(result!.Dependency, parentInstance));
    }

    [Fact]
    public void
        Resolve_NotAlreadyResolvedSingletonTypeInParentContainerButDependencyInChildContainer_GetsDependencyFromParent()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface2, TestClassWithInterfaceDependency>().AsSingleton();
        var parentInstance = new TestClassDefaultCtor();
        container.Register<ITestInterface>(parentInstance);
        var child         = container.GetChildContainer();
        var childInstance = new TestClassDefaultCtor();
        child.Register<ITestInterface>(childInstance);

        var result = child.Resolve<ITestInterface2>() as TestClassWithInterfaceDependency;

        Assert.True(ReferenceEquals(result!.Dependency, parentInstance));
    }

    [Fact]
    public void Resolve_ContainerHierarchy_ResolvesCorrectlyUsingHierarchy()
    {
        var rootContainer        = UtilityMethods.GetContainer();
        var firstChildContainer  = rootContainer.GetChildContainer();
        var secondChildContainer = firstChildContainer.GetChildContainer();
        var rootInstance         = new TestClassDefaultCtor();
        var firstChildInstance   = new TestClassDefaultCtor();
        var secondChildInstance  = new TestClassDefaultCtor();
        rootContainer.Register<ITestInterface2, TestClassWithInterfaceDependency>().AsMultiInstance();
        rootContainer.Register<ITestInterface>(rootInstance);
        firstChildContainer.Register<ITestInterface>(firstChildInstance);
        secondChildContainer.Register<ITestInterface>(secondChildInstance);
        rootContainer.Resolve<ITestInterface2>();

        var result = secondChildContainer.Resolve<ITestInterface2>() as TestClassWithInterfaceDependency;

        Assert.True(ReferenceEquals(result!.Dependency, secondChildInstance));
    }

    [Fact]
    public void ResolveAll_ChildContainerNoRegistrationsParentContainerHasRegistrations_ReturnsAllParentRegistrations()
    {
        var parentContainer = UtilityMethods.GetContainer();
        var childContainer  = parentContainer.GetChildContainer();
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "1");
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "2");
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "3");

        var result = childContainer.ResolveAll<ITestInterface>();

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void ResolveAll_ChildContainerHasRegistrationsParentContainerHasRegistrations_ReturnsAllRegistrations()
    {
        var parentContainer = UtilityMethods.GetContainer();
        var childContainer  = parentContainer.GetChildContainer();
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "1");
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "2");
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "3");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "4");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "5");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "6");

        var result = childContainer.ResolveAll<ITestInterface>();

        Assert.Equal(6, result.Count());
    }

    [Fact]
    public void ResolveAll_ChildContainerRegistrationsParentContainerNoRegistrations_ReturnsAllChildRegistrations()
    {
        var parentContainer = UtilityMethods.GetContainer();
        var childContainer  = parentContainer.GetChildContainer();
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "1");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "2");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "3");

        var result = childContainer.ResolveAll<ITestInterface>();

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void
        ResolveAll_ChildContainerRegistrationsOverrideParentContainerRegistrations_ReturnsChildRegistrationsWithoutDuplicates()
    {
        var parentContainer = UtilityMethods.GetContainer();
        var childContainer  = parentContainer.GetChildContainer();
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "1");
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "2");
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "3");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "1");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "2");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "3");

        var result = childContainer.ResolveAll<ITestInterface>();

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void ResolveAll_ChildContainerRegistrationOverridesParentContainerRegistration_ReturnsChildRegistrations()
    {
        var parentContainer = UtilityMethods.GetContainer();
        var childContainer  = parentContainer.GetChildContainer();
        var parentInstance  = new TestClassDefaultCtor();
        var childInstance   = new TestClassDefaultCtor();
        parentContainer.Register<ITestInterface>(parentInstance, "1");
        childContainer.Register<ITestInterface>(childInstance, "1");

        var result = childContainer.ResolveAll<ITestInterface>();

        Assert.Single(result);
        Assert.Same(childInstance, result.Single());
    }

    [Fact]
    public void
        ResolveAll_ParentContainerMultiInstanceRegistrationWithDependencyInChildContainer_ReturnsRegistrationWithChildContainerDependencyInstance()
    {
        var parentContainer = UtilityMethods.GetContainer();
        var childContainer  = parentContainer.GetChildContainer();
        var parentInstance  = new TestClassDefaultCtor();
        var childInstance   = new TestClassDefaultCtor();
        parentContainer.Register<ITestInterface2, TestClassWithInterfaceDependency>("1").AsMultiInstance();
        parentContainer.Register<ITestInterface>(parentInstance);
        childContainer.Register<ITestInterface2, TestClassWithInterfaceDependency>("2").AsMultiInstance();
        childContainer.Register<ITestInterface>(childInstance);

        var result = childContainer.ResolveAll<ITestInterface2>(false).ToArray();
        var item1  = result[0] as TestClassWithInterfaceDependency;
        var item2  = result[1] as TestClassWithInterfaceDependency;

        Assert.NotNull(item1);
        Assert.NotNull(item2);
        Assert.False(ReferenceEquals(item1, item2), "items are same instance");
        Assert.True(ReferenceEquals(item1!.Dependency, childInstance), "item1 has wrong dependency");
        Assert.True(ReferenceEquals(item2!.Dependency, childInstance), "item2 has wrong dependency");
    }

    [Fact]
    public void
        Resolve_IEnumerableDependencyClassInChildContainerChildContainerNoRegistrationsParentContainerHasRegistrations_ReturnsAllParentRegistrations()
    {
        var parentContainer = UtilityMethods.GetContainer();
        var childContainer  = parentContainer.GetChildContainer();
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "1");
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "2");
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "3");
        childContainer.Register<TestClassEnumerableDependency>();

        var result = childContainer.Resolve<TestClassEnumerableDependency>();

        Assert.Equal(3, result.EnumerableCount);
    }

    [Fact]
    public void
        Resolve_IEnumerableDependencyClassInChildContainerChildContainerHasRegistrationsParentContainerHasRegistrations_ReturnsAllRegistrations()
    {
        var parentContainer = UtilityMethods.GetContainer();
        var childContainer  = parentContainer.GetChildContainer();
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "1");
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "2");
        parentContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "3");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "4");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "5");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "6");
        childContainer.Register<TestClassEnumerableDependency>();

        var result = childContainer.Resolve<TestClassEnumerableDependency>();

        Assert.Equal(6, result.EnumerableCount);
    }

    [Fact]
    public void
        Resolve_IEnumerableDependencyClassInChildContainerChildContainerRegistrationsParentContainerNoRegistrations_ReturnsAllChildRegistrations()
    {
        var parentContainer = UtilityMethods.GetContainer();
        var childContainer  = parentContainer.GetChildContainer();
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "1");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "2");
        childContainer.Register<ITestInterface>(new TestClassDefaultCtor(), "3");
        childContainer.Register<TestClassEnumerableDependency>();

        var result = childContainer.Resolve<TestClassEnumerableDependency>();

        Assert.Equal(3, result.EnumerableCount);
    }

    [Fact]
    public void
        Resolve_IEnumerableDependencyClassInChildContainerParentContainerMultiInstanceRegistrationWithDependencyInChildContainer_ReturnsRegistrationWithChildContainerDependencyInstance()
    {
        var parentContainer = UtilityMethods.GetContainer();
        var childContainer  = parentContainer.GetChildContainer();
        var parentInstance  = new TestClassDefaultCtor();
        var childInstance   = new TestClassDefaultCtor();
        parentContainer.Register<ITestInterface2, TestClassWithInterfaceDependency>("1").AsMultiInstance();
        parentContainer.Register<ITestInterface>(parentInstance);
        childContainer.Register<ITestInterface2, TestClassWithInterfaceDependency>("2").AsMultiInstance();
        childContainer.Register<ITestInterface>(childInstance);
        childContainer.Register<TestClassEnumerableDependency2>();

        var result = childContainer.Resolve<TestClassEnumerableDependency2>().Enumerable!.ToArray();
        var item1  = result[0] as TestClassWithInterfaceDependency;
        var item2  = result[1] as TestClassWithInterfaceDependency;

        Assert.NotNull(item1);
        Assert.NotNull(item2);
        Assert.False(ReferenceEquals(item1, item2), "items are same instance");
        Assert.True(ReferenceEquals(item1!.Dependency, childInstance), "item1 has wrong dependency");
        Assert.True(ReferenceEquals(item2!.Dependency, childInstance), "item2 has wrong dependency");
    }

    [Fact]
    public void ToCustomLifetimeProvider_NullInstance_Throws()
    {
        var container = UtilityMethods.GetContainer();

        try
        {
            DependencyInjectionContainer.RegisterOptions.ToCustomLifetimeManager(null!, null!, "");

            Assert.False(true, "Exception not thrown");
        }
        catch (ArgumentNullException) { }
    }

    [Fact]
    public void ToCustomLifetimeProvider_NullProvider_Throws()
    {
        var container    = UtilityMethods.GetContainer();
        var registration = container.Register<ITestInterface, TestClassDefaultCtor>();

        try
        {
            DependencyInjectionContainer.RegisterOptions.ToCustomLifetimeManager(registration, null!, "");

            Assert.False(true, "Exception not thrown");
        }
        catch (ArgumentNullException) { }
    }

    [Fact]
    public void ToCustomLifetimeProvider_NullErrorString_Throws()
    {
        var container    = UtilityMethods.GetContainer();
        var registration = container.Register<ITestInterface, TestClassDefaultCtor>();

        try
        {
            DependencyInjectionContainer.RegisterOptions.ToCustomLifetimeManager(
                registration,
                new FakeLifetimeProvider(),
                null!);

            Assert.False(true, "Exception not thrown");
        }
        catch (ArgumentException) { }
    }

    [Fact]
    public void ToCustomLifetimeProvider_EmptyErrorString_Throws()
    {
        var container    = UtilityMethods.GetContainer();
        var registration = container.Register<ITestInterface, TestClassDefaultCtor>();

        try
        {
            DependencyInjectionContainer.RegisterOptions.ToCustomLifetimeManager(
                registration,
                new FakeLifetimeProvider(),
                "");

            Assert.False(true, "Exception not thrown");
        }
        catch (ArgumentException) { }
    }

#if MOQ
        [Fact]
        public void CustomLifetimeProvider_WhenResolved_CallsGetObjectOnLifetimeProvider()
        {
            var container = UtilityMethods.GetContainer();
            var providerMock = new Mock<DependencyInjectionContainer.IDICObjectLifetimeProvider>();
            providerMock.Setup(p => p.GetObject()).Returns(new TestClassDefaultCtor());
            var registration = container.Register<ITestInterface, TestClassDefaultCtor>();
            DependencyInjectionContainer.RegisterOptions.ToCustomLifetimeManager(registration, providerMock.Object, "Mock");

            container.Resolve<ITestInterface>();

            providerMock.Verify(p => p.GetObject(), Times.Once(), "not called");
        }
#endif

#if MOQ
        [Fact]
        public void CustomLifetimeProvider_GetObjectReturnsNull_CallsSetObjectOnProvider()
        {
            var container = UtilityMethods.GetContainer();
            var providerMock = new Mock<DependencyInjectionContainer.IDICObjectLifetimeProvider>();
            providerMock.Setup(p => p.GetObject()).Returns(null);
            providerMock.Setup(p => p.SetObject(It.IsAny<object>())).Verifiable();
            var registration = container.Register<ITestInterface, TestClassDefaultCtor>();
            DependencyInjectionContainer.RegisterOptions.ToCustomLifetimeManager(registration, providerMock.Object, "Mock");

            container.Resolve<ITestInterface>();

            providerMock.Verify(p => p.SetObject(It.IsAny<object>()), Times.Once(), "not called");
        }
#endif

#if MOQ
        [Fact]
        public void CustomLifetimeProvider_SwitchingToAnotherFactory_CallsReleaseObjectOnProvider()
        {
            var container = UtilityMethods.GetContainer();
            var providerMock = new Mock<DependencyInjectionContainer.IDICObjectLifetimeProvider>();
            providerMock.Setup(p => p.ReleaseObject()).Verifiable();
            var registration = container.Register<ITestInterface, TestClassDefaultCtor>();
            registration =
 DependencyInjectionContainer.RegisterOptions.ToCustomLifetimeManager(registration, providerMock.Object, "Mock");

            registration.AsSingleton();

            providerMock.Verify(p => p.ReleaseObject(), Times.AtLeastOnce(), "not called");
        }
#endif

#if MOQ
        [Fact]
        public void CustomLifetimeProvider_ContainerDisposed_CallsReleaseObjectOnProvider()
        {
            var container = UtilityMethods.GetContainer();
            var providerMock = new Mock<DependencyInjectionContainer.IDICObjectLifetimeProvider>();
            providerMock.Setup(p => p.ReleaseObject()).Verifiable();
            var registration = container.Register<ITestInterface, TestClassDefaultCtor>();
            DependencyInjectionContainer.RegisterOptions.ToCustomLifetimeManager(registration, providerMock.Object, "Mock");

            container.Dispose();

            providerMock.Verify(p => p.ReleaseObject(), Times.AtLeastOnce(), "not called");
        }
#endif

    [Fact]
    public void AutoRegister_TypeExcludedViaPredicate_FailsToResolveType()
    {
        var container = UtilityMethods.GetContainer();
        container.AutoRegister(new[] { GetType().Assembly }, t => t != typeof(ITestInterface));

        Assert.Throws<DICResolutionException>(() => container.Resolve<ITestInterface>());

        //Assert.IsType<>(result, typeof(DICResolutionException));
    }

    [Fact]
    public void AutoRegister_Resolve_MultipleCalls()
    {
        var container = UtilityMethods.GetContainer();

        container.AutoRegister(new[] { GetType().Assembly }, t => typeof(ITestInterface).IsAssignableFrom(t));
        Assert.NotNull(container.Resolve<ITestInterface>());
        Assert.Throws<DICResolutionException>(() => container.Resolve<ITestInterface2>());

        container.AutoRegister(new[] { GetType().Assembly }, t => typeof(ITestInterface2).IsAssignableFrom(t));
        Assert.NotNull(container.Resolve<ITestInterface>());
        Assert.NotNull(container.Resolve<ITestInterface2>());
    }

#if RESOLVE_OPEN_GENERICS
        [Fact]
        public void Register_OpenGeneric_DoesNotThrow()
        {
            var container = UtilityMethods.GetContainer();

            container.Register(typeof(IThing<>), typeof(DefaultThing<>));
        }
#endif

#if RESOLVE_OPEN_GENERICS
        [Fact]
        public void Resolve_RegisteredOpenGeneric_ReturnsInstance()
        {
            var container = UtilityMethods.GetContainer();
            container.Register(typeof(IThing<>), typeof(DefaultThing<>));

            var result = container.Resolve<IThing<object>>();

            Assert.IsType<>(result, typeof(DefaultThing<object>));
        }
#endif


#if RESOLVE_OPEN_GENERICS
        [Fact]
        public void Resolve_RegisteredOpenGenericInParent_CanBeResolvedByChild()
        {
            var container = UtilityMethods.GetContainer();
            container.Register(typeof(IThing<>), typeof(DefaultThing<>));

            var child = container.GetChildContainer();

            var result = child.Resolve<IThing<object>>();
             
            Assert.IsType<>(result, typeof(DefaultThing<object>));
        }
#endif

#if RESOLVE_OPEN_GENERICS
        [Fact]
        public void Resolve_RegisteredOpenGeneric_CanGetGenericParamAsRequestedType()
        {
            // container.Register(
            //     typeof(ILogger<>),
            //     (c, p) =>
            //     {
            //         var type = (p["__requestedType"] as Type)?.GenericTypeArguments[0];
            //         Debug.Assert(type != null, nameof(type) + " != null");
            //         return c.Resolve<ILoggerFactory>().CreateLogger(type);
            //     });

            var container = UtilityMethods.GetContainer();
            container.Register(typeof(IThing<>), (c, parameters) =>
            {
                Assert.NotNull(parameters["__requestedType"]);
                var genericTypeArguments = (parameters["__requestedType"] as Type).GenericTypeArguments;
                Assert.True(genericTypeArguments.Length > 0);
                var returnType = typeof(DefaultThing<>).MakeGenericType(genericTypeArguments);
                return Activator.CreateInstance(returnType);
            });

            var result = container.Resolve<IThing<object>>();

            Assert.IsType<>(result, typeof(DefaultThing<object>));
        }
#endif

#region Unregister

    private readonly ResolveOptions options = ResolveOptions.FailUnregisteredAndNameNotFound;

#region Unregister With Implementation

    [Fact]
    public void Unregister_RegisteredImplementation_CanUnregister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();

        bool unregistered = container.Unregister(typeof(TestClassDefaultCtor));
        bool resolved     = container.CanResolve<TestClassDefaultCtor>(options);

        Assert.True(unregistered);
        Assert.False(resolved);
    }

    [Fact]
    public void Unregister_NotRegisteredImplementation_CannotUnregister()
    {
        var container = UtilityMethods.GetContainer();

        bool unregistered = container.Unregister(typeof(TestClassDefaultCtor));

        Assert.False(unregistered);
    }

    [Fact]
    public void Unregister_RegisteredNamedImplementation_CanUnregister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>("TestName");

        bool unregistered = container.Unregister(typeof(TestClassDefaultCtor), "TestName");
        bool resolved     = container.CanResolve<TestClassDefaultCtor>("TestName", options);

        Assert.True(unregistered);
        Assert.False(resolved);
    }

    [Fact]
    public void Unregister_NotRegisteredNamedImplementation_CannotUnregister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>("TestName");

        bool unregistered = container.Unregister(typeof(TestClassDefaultCtor), "UnregisteredName");
        bool resolved     = container.CanResolve<TestClassDefaultCtor>("TestName", options);

        Assert.False(unregistered);
        Assert.True(resolved);
    }

#endregion

#region Unregister With Interface

    [Fact]
    public void Unregister_RegisteredInterface_CanUnregister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        bool unregistered = container.Unregister(typeof(ITestInterface));
        bool resolved     = container.CanResolve<ITestInterface>(options);

        Assert.True(unregistered);
        Assert.False(resolved);
    }

    [Fact]
    public void Unregister_NotRegisteredInterface_CannotUnregister()
    {
        var container = UtilityMethods.GetContainer();

        bool unregistered = container.Unregister(typeof(ITestInterface));

        Assert.False(unregistered);
    }

    [Fact]
    public void Unregister_RegisteredNamedInterface_CanUnregister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("TestName");

        bool unregistered = container.Unregister(typeof(ITestInterface), "TestName");
        bool resolved     = container.CanResolve<ITestInterface>("TestName", options);

        Assert.True(unregistered);
        Assert.False(resolved);
    }

    [Fact]
    public void Unregister_NotRegisteredNamedInterface_CannotUnregister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("TestName");

        bool unregistered = container.Unregister(typeof(ITestInterface), "UnregisteredName");
        bool resolved     = container.CanResolve<ITestInterface>("TestName", options);

        Assert.False(unregistered);
        Assert.True(resolved);
    }

#endregion

#region Unregister<T> With Implementation

    [Fact]
    public void Unregister_T_RegisteredImplementation_CanUnregister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>();

        bool unregistered = container.Unregister<TestClassDefaultCtor>();
        bool resolved     = container.CanResolve<TestClassDefaultCtor>(options);

        Assert.True(unregistered);
        Assert.False(resolved);
    }

    [Fact]
    public void Unregister_T_NotRegisteredImplementation_CannotUnregister()
    {
        var container = UtilityMethods.GetContainer();

        bool unregistered = container.Unregister<TestClassDefaultCtor>();

        Assert.False(unregistered);
    }

    [Fact]
    public void Unregister_T_RegisteredNamedImplementation_CanUnregister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>("TestName");

        bool unregistered = container.Unregister<TestClassDefaultCtor>("TestName");
        bool resolved     = container.CanResolve<TestClassDefaultCtor>("TestName", options);

        Assert.True(unregistered);
        Assert.False(resolved);
    }

    [Fact]
    public void Unregister_T_NotRegisteredNamedImplementation_CannotUnregister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<TestClassDefaultCtor>("TestName");

        bool unregistered = container.Unregister<TestClassDefaultCtor>("UnregisteredName");
        bool resolved     = container.CanResolve<TestClassDefaultCtor>("TestName", options);

        Assert.False(unregistered);
        Assert.True(resolved);
    }

#endregion

#region Unregister<T> With Interface

    [Fact]
    public void Unregister_T_RegisteredInterface_CanUnregister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>();

        bool unregistered = container.Unregister<ITestInterface>();
        bool resolved     = container.CanResolve<ITestInterface>(options);

        Assert.True(unregistered);
        Assert.False(resolved);
    }

    [Fact]
    public void Unregister_T_NotRegisteredInterface_CannotUnregister()
    {
        var container = UtilityMethods.GetContainer();

        bool unregistered = container.Unregister<ITestInterface>();

        Assert.False(unregistered);
    }

    [Fact]
    public void Unregister_T_RegisteredNamedInterface_CanUnregister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("TestName");

        bool unregistered = container.Unregister<ITestInterface>("TestName");
        bool resolved     = container.CanResolve<ITestInterface>("TestName", options);

        Assert.True(unregistered);
        Assert.False(resolved);
    }

    [Fact]
    public void Unregister_T_NotRegisteredNamedInterface_CannotUnregister()
    {
        var container = UtilityMethods.GetContainer();
        container.Register<ITestInterface, TestClassDefaultCtor>("TestName");

        bool unregistered = container.Unregister<ITestInterface>("UnregisteredName");
        bool resolved     = container.CanResolve<ITestInterface>("TestName", options);

        Assert.False(unregistered);
        Assert.True(resolved);
    }

#endregion

#endregion
}