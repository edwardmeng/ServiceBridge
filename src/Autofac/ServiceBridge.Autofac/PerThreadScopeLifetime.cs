using System;
using System.Threading;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;

namespace ServiceBridge.Autofac
{
    internal class PerThreadScopeLifetime : IComponentLifetime
    {
        private static readonly ThreadLocal<ILifetimeScope> _threadScope = new ThreadLocal<ILifetimeScope>();

        public ISharingLifetimeScope FindScope(ISharingLifetimeScope mostNestedVisibleScope)
        {
            if (_threadScope.Value == null)
            {
                _threadScope.Value = new LifetimeScope(mostNestedVisibleScope.ComponentRegistry);
            }
            return (ISharingLifetimeScope)_threadScope.Value;
        }
    }
}
