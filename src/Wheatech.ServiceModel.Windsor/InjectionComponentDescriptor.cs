using System;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;

namespace Wheatech.ServiceModel.Windsor
{
    internal class InjectionComponentDescriptor : IComponentModelDescriptor, IMetaComponentModelDescriptor
    {
        private readonly Type _implementType;

        public InjectionComponentDescriptor(Type implementType)
        {
            _implementType = implementType;
        }

        public void BuildComponentModel(IKernel kernel, ComponentModel model)
        {
        }

        public void ConfigureComponentModel(IKernel kernel, ComponentModel model)
        {
            model.Lifecycle.AddFirst(new InjectionConcern(kernel, _implementType));
        }
    }
}
