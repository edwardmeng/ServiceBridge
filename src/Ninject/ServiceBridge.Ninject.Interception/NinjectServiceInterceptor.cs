using System;
using System.Reflection;
using Ninject.Extensions.Interception;
using ServiceBridge.Interception;
using IInterceptor = Ninject.Extensions.Interception.IInterceptor;

namespace ServiceBridge.Ninject.Interception
{
    internal class NinjectServiceInterceptor : IInterceptor
    {
        private readonly PipelineManager _pipelineManager;
        private readonly IServiceContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectServiceInterceptor" /> with a pipeline manager.
        /// </summary>
        /// <param name="pipelineManager">The <see cref="PipelineManager" /> for the new instance.</param>
        /// <param name="container">Service container that can be used to resolve interceptors.</param>
        public NinjectServiceInterceptor(PipelineManager pipelineManager, IServiceContainer container)
        {
            _pipelineManager = pipelineManager;
            _container = container;
        }

        private InterceptorPipeline EnsurePipeline(IInvocation invocation)
        {
            var pipeline = _pipelineManager.GetPipeline(invocation.Request.Method);
            if (pipeline == InterceptorPipeline.Empty)
            {
                _pipelineManager.InitializePipeline(invocation.Request.Target.GetType(), _container);
                pipeline = _pipelineManager.GetPipeline(invocation.Request.Method);
            }
            return pipeline;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodReturn = EnsurePipeline(invocation).Invoke(new NinjectMethodInvocation(invocation),
                (injectionInvocation, injectionGetNext) =>
                {
                    try
                    {
                        invocation.Proceed();
                        return new NinjectMethodReturn(invocation);
                    }
                    catch (TargetInvocationException ex)
                    {
                        // The outer exception will always be a reflection exception; we want the inner, which is
                        // the underlying exception.
                        return new NinjectMethodReturn(invocation, ex.InnerException);
                    }
                    catch (Exception ex)
                    {
                        return new NinjectMethodReturn(invocation, ex);
                    }
                });
            if (methodReturn.Exception != null)
            {
                throw methodReturn.Exception;
            }
        }
    }
}
