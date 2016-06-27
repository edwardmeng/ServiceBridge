using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using Wheatech.ServiceModel.WebApi.Properties;

namespace Wheatech.ServiceModel.WebApi
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the <see cref="IServiceContainer"/> interface. 
    /// </summary>
    public static class ServiceContainerExtensions
    {
        /// <summary>
        /// Register an ASP.NET WebApi controller type with the <paramref name="container"/>. 
        /// </summary>
        /// <typeparam name="TController">The type of ASP.NET WebApi controller to be registered.</typeparam>
        /// <param name="container">Container to register with.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="container"/> is null.</exception>
        /// <exception cref="ArgumentException">The <typeparamref name="TController"/> is not a valid ASP.NET WebApi controller.</exception>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer RegisterApiController<TController>(this IServiceContainer container)
            where TController : class, IHttpController
        {
            return container.RegisterApiController(typeof(TController));
        }

        /// <summary>
        /// Register an ASP.NET WebApi controller type with the <paramref name="container"/>. 
        /// </summary>
        /// <param name="container">Container to register with.</param>
        /// <param name="controllerType">The type of ASP.NET WebApi controller to be registered.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="container"/> or <paramref name="controllerType"/> is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="controllerType"/> is not a valid ASP.NET WebApi controller.</exception>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer RegisterApiController(this IServiceContainer container, Type controllerType)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (controllerType == null)
            {
                throw new ArgumentNullException(nameof(controllerType));
            }
            if (!IsValidController(controllerType))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Invalid_ControllerType, controllerType), nameof(controllerType));
            }
            return container.RegisterApiControllerInternal(controllerType);
        }

        /// <summary>
        /// Register ASP.NET WebApi controller types in the <paramref name="assembly"/> with the <paramref name="container"/>. 
        /// </summary>
        /// <param name="container">Container to register with.</param>
        /// <param name="assembly">The assembly contains ASP.NET WebApi controllers.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="container"/> is null.</exception>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer RegisterApiControllers(this IServiceContainer container, Assembly assembly)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            IEnumerable<TypeInfo> types;
            try
            {
                types = assembly.DefinedTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.TakeWhile(type => type != null).Select(type => type.GetTypeInfo());
            }
            foreach (var controllerType in types)
            {
                if (IsValidController(controllerType))
                {
                    container.RegisterApiControllerInternal(controllerType);
                }
            }
            return container;
        }

        /// <summary>
        /// Register an ASP.NET WebApi controller type to the <paramref name="container"/> without validation.
        /// </summary>
        /// <param name="container">Container to register with.</param>
        /// <param name="controllerType">The type of ASP.NET WebApi controller to be registered.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        private static IServiceContainer RegisterApiControllerInternal(this IServiceContainer container, Type controllerType)
        {
            return container.Register(controllerType, null, ServiceLifetime.PerRequest);
        }

        /// <summary>
        /// Returns a value to indicate whethear the <paramref name="controllerType"/> is an ASP.NET WebApi controller type.
        /// </summary>
        /// <param name="controllerType">The ASP.NET WebApi controller type to be validated.</param>
        /// <returns><c>true</c> if the <paramref name="controllerType"/> is an ASP.NET WebApi controller type; otherwise, <c>false</c>.</returns>
        private static bool IsValidController(Type controllerType)
        {
            return !controllerType.IsInterface && !controllerType.IsAbstract && controllerType.IsPublic && controllerType.IsClass && typeof(IHttpController).IsAssignableFrom(controllerType);
        }
    }
}
