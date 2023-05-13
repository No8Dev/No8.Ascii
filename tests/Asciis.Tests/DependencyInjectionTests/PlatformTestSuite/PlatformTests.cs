using No8.Ascii.DependencyInjection;

namespace No8.Ascii.Tests.DependencyInjectionTests.PlatformTestSuite;

public interface ILogger
{
    void WriteLine(string text);
}

public class StringLogger : ILogger
{
    private readonly StringBuilder _builder = new();

    public string Log => _builder.ToString();
    
    public void WriteLine(string text)
    {
        _builder.AppendWithCRLF(text);
    }
}

public class PlatformTests
{
#region TestClasses
    public class TestClassNoInterface
    {
    }

    public interface ITestInterface
    {
    }

    public interface ITestInterface2
    {
    }

    public class TestClassWithInterface : ITestInterface
    {
    }

    public class TestClassWithInterface2 : ITestInterface
    {
    }

    public class TestClassWithConcreteDependency
    {
        public TestClassNoInterface? Dependency { get; set; }

        public TestClassWithConcreteDependency(TestClassNoInterface dependency)
        {
            Dependency = dependency;
        }

        public TestClassWithConcreteDependency()
        {

        }
    }

    public class TestClassWithInterfaceDependency
    {
        public ITestInterface Dependency { get; set; }

        public TestClassWithInterfaceDependency(ITestInterface dependency)
        {
            Dependency = dependency;
        }
    }

    public class TestClassWithParameters
    {
        public string StringProperty { get; set; }
        public int    IntProperty    { get; set; }

        public TestClassWithParameters(string stringProperty, int intProperty)
        {
            StringProperty = stringProperty;
            IntProperty    = intProperty;
        }
    }

    public class GenericClass<T> { }

    public class TestClassWithLazyFactory
    {
        private readonly Func<TestClassNoInterface> _factory;
        public           TestClassNoInterface       Prop1 { get; private set; }
        public           TestClassNoInterface       Prop2 { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestClassWithLazyFactory class.
        /// </summary>
        /// <param name="factory"></param>
        public TestClassWithLazyFactory(Func<TestClassNoInterface> factory)
        {
            _factory = factory;
            Prop1    = _factory.Invoke();
            Prop2    = _factory.Invoke();
        }
    }

    public class TestClassWithNamedLazyFactory
    {
        private readonly Func<string, TestClassNoInterface> _factory;
        public           TestClassNoInterface               Prop1 { get; private set; }
        public           TestClassNoInterface               Prop2 { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestClassWithLazyFactory class.
        /// </summary>
        /// <param name="factory"></param>
        public TestClassWithNamedLazyFactory(Func<string, TestClassNoInterface> factory)
        {
            _factory = factory;
            Prop1    = _factory.Invoke("Testing");
            Prop2    = _factory.Invoke("Testing");
        }
    }

    internal class TestclassWithNameAndParamsLazyFactory
    {
        private readonly Func<string, IDictionary<string, object>, TestClassWithParameters> _factory;
        public           TestClassWithParameters                                            Prop1 { get; private set; }

        public TestclassWithNameAndParamsLazyFactory(Func<string, IDictionary<string, object>, TestClassWithParameters> factory)
        {
            _factory = factory;
            Prop1 = _factory.Invoke("Testing", new Dictionary<string, object> { { "stringProperty", "Testing" }, { "intProperty", 22 } });
        }
    }

    internal class TestClassEnumerableDependency
    {
        private readonly IEnumerable<ITestInterface>? _enumerable;

        public int EnumerableCount => _enumerable?.Count() ?? 0;

        public TestClassEnumerableDependency(IEnumerable<ITestInterface> enumerable)
        {
            _enumerable = enumerable;
        }
    }

    public interface IThing<out T> where T : new()
    {
        T Get();
    }

    public class DefaultThing<T> : IThing<T> where T : new()
    {
        public T Get()
        {
            return new T();
        }
    }
#endregion


    private readonly ILogger _logger;
    private          int     _testsRun;
    private          int     _testsPassed;
    private          int     _testsFailed;

    private readonly List<Func<DependencyInjectionContainer, ILogger, bool>> _tests;

