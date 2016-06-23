using Wheatech.ServiceModel.StructureMap;
using Wheatech.ServiceModel.StructureMap.Interception;

namespace Wheatech.ServiceModel.UnitTests
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
