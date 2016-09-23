using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace ServiceBridge.AspNetCore
{
    public class ServiceBridgeStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<ServiceBridgeMiddleware>(builder);
                next(builder);
            };
        }
    }
}
