using System;
using Autofac;
using Autofac.Core;

namespace Wheatech.ServiceModel.Autofac
{
    internal class PerThreadScopeLifetime : IComponentLifetime
    {
        [ThreadStatic]
        private static ISharingLifetimeScope _threadScope;

        public ISharingLifetimeScope FindScope(ISharingLifetimeScope mostNestedVisibleScope)
        {

            return _threadScope ?? (_threadScope = (ISharingLifetimeScope)mostNestedVisibleScope.BeginLifetimeScope());
        }
    }
}
