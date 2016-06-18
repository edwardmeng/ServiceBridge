using System;
using System.Reflection;

namespace Wheatech.ServiceModel.Interception
{
    /// <summary>
    /// This interface is used to represent the call to a method. 
    /// </summary>
    /// <remarks>
    /// An implementation of <see cref="IMethodInvocation"/> is passed to the interceptors 
    /// so that they may manipulate the call (typically by changing the parameters) before the final target gets called. 
    /// </remarks>
    public interface IMethodInvocation
    {
        /// <summary>
        /// The method on <see cref="Target"/> that we're aiming at.
        /// </summary>
        MethodBase Method { get; }

        /// <summary>
        /// The object that the call is made on.
        /// </summary>
        object Target { get; }

        /// <summary>
        /// Collection of all parameters to the call: in, out and byref. 
        /// </summary>
        ParameterCollection Arguments { get; }

        /// <summary>
        /// Factory method that creates the correct implementation of <see cref="IMethodReturn"/>.
        /// </summary>
        /// <param name="returnValue">Return value to be placed in the <see cref="IMethodReturn"/> object.</param>
        /// <returns>New <see cref="IMethodReturn"/> object.</returns>
        IMethodReturn CreateMethodReturn(object returnValue);

        /// <summary>
        /// Factory method that creates the correct implementation of <see cref="IMethodReturn"/> in the presence of an exception. 
        /// </summary>
        /// <param name="exception">Exception to be set into the returned object.</param>
        /// <returns>New <see cref="IMethodReturn"/> object.</returns>
        IMethodReturn CreateExceptionReturn(Exception exception);
    }
}