using System.Reflection;
using Wheatech.ServiceModel.Interception;
using IMethodReturn = Microsoft.Practices.Unity.InterceptionExtension.IMethodReturn;

namespace Wheatech.ServiceModel.Unity.Interception
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
