using System;
using System.Configuration;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Wheatech.ServiceModel.Wcf
{
    /// <summary>
    /// The <see cref="ContainerInstanceProvider"/>
    /// class is used by WCF to create a new instance of a service contract.
    /// </summary>
    internal class ContainerInstanceProvider : IInstanceProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerInstanceProvider"/> class.
        /// </summary>
        /// <param name="contractType">The type of the service contract.</param>
        /// <param name="serviceType">The type of the service implementation.</param>
        /// <param name="address">The endpoint address of the service.</param>
        /// <exception cref="System.ArgumentNullException">serviceType</exception>
        public ContainerInstanceProvider(Type contractType, Type serviceType, string address)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            ServiceType = serviceType;
            ContractType = contractType;
            Address = address;
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext"/> object.
        /// </summary>
        /// <returns>
        /// A user-defined service object.
        /// </returns>
        /// <param name="instanceContext">
        /// The current <see cref="T:System.ServiceModel.InstanceContext"/> object.
        /// </param>
        public Object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext"/> object.
        /// </summary>
        /// <returns>
        /// The service object.
        /// </returns>
        /// <param name="instanceContext">
        /// The current <see cref="T:System.ServiceModel.InstanceContext"/> object.
        /// </param>
        /// <param name="message">
        /// The message that triggered the creation of a service object.
        /// </param>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            //if (!ServiceContainer.IsRegistered(ContractType, Address))
            //{
            //    ServiceContainer.Register(ContractType, ServiceType, Address);
            //}
            var instance = ServiceContainer.GetInstance(ContractType, Address);
            if (instance == null)
            {
                const String messageFormat = "No unity configuration was found for service type '{0}'";
                var failureMessage = string.Format(CultureInfo.InvariantCulture, messageFormat, ServiceType.FullName);

                throw new ConfigurationErrorsException(failureMessage);
            }

            return instance;
        }

        /// <summary>
        /// Called when an <see cref="T:System.ServiceModel.InstanceContext"/> object recycles a service object.
        /// </summary>
        /// <param name="instanceContext">
        /// The service's instance context.
        /// </param>
        /// <param name="instance">
        /// The service object to be recycled.
        /// </param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }

        /// <summary>
        /// Gets or sets the type of the service implementation.
        /// </summary>
        /// <value>
        /// The type of the service implementation.
        /// </value>
        protected Type ServiceType { get; set; }

        /// <summary>
        /// Gets or sets the type of the service contract.
        /// </summary>
        /// <value>
        /// The type of the service contract.
        /// </value>
        protected Type ContractType { get; set; }

        /// <summary>
        /// Gets or sets the endpoint address of the service.
        /// </summary>
        /// <value>
        /// The endpoint address of the service.
        /// </value>
        protected string Address { get; set; }
    }
}
