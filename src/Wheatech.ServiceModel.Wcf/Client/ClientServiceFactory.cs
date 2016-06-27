using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.ServiceModel;

namespace Wheatech.ServiceModel.Wcf
{
    /// <summary>
    /// Dynamic generator for a Windows Communication Foundation client service.
    /// </summary>
    public static class ClientServiceFactory
    {
        private static readonly AssemblyBuilder _assemblyBuilder;
        private static readonly ConcurrentDictionary<ClientServiceKey, Type> _serviceTypes = new ConcurrentDictionary<ClientServiceKey, Type>();

        static ClientServiceFactory()
        {
            _assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ILEmit_ClientServices"),
                                                              AssemblyBuilderAccess.Run);
        }

        /// <summary>
        /// Create a new client service instance of the <typeparamref name="TContract"/> using the default target endpoint from the application configuration file. 
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">Either there is no default endpoint information in the configuration file, more than one endpoint in the file, or no configuration file.</exception>
        public static TContract GetService<TContract>(string configurationName)
        {
            var serviceType = GetServiceType(typeof(TContract), configurationName);
            if (serviceType == null) return default(TContract);
            return (TContract)Activator.CreateInstance(serviceType);
        }

        internal static Type GetServiceType(Type contractType, string configurationName)
        {
            if (!contractType.IsInterface)
            {
                throw new InvalidOperationException(string.Format("The service contract type {0} must be an interface.",
                                                                  contractType.FullName));
            }
            if (contractType.IsGenericTypeDefinition)
            {
                throw new InvalidOperationException(
                    string.Format("The generic definition type {0} cannot be instantiated.", contractType.FullName));
            }

            if (!contractType.IsDefined(typeof(ServiceContractAttribute), true))
            {
                throw new InvalidOperationException(
                    string.Format("The type {0} is not service contract.", contractType.FullName));
            }
            return _serviceTypes.GetOrAdd(new ClientServiceKey(contractType, configurationName), key => CreateServiceType(key.ContractType, key.ConfigurationName));
        }

        private static Type CreateServiceType(Type contractType, string configurationName)
        {
            string typeName = "DynamicModule.ns.Wrapped_" + contractType.Name + "_" + Guid.NewGuid().ToString("N");
            TypeBuilder builder = GetModuleBuilder()
                .DefineType(typeName, TypeAttributes.Public, null, new[] { contractType });
            GenerateConstructor(builder);
            GenerateMethods(builder, contractType, configurationName);
            return builder.CreateType();
        }

        private static void GenerateConstructor(TypeBuilder builder)
        {
            // Define the constructor
            var constructorBuilder =
                builder.DefineConstructor(MethodAttributes.Public | MethodAttributes.RTSpecialName,
                                          CallingConventions.Standard, Type.EmptyTypes);
            var il = constructorBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Call(typeof(object).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Ret);
        }

        private static void GenerateMethods(TypeBuilder builder, Type contractType, string configurationName)
        {
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
                // Declare local variables to keep the wcf service client.
                var clientLocal = il.DeclareLocal(contractType, true);
                // Declare local variables to keep the wcf service client as IDisposable.
                var disposableLocal = il.DeclareLocal(typeof(IDisposable), true);
                var hasReturnValue = method.ReturnType != typeof(void);
                LocalBuilder returnValueLocal = null;
                if (hasReturnValue)
                {
                    // Declare local variables to keep the return value of the wcf service invocation.
                    returnValueLocal = il.DeclareLocal(method.ReturnType, true);
                }
                var endFinallyLabel = il.DefineLabel();
                // TContract client = null;
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Stloc, clientLocal);

                // var client = ServiceClientFactory.GetService<TContract>();
                il.BeginExceptionBlock();
                il.Emit(OpCodes.Ldstr, configurationName);
                il.Call(typeof(ServiceClientFactory).GetMethod("GetService", BindingFlags.Public | BindingFlags.Static,
                                                                null, new[] { typeof(string) }, null).MakeGenericMethod(contractType));
                il.Emit(OpCodes.Stloc, clientLocal);

                il.Emit(OpCodes.Ldloc, clientLocal);
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
                il.Call(method);
                if (hasReturnValue)
                {
                    il.Emit(OpCodes.Stloc, returnValueLocal);
                }

                il.BeginFinallyBlock();
                il.Emit(OpCodes.Ldloc, clientLocal);
                il.Emit(OpCodes.Isinst, typeof(IDisposable));
                il.Emit(OpCodes.Stloc, disposableLocal);

                il.Emit(OpCodes.Ldloc, disposableLocal);
                il.Emit(OpCodes.Brfalse, endFinallyLabel);

                il.Emit(OpCodes.Ldloc, disposableLocal);
                il.Call(typeof(IDisposable).GetMethod("Dispose"));

                il.MarkLabel(endFinallyLabel);
                il.EndExceptionBlock();
                if (hasReturnValue)
                {
                    il.Emit(OpCodes.Ldloc, returnValueLocal);
                }
                il.Emit(OpCodes.Ret);
            }

            Type[] inheritedInterfaces = contractType.GetInterfaces();
            foreach (Type inheritedInterface in inheritedInterfaces)
            {
                GenerateMethods(builder, inheritedInterface, configurationName);
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
