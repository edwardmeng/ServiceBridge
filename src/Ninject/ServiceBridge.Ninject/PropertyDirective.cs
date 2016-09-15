using System.Reflection;
using Ninject.Injection;
using Ninject.Planning.Directives;
using Ninject.Planning.Targets;

namespace ServiceBridge.Ninject
{
    /// <summary>
    /// Describes the injection of a property.
    /// </summary>
    internal class PropertyDirective : PropertyInjectionDirective
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDirective"/> class.
        /// </summary>
        /// <param name="member">The member the directive describes.</param>
        /// <param name="injector">The injector that will be triggered.</param>
        public PropertyDirective(PropertyInfo member, PropertyInjector injector) : base(member, injector)
        {
        }

        /// <summary>
        /// Creates a target for the property.
        /// </summary>
        /// <param name="propertyInfo">The property.</param>
        /// <returns>The target for the property.</returns>
        protected override ITarget CreateTarget(PropertyInfo propertyInfo)
        {
            return new PropertyTarget(propertyInfo);
        }
    }
}
