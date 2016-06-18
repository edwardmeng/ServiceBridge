using Ninject.Extensions.Interception.Infrastructure.Language;
using Ninject.Syntax;
using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.Ninject.Interception
{
    public class NinjectServiceContainerExtension: IServiceContainerExtension
    {
        public void Initialize(IServiceContainer container)
        {
            container.Register<PipelineManager>();
            container.Registering += OnRegistering;
        }

        public void Remove(IServiceContainer container)
        {
            container.Registering -= OnRegistering;
        }

        private void OnRegistering(object sender, ServiceRegisterEventArgs e)
        {
            if (e.ServiceType == typeof(PipelineManager))
            {
                ((NinjectServiceRegisterEventArgs)e).Lifetime = ServiceLifetime.Singleton;
            }
            else
            {
                var binding = ((NinjectServiceRegisterEventArgs)e).Binding;
                var container = (IServiceContainer)sender;
                (binding as IBindingOnSyntax<object>)?.Intercept().With(new NinjectServiceInterceptor(container.GetInstance<PipelineManager>(), container));
            }
        }
    }
}
