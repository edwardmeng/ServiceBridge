using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Wheatech.ServiceModel.Wcf
{
    /// <summary>
    /// Provides the base implementation used to create Windows Communication Foundation (WCF) client objects that can call services.
    /// </summary>
    /// <typeparam name="TChannel">The channel to be used to connect to the service.</typeparam>
    public class ClientBase<TChannel> : ICommunicationObject, IDisposable
          where TChannel : class
    {
        #region Fields

        private readonly InnerClient<TChannel> _innerClient;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the default target endpoint from the application configuration file. 
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">Either there is no default endpoint information in the configuration file, more than one endpoint in the file, or no configuration file.</exception>
        public ClientBase()
            : this("*")
        {
            _innerClient = new InnerClient<TChannel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the configuration information specified in the application configuration file by <paramref name="endpointConfigurationName"/>. 
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param><exception cref="T:System.ArgumentNullException">The specified endpoint information is null.</exception><exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public ClientBase(string endpointConfigurationName)
        {
            _innerClient = new InnerClient<TChannel>(endpointConfigurationName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param><param name="remoteAddress">The address of the service.</param><exception cref="T:System.ArgumentNullException">The endpoint is null.</exception><exception cref="T:System.ArgumentNullException">The remote address is null.</exception><exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public ClientBase(string endpointConfigurationName, string remoteAddress)
        {
            _innerClient = new InnerClient<TChannel>(endpointConfigurationName, remoteAddress);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the specified target address and endpoint information. 
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param><param name="remoteAddress">The address of the service.</param><exception cref="T:System.ArgumentNullException">The endpoint is null.</exception><exception cref="T:System.ArgumentNullException">The remote address is null.</exception><exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public ClientBase(string endpointConfigurationName, EndpointAddress remoteAddress)
        {
            _innerClient = new InnerClient<TChannel>(endpointConfigurationName, remoteAddress);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the specified binding and target address. 
        /// </summary>
        /// <param name="binding">The binding with which to make calls to the service.</param><param name="remoteAddress">The address of the service endpoint.</param><exception cref="T:System.ArgumentNullException">The binding is null.</exception><exception cref="T:System.ArgumentNullException">The remote address is null.</exception>
        public ClientBase(Binding binding, EndpointAddress remoteAddress)
        {
            _innerClient = new InnerClient<TChannel>(binding, remoteAddress);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the specified  <see cref="T:System.ServiceModel.Description.ServiceEndpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint for a service that allows clients to find and communicate with the service.</param>
        public ClientBase(ServiceEndpoint endpoint)
        {
            _innerClient = new InnerClient<TChannel>(endpoint);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the <paramref name="callbackInstance"/> as the callback object in a duplex conversation. 
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client application uses to listen for messages from the connected service.</param><exception cref="T:System.ArgumentNullException">The callback instance is null.</exception><exception cref="T:System.InvalidOperationException">Either there is no default endpoint information in the configuration file, more than one endpoint in the file, or no configuration file.</exception>
        public ClientBase(InstanceContext callbackInstance)
        {
            _innerClient = new InnerClient<TChannel>(callbackInstance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the specified callback service and endpoint configuration information. 
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client uses to listen for messages from the connected service.</param><param name="endpointConfigurationName">The name of the endpoint in the application configuration file. </param><exception cref="T:System.ArgumentNullException">The callback instance is null.</exception><exception cref="T:System.ArgumentNullException">The endpoint is null.</exception><exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public ClientBase(InstanceContext callbackInstance, string endpointConfigurationName)
        {
            _innerClient = new InnerClient<TChannel>(callbackInstance, endpointConfigurationName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class.
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client uses to listen for messages from the connected service.</param><param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param><param name="remoteAddress">The address of the service.</param><exception cref="T:System.ArgumentNullException">The callback instance is null.</exception><exception cref="T:System.ArgumentNullException">The endpoint is null.</exception><exception cref="T:System.ArgumentNullException">The remote address is null.</exception><exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public ClientBase(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress)
        {
            _innerClient = new InnerClient<TChannel>(callbackInstance, endpointConfigurationName, remoteAddress);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class.
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client uses to listen for messages from the connected service.</param><param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param><param name="remoteAddress">The address of the service.</param><exception cref="T:System.ArgumentNullException">The callback instance is null.</exception><exception cref="T:System.ArgumentNullException">The endpoint is null.</exception><exception cref="T:System.ArgumentNullException">The remote address is null.</exception><exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public ClientBase(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress)
        {
            _innerClient = new InnerClient<TChannel>(callbackInstance, endpointConfigurationName, remoteAddress);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class. 
        /// </summary>
        /// <param name="callbackInstance">The callback service.</param><param name="binding">The binding with which to call the service.</param><param name="remoteAddress">The address of the service endpoint.</param><exception cref="T:System.ArgumentNullException">The callback instance is null.</exception><exception cref="T:System.ArgumentNullException">The binding is null.</exception><exception cref="T:System.ArgumentNullException">The remote address is null.</exception>
        public ClientBase(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
        {
            _innerClient = new InnerClient<TChannel>(callbackInstance, binding, remoteAddress);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.ServiceModel.ClientBase`1"/> class using the specified  <see cref="T:System.ServiceModel.InstanceContext"/> and  <see cref="T:System.ServiceModel.Description.ServiceEndpoint"/> objects.
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client application uses to listen for messages from the connected service.</param><param name="endpoint">The endpoint for a service that allows clients to find and communicate with the service.</param>
        public ClientBase(InstanceContext callbackInstance, ServiceEndpoint endpoint)
        {
            _innerClient = new InnerClient<TChannel>(callbackInstance, endpoint);
        }

        #endregion

        #region ICommunicationObject Members

        /// <summary>
        /// Causes a communication object to transition immediately from its current state into the closed state.
        /// </summary>
        public void Abort() => _innerClient?.Abort();

        /// <summary>
        /// Causes a communication object to transition from its current state into the closed state.
        /// </summary>
        public void Close() => _innerClient?.Close();

        /// <summary>
        /// Causes a communication object to transition from its current state into the closed state.
        /// </summary>
        /// <param name="timeout">The <see cref="T:System.Timespan" /> that specifies how long the send operation has to complete before timing out.</param>
        void ICommunicationObject.Close(TimeSpan timeout) => ((ICommunicationObject) _innerClient)?.Close(timeout);

        /// <summary>
        /// Begins an asynchronous operation to close a communication object.
        /// </summary>
        /// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate that receives notification of the completion of the asynchronous close operation.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous close operation.</param>
        /// <returns>
        /// The <see cref="T:System.IAsyncResult" /> that references the asynchronous close operation.
        /// </returns>
        IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, object state) => ((ICommunicationObject) _innerClient)?.BeginClose(callback, state);

        /// <summary>
        /// Begins an asynchronous operation to close a communication object with a specified timeout.
        /// </summary>
        /// <param name="timeout">The <see cref="T:System.Timespan" /> that specifies how long the send operation has to complete before timing out.</param>
        /// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate that receives notification of the completion of the asynchronous close operation.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous close operation.</param>
        /// <returns>
        /// The <see cref="T:System.IAsyncResult" /> that references the asynchronous close operation.
        /// </returns>
        IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, object state) => ((ICommunicationObject) _innerClient)?.BeginClose(timeout, callback, state);

        /// <summary>
        /// Completes an asynchronous operation to close a communication object.
        /// </summary>
        /// <param name="result">The <see cref="T:System.IAsyncResult" /> that is returned by a call to the <see cref="M:System.ServiceModel.ICommunicationObject.BeginClose" /> method.</param>
        void ICommunicationObject.EndClose(IAsyncResult result) => ((ICommunicationObject) _innerClient)?.EndClose(result);

        /// <summary>
        /// Causes a communication object to transition from the created state into the opened state.
        /// </summary>
        public void Open() => _innerClient?.Open();

        /// <summary>
        /// Causes a communication object to transition from the created state into the opened state within a specified interval of time.
        /// </summary>
        /// <param name="timeout">The <see cref="T:System.Timespan" /> that specifies how long the send operation has to complete before timing out.</param>
        void ICommunicationObject.Open(TimeSpan timeout) => ((ICommunicationObject) _innerClient)?.Open(timeout);

        /// <summary>
        /// Begins an asynchronous operation to open a communication object.
        /// </summary>
        /// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate that receives notification of the completion of the asynchronous open operation.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous open operation.</param>
        /// <returns>
        /// The <see cref="T:System.IAsyncResult" /> that references the asynchronous open operation.
        /// </returns>
        IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, object state) => ((ICommunicationObject) _innerClient)?.BeginOpen(callback, state);

        /// <summary>
        /// Begins an asynchronous operation to open a communication object within a specified interval of time.
        /// </summary>
        /// <param name="timeout">The <see cref="T:System.Timespan" /> that specifies how long the send operation has to complete before timing out.</param>
        /// <param name="callback">The <see cref="T:System.AsyncCallback" /> delegate that receives notification of the completion of the asynchronous open operation.</param>
        /// <param name="state">An object, specified by the application, that contains state information associated with the asynchronous open operation.</param>
        /// <returns>
        /// The <see cref="T:System.IAsyncResult" /> that references the asynchronous open operation.
        /// </returns>
        IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => ((ICommunicationObject) _innerClient)?.BeginOpen(timeout, callback, state);

        /// <summary>
        /// Completes an asynchronous operation to open a communication object.
        /// </summary>
        /// <param name="result">The <see cref="T:System.IAsyncResult" /> that is returned by a call to the <see cref="M:System.ServiceModel.ICommunicationObject.BeginOpen" /> method.</param>
        void ICommunicationObject.EndOpen(IAsyncResult result) => ((ICommunicationObject) _innerClient)?.EndOpen(result);

        /// <summary>
        /// Gets the current state of the <see cref="ClientBase{TChannel}"/> object.
        /// </summary>
        /// <value>The value of the <see cref="CommunicationState"/> of the object.</value>
        public CommunicationState State => _innerClient?.State ?? CommunicationState.Closed;

        /// <summary>
        /// Occurs when the communication object completes its transition from the closing state into the closed state.
        /// </summary>
        event EventHandler ICommunicationObject.Closed
        {
            add
            {
                if (_innerClient != null)
                    ((ICommunicationObject)_innerClient).Closed += value;
            }
            remove
            {
                if (_innerClient != null)
                    ((ICommunicationObject)_innerClient).Closed -= value;
            }
        }

        /// <summary>
        /// Occurs when the communication object first enters the closing state.
        /// </summary>
        event EventHandler ICommunicationObject.Closing
        {
            add
            {
                if (_innerClient != null)
                    ((ICommunicationObject)_innerClient).Closing += value;
            }
            remove
            {
                if (_innerClient != null)
                    ((ICommunicationObject)_innerClient).Closing -= value;
            }
        }

        /// <summary>
        /// Occurs when the communication object first enters the faulted state.
        /// </summary>
        event EventHandler ICommunicationObject.Faulted
        {
            add
            {
                if (_innerClient != null)
                    ((ICommunicationObject)_innerClient).Faulted += value;
            }
            remove
            {
                if (_innerClient != null)
                    ((ICommunicationObject)_innerClient).Faulted -= value;
            }
        }

        /// <summary>
        /// Occurs when the communication object completes its transition from the opening state into the opened state.
        /// </summary>
        event EventHandler ICommunicationObject.Opened
        {
            add
            {
                if (_innerClient != null)
                    ((ICommunicationObject)_innerClient).Opened += value;
            }
            remove
            {
                if (_innerClient != null)
                    ((ICommunicationObject)_innerClient).Opened -= value;
            }
        }

        /// <summary>
        /// Occurs when the communication object first enters the opening state.
        /// </summary>
        event EventHandler ICommunicationObject.Opening
        {
            add
            {
                if (_innerClient != null)
                    ((ICommunicationObject)_innerClient).Opening += value;
            }
            remove
            {
                if (_innerClient != null)
                    ((ICommunicationObject)_innerClient).Opening -= value;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            // When the service client state is faulted, the dispose cannot be processed.
            // Or else an communication exception will be encountered, which will hide the actual exception.
            if (_innerClient != null && _innerClient.State != CommunicationState.Faulted)
                ((IDisposable)_innerClient).Dispose();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the inner channel used to send messages to variously configured service endpoints.
        /// </summary>
        /// <value>A channel of a specified type.</value>
        public TChannel Channel => _innerClient?.Channel;

        /// <summary>
        /// Gets the underlying <see cref="T:ChannelFactory{TChannel}"/> object.
        /// </summary>
        /// <value>A <see cref="T:ChannelFactory{TChannel}"/> object.</value>
        public ChannelFactory<TChannel> ChannelFactory => _innerClient?.ChannelFactory;

        /// <summary>
        /// Gets the client credentials used to call an operation.
        /// </summary>
        /// <value>Returns a <see cref="ClientCredentials"/> that represents the proof of identity presented by the client.</value>
        /// <remarks>
        /// Use the ClientCredentials property to configure the credentials that the <see cref="ClientBase{TChannel}"/> object uses to connect to the service.
        /// </remarks>
        public ClientCredentials ClientCredentials => _innerClient?.ClientCredentials;

        /// <summary>
        /// Gets the target endpoint for the service to which the WCF client can connect.
        /// </summary>
        /// <value>The target endpoint.</value>
        /// <remarks>
        /// Use the Endpoint property to obtain the <see cref="ServiceEndpoint"/> object to inspect or modify prior to using the <see cref="ClientBase{TChannel}"/> object.
        /// </remarks>
        public ServiceEndpoint Endpoint => _innerClient?.Endpoint;

        /// <summary>
        /// Gets the underlying <see cref="IClientChannel"/> implementation.
        /// </summary>
        /// <value>The client channel for the WCF client object.</value>
        /// <remarks>
        /// Use the InnerChannel property to obtain the <see cref="IClientChannel"/> that is used to connect to the service.
        /// </remarks>
        public IClientChannel InnerChannel => _innerClient?.InnerChannel;

        #endregion
    }
}
