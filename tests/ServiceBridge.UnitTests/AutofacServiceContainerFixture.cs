using ServiceBridge.Autofac;

namespace ServiceBridge.UnitTests
{
    public class AutofacServiceContainerFixture : ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            var container = new AutofacServiceContainer();
            container.AddExtension(new ServiceBridge.Autofac.Interception.AutofacServiceContainerExtension());
            return container;
        }

        protected override string WebName => "autofac";
    }
}
