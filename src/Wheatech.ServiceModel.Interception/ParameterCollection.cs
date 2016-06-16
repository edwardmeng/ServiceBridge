using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Wheatech.ServiceModel.Interception
{
    public class ParameterCollection : IEnumerable<IMethodParameter>
    {
        private readonly IOrderedDictionary _parameters = new OrderedDictionary();

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

        public IEnumerator<IMethodParameter> GetEnumerator()
        {
            return _parameters.Values.OfType<IMethodParameter>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual IMethodParameter this[int index] => _parameters.Count > index ? (IMethodParameter)_parameters[index] : null;

        public virtual IMethodParameter this[string name] => (IMethodParameter)_parameters[name];

        public virtual bool Contains(string name) => _parameters.Contains(name);

        public int Count => _parameters.Count;
    }
}
