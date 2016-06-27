using System.Web.Mvc;
using Wheatech.Activation;

[assembly: AssemblyActivator(typeof(Wheatech.ServiceModel.Mvc.ServiceModelActivator))]

namespace Wheatech.ServiceModel.Mvc
{
    internal class ServiceModelActivator
    {
        static ServiceModelActivator()
        {
            DependencyResolver.SetResolver(new ServiceModelDependencyResolver());
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
