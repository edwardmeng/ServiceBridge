using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace Wheatech.ServiceModel.Unity
{
    public class UnityServiceRegisterEventArgs : ServiceRegisterEventArgs
    {
        public UnityServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName) : base(serviceType, implementType, serviceName)
        {
        }

        public LifetimeManager Lifetime { get; set; }

        public List<InjectionMember> InjectionMembers { get; } = new List<InjectionMember>();
    }
}
