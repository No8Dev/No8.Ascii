using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace No8.Ascii.DependencyInjection;

public static class AssemblyExtensions
{
    public static Type[] SafeGetTypes(this Assembly assembly)
    {
        Type[] assemblies;

        try
        {
            assemblies = assembly.GetTypes();
        }
        catch (FileNotFoundException)
        {
            assemblies = new Type[] { };
        }
        catch (NotSupportedException)
        {
            assemblies = new Type[] { };
        }
        catch (ReflectionTypeLoadException e)
        {
            assemblies = e.Types.Where(t => t != null).OfType<Type>().ToArray();
        }
        return assemblies;
    }
}

public static class TypeExtensions
{
    private static readonly ConcurrentDictionary<GenericMethodCacheKey, MethodInfo> GenericMethodCache = new();

    /// <summary>
    /// Gets a generic method from a type given the method name, binding flags, generic types and parameter types
    /// </summary>
    /// <param name="sourceType">Source type</param>
    /// <param name="bindingFlags">Binding flags</param>
    /// <param name="methodName">Name of the method</param>
    /// <param name="genericTypes">Generic types to use to make the method generic</param>
    /// <param name="parameterTypes">Method parameters</param>
    /// <returns>MethodInfo or null if no matches found</returns>
    /// <exception cref="System.Reflection.AmbiguousMatchException"/>
    /// <exception cref="System.ArgumentException"/>
    public static MethodInfo? GetGenericMethod(this Type sourceType, BindingFlags bindingFlags, string methodName, Type[] genericTypes, Type[] parameterTypes)
    {
        var cacheKey = new GenericMethodCacheKey(sourceType, methodName, genericTypes, parameterTypes);

        // Shouldn't need any additional locking
        // we don't care if we do the method info generation
        // more than once before it gets cached.
        if (!GenericMethodCache.TryGetValue(cacheKey, out var method))
        {
            method = GetMethod(sourceType, bindingFlags, methodName, genericTypes, parameterTypes);
            if (method != null)
                GenericMethodCache[cacheKey] = method;
        }

        return method;
    }

    private static MethodInfo? GetMethod(Type sourceType, BindingFlags bindingFlags, string methodName, Type[] genericTypes, Type[] parameterTypes)
    {
        var methods =
            sourceType.GetMethods(bindingFlags).Where(
                mi => string.Equals(methodName, mi.Name, StringComparison.Ordinal)).Where(
                    mi => mi.ContainsGenericParameters).Where(mi => mi.GetGenericArguments().Length == genericTypes.Length).
                Where(mi => mi.GetParameters().Length == parameterTypes.Length).Select(
                    mi => mi.MakeGenericMethod(genericTypes)).Where(
                        mi => mi.GetParameters().Select(pi => pi.ParameterType).SequenceEqual(parameterTypes)).ToList();
        if (methods.Count > 1)
        {
            throw new AmbiguousMatchException();
        }

        return methods.FirstOrDefault();
    }

    private sealed class GenericMethodCacheKey
    {
        private readonly Type _sourceType;

        private readonly string _methodName;

        private readonly Type[] _genericTypes;

        private readonly Type[] _parameterTypes;

        public GenericMethodCacheKey(Type sourceType, string methodName, Type[] genericTypes, Type[] parameterTypes)
        {
            _sourceType = sourceType;
            _methodName = methodName;
            _genericTypes = genericTypes;
            _parameterTypes = parameterTypes;
        }

        private bool Equals(GenericMethodCacheKey other)
        {
            return _sourceType == other._sourceType &&
                   _methodName == other._methodName &&
                   _genericTypes.Equals(other._genericTypes) &&
                   _parameterTypes.Equals(other._parameterTypes);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) ||
                   obj is GenericMethodCacheKey other &&
                   Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_sourceType, _methodName, _genericTypes, _parameterTypes);
        }

        public static bool operator ==(GenericMethodCacheKey? left, GenericMethodCacheKey? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GenericMethodCacheKey? left, GenericMethodCacheKey? right)
        {
            return !Equals(left, right);
        }
    }

}


internal static class ReverseTypeExtender
{
    public static bool IsClass(this Type type)
    {
        return type.IsClass;
    }

    public static bool IsAbstract(this Type type)
    {
        return type.IsAbstract;
    }

    public static bool IsInterface(this Type type)
    {
        return type.IsInterface;
    }

    public static bool IsPrimitive(this Type type)
    {
        return type.IsPrimitive;
    }

    public static bool IsValueType(this Type type)
    {
        return type.IsValueType;
    }

    public static bool IsGenericType(this Type type)
    {
        return type.IsGenericType;
    }

    public static bool IsGenericParameter(this Type type)
    {
        return type.IsGenericParameter;
    }

    public static bool IsGenericTypeDefinition(this Type type)
    {
        return type.IsGenericTypeDefinition;
    }

    public static Type? BaseType(this Type type)
    {
        return type.BaseType;
    }

    public static Assembly Assembly(this Type type)
    {
        return type.Assembly;
    }
}


