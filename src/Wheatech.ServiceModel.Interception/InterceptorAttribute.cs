using System;
using System.Collections.Generic;
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
                    var prop = ReflectionHelper.GetPropertyFromMethod(methodInfo);
                    if (prop != null)
                    {
                        attributes.AddRange(prop.GetCustomAttributes<InterceptorAttribute>(inherits));
                    }
                }
            }
            attributes.AddRange(member.GetCustomAttributes<InterceptorAttribute>(inherits));
            return attributes.ToArray();
        }
    }
}
