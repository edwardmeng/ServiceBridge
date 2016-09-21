using System;
using MassActivation;
using ServiceBridge.Sample.Components;

namespace ServiceBridge.Samples.WebForm
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            ApplicationActivator.UseEnvironment(EnvironmentName.Development).Startup();
            ServiceContainer.GetInstance<ICacheRepository>().SetVale("Sample", "ServiceBridge");
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