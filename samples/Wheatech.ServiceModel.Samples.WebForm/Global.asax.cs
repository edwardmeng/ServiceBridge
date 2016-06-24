using System;
using Wheatech.Activation;
using Wheatech.ServiceModel.Sample.Components;

namespace Wheatech.ServiceModel.Samples.WebForm
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            ApplicationActivator.UseEnvironment(EnvironmentName.Development).Startup();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            ServiceContainer.GetInstance<ICacheRepository>().SetVale("Sample", "Wheatech");
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