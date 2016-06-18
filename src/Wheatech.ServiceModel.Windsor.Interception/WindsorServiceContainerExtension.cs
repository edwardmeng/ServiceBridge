using System;
using System.Linq;
using System.Reflection;
using Wheatech.ServiceModel.DynamicProxy;
using Wheatech.ServiceModel.Interception;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace Wheatech.ServiceModel.Windsor.Interception
{
    public class WindsorServiceContainerExtension: IServiceContainerExtension
    {
        public void Initialize(IServiceContainer container)
        {
            container.Registering += OnRegistering;
            ((WindsorServiceContainer) container).RegisterInstance(typeof(ServiceInterceptor), new ServiceInterceptor(new PipelineManager(), container));
        }

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
