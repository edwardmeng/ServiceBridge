using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Wheatech.ServiceModel.Mvc
{
    public class ServiceModelDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            return ServiceContainer.GetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return ServiceContainer.GetAllInstances(serviceType);
        }
    }
}
