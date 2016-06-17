using Autofac.Extras.DynamicProxy2;
using Castle.DynamicProxy;
using Wheatech.ServiceModel.DynamicProxy;

namespace Wheatech.ServiceModel.Autofac.Interception
{
    public class AutofacServiceContainerExtension : IServiceContainerExtension
    {
        public void Initialize(IServiceContainer container)
        {
            container.Registering += OnRegistering;
        }

        public void Remove(IServiceContainer container)
        {
            container.Registering -= OnRegistering;
        }

        private void OnRegistering(object sender, ServiceRegisterEventArgs e)
        {
            ((AutofacServiceRegisterEventArgs)e).Registration.EnableClassInterceptors(new ProxyGenerationOptions
            {
                Selector = new ServiceInterceptorSelector((IServiceContainer)sender)
            });
        }
    }
}
