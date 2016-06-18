using Autofac.Extras.DynamicProxy2;
using Castle.DynamicProxy;
using Wheatech.ServiceModel.DynamicProxy;
using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.Autofac.Interception
{
    public class AutofacServiceContainerExtension : IServiceContainerExtension
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
                ((AutofacServiceRegisterEventArgs)e).Lifetime = ServiceLifetime.Singleton;
            }
            else
            {
                ((AutofacServiceRegisterEventArgs)e).Registration.EnableClassInterceptors(new ProxyGenerationOptions
                {
                    Selector = new ServiceInterceptorSelector((IServiceContainer)sender)
                });
            }
        }
    }
}
