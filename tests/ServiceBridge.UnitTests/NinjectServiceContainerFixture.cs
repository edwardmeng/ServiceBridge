using ServiceBridge.Ninject;

namespace ServiceBridge.UnitTests
{
    public class NinjectServiceContainerFixture : ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            var container = new NinjectServiceContainer();
#if !NetCore
            container..AddExtension(new ServiceBridge.Ninject.Interception.NinjectServiceContainerExtension());
#endif
            return container;
        }

        protected override string WebName => "ninject";
    }
}
