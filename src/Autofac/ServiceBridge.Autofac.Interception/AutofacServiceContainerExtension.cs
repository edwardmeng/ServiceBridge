using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using ServiceBridge.DynamicProxy;
using ServiceBridge.Interception;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace ServiceBridge.Autofac.Interception
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
            container.Register<PipelineManager>(ServiceLifetime.Singleton).UseDefaultInterceptorFactory();
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
#if NetCore
                    // Fix the constructor injection in the interception mechanism.
                    if (type.IsAssignableTo<IProxyTargetAccessor>())
                    {
                        var constructors = InjectionAttribute.GetConstructors(type.GetTypeInfo().BaseType).ToArray();
                        if (constructors.Length > 0)
                        {
                            return constructors.Select(ctor => type.GetTypeInfo().GetConstructor(new[] { typeof(IInterceptor[]), typeof(IInterceptorSelector) }.Concat(ctor.GetParameters().Select(parameter => parameter.ParameterType)).ToArray())).ToArray();
                        }
                    }
                    return type.GetConstructors();
#else
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
#endif
                });
            }
        }
    }
}
