using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace Wheatech.ServiceModel.Unity
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> that wraps a Unity container.
    /// </summary>
    public class UnityServiceContainer : ServiceContainerBase, IDisposable
    {
        private IUnityContainer _container;
        private LifetimeManager _lifetimeManager;
        private InjectionMember[] _injectionMembers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UnityServiceContainer" /> class for a container.
        /// </summary>
        /// <param name="container">
        ///     The <see cref="IUnityContainer" /> to wrap with the <see cref="IServiceContainer" />
        ///     interface implementation.
        /// </param>
        /// <param name="lifetimeManager">
        ///     The <see cref="LifetimeManager"/> to register type mapping with.
        /// </param>
        /// <param name="injectionMembers">The <see cref="InjectionMember"/>s to register type mapping with.</param>
        public UnityServiceContainer(IUnityContainer container = null, LifetimeManager lifetimeManager = null, InjectionMember[] injectionMembers = null)
        {
            _container = container ?? new UnityContainer();
            _lifetimeManager = lifetimeManager ?? CreateDefaultInstanceLifetimeManager();
            _injectionMembers = injectionMembers ?? new InjectionMember[0];
            container.RegisterInstance<IServiceContainer>(this, new ExternallyControlledLifetimeManager());
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }
            _lifetimeManager = null;
            _injectionMembers = null;
        }

        private static LifetimeManager CreateDefaultInstanceLifetimeManager()
        {
            return new ContainerControlledLifetimeManager();
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

            return _container.ResolveAll(serviceType);
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
            _container.RegisterType(serviceType, implementationType, serviceName, _lifetimeManager, _injectionMembers);
        }
    }
}