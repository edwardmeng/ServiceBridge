using System.Linq;
using System.Reflection;
using Ninject.Planning.Directives;
using Ninject.Injection;
using Ninject.Planning.Targets;

namespace Wheatech.ServiceModel.Ninject
{
    /// <summary>
    /// Describes the injection of a constructor.
    /// </summary>
    internal class ConstructorDirective : ConstructorInjectionDirective
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorDirective"/> class.
        /// </summary>
        /// <param name="constructor">The constructor described by the directive.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        public ConstructorDirective(ConstructorInfo constructor, ConstructorInjector injector)
            : base(constructor, injector)
        {
        }

        /// <summary>
        /// Creates targets for the parameters of the method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The targets for the method's parameters.</returns>
        protected override ITarget[] CreateTargetsFromParameters(ConstructorInfo method)
        {
            return method.GetParameters().Select(parameter => new ParameterTarget(method, parameter)).OfType<ITarget>().ToArray();
        }
    }
}
