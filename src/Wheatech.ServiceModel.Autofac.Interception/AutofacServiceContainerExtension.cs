using System.Linq;
using Autofac;
using Autofac.Extras.DynamicProxy2;
using Castle.DynamicProxy;
using Wheatech.ServiceModel.DynamicProxy;
using Wheatech.ServiceModel.Interception;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace Wheatech.ServiceModel.Autofac.Interception
{
    /// <summary>
    /// The service container extension to enable interception mechanism for the <see cref="AutofacServiceContainer"/>.
    /// </summary>
    public class AutofacServiceContainerExtension : IServiceContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality. 
        /// </summary>
        /// <param name="container">The container this extension to extend.</param>
        public void Initialize(IServiceContainer container)
        {
            container.Registering += OnRegistering;
            container.Register<PipelineManager>(ServiceLifetime.Singleton);
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
            if (e.ServiceType != typeof(PipelineManager))
            {
                ((AutofacServiceRegisterEventArgs)e).Registration.EnableClassInterceptors(new ProxyGenerationOptions
                {
                    Selector = new ServiceInterceptorSelector((IServiceContainer)sender)
                }).FindConstructorsWith(type =>
                {
                    // Fix the constructor injection in the interception mechanism.
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
