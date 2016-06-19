using System.Linq;
using System.Reflection;
using Ninject.Injection;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;

namespace Wheatech.ServiceModel.Ninject
{
    /// <summary>
    /// Describes the injection of a method.
    /// </summary>
    internal class MethodDirective: MethodInjectionDirective
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodDirective"/> class.
        /// </summary>
        /// <param name="method">The method described by the directive.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        public MethodDirective(MethodInfo method, MethodInjector injector) : base(method, injector)
        {
        }

        /// <summary>
        /// Creates targets for the parameters of the method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The targets for the method's parameters.</returns>
        protected override ITarget[] CreateTargetsFromParameters(MethodInfo method)
        {
            return method.GetParameters().Select(parameter => new ParameterTarget(method, parameter)).OfType<ITarget>().ToArray();
        }
    }
}
