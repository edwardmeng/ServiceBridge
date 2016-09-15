namespace ServiceBridge.Sample.Components
{
    public class Startup
    {
        public void Configuration(IServiceContainer container)
        {
            container.Register<ICacheRepository, CacheRepository>(ServiceLifetime.Singleton);
        }
    }
}