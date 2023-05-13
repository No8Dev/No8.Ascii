using System.Runtime.Serialization;

namespace Asciis.App.DependencyInjection;

[Serializable]
public class DICResolutionException : Exception
{
    private const string ErrorText = "Unable to resolve type: {0}";

    public DICResolutionException(Type type)
        : base(string.Format(ErrorText, type.FullName))
    {
    }

    public DICResolutionException(Type type, Exception innerException)
        : base(string.Format(ErrorText, type.FullName), innerException)
    {
    }
    protected DICResolutionException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

[Serializable]
public class DICRegistrationTypeException : Exception
{
    private const string RegisterErrorText = "Cannot register type {0} - abstract classes or interfaces are not valid implementation types for {1}.";

    public DICRegistrationTypeException(Type type, string factory)
        : base(string.Format(RegisterErrorText, type.FullName, factory))
    {
    }

    public DICRegistrationTypeException(Type type, string factory, Exception innerException)
        : base(string.Format(RegisterErrorText, type.FullName, factory), innerException)
    {
    }
    protected DICRegistrationTypeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
[Serializable]

public class DICRegistrationException : Exception
{
    private const string ConvertErrorText           = "Cannot convert current registration of {0} to {1}";
    private const string GenericConstraintErrorText = "Type {1} is not valid for a registration of type {0}";

    public DICRegistrationException(Type type, string method)
        : base(string.Format(ConvertErrorText, type.FullName, method))
    {
    }

    public DICRegistrationException(Type type, string method, Exception innerException)
        : base(string.Format(ConvertErrorText, type.FullName, method), innerException)
    {
    }

    public DICRegistrationException(Type registerType, Type implementationType)
        : base(string.Format(GenericConstraintErrorText, registerType.FullName, implementationType.FullName))
    {
    }

    public DICRegistrationException(Type registerType, Type implementationType, Exception innerException)
        : base(string.Format(GenericConstraintErrorText, registerType.FullName, implementationType.FullName), innerException)
    {
    }
    protected DICRegistrationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

[Serializable]
public class DICWeakReferenceException : Exception
{
    private const string ErrorText = "Unable to instantiate {0} - referenced object has been reclaimed";

    public DICWeakReferenceException(Type type)
        : base(string.Format(ErrorText, type.FullName))
    {
    }

    public DICWeakReferenceException(Type type, Exception innerException)
        : base(string.Format(ErrorText, type.FullName), innerException)
    {
    }
    protected DICWeakReferenceException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
[Serializable]

public class DICConstructorResolutionException : Exception
{
    private const string ErrorText = "Unable to resolve constructor for {0} using provided Expression.";

    public DICConstructorResolutionException(Type type)
        : base(string.Format(ErrorText, type.FullName))
    {
    }

    public DICConstructorResolutionException(Type type, Exception innerException)
        : base(string.Format(ErrorText, type.FullName), innerException)
    {
    }

    public DICConstructorResolutionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DICConstructorResolutionException(string message)
        : base(message)
    {
    }
    protected DICConstructorResolutionException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
[Serializable]

public class DICAutoRegistrationException : Exception
{
    private const string ErrorText = "Duplicate implementation of type {0} found ({1}).";

    public DICAutoRegistrationException(Type registerType, IEnumerable<Type> types)
        : base(string.Format(ErrorText, registerType, GetTypesString(types)))
    {
    }

    public DICAutoRegistrationException(Type registerType, IEnumerable<Type> types, Exception innerException)
        : base(string.Format(ErrorText, registerType, GetTypesString(types)), innerException)
    {
    }
    protected DICAutoRegistrationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    private static string GetTypesString(IEnumerable<Type> types)
    {
        var typeNames = from type in types
            select type.FullName;

        return string.Join(",", typeNames.ToArray());
    }
}