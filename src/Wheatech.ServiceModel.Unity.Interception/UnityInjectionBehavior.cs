using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;
using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.Unity.Interception
{
    internal class UnityInjectionBehavior : IInterceptionBehavior
    {
        private readonly PipelineManager _pipelineManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityInjectionBehavior"/> with a pipeline manager.
        /// </summary>
        /// <param name="pipelineManager">The <see cref="PipelineManager"/> for the new instance.</param>
        internal UnityInjectionBehavior(PipelineManager pipelineManager)
        {
            _pipelineManager = pipelineManager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityInjectionBehavior"/> with the given information
        /// about what's being intercepted and the current set of injection policies.
        /// </summary>
        /// <param name="interceptionRequest">Information about what will be injected.</param>
        /// <param name="container">Service container that can be used to resolve call handlers.</param>
        public UnityInjectionBehavior(CurrentInterceptionRequest interceptionRequest, IServiceContainer container)
        {
            if (interceptionRequest == null)
            {
                throw new ArgumentNullException(nameof(interceptionRequest));
            }

            bool hasHandlers = false;

            var manager = new PipelineManager();

            foreach (var method in interceptionRequest.Interceptor.GetInterceptableMethods(interceptionRequest.TypeToIntercept, interceptionRequest.ImplementationType))
            {
                bool hasNewHandlers = manager.InitializePipeline(method, 
                    from attribute in GetInterceptorAttributes(method)
                    orderby attribute.Order
                    select attribute.CreateInterceptor(container));
                hasHandlers = hasHandlers || hasNewHandlers;
            }
            _pipelineManager = hasHandlers ? manager : null;
        }

        private IEnumerable<InterceptorAttribute> GetInterceptorAttributes(MethodImplementationInfo member)
        {
            if (member.InterfaceMethodInfo != null)
            {
                foreach (var attr in InterceptorAttribute.GetAttributes(member.InterfaceMethodInfo, true))
                {
                    yield return attr;
                }
            }
            foreach (var attr in InterceptorAttribute.GetAttributes(member.ImplementationMethodInfo, true))
            {
                yield return attr;
            }
        }

        /// <summary>
        /// Execute behavior processing.
        /// </summary>
        /// <param name="input">Inputs to the current call to the target.</param>
        /// <param name="getNext">Delegate to execute to get the next delegate in the behavior chain.</param>
        /// <returns>Return value from the target.</returns>
        public Microsoft.Practices.Unity.InterceptionExtension.IMethodReturn Invoke(
            Microsoft.Practices.Unity.InterceptionExtension.IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            Func<Microsoft.Practices.Unity.InterceptionExtension.IMethodInvocation, Microsoft.Practices.Unity.InterceptionExtension.IMethodReturn> defaultInvoke = invocation =>
            {
                try
                {
                    return getNext()(invocation, getNext);
                }
                catch (TargetInvocationException ex)
                {
                    // The outer exception will always be a reflection exception; we want the inner, which is
                    // the underlying exception.
                    return invocation.CreateExceptionMethodReturn(ex.InnerException);
                }
            };
            if (_pipelineManager == null) return defaultInvoke(input);
            var methodReturn = _pipelineManager.GetPipeline(input.MethodBase).Invoke(new UnityMethodInvocation(input),
                (injectionInvocation, injectionGetNex) =>
                    new UnityMethodReturn(defaultInvoke(((UnityMethodInvocation) injectionInvocation).Unwrap())));
            return ((UnityMethodReturn) methodReturn).Unwrap();
        }

        /// <summary>
        /// Returns the interfaces required by the behavior for the objects it intercepts.
        /// </summary>
        /// <returns>The required interfaces.</returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        /// <summary>
        /// Returns a flag indicating if this behavior will actually do anything when invoked.
        /// </summary>
        /// <remarks>This is used to optimize interception. If the behaviors won't actually
        /// do anything (for example, PIAB where no policies match) then the interception
        /// mechanism can be skipped completely.</remarks>
        public bool WillExecute => true;
    }
}
