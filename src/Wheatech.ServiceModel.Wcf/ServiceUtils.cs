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

        internal static bool IsValidService(Type serviceType)
        {
            return !serviceType.IsInterface && !serviceType.IsAbstract && !serviceType.IsGenericTypeDefinition && serviceType.IsClass &&
                   serviceType.IsPublic && serviceType.Assembly != typeof(ServiceContractAttribute).Assembly;
        }
    }
}
