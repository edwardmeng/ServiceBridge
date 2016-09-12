using System;
using System.Collections.Generic;

namespace ServiceBridge
{
    /// <summary>
    /// The generic Service Container interface. This interface is used
    /// to retrieve services (instances identified by type and optional
    /// name) from a container.
    /// </summary>
    public interface IServiceContainer : IServiceProvider
    {
        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <param name="instance">The existing instance to be injected.</param>
        /// <remarks>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para>
        /// </remarks>
        void InjectInstance(object instance);

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
        object GetInstance(Type serviceType, string serviceName = null);

        /// <summary>
        /// Get all instances of the given <paramref name="serviceType"/> currently
        /// registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>.</returns>
        IEnumerable<object> GetAllInstances(Type serviceType);

        /// <summary>
        /// Registers a type mapping with the container. 
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetime">The lifetime strategy of the resolved instances.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        IServiceContainer Register(Type serviceType, Type implementationType, string serviceName = null, ServiceLifetime lifetime = ServiceLifetime.Singleton);

        /// <summary>
        /// Check if a particular type/name pair has been registered with the container. 
        /// </summary>
        /// <param name="serviceType">Type to check registration for.</param>
        /// <param name="serviceName">Name to check registration for.</param>
        /// <returns><c>true</c> if this type/name pair has been registered, <c>false</c> if not.</returns>
        bool IsRegistered(Type serviceType, string serviceName = null);

        /// <summary>
        /// This event is raised when the <see cref="Register"/> method is called. 
        /// </summary>
        event EventHandler<ServiceRegisterEventArgs> Registering;

        /// <summary>
        /// Add an extension object to the container.
        /// </summary>
        /// <param name="extension"><see cref="IServiceContainerExtension"/> to add.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        IServiceContainer AddExtension(IServiceContainerExtension extension);

        /// <summary>
        /// Get access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="extensionType"><see cref="Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        IServiceContainerExtension GetExtension(Type extensionType);
    }
}
