using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Wheatech.ServiceModel.Interception
{
    /// <summary>
    /// Represents a list of either input or output parameters..
    /// </summary>
    public class ParameterCollection : IEnumerable<IMethodParameter>
    {
        private readonly IOrderedDictionary _parameters = new OrderedDictionary();

        /// <summary>
        /// Initialize a new <see cref="ParameterCollection"/> with the given collection
        /// of <see cref="IMethodParameter"/>s.
        /// </summary>
        /// <param name="parameters">The parameters to add to the collection.</param>
        public ParameterCollection(IEnumerable<IMethodParameter> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            foreach (var parameter in parameters)
            {
                _parameters.Add(parameter.Name, parameter);
            }
        }

        /// <summary>
        /// Gets an enumerator object to support the foreach construct.
        /// </summary>
        /// <returns>Enumerator object.</returns>
        public IEnumerator<IMethodParameter> GetEnumerator()
        {
            return _parameters.Values.OfType<IMethodParameter>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets a parameter by index. 
        /// </summary>
        /// <param name="index">The parameter index.</param>
        /// <returns>The <see cref="IMethodParameter"/> object.</returns>
        public virtual IMethodParameter this[int index] => _parameters.Count > index ? (IMethodParameter)_parameters[index] : null;

        /// <summary>
        /// Gets a parameter by name. 
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <returns>The <see cref="IMethodParameter"/> object if there is parameter with the specified name, otherwise null.</returns>
        public virtual IMethodParameter this[string name] => (IMethodParameter)_parameters[name];

        /// <summary>
        /// Checks to see if the collection contains the specified parameter name.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <returns><c>true</c> if the specified parameter name is in collection, <c>false</c> if it is not.</returns>
        public virtual bool Contains(string name) => _parameters.Contains(name);

        /// <summary>
        /// Gets total number of parameters in the collection.
        /// </summary>
        public int Count => _parameters.Count;
    }
}
