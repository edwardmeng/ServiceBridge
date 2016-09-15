namespace ServiceBridge.Interception
{
    /// <summary>
    /// This delegate type is passed to each interceptor's Invoke method. Call the delegate to get the next delegate to call to continue the chain.
    /// </summary>
    /// <returns>Next delegate in the interceptor chain to call.</returns>
    public delegate InvokeInterceptorHandler GetNextInterceptorHandler();

    /// <summary>
    /// This delegate type is the type that points to the next method to execute in the current pipeline. 
    /// </summary>
    /// <param name="input">Inputs to the current method call.</param>
    /// <param name="getNext">Delegate to get the next interceptor in the chain.</param>
    /// <returns>Return from the next method in the chain.</returns>

    public delegate IMethodReturn InvokeInterceptorHandler(IMethodInvocation input, GetNextInterceptorHandler getNext);

    /// <summary>
    /// Interceptors implement this interface and are called for each invocation of the pipelines that they're included in. 
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// Implement this method to execute your interceptor processing.
        /// </summary>
        /// <param name="invocation">Inputs to the current call to the target.</param>
        /// <param name="getNext">Delegate to execute to get the next delegate in the interceptor chain.</param>
        /// <returns>Return value from the target.</returns>
        IMethodReturn Invoke(IMethodInvocation invocation, GetNextInterceptorHandler getNext);
    }
}
