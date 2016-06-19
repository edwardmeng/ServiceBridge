using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Wheatech.ServiceModel.Properties;

namespace Wheatech.ServiceModel
{
    /// <summary>
    /// This class is a helper that provides a default implementation
    /// for most of the methods of <see cref="IServiceContainer"/>.
    /// </summary>
    public abstract class ServiceContainerBase : IServiceContainer, IDisposable
    {
        #region Fields

        private List<IServiceContainerExtension> _extensions = new List<IServiceContainerExtension>();

        private ConcurrentDictionary<Type, ConcurrentDictionary<ServiceName, ServiceRegistration>> _registrations =
            new ConcurrentDictionary<Type, ConcurrentDictionary<ServiceName, ServiceRegistration>>();

        #endregion

        #region ServiceName

        private struct ServiceName
        {
            public ServiceName(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public override string ToString()
            {
                return Name;
            }

            public override int GetHashCode()
            {
                return Name?.GetHashCode() ?? 0;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null)) return false;
                if (obj.GetType() != typeof(ServiceName)) return false;
                return ((ServiceName)obj).Name == Name;
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// Disposing the container also disposes any child containers,
        /// and disposes any instances whose lifetimes are managed
        /// by the container.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Shut FxCop up
        }

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// This class doesn't have a finalizer, so <paramref name="disposing"/> will always be true.</remarks>
        /// <param name="disposing">True if being called from the IDisposable.Dispose
        /// method, false if being called from a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var toRemove = new List<IServiceContainerExtension>(_extensions);
                toRemove.Reverse();
                foreach (var extension in toRemove)
                {
                    extension.Remove(this);
                    (extension as IDisposable)?.Dispose();
                }

