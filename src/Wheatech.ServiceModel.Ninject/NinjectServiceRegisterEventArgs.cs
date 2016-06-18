using System;

namespace Wheatech.ServiceModel.Ninject
{
    public class NinjectServiceRegisterEventArgs : ServiceRegisterEventArgs
    {
        public NinjectServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName) : base(serviceType, implementType, serviceName)
        {
        }

        public ServiceLifetime Lifetime { get; set; }
    }
}
