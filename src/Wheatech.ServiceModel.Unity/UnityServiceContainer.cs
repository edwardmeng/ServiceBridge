using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;

namespace Wheatech.ServiceModel.Unity
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> that wraps a Unity container.
    /// </summary>
    public class UnityServiceContainer : ServiceContainerBase
    {
        private IUnityContainer _container;
        private ServiceLifetime _lifetime;
        private InjectionMember[] _injectionMembers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UnityServiceContainer" /> class for a container.
        /// </summary>
        /// <param name="container">
        ///     The <see cref="IUnityContainer" /> to wrap with the <see cref="IServiceContainer" />
        ///     interface implementation.
        /// </param>
        /// <param name="lifetime">
        ///     The <see cref="ServiceLifetime"/> to register type mapping with.
        /// </param>
        /// <param name="injectionMembers">The <see cref="InjectionMember"/>s to register type mapping with.</param>
        public UnityServiceContainer(IUnityContainer container = null, ServiceLifetime lifetime = ServiceLifetime.Singleton, InjectionMember[] injectionMembers = null)
        {
            _container = container ?? new UnityContainer();
            _lifetime = lifetime;
            _injectionMembers = injectionMembers ?? new InjectionMember[0];
            _container.RegisterInstance<IServiceContainer>(this, new ExternallyControlledLifetimeManager());
        }

        /// <summary>
        /// Add an extension object to the container.
        /// </summary>
        /// <param name="extension">UnityContainerExtension to add.</param>
        public void AddUnityExtension(UnityContainerExtension extension)
        {
            _container.AddExtension(extension);
        }

        /// <summary>
        /// Get access to a configuration interface exposed by an extension. 
        /// </summary>
        /// <param name="configurationInterface">Type of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public object GetUnityExtension(Type configurationInterface)
        {
            return _container.Configure(configurationInterface);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_container != null)
                {
                    _container.Dispose();
                    _container = null;
                }
                _injectionMembers = null;
            }
        }

        /// <summary>
        /// Checks if a particular type/name pair has been registered with the container. 
        /// </summary>
        /// <param name="serviceType">Type to check registration for.</param>
        /// <param name="serviceName">Name to check registration for.</param>
        /// <returns><c>true</c> if this type/name pair has been registered, <c>false</c> if not.</returns>
        public override bool IsRegistered(Type serviceType, string serviceName = null)
        {
            if (_container == null)
            {
                throw new ObjectDisposedException("container");
            }
            return _container.IsRegistered(serviceType, serviceName);
        }

        /// <summary>
        /// Resolves the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="serviceName">Name of registered service you want. May be null.</param>
        /// <returns>
        ///     The requested service instance.
        /// </returns>
        protected override object DoGetInstance(Type serviceType, string serviceName)
        {
            if (_container == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            return _container.Resolve(serviceType, serviceName);
        }

        /// <summary>
        /// Resolves all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>
        ///     Sequence of service instance objects.
        /// </returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (_container == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            return from registration in _container.Registrations
                where registration.RegisteredType == serviceType
                select _container.Resolve(registration.RegisteredType, registration.Name);
        }

        /// <summary>
        /// Registers the type mapping.
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        protected override void DoRegister(Type serviceType, Type implementationType, string serviceName)
        {
            if (_container == null)
            {
                throw new ObjectDisposedException("container");
            }
            var args = new UnityServiceRegisterEventArgs(serviceType, implementationType, serviceName)
            {
                Lifetime = _lifetime,
            };
            args.InjectionMembers.AddRange(_injectionMembers);
            OnRegistering(args);
            LifetimeManager lifetimeManager;
            switch (args.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    lifetimeManager = new ContainerControlledLifetimeManager();
                    break;
                case ServiceLifetime.Transient:
                    lifetimeManager = new TransientLifetimeManager();
                    break;
                case ServiceLifetime.PerThread:
                    lifetimeManager = new PerThreadLifetimeManager();
                    break;
                case ServiceLifetime.PerResolve:
                    lifetimeManager = new PerResolveLifetimeManager();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _container.RegisterType(serviceType, implementationType, serviceName, lifetimeManager, args.InjectionMembers.ToArray());
        }
    }
}