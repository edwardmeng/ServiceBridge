using ServiceBridge.Autofac;
using ServiceBridge.Autofac.Interception;
using ServiceBridge.Ninject;
using ServiceBridge.Ninject.Interception;
using ServiceBridge.StructureMap;
using ServiceBridge.StructureMap.Interception;
using ServiceBridge.Unity;
using ServiceBridge.Unity.Interception;
using ServiceBridge.Windsor;
using ServiceBridge.Windsor.Interception;

namespace ServiceBridge.UnitTests.WebApp
{
    public static class Helper
    {
        public static void InitializeServiceContainer(string container)
        {
            switch (container)
            {
                case "autofac":
                    ServiceContainer.SetProvider(() => new AutofacServiceContainer().AddNewExtension<AutofacServiceContainerExtension>());
                    break;
                case "ninject":
                    ServiceContainer.SetProvider(() => new NinjectServiceContainer().AddNewExtension<NinjectServiceContainerExtension>());
                    break;
                case "structuremap":
                    ServiceContainer.SetProvider(() => new StructureMapServiceContainer().AddNewExtension<StructureMapServiceContainerExtension>());
                    break;
                case "unity":
                    ServiceContainer.SetProvider(() => new UnityServiceContainer().AddNewExtension<UnityServiceContainerExtension>());
                    break;
                case "windsor":
                    ServiceContainer.SetProvider(() => new WindsorServiceContainer().AddNewExtension<WindsorServiceContainerExtension>());
                    break;
            }
        }
    }
}