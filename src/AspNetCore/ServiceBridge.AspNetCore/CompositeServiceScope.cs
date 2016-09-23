using System;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceBridge.AspNetCore
{
    internal class CompositeServiceScope : IServiceScope
    {
        public CompositeServiceScope(IServiceProvider serviceProvider)
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
