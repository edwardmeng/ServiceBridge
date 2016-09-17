using Autofac.Core;
#if NetCore
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace ServiceBridge.Autofac
{
    internal class PerRequestScopeLifetime : IComponentLifetime
    {
        public ISharingLifetimeScope FindScope(ISharingLifetimeScope mostNestedVisibleScope)
        {
            var context = ServiceContainer.HostContext as HttpContext;
            if (context == null) return null;
#if NetCore
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