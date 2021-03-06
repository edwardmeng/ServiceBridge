﻿using System;
using MassActivation;

namespace ServiceBridge.Unity.Activation
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the <see cref="IActivatingEnvironment"/> interface.
    /// </summary>
    public static class ActivatingEnvironmentExtensions
    {
        /// <summary>
        /// Specify Unity as the service container to be used by the application.
        /// </summary>
        /// <param name="hostingEnvironment">An instance of <see cref="IActivatingEnvironment"/>.</param>
        /// <returns>The <see cref="IActivatingEnvironment"/>.</returns>
        public static IActivatingEnvironment UseUnity(this IActivatingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment == null) throw new ArgumentNullException(nameof(hostingEnvironment));
            ServiceContainer.SetProvider(() => new UnityServiceContainer());
            return hostingEnvironment;
        }
    }
}
