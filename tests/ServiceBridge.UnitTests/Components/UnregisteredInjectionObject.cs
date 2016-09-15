namespace ServiceBridge.UnitTests.Components
{
    public class UnregisteredInjectionObject
    {
        [Injection]
        public void Initialize(ILogger logger)
        {
            InjectionFromMethod = logger;
        }

        [Injection]
        public virtual ILogger InjectionFromProperty { get; set; }

        public virtual ILogger NotInjection { get; set; }

        public virtual ILogger InjectionFromMethod { get; set; }
    }
}
