using ServiceBridge.Autofac;
using ServiceBridge.Autofac.AspNet;
using ServiceBridge.Autofac.Interception;
using ServiceBridge.Ninject;
using ServiceBridge.Ninject.AspNet;
using ServiceBridge.Ninject.Interception;
using ServiceBridge.StructureMap;
using ServiceBridge.StructureMap.AspNet;
using ServiceBridge.StructureMap.Interception;
using ServiceBridge.Unity;
using ServiceBridge.Unity.AspNet;
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
                    ServiceContainer.SetProvider(() => new AutofacServiceContainer().AddNewExtension<AutofacServiceContainerExtension>().AddNewExtension<AutofacAspNetExtension>());
                    break;
                case "ninject":
                    ServiceContainer.SetProvider(() => new NinjectServiceContainer().AddNewExtension<NinjectServiceContainerExtension>().AddNewExtension<NinjectAspNetExtension>());
                    break;
                case "structuremap":
                    ServiceContainer.SetProvider(() => new StructureMapServiceContainer().AddNewExtension<StructureMapServiceContainerExtension>().AddNewExtension<StructureMapAspNetExtension>());
                    break;
                case "unity":
                    ServiceContainer.SetProvider(() => new UnityServiceContainer().AddNewExtension<UnityServiceContainerExtension>().AddNewExtension<UnityAspNetExtension>());
                    break;
                case "windsor":
                    ServiceContainer.SetProvider(() => new WindsorServiceContainer().AddNewExtension<WindsorServiceContainerExtension>());
                    break;
            }
        }
    }
}