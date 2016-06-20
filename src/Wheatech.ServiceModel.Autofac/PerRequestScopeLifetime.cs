using System.Web;
using Autofac.Core;

namespace Wheatech.ServiceModel.Autofac
{
    internal class PerRequestScopeLifetime: IComponentLifetime
    {
        public ISharingLifetimeScope FindScope(ISharingLifetimeScope mostNestedVisibleScope)
        {
            if (HttpContext.Current == null) return mostNestedVisibleScope;
            var scope = (ISharingLifetimeScope) HttpContext.Current.Items[typeof(IComponentLifetime)];
            if (scope == null)
            {
                HttpContext.Current.Items[typeof(IComponentLifetime)] = scope = (ISharingLifetimeScope)mostNestedVisibleScope.BeginLifetimeScope();
            }
            return scope;
        }
    }
}
