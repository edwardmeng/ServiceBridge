using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ServiceBridge.AspNetCore
{
    /// <summary>
    /// Represents an <see cref="IServiceContainer"/> composed of an instance of <see cref="IServiceProvider"/> and an instance of <see cref="IServiceContainer"/>.
    /// </summary>
    internal class CompositeServiceContainer : IServiceContainer
    {
        private readonly IServiceContainer _container;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initialize new instance of <see cref="CompositeServiceContainer"/>.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="serviceProvider"></param>
        public CompositeServiceContainer(IServiceContainer container, IServiceProvider serviceProvider)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            _container = container;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Implementation of <see cref="IServiceProvider.GetService"/>.
        /// </summary>
        /// <param name="serviceType">The requested service.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>The requested object.</returns>
        public object GetService(Type serviceType)
        {
            return GetInstance(serviceType);
        }

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <param name="instance">The existing instance to be injected.</param>
        public void InjectInstance(object instance)
        {
            _container.InjectInstance(instance);
        }

        /// <summary>
        /// Get an instance of the given named <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <param name="serviceName">Name the object was registered with.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>
        /// The requested service instance. 
        /// If the requested type/name has not been registerd, 
        /// returns null for interface or abstract class, 
        /// returns new instance for the other types.
        /// </returns>
        public object GetInstance(Type serviceType, string serviceName = null)
        {
            return _serviceProvider.GetService(serviceType) ?? _container.GetInstance(serviceType, serviceName);
        }

        /// <summary>
        /// Get all instances of the given <paramref name="serviceType"/> currently
        /// registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>.</returns>
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return
                ((IEnumerable) _serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(serviceType)) ?? new object[0]).OfType<object>()
                    .Union(_container.GetAllInstances(serviceType));
        }

        /// <summary>
        /// Registers a type mapping with the container. 
        /// </summary>
        /// <param name="serviceType"><see cref="System.Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="System.Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetime">The lifetime strategy of the resolved instances.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        public IServiceContainer Register(Type serviceType, Type implementationType, string serviceName = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            _container.Register(serviceType, implementationType, serviceName, lifetime);
            return this;
        }

        /// <summary>
        /// Registers a instance mapping with the container. 
        /// </summary>
        /// <param name="serviceType"><see cref="System.Type"/> that will be requested.</param>
        /// <param name="instance">The instance that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        public IServiceContainer RegisterInstance(Type serviceType, object instance, string serviceName = null)
        {
            _container.RegisterInstance(serviceType, instance, serviceName);
            return this;
        }

        /// <summary>
        /// Check if a particular type/name pair has been registered with the container. 
        /// </summary>
        /// <param name="serviceType">Type to check registration for.</param>
        /// <param name="serviceName">Name to check registration for.</param>
        /// <returns><c>true</c> if this type/name pair has been registered, <c>false</c> if not.</returns>
        public bool IsRegistered(Type serviceType, string serviceName = null)
        {
            return _container.IsRegistered(serviceType, serviceName) || _serviceProvider.GetService(serviceType) != null;
        }

        public event EventHandler<ServiceRegisterEventArgs> Registering;

        /// <summary>
        /// Add an extension object to the container.
        /// </summary>
        /// <param name="extension"><see cref="IServiceContainerExtension"/> to add.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        public IServiceContainer AddExtension(IServiceContainerExtension extension)
        {
            _container.AddExtension(extension);
            return this;
        }

        /// <summary>
        /// Get access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="extensionType"><see cref="System.Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public IServiceContainerExtension GetExtension(Type extensionType)
        {
            return _container.GetExtension(extensionType);
        }
    }
}
