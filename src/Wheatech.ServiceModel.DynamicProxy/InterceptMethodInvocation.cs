using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;
using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.DynamicProxy
{
    public class InterceptMethodInvocation : IMethodInvocation
    {
        private readonly IInvocation _invocation;
        private ParameterCollection _arguments;

        public InterceptMethodInvocation(IInvocation invocation)
        {
            _invocation = invocation;
        }

        public MethodBase MethodBase => _invocation.Method;

        public object Target => _invocation.InvocationTarget;

        public ParameterCollection Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    var parameters = new List<IMethodParameter>();
                    for (int i = 0; i < _invocation.Arguments.Length; i++)
                    {
                        parameters.Add(new InterceptMethodParameter(_invocation, i));
                    }
                    _arguments = new ParameterCollection(parameters);
                }
                return _arguments;
            }
        }

        public IMethodReturn CreateMethodReturn(object returnValue)
        {
            _invocation.ReturnValue = returnValue;
            return new InterceptMethodReturn(_invocation);
        }

        public IMethodReturn CreateExceptionReturn(Exception exception)
        {
            return new InterceptMethodReturn(_invocation, exception);
        }
    }
}
