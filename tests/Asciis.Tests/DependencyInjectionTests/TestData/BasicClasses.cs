using No8.Ascii.DependencyInjection;

namespace No8.Ascii.Tests.DependencyInjectionTests.TestData
{
    namespace BasicClasses
    {
        internal interface ITestInterface
        {
        }

        internal interface ITestInterface<T, TS>
        {
        }

        internal class TestClassDefaultCtor : ITestInterface
        {
            public string? Prop1 { get; set; }

            public TestClassDefaultCtor() { }

            public static ITestInterface CreateNew(DependencyInjectionContainer container)
            {
                return new TestClassDefaultCtor { Prop1 = "Testing" };
            }
        }

        internal interface ITestInterface2
        {
        }

        internal class TestClass2 : ITestInterface2
        {
        }

        internal class TestClassWithContainerDependency
        {
            public DependencyInjectionContainer Container { get; }

            public TestClassWithContainerDependency(DependencyInjectionContainer container)
            {
                Container = container ?? throw new ArgumentNullException(nameof(container));
            }
        }

        internal class TestClassWithInterfaceDependency : ITestInterface2
        {
            public ITestInterface Dependency { get; set; }

            public int Param1 { get; }

            public string? Param2 { get; }

            public TestClassWithInterfaceDependency(ITestInterface dependency)
            {
                Dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
            }

            public TestClassWithInterfaceDependency(ITestInterface dependency, int param1, string param2)
            {
                Dependency = dependency;
                Param1 = param1;
                Param2 = param2;
            }
        }

        internal class TestClassWithDependency
        {
            private TestClassDefaultCtor? Dependency { get; set; }

            public int Param1 { get; }

            public string? Param2 { get; }

            public TestClassWithDependency(TestClassDefaultCtor dependency)
            {
                Dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
            }

            public TestClassWithDependency(TestClassDefaultCtor dependency, int param1, string param2)
            {
                Param1 = param1;
                Param2 = param2;
            }
        }

        internal class TestClassPrivateCtor
        {
            private TestClassPrivateCtor() { }
        }

        internal class TestClassProtectedCtor
        {
            protected TestClassProtectedCtor() { }
        }

        internal class TestSubclassProtectedCtor
        {
            public TestSubclassProtectedCtor() { }
        }

        internal class TestClassWithParameters
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }

