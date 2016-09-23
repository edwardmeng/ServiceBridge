using MassActivation;
using Microsoft.AspNetCore.Hosting;
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
            services.Insert(0, ServiceDescriptor.Transient<IStartupFilter, ServiceBridgeStartupFilter>());
            var httpContextAccessor = new HttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor>(httpContextAccessor);
            services.Replace(ServiceDescriptor.Singleton<IControllerActivator,ServiceBasedControllerActivator>());
            container.RegisterInstance<IHttpContextAccessor>(httpContextAccessor);
        }
    }
}
