using Microsoft.AspNetCore.Mvc;
using ServiceBridge.UnitTests.CoreApp.Components;

namespace ServiceBridge.UnitTests.CoreApp.Controllers
{
    public class LifetimeController : Controller
    {
        public ActionResult Initialize(string container)
        {
            Helper.InitializeServiceContainer(container, HttpContext.RequestServices);
            ServiceContainer.Current.Register<LifetimeTarget>(ServiceLifetime.PerRequest);
            return Json("Success");
        }

        public ActionResult SetValue(string value)
        {
            var target = ServiceContainer.GetInstance<LifetimeTarget>();
            target.Value = value;

            var target2 = ServiceContainer.GetInstance<LifetimeTarget>();
            return Json(target2?.Value);
        }

        public ActionResult GetValue()
        {
            var target = ServiceContainer.GetInstance<LifetimeTarget>();
            return Json(target?.Value);
        }
    }
}
