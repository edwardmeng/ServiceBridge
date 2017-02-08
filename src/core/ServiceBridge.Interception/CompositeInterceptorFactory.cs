using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ServiceBridge.Interception
{
    internal class CompositeInterceptorFactory : IInterceptorFactory
    {
        private readonly List<IInterceptorFactory> _childFactories = new List<IInterceptorFactory>();
        public static readonly CompositeInterceptorFactory Default = new CompositeInterceptorFactory();

        public CompositeInterceptorFactory()
        {
            _childFactories = new List<IInterceptorFactory>();
        }

        public CompositeInterceptorFactory(IEnumerable<IInterceptorFactory> factories)
        {
            if (factories == null)
            {
                throw new ArgumentNullException(nameof(factories));
            }
            _childFactories = factories.ToList();
        }

        public CompositeInterceptorFactory(params IInterceptorFactory[] factories)
            :this((IEnumerable<IInterceptorFactory>)factories)
        {
        }

        public void TryAddFactory(IInterceptorFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            if (!_childFactories.Contains(factory))
            {
                _childFactories.Add(factory);
            }
        }

        public void TryAddFactory<T>()
            where T : IInterceptorFactory, new()
        {
            if (!_childFactories.OfType<T>().Any())
            {
                _childFactories.Add(new T());
            }
        }

        public bool RemoveFactory(IInterceptorFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            return _childFactories.Remove(factory);
        }

        public void RemoveFactories(Type factoryType)
        {
            if (factoryType == null)
            {
                throw new ArgumentNullException(nameof(factoryType));
            }
#if NetCore
            _childFactories.RemoveAll(factory => factoryType.GetTypeInfo().IsAssignableFrom(factory.GetType().GetTypeInfo()));
#else
            _childFactories.RemoveAll(factoryType.IsInstanceOfType);
#endif
        }

        public IEnumerable<IInterceptor> CreateInterceptors(MethodInfo interfaceMethod, MethodInfo implementMethod, IServiceContainer container)
        {
            return from factory in _childFactories
                   let childInterceptors = factory.CreateInterceptors(interfaceMethod, implementMethod, container)
                   where childInterceptors != null
                   from interceptor in childInterceptors
                   select interceptor;
        }

        public IEnumerable<IInterceptor> CreateInterceptors(ConstructorInfo constructor, IServiceContainer container)
        {
            return from factory in _childFactories
                   let childInterceptors = factory.CreateInterceptors(constructor, container)
                   where childInterceptors != null
                   from interceptor in childInterceptors
                   select interceptor;
        }
    }
}
