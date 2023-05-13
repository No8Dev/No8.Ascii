using System;
using Asciis.App.DependencyInjection;
using Asciis.App.Tests.DependencyInjectionTests.TestData.BasicClasses;

namespace Asciis.App.Tests.DependencyInjectionTests.TestData;

public class UtilityMethods
{
    internal static DependencyInjectionContainer GetContainer()
    {
        return new DependencyInjectionContainer();
    }

    internal static void RegisterInstanceStrongRef(DependencyInjectionContainer container)
    {
        var item = new TestClassDefaultCtor
                   {
                       Prop1 = "Testing"
                   };
        container.Register<TestClassDefaultCtor>(item).WithStrongReference();
    }

    internal static void RegisterInstanceWeakRef(DependencyInjectionContainer container)
    {
        var item = new TestClassDefaultCtor
                   {
                       Prop1 = "Testing"
                   };
        container.Register<TestClassDefaultCtor>(item).WithWeakReference();
    }

    internal static void RegisterFactoryStrongRef(DependencyInjectionContainer container)
    {
        var source = new TestClassDefaultCtor
                     {
                         Prop1 = "Testing"
                     };

        var item = new Func<DependencyInjectionContainer, NamedParameterOverloads, TestClassDefaultCtor>((c, p) => source);
        container.Register<TestClassDefaultCtor>(item).WithStrongReference();
    }

    internal static void RegisterFactoryWeakRef(DependencyInjectionContainer container)
    {
        var source = new TestClassDefaultCtor
                     {
                         Prop1 = "Testing"
                     };

        var item = new Func<DependencyInjectionContainer, NamedParameterOverloads, TestClassDefaultCtor>((c, p) => source);
        container.Register(item).WithWeakReference();
    }

    public static ITinyMessengerHub GetMessenger()
    {
        return new TinyMessengerHub();
    }

    public static void FakeDeliveryAction<T>(T message)
        where T:IPubSubMessage
    {
    }

    public static bool FakeMessageFilter<T>(T message)
        where T:IPubSubMessage
    {
        return true;
    }

    public static TinyMessageSubscriptionToken GetTokenWithOutOfScopeMessenger()
    {
        var messenger = UtilityMethods.GetMessenger();

        var token = new TinyMessageSubscriptionToken(messenger, typeof(TestMessage));

        return token;
    }
}