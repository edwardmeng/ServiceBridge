using Ninject.Syntax;

namespace ServiceBridge.Ninject.AspNet
{
    /// <summary>
    /// The service container extension to enable Asp.Net integration for the <see cref="NinjectServiceContainer"/>.
    /// </summary>
    internal class AspNetExtension : IServiceContainerExtension
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
#if NetCore
                ((IBindingInSyntax<object>)((NinjectServiceRegisterEventArgs)e).Binding).InScope(ctx => ServiceBridge.AspNetCore.ServiceBridgeMiddleware.CurrentContext);
#else 
                ((IBindingInSyntax<object>)((NinjectServiceRegisterEventArgs)e).Binding).InScope(ctx => System.Web.HttpContext.Current);
#endif
            }
        }
    }
}
