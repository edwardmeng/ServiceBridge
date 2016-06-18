using System;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace Wheatech.ServiceModel.Unity
{
    internal class UnityServiceContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.Policies.SetDefault<IConstructorSelectorPolicy>(new UnityConstructorSelectorPolicy());
            Context.Policies.SetDefault<IPropertySelectorPolicy>(new UnityPropertySelectorPolicy());
        }
    }
}
