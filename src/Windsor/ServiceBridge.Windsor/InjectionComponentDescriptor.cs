using System;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;

namespace ServiceBridge.Windsor
{
    internal class InjectionComponentDescriptor : IComponentModelDescriptor, IMetaComponentModelDescriptor
    {
        private readonly Type _implementType;
        private readonly IServiceContainer _container;

        public InjectionComponentDescriptor(IServiceContainer container, Type implementType)
        {
            _implementType = implementType;
            _container = container;
        }

        public void BuildComponentModel(IKernel kernel, ComponentModel model)
        {
        }

        public void ConfigureComponentModel(IKernel kernel, ComponentModel model)
        {
            model.Lifecycle.AddFirst(new InjectionConcern(_container, _implementType));
        }
    }
}
