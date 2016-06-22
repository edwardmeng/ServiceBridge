using System.Web;
using Autofac.Core;

namespace Wheatech.ServiceModel.Autofac
{
    internal class PerRequestScopeLifetime: IComponentLifetime
    {
        public ISharingLifetimeScope FindScope(ISharingLifetimeScope mostNestedVisibleScope)
        {
            if (HttpContext.Current == null) return null;
            if (!HttpContext.Current.Items.Contains(typeof(IComponentLifetime)))
            {
                lock (HttpContext.Current.Items.SyncRoot)
                {
                    if (!HttpContext.Current.Items.Contains(typeof(IComponentLifetime)))
                    {
                        var scope = (ISharingLifetimeScope)mostNestedVisibleScope.BeginLifetimeScope();
                        HttpContext.Current.Items[typeof(IComponentLifetime)] = scope;
                        HttpContext.Current.DisposeOnPipelineCompleted(scope);
                        return scope;
                    }
                }
            }
            return (ISharingLifetimeScope) HttpContext.Current.Items[typeof(IComponentLifetime)];
        }
    }
}
