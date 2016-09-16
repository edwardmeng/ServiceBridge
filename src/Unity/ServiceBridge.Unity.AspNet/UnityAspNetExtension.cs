namespace ServiceBridge.Unity.AspNet
{
    /// <summary>
    /// The service container extension to enable Asp.Net integration for the <see cref="UnityServiceContainer"/>.
    /// </summary>
    public class UnityAspNetExtension: IServiceContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        public void Initialize(IServiceContainer container)
        {
            container.Registering += OnRegistering;
        }

        /// <summary>
        /// Removes the extension's functions from the container. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        public void Remove(IServiceContainer container)
        {
            container.Registering -= OnRegistering;
        }

        private void OnRegistering(object sender, ServiceRegisterEventArgs e)
        {
            if (e.Lifetime == ServiceLifetime.PerRequest)
            {
                ((UnityServiceRegisterEventArgs)e).LifetimeManager = new PerRequestLifetimeManager();
            }
        }
    }
}
