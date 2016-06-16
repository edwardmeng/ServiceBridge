using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure;

namespace Wheatech.ServiceModel.Ninject
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> that wraps Ninject.
    /// </summary>
    public class NinjectServiceContainer : ServiceContainerBase, IDisposable
    {
        private IKernel _kernel;
        private Func<IContext, object> _scope;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NinjectServiceContainer" /> class for a container.
        /// </summary>
        /// <param name="kernel">
        ///     The <see cref="IKernel" /> to wrap with the <see cref="IServiceContainer" />
        ///     interface implementation.
        /// </param>
        /// <param name="scope">
        ///     The <see cref="scope"/> to register type mapping with.
        /// </param>
        public NinjectServiceContainer(IKernel kernel = null, Func<IContext, object> scope = null)
        {
            _kernel = kernel ?? new StandardKernel();
            _scope = scope ?? StandardScopeCallbacks.Singleton;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_kernel != null)
            {
                _kernel.Dispose();
                _kernel = null;
            }
            _scope = null;
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
            // key == null must be specifically handled as not asking for a specific keyed instance
            // http://commonservicelocator.codeplex.com/wikipage?title=API%20Reference&referringTitle=Home
            //     The implementation should be designed to expect a null for the string key parameter, 
            //     and MUST interpret this as a request to get the "default" instance for the requested 
            //     type. This meaning of default varies from locator to locator.
            if (serviceName == null)
            {
                return _kernel.Get(serviceType);
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
            if (_kernel == null)
            {
                throw new ObjectDisposedException("container");
            }
            var binding = _kernel.Bind(serviceType).To(implementationType).InScope(_scope);
            if (serviceName != null)
            {
                binding.Named(serviceName);
            }
        }
    }
}
