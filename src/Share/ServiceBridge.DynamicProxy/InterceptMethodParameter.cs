using System.Reflection;
using Castle.DynamicProxy;
using ServiceBridge.Interception;

namespace ServiceBridge.DynamicProxy
{
    internal class InterceptMethodParameter : IMethodParameter
    {
        private readonly IInvocation _invocation;
        private readonly int _parameterIndex;
        private ParameterInfo _parameter;

        public InterceptMethodParameter(IInvocation invocation, int parameterIndex)
        {
            _invocation = invocation;
            _parameterIndex = parameterIndex;
        }

        public ParameterInfo ParameterInfo => _parameter ?? (_parameter = _invocation.GetConcreteMethod().GetParameters()[_parameterIndex]);

        public string Name => ParameterInfo.Name;

        public object Value
        {
            get { return _invocation.Arguments[_parameterIndex]; }
            set { _invocation.SetArgumentValue(_parameterIndex, value); }
        }
    }
}
