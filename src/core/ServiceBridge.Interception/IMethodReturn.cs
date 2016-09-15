using System;

namespace ServiceBridge.Interception
{
    /// <summary>
    /// This interface is used to represent the return value from a method. 
    /// </summary>
    /// <remarks>
    /// An implementation of <see cref="IMethodReturn"/> is returned by interceptors, 
    /// and each interceptor can manipulate the parameters, return value, or add an exception on the way out.
    /// </remarks>
    public interface IMethodReturn
    {
        /// <summary>
        /// If the method threw an exception, the exception object is here. 
        /// </summary>
        Exception Exception { get; set; }

        /// <summary>
        /// The collection of output parameters. If the method has no output parameters, this is a zero-length list (never null). 
        /// </summary>
        ParameterCollection Outputs { get; }

        /// <summary>
        /// Returns value from the method call.
        /// </summary>
        /// <remarks>This value is null if the method has no return value.</remarks>
        object ReturnValue { get; set; }
    }
}
