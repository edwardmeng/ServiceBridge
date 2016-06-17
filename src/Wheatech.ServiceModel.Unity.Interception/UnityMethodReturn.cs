using System;
using System.Collections.Generic;
using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.Unity.Interception
{
    internal class UnityMethodReturn : IMethodReturn
    {
        private readonly Microsoft.Practices.Unity.InterceptionExtension.IMethodReturn _return;
        private ParameterCollection _outputs;

        public UnityMethodReturn(Microsoft.Practices.Unity.InterceptionExtension.IMethodReturn @return)
        {
            _return = @return;
        }

        public Exception Exception
        {
            get { return _return.Exception; }
            set { _return.Exception = value; }
        }

        public ParameterCollection Outputs
        {
            get
            {
                if (_outputs == null)
                {
                    var parameters = new List<IMethodParameter>();
                    for (int i = 0; i < _return.Outputs.Count; i++)
                    {
                        parameters.Add(new UnitMethodReturnParameter(_return,i));
                    }
                    _outputs = new ParameterCollection(parameters);
                }
                return _outputs;
            }
        }

        public object ReturnValue
        {
            get { return _return.ReturnValue; }
            set { _return.ReturnValue = value; }
        }

        public Microsoft.Practices.Unity.InterceptionExtension.IMethodReturn Unwrap()
        {
            return _return;
        }
    }
}
