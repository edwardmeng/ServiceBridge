using Wheatech.ServiceModel.Windsor;
using Wheatech.ServiceModel.Windsor.Interception;

namespace Wheatech.ServiceModel.UnitTests
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
