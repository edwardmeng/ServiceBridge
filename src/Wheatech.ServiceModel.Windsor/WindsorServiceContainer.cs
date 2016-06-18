using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Wheatech.ServiceModel.Windsor
{
    public class WindsorServiceContainer : ServiceContainerBase
    {
        private readonly IWindsorContainer _container;
        private readonly ServiceLifetime _lifetime;

        public WindsorServiceContainer(IWindsorContainer container = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            _lifetime = lifetime;
            _container = container ?? new WindsorContainer();
            _container.Register(Component.For<IServiceContainer>().Instance(this));
        }

        protected override object DoGetInstance(Type serviceType, string serviceName)
        {
            if (serviceName == null)
            {
                return _container.Resolve(serviceType);
            }
            else
            {
                return _container.Resolve(GetServiceName(serviceType, serviceName), serviceType);
            }
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _container.ResolveAll(serviceType).Cast<object>();
        }

        public override bool IsRegistered(Type serviceType, string serviceName = null)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                return _container.Kernel.HasComponent(serviceType);
            }
            else
            {
                return _container.Kernel.HasComponent(GetServiceName(serviceType, serviceName));
            }
        }

        protected override void DoRegister(Type serviceType, Type implementationType, string serviceName)
        {
            var registration = Component.For(serviceType)
                .ImplementedBy(implementationType)
                .Named(GetServiceName(serviceType, serviceName));
            var eventArgs = new WindsorServiceRegisterEventArgs(serviceType, implementationType, serviceName, registration) { Lifetime = _lifetime };
            OnRegistering(eventArgs);
            switch (eventArgs.Lifetime)
            {
                case ServiceLifetime.Poolable:
                    registration = eventArgs.Registration.LifestylePooled();
                    break;
                case ServiceLifetime.Scoped:
                    registration = eventArgs.Registration.LifestyleScoped();
                    break;
                case ServiceLifetime.Singleton:
                    registration = eventArgs.Registration.LifestyleSingleton();
                    break;
                case ServiceLifetime.Transient:
                    registration = eventArgs.Registration.LifestyleTransient();
                    break;
                case ServiceLifetime.PerThread:
                    registration = eventArgs.Registration.LifestylePerThread();
                    break;
                case ServiceLifetime.PerRequest:
                    registration = eventArgs.Registration.LifestylePerWebRequest();
                    break;
            }
            _container.Register(registration);
        }

        public void RegisterInstance(Type serviceType, object instance)
        {
            _container.Register(Component.For(serviceType).Instance(instance));
        }

        private string GetServiceName(Type serviceType, string serviceName)
        {
            return ComponentName.DefaultNameFor(serviceType) + (string.IsNullOrEmpty(serviceName) ? null : '.' + serviceName);
        }
    }
}
