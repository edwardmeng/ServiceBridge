using System;
using System.Linq;
using System.Reflection;
using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.StructureMap.Interception
{
    /// <summary>
    /// The service container extension to enable interception mechanism for the <see cref="StructureMapServiceContainer"/>.
    /// </summary>
    public class StructureMapServiceContainerExtension : IServiceContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        public void Initialize(IServiceContainer container)
        {
            container.Registering += OnRegistering;
            container.Register<PipelineManager>();
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
            if (e.ServiceType == typeof(PipelineManager))
            {
                e.Lifetime = ServiceLifetime.Singleton;
            }
            else if (ShouldIntercept(e.ImplementType))
            {
                ((StructureMapServiceRegisterEventArgs)e).Configuration.AddInterceptor(new DynamicProxyInterceptor(e.ServiceType, e.ImplementType));
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
