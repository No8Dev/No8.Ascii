//===============================================================================
// Based on concepts from TinyIoC
//===============================================================================

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace No8.Ascii.DependencyInjection;

public sealed class DependencyInjectionContainer
{
    /// <summary>
    /// Registration options for "fluent" API
    /// </summary>
    public sealed class RegisterOptions
    {
        private readonly DependencyInjectionContainer _container;
        private readonly TypeRegistration _registration;

        public RegisterOptions(DependencyInjectionContainer container, TypeRegistration registration)
        {
            _container    = container;
            _registration = registration;
        }

        /// <summary>
        /// Make registration a singleton (single instance) if possible
        /// </summary>
        /// <returns>RegisterOptions</returns>
        /// <exception cref="DICRegistrationException"></exception>
        public RegisterOptions AsSingleton()
        {
            var currentFactory = _container.GetCurrentFactory(_registration);

            if (currentFactory == null)
                throw new DICRegistrationException(_registration.Type, "singleton");

            return _container.AddUpdateRegistration(_registration, currentFactory.SingletonVariant);
        }

        /// <summary>
        /// Make registration multi-instance if possible
        /// </summary>
        /// <returns>RegisterOptions</returns>
        /// <exception cref="DICRegistrationException"></exception>
        public RegisterOptions AsMultiInstance()
        {
            var currentFactory = _container.GetCurrentFactory(_registration);

            if (currentFactory == null)
                throw new DICRegistrationException(_registration.Type, "multi-instance");

            return _container.AddUpdateRegistration(_registration, currentFactory.MultiInstanceVariant);
        }

        /// <summary>
        /// Make registration hold a weak reference if possible
        /// </summary>
        /// <returns>RegisterOptions</returns>
        /// <exception cref="DICRegistrationException"></exception>
        public RegisterOptions WithWeakReference()
        {
            var currentFactory = _container.GetCurrentFactory(_registration);

            if (currentFactory == null)
                throw new DICRegistrationException(_registration.Type, "weak reference");

            return _container.AddUpdateRegistration(_registration, currentFactory.WeakReferenceVariant);
        }

        /// <summary>
        /// Make registration hold a strong reference if possible
        /// </summary>
        /// <returns>RegisterOptions</returns>
        /// <exception cref="DICRegistrationException"></exception>
        public RegisterOptions WithStrongReference()
        {
            var currentFactory = _container.GetCurrentFactory(_registration);

            if (currentFactory == null)
                throw new DICRegistrationException(_registration.Type, "strong reference");

            return _container.AddUpdateRegistration(_registration, currentFactory.StrongReferenceVariant);
        }

        public RegisterOptions UsingConstructor<TRegisterType>(Expression<Func<TRegisterType>> constructor)
        {
            if (!IsValidAssignment(_registration.Type, typeof(TRegisterType)))
                throw new DICConstructorResolutionException(typeof(TRegisterType));

            var lambda = constructor as LambdaExpression;
            if (lambda == null)
                throw new DICConstructorResolutionException(typeof(TRegisterType));

            var newExpression = lambda.Body as NewExpression;
            if (newExpression == null)
                throw new DICConstructorResolutionException(typeof(TRegisterType));

            var constructorInfo = newExpression.Constructor;
            if (constructorInfo == null)
                throw new DICConstructorResolutionException(typeof(TRegisterType));

            var currentFactory = _container.GetCurrentFactory(_registration);
            if (currentFactory == null)
                throw new DICConstructorResolutionException(typeof(TRegisterType));

            currentFactory.SetConstructor(constructorInfo);

            return this;
        }

        /// <summary>
        /// Switches to a custom lifetime manager factory if possible.
        /// 
        /// Usually used for RegisterOptions "To*" extension methods such as the ASP.Net per-request one.
        /// </summary>
        /// <param name="instance">RegisterOptions instance</param>
        /// <param name="lifetimeProvider">Custom lifetime manager</param>
        /// <param name="errorString">Error string to display if switch fails</param>
        /// <returns>RegisterOptions</returns>
        public static RegisterOptions ToCustomLifetimeManager(RegisterOptions instance, IDICObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance), "instance is null.");

            if (lifetimeProvider == null)
                throw new ArgumentNullException(nameof(lifetimeProvider), "lifetimeProvider is null.");

            if (string.IsNullOrEmpty(errorString))
                throw new ArgumentException("errorString is null or empty.", nameof(errorString));

            var currentFactory = instance._container.GetCurrentFactory(instance._registration);

            if (currentFactory == null)
                throw new DICRegistrationException(instance._registration.Type, errorString);

