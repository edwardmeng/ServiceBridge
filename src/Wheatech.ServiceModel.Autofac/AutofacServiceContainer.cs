using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Wheatech.ServiceModel.Autofac
{
    /// <summary>
    /// An implementation of <see cref="IServiceContainer"/> that wraps Autofac.
    /// </summary>
    public class AutofacServiceContainer : ServiceContainerBase, IDisposable
    {
        private Dictionary<Tuple<Type, string>, Type> _registrations = new Dictionary<Tuple<Type, string>, Type>();

        private IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceContainer" /> class.
        /// </summary>
        public AutofacServiceContainer()
        {
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }
            _registrations = null;
        }

        private IContainer EnsureContainer()
        {
            if (_registrations == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (_container == null)
            {
                var containerBuilder = new ContainerBuilder();
                foreach (var registration in _registrations)
                {
                    if (registration.Key.Item2 == null)
                    {
                        containerBuilder.RegisterType(registration.Value).As(registration.Key.Item1);
                    }
                    else
                    {
                        containerBuilder.RegisterType(registration.Value).Named(registration.Key.Item2, registration.Key.Item1);
                    }
                }
                _container = containerBuilder.Build();
            }
            return _container;
        }

        /// <summary>
        /// Checks if a particular type/name pair has been registered with the container. 
        /// </summary>
        /// <param name="serviceType">Type to check registration for.</param>
        /// <param name="serviceName">Name to check registration for.</param>
        /// <returns><c>true</c> if this type/name pair has been registered, <c>false</c> if not.</returns>
        public override bool IsRegistered(Type serviceType, string serviceName = null)
        {
            if (_registrations == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            return _registrations.ContainsKey(Tuple.Create(serviceType, serviceName));
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
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);

            object instance = container.Resolve(enumerableType);
            return ((IEnumerable)instance).Cast<object>();
        }

        /// <summary>
        /// Registers the type mapping.
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        protected override void DoRegister(Type serviceType, Type implementationType, string serviceName)
        {
            if (_registrations == null)
            {
                throw new ObjectDisposedException("container");
            }
            if (_container != null)
            {
                _container.Dispose();
                _container = null;
            }
            _registrations[Tuple.Create(serviceType, serviceName)] = implementationType;
        }
    }
}
