using MassActivation;
using Microsoft.AspNetCore.Builder;

[assembly: AssemblyActivator(typeof(ServiceBridge.AspNetCore.Activation.ServiceBridgeActivator))]

namespace ServiceBridge.AspNetCore.Activation
{
    internal class ServiceBridgeActivator
    {
        public ServiceBridgeActivator(IApplicationBuilder builder)
        {
            builder.UseServiceBridge();
        }
    }
}
