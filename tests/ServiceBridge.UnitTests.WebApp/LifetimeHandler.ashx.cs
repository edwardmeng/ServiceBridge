using System.Web;
using ServiceBridge.UnitTests.WebApp.Components;

namespace ServiceBridge.UnitTests.WebApp
{
    /// <summary>
    /// LifetimeHandler 的摘要说明
    /// </summary>
    public class LifetimeHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var container = context.Request.QueryString["container"];
            var value = context.Request.QueryString["value"];
            if (!string.IsNullOrEmpty(container))
            {
                Helper.InitializeServiceContainer(container);
                ServiceContainer.Current.Register<LifetimeTarget>(ServiceLifetime.PerRequest);
                context.Response.Write("Success");
                context.Response.End();
            }
            else if (!string.IsNullOrEmpty(value))
            {
                var target = ServiceContainer.GetInstance<LifetimeTarget>();
                target.Value = value;

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