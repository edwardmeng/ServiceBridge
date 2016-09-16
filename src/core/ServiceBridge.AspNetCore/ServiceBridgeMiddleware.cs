using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ServiceBridge.AspNetCore
{
    internal class ServiceBridgeMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ThreadLocal<HttpContext> _context = new ThreadLocal<HttpContext>();

        public ServiceBridgeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            _context.Value = context;
            await _next(context);
            foreach (var contextItem in context.Items)
            {
                (contextItem.Value as IDisposable)?.Dispose();
            }
            _context.Value = null;
        }

        public static HttpContext CurrentContext => _context.Value;
    }
}
