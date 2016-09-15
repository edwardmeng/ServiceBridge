using System;
using System.Web;
using StructureMap;
using StructureMap.Pipeline;

namespace ServiceBridge.StructureMap
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
        }
    }
}
