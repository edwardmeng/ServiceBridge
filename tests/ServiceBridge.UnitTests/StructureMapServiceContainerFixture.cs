using ServiceBridge.StructureMap;
using ServiceBridge.StructureMap.Interception;

namespace ServiceBridge.UnitTests
{
    public class StructureMapServiceContainerFixture: ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            return new StructureMapServiceContainer().AddExtension(new StructureMapServiceContainerExtension());
        }

        protected override string WebName => "structuremap";
    }
}
