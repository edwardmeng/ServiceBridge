using System;
using MassActivation;

namespace ServiceBridge.StructureMap.Activation
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the <see cref="IActivatingEnvironment"/> interface.
    /// </summary>
    public static class ActivatingEnvironmentExtensions
    {
        /// <summary>
        /// Specify StructureMap as the service container to be used by the application.
        /// </summary>
        /// <param name="hostingEnvironment">An instance of <see cref="IActivatingEnvironment"/>.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that is configuring for the application.</returns>
        public static IServiceContainer UseStructureMap(this IActivatingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment == null) throw new ArgumentNullException(nameof(hostingEnvironment));
            ServiceContainer.SetProvider(() => new StructureMapServiceContainer());
            return ServiceContainer.Current;
        }
    }
}
