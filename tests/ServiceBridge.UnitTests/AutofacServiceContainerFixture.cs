using ServiceBridge.Autofac;

namespace ServiceBridge.UnitTests
{
    public class AutofacServiceContainerFixture : ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            var container = new AutofacServiceContainer();
#if !NetCore
            container.AddExtension(new ServiceBridge.Autofac.Interception.AutofacServiceContainerExtension());
#endif
            return container;
        }

        protected override string WebName => "autofac";
    }
}
