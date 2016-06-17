using System.Collections.Generic;

namespace Wheatech.ServiceModel.Interception
{
    /// <summary>
    /// The InterceptorPipeline class encapsulates a list of <see cref="IInterceptor"/>s
    /// and manages calling them in the proper order with the right inputs.
    /// </summary>
    public class InterceptorPipeline
    {
        public static readonly InterceptorPipeline Empty = new InterceptorPipeline();

        private readonly List<IInterceptor> _handlers;

        /// <summary>
        /// Creates a new <see cref="InterceptorPipeline"/> with an empty pipeline.
        /// </summary>
        public InterceptorPipeline()
        {
            _handlers = new List<IInterceptor>();
        }

        /// <summary>
        /// Creates a new <see cref="InterceptorPipeline"/> with the given collection
        /// of <see cref="IInterceptor"/>s.
        /// </summary>
        /// <param name="handlers">Collection of handlers to add to the pipeline.</param>
        public InterceptorPipeline(IEnumerable<IInterceptor> handlers)
        {
            _handlers = new List<IInterceptor>(handlers);
        }

        /// <summary>
        /// Get the number of handlers in this pipeline.
        /// </summary>
        public int Count => _handlers.Count;

        /// <summary>
        /// Execute the pipeline with the given input.
        /// </summary>
        /// <param name="input">Input to the method call.</param>
        /// <param name="target">The ultimate target of the call.</param>
        /// <returns>Return value from the pipeline.</returns>
        public IMethodReturn Invoke(IMethodInvocation input, InvokeInterceptorHandler target)
        {
            if (_handlers.Count == 0)
            {
                return target(input, null);
            }
            var handlerIndex = 0;

            return _handlers[0].Invoke(input, delegate
            {
                ++handlerIndex;
                return handlerIndex < _handlers.Count ? _handlers[handlerIndex].Invoke : target;
            });
        }
    }
}
