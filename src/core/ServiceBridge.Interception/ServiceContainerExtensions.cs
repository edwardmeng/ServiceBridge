using System;

namespace ServiceBridge.Interception
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the <see cref="IServiceContainer"/> interface.
    /// </summary>
    public static class ServiceContainerExtensions
    {
        private static CompositeInterceptorFactory EnsureInterceptorFactory(IServiceContainer container)
        {
            if (!container.IsRegistered<IInterceptorFactory>())
            {
                container.RegisterInstance<IInterceptorFactory>(CompositeInterceptorFactory.Default);
            }
            return CompositeInterceptorFactory.Default;
        }

        /// <summary>
        /// Make the <paramref name="container"/> to use the default interceptor factory.
        /// </summary>
        /// <param name="container">The container to add interceptor factory.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        public static IServiceContainer UseDefaultInterceptorFactory(this IServiceContainer container)
        {
            return container.UseInterceptorFactory<DefaultInterceptorFactory>();
        }

        /// <summary>
        /// Specify the interceptor factory to be used by the <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="T">The requested type of the interceptor factory.</typeparam>
        /// <param name="container">The container to add interceptor factory.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        public static IServiceContainer UseInterceptorFactory<T>(this IServiceContainer container)
            where T: IInterceptorFactory, new ()
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            EnsureInterceptorFactory(container).TryAddFactory<T>();
            return container;
        }

        /// <summary>
        /// Specify the interceptor factory to be used by the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The container to add interceptor factory.</param>
        /// <param name="factory">The requested interceptor factory.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        public static IServiceContainer UseInterceptorFactory(this IServiceContainer container, IInterceptorFactory factory)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            EnsureInterceptorFactory(container).TryAddFactory(factory);
            return container;
        }
    }
}
