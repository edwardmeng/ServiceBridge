using System;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceBridge.AspNetCore
{
    internal class CompositeServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CompositeServiceScopeFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceScope CreateScope()
        {
            return new CompositeServiceScope(_serviceProvider);
        }
    }
}
