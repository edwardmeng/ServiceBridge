using System;

namespace Wheatech.ServiceModel
{
    /// <summary>
    /// Event argument class for the <see cref="IServiceContainer.Registering"/> event. 
    /// </summary>
    public class ServiceRegisterEventArgs : EventArgs
    {
        /// <summary>
        /// Create a new instance of <see cref="ServiceRegisterEventArgs"/>. 
        /// </summary>
        /// <param name="serviceType">Type to map from.</param>
        /// <param name="implementType">Type to map to.</param>
        /// <param name="serviceName">Name for the registration.</param>
        public ServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName)
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
