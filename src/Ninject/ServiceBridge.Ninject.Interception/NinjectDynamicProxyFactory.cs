using System;
using System.Linq;
using Castle.DynamicProxy;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Interception.Parameters;
using Ninject.Extensions.Interception.ProxyFactory;
using Ninject.Extensions.Interception.Wrapper;
using Ninject.Infrastructure;
using Ninject.Parameters;

namespace ServiceBridge.Ninject.Interception
{
    /// <summary>
    ///     The default implementation of a proxy factory that uses a Castle DynamicProxy2 <see cref="ProxyGenerator" />.
    /// </summary>
    internal class NinjectDynamicProxyFactory : ProxyFactoryBase, IHaveKernel
    {
        private static readonly ProxyGenerationOptions ProxyOptions = ProxyGenerationOptions.Default;

        private static readonly ProxyGenerationOptions InterfaceProxyOptions = new ProxyGenerationOptions
        {
            BaseTypeForInterfaceProxy = typeof(ProxyBase)
        };

        private ProxyGenerator _generator = new ProxyGenerator();

        /// <summary>
        ///     Initializes a new instance of the <see cref="NinjectDynamicProxyFactory" /> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public NinjectDynamicProxyFactory(IKernel kernel)
        {
            Kernel = kernel;
        }

        /// <summary>
        ///     Gets the kernel.
        /// </summary>
        public IKernel Kernel { get; }

        /// <summary>
        ///     Releases all resources held by the object.
        /// </summary>
        /// <param name="disposing">
        ///     <see langword="True" /> if managed objects should be disposed, otherwise
        ///     <see langword="false" />.
        /// </param>
        public override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
                _generator = null;

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Wraps the specified instance in a proxy.
        /// </summary>
        /// <param name="context">The context in which the instance was activated.</param>
        /// <param name="reference">The <see cref="InstanceReference" /> to wrap.</param>
        public override void Wrap(IContext context, InstanceReference reference)
        {
            if (reference.Instance is IInterceptor ||
                reference.Instance is IProxyTargetAccessor)
                return;

            var wrapper = new DynamicProxyWrapper(Kernel, context, reference.Instance);

            var targetType = context.Request.Service;
            var additionalInterfaces =
                context.Parameters.OfType<AdditionalInterfaceParameter>().Select(ai => (Type) ai.GetValue(context, null)).ToArray();
            if (targetType.IsInterface)
            {
                reference.Instance = _generator.CreateInterfaceProxyWithoutTarget(targetType, additionalInterfaces, InterfaceProxyOptions, wrapper);
            }
            else
            {
                var parameters = context.Parameters.OfType<ConstructorArgument>()
                    .Select(parameter => parameter.GetValue(context, null))
                    .ToArray();
                reference.Instance = _generator.CreateClassProxy(targetType, additionalInterfaces, ProxyOptions, parameters, wrapper);
            }
        }

        /// <summary>
        ///     Unwraps the specified proxied instance.
        /// </summary>
        /// <param name="context">The context in which the instance was activated.</param>
        /// <param name="reference">The <see cref="InstanceReference" /> to unwrap.</param>
        public override void Unwrap(IContext context, InstanceReference reference)
        {
            var accessor = reference.Instance as IProxyTargetAccessor;

            var interceptors = accessor?.GetInterceptors();

            if (interceptors == null || interceptors.Length == 0)
                return;

            var wrapper = interceptors[0] as IWrapper;

            if (wrapper == null)
                return;

            reference.Instance = wrapper.Instance;
        }
    }
}