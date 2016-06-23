using System;
using Castle.Core;
using Castle.MicroKernel;

namespace Wheatech.ServiceModel.Windsor
{
    internal class InjectionConcern: ICommissionConcern
    {
        private readonly IKernel _kernel;
        private readonly Action<IKernel, object> _injectionExpression;

        public InjectionConcern(IKernel kernel, Type implementType)
        {
            _kernel = kernel;
            _injectionExpression = DynamicInjectionBuilder.GetOrCreate(implementType, false, true);
        }

        public void Apply(ComponentModel model, object component)
        {
            _injectionExpression(_kernel, component);
        }
    }
}
