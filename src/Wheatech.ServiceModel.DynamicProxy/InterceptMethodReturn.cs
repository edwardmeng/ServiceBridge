using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.DynamicProxy
{
    public class InterceptMethodReturn : IMethodReturn
    {
        private readonly IInvocation _invocation;
        private ParameterCollection _outputs;

        public InterceptMethodReturn(IInvocation invocation)
        {
            _invocation = invocation;
        }

        public InterceptMethodReturn(IInvocation invocation, Exception exception)
        {
            _invocation = invocation;
            Exception = exception;
        }

        public Exception Exception { get; set; }

        public ParameterCollection Outputs
        {
            get
            {
                if (_outputs == null)
                {
                    var parameters = new List<IMethodParameter>();
                    var methodParameters = _invocation.Method.GetParameters();
                    for (int i = 0; i < methodParameters.Length; i++)
                    {
                        if (methodParameters[i].ParameterType.IsByRef)
                        {
                            parameters.Add(new InterceptMethodParameter(_invocation, i));
                        }
                    }
                    _outputs = new ParameterCollection(parameters);
                }
                return _outputs;
            }
        }

        public object ReturnValue
        {
            get { return _invocation.ReturnValue; }
            set { _invocation.ReturnValue = value; }
        }
    }
}
