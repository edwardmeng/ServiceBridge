using System;
using System.Reflection;
using System.ServiceModel;

namespace Wheatech.ServiceModel.Wcf
{
    public class WcfServiceContainerExtension : IServiceContainerExtension
    {
        public void Initialize(IServiceContainer container)
        {
            container.Registering += OnRegistering;
        }

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
