using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ServiceBridge.AspNetCore
{
    /// <summary>
    /// A <see cref="IControllerActivator"/> that retrieves controllers as services from the <see cref="ServiceContainer"/>. 
    /// </summary>
    public class ServiceBridgeControllerActivator: IControllerActivator
    {
        /// <summary>
        /// Creates a controller.
        /// </summary>
        /// <param name="context">The <see cref="ControllerContext"/> for the executing action.</param>
        /// <returns>The resolved controller instance.</returns>
        public object Create(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return ServiceContainer.GetInstance(context.ActionDescriptor.ControllerTypeInfo.AsType());
        }

        /// <summary>
        /// Releases a controller. 
        /// </summary>
        /// <param name="context">The <see cref="ControllerContext"/> for the executing action.</param>
        /// <param name="controller">The controller to release.</param>
        public void Release(ControllerContext context, object controller)
        {
        }
    }
}
