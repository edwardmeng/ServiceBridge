using System;
using StructureMap.Pipeline;

namespace ServiceBridge.StructureMap
{
    /// <summary>
    /// Event argument for the <see cref="IServiceContainer.Registering"/> event raised in <see cref="StructureMapServiceContainer"/>.
    /// </summary>
    public class StructureMapServiceRegisterEventArgs : ServiceRegisterEventArgs
    {
        internal StructureMapServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName, ServiceLifetime lifetime, ConfiguredInstance configuration) 
            : base(serviceType, implementType, serviceName, lifetime)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the registration instance for the StructureMap service mapping.
        /// </summary>
        public ConfiguredInstance Configuration { get; }
    }
}
