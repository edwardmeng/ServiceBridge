using Wheatech.ServiceModel.DynamicProxy;
using Wheatech.ServiceModel.Interception;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace Wheatech.ServiceModel.Windsor.Interception
{
    public class WindsorServiceContainerExtension: IServiceContainerExtension
    {
        public void Initialize(IServiceContainer container)
        {
            ((WindsorServiceContainer) container).RegisterInstance(typeof(IInterceptor), new ServiceInterceptor(new PipelineManager(), container));
            container.Registering += OnRegistering;
        }

        public void Remove(IServiceContainer container)
        {
            container.Registering -= OnRegistering;
        }

        private void OnRegistering(object sender, ServiceRegisterEventArgs e)
        {
            //((WindsorServiceRegisterEventArgs)e).Registration.Interceptors()
        }
    }
}
