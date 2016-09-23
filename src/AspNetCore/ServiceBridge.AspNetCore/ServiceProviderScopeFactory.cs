using System;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceBridge.AspNetCore
{
    internal class ServiceProviderScopeFactory : IServiceScopeFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderScopeFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceScope CreateScope()
        {
            return new ServicePrividerScope(_serviceProvider);
        }
    }
}
