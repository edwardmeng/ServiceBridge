using System.Reflection;
using Ninject.Extensions.Interception;
using ServiceBridge.Interception;

namespace ServiceBridge.Ninject.Interception
{
    internal class NinjectMethodParameter : IMethodParameter
    {
        private readonly IInvocation _invocation;
        private readonly int _parameterIndex;
        private ParameterInfo _parameter;

        public NinjectMethodParameter(IInvocation invocation, int parameterIndex)
        {
            _invocation = invocation;
            _parameterIndex = parameterIndex;
        }

        public ParameterInfo ParameterInfo => _parameter ?? (_parameter = _invocation.Request.Method.GetParameters()[_parameterIndex]);

        public string Name => ParameterInfo.Name;

        public object Value
        {
            get { return _invocation.Request.Arguments[_parameterIndex]; }
            set { _invocation.Request.Arguments[_parameterIndex] = value; }
        }
    }
}
