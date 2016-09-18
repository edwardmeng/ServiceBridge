using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using ServiceBridge.DynamicInjection;

namespace ServiceBridge.Windsor
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> that wraps Castle Windsor.
    /// </summary>
    public class WindsorServiceContainer : ServiceContainerBase
    {
        private class WindsorKernel : DefaultKernel
        {
            public override ILifestyleManager CreateLifestyleManager(ComponentModel model, IComponentActivator activator)
            {
                if (model.LifestyleType == LifestyleType.PerWebRequest)
                {
                    var manager = new ScopedLifestyleManager(new PerRequestScopeAccessor());
                    manager.Init(activator, this, model);
                    return manager;
                }
                return base.CreateLifestyleManager(model, activator);
            }
        }

        private IWindsorContainer _container;
        private readonly IDictionary<Type, IDictionary<ServiceName, ServiceRegistration>> _registrations =
            new Dictionary<Type, IDictionary<ServiceName, ServiceRegistration>>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindsorServiceContainer" /> class for a container.
        /// </summary>
        /// <param name="container">
        ///     The <see cref="IWindsorContainer" /> to wrap with the <see cref="IServiceContainer" />
        ///     interface implementation.
        /// </param>
        public WindsorServiceContainer(IWindsorContainer container = null)
        {
            _container = container ?? new WindsorContainer(new WindsorKernel(), new DefaultComponentInstaller());
            var modelBuilder = _container.Kernel.ComponentModelBuilder;
            var oldInspector = modelBuilder.Contributors.OfType<ConstructorDependenciesModelInspector>().SingleOrDefault();
            if (oldInspector != null)
            {
                modelBuilder.RemoveContributor(oldInspector);
            }
            // Enable constructor injection.
            modelBuilder.AddContributor(new SelectConstructorInspector());
            RegisterInstance(typeof(IServiceContainer), this);
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
            }
        }

        private bool IsDynamicRegisterd(Type serviceType, string serviceName)
        {
            lock (_registrations)
            {
                IDictionary<ServiceName, ServiceRegistration> registrations;
                return _registrations.TryGetValue(serviceType, out registrations) && registrations.ContainsKey(new ServiceName(serviceName));
            }
        }

        private void AddDynamicRegistration(Type serviceType, string serviceName)
        {
            IDictionary<ServiceName, ServiceRegistration> registrations;
            if (!_registrations.TryGetValue(serviceType, out registrations))
            {
                registrations = new Dictionary<ServiceName, ServiceRegistration>();
                _registrations.Add(serviceType, registrations);
            }
            registrations.Add(new ServiceName(serviceName), new ServiceRegistration(serviceType, serviceType, serviceName));
        }

        /// <summary>
        /// Get an instance of the given named <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <param name="serviceName">Name the object was registered with.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>The requested service instance. If the requested type/name has not been registerd, returns null.</returns>
        public override object GetInstance(Type serviceType, string serviceName)
        {
            if (_container == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (!serviceType.IsInterface && !serviceType.IsAbstract && !IsRegistered(serviceType, serviceName) && !IsDynamicRegisterd(serviceType, serviceName))
            {
                lock (_registrations)
                {
                    if (!IsRegistered(serviceType, serviceName) && !IsDynamicRegisterd(serviceType, serviceName))
                    {
                        // Dynamically register the requesting type to the IWindsorContainer.
                        DoRegister(serviceType, serviceType, serviceName, ServiceLifetime.Transient);
                        // Add the dynamic registration to avoid the second time dynamic register.
                        AddDynamicRegistration(serviceType, serviceName);
                    }
                }
            }
            return base.GetInstance(serviceType, serviceName);
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
            return _container.Resolve(GetServiceName(serviceType, serviceName), serviceType);
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
            return _container.ResolveAll(serviceType).Cast<object>();
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
            var registration = Component.For(serviceType)
                .ImplementedBy(implementationType)
                .Named(GetServiceName(serviceType, serviceName))
                // Enable method injection.
                .AddDescriptor(new InjectionComponentDescriptor(this, implementationType))
                // Enable property injection
                .PropertiesIgnore(property => !InjectionAttribute.Matches(property));
            OnRegistering(new WindsorServiceRegisterEventArgs(serviceType, implementationType, serviceName, lifetime, registration));
            ApplyLifetime(registration, lifetime);
            _container.Register(registration);
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
            var registration = Component.For(serviceType).Instance(instance).Named(GetServiceName(serviceType, serviceName));
            ApplyLifetime(registration, lifetime);
            _container.Register(registration);
        }

        private string GetServiceName(Type serviceType, string serviceName)
        {
            return ComponentName.DefaultNameFor(serviceType) + (string.IsNullOrEmpty(serviceName) ? null : '.' + serviceName);
        }

        private void ApplyLifetime(ComponentRegistration<object> registration, ServiceLifetime? lifetime)
        {
            if (lifetime.HasValue)
            {
                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        registration.LifestyleSingleton();
                        break;
                    case ServiceLifetime.Transient:
                        registration.LifestyleTransient();
                        break;
                    case ServiceLifetime.PerThread:
                        registration.LifestylePerThread();
                        break;
                    case ServiceLifetime.PerRequest:
                        registration.LifestylePerWebRequest();
                        break;
                }
            }
        }
    }
}
