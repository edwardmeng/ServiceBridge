using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.UnitTests
{
    public class DoubleInputInterceptor : InterceptorAttribute, IInterceptor
    {
        public override IInterceptor CreateInterceptor(IServiceContainer container)
        {
            return this;
        }

        public IMethodReturn Invoke(IMethodInvocation invocation, GetNextInterceptorHandler getNext)
        {
            int i = (int)invocation.Arguments[0].Value * 2;
            invocation.Arguments[0].Value = i;
            return getNext()(invocation, getNext);
        }
    }
}