            public TestClassWithParameters(string stringProperty, int intProperty)
            {
                StringProperty = stringProperty;
                IntProperty = intProperty;
            }
        }

        internal class TestClassWithDependencyAndParameters
        {
            TestClassDefaultCtor? Dependency { get; set; }

            public int Param1 { get; private set; }

            public string Param2 { get; private set; }

            public TestClassWithDependencyAndParameters(TestClassDefaultCtor dependency, int param1, string param2)
            {
                Dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
                Param1     = param1;
                Param2     = param2;
            }
        }

        internal class TestClassNoInterfaceDefaultCtor
        {
            public TestClassNoInterfaceDefaultCtor() { }
        }

        internal class TestClassNoInterfaceDependency
        {
            public ITestInterface Dependency { get; set; }

            public TestClassNoInterfaceDependency(ITestInterface dependency)
            {
                Dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
            }
        }

        public class DisposableTestClassWithInterface : IDisposable, ITestInterface
        {
            public void Dispose() { }
        }

        public class GenericClassWithInterface<T, TS> : ITestInterface
        {
            public T?  Prop1 { get; set; }
            public TS? Prop2 { get; set; }

            public GenericClassWithInterface() { }

            public GenericClassWithInterface(T prop1, TS prop2)
            {
                Prop1 = prop1;
                Prop2 = prop2;
            }
        }

        internal class GenericClassWithGenericInterface<T, TS> : ITestInterface<T, TS>
        {
            public T?  Prop1 { get; set; }
            public TS? Prop2 { get; set; }

             public GenericClassWithGenericInterface() { }

             public GenericClassWithGenericInterface(T prop1, TS prop2)
            {
                Prop1 = prop1;
                Prop2 = prop2;
            }
        }

        internal class GenericClassWithParametersAndDependencies<T, TS>
        {
            public ITestInterface2 Dependency { get; }
            public T?              Prop1      { get; set; }
            public TS?             Prop2      { get; set; }

            public GenericClassWithParametersAndDependencies(ITestInterface2 dependency)
            {
                Dependency = dependency;
            }

            public GenericClassWithParametersAndDependencies(ITestInterface2 dependency, T prop1, TS prop2)
            {
                Dependency = dependency;
                Prop1 = prop1;
                Prop2 = prop2;
            }
        }

        internal class TestClassWithLazyFactory
        {
            private readonly Func<TestClassDefaultCtor> _factory;
            public           TestClassDefaultCtor?      Prop1 { get; private set; }
            public           TestClassDefaultCtor?      Prop2 { get; private set; }
            
            /// <summary>
            /// Initializes a new instance of the TestClassWithLazyFactory class.
            /// </summary>
            /// <param name="factory"></param>
            public TestClassWithLazyFactory(Func<TestClassDefaultCtor> factory)
            {
                _factory = factory;
            }

            public void Method1()
            {
                Prop1 = _factory.Invoke();
            }

            public void Method2()
            {
                Prop2 = _factory.Invoke();
            }

        }

        internal class TestClassWithNamedLazyFactory
        {
            private readonly Func<string, TestClassDefaultCtor> _factory;
            public           TestClassDefaultCtor?              Prop1 { get; private set; }
            public           TestClassDefaultCtor?              Prop2 { get; private set; }

            /// <summary>
            /// Initializes a new instance of the TestClassWithLazyFactory class.
            /// </summary>
            /// <param name="factory"></param>
            public TestClassWithNamedLazyFactory(Func<string, TestClassDefaultCtor> factory)
            {
                _factory = factory;
            }

            public void Method1()
            {
                Prop1 = _factory.Invoke("Testing");
            }

            public void Method2()
            {
                Prop2 = _factory.Invoke(string.Empty);
            }

        }

        internal class TestClassWithNameAndParamsLazyFactory
        {
            private readonly Func<string, IDictionary<string, object>, TestClassWithParameters> _factory;
            public           TestClassWithParameters?                                            Prop1 { get; }

            /// <summary>
            /// Initializes a new instance of the TestClassWithNameAndParamsLazyFactory class.
            /// </summary>
            /// <param name="factory"></param>
            public TestClassWithNameAndParamsLazyFactory(Func<string, IDictionary<string, object>, TestClassWithParameters> factory)
            {
                _factory = factory;
                Prop1 = _factory.Invoke("", new Dictionary<string, object> { { "stringProperty", "Testing" }, { "intProperty", 22 } });
            }

        }

        internal class TestClassMultiDepsMultiCtors
        {
            public TestClassDefaultCtor? Prop1                { get; }
            public TestClassDefaultCtor? Prop2                { get; }
            public int                   NumberOfDepsResolved { get; }

            public TestClassMultiDepsMultiCtors(TestClassDefaultCtor prop1)
            {
                Prop1 = prop1;
                NumberOfDepsResolved = 1;
            }

            public TestClassMultiDepsMultiCtors(TestClassDefaultCtor prop1, TestClassDefaultCtor prop2)
            {
                Prop1 = prop1;
                Prop2 = prop2;
                NumberOfDepsResolved = 2;
            }
        }

        internal class TestClassConstructorFailure
        {
            /// <summary>
            /// Initializes a new instance of the TestClassConstructorFailure class.
            /// </summary>
            public TestClassConstructorFailure()
            {
                throw new NotImplementedException();
            }
       }

        internal abstract class TestClassBase
        {
        }

        internal class TestClassWithBaseClass : TestClassBase
        {

        }

        internal class TestClassPropertyDependencies
        {
            public ITestInterface?  Property1 { get; set; }
            public ITestInterface2? Property2 { get; set; }
            public int              Property3 { get; set; }
            public string?          Property4 { get; set; }

            public TestClassDefaultCtor? ConcreteProperty { get; set; }

            public ITestInterface?  ReadOnlyProperty  { get;          private set; }
            public ITestInterface2? WriteOnlyProperty { internal get; set; }

            /// <summary>
            /// Initializes a new instance of the TestClassPropertyDependencies class.
            /// </summary>
            public TestClassPropertyDependencies() { }
        }

        internal class TestClassEnumerableDependency
        {
            public IEnumerable<ITestInterface>? Enumerable {get; private set;}

            public int EnumerableCount => Enumerable?.Count() ?? 0;

            public TestClassEnumerableDependency(IEnumerable<ITestInterface> enumerable)
            {
                Enumerable = enumerable;
            }
        }

        internal class TestClassEnumerableDependency2
        {
            public IEnumerable<ITestInterface2>? Enumerable { get; private set; }

            public int EnumerableCount => Enumerable?.Count() ?? 0;

            public TestClassEnumerableDependency2(IEnumerable<ITestInterface2> enumerable)
            {
                Enumerable = enumerable;
            }
        }

        public interface IThing<out T> where T: new()
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

        internal class TestClassWithConstructorAttrib
        {
            [DICConstructor]
            public TestClassWithConstructorAttrib()
            {
                AttributeConstructorUsed = true;
            }

            public TestClassWithConstructorAttrib(object someParameter)
            {
                AttributeConstructorUsed = false;
            }

            public bool AttributeConstructorUsed { get; }
        }

        internal class TestClassWithInternalConstructorAttrib
        {
            [DICConstructor]
            internal TestClassWithInternalConstructorAttrib()
            {
                AttributeConstructorUsed = true;
            }

            public TestClassWithInternalConstructorAttrib(object someParameter)
            {
                AttributeConstructorUsed = false;
            }

            public bool AttributeConstructorUsed { get; }
        }

        internal class TestClassWithManyConstructorAttribs
        {
            [DICConstructor]
            public TestClassWithManyConstructorAttribs()
            {
                MostGreedyAttribCtorUsed = false;
            }

            [DICConstructor]
            public TestClassWithManyConstructorAttribs(object someParameter)
            {
                MostGreedyAttribCtorUsed = true;
            }

            public TestClassWithManyConstructorAttribs(object a, object b)
            {
                MostGreedyAttribCtorUsed = false;
            }

            public bool MostGreedyAttribCtorUsed { get; }
        }
    }
}
