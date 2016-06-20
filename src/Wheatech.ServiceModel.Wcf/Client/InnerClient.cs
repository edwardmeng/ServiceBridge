using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Wheatech.ServiceModel.Wcf
{
    internal class InnerClient<TChannel> : System.ServiceModel.ClientBase<TChannel>
            where TChannel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the default target endpoint from the application configuration file. 
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">Either there is no default endpoint information in the configuration file, more than one endpoint in the file, or no configuration file.</exception>
        public InnerClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the configuration information specified in the application configuration file by <paramref name="endpointConfigurationName"/>. 
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param><exception cref="T:System.ArgumentNullException">The specified endpoint information is null.</exception><exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public InnerClient(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param><param name="remoteAddress">The address of the service.</param><exception cref="T:System.ArgumentNullException">The endpoint is null.</exception><exception cref="T:System.ArgumentNullException">The remote address is null.</exception><exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public InnerClient(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the specified target address and endpoint information. 
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param><param name="remoteAddress">The address of the service.</param><exception cref="T:System.ArgumentNullException">The endpoint is null.</exception><exception cref="T:System.ArgumentNullException">The remote address is null.</exception><exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public InnerClient(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the specified binding and target address. 
        /// </summary>
        /// <param name="binding">The binding with which to make calls to the service.</param><param name="remoteAddress">The address of the service endpoint.</param><exception cref="T:System.ArgumentNullException">The binding is null.</exception><exception cref="T:System.ArgumentNullException">The remote address is null.</exception>
        public InnerClient(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the specified  <see cref="T:System.ServiceModel.Description.ServiceEndpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint for a service that allows clients to find and communicate with the service.</param>
        public InnerClient(ServiceEndpoint endpoint)
            : base(endpoint)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the <paramref name="callbackInstance"/> as the callback object in a duplex conversation. 
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client application uses to listen for messages from the connected service.</param><exception cref="T:System.ArgumentNullException">The callback instance is null.</exception><exception cref="T:System.InvalidOperationException">Either there is no default endpoint information in the configuration file, more than one endpoint in the file, or no configuration file.</exception>
        public InnerClient(InstanceContext callbackInstance)
            : base(callbackInstance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the specified callback service and endpoint configuration information. 
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client uses to listen for messages from the connected service.</param><param name="endpointConfigurationName">The name of the endpoint in the application configuration file. </param><exception cref="T:System.ArgumentNullException">The callback instance is null.</exception><exception cref="T:System.ArgumentNullException">The endpoint is null.</exception><exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public InnerClient(InstanceContext callbackInstance, string endpointConfigurationName)
            : base(callbackInstance, endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class.
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client uses to listen for messages from the connected service.</param><param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param><param name="remoteAddress">The address of the service.</param><exception cref="T:System.ArgumentNullException">The callback instance is null.</exception><exception cref="T:System.ArgumentNullException">The endpoint is null.</exception><exception cref="T:System.ArgumentNullException">The remote address is null.</exception><exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public InnerClient(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class.
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client uses to listen for messages from the connected service.</param><param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param><param name="remoteAddress">The address of the service.</param><exception cref="T:System.ArgumentNullException">The callback instance is null.</exception><exception cref="T:System.ArgumentNullException">The endpoint is null.</exception><exception cref="T:System.ArgumentNullException">The remote address is null.</exception><exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public InnerClient(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class. 
        /// </summary>
        /// <param name="callbackInstance">The callback service.</param><param name="binding">The binding with which to call the service.</param><param name="remoteAddress">The address of the service endpoint.</param><exception cref="T:System.ArgumentNullException">The callback instance is null.</exception><exception cref="T:System.ArgumentNullException">The binding is null.</exception><exception cref="T:System.ArgumentNullException">The remote address is null.</exception>
        public InnerClient(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
            : base(callbackInstance, binding, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the specified  <see cref="T:System.ServiceModel.InstanceContext"/> and  <see cref="T:System.ServiceModel.Description.ServiceEndpoint"/> objects.
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client application uses to listen for messages from the connected service.</param><param name="endpoint">The endpoint for a service that allows clients to find and communicate with the service.</param>
        public InnerClient(InstanceContext callbackInstance, ServiceEndpoint endpoint)
            : base(callbackInstance, endpoint)
        {
        }

        public new TChannel Channel => base.Channel;
    }
}
