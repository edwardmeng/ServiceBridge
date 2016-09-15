using ServiceBridge.Autofac;
using ServiceBridge.Autofac.Interception;

namespace ServiceBridge.UnitTests
{
    public class AutofacServiceContainerFixture : ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            return new AutofacServiceContainer().AddExtension(new AutofacServiceContainerExtension());
        }

        protected override string WebName => "autofac";
    }
}
