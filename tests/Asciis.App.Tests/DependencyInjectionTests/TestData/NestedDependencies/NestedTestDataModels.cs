using System;

namespace Asciis.App.Tests.DependencyInjectionTests.TestData.NestedClassDependencies
{
    internal class Service1
    {
    }

    internal class Service2
    {
        private Service3 _service3;

        public Service2(Service3 service3)
        {
            _service3 = service3 ?? throw new ArgumentNullException(nameof(service3));
        }
    }

    internal class Service3
    {
    }

    internal class RootClass
    {
        private Service1? _service1;
        private Service2? _service2;

        public string StringProperty { get; set; }
        public int IntProperty { get; set; }

        public RootClass(Service1 service1, Service2 service2)
            : this(service1, service2, "DEFAULT", 1976)
        {
        }

        public RootClass(Service1 service1, Service2 service2, string stringProperty, int intProperty)
        {
            _service1  = service1 ?? throw new ArgumentNullException(nameof(service1));
            _service2  = service2 ?? throw new ArgumentNullException(nameof(service2));
            StringProperty = stringProperty;
            IntProperty    = intProperty;
        }
    }
}
