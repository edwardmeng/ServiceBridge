using System.Web.Http;
using Wheatech.Activation;

[assembly: AssemblyActivator(typeof(Wheatech.ServiceModel.WebApi.ServiceModelActivator))]

namespace Wheatech.ServiceModel.WebApi
{
    internal class ServiceModelActivator
    {
        static ServiceModelActivator()
        {
            GlobalConfiguration.Configuration.DependencyResolver = new ServiceModelDependencyResolver();
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
