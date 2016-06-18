using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Wheatech.ServiceModel.Windsor
{
    public class WindsorServiceContainer : ServiceContainerBase
    {
        private IWindsorContainer _container;
        private readonly ServiceLifetime _lifetime;

        public WindsorServiceContainer(IWindsorContainer container = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            _lifetime = lifetime;
            _container = container ?? new WindsorContainer();
            var modelBuilder = _container.Kernel.ComponentModelBuilder;
            var oldInspector = modelBuilder.Contributors.OfType<ConstructorDependenciesModelInspector>().SingleOrDefault();
            if (oldInspector!=null)
            {
                modelBuilder.RemoveContributor(oldInspector);
            }
            modelBuilder.AddContributor(new SelectConstructorInspector());
            _container.Register(Component.For<IServiceContainer>().Instance(this));
        }

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

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (_container == null)
            {
                throw new ObjectDisposedException("container");
            }
            return _container.ResolveAll(serviceType).Cast<object>();
        }

        protected override void DoRegister(Type serviceType, Type implementationType, string serviceName)
        {
            if (_container == null)
            {
                throw new ObjectDisposedException("container");
            }
            var registration = Component.For(serviceType)
                .ImplementedBy(implementationType)
                .Named(GetServiceName(serviceType, serviceName))
                .PropertiesRequire(property => property.CanWrite && !(property.SetMethod ?? property.GetMethod).IsStatic && property.IsDefined(typeof(InjectionAttribute)));
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
