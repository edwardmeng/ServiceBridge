using Wheatech.Hosting;

[assembly:AssemblyStartup(typeof(Wheatech.ServiceModel.ServiceModelStartup))]

namespace Wheatech.ServiceModel
{
    internal class ServiceModelStartup
    {
        public ServiceModelStartup(IHostingEnvironment environment)
        {
            environment.Use(ServiceContainer.Current);
        }
    }
}
