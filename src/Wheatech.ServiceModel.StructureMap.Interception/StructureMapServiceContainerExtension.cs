using System;
using System.Linq;
using System.Reflection;
using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.StructureMap.Interception
{
    public class StructureMapServiceContainerExtension : IServiceContainerExtension
    {
        public void Initialize(IServiceContainer container)
        {
            container.Registering += OnRegistering;
            container.Register<PipelineManager>();
        }

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
