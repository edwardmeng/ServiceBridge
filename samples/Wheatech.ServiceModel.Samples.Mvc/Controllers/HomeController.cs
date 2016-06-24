using System.Web.Mvc;
using Wheatech.ServiceModel.Sample.Components;

namespace Wheatech.ServiceModel.Samples.Mvc.Controllers
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