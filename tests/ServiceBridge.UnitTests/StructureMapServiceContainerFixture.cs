using ServiceBridge.StructureMap;

namespace ServiceBridge.UnitTests
{
    public class StructureMapServiceContainerFixture: ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            var container = new StructureMapServiceContainer();
            container.AddExtension(new ServiceBridge.StructureMap.Interception.StructureMapServiceContainerExtension());
            return container;
        }

        protected override string WebName => "structuremap";
    }
}
