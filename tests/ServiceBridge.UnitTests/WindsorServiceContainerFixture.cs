using ServiceBridge.Windsor;
using ServiceBridge.Windsor.Interception;

namespace ServiceBridge.UnitTests
{
    public class WindsorServiceContainerFixture : ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            return new WindsorServiceContainer().AddExtension(new WindsorServiceContainerExtension());
        }

        protected override string WebName => "windsor";
    }
}
