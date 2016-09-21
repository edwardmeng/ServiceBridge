using System;
using StructureMap;
using StructureMap.Pipeline;
#if NetCore
using Microsoft.AspNetCore.Http;
#endif

namespace ServiceBridge.StructureMap
{
    internal class PerRequestLifecycle : LifecycleBase
    {
        private readonly IContainer _container;

        public PerRequestLifecycle(IContainer container)
        {
            _container = container;
        }

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
#if NetCore
            var httpContext = _container.GetInstance<IHttpContextAccessor>()?.HttpContext;
            if (httpContext == null) return null;
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
            return (IObjectCache)httpContext.Items[typeof(PerRequestLifecycle)];
#else
            var httpContext = System.Web.HttpContext.Current;
            if (httpContext == null) return null;
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
            return (IObjectCache)httpContext.Items[typeof(PerRequestLifecycle)];
#endif
        }
    }
}
