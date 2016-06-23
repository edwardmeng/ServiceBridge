using System.Web;
using Wheatech.ServiceModel.Autofac;
using Wheatech.ServiceModel.Autofac.Interception;
using Wheatech.ServiceModel.Ninject;
using Wheatech.ServiceModel.Ninject.Interception;
using Wheatech.ServiceModel.StructureMap;
using Wheatech.ServiceModel.StructureMap.Interception;
using Wheatech.ServiceModel.UnitTests.WebApp.Components;
using Wheatech.ServiceModel.Unity;
using Wheatech.ServiceModel.Unity.Interception;
using Wheatech.ServiceModel.Windsor;
using Wheatech.ServiceModel.Windsor.Interception;

namespace Wheatech.ServiceModel.UnitTests.WebApp
{
    /// <summary>
    /// LifetimeHandler 的摘要说明
    /// </summary>
    public class LifetimeHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.QueryString["action"] == "initialize")
            {
                switch (context.Request.QueryString["container"])
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
                ServiceContainer.Current.Register<LifetimeTarget>();
                context.Response.Write("Success");
                context.Response.End();
            }
            else if (context.Request.QueryString["action"] == "one")
            {
                var target = ServiceContainer.GetInstance<LifetimeTarget>();
                target.Value = context.Request.QueryString["value"];

                var target2 = ServiceContainer.GetInstance<LifetimeTarget>();
                context.Response.Write(target2?.Value);
            }
            else
            {
                var target = ServiceContainer.GetInstance<LifetimeTarget>();
                context.Response.Write(target?.Value);
            }
        }

        public bool IsReusable => false;
    }
}