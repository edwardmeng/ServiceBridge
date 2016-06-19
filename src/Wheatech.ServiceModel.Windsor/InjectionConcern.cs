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

        public InjectionConcern(IKernel kernel, Type implementType)
        {
            _kernel = kernel;
            _implementType = implementType;
        }

        public void Apply(ComponentModel model, object component)
        {
            foreach (var method in InjectionAttribute.GetMethods(_implementType))
            {
                var arguments = new List<object>();
                foreach (var parameter in method.GetParameters())
                {
                    if (parameter.IsOut)
                    {
                        arguments.Add(null);
                    }
                    else if (parameter.ParameterType.IsByRef)
                    {
                        arguments.Add(_kernel.Resolve(parameter.ParameterType.GetElementType()));
                    }
                    else
                    {
                        arguments.Add(_kernel.Resolve(parameter.ParameterType));
                    }
                }
                method.Invoke(component, arguments.ToArray());
            }
        }
    }
}
