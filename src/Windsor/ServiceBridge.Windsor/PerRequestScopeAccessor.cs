using System.Web;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace ServiceBridge.Windsor
{
    internal class PerRequestScopeAccessor:IScopeAccessor
    {
        public void Dispose()
        {
            var httpContext = ServiceContainer.HostContext as HttpContext;
            if (httpContext == null) return;
            if (httpContext.Items.Contains(typeof(PerRequestScopeAccessor)))
            {
                lock (httpContext.Items.SyncRoot)
                {
                    var scope = (ILifetimeScope)httpContext.Items[typeof(PerRequestScopeAccessor)];
                    scope?.Dispose();
                }
            }
        }

        public ILifetimeScope GetScope(CreationContext context)
        {
            var httpContext = ServiceContainer.HostContext as HttpContext;
            if (httpContext == null) return null;
            if (!httpContext.Items.Contains(typeof(PerRequestScopeAccessor)))
            {
                lock (httpContext.Items.SyncRoot)
                {
                    if (!httpContext.Items.Contains(typeof(PerRequestScopeAccessor)))
                    {
                        var scope = new DefaultLifetimeScope(new ScopeCache());
                        httpContext.Items[typeof(PerRequestScopeAccessor)] = scope;
                        httpContext.DisposeOnPipelineCompleted(scope);
                        return scope;
                    }
                }
            }
            return (ILifetimeScope)httpContext.Items[typeof(PerRequestScopeAccessor)];
        }
    }
}
