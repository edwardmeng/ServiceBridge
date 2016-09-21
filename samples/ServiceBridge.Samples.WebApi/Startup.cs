using MassActivation;
using ServiceBridge.Autofac.Activation;
using ServiceBridge.Autofac.Interception;

namespace ServiceBridge.Samples.WebApi
{
    public class Startup
    {
        public Startup(IActivatingEnvironment environment)
        {
            environment.UseAutofac();
        }

        public void Configuration(IServiceContainer container)
        {
            container.EnableInterception();
        }
    }
}