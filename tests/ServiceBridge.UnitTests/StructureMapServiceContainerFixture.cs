using ServiceBridge.StructureMap;

namespace ServiceBridge.UnitTests
{
    public class StructureMapServiceContainerFixture: ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            var container = new StructureMapServiceContainer();
#if !NetCore
            container.AddExtension(new ServiceBridge.StructureMap.Interception.StructureMapServiceContainerExtension());
#endif
            return container;
        }

        protected override string WebName => "structuremap";
    }
}
