using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ServiceBridge
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
    /// <para>
    /// For methods, this attribute is used to indicate the methods should be called when
    /// the container is building an object.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Method)]
    public class InjectionAttribute : Attribute
    {
        /// <summary>
        /// Returns the declared properties of a type or its base types marked with <see cref="InjectionAttribute"/>.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>An enumerable of the <see cref="T:System.Reflection.PropertyInfo" /> objects.</returns>
        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return GetPropertiesHierarchy(type).Where(property => property.IsDefined(typeof(InjectionAttribute), false));
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
#if NetCore
            return type.GetTypeInfo().DeclaredConstructors
                .Where(c => !c.IsStatic && c.IsDefined(typeof(InjectionAttribute), false))
                .OrderBy(constructor => constructor.GetParameters().Length);
#else
            return type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(c => c.IsDefined(typeof(InjectionAttribute), false))
                .OrderBy(constructor => constructor.GetParameters().Length);
#endif
        }

        /// <summary>
        /// Returns the declared methods of a type or its base types marked with <see cref="InjectionAttribute"/>.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>An enumerable of the <see cref="MethodInfo"/> objects.</returns>
        public static IEnumerable<MethodInfo> GetMethods(Type type)
        {
            return GetMethodsHierarchy(type).Where(method => method.IsDefined(typeof(InjectionAttribute), false));
        }

        /// <summary>
        /// Returns a value indicating the specified property can be injected.
        /// </summary>
        /// <param name="property">The property to be checked.</param>
        /// <returns><c>true</c> if the specified property can be injected; otherwise, <c>false</c>.</returns>
        public static bool Matches(PropertyInfo property)
        {
#if NetCore
            var method = property.SetMethod ?? property.GetMethod;
#else
            var method = property.GetSetMethod(true) ?? property.GetGetMethod(true);
#endif
            return property.CanWrite && !method.IsStatic && property.GetIndexParameters().Length == 0 && property.IsDefined(typeof(InjectionAttribute), false);
        }

        /// <summary>
        /// Returns a value indicating the specified method or constructor can be injected.
        /// </summary>
        /// <param name="method">The method or constructor to be checked.</param>
        /// <returns><c>true</c> if the specified method or constructor can be injected; otherwise, <c>false</c>.</returns>
        public static bool Matches(MethodBase method)
        {
            return !method.IsStatic && method.IsPublic && method.IsDefined(typeof(InjectionAttribute), false) && (method.IsConstructor || !method.IsSpecialName);
        }

        private static IEnumerable<PropertyInfo> GetPropertiesHierarchy(Type type)
        {
            if (type == null || type == typeof(object))
                return Enumerable.Empty<PropertyInfo>();
            var properties = new List<Tuple<PropertyInfo, PropertyInfo>>();
#if NetCore
            properties.AddRange(from property in type.GetTypeInfo().DeclaredProperties
                                let method = property.SetMethod ?? property.GetMethod
                                where !method.IsStatic && property.CanWrite && property.GetIndexParameters().Length == 0
                                select Tuple.Create(property, ReflectionHelper.GetPropertyFromMethod(method.GetRuntimeBaseDefinition())));
            foreach (var property in GetPropertiesHierarchy(type.GetTypeInfo().BaseType))
            {
                if (properties.All(p => !Equals(p.Item2, property)))
                {
                    properties.Add(Tuple.Create(property, ReflectionHelper.GetPropertyFromMethod((property.SetMethod ?? property.GetMethod).GetRuntimeBaseDefinition())));
                }
            }
#else
            properties.AddRange(from property in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                let method = property.GetSetMethod(true) ?? property.GetGetMethod(true)
                                where property.CanWrite && property.GetIndexParameters().Length == 0
                                select Tuple.Create(property, ReflectionHelper.GetPropertyFromMethod(method.GetBaseDefinition())));
            foreach (var property in GetPropertiesHierarchy(type.BaseType))
            {
                if (properties.All(p => p.Item2 != property))
                {
                    properties.Add(Tuple.Create(property, ReflectionHelper.GetPropertyFromMethod((property.GetSetMethod(true) ?? property.GetGetMethod(true)).GetBaseDefinition())));
                }
            }
#endif
            return properties.Select(p => p.Item1);
        }

        private static IEnumerable<MethodInfo> GetMethodsHierarchy(Type type)
        {
            if (type == null || type == typeof(object))
                return Enumerable.Empty<MethodInfo>();
            var methods = new List<Tuple<MethodInfo, MethodInfo>>();
#if NetCore
            methods.AddRange(from method in type.GetTypeInfo().DeclaredMethods
                             where !method.IsSpecialName && !method.IsStatic
                             select Tuple.Create(method, method.GetRuntimeBaseDefinition()));
            foreach (var method in GetMethodsHierarchy(type.GetTypeInfo().BaseType))
            {
                if (methods.All(x => !Equals(x.Item2, method)))
                {
                    methods.Add(Tuple.Create(method, method.GetRuntimeBaseDefinition()));
                }
            }
#else
            methods.AddRange(from method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                where !method.IsSpecialName
                select Tuple.Create(method, method.GetBaseDefinition()));
            foreach (var method in GetMethodsHierarchy(type.BaseType))
            {
                if (methods.All(x => x.Item2 != method))
                {
                    methods.Add(Tuple.Create(method, method.GetBaseDefinition()));
                }
            }
#endif
            return methods.Select(x => x.Item1);
        }
    }
}
