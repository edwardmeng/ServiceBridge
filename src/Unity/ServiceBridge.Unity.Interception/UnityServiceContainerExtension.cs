using System.Linq;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace ServiceBridge.Unity.Interception
{
    /// <summary>
    /// The service container extension to enable interception mechanism for the <see cref="UnityServiceContainer"/>.
    /// </summary>
    public class UnityServiceContainerExtension : IServiceContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        public void Initialize(IServiceContainer container)
        {
            ((UnityServiceContainer)container).AddUnityExtension(new Microsoft.Practices.Unity.InterceptionExtension.Interception());
            container.Registering += OnRegistering;
        }

        /// <summary>
        /// Removes the extension's functions from the container. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        public void Remove(IServiceContainer container)
        {
            ((Microsoft.Practices.Unity.InterceptionExtension.Interception)((UnityServiceContainer)container).GetUnityExtension(typeof(Microsoft.Practices.Unity.InterceptionExtension.Interception))).Remove();
            container.Registering -= OnRegistering;
        }

        private void OnRegistering(object sender, ServiceRegisterEventArgs e)
        {
            var injectionMembers = ((UnityServiceRegisterEventArgs)e).InjectionMembers;
            if (!injectionMembers.Any(member => member is DefaultInterceptor || member is Interceptor))
            {
                injectionMembers.Add(new Interceptor<VirtualMethodInterceptor>());
            }
            injectionMembers.Add(new InterceptionBehavior<UnityInjectionBehavior>());
        }
    }
}
