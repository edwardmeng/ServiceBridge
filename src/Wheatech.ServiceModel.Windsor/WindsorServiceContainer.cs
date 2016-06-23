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

namespace Wheatech.ServiceModel.Windsor
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
            _container.Register(Component.For<IServiceContainer>().Instance(this));
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
            if (serviceName == null)
            {
                return _container.Resolve(serviceType);
            }
            else
            {
                return _container.Resolve(GetServiceName(serviceType, serviceName), serviceType);
            }
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
            DynamicInjectionBuilder.GetOrCreate(instance.GetType(), true, true)(_container.Kernel, instance);
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
                .AddDescriptor(new InjectionComponentDescriptor(implementationType))
                // Enable property injection
                .PropertiesIgnore(property => !InjectionAttribute.Matches(property));
            OnRegistering(new WindsorServiceRegisterEventArgs(serviceType, implementationType, serviceName, lifetime, registration));
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
            _container.Register(registration);
        }

        /// <summary>
        /// Registers an instance to the container.
        /// </summary>
        /// <param name="serviceType">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to be returned.</param>
        public void RegisterInstance(Type serviceType, object instance)
        {
            if (_container == null)
            {
                throw new ObjectDisposedException("container");
            }
            _container.Register(Component.For(serviceType).Instance(instance));
        }

        private string GetServiceName(Type serviceType, string serviceName)
        {
            return ComponentName.DefaultNameFor(serviceType) + (string.IsNullOrEmpty(serviceName) ? null : '.' + serviceName);
        }
    }
}
