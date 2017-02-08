using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ServiceBridge.Interception
{
    /// <summary>
    /// A collection of <see cref="InterceptorPipeline"/> objects, indexed
    /// by <see cref="MethodBase"/>. Returns an empty pipeline if a
    /// MethodBase is requested that isn't in the dictionary.
    /// </summary>
    public class PipelineManager
    {
        private readonly Dictionary<InterceptorPipelineKey, InterceptorPipeline> _pipelines =
            new Dictionary<InterceptorPipelineKey, InterceptorPipeline>();

        private readonly IInterceptorFactory _interceptorFactory;

        /// <summary>
        /// Initialize new instance of <see cref="PipelineManager"/> with the specified interceptor factory.
        /// </summary>
        /// <param name="interceptorFactory"></param>
        public PipelineManager(IInterceptorFactory interceptorFactory)
        {
            if (interceptorFactory == null)
            {
                throw new ArgumentNullException(nameof(interceptorFactory));
            }
            _interceptorFactory = interceptorFactory;
        }

        /// <summary>
        /// Retrieve the pipeline associated with the requested <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The methodBase for which the pipeline is being requested.</param>
        /// <returns>The handler pipeline for the given methodBase. If no pipeline has
        /// been set, returns a new empty pipeline.</returns>
        public InterceptorPipeline GetPipeline(MethodBase method)
        {
            InterceptorPipeline pipeline;
            return _pipelines.TryGetValue(InterceptorPipelineKey.ForMethod(method), out pipeline) ? pipeline : InterceptorPipeline.Empty;
        }

        /// <summary>
        /// Set a new pipeline for a methodBase.
        /// </summary>
        /// <param name="method">The methodBase on which the pipeline should be set.</param>
        /// <param name="pipeline">The new pipeline.</param>
        public void SetPipeline(MethodBase method, InterceptorPipeline pipeline)
        {
            _pipelines[InterceptorPipelineKey.ForMethod(method)] = pipeline;
        }

        /// <summary>
        /// Initialize the pipeline for the given method, creating it if necessary.
        /// </summary>
        /// <param name="interfaceMethod"><see cref="MethodInfo"/> for the interface method (may be null if no interface).</param>
        /// <param name="implementMethod"><see cref="MethodInfo"/> for implementing method.</param>
        /// <param name="container">Service container that can be used to resolve interceptors.</param>
        /// <returns>True if the pipeline has any interceptor in it, false if not.</returns>
        public bool InitializePipeline(MethodInfo interfaceMethod, MethodInfo implementMethod, IServiceContainer container)
        {
            if (implementMethod == null)
            {
                throw new ArgumentNullException(nameof(implementMethod));
            }

            var pipeline = CreatePipeline(implementMethod, _interceptorFactory.CreateInterceptors(interfaceMethod, implementMethod, container));
            if (interfaceMethod != null)
            {
                _pipelines[InterceptorPipelineKey.ForMethod(interfaceMethod)] = pipeline;
            }

            return pipeline.Count > 0;
        }

        /// <summary>
        /// Initialize the pipeline for the given constructor, creating it if necessary.
        /// </summary>
        /// <param name="constructor">The <see cref="ConstructorInfo"/> for the implementing constructor.</param>
        /// <param name="container">Service container that can be used to resolve interceptors.</param>
        /// <returns>True if the pipeline has any interceptor in it, false if not.</returns>
        public bool InitializePipeline(ConstructorInfo constructor, IServiceContainer container)
        {
            if (constructor == null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }
            var pipeline = CreatePipeline(constructor, _interceptorFactory.CreateInterceptors(constructor, container));
            return pipeline.Count > 0;
        }

        /// <summary>
        /// Initialize the pipeline for the given type, creating it if necessary.
        /// </summary>
        /// <param name="implementType">The implementation type.</param>
        /// <param name="container">Service container that can be used to resolve interceptors.</param>
        public void InitializePipeline(Type implementType, IServiceContainer container)
        {
            var methodMappings = new Dictionary<MethodInfo, MethodInfo>();
#if NetCore
            foreach (var itf in implementType.GetTypeInfo().ImplementedInterfaces)
            {
                var mapping = implementType.GetTypeInfo().GetRuntimeInterfaceMap(itf);
                for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
                {
                    if (!methodMappings.ContainsKey(mapping.TargetMethods[i]))
                    {
                        methodMappings[mapping.TargetMethods[i]] = mapping.InterfaceMethods[i];
                        InitializePipeline(mapping.InterfaceMethods[i], mapping.TargetMethods[i], container);
                    }
                }
            }
            foreach (var constructor in implementType.GetTypeInfo().DeclaredConstructors)
            {
                InitializePipeline(constructor, container);
            }
#else
            foreach (Type itf in implementType.GetInterfaces())
            {
                var mapping = implementType.GetInterfaceMap(itf);
                for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
                {
                    if (!methodMappings.ContainsKey(mapping.TargetMethods[i]))
                    {
                        methodMappings[mapping.TargetMethods[i]] = mapping.InterfaceMethods[i];
                        InitializePipeline(mapping.InterfaceMethods[i], mapping.TargetMethods[i], container);
                    }
                }
            }
            foreach (var constructor in implementType.GetConstructors())
            {
                InitializePipeline(constructor, container);
            }
#endif
        }

        private InterceptorPipeline CreatePipeline(MethodBase methodBase, IEnumerable<IInterceptor> interceptors)
        {
            InterceptorPipelineKey key = InterceptorPipelineKey.ForMethod(methodBase);
            if (_pipelines.ContainsKey(key))
            {
                return _pipelines[key];
            }
            InterceptorPipeline pipeline;
#if NetCore
            if (methodBase.IsConstructor)
#else
            if (methodBase.MemberType == MemberTypes.Constructor)
#endif
            {
                pipeline = new InterceptorPipeline(interceptors);
                _pipelines[key] = pipeline;
                return pipeline;
            }
            var method = (MethodInfo) methodBase;
#if NetCore
            var baseMethod = method.GetRuntimeBaseDefinition();
#else
            var baseMethod = method.GetBaseDefinition();
#endif
            if (baseMethod == null || Equals(baseMethod, method))
            {
                pipeline = new InterceptorPipeline(interceptors);
                _pipelines[key] = pipeline;
                return pipeline;
            }

            pipeline = CreatePipeline(baseMethod, interceptors);
            _pipelines[key] = pipeline;
            return pipeline;
        }
    }
}