                _extensions.Clear();
                _extensions = null;
                _registrations = null;
            }
        }

        #endregion

        #region Get Instance

        /// <summary>
        /// Implementation of <see cref="IServiceProvider.GetService"/>.
        /// </summary>
        /// <param name="serviceType">The requested service.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>The requested object.</returns>
        public virtual object GetService(Type serviceType)
        {
            return GetInstance(serviceType, null);
        }

        /// <summary>
        /// Get an instance of the given named <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <param name="serviceName">Name the object was registered with.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public virtual object GetInstance(Type serviceType, string serviceName)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            try
            {
                return DoGetInstance(serviceType, serviceName);
            }
            catch (Exception ex)
            {
                throw new ActivationException(
                    FormatActivationExceptionMessage(ex, serviceType, serviceName),
                    ex);
            }
        }

        /// <summary>
        /// Get all instances of the given <paramref name="serviceType"/> currently
        /// registered in the container.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>.</returns>
        public virtual IEnumerable<object> GetAllInstances(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            try
            {
                return DoGetAllInstances(serviceType);
            }
            catch (Exception ex)
            {
                throw new ActivationException(
                    FormatActivateAllExceptionMessage(ex, serviceType),
                    ex);
            }
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of resolving
        /// the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="serviceName">Name of registered service you want. May be null.</param>
        /// <returns>The requested service instance.</returns>
        protected abstract object DoGetInstance(Type serviceType, string serviceName);

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of
        /// resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>Sequence of service instance objects.</returns>
        protected virtual IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return GetRegistrations(serviceType).Select(registration => DoGetInstance(registration.ServiceType, registration.ServiceName));
        }

        #endregion

        #region Register

        /// <summary>
        /// Checking if a particular type/name pair has been registered with the container. 
        /// </summary>
        /// <param name="serviceType">Type to check registration for.</param>
        /// <param name="serviceName">Name to check registration for.</param>
        /// <returns><c>true</c> if this type/name pair has been registered, <c>false</c> if not.</returns>
        public virtual bool IsRegistered(Type serviceType, string serviceName = null)
        {
            if (_registrations == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            ConcurrentDictionary<ServiceName, ServiceRegistration> registrations;
            return _registrations.TryGetValue(serviceType, out registrations) && registrations.ContainsKey(new ServiceName(serviceName));
        }

        /// <summary>
        /// Gets all the registrations for the specified service type.
        /// </summary>
        /// <param name="serviceType">Type to check registration for.</param>
        /// <returns>All the registrations for the <paramref name="serviceType"/>.</returns>
        protected virtual IEnumerable<ServiceRegistration> GetRegistrations(Type serviceType)
        {
            if (_registrations == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            ConcurrentDictionary<ServiceName, ServiceRegistration> registrations;
            return _registrations.TryGetValue(serviceType, out registrations) ? registrations.Values : Enumerable.Empty<ServiceRegistration>();
        }

        /// <summary>
        /// Registers a type mapping with the container. 
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        public IServiceContainer Register(Type serviceType, Type implementationType, string serviceName = null)
        {
            if (_registrations == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            try
            {
                DoRegister(serviceType, implementationType, serviceName);
                _registrations
                    .GetOrAdd(serviceType, key => new ConcurrentDictionary<ServiceName, ServiceRegistration>())
                    .GetOrAdd(new ServiceName(serviceName), name => new ServiceRegistration(serviceType, implementationType, serviceName));
                return this;
            }
            catch (Exception ex)
            {
                throw new RegistrationException(FormatRegistrationExceptionMessage(ex, serviceType, implementationType, serviceName), ex);
            }
        }

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of
        /// registering the type mapping.
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        protected abstract void DoRegister(Type serviceType, Type implementationType, string serviceName);

        /// <summary>
        /// This event is raised when the <see cref="IServiceContainer.Register"/> method is called. 
        /// </summary>
        public event EventHandler<ServiceRegisterEventArgs> Registering;

        /// <summary>
        /// Called by the container when the service type is registering.
        /// </summary>
        /// <param name="e">The event argument.</param>
        protected virtual void OnRegistering(ServiceRegisterEventArgs e)
        {
            Registering?.Invoke(this, e);
        }

        #endregion

        #region Extension

        /// <summary>
        /// Add an extension object to the container.
        /// </summary>
        /// <param name="extension"><see cref="IServiceContainerExtension"/> to add.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        public virtual IServiceContainer AddExtension(IServiceContainerExtension extension)
        {
            if (_extensions == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }
            _extensions.Add(extension);
            extension.Initialize(this);
            return this;
        }

        /// <summary>
        /// Get access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="extensionType"><see cref="Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public virtual IServiceContainerExtension GetExtension(Type extensionType)
        {
            if (_extensions == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (extensionType == null)
            {
                throw new ArgumentNullException(nameof(extensionType));
            }
            return _extensions.FirstOrDefault(ex => extensionType.GetTypeInfo().IsAssignableFrom(ex.GetType().GetTypeInfo()));
        }

        #endregion

        #region Message

        /// <summary>
        /// Format the exception message for use in an <see cref="ActivationException"/>
        /// that occurs while resolving a single service.
        /// </summary>
        /// <param name="actualException">The actual exception thrown by the implementation.</param>
        /// <param name="serviceType">Type of service requested.</param>
        /// <param name="serviceName">Name requested.</param>
        /// <returns>The formatted exception message string.</returns>
        protected virtual string FormatActivationExceptionMessage(Exception actualException, Type serviceType, string serviceName)
        {
            return string.Format(CultureInfo.CurrentUICulture, Resources.ActivationExceptionMessage, serviceType.Name, serviceName);
        }

        /// <summary>
        /// Format the exception message for use in an <see cref="ActivationException"/>
        /// that occurs while resolving multiple service instances.
        /// </summary>
        /// <param name="actualException">The actual exception thrown by the implementation.</param>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>The formatted exception message string.</returns>
        protected virtual string FormatActivateAllExceptionMessage(Exception actualException, Type serviceType)
        {
            return string.Format(CultureInfo.CurrentUICulture, Resources.ActivateAllExceptionMessage, serviceType.Name);
        }

        /// <summary>
        /// Format the exception message for use in an <see cref="RegistrationException"/>
        /// that occurs while registering a type mapping.
        /// </summary>
        /// <param name="actualException">The actual exception thrown by the implementation.</param>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        /// <returns>The formatted exception message string.</returns>
        protected virtual string FormatRegistrationExceptionMessage(Exception actualException, Type serviceType, Type implementationType, string serviceName)
        {
            return string.Format(CultureInfo.CurrentUICulture, Resources.RegistrationExceptionMessage, serviceType.Name, implementationType.Name, serviceName);
        }

        #endregion
    }
}
