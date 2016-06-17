﻿using System;
using System.Collections.Generic;
using Wheatech.ServiceModel.Properties;

namespace Wheatech.ServiceModel
{
    /// <summary>
    /// This class provides the ambient container for this application. If your
    /// framework defines such an ambient container, use <see cref="Current"/>
    /// to get it.
    /// </summary>
    public static class ServiceContainer
    {
        private static Func<IServiceContainer> _currentProvider;

        /// <summary>
        /// The current ambient container.
        /// </summary>
        public static IServiceContainer Current
        {
            get
            {
                if (!HasProvider) throw new InvalidOperationException(Resources.ProviderNotSetMessage);
                return _currentProvider();
            }
        }

        /// <summary>
        /// Set the delegate that is used to retrieve the current container.
        /// </summary>
        /// <param name="newProvider">Delegate that, when called, will return
        /// the current ambient container.</param>
        public static void SetProvider(Func<IServiceContainer> newProvider)
        {
            _currentProvider = newProvider;
        }

        /// <summary>
        /// Returns a value indicates whether the provider for the service container has been specified.
        /// </summary>
        public static bool HasProvider => _currentProvider != null;

        /// <summary>
        /// Get an instance of the given named <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <param name="serviceName">Name the object was registered with.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public static object GetInstance(Type serviceType, string serviceName = null)
        {
            return Current.GetInstance(serviceType, serviceName);
        }

        /// <summary>
        /// Get an instance of the given named <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <param name="serviceName">Name the object was registered with.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>The requested service instance.</returns>
        public static TService GetInstance<TService>(string serviceName = null)
        {
            return Current.GetInstance<TService>(serviceName);
        }

        /// <summary>
        /// Get all instances of the given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">Type of object requested.</param>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>A sequence of instances of the requested <paramref name="serviceType"/>.</returns>
        public static IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Current.GetAllInstances(serviceType);
        }

        /// <summary>
        /// Get all instances of the given <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">Type of object requested.</typeparam>
        /// <exception cref="ActivationException">If there are errors resolving the service instance.</exception>
        /// <returns>A sequence of instances of the requested <typeparamref name="TService"/>.</returns>
        public static IEnumerable<TService> GetAllInstances<TService>()
        {
            return Current.GetAllInstances<TService>();
        }

        /// <summary>
        /// Registers a type mapping. 
        /// </summary>
        /// <param name="serviceType"><see cref="Type"/> that will be requested.</param>
        /// <param name="implementationType"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="serviceName">Name to use for registration, null if a default registration.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        public static IServiceContainer Register(Type serviceType, Type implementationType, string serviceName = null)
        {
            return Current.Register(serviceType, implementationType, serviceName);
        }

        /// <summary>
        /// Register a type mapping. 
        /// </summary>
        /// <typeparam name="TService"><see cref="Type"/> that will be requested.</typeparam>
        /// <typeparam name="TImplementation"><see cref="Type"/> that will actually be returned.</typeparam>
        /// <param name="serviceName">Name of this mapping.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer Register<TService, TImplementation>(string serviceName = null)
            where TImplementation : TService
        {
            return Current.Register<TService, TImplementation>(serviceName);
        }

        /// <summary>
        /// Register a given type. 
        /// </summary>
        /// <typeparam name="T">The type to be registered.</typeparam>
        /// <param name="serviceName">Name of this registration.</param>
        /// <returns>The <see cref="IServiceContainer"/> object that this method was called on.</returns>
        /// <exception cref="RegistrationException">If there are errors registering the type mapping.</exception>
        public static IServiceContainer Register<T>(string serviceName = null)
        {
            return Current.Register<T>(serviceName);
        }

        /// <summary>
        /// Check if a particular type/name pair has been registered. 
        /// </summary>
        /// <param name="serviceType">Type to check registration for.</param>
        /// <param name="serviceName">Name to check registration for.</param>
        /// <returns><c>true</c> if this type/name pair has been registered, <c>false</c> if not.</returns>
        public static bool IsRegistered(Type serviceType, string serviceName = null)
        {
            return Current.IsRegistered(serviceType, serviceName);
        }

        /// <summary>
        /// Check if a particular type/name pair has been registered. 
        /// </summary>
        /// <typeparam name="T">Type to check registration for.</typeparam>
        /// <param name="serviceName">Name to check registration for.</param>
        /// <returns><c>true</c> if this type/name pair has been registered, <c>false</c> if not.</returns>
        public static bool IsRegistered<T>(string serviceName = null)
        {
            return Current.IsRegistered<T>(serviceName);
        }
    }
}