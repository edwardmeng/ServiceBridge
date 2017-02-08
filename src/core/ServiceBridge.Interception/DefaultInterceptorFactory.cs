using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ServiceBridge.Interception
{
    internal class DefaultInterceptorFactory : IInterceptorFactory
    {
        public IEnumerable<IInterceptor> CreateInterceptors(MethodInfo interfaceMethod, MethodInfo implementMethod, IServiceContainer container)
        {
            return from attribute in GetInterceptorAttributes(interfaceMethod, implementMethod)
                orderby attribute.Order
                select attribute.CreateInterceptor(container);
        }

        public IEnumerable<IInterceptor> CreateInterceptors(ConstructorInfo constructor, IServiceContainer container)
        {
            return from attribute in InterceptorAttribute.GetAttributes(constructor, false)
            orderby attribute.Order
            select attribute.CreateInterceptor(container);
        }

        private IEnumerable<InterceptorAttribute> GetInterceptorAttributes(MethodInfo interfaceMethod, MethodInfo implementMethod)
        {
            if (interfaceMethod != null)
            {
                foreach (var attr in InterceptorAttribute.GetAttributes(interfaceMethod, true))
                {
                    yield return attr;
                }
            }

            foreach (var attr in InterceptorAttribute.GetAttributes(implementMethod, true))
            {
                yield return attr;
            }
        }
    }
}
