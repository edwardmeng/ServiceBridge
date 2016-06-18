using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        /// <summary>
        /// Returns the declared properties of a type or its base types.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>An enumerable of the <see cref="T:System.Reflection.PropertyInfo" /> objects.</returns>
        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return GetPropertiesHierarchy(type)
                .Where(property => property.CanWrite && !(property.SetMethod ?? property.GetMethod).IsStatic && property.IsDefined(typeof(InjectionAttribute)));
        }

        /// <summary>
        /// Returns all the public constructors defined for the current reflected <see cref="Type"/>.
        /// </summary>
        /// <value>
        /// An enumeration of <see cref="ConstructorInfo"/> ConstructorInfo objects representing all the public instance constructors defined for the 
        /// current reflected <see cref="Type"/>, but not including the type initializer (static constructor).
        /// </value>
        public static IEnumerable<ConstructorInfo> GetConstructors(Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors
                .Where(c => !c.IsStatic && c.IsPublic && c.IsDefined(typeof(InjectionAttribute)))
                .OrderBy(constructor => constructor.GetParameters().Length);
        }

        private static IEnumerable<PropertyInfo> GetPropertiesHierarchy(Type type)
        {
            if (type == null)
                return Enumerable.Empty<PropertyInfo>();
            if (type == typeof(object))
                return type.GetTypeInfo().DeclaredProperties;
            return type.GetTypeInfo().DeclaredProperties.Concat(GetPropertiesHierarchy(type.GetTypeInfo().BaseType));
        }
    }
}
