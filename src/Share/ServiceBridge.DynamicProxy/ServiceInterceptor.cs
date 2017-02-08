using System;
using System.Reflection;
using Castle.DynamicProxy;
using ServiceBridge.Interception;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace ServiceBridge.DynamicProxy
{
    internal class ServiceInterceptor : IInterceptor
    {
        private readonly IServiceContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInterceptor" /> with a pipeline manager.
        /// </summary>
        /// <param name="pipelineManager">The <see cref="PipelineManager" /> for the new instance.</param>
        /// <param name="container">Service container that can be used to resolve interceptors.</param>
        public ServiceInterceptor(PipelineManager pipelineManager, IServiceContainer container)
        {
            PipelineManager = pipelineManager;
            _container = container;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInterceptor" />.
        /// </summary>
        /// <param name="container">Service container that can be used to resolve interceptors.</param>
        public ServiceInterceptor(IServiceContainer container)
        {
            _container = container;
        }

        public PipelineManager PipelineManager { get; set; }

        private InterceptorPipeline EnsurePipeline(IInvocation invocation)
        {
            var pipeline = PipelineManager.GetPipeline(invocation.Method);
            if (pipeline == InterceptorPipeline.Empty)
            {
                PipelineManager.InitializePipeline(invocation.TargetType, _container);
                pipeline = PipelineManager.GetPipeline(invocation.Method);
            }
            return pipeline;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodReturn = EnsurePipeline(invocation).Invoke(new InterceptMethodInvocation(invocation),
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