    public PlatformTests(ILogger logger)
    {
        _logger = logger;

        _tests = new List<Func<DependencyInjectionContainer, ILogger, bool>>()
                 {
                     AutoRegisterAppDomain,
                     AutoRegisterAssemblySpecified,
                     AutoRegisterPredicateExclusion,
                     RegisterConcrete,
                     ResolveConcreteUnregisteredDefaultOptions,
                     ResolveConcreteRegisteredDefaultOptions,
                     RegisterNamedConcrete,
                     ResolveNamedConcrete,
                     RegisterInstance,
                     RegisterInterface,
                     RegisterStrongRef,
                     RegisterWeakRef,
                     RegisterFactory,
                     RegisterAndSpecifyConstructor,
                     RegisterBoundGeneric,
                     ResolveLazyFactory,
                     ResolveNamedLazyFactory,
                     ResolveNamedAndParamsLazyFactory,
                     ResolveAll,
                     IEnumerableDependency,
                     RegisterMultiple,
                     NonGenericRegister,
                     OpenGenericRegistration,
                     OpenGenericResolution,
                     OpenGenericCanResolve
                 };
    }

    public void RunTests(out int testsRun, out int testsPassed, out int testsFailed)
    {
        _testsRun    = 0;
        _testsPassed = 0;
        _testsFailed = 0;

        foreach (var test in _tests)
        {
            var container = GetContainer();
            try
            {
                _testsRun++;
                if (test.Invoke(container, _logger))
                    _testsPassed++;
                else
                {
                    _testsFailed++;
                    _logger.WriteLine("Test Failed");
                }
            }
            catch (Exception ex)
            {
                _testsFailed++;
                _logger.WriteLine($"Test Failed: {ex.Message}");
            }
        }

        testsRun    = _testsRun;
        testsPassed = _testsPassed;
        testsFailed = _testsFailed;
    }

    private DependencyInjectionContainer GetContainer()
    {
        return new DependencyInjectionContainer();
    }

    private bool AutoRegisterAppDomain(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("AutoRegisterAppDomain");
        container.AutoRegister();
        container.Resolve<ITestInterface>();
        return true;
    }

    private bool AutoRegisterAssemblySpecified(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("AutoRegisterAssemblySpecified");
        container.AutoRegister(new[] { this.GetType().Assembly });
        container.Resolve<ITestInterface>();
        return true;
    }

    private bool RegisterConcrete(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("RegisterConcrete");
        container.Register<TestClassNoInterface>();
        container.Resolve<TestClassNoInterface>();
        return true;
    }

    private bool ResolveConcreteUnregisteredDefaultOptions(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("ResolveConcreteUnregisteredDefaultOptions");
        var output = container.Resolve<TestClassNoInterface>();

        return output is TestClassNoInterface;
    }

    private bool ResolveConcreteRegisteredDefaultOptions(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("ResolveConcreteRegisteredDefaultOptions");
        container.Register<TestClassNoInterface>();
        var output = container.Resolve<TestClassNoInterface>();

        return output is TestClassNoInterface;
    }

    private bool RegisterNamedConcrete(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("RegisterNamedConcrete");
        container.Register<TestClassNoInterface>("Testing");
        var output = container.Resolve<TestClassNoInterface>("Testing");

        return output is TestClassNoInterface;
    }

    private bool ResolveNamedConcrete(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("ResolveNamedConcrete");
        container.Register<TestClassNoInterface>("Testing");
        var output = container.Resolve<TestClassNoInterface>("Testing");

        return output is TestClassNoInterface;
    }

    private bool RegisterInstance(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("RegisterInstance");
        var obj = new TestClassNoInterface();
        container.Register<TestClassNoInterface>(obj);
        var output = container.Resolve<TestClassNoInterface>();
        return ReferenceEquals(obj, output);
    }

    private bool RegisterInterface(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("RegisterInterface");
        container.Register<ITestInterface, TestClassWithInterface>();
        var output = container.Resolve<ITestInterface>();
        return output is ITestInterface;
    }

    private bool RegisterStrongRef(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("RegisterStrongRef");
        var obj = new TestClassNoInterface();
        container.Register(obj).WithStrongReference();
        var output = container.Resolve<TestClassNoInterface>();

        return output is TestClassNoInterface;
    }

    private bool RegisterWeakRef(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("RegisterWeakRef");
        var obj = new TestClassNoInterface();
        container.Register(obj).WithWeakReference();
        var output = container.Resolve<TestClassNoInterface>();

        return output is TestClassNoInterface;
    }

