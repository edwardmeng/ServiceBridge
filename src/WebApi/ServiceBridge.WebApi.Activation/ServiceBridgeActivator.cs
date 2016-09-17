using System.Web.Http;
using MassActivation;

[assembly: AssemblyActivator(typeof(ServiceBridge.WebApi.Activation.ServiceBridgeActivator))]

namespace ServiceBridge.WebApi.Activation
{
    internal class ServiceBridgeActivator
    {
        static ServiceBridgeActivator()
        {
            GlobalConfiguration.Configuration.DependencyResolver = new ServiceBridgeDependencyResolver();
        }

        public void Configuration(IActivatingEnvironment environment, IServiceContainer container)
        {
            // We have to register the controllers at the application configuration stage.
            // Since there are some IoC implementations cannot register types after resolve instances.
            foreach (var assembly in environment.GetAssemblies())
            {
                if (!assembly.IsDynamic)
                {
                    container.RegisterApiControllers(assembly);
                }
            }
        }
    }
}
