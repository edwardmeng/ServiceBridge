using ServiceBridge.Interception;

namespace ServiceBridge.UnitTests
{
    public class TripleOutputInterceptor : InterceptorAttribute, IInterceptor
    {
        public IMethodReturn Invoke(IMethodInvocation invocation, GetNextInterceptorHandler getNext)
        {
            var result = getNext()(invocation, getNext);
            if (result.Exception == null)
            {
                int output = (int)result.Outputs[0].Value * 3;
                result.Outputs[0].Value = output;
            }
            return result;
        }

        public override IInterceptor CreateInterceptor(IServiceContainer container)
        {
            return this;
        }
    }
}
