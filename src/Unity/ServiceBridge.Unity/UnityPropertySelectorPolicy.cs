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
    /// An implementation of <see cref="IPropertySelectorPolicy"/> that is aware of
    /// the build keys used by the unity container.
    /// </summary>
    internal class UnityPropertySelectorPolicy : IPropertySelectorPolicy
    {

        /// <summary>
        /// Returns sequence of properties on the given type that
        /// should be set as part of building that object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>Sequence of <see cref="PropertyInfo"/> objects
        /// that contain the properties to set.</returns>
        public IEnumerable<SelectedProperty> SelectProperties(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            Type typeToInjection = context.BuildKey.Type;

            foreach (var property in typeToInjection.GetPropertiesHierarchical().Where(p => p.CanWrite))
            {
                var propertyMethod = property.SetMethod ?? property.GetMethod;
                if (propertyMethod.IsStatic)
                {
                    // Skip static properties. In the previous implementation the reflection query took care of this.
                    continue;
                }

                // Ignore indexers and return properties marked with the attribute
                if (property.GetIndexParameters().Length == 0 &&
                   (property.IsDefined(typeof(InjectionAttribute), false) || property.IsDefined(typeof(DependencyResolutionAttribute), false)))
                {
                    IDependencyResolverPolicy resolver;
                    var dependencyAttribute = property.GetCustomAttribute<DependencyResolutionAttribute>();
                    if (dependencyAttribute != null)
                    {
                        resolver = dependencyAttribute.CreateResolver(property.PropertyType);
                    }
                    else
                    {
                        resolver = new NamedTypeDependencyResolverPolicy(property.PropertyType, null);
                    }
                    yield return new SelectedProperty(property, resolver);
                }
            }
        }
    }
}
