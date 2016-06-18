using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;
using Wheatech.ServiceModel.Interception;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace Wheatech.ServiceModel.DynamicProxy
{
    public class ServiceInterceptorSelector : IInterceptorSelector
    {
        private readonly IServiceContainer _container;

        public ServiceInterceptorSelector(IServiceContainer container)
        {
            _container = container;
        }

        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            return new List<IInterceptor>(interceptors) { new ServiceInterceptor(_container.GetInstance<PipelineManager>(), _container) }.ToArray();
        }
    }
}
