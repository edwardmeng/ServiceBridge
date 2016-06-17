using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wheatech.ServiceModel.Interception
{
    /// <summary>
    /// Base class for interceptor attributes used in the interception mechanism.
    /// </summary>
    public abstract class InterceptorAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the order in which the interceptor will be executed. 
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Derived classes implement this method. When called, it creates a new interceptor as specified in the attribute configuration. 
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> to use when creating interceptors, if necessary.</param>
        /// <returns>A new interceptor object.</returns>
        public abstract IInterceptor CreateInterceptor(IServiceContainer container);

        /// <summary>
        /// Given a particular MemberInfo, find all the attributes that apply to this
        /// member. Specifically, it returns the attributes on the type, then (if it's a
        /// property accessor) on the property, then on the member itself.
        /// </summary>
        /// <param name="member">The member to look at.</param>
        /// <param name="inherits">true to include attributes inherited from base classes.</param>
        /// <returns>Array of found attributes.</returns>
        public static InterceptorAttribute[] GetAttributes(MemberInfo member, bool inherits)
        {
            if(member == null)throw new ArgumentNullException(nameof(member));
            var attributes = new List<InterceptorAttribute>();

            if (member.DeclaringType != null)
            {
                attributes.AddRange(member.DeclaringType.GetCustomAttributes<InterceptorAttribute>(inherits));

                var methodInfo = member as MethodInfo;
                if (methodInfo != null)
                {
                    PropertyInfo prop = GetPropertyFromMethod(methodInfo);
                    if (prop != null)
                    {
                        attributes.AddRange(prop.GetCustomAttributes<InterceptorAttribute>(inherits));
                    }
                }
            }
            attributes.AddRange(member.GetCustomAttributes<InterceptorAttribute>(inherits));
            return attributes.ToArray();
        }

        /// <summary>
        /// Given a MethodInfo for a property's get or set method,
        /// return the corresponding property info.
        /// </summary>
        /// <param name="method">MethodBase for the property's get or set method.</param>
        /// <returns>PropertyInfo for the property, or null if method is not part of a property.</returns>
        private static PropertyInfo GetPropertyFromMethod(MethodInfo method)
        {
            if (!method.IsSpecialName) return null;
            var containingType = method.DeclaringType;
            if (containingType == null) return null;
            var isGetter = method.Name.StartsWith("get_", StringComparison.Ordinal);
            var isSetter = method.Name.StartsWith("set_", StringComparison.Ordinal);
            if (!isSetter && !isGetter) return null;
            var propertyName = method.Name.Substring(4);
            Type propertyType;
            Type[] indexerTypes;

            GetPropertyTypes(method, isGetter, out propertyType, out indexerTypes);

            return containingType.GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                null,
                propertyType,
                indexerTypes,
                null);
        }

        private static void GetPropertyTypes(MethodInfo method, bool isGetter, out Type propertyType, out Type[] indexerTypes)
        {
            var parameters = method.GetParameters();
            if (isGetter)
            {
                propertyType = method.ReturnType;
                indexerTypes =
                    parameters.Length == 0
                        ? Type.EmptyTypes
                        : parameters.Select(pi => pi.ParameterType).ToArray();
            }
            else
            {
                propertyType = parameters[parameters.Length - 1].ParameterType;
                indexerTypes =
                    parameters.Length == 1
                        ? Type.EmptyTypes
                        : parameters.Take(parameters.Length - 1).Select(pi => pi.ParameterType).ToArray();
            }
        }
    }
}
