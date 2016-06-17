using System;
using Ninject.Activation;

namespace Wheatech.ServiceModel.Ninject
{
    public class NinjectServiceRegisterEventArgs : ServiceRegisterEventArgs
    {
        public NinjectServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName) : base(serviceType, implementType, serviceName)
        {
        }

        public Func<IContext, object> Scope { get; set; }
    }
}
