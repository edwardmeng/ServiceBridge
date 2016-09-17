using System;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceBridge.AspNetCore
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the <see cref="IServiceContainer"/> interface. 
    /// </summary>
    public static class ServiceContainerExtensions
    {
        public static IServiceContainer Register(this IServiceContainer container, ServiceDescriptor descriptor)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (descriptor.ImplementationInstance != null)
            {
                container.RegisterInstance(descriptor.ServiceType, descriptor.ImplementationInstance);
            }
            else if (descriptor.ImplementationType != null)
            {
                container.Register(descriptor.ServiceType, descriptor.ImplementationType, lifetime: ConvertLifetime(descriptor));
            }
            else if (descriptor.ImplementationFactory != null)
            {
                container.RegisterInstance(descriptor.ServiceType, descriptor.ImplementationFactory(container));
            }
            return container;
        }

        private static ServiceLifetime ConvertLifetime(ServiceDescriptor descriptor)
        {
            switch (descriptor.Lifetime)
            {
                case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton:
                    return ServiceLifetime.Singleton;
                case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped:
                    return ServiceLifetime.PerThread;
                case Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient:
                    return ServiceLifetime.Transient;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
