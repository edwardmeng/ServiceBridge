using Wheatech.ServiceModel.Autofac;
using Wheatech.ServiceModel.Autofac.Interception;

namespace Wheatech.ServiceModel.UnitTests
{
    public class AutofacServiceContainerFixture : ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            return new AutofacServiceContainer().AddExtension(new AutofacServiceContainerExtension());
        }
    }
}
