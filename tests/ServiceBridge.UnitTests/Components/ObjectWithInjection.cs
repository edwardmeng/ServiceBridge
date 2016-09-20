namespace ServiceBridge.UnitTests
{
    public class ObjectWithInjection
    {
        public ObjectWithInjection()
        {
        }

        [Injection]
        public ObjectWithInjection(ILogger logger)
        {
            InjectionFromConstructor = logger;
        }

        public ObjectWithInjection(InstanceObject obj)
        {
            NotInjectionFromConstructor = obj;
        }

        [Injection]
        public void Initialize(ILogger logger)
        {
            InjectionFromMethod = logger;
        }

        public virtual ILogger InjectionFromConstructor { get; set; }

        public virtual InstanceObject NotInjectionFromConstructor { get; set; }

        [Injection]
        public virtual ILogger InjectionFromProperty { get; set; }

        public virtual ILogger NotInjection { get; set; }

        public virtual ILogger InjectionFromMethod { get; set; }
    }
}
