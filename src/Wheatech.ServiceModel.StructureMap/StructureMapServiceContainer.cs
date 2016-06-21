using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StructureMap;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;

namespace Wheatech.ServiceModel.StructureMap
{
    public class StructureMapServiceContainer : ServiceContainerBase
    {
        private IContainer _container;
        private readonly ServiceLifetime _lifetime;

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

        public StructureMapServiceContainer(IContainer container = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            _container = container ?? new Container();
            _lifetime = lifetime;
        }

        protected override object DoGetInstance(Type serviceType, string serviceName)
        {
            return string.IsNullOrEmpty(serviceName) ? _container.GetInstance(serviceType) : _container.GetInstance(serviceType, serviceName);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType).Cast<object>();
        }

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
                instance.AddInterceptor(new ActivatorInterceptor<object>((context, x) => methodsInjection(context,x)));
                registry.Policies.ConstructorSelector<InjectionConstructorSelector>();
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
