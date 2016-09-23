using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace ServiceBridge.AspNetCore
{
    /// <summary>
    /// Implementation of <see cref="IStartupFilter"/> to integrate ServiceBridge with AspNetCore.
    /// </summary>
    public class ServiceBridgeStartupFilter : IStartupFilter
    {
        /// <summary>
        /// Configuration the application to integrate ServiceBridge with AspNetCore.
        /// </summary>
        /// <param name="next">The next configuration action.</param>
        /// <returns>The integrated configuration action.</returns>
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

