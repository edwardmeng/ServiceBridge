using MassActivation;

[assembly:AssemblyActivator(typeof(ServiceBridge.ServiceBridgeActivator))]

namespace ServiceBridge
{
    [ActivationPriority(ActivationPriority.Low)]
    internal class ServiceBridgeActivator
    {
        public ServiceBridgeActivator(IActivatingEnvironment environment)
        {
            environment.Use(ServiceContainer.Current);
        }
    }
}
