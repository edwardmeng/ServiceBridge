using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;
using ServiceBridge.DynamicInjection;

namespace ServiceBridge.StructureMap
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> that wraps StructureMap.
    /// </summary>
    public class StructureMapServiceContainer : ServiceContainerBase
    {
        private IContainer _container;

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
        public StructureMapServiceContainer(IContainer container = null)
        {
            _container = container ?? new Container();
            RegisterInstance(typeof(IServiceContainer), this);
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
            if (_container == null)
            {
                throw new ObjectDisposedException("container");
            }
            return _container.GetAllInstances(serviceType).Cast<object>();
        }

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <param name="instance">The existing instance to be injected.</param>
        protected override void DoInjectInstance(object instance)
        {
            if (_container == null)
            {
                throw new ObjectDisposedException("container");
            }
            DynamicInjectionBuilder.GetOrCreate(instance.GetType(), true, true)(this, instance);
        }

        /// <summary>
        /// Registers the type mapping.
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetime">The lifetime strategy of the resolved instances.</param>
        protected override void DoRegister(Type serviceType, Type implementationType, string serviceName, ServiceLifetime lifetime)
        {
            if (_container == null)
            {
                throw new ObjectDisposedException("container");
            }
            _container.Configure(configure =>
            {
                var registry = new Registry();
                var instance = registry.For(serviceType).Use(implementationType);
                if (!string.IsNullOrEmpty(serviceName))
                {
                    instance.Named(serviceName);
                }
                OnRegistering(new StructureMapServiceRegisterEventArgs(serviceType, implementationType, serviceName, lifetime, instance));
                ApplyLifetime(instance, lifetime);
                // Enable the method injection
                instance.AddInterceptor(new ActivatorInterceptor<object>((context, x) => DynamicInjectionBuilder.GetOrCreate(implementationType, false, true)(this, x)));
                // Enable the constructor injection
                registry.Policies.ConstructorSelector<InjectionConstructorSelector>();
                // Enable the property injection
                registry.Policies.SetAllProperties(convention => convention.Matching(InjectionAttribute.Matches));
                configure.AddRegistry(registry);
            });
        }

        /// <summary>
        /// Registering the instance mapping.
        /// </summary>
        /// <param name="serviceType"><see cref="System.Type"/> that will be requested.</param>
        /// <param name="instance">The instance that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetime">The lifetime strategy of the resolved instances.</param>
        protected override void DoRegisterInstance(Type serviceType, object instance, string serviceName, ServiceLifetime? lifetime)
        {
            _container.Configure(configure =>
            {
                var objectInstance = configure.For(serviceType).Add(instance);
                if (!string.IsNullOrEmpty(serviceName))
                {
                    objectInstance.Named(serviceName);
                }
                ApplyLifetime(objectInstance, lifetime);
            });
        }

        private void ApplyLifetime<T>(ExpressedInstance<T> instance, ServiceLifetime? lifetime)
        {
            if (lifetime.HasValue)
            {
                switch (lifetime.Value)
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
                        instance.LifecycleIs<PerRequestLifecycle>();
                        break;
                }
            }
        }
    }
}
