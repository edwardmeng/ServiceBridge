using System;

namespace Wheatech.ServiceModel
{
    /// <summary>
    /// This attribute is used to mark properties or constructor for injection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For properties, this attribute is necessary for injection to happen.
    /// </para>
    /// <para>
    /// For constructors, this attribute is used to indicate which constructor to choose when
    /// the container attempts to build a type.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property)]
    public class InjectionAttribute : Attribute
    {
    }
}
