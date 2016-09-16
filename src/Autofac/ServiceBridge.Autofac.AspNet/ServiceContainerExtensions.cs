using System;

namespace ServiceBridge.Autofac.AspNet
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the <see cref="IServiceContainer"/> interface.
    /// </summary>
    public static class ServiceContainerExtensions
    {
        /// <summary>
        /// Specifies the service container to be enabled by using Autofac interception mechanism.
        /// </summary>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        public static void EnableAspNet(this IServiceContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            container.AddNewExtension<AspNetExtension>();
        }
    }
}
