using System.Web;
using Wheatech.Hosting;

[assembly: AssemblyStartup(typeof(Wheatech.ServiceModel.Web.ServiceModelStartup))]

namespace Wheatech.ServiceModel.Web
{
    internal class ServiceModelStartup
    {
        public ServiceModelStartup(IHostingEnvironment hostingEnvironment, IServiceContainer container)
        {
            HttpApplication.RegisterModule(typeof(ServiceContainerHttpModule));
        }
    }
}
