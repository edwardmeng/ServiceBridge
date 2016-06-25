using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Wheatech.ServiceModel.DynamicInjection;

namespace Wheatech.ServiceModel.Autofac
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> that wraps Autofac.
    /// </summary>
    public class AutofacServiceContainer : ServiceContainerBase
    {
        private ContainerBuilder _builder;
        private IContainer _container;
        private readonly object _lockobj = new object();

        private readonly IDictionary<Type, IDictionary<ServiceName, ServiceRegistration>> _registrations =
            new Dictionary<Type, IDictionary<ServiceName, ServiceRegistration>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceContainer" /> class.
        /// </summary>
        public AutofacServiceContainer(ContainerBuilder builder = null)
        {
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
            }
        }

        private IContainer EnsureContainer()
        {
            if (_builder == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (_container == null)
            {
                Thread.MemoryBarrier();
                lock (_lockobj)
                {
                    if (_container == null)
                    {
                        _container = _builder.Build();
                    }
                }
            }
            return _container;
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
            if (_builder == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (!serviceType.IsInterface && !serviceType.IsAbstract && !IsRegistered(serviceType, serviceName) && !IsDynamicRegisterd(serviceType,serviceName))
            {
                lock (_registrations)
                {
                    if (!IsRegistered(serviceType, serviceName) && !IsDynamicRegisterd(serviceType, serviceName))
                    {
                        if (_container == null)
                        {
                            // Dynamically register the requesting type to the ContainerBuilder that have not been built to IContainer.
                            DoRegister(serviceType, serviceType, serviceName, ServiceLifetime.Transient);
                        }
                        else
                        {
                            // Dynamically register the requesting type to the container by using new ContainerBuilder.
                            var builder = new ContainerBuilder();
                            Register(builder, serviceType, serviceType, serviceName, ServiceLifetime.Transient);
                            builder.Update(_container);
                        }
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
            var container = EnsureContainer();
            return serviceName != null ? container.ResolveNamed(serviceName, serviceType) : container.Resolve(serviceType);
        }

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <param name="instance">The existing instance to be injected.</param>
        protected override void DoInjectInstance(object instance)
        {
            if (_builder == null)
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
            if (_builder == null)
            {
                throw new ObjectDisposedException("container");
            }
            Register(_builder, serviceType, implementationType, serviceName, lifetime);
        }

        private void Register(ContainerBuilder builder, Type serviceType, Type implementationType, string serviceName, ServiceLifetime lifetime)
        {
            var registration = serviceName == null
                ? builder.RegisterType(implementationType).As(serviceType)
                : builder.RegisterType(implementationType).Named(serviceName, serviceType);
            registration
                .FindConstructorsWith(type =>
                {
                    var constructors = InjectionAttribute.GetConstructors(type).ToArray();
                    return constructors.Length > 0 ? constructors : type.GetConstructors();
                })
                .UsingConstructor(new MostParametersConstructorSelector())
                .OnActivated(args => DynamicInjectionBuilder.GetOrCreate(implementationType, true, true)(this, args.Instance));
            OnRegistering(new AutofacServiceRegisterEventArgs(serviceType, implementationType, serviceName, lifetime, registration));
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    registration.SingleInstance();
                    break;
                case ServiceLifetime.Transient:
                    registration.InstancePerDependency();
                    break;
                case ServiceLifetime.PerThread:
                    registration.RegistrationData.Sharing = InstanceSharing.Shared;
                    registration.RegistrationData.Lifetime = new PerThreadScopeLifetime();
                    break;
                case ServiceLifetime.PerRequest:
                    registration.RegistrationData.Sharing = InstanceSharing.Shared;
                    registration.RegistrationData.Lifetime = new PerRequestScopeLifetime();
                    break;
            }
        }
    }
}
