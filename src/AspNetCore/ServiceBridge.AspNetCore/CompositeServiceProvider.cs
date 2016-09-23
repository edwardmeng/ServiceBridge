using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceBridge.AspNetCore
{
    /// <summary>
    /// Represents an <see cref="IServiceProvider"/> composed of multiple child service providers.
    /// </summary>
    public class CompositeServiceProvider : IServiceProvider
    {
        /// <summary>
        /// Initialize a new <see cref="CompositeServiceProvider"/> using the specified child service providers.
        /// </summary>
        /// <param name="serviceProviders">The child service providers.</param>
        public CompositeServiceProvider(IEnumerable<IServiceProvider> serviceProviders)
        {
            if (serviceProviders == null)
            {
                throw new ArgumentNullException(nameof(serviceProviders));
            }
            ServiceProviders = serviceProviders.ToArray();
        }

        /// <summary>
        /// Initialize a new <see cref="CompositeServiceProvider"/> using the specified child service providers.
        /// </summary>
        /// <param name="serviceProviders">The child service providers.</param>
        public CompositeServiceProvider(params IServiceProvider[] serviceProviders)
            : this((IEnumerable<IServiceProvider>)serviceProviders)
        {
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type serviceType.
        /// -or-
        /// <c>null</c> if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            foreach (var serviceProvider in ServiceProviders)
            {
                var service = serviceProvider.GetService(serviceType);
                if (service != null)
                {
                    return service;
                }
            }
            return null;
        }

        internal IServiceProvider[] ServiceProviders { get; }
    }
}
