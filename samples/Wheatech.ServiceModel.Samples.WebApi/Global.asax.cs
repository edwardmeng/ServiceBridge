using System;
using System.Web.Http;
using System.Web.Routing;
using Wheatech.Activation;
using Wheatech.ServiceModel.Sample.Components;

namespace Wheatech.ServiceModel.Samples.WebApi
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            ApplicationActivator.UseEnvironment(EnvironmentName.Development).Startup();
            RegisterRoutes(RouteTable.Routes);
            ServiceContainer.GetInstance<ICacheRepository>().SetVale("Sample", "Wheatech");
        }

        private void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}