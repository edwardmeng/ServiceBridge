using System;
using Castle.MicroKernel.Registration;

namespace Wheatech.ServiceModel.Windsor
{
    public class WindsorServiceRegisterEventArgs : ServiceRegisterEventArgs
    {
        public WindsorServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName, ComponentRegistration<object> registration)
            : base(serviceType, implementType, serviceName)
        {
            Registration = registration;
        }

        public ServiceLifetime Lifetime { get; set; }

        public ComponentRegistration<object> Registration { get; }
    }
}
