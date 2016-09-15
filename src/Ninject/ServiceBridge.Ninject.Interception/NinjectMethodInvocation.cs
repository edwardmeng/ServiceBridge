using System;
using System.Collections.Generic;
using System.Reflection;
using Ninject.Extensions.Interception;
using ServiceBridge.Interception;

namespace ServiceBridge.Ninject.Interception
{
    internal class NinjectMethodInvocation : IMethodInvocation
    {
        private readonly IInvocation _invocation;
        private ParameterCollection _arguments;

        public NinjectMethodInvocation(IInvocation invocation)
        {
            _invocation = invocation;
        }

        public MethodBase Method => _invocation.Request.Method;

        public object Target => _invocation.Request.Target;

        public ParameterCollection Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    var parameters = new List<IMethodParameter>();
                    for (int i = 0; i < _invocation.Request.Arguments.Length; i++)
                    {
                        parameters.Add(new NinjectMethodParameter(_invocation, i));
                    }
                    _arguments = new ParameterCollection(parameters);
                }
                return _arguments;
            }
        }

        public IMethodReturn CreateMethodReturn(object returnValue)
        {
            _invocation.ReturnValue = returnValue;
            return new NinjectMethodReturn(_invocation);
        }

        public IMethodReturn CreateExceptionReturn(Exception exception)
        {
            return new NinjectMethodReturn(_invocation, exception);
        }
    }
}
