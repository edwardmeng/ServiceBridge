using System;

namespace ServiceBridge
{
    /// <summary>
    /// The ServiceRegistration is used to represent the registration of the service.
    /// </summary>
    public class ServiceRegistration
    {
        /// <summary>
        /// Initialize new instance of <see cref="ServiceRegistration"/>.
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        public ServiceRegistration(Type serviceType, Type implementType, string serviceName)
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
