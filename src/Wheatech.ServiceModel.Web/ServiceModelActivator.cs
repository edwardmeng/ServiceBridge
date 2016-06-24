using System.Web;
using Wheatech.Activation;

[assembly: AssemblyActivator(typeof(Wheatech.ServiceModel.Web.ServiceModelActivator))]

namespace Wheatech.ServiceModel.Web
{
    internal class ServiceModelActivator
    {
        public ServiceModelActivator(IActivatingEnvironment environment, IServiceContainer container)
        {
            HttpApplication.RegisterModule(typeof(ServiceContainerHttpModule));
        }
    }
}
