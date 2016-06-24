using Wheatech.Activation;

[assembly:AssemblyActivator(typeof(Wheatech.ServiceModel.ServiceModelActivator))]

namespace Wheatech.ServiceModel
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
