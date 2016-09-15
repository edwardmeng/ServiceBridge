namespace ServiceBridge.UnitTests
{
    public interface ICanChangeParameters
    {
        int MostRecentInput { get; set; }

        [DoubleInputInterceptor]
        void DoSomething(int i);

        [TripleOutputInterceptor]
        void DoSomethingElse(int i, out int j);

        [TripleOutputInterceptor]
        void DoSomethingElseWithRef(int i, ref int j);
    }
}
