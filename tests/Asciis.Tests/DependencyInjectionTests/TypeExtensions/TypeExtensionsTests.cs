using System.Reflection;
using No8.Ascii.DependencyInjection;
using No8.Ascii.Tests.Helpers;
using Xunit;

namespace No8.Ascii.Tests.DependencyInjectionTests.TypeExtensions;

public interface ITestInterface
{
}

public class ClassImplementingITestInterface : ITestInterface
{
}

public class AnotherClassImplementingITestInterface : ITestInterface
{
        
}

public class ClassNotImplementingITestInterface
{
}

[TestClass]
public class TypeExtensionsTests
{
    [Fact]
    public void GetGenericMethod_RegisterOneGenericParameterNoParameters_ReturnsCorrectMethod()
    {
        var firstGenericParameter = typeof(ClassNotImplementingITestInterface);

        var method = typeof(DependencyInjectionContainer).GetGenericMethod(
            BindingFlags.Public | BindingFlags.Instance, 
            "Register", 
            new[] {firstGenericParameter}, 
            new Type[] { }
        );

        Assert.IsAssignableFrom<MethodInfo>(method);
        Assert.True(method!.IsGenericMethod);
        Assert.Empty(method.GetParameters());
        Assert.Single(method.GetGenericArguments());
        Assert.Equal(firstGenericParameter, method.GetGenericArguments()[0]);
    }

    [Fact]
    public void GetGenericMethod_RegisterTwoAcceptableGenericParameterNoParameters_ReturnsCorrectMethod()
    {
        var firstGenericParameter  = typeof(ITestInterface);
        var secondGenericParameter = typeof(ClassImplementingITestInterface);

        var method = typeof(DependencyInjectionContainer).GetGenericMethod(
            BindingFlags.Public | BindingFlags.Instance,
            "Register",
            new[] { firstGenericParameter, secondGenericParameter },
            new Type[] { }
        );

        Assert.IsAssignableFrom<MethodInfo>(method);
        Assert.True(method!.IsGenericMethod);
        Assert.Empty(method.GetParameters());
        Assert.Equal(2, method.GetGenericArguments().Length);
        Assert.Equal(firstGenericParameter, method.GetGenericArguments()[0]);
        Assert.Equal(secondGenericParameter, method.GetGenericArguments()[1]);
    }

    [Fact]
    public void GetGenericMethod_TwiceWithDifferentGenericParamters_ReturnsCorrectMethods()
    {
        var methodOneFirstGenericParameter  = typeof(ITestInterface);
        var methodOneSecondGenericParameter = typeof(ClassImplementingITestInterface);
        var methodTwoFirstGenericParameter  = typeof(ITestInterface);
        var methodTwoSecondGenericParameter = typeof(AnotherClassImplementingITestInterface);

        var methodOne = typeof(DependencyInjectionContainer).GetGenericMethod(
            BindingFlags.Public | BindingFlags.Instance,
            "Register",
            new[] { methodOneFirstGenericParameter, methodOneSecondGenericParameter },
            new Type[] { });
        var methodTwo = typeof(DependencyInjectionContainer).GetGenericMethod(
            BindingFlags.Public | BindingFlags.Instance,
            "Register",
            new[] { methodTwoFirstGenericParameter, methodTwoSecondGenericParameter },
            new Type[] { });

        Assert.IsAssignableFrom<MethodInfo>(methodOne);
        Assert.True(methodOne!.IsGenericMethod);
        Assert.Empty(methodOne.GetParameters());
        Assert.Equal(2, methodOne.GetGenericArguments().Length);
        Assert.Equal(methodOneFirstGenericParameter, methodOne.GetGenericArguments()[0]);
        Assert.Equal(methodOneSecondGenericParameter, methodOne.GetGenericArguments()[1]);

        Assert.IsAssignableFrom<MethodInfo>(methodTwo);
        Assert.True(methodTwo!.IsGenericMethod);
        Assert.Empty(methodTwo.GetParameters());
        Assert.Equal(2, methodTwo.GetGenericArguments().Length);
        Assert.Equal(methodTwoFirstGenericParameter, methodTwo.GetGenericArguments()[0]);
        Assert.Equal(methodTwoSecondGenericParameter, methodTwo.GetGenericArguments()[1]);
    }

    [Fact]
    public void GetGenericMethod_RegisterTwoUnacceptableGenericParameterNoParameters_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var _ = typeof(DependencyInjectionContainer).GetGenericMethod(
                BindingFlags.Public | BindingFlags.Instance,
                "Register",
                new[] { typeof(ITestInterface), typeof(ClassNotImplementingITestInterface) },
                new Type[] { }
            );

            Assert.False(true);
        });
    }

    [Fact]
    public void GetGenericMethod_RegisterTwoAcceptableGenericParameterMethodParameters_ReturnsCorrectMethod()
    {
        var firstGenericParameter  = typeof(ITestInterface);
        var secondGenericParameter = typeof(ClassImplementingITestInterface);
        var firstParameter         = typeof(string);

        var method = typeof(DependencyInjectionContainer).GetGenericMethod(
            BindingFlags.Public | BindingFlags.Instance,
            "Register",
            new[] { firstGenericParameter, secondGenericParameter },
            new[] { firstParameter }
        );

        Assert.IsAssignableFrom<MethodInfo>(method);
        Assert.True(method!.IsGenericMethod);
        Assert.Single(method.GetParameters());
        Assert.Equal(firstParameter, method.GetParameters()[0].ParameterType);
        Assert.Equal(2, method.GetGenericArguments().Length);
        Assert.Equal(firstGenericParameter, method.GetGenericArguments()[0]);
        Assert.Equal(secondGenericParameter, method.GetGenericArguments()[1]);
    }

    [Fact]
    public void GetGenericMethod_RegisterWithGenericTypeAsAMethodParameter_ReturnsCorrectMethod()
    {
        var firstGenericParameter  = typeof(ITestInterface);
        var secondGenericParameter = typeof(ClassImplementingITestInterface);
        var firstParameter         = typeof(ClassImplementingITestInterface);

        var method = typeof(DependencyInjectionContainer).GetGenericMethod(
            BindingFlags.Public | BindingFlags.Instance,
            "Register",
            new[] { firstGenericParameter, secondGenericParameter },
            new[] { firstParameter }
        );

        Assert.IsAssignableFrom<MethodInfo>(method);
        Assert.True(method!.IsGenericMethod);
        Assert.Single(method.GetParameters());
        Assert.Equal(firstParameter, method.GetParameters()[0].ParameterType);
        Assert.Equal(2, method.GetGenericArguments().Length);
        Assert.Equal(firstGenericParameter, method.GetGenericArguments()[0]);
        Assert.Equal(secondGenericParameter, method.GetGenericArguments()[1]);
    }
}