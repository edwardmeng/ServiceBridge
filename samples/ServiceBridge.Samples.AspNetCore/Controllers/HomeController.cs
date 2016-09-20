using Microsoft.AspNetCore.Mvc;

namespace ServiceBridge.Samples.AspNetCore
{
    public class HomeController : Controller
    {
        [Injection]
        public ICacheRepository Repository { get; set; }

        public IActionResult Index()
        {
            return View(Repository.GetVale("Sample"));
        }
    }
}
