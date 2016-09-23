using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace ServiceBridge.AspNetCore
{
    internal class ServiceBridgeMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _app;

        public ServiceBridgeMiddleware(RequestDelegate next, IApplicationBuilder app)
        {
            _next = next;
            _app = app;
            Initialize();
        }

        private void Initialize()
        {
            var serviceProvider = _app.ApplicationServices;
            if (!Flattern(serviceProvider).Contains(ServiceContainer.Current))
            {
                serviceProvider = new CompositeServiceProvider(serviceProvider, ServiceContainer.Current);
                _app.ApplicationServices = serviceProvider;
            }
        }

        private static IEnumerable<IServiceProvider> Flattern(IServiceProvider serviceProvider)
        {
            var compositeProvider = serviceProvider as CompositeServiceProvider;
            if (compositeProvider != null)
            {
                return compositeProvider.ServiceProviders.SelectMany(Flattern);
            }
            return new[] { serviceProvider };
        }

        public async Task Invoke(HttpContext context)
        {
            var existingFeature = context.Features.Get<IServiceProvidersFeature>();
            using (var feature = new RequestServicesFeature(new CompositeServiceScopeFactory(_app.ApplicationServices)))
            {
                try
                {
                    context.Features.Set<IServiceProvidersFeature>(feature);
                    await _next.Invoke(context);
                }
                finally
                {
                    context.Features.Set(existingFeature);
                }
            }
        }
    }
}

