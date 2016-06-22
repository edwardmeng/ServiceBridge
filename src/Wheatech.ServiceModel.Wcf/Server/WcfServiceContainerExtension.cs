using System;
using System.Reflection;
using System.ServiceModel;

namespace Wheatech.ServiceModel.Wcf
{
    /// <summary>
    /// The service container extension for controlling the lifetime of the registing services.
    /// </summary>
    /// <remarks>
    /// This extension should be used in the WCF host server side.
    /// </remarks>
    public class WcfServiceContainerExtension : IServiceContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        public void Initialize(IServiceContainer container)
        {
            container.Registering += OnRegistering;
        }

        /// <summary>
        /// Removes the extension's functions from the container. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        public void Remove(IServiceContainer container)
        {
            container.Registering -= OnRegistering;
        }

        private void OnRegistering(object sender, ServiceRegisterEventArgs e)
        {
            if (e.ServiceType.IsDefined(typeof(ServiceContractAttribute)))
            {
                var attribute = (ServiceBehaviorAttribute)Attribute.GetCustomAttribute(e.ImplementType, typeof(ServiceBehaviorAttribute));
                if (attribute != null)
                {
                    switch (attribute.InstanceContextMode)
                    {
                        case InstanceContextMode.PerCall:
                            e.Lifetime = ServiceLifetime.Transient;
                            break;
                        case InstanceContextMode.Single:
                            e.Lifetime = ServiceLifetime.Singleton;
                            break;
                        case InstanceContextMode.PerSession:
                            e.Lifetime = ServiceLifetime.PerThread;
                            break;
                    }
                }
            }
        }
    }
}
