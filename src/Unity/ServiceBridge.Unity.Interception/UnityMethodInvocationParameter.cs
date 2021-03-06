﻿using System.Reflection;
using ServiceBridge.Interception;
using IMethodInvocation = Microsoft.Practices.Unity.InterceptionExtension.IMethodInvocation;

namespace ServiceBridge.Unity.Interception
{
    internal class UnityMethodInvocationParameter : IMethodParameter
    {
        private readonly IMethodInvocation _invocation;
        private readonly int _parameterIndex;

        public UnityMethodInvocationParameter(IMethodInvocation invocation, int parameterIndex)
        {
            _invocation = invocation;
            _parameterIndex = parameterIndex;
        }

        public ParameterInfo ParameterInfo => _invocation.Arguments.GetParameterInfo(_parameterIndex);

        public string Name => ParameterInfo.Name;

        public object Value
        {
            get { return _invocation.Arguments[_parameterIndex]; }
            set { _invocation.Arguments[_parameterIndex] = value; }
        }
    }
}