    private bool RegisterFactory(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("RegisterFactory");
        container.Register((c, p) => new TestClassNoInterface());
        var output = container.Resolve<TestClassNoInterface>();

        return output is TestClassNoInterface;
    }

    private bool RegisterAndSpecifyConstructor(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("RegisterAndSpecifyConstructor");
        container.Register<TestClassWithConcreteDependency>().UsingConstructor(() => new TestClassWithConcreteDependency());
        var output = container.Resolve<TestClassWithConcreteDependency>();

        return output is TestClassWithConcreteDependency;
    }

    private bool RegisterBoundGeneric(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("RegisterBoundGeneric");
        container.Register<GenericClass<string>>();
        var output = container.Resolve<GenericClass<string>>();

        return output is GenericClass<string>;
    }

    private bool ResolveLazyFactory(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("ResolveLazyFactory");
        container.Register<TestClassNoInterface>();
        container.Register<TestClassWithLazyFactory>();
        var output = container.Resolve<TestClassWithLazyFactory>();
        return (output.Prop1 != null) && (output.Prop2 != null);
    }

    private bool ResolveNamedLazyFactory(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("ResolveNamedLazyFactory");
        container.Register<TestClassNoInterface>("Testing");
        container.Register<TestClassWithNamedLazyFactory>();
        var output = container.Resolve<TestClassWithNamedLazyFactory>();
        return (output.Prop1 != null) && (output.Prop2 != null);
    }

    private bool ResolveNamedAndParamsLazyFactory(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("ResolveNamedAndParamsLazyFactory");
        container.Register<TestClassWithParameters>("Testing");
        container.Register<TestclassWithNameAndParamsLazyFactory>();
        var output = container.Resolve<TestclassWithNameAndParamsLazyFactory>();
        return (output.Prop1 != null);
    }

    private bool ResolveAll(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("ResolveAll");
        container.Register<ITestInterface, TestClassWithInterface>();
        container.Register<ITestInterface, TestClassWithInterface>("Named1");
        container.Register<ITestInterface, TestClassWithInterface>("Named2");

        IEnumerable<ITestInterface> result = container.ResolveAll<ITestInterface>();

        return (result.Count() == 3);
    }

    private bool IEnumerableDependency(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("IEnumerableDependency");
        container.Register<ITestInterface, TestClassWithInterface>();
        container.Register<ITestInterface, TestClassWithInterface>("Named1");
        container.Register<ITestInterface, TestClassWithInterface>("Named2");
        container.Register<TestClassEnumerableDependency>();

        var result = container.Resolve<TestClassEnumerableDependency>();

        return (result.EnumerableCount == 2);
    }

    private bool RegisterMultiple(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("RegisterMultiple");
        container.RegisterMultiple<ITestInterface>(new Type[] { typeof(TestClassWithInterface), typeof(TestClassWithInterface2) });

        var result = container.ResolveAll<ITestInterface>();

        return (result.Count() == 2);
    }

    private bool NonGenericRegister(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("NonGenericRegister");
        container.Register(typeof(ITestInterface), typeof(TestClassWithInterface));

        var result = container.Resolve<ITestInterface>(ResolveOptions.FailUnregisteredAndNameNotFound);

        return true;
    }

    private bool AutoRegisterPredicateExclusion(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("AutoRegisterPredicateExclusion");
        container.AutoRegister(t => t != typeof(ITestInterface));

        try
        {
            container.Resolve<ITestInterface>();
            return false;
        }
        catch (DICResolutionException)
        {
        }

        return true;
    }

    private bool OpenGenericRegistration(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("OpenGenericRegistration");

        container.Register(typeof(IThing<>), typeof(DefaultThing<>));

        return true;
    }

    private bool OpenGenericResolution(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("OpenGenericResolution");

        container.Register(typeof(IThing<>), typeof(DefaultThing<>));

        var result = container.Resolve<IThing<object>>();

        return result != null && result.GetType() == typeof(DefaultThing<object>);
    }

    private bool OpenGenericCanResolve(DependencyInjectionContainer container, ILogger logger)
    {
        logger.WriteLine("OpenGenericCanResolve");

        container.Register(typeof(IThing<>), typeof(DefaultThing<>));

        return container.CanResolve(typeof(IThing<int>));
    }
}