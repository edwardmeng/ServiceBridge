using System;

namespace Wheatech.ServiceModel
{
    /// <summary>
    /// The ServiceRegistration is used to represent the registration of the service.
    /// </summary>
    public class ServiceRegistration
    {
        internal ServiceRegistration(Type serviceType, Type implementType, string serviceName)
        {
            ServiceType = serviceType;
            ImplementType = implementType;
            ServiceName = serviceName;
        }

        /// <summary>
        /// Gets the name of the registration.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets the type to map from.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Gets the type to map to.
        /// </summary>
        public Type ImplementType { get; }
    }
}
