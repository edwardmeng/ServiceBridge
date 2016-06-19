using System;
using System.Linq;
using Autofac;
using Autofac.Core.Activators.Reflection;

namespace Wheatech.ServiceModel.Autofac
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> that wraps Autofac.
    /// </summary>
    public class AutofacServiceContainer : ServiceContainerBase
    {
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
            var injectionExpression = new DynamicInjectionBuilder(implementationType).Build();
            registration
                .FindConstructorsWith(type =>
                {
                    var constructors = InjectionAttribute.GetConstructors(type).ToArray();
                    return constructors.Length > 0 ? constructors : type.GetConstructors();
                })
                .UsingConstructor(new MostParametersConstructorSelector())
                .OnActivated(args => injectionExpression(_container, args.Instance));
            var eventArgs = new AutofacServiceRegisterEventArgs(serviceType, implementationType, serviceName, registration) { Lifetime = _lifetime };
            OnRegistering(eventArgs);
            switch (eventArgs.Lifetime)
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
        }
    }
}
