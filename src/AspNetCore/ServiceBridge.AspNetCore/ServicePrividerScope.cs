using System;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceBridge.AspNetCore
{
    internal class ServicePrividerScope : IServiceScope
    {
        public ServicePrividerScope(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void Dispose()
        {
            (ServiceProvider as IDisposable)?.Dispose();
        }

        public IServiceProvider ServiceProvider { get; }
    }
}
