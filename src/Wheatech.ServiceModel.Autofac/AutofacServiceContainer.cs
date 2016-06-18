using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;

namespace Wheatech.ServiceModel.Autofac
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> that wraps Autofac.
    /// </summary>
    public class AutofacServiceContainer : ServiceContainerBase
    {
        private Dictionary<Tuple<Type, string>, Type> _registrations = new Dictionary<Tuple<Type, string>, Type>();
        private ContainerBuilder _builder;
        private IContainer _container;
        private readonly ServiceLifetime _lifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceContainer" /> class.
        /// </summary>
        public AutofacServiceContainer(ContainerBuilder builder = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            _lifetime = lifetime;
            _builder = builder ?? new ContainerBuilder();
            _builder.RegisterInstance(this).As<IServiceContainer>().ExternallyOwned();
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
                _builder = null;
                _registrations = null;
            }
        }

        private IContainer EnsureContainer()
        {
            if (_builder == null)
            {
                throw new ObjectDisposedException("container");
            }
            return _container ?? (_container = _builder.Build());
        }

        /// <summary>
        /// Checks if a particular type/name pair has been registered with the container. 
        /// </summary>
        /// <param name="serviceType">Type to check registration for.</param>
        /// <param name="serviceName">Name to check registration for.</param>
        /// <returns><c>true</c> if this type/name pair has been registered, <c>false</c> if not.</returns>
        public override bool IsRegistered(Type serviceType, string serviceName = null)
        {
            if (_builder == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (_container == null)
            {
                return _registrations.ContainsKey(Tuple.Create(serviceType, serviceName));
            }
            return serviceName == null ? _container.IsRegistered(serviceType) : _container.IsRegisteredWithName(serviceName, serviceType);
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
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            var container = EnsureContainer();
            return serviceName != null ? container.ResolveNamed(serviceName, serviceType) : container.Resolve(serviceType);
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
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            var container = EnsureContainer();
            return from registration in container.ComponentRegistry.Registrations
                where (from service in registration.Services
                    where (service as TypedService)?.ServiceType == serviceType || (service as KeyedService)?.ServiceType == serviceType
                    select service).Any()
                select container.ResolveComponent(registration, Enumerable.Empty<Parameter>());
        }

        /// <summary>
        /// Registers the type mapping.
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        protected override void DoRegister(Type serviceType, Type implementationType, string serviceName)
        {
            if (_builder == null)
            {
                throw new ObjectDisposedException("container");
            }
            var registration = serviceName == null
                ? _builder.RegisterType(implementationType).As(serviceType)
                : _builder.RegisterType(implementationType).Named(serviceName, serviceType);
            var args = new AutofacServiceRegisterEventArgs(serviceType, implementationType, serviceName, registration) { Lifetime = _lifetime };
            OnRegistering(args);
            switch (args.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    registration.SingleInstance();
                    break;
                case ServiceLifetime.PerDependency:
                    registration.InstancePerDependency();
                    break;
                case ServiceLifetime.PerLifetimeScope:
                    registration.InstancePerLifetimeScope();
                    break;
                case ServiceLifetime.PerRequest:
                    registration.InstancePerRequest();
                    break;
            }
            _registrations[Tuple.Create(serviceType, serviceName)] = implementationType;
        }
    }
}
