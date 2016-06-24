using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Wheatech.ServiceModel.WebApi
{
    /// <summary>
    /// ASP.NET WebApi dependency resolver implementation for ServiceModel.
    /// </summary>
    public class ServiceModelDependencyResolver : IDependencyResolver
    {
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>Retrieves a service from the scope.</summary>
        /// <returns>The retrieved service.</returns>
        /// <param name="serviceType">The service to be retrieved.</param>
        public object GetService(Type serviceType)
        {
            return ServiceContainer.GetInstance(serviceType);
        }

        /// <summary>Retrieves a collection of services from the scope.</summary>
        /// <returns>The retrieved collection of services.</returns>
        /// <param name="serviceType">The collection of services to be retrieved.</param>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return ServiceContainer.GetAllInstances(serviceType);
        }

        /// <summary> Starts a resolution scope. </summary>
        /// <returns>The dependency scope.</returns>
        public IDependencyScope BeginScope()
        {
            return this;
        }
    }
}
