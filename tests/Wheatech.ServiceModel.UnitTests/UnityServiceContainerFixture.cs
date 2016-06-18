using Wheatech.ServiceModel.Unity;
using Wheatech.ServiceModel.Unity.Interception;

namespace Wheatech.ServiceModel.UnitTests
{
    public class UnityServiceContainerFixture : ServiceContainerFixtureBase
    {
        protected override IServiceContainer CreateContainer()
        {
            return new UnityServiceContainer().AddExtension(new UnityServiceContainerExtension());
        }
    }
}
