using Wheatech.ServiceModel.Ninject;

namespace Wheatech.ServiceModel.UnitTests
{
    public class NinjectServiceContainerFixture: ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            return new NinjectServiceContainer();
        }
    }
}
