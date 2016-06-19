namespace Wheatech.ServiceModel.UnitTests
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

        public ObjectWithInjection(ICanChangeParameters obj)
        {
            NotInjectionFromConstructor = obj;
        }

        [Injection]
        public void Initialize(ILogger logger)
        {
            InjectionFromMethod = logger;
        }

        public ILogger InjectionFromConstructor { get; set; }

        public ICanChangeParameters NotInjectionFromConstructor { get; set; }

        [Injection]
        public virtual ILogger InjectionFromProperty { get; set; }

        public virtual ILogger NotInjectionFromProperty { get; set; }

        public virtual ILogger InjectionFromMethod { get; set; }
    }
}
