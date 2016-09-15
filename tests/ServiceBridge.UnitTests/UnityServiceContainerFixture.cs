using ServiceBridge.Unity;
using ServiceBridge.Unity.Interception;

namespace ServiceBridge.UnitTests
{
    public class UnityServiceContainerFixture : ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            return new UnityServiceContainer().AddExtension(new UnityServiceContainerExtension());
        }

        protected override string WebName => "unity";
    }
}
