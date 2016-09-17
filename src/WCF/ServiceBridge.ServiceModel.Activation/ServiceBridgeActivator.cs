using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using MassActivation;

[assembly: AssemblyActivator(typeof(ServiceBridge.ServiceModel.ServiceBridgeActivator))]

namespace ServiceBridge.ServiceModel
{
    internal class ServiceBridgeActivator
    {
        public void Configuration(IActivatingEnvironment environment, IServiceContainer container)
        {
            // We have to register the services at the application configuration stage.
            // Since there are some IoC implementations cannot register types after resolve instances.
            var types = new List<Type>();
            foreach (var assembly in environment.GetAssemblies())
            {
                if (!assembly.IsDynamic)
                {
                    IEnumerable<Type> assemblyTypes;
                    try
                    {
                        assemblyTypes = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        assemblyTypes = ex.Types.TakeWhile(type => type != null);
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

        private Type GetTypeFromName(string typeName, Type[] types)
        {
            var index = CommaIndexInTypeName(typeName);
            return index > 0 ? Type.GetType(typeName, false, false) : types.FirstOrDefault(type => type.FullName == typeName);
        }

        private void ConfigureServices(IServiceContainer container, Type[] types)
        {
            foreach (var serviceType in types)
            {
                if (ServiceUtils.IsValidService(serviceType))
                {
                    container.RegisterWcfServiceInternal(serviceType);
                }
            }
        }

        private void ConfigureClients(IServiceContainer container, Type[] types)
        {
            var section = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");
            var contractTypes = new Dictionary<string, Type>();
            foreach (var contractType in types)
            {
                var contractAttribute = (ServiceContractAttribute)Attribute.GetCustomAttribute(contractType, typeof(ServiceContractAttribute), false);
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
