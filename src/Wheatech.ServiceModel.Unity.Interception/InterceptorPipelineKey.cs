using System;
using System.Reflection;

namespace Wheatech.ServiceModel.Unity.Interception
{
    /// <summary>
    /// Key for interceptor pipelines.
    /// </summary>
    internal class InterceptorPipelineKey
    {
        private readonly Module _module;
        private readonly int _metadataToken;

        /// <summary>
        /// Creates a new <see cref="InterceptorPipelineKey"/> for the supplied method.
        /// </summary>
        /// <param name="methodBase">The method for the key.</param>
        /// <returns>The new key.</returns>
        public static InterceptorPipelineKey ForMethod(MethodBase methodBase)
        {
            if (methodBase == null)
            {
                throw new ArgumentNullException(nameof(methodBase));
            }

            return new InterceptorPipelineKey(methodBase.DeclaringType?.Module, methodBase.MetadataToken);
        }

        private InterceptorPipelineKey(Module module, int metadataToken)
        {
            _module = module;
            _metadataToken = metadataToken;
        }

        /// <summary>
        /// Compare two <see cref="InterceptorPipelineKey"/> instances.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True if the two keys are equal, false if not.</returns>
        public override bool Equals(object obj)
        {
            var key = obj as InterceptorPipelineKey;
            if (key == null) return false;
            return this == key;
        }

        /// <summary>
        /// Calculate a hash code for this instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return (_module?.GetHashCode() ?? 0) ^ _metadataToken;
        }

        /// <summary>
        /// Compare two <see cref="InterceptorPipelineKey"/> instances for equality.
        /// </summary>
        /// <param name="left">First of the two keys to compare.</param>
        /// <param name="right">Second of the two keys to compare.</param>
        /// <returns>True if the values of the keys are the same, else false.</returns>
        public static bool operator ==(InterceptorPipelineKey left, InterceptorPipelineKey right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null)) return false;
            return left._module == right._module &&
                   left._metadataToken == right._metadataToken;
        }

        /// <summary>
        /// Compare two <see cref="InterceptorPipelineKey"/> instances for inequality.
        /// </summary>
        /// <param name="left">First of the two keys to compare.</param>
        /// <param name="right">Second of the two keys to compare.</param>
        /// <returns>false if the values of the keys are the same, else true.</returns>
        public static bool operator !=(InterceptorPipelineKey left, InterceptorPipelineKey right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Compare two <see cref="InterceptorPipelineKey"/> instances.
        /// </summary>
        /// <param name="other">Object to compare to.</param>
        /// <returns>True if the two keys are equal, false if not.</returns>
        public bool Equals(InterceptorPipelineKey other)
        {
            return this == other;
        }
    }
}
