using Wheatech.Activation;

[assembly:AssemblyActivator(typeof(Wheatech.ServiceModel.ServiceModelActivator))]

namespace Wheatech.ServiceModel
{
    internal class ServiceModelActivator
    {
        public ServiceModelActivator(IActivatingEnvironment environment)
        {
            environment.Use(ServiceContainer.Current);
        }
    }
}
