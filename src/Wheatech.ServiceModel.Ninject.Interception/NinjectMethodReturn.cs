using System;
using System.Collections.Generic;
using Ninject.Extensions.Interception;
using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.Ninject.Interception
{
    internal class NinjectMethodReturn : IMethodReturn
    {
        private readonly IInvocation _invocation;
        private ParameterCollection _outputs;

        public NinjectMethodReturn(IInvocation invocation)
        {
            _invocation = invocation;
        }

        public NinjectMethodReturn(IInvocation invocation, Exception exception)
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
                    var methodParameters = _invocation.Request.Method.GetParameters();
                    for (int i = 0; i < methodParameters.Length; i++)
                    {
                        if (methodParameters[i].ParameterType.IsByRef)
                        {
                            parameters.Add(new NinjectMethodParameter(_invocation, i));
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
