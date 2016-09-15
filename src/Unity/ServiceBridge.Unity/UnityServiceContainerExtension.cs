using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace ServiceBridge.Unity
{
    internal class UnityServiceContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.Policies.SetDefault<IConstructorSelectorPolicy>(new UnityConstructorSelectorPolicy());
            Context.Policies.SetDefault<IPropertySelectorPolicy>(new UnityPropertySelectorPolicy());
            Context.Policies.SetDefault<IMethodSelectorPolicy>(new UnityMethodSelectorPolicy());
        }
    }
}
