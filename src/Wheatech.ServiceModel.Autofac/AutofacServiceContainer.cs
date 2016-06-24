﻿using System;
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
            var registration = serviceName == null
                ? _builder.RegisterType(implementationType).As(serviceType)
                : _builder.RegisterType(implementationType).Named(serviceName, serviceType);
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