            return instance._container.AddUpdateRegistration(instance._registration, currentFactory.GetCustomObjectLifetimeVariant(lifetimeProvider, errorString));
        }
    }

    /// <summary>
    /// Registration options for "fluent" API when registering multiple implementations
    /// </summary>
    public sealed class MultiRegisterOptions
    {
        private IEnumerable<RegisterOptions> _registerOptions;

        /// <summary>
        /// Initializes a new instance of the MultiRegisterOptions class.
        /// </summary>
        /// <param name="registerOptions">Registration options</param>
        public MultiRegisterOptions(IEnumerable<RegisterOptions> registerOptions)
        {
            _registerOptions = registerOptions;
        }

        /// <summary>
        /// Make registration a singleton (single instance) if possible
        /// </summary>
        /// <returns>RegisterOptions</returns>
        /// <exception cref="DICRegistrationException"></exception>
        public MultiRegisterOptions AsSingleton()
        {
            _registerOptions = ExecuteOnAllRegisterOptions(ro => ro.AsSingleton());
            return this;
        }

        /// <summary>
        /// Make registration multi-instance if possible
        /// </summary>
        /// <returns>MultiRegisterOptions</returns>
        /// <exception cref="DICRegistrationException"></exception>
        public MultiRegisterOptions AsMultiInstance()
        {
            _registerOptions = ExecuteOnAllRegisterOptions(ro => ro.AsMultiInstance());
            return this;
        }

        /// <summary>
        /// Switches to a custom lifetime manager factory if possible.
        /// 
        /// Usually used for RegisterOptions "To*" extension methods such as the ASP.Net per-request one.
        /// </summary>
        /// <param name="instance">MultiRegisterOptions instance</param>
        /// <param name="lifetimeProvider">Custom lifetime manager</param>
        /// <param name="errorString">Error string to display if switch fails</param>
        /// <returns>MultiRegisterOptions</returns>
        public static MultiRegisterOptions ToCustomLifetimeManager(
            MultiRegisterOptions           instance,
            IDICObjectLifetimeProvider lifetimeProvider,
            string                         errorString)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance), "instance is null.");

            if (lifetimeProvider == null)
                throw new ArgumentNullException(nameof(lifetimeProvider), "lifetimeProvider is null.");

            if (string.IsNullOrEmpty(errorString))
                throw new ArgumentException("errorString is null or empty.", nameof(errorString));

            instance._registerOptions = instance.ExecuteOnAllRegisterOptions(ro => RegisterOptions.ToCustomLifetimeManager(ro, lifetimeProvider, errorString));

            return instance;
        }

        private IEnumerable<RegisterOptions> ExecuteOnAllRegisterOptions(Func<RegisterOptions, RegisterOptions> action)
        {
            var newRegisterOptions = new List<RegisterOptions>();

            foreach (var registerOption in _registerOptions)
            {
                newRegisterOptions.Add(action(registerOption));
            }

            return newRegisterOptions;
        }
    }
        

        
        
    public DependencyInjectionContainer GetChildContainer()
    {
        return new DependencyInjectionContainer(this);
    }
        

        
    /// <summary>
    /// Attempt to automatically register all non-generic classes and interfaces in the current app domain.
    /// 
    /// If more than one class implements an interface then only one implementation will be registered
    /// although no error will be thrown.
    /// </summary>
    public void AutoRegister()
    {
        AutoRegisterInternal(AppDomain.CurrentDomain.GetAssemblies().Where(a => !IsIgnoredAssembly(a)), DuplicateImplementationActions.RegisterSingle, null);
    }

    /// <summary>
    /// Attempt to automatically register all non-generic classes and interfaces in the current app domain.
    /// Types will only be registered if they pass the supplied registration predicate.
    /// 
    /// If more than one class implements an interface then only one implementation will be registered
    /// although no error will be thrown.
    /// </summary>
    /// <param name="registrationPredicate">Predicate to determine if a particular type should be registered</param>
    public void AutoRegister(Func<Type, bool> registrationPredicate)
    {
        AutoRegisterInternal(AppDomain.CurrentDomain.GetAssemblies().Where(a => !IsIgnoredAssembly(a)), DuplicateImplementationActions.RegisterSingle, registrationPredicate);
    }

    /// <summary>
    /// Attempt to automatically register all non-generic classes and interfaces in the current app domain.
    /// </summary>
    /// <param name="duplicateAction">What action to take when encountering duplicate implementations of an interface/base class.</param>
    /// <exception cref="DICAutoRegistrationException"/>
    public void AutoRegister(DuplicateImplementationActions duplicateAction)
    {
        AutoRegisterInternal(AppDomain.CurrentDomain.GetAssemblies().Where(a => !IsIgnoredAssembly(a)), duplicateAction, null);
    }

    /// <summary>
    /// Attempt to automatically register all non-generic classes and interfaces in the current app domain.
    /// Types will only be registered if they pass the supplied registration predicate.
    /// </summary>
    /// <param name="duplicateAction">What action to take when encountering duplicate implementations of an interface/base class.</param>
    /// <param name="registrationPredicate">Predicate to determine if a particular type should be registered</param>
    /// <exception cref="DICAutoRegistrationException"/>
    public void AutoRegister(DuplicateImplementationActions duplicateAction, Func<Type, bool> registrationPredicate)
    {
        AutoRegisterInternal(AppDomain.CurrentDomain.GetAssemblies().Where(a => !IsIgnoredAssembly(a)), duplicateAction, registrationPredicate);
    }

    /// <summary>
    /// Attempt to automatically register all non-generic classes and interfaces in the specified assemblies
    /// 
    /// If more than one class implements an interface then only one implementation will be registered
    /// although no error will be thrown.
    /// </summary>
    /// <param name="assemblies">Assemblies to process</param>
    public void AutoRegister(IEnumerable<Assembly> assemblies)
    {
        AutoRegisterInternal(assemblies, DuplicateImplementationActions.RegisterSingle, null);
    }

    /// <summary>
    /// Attempt to automatically register all non-generic classes and interfaces in the specified assemblies
    /// Types will only be registered if they pass the supplied registration predicate.
    /// 
    /// If more than one class implements an interface then only one implementation will be registered
    /// although no error will be thrown.
    /// </summary>
    /// <param name="assemblies">Assemblies to process</param>
    /// <param name="registrationPredicate">Predicate to determine if a particular type should be registered</param>
    public void AutoRegister(IEnumerable<Assembly> assemblies, Func<Type, bool> registrationPredicate)
    {
        AutoRegisterInternal(assemblies, DuplicateImplementationActions.RegisterSingle, registrationPredicate);
    }

    /// <summary>
    /// Attempt to automatically register all non-generic classes and interfaces in the specified assemblies
    /// </summary>
    /// <param name="assemblies">Assemblies to process</param>
    /// <param name="duplicateAction">What action to take when encountering duplicate implementations of an interface/base class.</param>
    /// <exception cref="DICAutoRegistrationException"/>
    public void AutoRegister(IEnumerable<Assembly> assemblies, DuplicateImplementationActions duplicateAction)
    {
        AutoRegisterInternal(assemblies, duplicateAction, null);
    }

    /// <summary>
    /// Attempt to automatically register all non-generic classes and interfaces in the specified assemblies
    /// Types will only be registered if they pass the supplied registration predicate.
    /// </summary>
    /// <param name="assemblies">Assemblies to process</param>
    /// <param name="duplicateAction">What action to take when encountering duplicate implementations of an interface/base class.</param>
    /// <param name="registrationPredicate">Predicate to determine if a particular type should be registered</param>
    /// <exception cref="DICAutoRegistrationException"/>
    public void AutoRegister(IEnumerable<Assembly> assemblies, DuplicateImplementationActions duplicateAction, Func<Type, bool> registrationPredicate)
    {
        AutoRegisterInternal(assemblies, duplicateAction, registrationPredicate);
    }

    /// <summary>
    /// Creates/replaces a container class registration with default options.
    /// </summary>
    /// <param name="registerType">Type to register</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register(Type registerType)
    {
        return RegisterInternal(registerType, string.Empty, GetDefaultObjectFactory(registerType, registerType));
    }

    /// <summary>
    /// Creates/replaces a named container class registration with default options.
    /// </summary>
    /// <param name="registerType">Type to register</param>
    /// <param name="name">Name of registration</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register(Type registerType, string name)
    {
        return RegisterInternal(registerType, name, GetDefaultObjectFactory(registerType, registerType));

    }

    /// <summary>
    /// Creates/replaces a container class registration with a given implementation and default options.
    /// </summary>
    /// <param name="registerType">Type to register</param>
    /// <param name="registerImplementation">Type to instantiate that implements RegisterType</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register(Type registerType, Type registerImplementation)
    {
        return RegisterInternal(registerType, string.Empty, GetDefaultObjectFactory(registerType, registerImplementation));
    }

    /// <summary>
    /// Creates/replaces a named container class registration with a given implementation and default options.
    /// </summary>
    /// <param name="registerType">Type to register</param>
    /// <param name="registerImplementation">Type to instantiate that implements RegisterType</param>
    /// <param name="name">Name of registration</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register(Type registerType, Type registerImplementation, string name)
    {
        return RegisterInternal(registerType, name, GetDefaultObjectFactory(registerType, registerImplementation));
    }

    /// <summary>
    /// Creates/replaces a container class registration with a specific, strong referenced, instance.
    /// </summary>
    /// <param name="registerType">Type to register</param>
    /// <param name="instance">Instance of RegisterType to register</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register(Type registerType, object instance)
    {
        return RegisterInternal(registerType, string.Empty, new InstanceFactory(registerType, registerType, instance));
    }

    /// <summary>
    /// Creates/replaces a named container class registration with a specific, strong referenced, instance.
    /// </summary>
    /// <param name="registerType">Type to register</param>
    /// <param name="instance">Instance of RegisterType to register</param>
    /// <param name="name">Name of registration</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register(Type registerType, object instance, string name)
    {
        return RegisterInternal(registerType, name, new InstanceFactory(registerType, registerType, instance));
    }

    /// <summary>
    /// Creates/replaces a container class registration with a specific, strong referenced, instance.
    /// </summary>
    /// <param name="registerType">Type to register</param>
    /// <param name="registerImplementation">Type of instance to register that implements RegisterType</param>
    /// <param name="instance">Instance of RegisterImplementation to register</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register(Type registerType, Type registerImplementation, object instance)
    {
        return RegisterInternal(registerType, string.Empty, new InstanceFactory(registerType, registerImplementation, instance));
    }

    /// <summary>
    /// Creates/replaces a named container class registration with a specific, strong referenced, instance.
    /// </summary>
    /// <param name="registerType">Type to register</param>
    /// <param name="registerImplementation">Type of instance to register that implements RegisterType</param>
    /// <param name="instance">Instance of RegisterImplementation to register</param>
    /// <param name="name">Name of registration</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name)
    {
        return RegisterInternal(registerType, name, new InstanceFactory(registerType, registerImplementation, instance));
    }

    /// <summary>
    /// Creates/replaces a container class registration with a user specified factory
    /// </summary>
    /// <param name="registerType">Type to register</param>
    /// <param name="factory">Factory/lambda that returns an instance of RegisterType</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register(Type registerType, Func<DependencyInjectionContainer, NamedParameterOverloads, object> factory)
    {
        return RegisterInternal(registerType, string.Empty, new DelegateFactory(registerType, factory));
    }

    /// <summary>
    /// Creates/replaces a container class registration with a user specified factory
    /// </summary>
    /// <param name="registerType">Type to register</param>
    /// <param name="factory">Factory/lambda that returns an instance of RegisterType</param>
    /// <param name="name">Name of registration</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register(Type registerType, Func<DependencyInjectionContainer, NamedParameterOverloads, object> factory, string name)
    {
        return RegisterInternal(registerType, name, new DelegateFactory(registerType, factory));
    }

    /// <summary>
    /// Creates/replaces a container class registration with default options.
    /// </summary>
    /// <typeparam name="TRegisterType">Type to register</typeparam>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register<TRegisterType>()
        where TRegisterType : class
    {
        return Register(typeof(TRegisterType));
    }

    /// <summary>
    /// Creates/replaces a named container class registration with default options.
    /// </summary>
    /// <typeparam name="TRegisterType">Type to register</typeparam>
    /// <param name="name">Name of registration</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register<TRegisterType>(string name)
        where TRegisterType : class
    {
        return Register(typeof(TRegisterType), name);
    }

    /// <summary>
    /// Creates/replaces a container class registration with a given implementation and default options.
    /// </summary>
    /// <typeparam name="TRegisterType">Type to register</typeparam>
    /// <typeparam name="TRegisterImplementation">Type to instantiate that implements RegisterType</typeparam>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register<TRegisterType, TRegisterImplementation>()
        where TRegisterType : class
        where TRegisterImplementation : class, TRegisterType
    {
        return Register(typeof(TRegisterType), typeof(TRegisterImplementation));
    }

    /// <summary>
    /// Creates/replaces a named container class registration with a given implementation and default options.
    /// </summary>
    /// <typeparam name="TRegisterType">Type to register</typeparam>
    /// <typeparam name="TRegisterImplementation">Type to instantiate that implements RegisterType</typeparam>
    /// <param name="name">Name of registration</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register<TRegisterType, TRegisterImplementation>(string name)
        where TRegisterType : class
        where TRegisterImplementation : class, TRegisterType
    {
        return Register(typeof(TRegisterType), typeof(TRegisterImplementation), name);
    }

    /// <summary>
    /// Creates/replaces a container class registration with a specific, strong referenced, instance.
    /// </summary>
    /// <typeparam name="TRegisterType">Type to register</typeparam>
    /// <param name="instance">Instance of RegisterType to register</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register<TRegisterType>(TRegisterType instance)
        where TRegisterType : class
    {
        return Register(typeof(TRegisterType), instance);
    }

    /// <summary>
    /// Creates/replaces a named container class registration with a specific, strong referenced, instance.
    /// </summary>
    /// <typeparam name="TRegisterType">Type to register</typeparam>
    /// <param name="instance">Instance of RegisterType to register</param>
    /// <param name="name">Name of registration</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register<TRegisterType>(TRegisterType instance, string name)
        where TRegisterType : class
    {
        return Register(typeof(TRegisterType), instance, name);
    }

    /// <summary>
    /// Creates/replaces a container class registration with a specific, strong referenced, instance.
    /// </summary>
    /// <typeparam name="TRegisterType">Type to register</typeparam>
    /// <typeparam name="TRegisterImplementation">Type of instance to register that implements RegisterType</typeparam>
    /// <param name="instance">Instance of RegisterImplementation to register</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register<TRegisterType, TRegisterImplementation>(TRegisterImplementation instance)
        where TRegisterType : class
        where TRegisterImplementation : class, TRegisterType
    {
        return Register(typeof(TRegisterType), typeof(TRegisterImplementation), instance);
    }

    /// <summary>
    /// Creates/replaces a named container class registration with a specific, strong referenced, instance.
    /// </summary>
    /// <typeparam name="TRegisterType">Type to register</typeparam>
    /// <typeparam name="TRegisterImplementation">Type of instance to register that implements RegisterType</typeparam>
    /// <param name="instance">Instance of RegisterImplementation to register</param>
    /// <param name="name">Name of registration</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register<TRegisterType, TRegisterImplementation>(TRegisterImplementation instance, string name)
        where TRegisterType : class
        where TRegisterImplementation : class, TRegisterType
    {
        return Register(typeof(TRegisterType), typeof(TRegisterImplementation), instance, name);
    }

    /// <summary>
    /// Creates/replaces a container class registration with a user specified factory
    /// </summary>
    /// <typeparam name="TRegisterType">Type to register</typeparam>
    /// <param name="factory">Factory/lambda that returns an instance of RegisterType</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register<TRegisterType>(Func<DependencyInjectionContainer, NamedParameterOverloads, TRegisterType> factory)
        where TRegisterType : class
    {
        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        return Register(typeof(TRegisterType), factory);
    }

    /// <summary>
    /// Creates/replaces a named container class registration with a user specified factory
    /// </summary>
    /// <typeparam name="TRegisterType">Type to register</typeparam>
    /// <param name="factory">Factory/lambda that returns an instance of RegisterType</param>
    /// <param name="name">Name of registration</param>
    /// <returns>RegisterOptions for fluent API</returns>
    public RegisterOptions Register<TRegisterType>(Func<DependencyInjectionContainer, NamedParameterOverloads, TRegisterType> factory, string name)
        where TRegisterType : class
    {
        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        return Register(typeof(TRegisterType), factory, name);
    }

    /// <summary>
    /// Register multiple implementations of a type.
    /// 
    /// Internally this registers each implementation using the full name of the class as its registration name.
    /// </summary>
    /// <typeparam name="TRegisterType">Type that each implementation implements</typeparam>
    /// <param name="implementationTypes">Types that implement RegisterType</param>
    /// <returns>MultiRegisterOptions for the fluent API</returns>
    public MultiRegisterOptions RegisterMultiple<TRegisterType>(IEnumerable<Type> implementationTypes)
    {
        return RegisterMultiple(typeof(TRegisterType), implementationTypes);
    }

    /// <summary>
    /// Register multiple implementations of a type.
    /// 
    /// Internally this registers each implementation using the full name of the class as its registration name.
    /// </summary>
    /// <param name="registrationType">Type that each implementation implements</param>
    /// <param name="implementationTypes">Types that implement RegisterType</param>
    /// <returns>MultiRegisterOptions for the fluent API</returns>
    public MultiRegisterOptions RegisterMultiple(Type registrationType, IEnumerable<Type> implementationTypes)
    {
        if (implementationTypes == null)
            throw new ArgumentNullException(nameof(implementationTypes), "implementationTypes is null.");

        var types = implementationTypes as Type[] ?? implementationTypes.ToArray();
        foreach (var type in types)
            if (!registrationType.IsAssignableFrom(type))
                throw new ArgumentException($"types: The type {registrationType.FullName} is not assignable from {type.FullName}");

        if (types.Length != types.Distinct().Count())
        {
            var queryForDuplicatedTypes =
                types.GroupBy(i => i)
                     .Where(j => j.Count() > 1)
                     .Select(j => j.Key.FullName);

            var fullNamesOfDuplicatedTypes = string.Join(",\n", queryForDuplicatedTypes.ToArray());
            var multipleRegMessage =
                $"types: The same implementation type cannot be specified multiple times for {registrationType.FullName}\n\n{fullNamesOfDuplicatedTypes}";
            throw new ArgumentException(multipleRegMessage);
        }

        var registerOptions = new List<RegisterOptions>();

        foreach (var type in types)
            registerOptions.Add(Register(registrationType, type, type.FullName!));

        return new MultiRegisterOptions(registerOptions);
    }
        

        

    /// <summary>
    /// Remove a container class registration.
    /// </summary>
    /// <typeparam name="TRegisterType">Type to un-register</typeparam>
    /// <returns>true if the registration is successfully found and removed; otherwise, false.</returns>
    public bool Unregister<TRegisterType>()
    {
        return Unregister(typeof(TRegisterType), string.Empty);
    }

    /// <summary>
    /// Remove a named container class registration.
    /// </summary>
    /// <typeparam name="TRegisterType">Type to un-register</typeparam>
    /// <param name="name">Name of registration</param>
    /// <returns>true if the registration is successfully found and removed; otherwise, false.</returns>
    public bool Unregister<TRegisterType>(string name)
    {
        return Unregister(typeof(TRegisterType), name);
    }

    /// <summary>
    /// Remove a container class registration.
    /// </summary>
    /// <param name="registerType">Type to un-register</param>
    /// <returns>true if the registration is successfully found and removed; otherwise, false.</returns>
    public bool Unregister(Type registerType)
    {
        return Unregister(registerType, string.Empty);
    }

    /// <summary>
    /// Remove a named container class registration.
    /// </summary>
    /// <param name="registerType">Type to un-register</param>
    /// <param name="name">Name of registration</param>
    /// <returns>true if the registration is successfully found and removed; otherwise, false.</returns>
    public bool Unregister(Type registerType, string name)
    {
        var typeRegistration = new TypeRegistration(registerType, name);

        return RemoveRegistration(typeRegistration);
    }
        
        
    /// <summary>
    /// Attempts to resolve a type using default options.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public object Resolve(Type resolveType)
    {
        return ResolveInternal(new TypeRegistration(resolveType), NamedParameterOverloads.Default, ResolveOptions.Default)!;
    }

    /// <summary>
    /// Attempts to resolve a type using specified options.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public object Resolve(Type resolveType, ResolveOptions options)
    {
        return ResolveInternal(new TypeRegistration(resolveType), NamedParameterOverloads.Default, options)!;
    }

    /// <summary>
    /// Attempts to resolve a type using default options and the supplied name.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="name">Name of registration</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public object Resolve(Type resolveType, string name)
    {
        return ResolveInternal(new TypeRegistration(resolveType, name), NamedParameterOverloads.Default, ResolveOptions.Default)!;
    }

    /// <summary>
    /// Attempts to resolve a type using supplied options and  name.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="name">Name of registration</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public object Resolve(Type resolveType, string name, ResolveOptions options)
    {
        return ResolveInternal(new TypeRegistration(resolveType, name), NamedParameterOverloads.Default, options)!;
    }

    /// <summary>
    /// Attempts to resolve a type using default options and the supplied constructor parameters.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public object Resolve(Type resolveType, NamedParameterOverloads parameters)
    {
        return ResolveInternal(new TypeRegistration(resolveType), parameters, ResolveOptions.Default)!;
    }

    /// <summary>
    /// Attempts to resolve a type using specified options and the supplied constructor parameters.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public object Resolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options)
    {
        return ResolveInternal(new TypeRegistration(resolveType), parameters, options)!;
    }

    /// <summary>
    /// Attempts to resolve a type using default options and the supplied constructor parameters and name.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="name">Name of registration</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public object Resolve(Type resolveType, string name, NamedParameterOverloads parameters)
    {
        return ResolveInternal(new TypeRegistration(resolveType, name), parameters, ResolveOptions.Default)!;
    }

    /// <summary>
    /// Attempts to resolve a named type using specified options and the supplied constructor parameters.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="name">Name of registration</param>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public object Resolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options)
    {
        return ResolveInternal(new TypeRegistration(resolveType, name), parameters, options)!;
    }

    /// <summary>
    /// Attempts to resolve a type using default options.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public TResolveType Resolve<TResolveType>()
        where TResolveType : class
    {
        return (TResolveType)Resolve(typeof(TResolveType));
    }

    /// <summary>
    /// Attempts to resolve a type using specified options.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="options">Resolution options</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public TResolveType Resolve<TResolveType>(ResolveOptions options)
        where TResolveType : class
    {
        return (TResolveType)Resolve(typeof(TResolveType), options);
    }

    /// <summary>
    /// Attempts to resolve a type using default options and the supplied name.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="name">Name of registration</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public TResolveType Resolve<TResolveType>(string name)
        where TResolveType : class
    {
        return (TResolveType)Resolve(typeof(TResolveType), name);
    }

    /// <summary>
    /// Attempts to resolve a type using supplied options and  name.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="name">Name of registration</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public TResolveType Resolve<TResolveType>(string name, ResolveOptions options)
        where TResolveType : class
    {
        return (TResolveType)Resolve(typeof(TResolveType), name, options);
    }

    /// <summary>
    /// Attempts to resolve a type using default options and the supplied constructor parameters.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public TResolveType Resolve<TResolveType>(NamedParameterOverloads parameters)
        where TResolveType : class
    {
        return (TResolveType)Resolve(typeof(TResolveType), parameters);
    }

    /// <summary>
    /// Attempts to resolve a type using specified options and the supplied constructor parameters.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public TResolveType Resolve<TResolveType>(NamedParameterOverloads parameters, ResolveOptions options)
        where TResolveType : class
    {
        return (TResolveType)Resolve(typeof(TResolveType), parameters, options);
    }

    /// <summary>
    /// Attempts to resolve a type using default options and the supplied constructor parameters and name.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="name">Name of registration</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public TResolveType Resolve<TResolveType>(string name, NamedParameterOverloads parameters)
        where TResolveType : class
    {
        return (TResolveType)Resolve(typeof(TResolveType), name, parameters);
    }

    /// <summary>
    /// Attempts to resolve a named type using specified options and the supplied constructor parameters.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="name">Name of registration</param>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Instance of type</returns>
    /// <exception cref="DICResolutionException">Unable to resolve the type.</exception>
    public TResolveType Resolve<TResolveType>(string name, NamedParameterOverloads parameters, ResolveOptions options)
        where TResolveType : class
    {
        return (TResolveType)Resolve(typeof(TResolveType), name, parameters, options);
    }

    /// <summary>
    /// Attempts to predict whether a given type can be resolved with default options.
    ///
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve(Type resolveType)
    {
        return CanResolveInternal(new TypeRegistration(resolveType), NamedParameterOverloads.Default, ResolveOptions.Default);
    }

    /// <summary>
    /// Attempts to predict whether a given named type can be resolved with default options.
    ///
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="name">Name of registration</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    private bool CanResolve(Type resolveType, string name)
    {
        return CanResolveInternal(new TypeRegistration(resolveType, name), NamedParameterOverloads.Default, ResolveOptions.Default);
    }

    /// <summary>
    /// Attempts to predict whether a given type can be resolved with the specified options.
    ///
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve(Type resolveType, ResolveOptions options)
    {
        return CanResolveInternal(new TypeRegistration(resolveType), NamedParameterOverloads.Default, options);
    }

    /// <summary>
    /// Attempts to predict whether a given named type can be resolved with the specified options.
    ///
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="name">Name of registration</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve(Type resolveType, string name, ResolveOptions options)
    {
        return CanResolveInternal(new TypeRegistration(resolveType, name), NamedParameterOverloads.Default, options);
    }

    /// <summary>
    /// Attempts to predict whether a given type can be resolved with the supplied constructor parameters and default options.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// 
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="parameters">User supplied named parameter overloads</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve(Type resolveType, NamedParameterOverloads parameters)
    {
        return CanResolveInternal(new TypeRegistration(resolveType), parameters, ResolveOptions.Default);
    }

    /// <summary>
    /// Attempts to predict whether a given named type can be resolved with the supplied constructor parameters and default options.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// 
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="name">Name of registration</param>
    /// <param name="parameters">User supplied named parameter overloads</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve(Type resolveType, string name, NamedParameterOverloads parameters)
    {
        return CanResolveInternal(new TypeRegistration(resolveType, name), parameters, ResolveOptions.Default);
    }

    /// <summary>
    /// Attempts to predict whether a given type can be resolved with the supplied constructor parameters options.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// 
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="parameters">User supplied named parameter overloads</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options)
    {
        return CanResolveInternal(new TypeRegistration(resolveType), parameters, options);
    }

    /// <summary>
    /// Attempts to predict whether a given named type can be resolved with the supplied constructor parameters options.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// 
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="name">Name of registration</param>
    /// <param name="parameters">User supplied named parameter overloads</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options)
    {
        return CanResolveInternal(new TypeRegistration(resolveType, name), parameters, options);
    }

    /// <summary>
    /// Attempts to predict whether a given type can be resolved with default options.
    ///
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve<TResolveType>()
        where TResolveType : class
    {
        return CanResolve(typeof(TResolveType));
    }

    /// <summary>
    /// Attempts to predict whether a given named type can be resolved with default options.
    ///
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve<TResolveType>(string name)
        where TResolveType : class
    {
        return CanResolve(typeof(TResolveType), name);
    }

    /// <summary>
    /// Attempts to predict whether a given type can be resolved with the specified options.
    ///
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="options">Resolution options</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve<TResolveType>(ResolveOptions options)
        where TResolveType : class
    {
        return CanResolve(typeof(TResolveType), options);
    }

    /// <summary>
    /// Attempts to predict whether a given named type can be resolved with the specified options.
    ///
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="name">Name of registration</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve<TResolveType>(string name, ResolveOptions options)
        where TResolveType : class
    {
        return CanResolve(typeof(TResolveType), name, options);
    }

    /// <summary>
    /// Attempts to predict whether a given type can be resolved with the supplied constructor parameters and default options.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// 
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="parameters">User supplied named parameter overloads</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve<TResolveType>(NamedParameterOverloads parameters)
        where TResolveType : class
    {
        return CanResolve(typeof(TResolveType), parameters);
    }

    /// <summary>
    /// Attempts to predict whether a given named type can be resolved with the supplied constructor parameters and default options.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// 
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="name">Name of registration</param>
    /// <param name="parameters">User supplied named parameter overloads</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve<TResolveType>(string name, NamedParameterOverloads parameters)
        where TResolveType : class
    {
        return CanResolve(typeof(TResolveType), name, parameters);
    }

    /// <summary>
    /// Attempts to predict whether a given type can be resolved with the supplied constructor parameters options.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// 
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="parameters">User supplied named parameter overloads</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve<TResolveType>(NamedParameterOverloads parameters, ResolveOptions options)
        where TResolveType : class
    {
        return CanResolve(typeof(TResolveType), parameters, options);
    }

    /// <summary>
    /// Attempts to predict whether a given named type can be resolved with the supplied constructor parameters options.
    ///
    /// Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one exists).
    /// All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will fail.
    /// 
    /// Note: Resolution may still fail if user defined factory registrations fail to construct objects when called.
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="name">Name of registration</param>
    /// <param name="parameters">User supplied named parameter overloads</param>
    /// <param name="options">Resolution options</param>
    /// <returns>Bool indicating whether the type can be resolved</returns>
    public bool CanResolve<TResolveType>(string name, NamedParameterOverloads parameters, ResolveOptions options)
        where TResolveType : class
    {
        return CanResolve(typeof(TResolveType), name, parameters, options);
    }

    /// <summary>
    /// Attempts to resolve a type using the default options
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve(Type resolveType, [MaybeNullWhen(false)] out object resolvedType)
    {
        try
        {
            resolvedType = Resolve(resolveType);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the given options
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="options">Resolution options</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve(Type resolveType, ResolveOptions options, [MaybeNullWhen(false)] out object resolvedType)
    {
        try
        {
            resolvedType = Resolve(resolveType, options);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the default options and given name
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="name">Name of registration</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve(Type resolveType, string name, [MaybeNullWhen(false)] out object resolvedType)
    {
        try
        {
            resolvedType = Resolve(resolveType, name);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the given options and name
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="name">Name of registration</param>
    /// <param name="options">Resolution options</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve(Type resolveType, string name, ResolveOptions options, [MaybeNullWhen(false)] out object resolvedType)
    {
        try
        {
            resolvedType = Resolve(resolveType, name, options);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the default options and supplied constructor parameters
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve(Type resolveType, NamedParameterOverloads parameters, [MaybeNullWhen(false)] out object resolvedType)
    {
        try
        {
            resolvedType = Resolve(resolveType, parameters);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the default options and supplied name and constructor parameters
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="name">Name of registration</param>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve(Type resolveType, string name, NamedParameterOverloads parameters, [MaybeNullWhen(false)] out object resolvedType)
    {
        try
        {
            resolvedType = Resolve(resolveType, name, parameters);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the supplied options and constructor parameters
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="options">Resolution options</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options, [MaybeNullWhen(false)] out object resolvedType)
    {
        try
        {
            resolvedType = Resolve(resolveType, parameters, options);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the supplied name, options and constructor parameters
    /// </summary>
    /// <param name="resolveType">Type to resolve</param>
    /// <param name="name">Name of registration</param>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="options">Resolution options</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options, [MaybeNullWhen(false)] out object resolvedType)
    {
        try
        {
            resolvedType = Resolve(resolveType, name, parameters, options);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the default options
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve<TResolveType>([MaybeNullWhen(false)] out TResolveType resolvedType)
        where TResolveType : class
    {
        try
        {
            resolvedType = Resolve<TResolveType>();
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = default;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the given options
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="options">Resolution options</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve<TResolveType>(ResolveOptions options, [MaybeNullWhen(false)] out TResolveType resolvedType)
        where TResolveType : class
    {
        try
        {
            resolvedType = Resolve<TResolveType>(options);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = default;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the default options and given name
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="name">Name of registration</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve<TResolveType>(string name, [MaybeNullWhen(false)] out TResolveType resolvedType)
        where TResolveType : class
    {
        try
        {
            resolvedType = Resolve<TResolveType>(name);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = default;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the given options and name
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="name">Name of registration</param>
    /// <param name="options">Resolution options</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve<TResolveType>(string name, ResolveOptions options, [MaybeNullWhen(false)] out TResolveType resolvedType)
        where TResolveType : class
    {
        try
        {
            resolvedType = Resolve<TResolveType>(name, options);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = default;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the default options and supplied constructor parameters
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve<TResolveType>(NamedParameterOverloads parameters, [MaybeNullWhen(false)] out TResolveType resolvedType)
        where TResolveType : class
    {
        try
        {
            resolvedType = Resolve<TResolveType>(parameters);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = default;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the default options and supplied name and constructor parameters
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="name">Name of registration</param>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve<TResolveType>(string name, NamedParameterOverloads parameters, [MaybeNullWhen(false)] out TResolveType resolvedType)
        where TResolveType : class
    {
        try
        {
            resolvedType = Resolve<TResolveType>(name, parameters);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = default(TResolveType);
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the supplied options and constructor parameters
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="options">Resolution options</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve<TResolveType>(NamedParameterOverloads parameters, ResolveOptions options, [MaybeNullWhen(false)] out TResolveType resolvedType)
        where TResolveType : class
    {
        try
        {
            resolvedType = Resolve<TResolveType>(parameters, options);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = default;
            return false;
        }
    }

    /// <summary>
    /// Attempts to resolve a type using the supplied name, options and constructor parameters
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolve</typeparam>
    /// <param name="name">Name of registration</param>
    /// <param name="parameters">User specified constructor parameters</param>
    /// <param name="options">Resolution options</param>
    /// <param name="resolvedType">Resolved type or default if resolve fails</param>
    /// <returns>True if resolved successfully, false otherwise</returns>
    public bool TryResolve<TResolveType>(string name, NamedParameterOverloads parameters, ResolveOptions options, [MaybeNullWhen(false)] out TResolveType resolvedType)
        where TResolveType : class
    {
        try
        {
            resolvedType = Resolve<TResolveType>(name, parameters, options);
            return true;
        }
        catch (DICResolutionException)
        {
            resolvedType = default;
            return false;
        }
    }

    /// <summary>
    /// Returns all registrations of a type
    /// </summary>
    /// <param name="resolveType">Type to resolveAll</param>
    /// <param name="includeUnnamed">Whether to include un-named (default) registrations</param>
    /// <returns>IEnumerable</returns>
    public IEnumerable<object> ResolveAll(Type resolveType, bool includeUnnamed)
    {
        return ResolveAllInternal(resolveType, includeUnnamed);
    }

    /// <summary>
    /// Returns all registrations of a type, both named and unnamed
    /// </summary>
    /// <param name="resolveType">Type to resolveAll</param>
    /// <returns>IEnumerable</returns>
    public IEnumerable<object> ResolveAll(Type resolveType)
    {
        return ResolveAll(resolveType, true);
    }

    /// <summary>
    /// Returns all registrations of a type
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolveAll</typeparam>
    /// <param name="includeUnnamed">Whether to include un-named (default) registrations</param>
    /// <returns>IEnumerable</returns>
    public IEnumerable<TResolveType> ResolveAll<TResolveType>(bool includeUnnamed)
        where TResolveType : class
    {
        return ResolveAll(typeof(TResolveType), includeUnnamed).Cast<TResolveType>();
    }

    /// <summary>
    /// Returns all registrations of a type, both named and unnamed
    /// </summary>
    /// <typeparam name="TResolveType">Type to resolveAll</typeparam>
    /// <returns>IEnumerable</returns>
    public IEnumerable<TResolveType> ResolveAll<TResolveType>()
        where TResolveType : class
    {
        return ResolveAll<TResolveType>(true);
    }

    /// <summary>
    /// Attempts to resolve all public property dependencies on the given object.
    /// </summary>
    /// <param name="input">Object to "build up"</param>
    public void BuildUp(object input)
    {
        BuildUpInternal(input, ResolveOptions.Default);
    }

    /// <summary>
    /// Attempts to resolve all public property dependencies on the given object using the given resolve options.
    /// </summary>
    /// <param name="input">Object to "build up"</param>
    /// <param name="resolveOptions">Resolve options to use</param>
    public void BuildUp(object input, ResolveOptions resolveOptions)
    {
        BuildUpInternal(input, resolveOptions);
    }
        
        
    /// <summary>
    /// Provides custom lifetime management for ASP.Net per-request lifetimes etc.
    /// </summary>
    public interface IDICObjectLifetimeProvider
    {
        /// <summary>
        /// Gets the stored object if it exists, or null if not
        /// </summary>
        /// <returns>Object instance or null</returns>
        object? GetObject();

        /// <summary>
        /// Store the object
        /// </summary>
        /// <param name="value">Object to store</param>
        void SetObject(object value);

        /// <summary>
        /// Release the object
        /// </summary>
        void ReleaseObject();
    }

    private abstract class ObjectFactoryBase
    {
        /// <summary>
        /// Whether to assume this factory successfully constructs its objects
        /// 
        /// Generally set to true for delegate style factories as CanResolve cannot delve
        /// into the delegates they contain.
        /// </summary>
        public virtual bool AssumeConstruction => false;

        /// <summary>
        /// The type the factory instantiates
        /// </summary>
        public abstract Type CreatesType { get; }

        /// <summary>
        /// Constructor to use, if specified
        /// </summary>
        public ConstructorInfo? Constructor { get; private set; }

        /// <summary>
        /// Create the type
        /// </summary>
        /// <param name="requestedType">Type user requested to be resolved</param>
        /// <param name="container">Container that requested the creation</param>
        /// <param name="parameters">Any user parameters passed</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract object GetObject(Type requestedType, DependencyInjectionContainer container, NamedParameterOverloads parameters, ResolveOptions options);
        public virtual ObjectFactoryBase SingletonVariant                                                                                    => throw new DICRegistrationException(GetType(), "singleton");
        public virtual ObjectFactoryBase MultiInstanceVariant                                                                                => throw new DICRegistrationException(GetType(), "multi-instance");
        public virtual ObjectFactoryBase StrongReferenceVariant                                                                              => throw new DICRegistrationException(GetType(), "strong reference");
        public virtual ObjectFactoryBase WeakReferenceVariant                                                                                => throw new DICRegistrationException(GetType(), "weak reference");
        public virtual ObjectFactoryBase GetCustomObjectLifetimeVariant(IDICObjectLifetimeProvider lifetimeProvider, string errorString) => throw new DICRegistrationException(GetType(), errorString);

        public virtual void SetConstructor(ConstructorInfo constructor)
        {
            Constructor = constructor;
        }

        public virtual ObjectFactoryBase GetFactoryForChildContainer(Type type, DependencyInjectionContainer parent, DependencyInjectionContainer child)
        {
            return this;
        }
    }

    /// <summary>
    /// IObjectFactory that creates new instances of types for each resolution
    /// </summary>
    private class MultiInstanceFactory : ObjectFactoryBase
    {
        private readonly Type _registerType;
        private readonly Type _registerImplementation;
        public override  Type CreatesType => _registerImplementation;

        public MultiInstanceFactory(Type registerType, Type registerImplementation)
        {
            if (registerImplementation.IsAbstract() || registerImplementation.IsInterface())
                throw new DICRegistrationTypeException(registerImplementation, "MultiInstanceFactory");

            if (!IsValidAssignment(registerType, registerImplementation))
                throw new DICRegistrationTypeException(registerImplementation, "MultiInstanceFactory");

            _registerType           = registerType;
            _registerImplementation = registerImplementation;
        }

        public override object GetObject(Type requestedType, DependencyInjectionContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            try
            {
                return container.ConstructType(requestedType, _registerImplementation, Constructor, parameters, options);
            }
            catch (DICResolutionException ex)
            {
                throw new DICResolutionException(_registerType, ex);
            }
        }

        public override ObjectFactoryBase SingletonVariant => new SingletonFactory(_registerType, _registerImplementation);

        public override ObjectFactoryBase GetCustomObjectLifetimeVariant(IDICObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            return new CustomObjectLifetimeFactory(_registerType, _registerImplementation, lifetimeProvider, errorString);
        }

        public override ObjectFactoryBase MultiInstanceVariant => this;
    }

    /// <summary>
    /// IObjectFactory that invokes a specified delegate to construct the object
    /// </summary>
    private class DelegateFactory : ObjectFactoryBase
    {
        private readonly Type _registerType;

        private readonly Func<DependencyInjectionContainer, NamedParameterOverloads, object> _factory;

        public override bool AssumeConstruction => true;

        public override Type CreatesType => _registerType;

        public override object GetObject(Type requestedType, DependencyInjectionContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            // Make the requested type available to the factory function
            parameters = new NamedParameterOverloads(parameters)
                         {
                             ["__requestedType"] = requestedType
                         };

            try
            {
                return _factory.Invoke(container, parameters);
            }
            catch (Exception ex)
            {
                throw new DICResolutionException(_registerType, ex);
            }
        }

        public DelegateFactory(Type registerType, Func<DependencyInjectionContainer, NamedParameterOverloads, object> factory)
        {
            _factory      = factory ?? throw new ArgumentNullException(nameof(factory));
            _registerType = registerType;
        }

        public override ObjectFactoryBase SingletonVariant => new DelegateSingletonFactory(_registerType, _factory);
        public override ObjectFactoryBase WeakReferenceVariant => new WeakDelegateFactory(_registerType, _factory);
        public override ObjectFactoryBase StrongReferenceVariant => this;

        public override void SetConstructor(ConstructorInfo constructor)
        {
            throw new DICConstructorResolutionException("Constructor selection is not possible for delegate factory registrations");
        }
    }

    /// <summary>
    /// IObjectFactory that invokes a specified delegate to construct the object
    /// Holds the delegate using a weak reference
    /// </summary>
    private class WeakDelegateFactory : ObjectFactoryBase
    {
        private readonly Type _registerType;

        private readonly WeakReference _factory;

        public override bool AssumeConstruction => true;

        public override Type CreatesType => _registerType;

        public override object GetObject(Type requestedType, DependencyInjectionContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            if (_factory.Target is not Func<DependencyInjectionContainer, NamedParameterOverloads, object> factory)
                throw new DICWeakReferenceException(_registerType);

            try
            {
                return factory.Invoke(container, parameters);
            }
            catch (Exception ex)
            {
                throw new DICResolutionException(_registerType, ex);
            }
        }

        public WeakDelegateFactory(Type registerType, Func<DependencyInjectionContainer, NamedParameterOverloads, object> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _factory = new WeakReference(factory);

            _registerType = registerType;
        }

        public override ObjectFactoryBase StrongReferenceVariant
        {
            get
            {
                if (_factory.Target is not Func<DependencyInjectionContainer, NamedParameterOverloads, object> factory)
                    throw new DICWeakReferenceException(_registerType);

                return new DelegateFactory(_registerType, factory);
            }
        }

        public override ObjectFactoryBase WeakReferenceVariant => this;

        public override void SetConstructor(ConstructorInfo constructor)
        {
            throw new DICConstructorResolutionException("Constructor selection is not possible for delegate factory registrations");
        }
    }

    /// <summary>
    /// Stores an particular instance to return for a type
    /// </summary>
    private class InstanceFactory : ObjectFactoryBase, IDisposable
    {
        private readonly Type   _registerType;
        private readonly Type   _registerImplementation;
        private readonly object _instance;

        public override bool AssumeConstruction => true;

        public InstanceFactory(Type registerType, Type registerImplementation, object instance)
        {
            if (!IsValidAssignment(registerType, registerImplementation))
                throw new DICRegistrationTypeException(registerImplementation, "InstanceFactory");

            _registerType           = registerType;
            _registerImplementation = registerImplementation;
            _instance               = instance;
        }

        public override Type CreatesType => _registerImplementation;

        public override object GetObject(Type requestedType, DependencyInjectionContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            return _instance;
        }

        public override ObjectFactoryBase MultiInstanceVariant => new MultiInstanceFactory(_registerType, _registerImplementation);
        public override ObjectFactoryBase WeakReferenceVariant => new WeakInstanceFactory(_registerType, _registerImplementation, _instance);
        public override ObjectFactoryBase StrongReferenceVariant => this;

        public override void SetConstructor(ConstructorInfo constructor)
        {
            throw new DICConstructorResolutionException("Constructor selection is not possible for instance factory registrations");
        }

        public void Dispose()
        {
            if (_instance is IDisposable disposable)
                disposable.Dispose();
        }
    }

    /// <summary>
    /// Stores an particular instance to return for a type
    /// 
    /// Stores the instance with a weak reference
    /// </summary>
    private class WeakInstanceFactory : ObjectFactoryBase, IDisposable
    {
        private readonly Type          _registerType;
        private readonly Type          _registerImplementation;
        private readonly WeakReference _instance;

        public WeakInstanceFactory(Type registerType, Type registerImplementation, object instance)
        {
            if (!IsValidAssignment(registerType, registerImplementation))
                throw new DICRegistrationTypeException(registerImplementation, "WeakInstanceFactory");

            _registerType           = registerType;
            _registerImplementation = registerImplementation;
            _instance               = new WeakReference(instance);
        }

        public override Type CreatesType => _registerImplementation;

        public override object GetObject(Type requestedType, DependencyInjectionContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            var instance = _instance.Target;

            if (instance == null)
                throw new DICWeakReferenceException(_registerType);

            return instance;
        }

        public override ObjectFactoryBase MultiInstanceVariant => new MultiInstanceFactory(_registerType, _registerImplementation);
        public override ObjectFactoryBase WeakReferenceVariant => this;

        public override ObjectFactoryBase StrongReferenceVariant
        {
            get
            {
                var instance = _instance.Target;

                if (instance == null)
                    throw new DICWeakReferenceException(_registerType);

                return new InstanceFactory(_registerType, _registerImplementation, instance);
            }
        }

        public override void SetConstructor(ConstructorInfo constructor)
        {
            throw new DICConstructorResolutionException("Constructor selection is not possible for instance factory registrations");
        }

        public void Dispose()
        {
            if (_instance.Target is IDisposable disposable)
                disposable.Dispose();
        }
    }

    /// <summary>
    /// A factory that lazy instantiates a type and always returns the same instance
    /// </summary>
    private class SingletonFactory : ObjectFactoryBase, IDisposable
    {
        private readonly Type    _registerType;
        private readonly Type    _registerImplementation;
        private readonly object  _singletonLock = new();
        private          object? _current;

        public SingletonFactory(Type registerType, Type registerImplementation)
        {
            if (registerImplementation.IsAbstract() || registerImplementation.IsInterface())
                throw new DICRegistrationTypeException(registerImplementation, "SingletonFactory");

            if (!IsValidAssignment(registerType, registerImplementation))
                throw new DICRegistrationTypeException(registerImplementation, "SingletonFactory");

            _registerType           = registerType;
            _registerImplementation = registerImplementation;
        }

        public override Type CreatesType => _registerImplementation;

        public override object GetObject(Type requestedType, DependencyInjectionContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            if (parameters.Count != 0)
                throw new ArgumentException("Cannot specify parameters for singleton types");

            lock (_singletonLock)
            {
                _current ??= container.ConstructType(
                    requestedType,
                    _registerImplementation,
                    Constructor,
                    options);
            }

            return _current;
        }

        public override ObjectFactoryBase SingletonVariant => this;

        public override ObjectFactoryBase GetCustomObjectLifetimeVariant(IDICObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            return new CustomObjectLifetimeFactory(_registerType, _registerImplementation, lifetimeProvider, errorString);
        }

        public override ObjectFactoryBase MultiInstanceVariant => new MultiInstanceFactory(_registerType, _registerImplementation);

        public override ObjectFactoryBase GetFactoryForChildContainer(Type type, DependencyInjectionContainer parent, DependencyInjectionContainer child)
        {
            // We make sure that the singleton is constructed before the child container takes the factory.
            // Otherwise the results would vary depending on whether or not the parent container had resolved
            // the type before the child container does.
            GetObject(type, parent, NamedParameterOverloads.Default, ResolveOptions.Default);
            return this;
        }

        public void Dispose()
        {
            if (_current is IDisposable disposable)
                disposable.Dispose();
        }
    }

    /// <summary>
    /// A factory that lazy instantiates a type using a factory method and after construction
    /// always returns the same instance
    /// </summary>
    private class DelegateSingletonFactory : ObjectFactoryBase, IDisposable
    {
        private readonly Func<DependencyInjectionContainer, NamedParameterOverloads, object> _factory;
        private readonly object                                                  _singletonLock = new();
        private          object?                                                 _instance;

        public DelegateSingletonFactory(Type creatingType, Func<DependencyInjectionContainer, NamedParameterOverloads, object> factory)
        {
            _factory    = factory;
            CreatesType = creatingType;
        }

        public override Type CreatesType { get; }

        public override object GetObject(Type requestedType, DependencyInjectionContainer container, NamedParameterOverloads parameters,
                                         ResolveOptions options)
        {
            if (_instance == null)
            {
                lock (_singletonLock)
                {
                    _instance ??= _factory(container, parameters);
                }
            }

            return _instance;
        }

        public void Dispose()
        {
            if (_instance is IDisposable disposable)
            {
                disposable.Dispose();
                _instance = null;
            }
        }
    }

    /// <summary>
    /// A factory that offloads lifetime to an external lifetime provider
    /// </summary>
    private class CustomObjectLifetimeFactory : ObjectFactoryBase, IDisposable
    {
        private readonly object                         _singletonLock = new();
        private readonly Type                           _registerType;
        private readonly Type                           _registerImplementation;
        private readonly IDICObjectLifetimeProvider _lifetimeProvider;

        public CustomObjectLifetimeFactory(Type registerType, Type registerImplementation, IDICObjectLifetimeProvider lifetimeProvider, string errorMessage)
        {
            if (lifetimeProvider == null)
                throw new ArgumentNullException(nameof(lifetimeProvider), "lifetimeProvider is null.");

            if (!IsValidAssignment(registerType, registerImplementation))
                throw new DICRegistrationTypeException(registerImplementation, "SingletonFactory");

            if (registerImplementation.IsAbstract() || registerImplementation.IsInterface())
                throw new DICRegistrationTypeException(registerImplementation, errorMessage);

            _registerType           = registerType;
            _registerImplementation = registerImplementation;
            _lifetimeProvider       = lifetimeProvider;
        }

        public override Type CreatesType => _registerImplementation;

        public override object GetObject(Type requestedType, DependencyInjectionContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            object? current;

            lock (_singletonLock)
            {
                current = _lifetimeProvider.GetObject();
                if (current == null)
                {
                    current = container.ConstructType(requestedType, _registerImplementation, Constructor, options);
                    _lifetimeProvider.SetObject(current);
                }
            }

            return current;
        }

        public override ObjectFactoryBase SingletonVariant
        {
            get
            {
                _lifetimeProvider.ReleaseObject();
                return new SingletonFactory(_registerType, _registerImplementation);
            }
        }

        public override ObjectFactoryBase MultiInstanceVariant
        {
            get
            {
                _lifetimeProvider.ReleaseObject();
                return new MultiInstanceFactory(_registerType, _registerImplementation);
            }
        }

        public override ObjectFactoryBase GetCustomObjectLifetimeVariant(IDICObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            _lifetimeProvider.ReleaseObject();
            return new CustomObjectLifetimeFactory(_registerType, _registerImplementation, lifetimeProvider, errorString);
        }

        public override ObjectFactoryBase GetFactoryForChildContainer(Type type, DependencyInjectionContainer parent, DependencyInjectionContainer child)
        {
            // We make sure that the singleton is constructed before the child container takes the factory.
            // Otherwise the results would vary depending on whether or not the parent container had resolved
            // the type before the child container does.
            GetObject(type, parent, NamedParameterOverloads.Default, ResolveOptions.Default);
            return this;
        }

        public void Dispose()
        {
            _lifetimeProvider.ReleaseObject();
        }
    }


    static DependencyInjectionContainer()
    {
    }

    /// <summary>
    /// Lazy created Singleton instance of the container for simple scenarios
    /// </summary>
    public static DependencyInjectionContainer Current { get; } = new();


    public sealed class TypeRegistration
    {
        public Type   Type { get; }
        public string Name { get; }

        public TypeRegistration(Type type)
            : this(type, string.Empty)
        {
        }

        public TypeRegistration(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        private bool Equals(TypeRegistration other)
        {
            return Type == other.Type && 
                   Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is TypeRegistration other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Name);
        }

        public static bool operator ==(TypeRegistration? left, TypeRegistration? right) { return Equals(left, right); }
        public static bool operator !=(TypeRegistration? left, TypeRegistration? right) { return !Equals(left, right); }
    }

    private readonly        ConcurrentDictionary<TypeRegistration, ObjectFactoryBase> _registeredTypes;
    private delegate        object ObjectConstructor(params object[] parameters);
    private static readonly ConcurrentDictionary<ConstructorInfo, ObjectConstructor> ObjectConstructorCache = new();
        
    public DependencyInjectionContainer()
    {
        _parent          = null;
        _registeredTypes = new ConcurrentDictionary<TypeRegistration, ObjectFactoryBase>();

        RegisterDefaultTypes();
    }

    private readonly DependencyInjectionContainer? _parent;

    private DependencyInjectionContainer(DependencyInjectionContainer parent)
        : this()
    {
        _parent = parent;
    }
        
    private readonly object _autoRegisterLock = new();
    
    private void AutoRegisterInternal(IEnumerable<Assembly> assemblies, DuplicateImplementationActions duplicateAction, Func<Type, bool>? registrationPredicate)
    {
        lock (_autoRegisterLock)
        {
            var types = assemblies.SelectMany(a => a.SafeGetTypes()).Where(t => !IsIgnoredType(t, registrationPredicate)).ToList();

            var concreteTypes = types
                               .Where(type => type.IsClass() && (type.IsAbstract() == false) && (type != GetType() && (type.DeclaringType != GetType()) && (!type.IsGenericTypeDefinition())))
                               .ToList();

            foreach (var type in concreteTypes)
            {
                try
                {
                    RegisterInternal(type, string.Empty, GetDefaultObjectFactory(type, type));
                }
                catch (MethodAccessException)
                {
                    // Ignore methods we can't access - added for Silverlight
                }
            }

            var abstractInterfaceTypes = from type in types
                where ((type.IsInterface() || type.IsAbstract()) && (type.DeclaringType != GetType()) && (!type.IsGenericTypeDefinition()))
                select type;

            foreach (var type in abstractInterfaceTypes)
            {
                var localType = type;
                var implementations = concreteTypes
                                     .Where(implementationType => localType.IsAssignableFrom(implementationType)).ToArray();

                if (implementations.Skip(1).Any())
                {
                    if (duplicateAction == DuplicateImplementationActions.Fail)
                        throw new DICAutoRegistrationException(type, implementations);

                    if (duplicateAction == DuplicateImplementationActions.RegisterMultiple)
                    {
                        RegisterMultiple(type, implementations);
                    }
                }

                var firstImplementation = implementations.FirstOrDefault();
                if (firstImplementation != null)
                {
                    try
                    {
                        RegisterInternal(type, string.Empty, GetDefaultObjectFactory(type, firstImplementation));
                    }
                    catch (MethodAccessException)
                    {
                        // Ignore methods we can't access - added for Silverlight
                    }
                }
            }
        }
    }

    private bool IsIgnoredAssembly(Assembly assembly)
    {
        // TODO - find a better way to remove "system" assemblies from the auto registration
        var ignoreChecks = new List<Func<Assembly, bool>>()
                           {
                               asm => asm.FullName!.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase),
                               asm => asm.FullName!.StartsWith("System.", StringComparison.OrdinalIgnoreCase),
                               asm => asm.FullName!.StartsWith("System,", StringComparison.OrdinalIgnoreCase),
                               asm => asm.FullName!.StartsWith("CR_ExtUnitTest", StringComparison.OrdinalIgnoreCase),
                               asm => asm.FullName!.StartsWith("mscorlib,", StringComparison.OrdinalIgnoreCase),
                               asm => asm.FullName!.StartsWith("CR_VSTest", StringComparison.OrdinalIgnoreCase),
                               asm => asm.FullName!.StartsWith("DevExpress.CodeRush", StringComparison.OrdinalIgnoreCase),
                               asm => asm.FullName!.StartsWith("XUnit.", StringComparison.OrdinalIgnoreCase),
                           };

        foreach (var check in ignoreChecks)
        {
            if (check(assembly))
                return true;
        }

        return false;
    }

    private bool IsIgnoredType(Type type, Func<Type, bool>? registrationPredicate)
    {
        // TODO - find a better way to remove "system" types from the auto registration
        var ignoreChecks = new List<Func<Type, bool>>()
                           {
                               t => t.FullName!.StartsWith("System.", StringComparison.OrdinalIgnoreCase),
                               t => t.FullName!.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase),
                               t => t.IsPrimitive(),
                               t => (t.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length == 0) && !(t.IsInterface() || t.IsAbstract()),
                           };

        if (registrationPredicate != null)
        {
            ignoreChecks.Add(t => !registrationPredicate(t));
        }

        foreach (var check in ignoreChecks)
        {
            if (check(type))
                return true;
        }

        return false;
    }

    private void RegisterDefaultTypes()
    {
        Register(this);

        // Only register the PubSubMessenger singleton if we are the root container
        if (_parent == null)
            Register<ITinyMessengerHub, TinyMessengerHub>();
    }

    private ObjectFactoryBase? GetCurrentFactory(TypeRegistration registration)
    {
        _registeredTypes.TryGetValue(registration, out var current);

        return current;
    }

    private RegisterOptions RegisterInternal(Type registerType, string name, ObjectFactoryBase factory)
    {
        var typeRegistration = new TypeRegistration(registerType, name);

        return AddUpdateRegistration(typeRegistration, factory);
    }

    private RegisterOptions AddUpdateRegistration(TypeRegistration typeRegistration, ObjectFactoryBase factory)
    {
        _registeredTypes[typeRegistration] = factory;

        return new RegisterOptions(this, typeRegistration);
    }

    private bool RemoveRegistration(TypeRegistration typeRegistration)
    {
        return _registeredTypes.Remove(typeRegistration, out _);
    }

    private ObjectFactoryBase GetDefaultObjectFactory(Type registerType, Type registerImplementation)
    {
        if (registerType.IsInterface() || registerType.IsAbstract())
            return new SingletonFactory(registerType, registerImplementation);

        return new MultiInstanceFactory(registerType, registerImplementation);
    }

    private bool CanResolveInternal(TypeRegistration registration, NamedParameterOverloads parameters, ResolveOptions options)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        var checkType = registration.Type;
        var name      = registration.Name;

        if (_registeredTypes.TryGetValue(new TypeRegistration(checkType, name), out var factory))
        {
            if (factory.AssumeConstruction)
                return true;

            if (factory.Constructor == null)
                return GetBestConstructor(factory.CreatesType, parameters, options) != null;

            return CanConstruct(factory.Constructor, parameters, options);
        }

        if (checkType.IsInterface() && checkType.IsGenericType())
        {
            // if the type is registered as an open generic, then see if the open generic is registered
            if (_registeredTypes.TryGetValue(new TypeRegistration(checkType.GetGenericTypeDefinition(), name), out factory))
            {
                if (factory.AssumeConstruction)
                    return true;

                if (factory.Constructor == null)
                    return GetBestConstructor(factory.CreatesType, parameters, options) != null;
                return CanConstruct(factory.Constructor, parameters, options);
            }
        }

        // Fail if requesting named resolution and settings set to fail if unresolved
        // Or bubble up if we have a parent
        if (!string.IsNullOrEmpty(name) && options.NamedResolutionFailureAction == NamedResolutionFailureActions.Fail)
            return _parent?.CanResolveInternal(registration, parameters, options) ?? false;

        // Attempted unnamed fallback container resolution if relevant and requested
        if (!string.IsNullOrEmpty(name) && options.NamedResolutionFailureAction == NamedResolutionFailureActions.AttemptUnnamedResolution)
        {
            if (_registeredTypes.TryGetValue(new TypeRegistration(checkType), out factory))
            {
                if (factory.AssumeConstruction)
                    return true;

                return GetBestConstructor(factory.CreatesType, parameters, options) != null;
            }
        }

        // Check if type is an automatic lazy factory request
        if (IsAutomaticLazyFactoryRequest(checkType))
            return true;

        // Check if type is an IEnumerable<ResolveType>
        if (IsIEnumerableRequest(registration.Type))
            return true;

        // Attempt unregistered construction if possible and requested
        // If we cant', bubble if we have a parent
        if ((options.UnregisteredResolutionAction == UnregisteredResolutionActions.AttemptResolve) || (checkType.IsGenericType() && options.UnregisteredResolutionAction == UnregisteredResolutionActions.GenericsOnly))
            return (GetBestConstructor(checkType, parameters, options) != null) || 
                   (_parent?.CanResolveInternal(registration, parameters, options) ?? false);

        // Bubble resolution up the container tree if we have a parent
        if (_parent != null)
            return _parent.CanResolveInternal(registration, parameters, options);

        return false;
    }

    private bool IsIEnumerableRequest(Type type)
    {
        if (!type.IsGenericType())
            return false;

        var genericType = type.GetGenericTypeDefinition();

        if (genericType == typeof(IEnumerable<>))
            return true;

        return false;
    }

    private bool IsAutomaticLazyFactoryRequest(Type type)
    {
        if (!type.IsGenericType())
            return false;

        Type genericType = type.GetGenericTypeDefinition();

        // Just a func
        if (genericType == typeof(Func<>))
            return true;

        // 2 parameter func with string as first parameter (name)
        if ((genericType == typeof(Func<,>) && type.GetGenericArguments()[0] == typeof(string)))
            return true;

        // 3 parameter func with string as first parameter (name) and IDictionary<string, object> as second (parameters)
        if ((genericType == typeof(Func<,,>) && type.GetGenericArguments()[0] == typeof(string) && type.GetGenericArguments()[1] == typeof(IDictionary<string, object>)))
            return true;

        return false;
    }

    private ObjectFactoryBase? GetParentObjectFactory(TypeRegistration registration)
    {
        if (_parent == null)
            return null;

        if (_parent._registeredTypes.TryGetValue(registration, out var factory))
            return factory.GetFactoryForChildContainer(registration.Type, _parent, this);

        // Attempt container resolution of open generic
        if (registration.Type.IsGenericType())
        {
            var openTypeRegistration = new TypeRegistration(registration.Type.GetGenericTypeDefinition(),
                registration.Name);

            if (_parent._registeredTypes.TryGetValue(openTypeRegistration, out factory))
                return factory.GetFactoryForChildContainer(registration.Type, _parent, this);

            return _parent.GetParentObjectFactory(registration);
        }

        return _parent.GetParentObjectFactory(registration);
    }

    private object? ResolveInternal(TypeRegistration registration, NamedParameterOverloads parameters, ResolveOptions options)
    {
        // Attempt container resolution
        if (_registeredTypes.TryGetValue(registration, out var factory))
        {
            try
            {
                return factory.GetObject(registration.Type, this, parameters, options);
            }
            catch (DICResolutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DICResolutionException(registration.Type, ex);
            }
        }

        // Attempt container resolution of open generic
        if (registration.Type.IsGenericType())
        {
            var openTypeRegistration = new TypeRegistration(registration.Type.GetGenericTypeDefinition(),
                registration.Name);

            if (_registeredTypes.TryGetValue(openTypeRegistration, out factory))
            {
                try
                {
                    return factory.GetObject(registration.Type, this, parameters, options);
                }
                catch (DICResolutionException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new DICResolutionException(registration.Type, ex);
                }
            }
        }

        // Attempt to get a factory from parent if we can
        var bubbledObjectFactory = GetParentObjectFactory(registration);
        if (bubbledObjectFactory != null)
        {
            try
            {
                return bubbledObjectFactory.GetObject(registration.Type, this, parameters, options);
            }
            catch (DICResolutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DICResolutionException(registration.Type, ex);
            }
        }

        // Fail if requesting named resolution and settings set to fail if unresolved
        if (!string.IsNullOrEmpty(registration.Name) && options.NamedResolutionFailureAction == NamedResolutionFailureActions.Fail)
            throw new DICResolutionException(registration.Type);

        // Attempted unnamed fallback container resolution if relevant and requested
        if (!string.IsNullOrEmpty(registration.Name) && options.NamedResolutionFailureAction == NamedResolutionFailureActions.AttemptUnnamedResolution)
        {
            if (_registeredTypes.TryGetValue(new TypeRegistration(registration.Type, string.Empty), out factory))
            {
                try
                {
                    return factory.GetObject(registration.Type, this, parameters, options);
                }
                catch (DICResolutionException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new DICResolutionException(registration.Type, ex);
                }
            }
        }

        // Attempt to construct an automatic lazy factory if possible
        if (IsAutomaticLazyFactoryRequest(registration.Type))
            return GetLazyAutomaticFactoryRequest(registration.Type);

        if (IsIEnumerableRequest(registration.Type))
            return GetIEnumerableRequest(registration.Type);

        // Attempt unregistered construction if possible and requested
        if ((options.UnregisteredResolutionAction == UnregisteredResolutionActions.AttemptResolve) || (registration.Type.IsGenericType() && options.UnregisteredResolutionAction == UnregisteredResolutionActions.GenericsOnly))
        {
            if (!registration.Type.IsAbstract() && !registration.Type.IsInterface())
                return ConstructType(null, registration.Type, parameters, options);
        }

        // Unable to resolve - throw
        throw new DICResolutionException(registration.Type);
    }

    private object? GetLazyAutomaticFactoryRequest(Type type)
    {
        if (!type.IsGenericType())
            return null;

        var genericType      = type.GetGenericTypeDefinition();
        var genericArguments = type.GetGenericArguments();

        // Just a func
        if (genericType == typeof(Func<>))
        {
            var returnType = genericArguments[0];

            var resolveMethod = typeof(DependencyInjectionContainer).GetMethod("Resolve", new Type[] { });
            resolveMethod = resolveMethod?.MakeGenericMethod(returnType);

            if (resolveMethod != null)
            {
                var resolveCall = Expression.Call(Expression.Constant(this), resolveMethod);

                var resolveLambda = Expression.Lambda(resolveCall).Compile();

                return resolveLambda;
            }

            return null;
        }

        // 2 parameter func with string as first parameter (name)
        if ((genericType == typeof(Func<,>)) && (genericArguments[0] == typeof(string)))
        {
            var returnType = genericArguments[1];

            var resolveMethod = typeof(DependencyInjectionContainer).GetMethod("Resolve", new[] { typeof(string) });
            resolveMethod = resolveMethod?.MakeGenericMethod(returnType);

            if (resolveMethod != null)
            {
                var resolveParameters = new[] { Expression.Parameter(typeof(string), "name") };

                // ReSharper disable once CoVariantArrayConversion
                var resolveCall = Expression.Call(Expression.Constant(this), resolveMethod, resolveParameters);

                var resolveLambda = Expression.Lambda(resolveCall, resolveParameters).Compile();

                return resolveLambda;
            }

            return null;
        }

        // 3 parameter func with string as first parameter (name) and IDictionary<string, object> as second (parameters)
        if ((genericType == typeof(Func<,,>) && type.GetGenericArguments()[0] == typeof(string) && type.GetGenericArguments()[1] == typeof(IDictionary<string, object>)))
        {
            var returnType = genericArguments[2];

            var name       = Expression.Parameter(typeof(string), "name");
            var parameters = Expression.Parameter(typeof(IDictionary<string, object>), "parameters");

            var resolveMethod = typeof(DependencyInjectionContainer).GetMethod("Resolve", new[] { typeof(string), typeof(NamedParameterOverloads) });
            resolveMethod = resolveMethod?.MakeGenericMethod(returnType);
            if (resolveMethod != null)
            {
                var resolveCall = Expression.Call(
                    Expression.Constant(this),
                    resolveMethod,
                    name,
                    Expression.Call(typeof(NamedParameterOverloads), "FromIDictionary", null, parameters));

                var resolveLambda = Expression.Lambda(resolveCall, name, parameters).Compile();

                return resolveLambda;
            }

            return null;
        }

        throw new DICResolutionException(type);
    }

    private object? GetIEnumerableRequest(Type type)
    {
        var genericResolveAllMethod = GetType().GetGenericMethod(BindingFlags.Public | BindingFlags.Instance, "ResolveAll", type.GetGenericArguments(), new[] { typeof(bool) });

        return genericResolveAllMethod?.Invoke(this, new object[] { false });
    }

    private bool CanConstruct(ConstructorInfo ctor, NamedParameterOverloads parameters, ResolveOptions options)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        foreach (var parameter in ctor.GetParameters())
        {
            if (string.IsNullOrEmpty(parameter.Name))
                return false;

            var isParameterOverload = parameters.ContainsKey(parameter.Name);

            if (parameter.ParameterType.IsPrimitive() && !isParameterOverload)
                return false;

            if (!isParameterOverload && !CanResolveInternal(new TypeRegistration(parameter.ParameterType), NamedParameterOverloads.Default, options))
                return false;
        }

        return true;
    }

    private ConstructorInfo? GetBestConstructor(Type type, NamedParameterOverloads parameters, ResolveOptions options)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));

        if (type.IsValueType())
            return null;

        // Get constructors in reverse order based on the number of parameters
        // i.e. be as "greedy" as possible so we satisfy the most amount of dependencies possible
        var ctors = GetTypeConstructors(type);

        foreach (var ctor in ctors)
        {
            if (CanConstruct(ctor, parameters, options))
                return ctor;
        }

        return null;
    }

    private IEnumerable<ConstructorInfo> GetTypeConstructors(Type type)
    {
        var candidateCtors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                 .Where(x => !x.IsPrivate) // Includes internal constructors but not private constructors
                                 .Where(x => !x.IsFamily) // Excludes protected constructors
                                 .ToList();

        var attributeCtors = candidateCtors.Where(x => x.GetCustomAttributes(typeof(DICConstructorAttribute), false).Any())
                                           .ToList();

        if (attributeCtors.Any())
            candidateCtors = attributeCtors;

        return candidateCtors.OrderByDescending(ctor => ctor.GetParameters().Length);
    }

    private object ConstructType(Type? requestedType, Type implementationType, ConstructorInfo? constructor, ResolveOptions options)
    {
        return ConstructType(requestedType, implementationType, constructor, NamedParameterOverloads.Default, options);
    }

    private object ConstructType(Type? requestedType, Type implementationType, NamedParameterOverloads parameters, ResolveOptions options)
    {
        return ConstructType(requestedType, implementationType, null, parameters, options);
    }

    private object ConstructType(Type? requestedType, Type implementationType, ConstructorInfo? constructor, NamedParameterOverloads parameters, ResolveOptions options)
    {
        var typeToConstruct = implementationType;

        if (implementationType.IsGenericTypeDefinition())
        {
            if (requestedType == null || !requestedType.IsGenericType() || !requestedType.GetGenericArguments().Any())
                throw new DICResolutionException(typeToConstruct);

            typeToConstruct = typeToConstruct.MakeGenericType(requestedType.GetGenericArguments());
        }
        if (constructor == null)
        {
            // Try and get the best constructor that we can construct
            // if we can't construct any then get the constructor
            // with the least number of parameters so we can throw a meaningful
            // resolve exception
            constructor = GetBestConstructor(typeToConstruct, parameters, options) ?? GetTypeConstructors(typeToConstruct).LastOrDefault();
        }

        if (constructor == null)
            throw new DICResolutionException(typeToConstruct);

        var ctorParams = constructor.GetParameters();
        var args       = new object[ctorParams.Length];

        for (int parameterIndex = 0; parameterIndex < ctorParams.Length; parameterIndex++)
        {
            var currentParam = ctorParams[parameterIndex];

            try
            {
                if (string.IsNullOrEmpty(currentParam.Name))
                    continue;

                var value = parameters.ContainsKey(currentParam.Name)
                    ? parameters[currentParam.Name]
                    : ResolveInternal(
                        new TypeRegistration(currentParam.ParameterType),
                        NamedParameterOverloads.Default,
                        options);
                if (value != null)
                    args[parameterIndex] = value;
            }
            catch (DICResolutionException ex)
            {
                // If a constructor parameter can't be resolved
                // it will throw, so wrap it and throw that this can't
                // be resolved.
                throw new DICResolutionException(typeToConstruct, ex);
            }
            catch (Exception ex)
            {
                throw new DICResolutionException(typeToConstruct, ex);
            }
        }

        try
        {
            var constructionDelegate = CreateObjectConstructionDelegateWithCache(constructor);
            return constructionDelegate.Invoke(args);
        }
        catch (Exception ex)
        {
            throw new DICResolutionException(typeToConstruct, ex);
        }
    }

    private static ObjectConstructor CreateObjectConstructionDelegateWithCache(ConstructorInfo constructor)
    {
        if (ObjectConstructorCache.TryGetValue(constructor, out var objectConstructor))
            return objectConstructor;

        // We could lock the cache here, but there's no real side
        // effect to two threads creating the same ObjectConstructor
        // at the same time, compared to the cost of a lock for 
        // every creation.
        var constructorParams = constructor.GetParameters();
        var lambdaParams      = Expression.Parameter(typeof(object[]), "parameters");
        var newParams         = new Expression[constructorParams.Length];

        for (int i = 0; i < constructorParams.Length; i++)
        {
            var paramsParameter = Expression.ArrayIndex(lambdaParams, Expression.Constant(i));

            newParams[i] = Expression.Convert(paramsParameter, constructorParams[i].ParameterType);
        }

        var newExpression = Expression.New(constructor, newParams);

        var constructionLambda = Expression.Lambda(typeof(ObjectConstructor), newExpression, lambdaParams);

        objectConstructor = (ObjectConstructor)constructionLambda.Compile();

        ObjectConstructorCache[constructor] = objectConstructor;
        return objectConstructor;
    }

    private void BuildUpInternal(object input, ResolveOptions resolveOptions)
    {
        var properties = from property in input.GetType().GetProperties()
            where (property.GetGetMethod() != null) && (property.GetSetMethod() != null) && !property.PropertyType.IsValueType()
            select property;

        foreach (var property in properties)
        {
            if (property.GetValue(input, null) == null)
            {
                try
                {
                    property.SetValue(input, ResolveInternal(new TypeRegistration(property.PropertyType), NamedParameterOverloads.Default, resolveOptions), null);
                }
                catch (DICResolutionException)
                {
                    // Catch any resolution errors and ignore them
                }
            }
        }
    }

    private IEnumerable<TypeRegistration> GetParentRegistrationsForType(Type resolveType)
    {
        if (_parent == null)
            return new TypeRegistration[] { };

        var registrations = _parent._registeredTypes.Keys.Where(tr => tr.Type == resolveType);

        return registrations.Concat(_parent.GetParentRegistrationsForType(resolveType));
    }

    private IEnumerable<object> ResolveAllInternal(Type resolveType, bool includeUnnamed)
    {
        var registrations = _registeredTypes.Keys.Where(tr => tr.Type == resolveType).Concat(GetParentRegistrationsForType(resolveType)).Distinct();

        if (!includeUnnamed)
            registrations = registrations.Where(tr => tr.Name != string.Empty);

        return registrations
              .Select(
                   registration => ResolveInternal(
                       registration,
                       NamedParameterOverloads.Default,
                       ResolveOptions.Default))
              .OfType<object>();
    }

    private static bool IsValidAssignment(Type registerType, Type registerImplementation)
    {
        if (!registerType.IsGenericTypeDefinition())
        {
            if (!registerType.IsAssignableFrom(registerImplementation))
                return false;
        }
        else
        {
            if (registerType.IsInterface())
            {
                if (!registerImplementation.FindInterfaces((t, _) => t.Name == registerType.Name, null).Any())
                    return false;
            }
            else if (registerType.IsAbstract() && registerImplementation.BaseType() != registerType)
            {
                return false;
            }
        }
        return true;
    }
}



