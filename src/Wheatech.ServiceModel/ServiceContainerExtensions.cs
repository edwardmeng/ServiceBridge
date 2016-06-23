using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wheatech.ServiceModel
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the <see cref="IServiceContainer"/> interface. 
    /// </summary>
    public static class ServiceContainerExtensions
    {
        #region Get Instance

        /// <summary>
        /// Get an instance of the given named <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <param name="container">Container to resolve through.</param>
        /// <param name="serviceName">Name the object was registered with.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public static TService GetInstance<TService>(this IServiceContainer container, string serviceName = null)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return (TService)container.GetInstance(typeof(TService), serviceName);
        }

        /// <summary>
        /// Get all instances of the given <typeparamref name="TService"/> currently
        /// registered in the container.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <param name="container">Container to resolve through.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>A sequence of instances of the requested <typeparamref name="TService"/>.</returns>
        public static IEnumerable<TService> GetAllInstances<TService>(this IServiceContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            foreach (object item in container.GetAllInstances(typeof(TService)))
            {
                yield return (TService)item;
            }
        }

        #endregion

        #region Register

        /// <summary>
        /// Registers a type mapping with the container. 
        /// </summary>
        /// <param name="container">Container to register with.</param>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetime">The lifetime strategy of the resolved instances.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        public static IServiceContainer Register(this IServiceContainer container, Type serviceType, string serviceName = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.Register(serviceType, serviceType, serviceName, lifetime);
        }

        /// <summary>
        /// Register a type mapping with the container. 
        /// </summary>
        /// <typeparam name="TService"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TImplementation"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to register with.</param>
        /// <param name="serviceName">Name of this mapping.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer Register<TService, TImplementation>(this IServiceContainer container, string serviceName = null)
            where TImplementation : TService
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.Register(typeof(TService), typeof(TImplementation), serviceName);
        }

        /// <summary>
        /// Register a type mapping with the container. 
        /// </summary>
        /// <typeparam name="TService"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TImplementation"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to register with.</param>
        /// <param name="lifetime">The lifetime strategy of the resolved instances.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer Register<TService, TImplementation>(this IServiceContainer container, ServiceLifetime lifetime)
            where TImplementation : TService
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.Register(typeof(TService), typeof(TImplementation), null, lifetime);
        }

        /// <summary>
        /// Register a type mapping with the container. 
        /// </summary>
        /// <typeparam name="TService"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TImplementation"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="container">Container to register with.</param>
        /// <param name="serviceName">Name of this mapping.</param>
        /// <param name="lifetime">The lifetime strategy of the resolved instances.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer Register<TService, TImplementation>(this IServiceContainer container, string serviceName, ServiceLifetime lifetime)
            where TImplementation : TService
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.Register(typeof(TService), typeof(TImplementation), serviceName, lifetime);
        }

        /// <summary>
        /// Register a given type with the container. 
        /// </summary>
        /// <typeparam name="T">The type to be registered.</typeparam>
        /// <param name="container">Container to register with.</param>
        /// <param name="serviceName">Name of this registration.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer Register<T>(this IServiceContainer container, string serviceName = null)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.Register(typeof(T), serviceName);
        }

        /// <summary>
        /// Register a given type with the container. 
        /// </summary>
        /// <typeparam name="T">The type to be registered.</typeparam>
        /// <param name="container">Container to register with.</param>
        /// <param name="lifetime">The lifetime strategy of the resolved instances.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer Register<T>(this IServiceContainer container, ServiceLifetime lifetime)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.Register(typeof(T), null, lifetime);
        }

        /// <summary>
        /// Register a given type with the container. 
        /// </summary>
        /// <typeparam name="T">The type to be registered.</typeparam>
        /// <param name="container">Container to register with.</param>
        /// <param name="serviceName">Name of this registration.</param>
        /// <param name="lifetime">The lifetime strategy of the resolved instances.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer Register<T>(this IServiceContainer container, string serviceName, ServiceLifetime lifetime)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.Register(typeof(T), serviceName, lifetime);
        }

        /// <summary>
        /// Check if a particular type/name pair has been registered with the container. 
        /// </summary>
        /// <typeparam name="T">Type to check registration for.</typeparam>
        /// <param name="container">Container to register with.</param>
        /// <param name="serviceName">Name to check registration for.</param>
        /// <returns><c>true</c> if this type/name pair has been registered, <c>false</c> if not.</returns>
        public static bool IsRegistered<T>(this IServiceContainer container, string serviceName = null)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.IsRegistered(typeof(T), serviceName);
        }

        /// <summary>
        /// Registers types in the supplied assemblies by using the specified rules for name and registration types.
        /// </summary>
        /// <param name="container">The container to configure.</param>
        /// <param name="assemblies">The assemblies to register.</param>
        /// <param name="getFromTypes">A function that gets the types that will be requested for each type to configure.</param>
        /// <param name="getName">A function that gets the name to use for the registration of each type. Defaults to no name.</param>
        /// <param name="overwriteExistingMappings"><see langword="true"/> to overwrite existing mappings; otherwise, <see langword="false"/>. Defaults to <see langword="false"/>.</param>
        /// <returns>
        /// The container that this method was called on.
        /// </returns>
        public static IServiceContainer Register(this IServiceContainer container, IEnumerable<Assembly> assemblies,
            Func<Type, IEnumerable<Type>> getFromTypes = null, Func<Type, string> getName = null, bool overwriteExistingMappings = false)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            return container.Register(assemblies.SelectMany(assembly =>
            {
                IEnumerable<TypeInfo> definedTypes;
                try
                {
                    definedTypes = assembly.DefinedTypes;
                }
                catch (ReflectionTypeLoadException ex)
                {
                    definedTypes = from definedType in ex.Types.TakeWhile(type => type != null) select definedType.GetTypeInfo();
                }
                return from definedType in definedTypes
                       where definedType.IsClass & !definedType.IsAbstract && !definedType.IsValueType && definedType.IsVisible
                       select definedType.AsType();
            }), getFromTypes, getName, overwriteExistingMappings);
        }

        /// <summary>
        /// Registers the supplied types by using the specified rules for name and registration types.
        /// </summary>
        /// <param name="container">The container to configure.</param>
        /// <param name="types">The types to register. </param>
        /// <param name="getFromTypes">A function that gets the types that will be requested for each type to configure.</param>
        /// <param name="getName">A function that gets the name to use for the registration of each type. Defaults to no name.</param>
        /// <param name="overwriteExistingMappings"><see langword="true"/> to overwrite existing mappings; otherwise, <see langword="false"/>. Defaults to <see langword="false"/>.</param>
        /// <returns>
        /// The container that this method was called on.
        /// </returns>
        public static IServiceContainer Register(this IServiceContainer container, IEnumerable<Type> types,
            Func<Type, IEnumerable<Type>> getFromTypes = null, Func<Type, string> getName = null, bool overwriteExistingMappings = false)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (types == null) throw new ArgumentNullException(nameof(types));
            if (getFromTypes == null)
            {
                getFromTypes = type =>
                {
                    var implementedTypes = GetImplementedInterfacesToMap(type).ToArray();
                    if (implementedTypes.Length > 0)
                    {
                        var matchingInterfaceName = "I" + type.Name;
                        var matchingType = implementedTypes.FirstOrDefault(i => string.Equals(i.Name, matchingInterfaceName, StringComparison.Ordinal));
                        return matchingType != null ? new[] { matchingType } : implementedTypes;
                    }
                    return new[] { type };
                };
            }
            if (getName == null)
            {
                getName = type => null;
            }
            foreach (var type in types)
            {
                string name = getName(type);
                foreach (var from in getFromTypes(type))
                {
                    if (overwriteExistingMappings || !container.IsRegistered(from, name))
                    {
                        container.Register(from, type, name);
                    }
                }
            }

            return container;
        }

        private static bool GenericParametersMatch(Type[] parameters, Type[] interfaceArguments)
        {
            if (parameters.Length != interfaceArguments.Length)
            {
                return false;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] != interfaceArguments[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static IEnumerable<Type> FilterMatchingGenericInterfaces(TypeInfo typeInfo)
        {
            var parameters = typeInfo.GenericTypeParameters;

            foreach (var @interface in typeInfo.ImplementedInterfaces)
            {
                var interfaceTypeInfo = @interface.GetTypeInfo();

                if (!(interfaceTypeInfo.IsGenericType && interfaceTypeInfo.ContainsGenericParameters))
                {
                    // skip non generic interfaces, or interfaces without generic parameters
                    continue;
                }

                if (GenericParametersMatch(parameters, interfaceTypeInfo.GenericTypeArguments))
                {
                    yield return interfaceTypeInfo.GetGenericTypeDefinition();
                }
            }
        }

        private static IEnumerable<Type> GetImplementedInterfacesToMap(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            if (!typeInfo.IsGenericType)
            {
                return typeInfo.ImplementedInterfaces;
            }
            if (!typeInfo.IsGenericTypeDefinition)
            {
                return typeInfo.ImplementedInterfaces;
            }
            else
            {
                return FilterMatchingGenericInterfaces(typeInfo);
            }
        }

        #endregion

        #region Extension

        /// <summary>
        /// Creates a new extension object and adds it to the container.
        /// </summary>
        /// <typeparam name="TExtension">Type of <see cref="IServiceContainerExtension"/> to add.</typeparam>
        /// <param name="container">Container to add the extension to.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public static IServiceContainer AddNewExtension<TExtension>(this IServiceContainer container)
            where TExtension : IServiceContainerExtension, new()
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return container.AddExtension(new TExtension());
        }

        /// <summary>
        /// Resolve access to an extension object.
        /// </summary>
        /// <typeparam name="TExtension">Type of <see cref="IServiceContainerExtension"/> required.</typeparam>
        /// <param name="container">Container to resolve extension.</param>
        /// <returns>The requested extension, or null if not found.</returns>
        public static TExtension GetExtension<TExtension>(this IServiceContainer container)
            where TExtension : IServiceContainerExtension
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return (TExtension)container.GetExtension(typeof(TExtension));
        }

        #endregion
    }
}
