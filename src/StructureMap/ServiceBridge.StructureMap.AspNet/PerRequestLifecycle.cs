using System;
using StructureMap;
using StructureMap.Pipeline;
#if NetCore
using ServiceBridge.AspNetCore;
#else
using System.Web;
#endif

namespace ServiceBridge.StructureMap.AspNet
{
    internal class PerRequestLifecycle:LifecycleBase
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
#if NetCore
            if (ServiceBridgeMiddleware.CurrentContext == null) return null;
            if (!ServiceBridgeMiddleware.CurrentContext.Items.ContainsKey(typeof(PerRequestLifecycle)))
            {
                lock (ServiceBridgeMiddleware.CurrentContext)
                {
                    if (!ServiceBridgeMiddleware.CurrentContext.Items.ContainsKey(typeof(PerRequestLifecycle)))
                    {
                        var cache = new PerRequestLifecycleObjectCache();
                        ServiceBridgeMiddleware.CurrentContext.Items[typeof(PerRequestLifecycle)] = cache;
                        return cache;
                    }
                }
            }
            return (IObjectCache)ServiceBridgeMiddleware.CurrentContext.Items[typeof(PerRequestLifecycle)];
#else
            if (HttpContext.Current == null) return null;
            if (!HttpContext.Current.Items.Contains(typeof(PerRequestLifecycle)))
            {
                lock (HttpContext.Current.Items.SyncRoot)
                {
                    if (!HttpContext.Current.Items.Contains(typeof(PerRequestLifecycle)))
                    {
                        var cache = new PerRequestLifecycleObjectCache();
                        HttpContext.Current.Items[typeof(PerRequestLifecycle)] = cache;
                        HttpContext.Current.DisposeOnPipelineCompleted(cache);
                        return cache;
                    }
                }
            }
            return (IObjectCache)HttpContext.Current.Items[typeof(PerRequestLifecycle)];
#endif
        }
    }
}
