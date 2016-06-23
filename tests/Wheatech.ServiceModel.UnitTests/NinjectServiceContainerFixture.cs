using Wheatech.ServiceModel.Ninject;
using Wheatech.ServiceModel.Ninject.Interception;

namespace Wheatech.ServiceModel.UnitTests
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
