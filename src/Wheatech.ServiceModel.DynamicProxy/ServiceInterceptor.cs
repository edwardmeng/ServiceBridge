using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;
using Wheatech.ServiceModel.Interception;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace Wheatech.ServiceModel.DynamicProxy
{
    public class ServiceInterceptor : IInterceptor
    {
        private readonly PipelineManager _pipelineManager;
        private readonly IServiceContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInterceptor" /> with a pipeline manager.
        /// </summary>
        /// <param name="pipelineManager">The <see cref="PipelineManager" /> for the new instance.</param>
        /// <param name="container">Service container that can be used to resolve interceptors.</param>
        public ServiceInterceptor(PipelineManager pipelineManager, IServiceContainer container)
        {
            _pipelineManager = pipelineManager;
            _container = container;
        }

        private void InitializePipelines(Type implementationType)
        {
            var methodMappings = new Dictionary<MethodInfo, MethodInfo>();
            foreach (Type itf in implementationType.GetInterfaces())
            {
                var mapping = implementationType.GetInterfaceMap(itf);
                for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
                {
                    if (!methodMappings.ContainsKey(mapping.TargetMethods[i]))
                    {
                        methodMappings[mapping.TargetMethods[i]] = mapping.InterfaceMethods[i];
                        _pipelineManager.InitializePipeline(mapping.InterfaceMethods[i], mapping.TargetMethods[i], _container);
                    }
                }
            }
        }

        public void Intercept(IInvocation invocation)
        {
            var pipeline = _pipelineManager.GetPipeline(invocation.Method);
            if (pipeline == InterceptorPipeline.Empty)
            {
                InitializePipelines(invocation.TargetType);
                pipeline= _pipelineManager.GetPipeline(invocation.Method);
            }
            var methodReturn = pipeline.Invoke(new InterceptMethodInvocation(invocation),
                (injectionInvocation, injectionGetNext) =>
                {
                    try
                    {
                        invocation.Proceed();
                        return new InterceptMethodReturn(invocation);
                    }
                    catch (TargetInvocationException ex)
                    {
                        // The outer exception will always be a reflection exception; we want the inner, which is
                        // the underlying exception.
                        return new InterceptMethodReturn(invocation, ex.InnerException);
                    }
                    catch (Exception ex)
                    {
                        return new InterceptMethodReturn(invocation, ex);
                    }
                });
            if (methodReturn.Exception != null)
            {
                throw methodReturn.Exception;
            }
        }
    }
}
