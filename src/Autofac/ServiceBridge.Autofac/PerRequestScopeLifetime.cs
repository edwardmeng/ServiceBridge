using Autofac.Core;
#if NetCore
using Microsoft.AspNetCore.Http;
#endif

namespace ServiceBridge.Autofac
{
    internal class PerRequestScopeLifetime : IComponentLifetime
    {
        private readonly IServiceContainer _container;

        public PerRequestScopeLifetime(IServiceContainer container)
        {
            _container = container;
        }

        public ISharingLifetimeScope FindScope(ISharingLifetimeScope mostNestedVisibleScope)
        {
#if NetCore
            var context = _container.GetInstance<IHttpContextAccessor>()?.HttpContext;
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
            return (ISharingLifetimeScope)context.Items[typeof(IComponentLifetime)];
#else
            var context = System.Web.HttpContext.Current;
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
            return (ISharingLifetimeScope)context.Items[typeof(IComponentLifetime)];
#endif
        }
    }
}