using System;
using System.Linq;
using System.Reflection;
using Wheatech.ServiceModel.DynamicProxy;
using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.Windsor.Interception
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
            ((WindsorServiceContainer)container).RegisterInstance(typeof(ServiceInterceptor), new ServiceInterceptor(new PipelineManager(), container));
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
