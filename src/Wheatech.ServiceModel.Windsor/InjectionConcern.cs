using System;
using Castle.Core;
using Wheatech.ServiceModel.DynamicInjection;

namespace Wheatech.ServiceModel.Windsor
{
    internal class InjectionConcern: ICommissionConcern
    {
        private readonly IServiceContainer _container;
        private readonly Action<IServiceContainer, object> _injectionExpression;

        public InjectionConcern(IServiceContainer container, Type implementType)
        {
            _container = container;
            _injectionExpression = DynamicInjectionBuilder.GetOrCreate(implementType, false, true);
        }

        public void Apply(ComponentModel model, object component)
        {
            _injectionExpression(_container, component);
        }
    }
}
