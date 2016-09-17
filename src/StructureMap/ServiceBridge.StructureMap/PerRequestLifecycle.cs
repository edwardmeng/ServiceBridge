using System;
using StructureMap;
using StructureMap.Pipeline;
#if NetCore
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace ServiceBridge.StructureMap
{
    internal class PerRequestLifecycle : LifecycleBase
    {
        private class PerRequestLifecycleObjectCache : LifecycleObjectCache, IDisposable
        {
            public void Dispose()
            {
                DisposeAndClear();
            }
        }

        public override void EjectAll(ILifecycleContext context)
        {
            FindCache(context)?.DisposeAndClear();
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {
            var httpContext = ServiceContainer.HostContext as HttpContext;
            if (httpContext == null) return null;
#if NetCore
            if (!httpContext.Items.ContainsKey(typeof(PerRequestLifecycle)))
            {
                lock (httpContext)
                {
                    if (!httpContext.Items.ContainsKey(typeof(PerRequestLifecycle)))
                    {
                        var cache = new PerRequestLifecycleObjectCache();
                        httpContext.Items[typeof(PerRequestLifecycle)] = cache;
                        return cache;
                    }
                }
            }
#else
            if (!httpContext.Items.Contains(typeof(PerRequestLifecycle)))
            {
                lock (httpContext.Items.SyncRoot)
                {
                    if (!httpContext.Items.Contains(typeof(PerRequestLifecycle)))
                    {
                        var cache = new PerRequestLifecycleObjectCache();
                        httpContext.Items[typeof(PerRequestLifecycle)] = cache;
                        httpContext.DisposeOnPipelineCompleted(cache);
                        return cache;
                    }
                }
            }
#endif
            return (IObjectCache)httpContext.Items[typeof(PerRequestLifecycle)];
        }
    }
}
