using System.Linq;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Wheatech.ServiceModel.Unity.Interception
{
    public class UnityServiceContainerExtension : IServiceContainerExtension
    {
        public void Initialize(IServiceContainer container)
        {
            ((UnityServiceContainer)container).AddUnityExtension(new Microsoft.Practices.Unity.InterceptionExtension.Interception());
            container.Registering += OnRegistering;
        }

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
