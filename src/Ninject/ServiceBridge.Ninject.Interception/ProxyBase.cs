using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace ServiceBridge.Ninject.Interception
{
    public class ProxyBase
    {
        private class ObjectMethodsInvocation : IInvocation
        {
            public ObjectMethodsInvocation(object proxy, object target, MethodInfo method, object[] arguments)
            {
                Proxy = proxy;
                InvocationTarget = target;
                Method = method;
                MethodInvocationTarget = method;
                Arguments = arguments;
            }

            public object GetArgumentValue(int index)
            {
                throw new NotSupportedException();
            }

            public MethodInfo GetConcreteMethod()
            {
                return Method;
            }

            public MethodInfo GetConcreteMethodInvocationTarget()
            {
                throw new NotSupportedException();
            }

            public void Proceed()
            {
                Method.Invoke(InvocationTarget, Arguments);
            }

            public void SetArgumentValue(int index, object value)
            {
                throw new NotSupportedException();
            }

            public object[] Arguments { get; }

            public Type[] GenericArguments => new Type[0];

            public object InvocationTarget { get; }
            public MethodInfo Method { get; }
            public MethodInfo MethodInvocationTarget { get; }
            public object Proxy { get; }
            public object ReturnValue { get; set; }

            public Type TargetType => InvocationTarget.GetType();
        }

        private static readonly MethodInfo ToStringMethodInfo = typeof(object).GetMethod("ToString");
        private static readonly MethodInfo GetHashCodeMethodInfo = typeof(object).GetMethod("GetHashCode");
        private static readonly MethodInfo EqualsMethodInfo = typeof(object).GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public);

        public override int GetHashCode()
        {
            return InterceptMethod(() => base.GetHashCode(), GetHashCodeMethodInfo, new object[0]);
        }

        public override string ToString()
        {
            return InterceptMethod(() => base.ToString(), ToStringMethodInfo, new object[0]);
        }

        public override bool Equals(object obj)
        {
            return InterceptMethod(() => base.Equals(obj), EqualsMethodInfo, new[] { obj });
        }

        private TResult InterceptMethod<TResult>(Func<TResult> invokeBase, MethodInfo method, object[] arguments)
        {
            var proxy = this as IProxyTargetAccessor;
            if (proxy == null)
                return invokeBase();

            var interceptor = proxy.GetInterceptors().FirstOrDefault() as DynamicProxyWrapper;
            if (interceptor == null)
                return invokeBase();

            var invocation = new ObjectMethodsInvocation(this, interceptor.Instance, method, arguments);
            interceptor.Intercept(invocation);
            return (TResult)invocation.ReturnValue;
        }
    }
}
