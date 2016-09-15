using System.Web.Mvc;
using ServiceBridge.Sample.Components;

namespace ServiceBridge.Samples.Mvc.Controllers
{
    public class HomeController : Controller
    {
        [Injection]
        public ICacheRepository Repository { get; set; }

        public ActionResult Index()
        {
            return View(Repository.GetVale("Sample"));
        }
    }
}