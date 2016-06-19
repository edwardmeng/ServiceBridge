using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.Utility;

namespace Wheatech.ServiceModel.Unity
{
    /// <summary>
    /// An implementation of <see cref="IMethodSelectorPolicy"/> that is aware
    /// of the build keys used by the Unity container.
    /// </summary>
    internal class UnityMethodSelectorPolicy : IMethodSelectorPolicy
    {
        /// <summary>
        /// Return the sequence of methods to call while building the target object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>Sequence of methods to call.</returns>
        public virtual IEnumerable<SelectedMethod> SelectMethods(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            var candidateMethods = context.BuildKey.Type.GetMethodsHierarchical()
                                    .Where(m => m.IsStatic == false && m.IsPublic);

            foreach (var method in candidateMethods)
            {
                if (method.IsDefined(typeof(InjectionMethodAttribute), false) || method.IsDefined(typeof(InjectionAttribute), false))
                {
                    yield return CreateSelectedMethod(method);
                }
            }
        }

        private SelectedMethod CreateSelectedMethod(MethodInfo method)
        {
            var result = new SelectedMethod(method);
            foreach (var parameter in method.GetParameters())
            {
                result.AddParameterResolver(CreateResolver(parameter));
            }

            return result;
        }

        private IDependencyResolverPolicy CreateResolver(ParameterInfo parameter)
        {
            var attributes = parameter.GetCustomAttributes(false)
                .OfType<DependencyResolutionAttribute>()
                .ToList();

            if (attributes.Count > 0)
            {
                return attributes[0].CreateResolver(parameter.ParameterType);
            }

            return new NamedTypeDependencyResolverPolicy(parameter.ParameterType, null);
        }
    }
}
