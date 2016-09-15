using System.Web.Http;
using ServiceBridge.UnitTests.WebApp.Components;

namespace ServiceBridge.UnitTests.WebApp
{
    public class LifetimeController: ApiController
    {
        public string Get(string container)
        {
            Helper.InitializeServiceContainer(container);
            ServiceContainer.Current.Register<LifetimeTarget>(ServiceLifetime.PerRequest);
            return "Success";
        }

        public string Get(int value)
        {
            var target = ServiceContainer.GetInstance<LifetimeTarget>();
            target.Value = value.ToString();

            var target2 = ServiceContainer.GetInstance<LifetimeTarget>();
            return target2?.Value;
        }

        public string Get()
        {
            var target = ServiceContainer.GetInstance<LifetimeTarget>();
            return target?.Value;
        }
    }
}
