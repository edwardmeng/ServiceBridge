using System;
using System.Reflection;
using Ninject.Planning.Bindings;
using NinjectPropertyTarget = Ninject.Planning.Targets.PropertyTarget;

namespace Wheatech.ServiceModel.Ninject
{
    /// <summary>
    /// Represents an injection target for a <see cref="PropertyInfo"/>.
    /// </summary>
    internal class PropertyTarget : NinjectPropertyTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTarget"/> class.
        /// </summary>
        /// <param name="site">The property that this target represents.</param>
        public PropertyTarget(PropertyInfo site) : base(site)
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
