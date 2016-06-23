using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wheatech.ServiceModel.Autofac;
using Wheatech.ServiceModel.Autofac.Interception;
using Wheatech.ServiceModel.Ninject;
using Wheatech.ServiceModel.Ninject.Interception;
using Wheatech.ServiceModel.StructureMap;
using Wheatech.ServiceModel.StructureMap.Interception;
using Wheatech.ServiceModel.Unity;
using Wheatech.ServiceModel.Unity.Interception;
using Wheatech.ServiceModel.Windsor;
using Wheatech.ServiceModel.Windsor.Interception;

namespace Wheatech.ServiceModel.UnitTests.WebApp
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