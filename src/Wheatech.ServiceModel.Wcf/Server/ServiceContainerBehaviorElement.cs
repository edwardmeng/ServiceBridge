using System;
using System.ServiceModel.Configuration;

namespace Wheatech.ServiceModel.Wcf
{
    /// <summary>
    /// Represents a configuration element that is used to enable dependency injection for the WCF services. 
    /// This class cannot be inherited.
    /// </summary>
    public sealed class ServiceContainerBehaviorElement : BehaviorExtensionElement
    {
        /// <summary>
        /// Creates a behavior extension based on the current configuration settings.
        /// </summary>
        /// <returns>
        /// The behavior extension.
        /// </returns>
        protected override object CreateBehavior() => new ContainerServiceBehavior();

        /// <summary>
        /// Gets the type of behavior.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Type"/>.
        /// </returns>
        public override Type BehaviorType => typeof(ContainerServiceBehavior);
    }
}
