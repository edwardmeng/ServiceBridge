using System;

namespace Wheatech.ServiceModel
{
    /// <summary>
    /// The attribute for service methods that wish to be started by the container automatically .
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class StartupAttribute : Attribute
    {
    }
}
