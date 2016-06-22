using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;

namespace Wheatech.ServiceModel.StructureMap
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> that wraps StructureMap.
    /// </summary>
    public class StructureMapServiceContainer : ServiceContainerBase
    {
        private IContainer _container;
        private readonly ServiceLifetime _lifetime;

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
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StructureMapServiceContainer" /> class for a container.
        /// </summary>
        /// <param name="container">
        ///     The <see cref="IContainer" /> to wrap with the <see cref="IServiceContainer" />
        ///     interface implementation.
        /// </param>
        /// <param name="lifetime">
        ///     The <see cref="ServiceLifetime"/> to register type mapping with.
        /// </param>
        public StructureMapServiceContainer(IContainer container = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            _container = container ?? new Container();
            _lifetime = lifetime;
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
            return string.IsNullOrEmpty(serviceName) ? _container.GetInstance(serviceType) : _container.GetInstance(serviceType, serviceName);
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
            return _container.GetAllInstances(serviceType).Cast<object>();
        }

        /// <summary>
        /// Registers the type mapping.
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        protected override void DoRegister(Type serviceType, Type implementationType, string serviceName)
        {
            _container.Configure(configure =>
            {
                var registry = new Registry();
                var instance = registry.For(serviceType).Use(implementationType);
                if (!string.IsNullOrEmpty(serviceName))
                {
                    instance.Named(serviceName);
                }
                var args = new StructureMapServiceRegisterEventArgs(serviceType, implementationType, serviceName, instance) {Lifetime = _lifetime};
                OnRegistering(args);
                switch (args.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        instance.Singleton();
                        break;
                    case ServiceLifetime.Transient:
                        instance.Transient();
                        break;
                    case ServiceLifetime.PerThread:
                        instance.LifecycleIs<ThreadLocalStorageLifecycle>();
                        break;
                    case ServiceLifetime.PerRequest:
                        //instance.LifecycleIs<>()
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                var methodsInjection = new DynamicInjectionBuilder(implementationType).Build();
                // Enable the method injection
                instance.AddInterceptor(new ActivatorInterceptor<object>((context, x) => methodsInjection(context,x)));
                // Enable the constructor injection
                registry.Policies.ConstructorSelector<InjectionConstructorSelector>();
                // Enable the property injection
                registry.Policies.SetAllProperties(
                    convention =>
                        convention.Matching(
                            property =>
                                property.CanWrite && !(property.SetMethod ?? property.GetMethod).IsStatic && property.GetIndexParameters().Length == 0 &&
                                property.IsDefined(typeof(InjectionAttribute), false)));
                configure.AddRegistry(registry);
            });
        }
    }
}
