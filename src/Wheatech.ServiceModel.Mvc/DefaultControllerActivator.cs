using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wheatech.ServiceModel.Mvc
{
    public class DefaultControllerActivator:IControllerActivator
    {
        public IController Create(RequestContext requestContext, Type controllerType)
        {
            return (IController) ServiceContainer.GetInstance(controllerType);
        }
    }
}
