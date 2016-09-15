using System.Web;

[assembly: PreApplicationStartMethod(typeof(ServiceBridge.Web.PreApplicationStartup), "Configuration")]

namespace ServiceBridge.Web
{
    public static class PreApplicationStartup
    {
        public static void Configuration()
        {
            HttpApplication.RegisterModule(typeof(ServiceContainerHttpModule));
        }
    }
}
