using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.Utility;

namespace ServiceBridge.Unity
{
    /// <summary>
    /// An implementation of <see cref="IConstructorSelectorPolicy"/> that is
    /// aware of the build keys used by the Unity container.
    /// </summary>
    internal class UnityConstructorSelectorPolicy : IConstructorSelectorPolicy
    {
        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>The chosen constructor.</returns>
        public SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            if(context == null)throw new ArgumentNullException(nameof(context));
            var typeToConstruct = context.BuildKey.Type;
            var constructors = typeToConstruct.GetConstructors();
            var selectedConstructor =
                SelectConstructor(constructors.Where(ctor => ctor.IsDefined(typeof(InjectionAttribute), true)).ToArray()) ??
                SelectConstructor(constructors.Where(ctor => ctor.IsDefined(typeof(InjectionConstructorAttribute), true)).ToArray()) ??
                SelectConstructor(constructors);
            if (selectedConstructor != null)
            {
                var result = new SelectedConstructor(selectedConstructor);
                foreach (var parameter in selectedConstructor.GetParameters())
                {
                    var attrs = parameter.GetCustomAttributes<DependencyResolutionAttribute>(false).ToArray();
                    if (attrs.Length > 0)
                    {
                        // Since this attribute is defined with MultipleUse = false, the compiler will
                        // enforce at most one. So we don't need to check for more.
                        result.AddParameterResolver(attrs[0].CreateResolver(parameter.ParameterType));
                    }
                    else
                    {
                        // No attribute, just go back to the container for the default for that type.
                        result.AddParameterResolver(new NamedTypeDependencyResolverPolicy(parameter.ParameterType, null));
                    }
                }
                return result;
            }
            return null;
        }

        private static ConstructorInfo SelectConstructor(ConstructorInfo[] constructors)
        {
            switch (constructors.Length)
            {
                case 0:
                    return null;

                case 1:
                    return constructors[0];

                default:
                    Array.Sort(constructors, new ConstructorLengthComparer());
                    return constructors[0];
            }
        }

        private class ConstructorLengthComparer : IComparer<ConstructorInfo>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="y">The second object to compare.</param>
            /// <param name="x">The first object to compare.</param>
            /// <returns>
            /// Value Condition Less than zero is less than y. Zero equals y. Greater than zero is greater than y.
            /// </returns>
            public int Compare(ConstructorInfo x, ConstructorInfo y)
            {
                Guard.ArgumentNotNull(x, "x");
                Guard.ArgumentNotNull(y, "y");

                return y.GetParameters().Length - x.GetParameters().Length;
            }
        }
    }
}
