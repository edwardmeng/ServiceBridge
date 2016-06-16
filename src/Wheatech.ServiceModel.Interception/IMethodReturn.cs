using System;

namespace Wheatech.ServiceModel.Interception
{
    public interface IMethodReturn
    {
        Exception Exception { get; set; }

        ParameterCollection Outputs { get; }

        object ReturnValue { get; set; }
    }
}
