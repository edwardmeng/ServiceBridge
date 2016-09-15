using Wheatech.Activation;

[assembly:AssemblyActivator(typeof(ServiceBridge.ServiceModelActivator))]

namespace ServiceBridge
{
    [ActivationPriority(ActivationPriority.Low)]
    internal class ServiceModelActivator
    {
        public ServiceModelActivator(IActivatingEnvironment environment)
        {
            environment.Use(ServiceContainer.Current);
        }
    }
}
