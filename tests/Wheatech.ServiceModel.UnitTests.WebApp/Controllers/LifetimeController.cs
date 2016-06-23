using System.Web.Mvc;
using Wheatech.ServiceModel.UnitTests.WebApp.Components;

namespace Wheatech.ServiceModel.UnitTests.WebApp.Controllers
{
    public class LifetimeController : Controller
    {
        public ActionResult Initialize(string container)
        {
            Helper.InitializeServiceContainer(container);
            ServiceContainer.Current.Register<LifetimeTarget>(ServiceLifetime.PerRequest);
            return Json("Success",JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetValue(string value)
        {
            var target = ServiceContainer.GetInstance<LifetimeTarget>();
            target.Value = value;

            var target2 = ServiceContainer.GetInstance<LifetimeTarget>();
            return Json(target2?.Value, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetValue()
        {
            var target = ServiceContainer.GetInstance<LifetimeTarget>();
            return Json(target?.Value, JsonRequestBehavior.AllowGet);
        }
    }
}