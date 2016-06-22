using System;
using Wheatech.Hosting;

namespace Wheatech.ServiceModel.Windsor
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the <see cref="IHostingEnvironment"/> interface.
    /// </summary>
    public static class HostingEnvironmentExtensions
    {
        /// <summary>
        /// Specify Castle Windsor as the service container to be used by the application.
        /// </summary>
        /// <param name="hostingEnvironment">An instance of <see cref="IHostingEnvironment"/>.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that is configuring for the application.</returns>
        public static IServiceContainer UseWindsor(this IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment == null) throw new ArgumentNullException(nameof(hostingEnvironment));
            ServiceContainer.SetProvider(() => new WindsorServiceContainer());
            return ServiceContainer.Current;
        }
    }
}
