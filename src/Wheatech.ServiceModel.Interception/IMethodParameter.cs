using System.Reflection;

namespace Wheatech.ServiceModel.Interception
{
    public interface IMethodParameter
    {
        ParameterInfo ParameterInfo { get; }

        string Name { get; }

        object Value { get; set; }
    }
}
