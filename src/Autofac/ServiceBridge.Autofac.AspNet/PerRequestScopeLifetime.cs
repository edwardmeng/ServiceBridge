using Autofac.Core;
#if NetCore
using ServiceBridge.AspNetCore;
#else
using System.Web;
#endif

namespace ServiceBridge.Autofac.AspNet
{
    internal class PerRequestScopeLifetime : IComponentLifetime
    {
        public ISharingLifetimeScope FindScope(ISharingLifetimeScope mostNestedVisibleScope)
        {
#if NetCore
            var context = ServiceBridgeMiddleware.CurrentContext;
            if (context == null) return null;
            if (!context.Items.ContainsKey(typeof(IComponentLifetime)))
            {
                lock (context)
                {
                    if (!context.Items.ContainsKey(typeof(IComponentLifetime)))
                    {
                        var scope = (ISharingLifetimeScope)mostNestedVisibleScope.BeginLifetimeScope(mostNestedVisibleScope.ComponentRegistry);
                        context.Items[typeof(IComponentLifetime)] = scope;
                        return scope;
                    }
                }
            }
#else
            var context = HttpContext.Current;
            if (context == null) return null;
            if (!context.Items.Contains(typeof(IComponentLifetime)))
            {
                lock (context.Items.SyncRoot)
                {
                    if (!context.Items.Contains(typeof(IComponentLifetime)))
                    {
                        var scope = (ISharingLifetimeScope)mostNestedVisibleScope.BeginLifetimeScope(mostNestedVisibleScope.ComponentRegistry);
                        context.Items[typeof(IComponentLifetime)] = scope;
                        context.DisposeOnPipelineCompleted(scope);
                        return scope;
                    }
                }
            }
#endif
            return (ISharingLifetimeScope)context.Items[typeof(IComponentLifetime)];
        }
    }
}