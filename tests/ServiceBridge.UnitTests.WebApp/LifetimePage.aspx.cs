using System;
using ServiceBridge.UnitTests.WebApp.Components;

namespace ServiceBridge.UnitTests.WebApp
{
    public partial class LifetimePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var container = Request.QueryString["container"];
            var value = Request.QueryString["value"];
            if (!string.IsNullOrEmpty(container))
            {
                Helper.InitializeServiceContainer(container);
                ServiceContainer.Current.Register<LifetimeTarget>(ServiceLifetime.PerRequest);
                Response.Write("Success");
                Response.End();
            }
            else if (!string.IsNullOrEmpty(value))
            {
                var target = ServiceContainer.GetInstance<LifetimeTarget>();
                target.Value = value;

                var target2 = ServiceContainer.GetInstance<LifetimeTarget>();
                Response.Write(target2?.Value);
            }
            else
            {
                var target = ServiceContainer.GetInstance<LifetimeTarget>();
                Response.Write(target?.Value);
            }
            Response.Flush();
            Response.End();
        }
    }
}