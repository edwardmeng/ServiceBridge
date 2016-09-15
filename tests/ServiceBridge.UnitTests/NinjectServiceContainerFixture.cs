using ServiceBridge.Ninject;
using ServiceBridge.Ninject.Interception;

namespace ServiceBridge.UnitTests
{
    public class NinjectServiceContainerFixture : ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            return new NinjectServiceContainer().AddExtension(new NinjectServiceContainerExtension());
        }

        protected override string WebName => "ninject";
    }
}
