using System;
using StructureMap.Pipeline;

namespace Wheatech.ServiceModel.StructureMap
{
    /// <summary>
    /// Event argument for the <see cref="IServiceContainer.Registering"/> event raised in <see cref="StructureMapServiceContainer"/>.
    /// </summary>
    public class StructureMapServiceRegisterEventArgs : ServiceRegisterEventArgs
    {
        internal StructureMapServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName, ConfiguredInstance configuration) 
            : base(serviceType, implementType, serviceName)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the registration instance for the StructureMap service mapping.
        /// </summary>
        public ConfiguredInstance Configuration { get; }
    }
}
