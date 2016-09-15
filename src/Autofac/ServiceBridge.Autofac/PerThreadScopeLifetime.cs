using System;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;

namespace ServiceBridge.Autofac
{
    internal class PerThreadScopeLifetime : IComponentLifetime
    {
        [ThreadStatic]
        private static ILifetimeScope _threadScope;

        public ISharingLifetimeScope FindScope(ISharingLifetimeScope mostNestedVisibleScope)
        {
            return (ISharingLifetimeScope)(_threadScope ?? (_threadScope = new LifetimeScope(mostNestedVisibleScope.ComponentRegistry)));
        }
    }
}
