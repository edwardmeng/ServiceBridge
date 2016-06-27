using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Wheatech.ServiceModel.Wcf.Properties;

namespace Wheatech.ServiceModel.Wcf
{
    /// <summary>
    /// Dynamic generator for a Windows Communication Foundation service client proxies.
    /// </summary>
    public static class ServiceClientFactory
    {
        private static readonly AssemblyBuilder _assemblyBuilder;
        private static readonly ConcurrentDictionary<Type, Type> _serviceTypes = new ConcurrentDictionary<Type, Type>();

        static ServiceClientFactory()
        {
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ILEmit_ServiceClients"), AssemblyBuilderAccess.Run);
        }

        /// <summary>
        /// Create a new service client instance of the <typeparamref name="TContract"/> using the default target endpoint from the application configuration file. 
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">Either there is no default endpoint information in the configuration file, more than one endpoint in the file, or no configuration file.</exception>
        public static TContract GetService<TContract>()
        {
            var serviceType = GetServiceType(typeof(TContract));
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType);
        }

        /// <summary>
        /// Create a new service client instance of the <typeparamref name="TContract"/> using the configuration information specified in the application configuration file by <paramref name="endpointConfigurationName"/>. 
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        /// <exception cref="T:System.ArgumentNullException">The specified endpoint information is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public static TContract GetService<TContract>(string endpointConfigurationName)
        {
            var serviceType = GetServiceType(typeof(TContract));
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType, endpointConfigurationName);
        }

        /// <summary>
        /// Create a new service client instance of the <typeparamref name="TContract"/>.
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        /// <param name="remoteAddress">The address of the service.</param>
        /// <exception cref="T:System.ArgumentNullException">The endpoint is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">The remote address is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public static TContract GetService<TContract>(string endpointConfigurationName, string remoteAddress)
        {
            var serviceType = GetServiceType(typeof(TContract));
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType, endpointConfigurationName, remoteAddress);
        }

        /// <summary>
        /// Create a new service client instance of the <typeparamref name="TContract"/> using the specified target address and endpoint information. 
        /// </summary>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        /// <param name="remoteAddress">The address of the service.</param>
        /// <exception cref="T:System.ArgumentNullException">The endpoint is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">The remote address is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public static TContract GetService<TContract>(string endpointConfigurationName, EndpointAddress remoteAddress)
        {
            var serviceType = GetServiceType(typeof(TContract));
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType, endpointConfigurationName, remoteAddress);
        }

        /// <summary>
        /// Create a new service client instance of the <typeparamref name="TContract"/> using the specified binding and target address. 
        /// </summary>
        /// <param name="binding">The binding with which to make calls to the service.</param>
        /// <param name="remoteAddress">The address of the service endpoint.</param>
        /// <exception cref="T:System.ArgumentNullException">The binding is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">The remote address is null.</exception>
        public static TContract GetService<TContract>(Binding binding, EndpointAddress remoteAddress)
        {
            var serviceType = GetServiceType(typeof(TContract));
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType, binding, remoteAddress);
        }

        /// <summary>
        /// Create a new service client instance of the <typeparamref name="TContract"/> using the specified  <see cref="T:System.ServiceModel.Description.ServiceEndpoint"/>.
        /// </summary>
        /// <param name="endpoint">The endpoint for a service that allows clients to find and communicate with the service.</param>
        public static TContract GetService<TContract>(ServiceEndpoint endpoint)
        {
            var serviceType = GetServiceType(typeof(TContract));
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType, endpoint);
        }

        /// <summary>
        /// Create a new service client instance of the <typeparamref name="TContract"/> using the <paramref name="callbackInstance"/> as the callback object in a duplex conversation. 
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client application uses to listen for messages from the connected service.</param>
        /// <exception cref="T:System.ArgumentNullException">The callback instance is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">Either there is no default endpoint information in the configuration file, more than one endpoint in the file, or no configuration file.</exception>
        public static TContract GetService<TContract>(InstanceContext callbackInstance)
        {
            var serviceType = GetServiceType(typeof(TContract));
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType, callbackInstance);
        }

        /// <summary>
        /// Create a new service client instance of the <typeparamref name="TContract"/> using the specified callback service and endpoint configuration information. 
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client uses to listen for messages from the connected service.</param>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file. </param>
        /// <exception cref="T:System.ArgumentNullException">The callback instance is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">The endpoint is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public static TContract GetService<TContract>(InstanceContext callbackInstance, string endpointConfigurationName)
        {
            var serviceType = GetServiceType(typeof(TContract));
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType, callbackInstance, endpointConfigurationName);
        }

        /// <summary>
        /// Create a new service client instance of the <typeparamref name="TContract"/>.
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client uses to listen for messages from the connected service.</param>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        /// <param name="remoteAddress">The address of the service.</param>
        /// <exception cref="T:System.ArgumentNullException">The callback instance is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">The endpoint is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">The remote address is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public static TContract GetService<TContract>(InstanceContext callbackInstance, string endpointConfigurationName,
                                                      string remoteAddress)
        {
            var serviceType = GetServiceType(typeof(TContract));
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType, callbackInstance, endpointConfigurationName, remoteAddress);
        }

        /// <summary>
        /// Create a new service client instance of the <typeparamref name="TContract"/>.
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client uses to listen for messages from the connected service.</param>
        /// <param name="endpointConfigurationName">The name of the endpoint in the application configuration file.</param>
        /// <param name="remoteAddress">The address of the service.</param>
        /// <exception cref="T:System.ArgumentNullException">The callback instance is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">The endpoint is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">The remote address is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The endpoint cannot be found or the endpoint contract is not valid.</exception>
        public static TContract GetService<TContract>(InstanceContext callbackInstance, string endpointConfigurationName,
                                                      EndpointAddress remoteAddress)
        {
            var serviceType = GetServiceType(typeof(TContract));
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType, callbackInstance, endpointConfigurationName, remoteAddress);
        }

        /// <summary>
        /// Create a new service client instance of the <typeparamref name="TContract"/>. 
        /// </summary>
        /// <param name="callbackInstance">The callback service.</param>
        /// <param name="binding">The binding with which to call the service.</param>
        /// <param name="remoteAddress">The address of the service endpoint.</param>
        /// <exception cref="T:System.ArgumentNullException">The callback instance is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">The binding is null.</exception>
        /// <exception cref="T:System.ArgumentNullException">The remote address is null.</exception>
        public static TContract GetService<TContract>(InstanceContext callbackInstance, Binding binding,
                                                      EndpointAddress remoteAddress)
        {
            var serviceType = GetServiceType(typeof(TContract));
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType, callbackInstance, binding, remoteAddress);
        }

        /// <summary>
        /// Create a new service client instance of the <typeparamref name="TContract"/> using the specified  <see cref="T:System.ServiceModel.InstanceContext"/> and  <see cref="T:System.ServiceModel.Description.ServiceEndpoint"/> objects.
        /// </summary>
        /// <param name="callbackInstance">The callback object that the client application uses to listen for messages from the connected service.</param>
        /// <param name="endpoint">The endpoint for a service that allows clients to find and communicate with the service.</param>
        public static TContract GetService<TContract>(InstanceContext callbackInstance, ServiceEndpoint endpoint)
        {
            var serviceType = GetServiceType(typeof(TContract));
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType, callbackInstance, endpoint);
        }

        private static Type GetServiceType(Type contractType)
        {
            if (contractType.IsGenericTypeDefinition)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.Contract_Cannot_GenericType, contractType.FullName));
            }
            return _serviceTypes.GetOrAdd(contractType, CreateServiceType);
        }

        private static Type CreateServiceType(Type contractType)
        {
            string typeName = "DynamicModule.ns.Wrapped_" + contractType.Name + "_" + Guid.NewGuid().ToString("N");
            var builder = GetModuleBuilder().DefineType(typeName, TypeAttributes.Public, typeof(ClientBase<>).MakeGenericType(contractType), new[] { contractType });
            GenerateConstructors(builder, contractType);
            GenerateMethods(builder, contractType);
            return builder.CreateType();
        }

        private static void GenerateConstructors(TypeBuilder builder, Type contractType)
        {
            // ctor(InstanceContext callbackInstance, ServiceEndpoint endpoint)
            GenerateConstructor(builder, contractType, new[] { typeof(InstanceContext), typeof(ServiceEndpoint) });
            // ctor(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
            GenerateConstructor(builder, contractType, new[] { typeof(InstanceContext), typeof(Binding), typeof(EndpointAddress) });
            // ctor(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress)
            GenerateConstructor(builder, contractType, new[] { typeof(InstanceContext), typeof(string), typeof(EndpointAddress) });
            // ctor(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress)
            GenerateConstructor(builder, contractType, new[] { typeof(InstanceContext), typeof(string), typeof(string) });
            // ctor(InstanceContext callbackInstance, string endpointConfigurationName)
            GenerateConstructor(builder, contractType, new[] { typeof(InstanceContext), typeof(string) });
            // ctor(InstanceContext callbackInstance)
            GenerateConstructor(builder, contractType, new[] { typeof(InstanceContext) });
            // ctor(ServiceEndpoint endpoint)
            GenerateConstructor(builder, contractType, new[] { typeof(ServiceEndpoint) });
            // ctor(Binding binding, EndpointAddress remoteAddress)
            GenerateConstructor(builder, contractType, new[] { typeof(Binding), typeof(EndpointAddress) });
            // ctor(string endpointConfigurationName, EndpointAddress remoteAddress)
            GenerateConstructor(builder, contractType, new[] { typeof(string), typeof(EndpointAddress) });
            // ctor(string endpointConfigurationName, string remoteAddress)
            GenerateConstructor(builder, contractType, new[] { typeof(string), typeof(string) });
            // ctor(string endpointConfigurationName)
            GenerateConstructor(builder, contractType, new[] { typeof(string) });
            // ctor()
            GenerateConstructor(builder, contractType, Type.EmptyTypes);
        }

        private static void GenerateConstructor(TypeBuilder builder, Type contractType, Type[] constructorParameters)
        {
            // Define the constructor
            var constructorBuilder =
                builder.DefineConstructor(MethodAttributes.Public | MethodAttributes.RTSpecialName,
                                          CallingConventions.Standard, constructorParameters);
            var il = constructorBuilder.GetILGenerator();
            var originalConstructor =
                typeof(ClientBase<>).MakeGenericType(contractType).GetConstructor(constructorParameters);
            if (originalConstructor != null)
            {
                il.Emit(OpCodes.Ldarg_0);
                for (int i = 0; i < constructorParameters.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            il.Emit(OpCodes.Ldarg_1);
                            break;
                        case 1:
                            il.Emit(OpCodes.Ldarg_2);
                            break;
                        case 2:
                            il.Emit(OpCodes.Ldarg_3);
                            break;
                        default:
                            il.Emit(OpCodes.Ldarg, i + 1);
                            break;
                    }
                }

                // Call the base constructor
                il.Call(originalConstructor);
            }
            il.Emit(OpCodes.Ret);
        }

        private static void GenerateMethods(TypeBuilder builder, Type contractType)
        {
            var baseClassType = typeof(ClientBase<>).MakeGenericType(contractType);
            foreach (var method in contractType.GetMethods())
            {
                var parameterTypes = method.GetParameters().Select(param => param.ParameterType).ToArray(); // declare the method with the correct parameters
                var methodBuilder = builder.DefineMethod(method.DeclaringType.Name + "." + method.Name,
                                                         MethodAttributes.VtableLayoutMask | MethodAttributes.HideBySig |
                                                         MethodAttributes.Virtual | MethodAttributes.Final |
                                                         MethodAttributes.Private, method.ReturnType, parameterTypes);

                // declare that we override the interface method
                builder.DefineMethodOverride(methodBuilder, method);

                // Start building the method
                //methodBuilder.CreateMethodBody(null, 0);
                var il = methodBuilder.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0); // this

                // Get the Channel Property of the ClientBase
                var channelProperty = baseClassType.GetProperty("Channel", BindingFlags.Instance | BindingFlags.Public);
                // Get the channel: "base.Channel<TInterface>."
                il.Call(channelProperty.GetGetMethod());

                // Prepare the parameters for the call
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            il.Emit(OpCodes.Ldarg_1);
                            break;
                        case 1:
                            il.Emit(OpCodes.Ldarg_2);
                            break;
                        case 2:
                            il.Emit(OpCodes.Ldarg_3);
                            break;
                        default:
                            il.Emit(OpCodes.Ldarg, i + 1);
                            break;
                    }
                }

                // Call the Channel via the interface
                il.Call(method);
                il.Emit(OpCodes.Ret);

            }

            Type[] inheritedInterfaces = contractType.GetInterfaces();
            foreach (Type inheritedInterface in inheritedInterfaces)
            {
                GenerateMethods(builder, inheritedInterface);
            }
        }

        private static ModuleBuilder GetModuleBuilder()
        {
            string name = Guid.NewGuid().ToString("N");
            return _assemblyBuilder.DefineDynamicModule(name);
        }

        /// <summary>
        /// Calls the method indicated by the passed method descriptor.
        /// </summary>
        private static void Call(this ILGenerator il, MethodBase methodInfo)
        {
            if (methodInfo.IsFinal || !methodInfo.IsVirtual)
            {
                if (methodInfo.MemberType == MemberTypes.Constructor)
                {
                    il.Emit(OpCodes.Call, (ConstructorInfo)methodInfo);
                }
                else
                {
                    il.Emit(OpCodes.Call, (MethodInfo)methodInfo);
                }
            }
            else
            {
                if (methodInfo.MemberType == MemberTypes.Constructor)
                {
                    il.Emit(OpCodes.Callvirt, (ConstructorInfo)methodInfo);
                }
                else
                {
                    il.Emit(OpCodes.Callvirt, (MethodInfo)methodInfo);
                }
            }
        }
    }
}
