using System;
using System.Reflection;
using System.ServiceModel;

namespace Wheatech.ServiceModel.Wcf
{
    internal static class ServiceUtils
    {
        public static string GetServiceName(Type serviceType)
        {
            string serviceName = null;
            var behaviorAttribute = serviceType.GetCustomAttribute<ServiceBehaviorAttribute>();
            if (behaviorAttribute != null)
            {
                serviceName = string.IsNullOrEmpty(behaviorAttribute.ConfigurationName) ? behaviorAttribute.Name : behaviorAttribute.ConfigurationName;
            }
            if (string.IsNullOrEmpty(serviceName))
            {
                serviceName = serviceType.FullName;
            }
            return serviceName;
        }

        /// <summary>
        /// Determine the lifetime for the specified service implementation according to the ServiceBehaviorAttribute markup.
        /// </summary>
        /// <param name="serviceType">The type of service implementation.</param>
        /// <returns>The lifetime of the specified service implementation.</returns>
        public static ServiceLifetime GetServiceLifetime(Type serviceType)
        {
            var attribute = serviceType.GetCustomAttribute<ServiceBehaviorAttribute>();
            var lifetime = ServiceLifetime.PerThread;
            if (attribute != null)
            {
                switch (attribute.InstanceContextMode)
                {
                    case InstanceContextMode.PerCall:
                        lifetime = ServiceLifetime.Transient;
                        break;
                    case InstanceContextMode.Single:
                        lifetime = ServiceLifetime.Singleton;
                        break;
                    case InstanceContextMode.PerSession:
                        lifetime = ServiceLifetime.PerThread;
                        break;
                }
            }
            return lifetime;
        }
    }
}
