using System;
using System.Collections.Generic;
using Castle.Core;
using Castle.MicroKernel;

namespace Wheatech.ServiceModel.Windsor
{
    internal class InjectionConcern: ICommissionConcern
    {
        private readonly IKernel _kernel;
        private readonly Type _implementType;
        private readonly Action<IKernel, object> _injectionExpression;

        public InjectionConcern(IKernel kernel, Type implementType)
        {
            _kernel = kernel;
            _implementType = implementType;
            _injectionExpression = new DynamicInjectionBuilder(implementType).Build();
        }

        public void Apply(ComponentModel model, object component)
        {
            _injectionExpression(_kernel, component);
        }
    }
}
