using System.Reflection;
using ServiceBridge.Interception;
using IMethodReturn = Microsoft.Practices.Unity.InterceptionExtension.IMethodReturn;

namespace ServiceBridge.Unity.Interception
{
    internal class UnitMethodReturnParameter : IMethodParameter
    {
        private readonly IMethodReturn _return;
        private readonly int _parameterIndex;

        public UnitMethodReturnParameter(IMethodReturn @return, int parameterIndex)
        {
            _return = @return;
            _parameterIndex = parameterIndex;
        }

        public ParameterInfo ParameterInfo => _return.Outputs.GetParameterInfo(_parameterIndex);

        public string Name => ParameterInfo.Name;

        public object Value
        {
            get { return _return.Outputs[_parameterIndex]; }
            set { _return.Outputs[_parameterIndex] = value; }
        }
    }
}
