namespace ServiceBridge.UnitTests
{
    public class CanChangeParametersTarget:ICanChangeParameters
    {
        public virtual int MostRecentInput { get; set; }

        public virtual void DoSomething(int i)
        {
            MostRecentInput = i;
        }

        public virtual void DoSomethingElse(int i, out int j)
        {
            j = i + 5;
        }

        public virtual void DoSomethingElseWithRef(int i, ref int j)
        {
            j = i + j + 5;
        }
    }
}
