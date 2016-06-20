using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Wheatech.ServiceModel.Wcf
{
    /// <summary>
    /// The <see cref="ContainerServiceBehavior"/>
    /// class is used to provide a service behavior for configuring unity injection in WCF.
    /// </summary>
    internal class ContainerServiceBehavior : IServiceBehavior
    {
        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description.
        /// </param>
        /// <param name="serviceHostBase">
        /// The service host that is currently being constructed.
        /// </param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        /// <summary>
        /// Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description of the service.
        /// </param>
        /// <param name="serviceHostBase">
        /// The host of the service.
        /// </param>
        /// <param name="endpoints">
        /// The service endpoints.
        /// </param>
        /// <param name="bindingParameters">
        /// Custom objects to which binding elements have access.
        /// </param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors,
        /// security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description.
        /// </param>
        /// <param name="serviceHostBase">
        /// The host that is currently being built.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="serviceDescription"/> value is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="serviceHostBase"/> value is <c>null</c>.
        /// </exception>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            if (serviceDescription == null)
            {
                throw new ArgumentNullException(nameof(serviceDescription));
            }

            if (serviceHostBase == null)
            {
                throw new ArgumentNullException(nameof(serviceHostBase));
            }
            var contractTypeMappings = new Dictionary<string, Type>();
            foreach (var endpoint in serviceDescription.Endpoints)
            {
                contractTypeMappings.Add(endpoint.Address.Uri.ToString(), endpoint.Contract.ContractType);
            }
            foreach (var channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var dispatcher = (ChannelDispatcher) channelDispatcherBase;
                foreach (var endpointDispatcher in dispatcher.Endpoints)
                {
                    if (!endpointDispatcher.IsSystemEndpoint)
                    {
                        var address = endpointDispatcher.EndpointAddress.Uri.ToString();
                        var contractType = contractTypeMappings[endpointDispatcher.EndpointAddress.Uri.ToString()];
                        endpointDispatcher.DispatchRuntime.InstanceProvider = new ContainerInstanceProvider(contractType, serviceDescription.ServiceType, address);
                    }
                }
            }
        }
    }
}
