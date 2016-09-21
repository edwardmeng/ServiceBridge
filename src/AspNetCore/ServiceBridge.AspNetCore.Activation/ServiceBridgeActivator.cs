using MassActivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

[assembly: AssemblyActivator(typeof(ServiceBridge.AspNetCore.Activation.ServiceBridgeActivator))]

namespace ServiceBridge.AspNetCore.Activation
{
    public class ServiceBridgeActivator
    {
        public void Configuration(IActivatingEnvironment environment, IServiceContainer container, IServiceCollection services)
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Replace(ServiceDescriptor.Singleton<IControllerActivator, ServiceBridgeControllerActivator>());
            container.RegisterServices(services);
        }
    }
}
