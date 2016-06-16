using System;
using System.Reflection;

namespace Wheatech.ServiceModel.Interception
{
    public interface IMethodInvocation
    {
        MethodBase MethodBase { get; }

        object Target { get; }

        ParameterCollection Arguments { get; }

        IMethodReturn CreateMethodReturn(object returnValue);

        IMethodReturn CreateExceptionReturn(Exception exception);
    }
}