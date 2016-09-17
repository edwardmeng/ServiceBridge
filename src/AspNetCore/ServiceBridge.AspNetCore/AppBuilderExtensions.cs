using System;
using Microsoft.AspNetCore.Builder;

namespace ServiceBridge.AspNetCore
{
    /// <summary>
    /// Extension methods for adding the ServiceBridge middleware to an <see cref="IApplicationBuilder"/>. 
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Uses ServiceBridge middleware to the specified <see cref="IApplicationBuilder"/>. 
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseServiceBridge(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            return builder.Use(async (context, next) =>
            {
                ServiceContainer.HostContext = context;
                await next();
                foreach (var contextItem in context.Items)
                {
                    (contextItem.Value as IDisposable)?.Dispose();
                }
                ServiceContainer.HostContext = null;
            });
        }
    }
}
