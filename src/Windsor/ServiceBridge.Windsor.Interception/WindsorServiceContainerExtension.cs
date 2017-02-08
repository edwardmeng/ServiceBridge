using System;
using System.Linq;
using System.Reflection;
using ServiceBridge.DynamicProxy;
using ServiceBridge.Interception;

namespace ServiceBridge.Windsor.Interception
{
    /// <summary>
    /// The service container extension to enable interception mechanism for the <see cref="WindsorServiceContainer"/>.
    /// </summary>
    public class WindsorServiceContainerExtension : IServiceContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        public void Initialize(IServiceContainer container)
        {
            container.Registering += OnRegistering;
            var interceptor = new ServiceInterceptor(container);
            ((WindsorServiceContainer)container).RegisterInstance(typeof(ServiceInterceptor), interceptor);
            container.UseDefaultInterceptorFactory();
            interceptor.PipelineManager = new PipelineManager(container.GetInstance<IInterceptorFactory>());
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
            if (ShouldIntercept(e.ImplementType))
            {
                ((WindsorServiceRegisterEventArgs)e).Registration.Interceptors<ServiceInterceptor>();
            }
        }

        private bool ShouldIntercept(Type type)
        {
            return type
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Any(method => method.DeclaringType != typeof(object) && !method.IsPrivate && !method.IsFinal);
        }
    }
}
