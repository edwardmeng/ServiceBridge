using System.Linq;
using Autofac;
using Autofac.Extras.DynamicProxy2;
using Castle.DynamicProxy;
using Wheatech.ServiceModel.DynamicProxy;
using Wheatech.ServiceModel.Interception;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

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
                }).FindConstructorsWith(type =>
                {
                    if (type.IsAssignableTo<IProxyTargetAccessor>())
                    {
                        var constructors = InjectionAttribute.GetConstructors(type.BaseType).ToArray();
                        if (constructors.Length > 0)
                        {
                            return constructors.Select(ctor => type.GetConstructor(new[] {typeof(IInterceptor[]), typeof(IInterceptorSelector)}.Concat(ctor.GetParameters().Select(parameter => parameter.ParameterType)).ToArray())).ToArray();
                        }
                    }
                    return type.GetConstructors();
                });
            }
        }
    }
}
