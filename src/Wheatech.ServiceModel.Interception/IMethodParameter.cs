using System.Reflection;

namespace Wheatech.ServiceModel.Interception
{
    /// <summary>
    /// This interface is used to represent the parameter when call to a method. 
    /// </summary>
    public interface IMethodParameter
    {
        /// <summary>
        /// Gets the <see cref="ParameterInfo"/> for the parameter.
        /// </summary>
        /// <value>ParameterInfo object describing the parameter.</value>
        ParameterInfo ParameterInfo { get; }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        object Value { get; set; }
    }
}
