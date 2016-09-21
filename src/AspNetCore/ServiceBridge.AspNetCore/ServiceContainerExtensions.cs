using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ServiceBridge.AspNetCore
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the <see cref="IServiceContainer"/> interface. 
    /// </summary>
    public static class ServiceContainerExtensions
    {
        /// <summary>
        /// Register services to integrate NetCore dependency injection.
        /// </summary>
        /// <param name="container">Container to register with.</param>
        /// <param name="services">The service collections.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        public static IServiceContainer RegisterServices(this IServiceContainer container, IServiceCollection services)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if(services == null)throw new ArgumentNullException(nameof(services));
            IServiceProvider serviceProvider = null;
            foreach (var descriptor in services)
            {
                if (descriptor.ImplementationInstance != null)
                {
                    container.RegisterInstance(descriptor.ServiceType, descriptor.ImplementationInstance);
                }
                else if (descriptor.ImplementationType != null && !descriptor.ImplementationType.GetTypeInfo().IsGenericTypeDefinition)
                {
                    container.Register(descriptor.ServiceType, descriptor.ImplementationType, lifetime: ConvertLifetime(descriptor));
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    if (serviceProvider == null)
                    {
                        serviceProvider = services.BuildServiceProvider();
                    }
                    container.RegisterInstance(descriptor.ServiceType, descriptor.ImplementationFactory(serviceProvider));
                }
            }
            return container;
        }

        private static ServiceLifetime ConvertLifetime(ServiceDescriptor descriptor)
        {
            switch (descriptor.Lifetime)
            {
                case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton:
                    return ServiceLifetime.Singleton;
                case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped:
                    return ServiceLifetime.PerThread;
                case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient:
                    return ServiceLifetime.Transient;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Register an ASP.NET MVC controller type to the <paramref name="container"/> without validation.
        /// </summary>
        /// <param name="container">Container to register with.</param>
        /// <param name="controllerType">The type of ASP.NET MVC controller to be registered.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        private static IServiceContainer RegisterMvcControllerInternal(this IServiceContainer container, Type controllerType)
        {
            return container.Register(controllerType, null, ServiceLifetime.PerRequest);
        }

        /// <summary>
        /// Returns a value to indicate whethear the <paramref name="controllerType"/> is an ASP.NET MVC controller type.
        /// </summary>
        /// <param name="controllerType">The ASP.NET MVC controller type to be validated.</param>
        /// <returns><c>true</c> if the <paramref name="controllerType"/> is an ASP.NET MVC controller type; otherwise, <c>false</c>.</returns>
        private static bool IsValidController(TypeInfo controllerType)
        {
            return !controllerType.IsInterface && !controllerType.IsAbstract && controllerType.IsClass && typeof(ControllerBase).IsAssignableFrom(controllerType.AsType());
        }

        /// <summary>
        /// Register ASP.NET MVC controller types in the <paramref name="assembly"/> with the <paramref name="container"/>. 
        /// </summary>
        /// <param name="container">Container to register with.</param>
        /// <param name="assembly">The assembly contains ASP.NET MVC controllers.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="container"/> is null.</exception>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer RegisterMvcControllers(this IServiceContainer container, Assembly assembly)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            IEnumerable<TypeInfo> types;
            try
            {
                types = assembly.DefinedTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.TakeWhile(type => type != null).Select(type => type.GetTypeInfo());
            }
            foreach (var type in types)
            {
                if (IsValidController(type))
                {
                    container.RegisterMvcControllerInternal(type.AsType());
                }
            }
            return container;
        }

        /// <summary>
        /// Register an ASP.NET MVC controller type with the <paramref name="container"/>. 
        /// </summary>
        /// <param name="container">Container to register with.</param>
        /// <param name="controllerType">The type of ASP.NET MVC controller to be registered.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="container"/> or <paramref name="controllerType"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="controllerType"/> is not a valid ASP.NET MVC controller.</exception>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer RegisterMvcController(this IServiceContainer container, Type controllerType)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (controllerType == null)
            {
                throw new ArgumentNullException(nameof(controllerType));
            }
            if (!IsValidController(controllerType.GetTypeInfo()))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Invalid_ControllerType, controllerType), nameof(controllerType));
            }
            return container.RegisterMvcControllerInternal(controllerType);
        }

        /// <summary>
        /// Register an ASP.NET MVC controller type with the <paramref name="container"/>. 
        /// </summary>
        /// <typeparam name="TController">The type of ASP.NET MVC controller to be registered.</typeparam>
        /// <param name="container">Container to register with.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="container"/> is null.</exception>
        /// <exception cref="ArgumentException">The <typeparamref name="TController"/> is not a valid ASP.NET MVC controller.</exception>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer RegisterMvcController<TController>(this IServiceContainer container)
            where TController : ControllerBase
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            return container.RegisterMvcController(typeof(TController));
        }
    }
}
