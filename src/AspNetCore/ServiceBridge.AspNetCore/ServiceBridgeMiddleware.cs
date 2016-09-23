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
            if (ServiceContainer.HasProvider)
            {
                var compositeContainer = new CompositeServiceContainer(ServiceContainer.Current, _app.ApplicationServices);
                ServiceContainer.SetProvider(()=> compositeContainer);
                _app.ApplicationServices = compositeContainer;
            }
        }

        public async Task Invoke(HttpContext context)
        {
            var existingFeature = context.Features.Get<IServiceProvidersFeature>();
            using (var feature = new RequestServicesFeature(new ServiceProviderScopeFactory(_app.ApplicationServices)))
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

