using System;
using Ninject.Syntax;

namespace Wheatech.ServiceModel.Ninject
{
    public class NinjectServiceRegisterEventArgs : ServiceRegisterEventArgs
    {
        public NinjectServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName, IBindingSyntax binding) : base(serviceType, implementType, serviceName)
        {
            Binding = binding;
        }

        public ServiceLifetime Lifetime { get; set; }

        public IBindingSyntax Binding { get; }
    }
}
