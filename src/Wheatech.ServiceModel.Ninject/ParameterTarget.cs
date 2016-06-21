using System;
using System.Reflection;
using Ninject.Planning.Bindings;
using NinjectParameterTarget = Ninject.Planning.Targets.ParameterTarget;

namespace Wheatech.ServiceModel.Ninject
{
    /// <summary>
    /// Represents an injection target for a <see cref="ParameterInfo"/>.
    /// </summary>
    internal class ParameterTarget : NinjectParameterTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterTarget"/> class.
        /// </summary>
        /// <param name="method">The method that defines the parameter.</param>
        /// <param name="site">The parameter that this target represents.</param>
        public ParameterTarget(MethodBase method, ParameterInfo site) : base(method, site)
        {
        }

        /// <summary>
        /// Reads the resolution constraint from target.
        /// </summary>
        /// <returns>The resolution constraint.</returns>
        protected override Func<IBindingMetadata, bool> ReadConstraintFromTarget()
        {
            return base.ReadConstraintFromTarget() ?? (metadata => metadata.Name == null);
        }
    }
}
