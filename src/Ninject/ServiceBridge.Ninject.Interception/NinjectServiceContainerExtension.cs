using System;
using System.Linq;
using System.Reflection;
using Ninject.Extensions.Interception.Infrastructure.Language;
using Ninject.Extensions.Interception.ProxyFactory;
using Ninject.Syntax;
using ServiceBridge.Interception;

namespace ServiceBridge.Ninject.Interception
{
    /// <summary>
    /// The service container extension to enable interception mechanism for the <see cref="NinjectServiceContainer"/>.
    /// </summary>
    public class NinjectServiceContainerExtension : IServiceContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        public void Initialize(IServiceContainer container)
        {
            container.Register<PipelineManager>(ServiceLifetime.Singleton);
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
            if (e.ServiceType != typeof(PipelineManager)&& ShouldIntercept(e.ImplementType))
            {
                var binding = ((NinjectServiceRegisterEventArgs)e).Binding;
                var container = (IServiceContainer)sender;
                (binding as IBindingOnSyntax<object>)?.Intercept().With(new NinjectServiceInterceptor(container.GetInstance<PipelineManager>(), container));
            }
        }

        private bool ShouldIntercept(Type type)
        {
            return !type.IsSealed && type
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Any(method => method.DeclaringType != typeof(object) && !method.IsPrivate && !method.IsFinal);
        }
    }
}
