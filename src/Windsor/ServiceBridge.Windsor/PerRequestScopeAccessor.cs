using System.Web;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace ServiceBridge.Windsor
{
    internal class PerRequestScopeAccessor:IScopeAccessor
    {
        public void Dispose()
        {
            if (HttpContext.Current == null) return;
            if (HttpContext.Current.Items.Contains(typeof(PerRequestScopeAccessor)))
            {
                lock (HttpContext.Current.Items.SyncRoot)
                {
                    var scope = (ILifetimeScope) HttpContext.Current.Items[typeof(PerRequestScopeAccessor)];
                    scope?.Dispose();
                }
            }
        }

        public ILifetimeScope GetScope(CreationContext context)
        {
            if (HttpContext.Current == null) return null;
            if (!HttpContext.Current.Items.Contains(typeof(PerRequestScopeAccessor)))
            {
                lock (HttpContext.Current.Items.SyncRoot)
                {
                    if (!HttpContext.Current.Items.Contains(typeof(PerRequestScopeAccessor)))
                    {
                        var scope = new DefaultLifetimeScope(new ScopeCache());
                        HttpContext.Current.Items[typeof(PerRequestScopeAccessor)] = scope;
                        HttpContext.Current.DisposeOnPipelineCompleted(scope);
                        return scope;
                    }
                }
            }
            return (ILifetimeScope) HttpContext.Current.Items[typeof(PerRequestScopeAccessor)];
        }
    }
}
