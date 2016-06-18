using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Infrastructure;

namespace Wheatech.ServiceModel.Ninject
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> that wraps Ninject.
    /// </summary>
    public class NinjectServiceContainer : ServiceContainerBase
    {
        private IKernel _kernel;
        private readonly ServiceLifetime _lifetime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NinjectServiceContainer" /> class for a container.
        /// </summary>
        /// <param name="kernel">
        ///     The <see cref="IKernel" /> to wrap with the <see cref="IServiceContainer" />
        ///     interface implementation.
        /// </param>
        /// <param name="lifetime">
        ///     The <see cref="lifetime"/> to register type mapping with.
        /// </param>
        public NinjectServiceContainer(IKernel kernel = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            _kernel = kernel ?? new StandardKernel();
            _lifetime = lifetime;
            _kernel.Bind<IServiceContainer>().ToConstant(this);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_kernel != null)
                {
                    _kernel.Dispose();
                    _kernel = null;
                }
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
            if (_kernel == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (serviceName == null)
            {
                return (bool)_kernel.CanResolve(serviceType);
            }
            else
            {
                return (bool)_kernel.CanResolve(serviceType, b => b.Name == serviceName);
            }
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
            if (_kernel == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            return _kernel.Get(serviceType, serviceName);
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
            if (_kernel == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            return _kernel.GetAll(serviceType);
        }

        /// <summary>
        /// Registers the type mapping.
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        protected override void DoRegister(Type serviceType, Type implementationType, string serviceName)
        {
            if (_kernel == null) throw new ObjectDisposedException("container");
            var args = new NinjectServiceRegisterEventArgs(serviceType, implementationType, serviceName) { Lifetime = _lifetime };
            OnRegistering(args);
            var binding = _kernel.Bind(serviceType).To(implementationType);
            if (serviceName != null) binding.Named(serviceName);
            switch (args.Lifetime)
            {
                case ServiceLifetime.Transient:
                    binding.InScope(StandardScopeCallbacks.Transient);
                    break;
                case ServiceLifetime.Singleton:
                    binding.InScope(StandardScopeCallbacks.Singleton);
                    break;
                case ServiceLifetime.Thread:
                    binding.InScope(StandardScopeCallbacks.Thread);
                    break;
            }
        }
    }
}
