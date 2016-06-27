using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Wheatech.Activation;

[assembly: AssemblyActivator(typeof(Wheatech.ServiceModel.Wcf.ServiceModelActivator))]

namespace Wheatech.ServiceModel.Wcf
{
    internal class ServiceModelActivator
    {
        public void Configuration(IActivatingEnvironment environment, IServiceContainer container)
        {
            // We have to register the services at the application configuration stage.
            // Since there are some IoC implementations cannot register types after resolve instances.
            var types = new List<TypeInfo>();
            foreach (var assembly in environment.GetAssemblies())
            {
                if (!assembly.IsDynamic)
                {
                    IEnumerable<TypeInfo> assemblyTypes;
                    try
                    {
                        assemblyTypes = assembly.DefinedTypes;
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        assemblyTypes = ex.Types.TakeWhile(type => type != null).Select(type => type.GetTypeInfo());
                    }
                    types.AddRange(assemblyTypes);
                }
            }
            ConfigureServices(container, types.ToArray());
            ConfigureClients(container, types.ToArray());
        }

        /// <summary>
        /// Returns the index of the comma separating the type from the assembly, or
        /// -1 of there is no assembly
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private static int CommaIndexInTypeName(string typeName)
        {

            // Look for the last comma
            int commaIndex = typeName.LastIndexOf(',');

            // If it doesn't have one, there is no assembly
            if (commaIndex < 0)
                return -1;

            // It has a comma, we need to account for the generics syntax.
            // E.g. it could be "SomeType[int,string]

            // Check for a ]
            int rightBracketIndex = typeName.LastIndexOf(']');

            // If it has one, and it's after the last comma, there is no assembly
            if (rightBracketIndex > commaIndex)
                return -1;

            // The comma that we want is the first one after the last ']'
            commaIndex = typeName.IndexOf(',', rightBracketIndex + 1);

            // There is an assembly
            return commaIndex;
        }

        private Type GetTypeFromName(string typeName, TypeInfo[] types)
        {
            var index = CommaIndexInTypeName(typeName);
            return index > 0 ? Type.GetType(typeName, false, false) : types.FirstOrDefault(type => type.FullName == typeName);
        }

        private IEnumerable<Type> GetServiceContracts(Type serviceType)
        {
            foreach (var contractType in serviceType.GetInterfaces())
            {
                if (contractType.IsDefined(typeof(ServiceContractAttribute)))
                {
                    yield return contractType;
                }
            }
            var contractClass = serviceType;
            while (contractClass != null && contractClass != typeof(object))
            {
                if (contractClass.IsDefined(typeof(ServiceContractAttribute)))
                {
                    yield return contractClass;
                }
                contractClass = contractClass.BaseType;
            }
        }

        private void ConfigureServices(IServiceContainer container, TypeInfo[] types)
        {
            foreach (var serviceType in types)
            {
                if (!serviceType.IsInterface && !serviceType.IsAbstract && !serviceType.IsGenericTypeDefinition && serviceType.IsClass &&
                    serviceType.IsPublic && serviceType.Assembly != typeof(ServiceContractAttribute).Assembly)
                {
                    var serviceName = ServiceUtils.GetServiceName(serviceType);
                    var lifetime = ServiceUtils.GetServiceLifetime(serviceType);
                    foreach (var contractType in GetServiceContracts(serviceType))
                    {
                        container.Register(contractType, serviceType, serviceName, lifetime);
                    }
                }
            }
        }

        private void ConfigureClients(IServiceContainer container, TypeInfo[] types)
        {
            var section = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");
            var contractTypes = new Dictionary<string, Type>();
            foreach (var contractType in types)
            {
                var contractAttribute = contractType.GetCustomAttribute<ServiceContractAttribute>(false);
                if (!string.IsNullOrEmpty(contractAttribute?.ConfigurationName))
                {
                    contractTypes.Add(contractAttribute.ConfigurationName, contractType);
                }
            }
            var contractEndPoints = new Dictionary<Type, List<string>>();
            for (int i = 0; i < section.Endpoints.Count; i++)
            {
                var endpoint = section.Endpoints[i];
                Type contractType;
                if (!contractTypes.TryGetValue(endpoint.Contract, out contractType))
                {
                    contractType = GetTypeFromName(endpoint.Contract, types);
                }
                if (contractType != null)
                {
                    List<string> endPoints;
                    if (!contractEndPoints.TryGetValue(contractType, out endPoints))
                    {
                        endPoints = new List<string>();
                        contractEndPoints.Add(contractType, endPoints);
                    }
                    endPoints.Add(endpoint.Name);
                }
            }
            foreach (var contractEndPoint in contractEndPoints)
            {
                if (contractEndPoint.Value.Count == 1)
                {
                    var endpointConfigurationName = contractEndPoint.Value[0];
                    var serviceType = ClientServiceFactory.GetServiceType(contractEndPoint.Key, endpointConfigurationName);
                    container.Register(contractEndPoint.Key, serviceType, string.IsNullOrEmpty(endpointConfigurationName) ? null : endpointConfigurationName);
                    if (!string.IsNullOrEmpty(endpointConfigurationName))
                    {
                        container.Register(contractEndPoint.Key, serviceType);
                    }
                }
                else
                {
                    foreach (var endpointConfigurationName in contractEndPoint.Value)
                    {
                        var serviceType = ClientServiceFactory.GetServiceType(contractEndPoint.Key, endpointConfigurationName);
                        container.Register(contractEndPoint.Key, serviceType, string.IsNullOrEmpty(endpointConfigurationName) ? null : endpointConfigurationName);
                    }
                }
            }
        }
    }
}
