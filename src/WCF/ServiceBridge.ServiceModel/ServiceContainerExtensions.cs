using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using ServiceBridge.ServiceModel.Properties;

namespace ServiceBridge.ServiceModel
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the <see cref="IServiceContainer"/> interface. 
    /// </summary>
    public static class ServiceContainerExtensions
    {
        /// <summary>
        /// Register a WCF service type with the <paramref name="container"/>. 
        /// </summary>
        /// <typeparam name="TService">The type of WCF service to be registered.</typeparam>
        /// <param name="container">Container to register with.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="container"/> is null.</exception>
        /// <exception cref="ArgumentException">The <typeparamref name="TService"/> is not a valid WCF service.</exception>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer RegisterWcfService<TService>(this IServiceContainer container)
            where TService : class
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            return container.RegisterWcfService(typeof(TService));
        }

        /// <summary>
        /// Register a WCF service type with the <paramref name="container"/>. 
        /// </summary>
        /// <param name="container">Container to register with.</param>
        /// <param name="serviceType">The type of WCF service to be registered.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="container"/> or <paramref name="serviceType"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="serviceType"/> is not a valid WCF service.</exception>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer RegisterWcfService(this IServiceContainer container, Type serviceType)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (!ServiceUtils.IsValidService(serviceType))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Invalid_Servce_Implementation, serviceType), nameof(serviceType));
            }
            return container.RegisterWcfServiceInternal(serviceType);
        }

        /// <summary>
        /// Register a WCF service type to the <paramref name="container"/> without validation.
        /// </summary>
        /// <param name="container">Container to register with.</param>
        /// <param name="serviceType">The type of WCF service to be registered.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        internal static IServiceContainer RegisterWcfServiceInternal(this IServiceContainer container, Type serviceType)
        {
            var serviceName = ServiceUtils.GetServiceName(serviceType);
            var lifetime = GetServiceLifetime(serviceType);
            foreach (var contractType in GetServiceContracts(serviceType))
            {
                container.Register(contractType, serviceType, serviceName, lifetime);
            }
            return container;
        }

        /// <summary>
        /// Register WCF service types in the <paramref name="assembly"/> with the <paramref name="container"/>. 
        /// </summary>
        /// <param name="container">Container to register with.</param>
        /// <param name="assembly">The assembly contains WCF services.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="container"/> is null.</exception>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer RegisterWcfServices(this IServiceContainer container, Assembly assembly)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            IEnumerable<Type> types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.TakeWhile(type => type != null);
            }
            foreach (var serviceType in types)
            {
                if (ServiceUtils.IsValidService(serviceType))
                {
                    container.RegisterWcfServiceInternal(serviceType);
                }
            }
            return container;
        }

        /// <summary>
        /// Determine the lifetime for the specified service implementation according to the ServiceBehaviorAttribute markup.
        /// </summary>
        /// <param name="serviceType">The type of service implementation.</param>
        /// <returns>The lifetime of the specified service implementation.</returns>
        private static ServiceLifetime GetServiceLifetime(Type serviceType)
        {
            var attribute = (ServiceBehaviorAttribute)Attribute.GetCustomAttribute(serviceType, typeof(ServiceBehaviorAttribute));
            var lifetime = ServiceLifetime.PerThread;
            if (attribute != null)
            {
                switch (attribute.InstanceContextMode)
                {
                    case InstanceContextMode.PerCall:
                        lifetime = ServiceLifetime.Transient;
                        break;
                    case InstanceContextMode.Single:
                        lifetime = ServiceLifetime.Singleton;
                        break;
                    case InstanceContextMode.PerSession:
                        lifetime = ServiceLifetime.PerThread;
                        break;
                }
            }
            return lifetime;
        }

        /// <summary>
        /// Gets all the contract types for the WCF service.
        /// </summary>
        /// <param name="serviceType">The type of WCF service.</param>
        /// <returns>All implemented service contracts.</returns>
        private static IEnumerable<Type> GetServiceContracts(Type serviceType)
        {
            // Enumerate all the implemented interfaces marked with ServiceContractAttribute.
            foreach (var contractType in serviceType.GetInterfaces())
            {
                if (contractType.IsDefined(typeof(ServiceContractAttribute), false))
                {
                    yield return contractType;
                }
            }
            // Enumerate all the inherit hierarchy classes marked with ServiceContractAttribute.
            var contractClass = serviceType;
            while (contractClass != null && contractClass != typeof(object))
            {
                if (contractClass.IsDefined(typeof(ServiceContractAttribute), false))
                {
                    yield return contractClass;
                }
                contractClass = contractClass.BaseType;
            }
        }
    }
}
