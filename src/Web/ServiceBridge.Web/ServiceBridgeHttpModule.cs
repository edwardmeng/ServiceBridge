using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ServiceBridge.Web
{
    /// <summary>
    /// An <see cref="IHttpModule"/> that injects dependencies into pages and user controls.
    /// </summary>
    public class ServiceBridgeHttpModule : IHttpModule
    {
        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">A <see cref="HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            context.PreRequestHandlerExecute += OnPreRequestHandlerExecute;
        }

        private void OnPreRequestHandlerExecute(object sender, EventArgs e)
        {
            var page = ((HttpApplication)sender).Context.CurrentHandler as Page;
            if (page == null) return;
            ServiceContainer.InjectInstance(page);
            page.InitComplete += OnPageInitComplete;
        }

        private void OnPageInitComplete(object sender, EventArgs e)
        {
            InjectUserControls((Page)sender, true);
        }

        /// <summary>
        /// Search for usercontrols within the parent control
        /// and inject their dependencies using KernelContainer.
        /// </summary>
        /// <param name="parent">The parent control.</param>
        /// <param name="skipDataBoundControls">if set to <c>true</c> special handling of DataBoundControls is skipped.</param>
        private static void InjectUserControls(Control parent, bool skipDataBoundControls)
        {
            if (parent == null)
            {
                return;
            }

            if (skipDataBoundControls)
            {
                var dataBoundControl = parent as BaseDataBoundControl;
                if (dataBoundControl != null)
                {
                    dataBoundControl.DataBound += InjectDataBoundControl;
                    return;
                }
            }

            foreach (Control control in parent.Controls)
            {
                if (control is UserControl)
                {
                    ServiceContainer.InjectInstance(control);
                }

                InjectUserControls(control, skipDataBoundControls);
            }
        }

        /// <summary>
        /// Injects a data bound control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void InjectDataBoundControl(object sender, EventArgs e)
        {
            var dataBoundControl = sender as BaseDataBoundControl;
            if (dataBoundControl != null)
            {
                dataBoundControl.DataBound -= InjectDataBoundControl;
                InjectUserControls(dataBoundControl, false);
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
