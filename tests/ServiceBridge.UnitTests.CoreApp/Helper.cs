using System;
using Microsoft.AspNetCore.Http;
using ServiceBridge.Autofac;
using ServiceBridge.Ninject;
using ServiceBridge.StructureMap;

namespace ServiceBridge.UnitTests.CoreApp
{
    public static class Helper
    {
        public static void InitializeServiceContainer(string container, IServiceProvider requestServices)
        {
            switch (container)
            {
                case "autofac":
                    ServiceContainer.SetProvider(() => new AutofacServiceContainer());
                    break;
                case "ninject":
                    ServiceContainer.SetProvider(() => new NinjectServiceContainer());
                    break;
                case "structuremap":
                    ServiceContainer.SetProvider(() => new StructureMapServiceContainer());
                    break;
            }
            ServiceContainer.RegisterInstance<IHttpContextAccessor>(requestServices.GetService(typeof(IHttpContextAccessor)));
        }
    }
}