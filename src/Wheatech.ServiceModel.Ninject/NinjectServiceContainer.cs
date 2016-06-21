using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Infrastructure;
using Ninject.Planning.Strategies;
using Ninject.Syntax;

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
            _kernel = kernel ?? new StandardKernel(new NinjectSettings { InjectAttribute = typeof(InjectionAttribute) });
            _kernel.Components.RemoveAll<IPlanningStrategy>();
            _kernel.Components.Add<IPlanningStrategy, ConstructorStrategy>();
            _kernel.Components.Add<IPlanningStrategy, PropertyStrategy>();
            _kernel.Components.Add<IPlanningStrategy, MethodStrategy>();
            _kernel.Bind<IServiceContainer>().ToConstant(this);
            _lifetime = lifetime;
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
            var binding = _kernel.Bind(serviceType).To(implementationType);
            IBindingSyntax syntax = binding;
            if (serviceName != null) syntax = binding.Named(serviceName);
            var args = new NinjectServiceRegisterEventArgs(serviceType, implementationType, serviceName, syntax) { Lifetime = _lifetime };
            OnRegistering(args);
            switch (args.Lifetime)
            {
                case ServiceLifetime.Transient:
                    binding.InScope(StandardScopeCallbacks.Transient);
                    break;
                case ServiceLifetime.Singleton:
                    binding.InScope(StandardScopeCallbacks.Singleton);
                    break;
                case ServiceLifetime.PerThread:
                    binding.InScope(StandardScopeCallbacks.Thread);
                    break;
                case ServiceLifetime.PerRequest:
                    binding.InScope(StandardScopeCallbacks.Thread);
                    break;
            }
        }
    }
}
