using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.Unity.Interception
{
    public class UnityMethodInvocation : IMethodInvocation
    {
        private readonly Microsoft.Practices.Unity.InterceptionExtension.IMethodInvocation _invocation;
        private ParameterCollection _arguments;

        public UnityMethodInvocation(Microsoft.Practices.Unity.InterceptionExtension.IMethodInvocation invocation)
        {
            _invocation = invocation;
        }

        public MethodBase MethodBase => _invocation.MethodBase;

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
    }
}
