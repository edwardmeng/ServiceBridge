using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace ServiceBridge.Unity
{
    /// <summary>
    /// Event argument for the <see cref="IServiceContainer.Registering"/> event raised in <see cref="UnityServiceContainer"/>.
    /// </summary>
    public class UnityServiceRegisterEventArgs : ServiceRegisterEventArgs
    {
        internal UnityServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName, ServiceLifetime lifetime) 
            : base(serviceType, implementType, serviceName, lifetime)
        {
        }

        /// <summary>
        /// Gets the injection members for the Unity service mapping.
        /// </summary>
        public List<InjectionMember> InjectionMembers { get; } = new List<InjectionMember>();
    }
}
