using System;
using System.ServiceModel;

namespace ServiceBridge.ServiceModel
{
    /// <summary>
    /// The <see cref="ContainerServiceHost"/>
    /// class provides a service host that creates WCF service instances using Unity.
    /// </summary>
    internal class ContainerServiceHost : ServiceHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerServiceHost"/> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="baseAddresses">The base addresses.</param>
        public ContainerServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }

        /// <summary>
        /// Invoked during the transition of a communication object into the opening state.
        /// </summary>
        protected override void OnOpening()
        {
            if (Description.Behaviors.Find<ContainerServiceAttribute>() == null)
            {
                Description.Behaviors.Add(new ContainerServiceAttribute());
            }

            base.OnOpening();
        }
    }
}
