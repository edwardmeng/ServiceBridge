using System.Collections.Generic;
using System.Reflection;

namespace ServiceBridge.Interception
{
    /// <summary>
    /// The interface is used to represent the factory to create interceptors.
    /// </summary>
    public interface IInterceptorFactory
    {
        /// <summary>
        /// Creates the ordered interceptors for the given implementation method and its interface method if exists.
        /// </summary>
        /// <param name="interfaceMethod"><see cref="MethodInfo"/> for the interface method (may be null if no interface).</param>
        /// <param name="implementMethod"><see cref="MethodInfo"/> for implementing method.</param>
        /// <param name="container">Service container that can be used to resolve interceptors.</param>
        /// <returns>The ordered interceptors for the given method.</returns>
        IEnumerable<IInterceptor> CreateInterceptors(MethodInfo interfaceMethod, MethodInfo implementMethod, IServiceContainer container);

        /// <summary>
        /// Creates the ordered interceptors for the given contructor method.
        /// </summary>
        /// <param name="constructor"><see cref="ConstructorInfo"/> for the intercepting contructor.</param>
        /// <param name="container">Service container that can be used to resolve interceptors.</param>
        /// <returns>The ordered interceptors for the given constructor.</returns>
        IEnumerable<IInterceptor> CreateInterceptors(ConstructorInfo constructor, IServiceContainer container);
    }
}
