using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ServiceBridge.Mvc
{
    /// <summary>
    /// ASP.NET MVC dependency resolver implementation for ServiceModel.
    /// </summary>
    public class ServiceBridgeDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// Gets the service of the specified type.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns>The service instance or null if none is configured.</returns>
        public object GetService(Type serviceType)
        {
            return ServiceContainer.GetInstance(serviceType);
        }

        /// <summary>
        /// Gets the services of the specidies type.
        /// </summary>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns>All service instances or an empty enumerable if none is configured.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return ServiceContainer.GetAllInstances(serviceType);
        }
    }
}
