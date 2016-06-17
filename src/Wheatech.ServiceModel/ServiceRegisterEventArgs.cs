using System;

namespace Wheatech.ServiceModel
{
    public class ServiceRegisterEventArgs : EventArgs
    {
        public ServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName)
        {
            ServiceType = serviceType;
            ImplementType = implementType;
            ServiceName = serviceName;
        }

        public string ServiceName { get; }

        public Type ServiceType { get; }

        public Type ImplementType { get; }
    }
}
