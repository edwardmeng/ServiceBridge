using System;
using System.Linq;
using System.Reflection;
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
            else if (ShouldIntercept(e.ImplementType))
            {
                var binding = ((NinjectServiceRegisterEventArgs)e).Binding;
                var container = (IServiceContainer)sender;
                (binding as IBindingOnSyntax<object>)?.Intercept().With(new NinjectServiceInterceptor(container.GetInstance<PipelineManager>(), container));
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
