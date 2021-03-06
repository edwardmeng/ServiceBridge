﻿using System;
using Ninject.Syntax;

namespace ServiceBridge.Ninject
{
    /// <summary>
    /// Event argument for the <see cref="IServiceContainer.Registering"/> event raised in <see cref="NinjectServiceContainer"/>.
    /// </summary>
    public class NinjectServiceRegisterEventArgs : ServiceRegisterEventArgs
    {
        internal NinjectServiceRegisterEventArgs(Type serviceType, Type implementType, string serviceName, ServiceLifetime lifetime, IBindingSyntax binding) : 
            base(serviceType, implementType, serviceName, lifetime)
        {
            Binding = binding;
        }

        /// <summary>
        /// Gets the registration instance for the Autofac service mapping.
        /// </summary>
        public IBindingSyntax Binding { get; }
    }
}
