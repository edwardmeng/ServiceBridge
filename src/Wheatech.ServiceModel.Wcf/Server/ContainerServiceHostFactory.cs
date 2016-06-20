﻿using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Wheatech.ServiceModel.Wcf
{
    /// <summary>
    /// The <see cref="ContainerServiceHostFactory"/>
    /// class is used to create a <see cref="ServiceHost"/> instance that supports creating service instances with Unity.
    /// </summary>
    public class ContainerServiceHostFactory : ServiceHostFactory
    {
        /// <summary>
        /// Creates a <see cref="T:System.ServiceModel.ServiceHost"/> for a specified type of service with a specific base address.
        /// </summary>
        /// <param name="serviceType">
        /// Specifies the type of service to host.
        /// </param>
        /// <param name="baseAddresses">
        /// The <see cref="T:System.Array"/> of type <see cref="T:System.Uri"/> that contains the base addresses for the service hosted.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.ServiceModel.ServiceHost"/> for the type of service specified with a specific base address.
        /// </returns>
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new ContainerServiceHost(serviceType, baseAddresses);
        }
    }
}
