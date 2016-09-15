using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceBridge.Interception;
using IMethodInvocation = ServiceBridge.Interception.IMethodInvocation;
using IMethodReturn = ServiceBridge.Interception.IMethodReturn;
using ParameterCollection = ServiceBridge.Interception.ParameterCollection;

namespace ServiceBridge.Unity.Interception
{
    internal class UnityMethodInvocation : IMethodInvocation
    {
        private readonly Microsoft.Practices.Unity.InterceptionExtension.IMethodInvocation _invocation;
        private ParameterCollection _arguments;

        public UnityMethodInvocation(Microsoft.Practices.Unity.InterceptionExtension.IMethodInvocation invocation)
        {
            _invocation = invocation;
        }

        public MethodBase Method => _invocation.MethodBase;

        public object Target => _invocation.Target;

        public ParameterCollection Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    var parameters = new List<IMethodParameter>();
                    for (int i = 0; i < _invocation.Arguments.Count; i++)
                    {
                        parameters.Add(new UnityMethodInvocationParameter(_invocation,i));
                    }
                    _arguments = new ParameterCollection(parameters);
                }
                return _arguments;
            }
        }

        public IMethodReturn CreateMethodReturn(object returnValue)
        {
            return new UnityMethodReturn(_invocation.CreateMethodReturn(returnValue,
                (from argument in Arguments
                    where argument.ParameterInfo.IsOut
                    select argument.Value).ToArray()));
        }

        public IMethodReturn CreateExceptionReturn(Exception exception)
        {
            return new UnityMethodReturn(_invocation.CreateExceptionMethodReturn(exception));
        }

        public Microsoft.Practices.Unity.InterceptionExtension.IMethodInvocation Unwrap()
        {
            return _invocation;
        }
    }
}
