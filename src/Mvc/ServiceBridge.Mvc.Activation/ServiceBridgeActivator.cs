using System.Web.Mvc;
using MassActivation;

[assembly: AssemblyActivator(typeof(ServiceBridge.Mvc.Activation.ServiceBridgeActivator))]

namespace ServiceBridge.Mvc.Activation
{
    internal class ServiceBridgeActivator
    {
        static ServiceBridgeActivator()
        {
            DependencyResolver.SetResolver(new ServiceBridgeDependencyResolver());
        }

        public void Configuration(IActivatingEnvironment environment, IServiceContainer container)
        {
            // We have to register the controllers at the application configuration stage.
            // Since there are some IoC implementations cannot register types after resolve instances.
            foreach (var assembly in environment.GetAssemblies())
            {
                if (!assembly.IsDynamic)
                {
                    container.RegisterMvcControllers(assembly);
                }
            }
        }
    }
}
