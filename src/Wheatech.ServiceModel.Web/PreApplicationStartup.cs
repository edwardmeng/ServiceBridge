using System.Web;

[assembly: PreApplicationStartMethod(typeof(Wheatech.ServiceModel.Web.PreApplicationStartup), "Configuration")]

namespace Wheatech.ServiceModel.Web
{
    public static class PreApplicationStartup
    {
        public static void Configuration()
        {
            HttpApplication.RegisterModule(typeof(ServiceContainerHttpModule));
        }
    }
}
