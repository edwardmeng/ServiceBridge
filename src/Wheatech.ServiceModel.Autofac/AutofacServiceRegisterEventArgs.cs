using System;
using Autofac.Builder;

namespace Wheatech.ServiceModel.Autofac
{
    public class AutofacServiceRegisterEventArgs : ServiceRegisterEventArgs
    {
        public AutofacServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName, IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration)
            : base(serviceType, implementType, serviceName)
        {
            Registration = registration;
        }

        public IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> Registration { get; }
    }
}
