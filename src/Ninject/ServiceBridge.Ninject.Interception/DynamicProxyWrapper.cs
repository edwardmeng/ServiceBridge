using Castle.DynamicProxy;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Interception.Request;
using Ninject.Extensions.Interception.Wrapper;

namespace ServiceBridge.Ninject.Interception
{

    /// <summary>
    ///     Defines an interception wrapper that can convert a Castle DynamicProxy2 <see cref="IInvocation" />
    ///     into a Ninject <see cref="IRequest" /> for interception.
    /// </summary>
    internal class DynamicProxyWrapper : StandardWrapper, IInterceptor
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicProxyWrapper" /> class.
        /// </summary>
        /// <param name="kernel">The kernel associated with the wrapper.</param>
        /// <param name="context">The context in which the instance was activated.</param>
        /// <param name="instance">The wrapped instance.</param>
        public DynamicProxyWrapper(IKernel kernel, IContext context, object instance)
            : base(kernel, context, instance)
        {
        }

        /// <summary>
        ///     Intercepts the specified invocation.
        /// </summary>
        /// <param name="castleInvocation">The invocation.</param>
        /// <returns>The return value of the invocation, once it is completed.</returns>
        public void Intercept(IInvocation castleInvocation)
        {
            var request = CreateRequest(castleInvocation);
            var invocation = CreateInvocation(request);
            invocation.Proceed();
            castleInvocation.ReturnValue = invocation.ReturnValue;
        }

        private IProxyRequest CreateRequest(IInvocation castleInvocation)
        {
            var requestFactory = Context.Kernel.Components.Get<IProxyRequestFactory>();

            return requestFactory.Create(
                Context,
                castleInvocation.Proxy,
                Instance,
                castleInvocation.GetConcreteMethod(),
                castleInvocation.Arguments,
                castleInvocation.GenericArguments);
        }
    }
}
