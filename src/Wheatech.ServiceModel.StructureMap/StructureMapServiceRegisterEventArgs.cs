using System;
using StructureMap.Pipeline;

namespace Wheatech.ServiceModel.StructureMap
{
    public class StructureMapServiceRegisterEventArgs : ServiceRegisterEventArgs
    {
        public StructureMapServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName, ConfiguredInstance configuration) : base(serviceType, implementType, serviceName)
        {
            Configuration = configuration;
        }

        public ConfiguredInstance Configuration { get; }
    }
}
