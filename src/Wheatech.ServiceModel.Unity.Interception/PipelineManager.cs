using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;
using IInterceptor = Wheatech.ServiceModel.Interception.IInterceptor;

namespace Wheatech.ServiceModel.Unity.Interception
{
    /// <summary>
    /// A collection of <see cref="InterceptorPipeline"/> objects, indexed
    /// by <see cref="MethodBase"/>. Returns an empty pipeline if a
    /// MethodBase is requested that isn't in the dictionary.
    /// </summary>
    internal class PipelineManager
    {
        private readonly Dictionary<InterceptorPipelineKey, InterceptorPipeline> _pipelines =
            new Dictionary<InterceptorPipelineKey, InterceptorPipeline>();

        /// <summary>
        /// Retrieve the pipeline associated with the requested <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The method for which the pipeline is being requested.</param>
        /// <returns>The handler pipeline for the given method. If no pipeline has
        /// been set, returns a new empty pipeline.</returns>
        public InterceptorPipeline GetPipeline(MethodBase method)
        {
            InterceptorPipeline pipeline;
            return _pipelines.TryGetValue(InterceptorPipelineKey.ForMethod(method), out pipeline) ? pipeline : InterceptorPipeline.Empty;
        }

        /// <summary>
        /// Set a new pipeline for a method.
        /// </summary>
        /// <param name="method">The method on which the pipeline should be set.</param>
        /// <param name="pipeline">The new pipeline.</param>
        public void SetPipeline(MethodBase method, InterceptorPipeline pipeline)
        {
            _pipelines[InterceptorPipelineKey.ForMethod(method)] = pipeline;
        }

        /// <summary>
        /// Get the pipeline for the given method, creating it if necessary.
        /// </summary>
        /// <param name="method">Method to retrieve the pipeline for.</param>
        /// <param name="interceptors">Interceptors to initialize the pipeline with</param>
        /// <returns>True if the pipeline has any interceptor in it, false if not.</returns>
        public bool InitializePipeline(MethodImplementationInfo method, IEnumerable<IInterceptor> interceptors)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var pipeline = CreatePipeline(method.ImplementationMethodInfo, interceptors);
            if (method.InterfaceMethodInfo != null)
            {
                _pipelines[InterceptorPipelineKey.ForMethod(method.InterfaceMethodInfo)] = pipeline;
            }

            return pipeline.Count > 0;
        }

        private InterceptorPipeline CreatePipeline(MethodInfo method, IEnumerable<IInterceptor> interceptors)
        {
            InterceptorPipelineKey key = InterceptorPipelineKey.ForMethod(method);
            if (_pipelines.ContainsKey(key))
            {
                return _pipelines[key];
            }

            if (method.GetBaseDefinition() == method)
            {
                _pipelines[key] = new InterceptorPipeline(interceptors);
                return _pipelines[key];
            }

            var basePipeline = CreatePipeline(method.GetBaseDefinition(), interceptors);
            _pipelines[key] = basePipeline;
            return basePipeline;
        }
    }
}
