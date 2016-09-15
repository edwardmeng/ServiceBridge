using System;
using Autofac.Builder;

namespace ServiceBridge.Autofac
{
    /// <summary>
    /// Event argument for the <see cref="IServiceContainer.Registering"/> event raised in <see cref="AutofacServiceContainer"/>.
    /// </summary>
    public class AutofacServiceRegisterEventArgs : ServiceRegisterEventArgs
    {
        internal AutofacServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName, ServiceLifetime lifetime,
            IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration)
            : base(serviceType, implementType, serviceName, lifetime)
        {
            Registration = registration;
        }

        /// <summary>
        /// Gets the registration instance for the Autofac service mapping.
        /// </summary>
        public IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> Registration { get; }
    }
}
