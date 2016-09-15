using System.Web.Mvc;
using MassActivation;

[assembly: AssemblyActivator(typeof(ServiceBridge.Mvc.Activation.ServiceModelActivator))]

namespace ServiceBridge.Mvc.Activation
{
    internal class ServiceModelActivator
    {
        static ServiceModelActivator()
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
