using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Wheatech.Hosting;

[assembly: AssemblyStartup(typeof(Wheatech.ServiceModel.Wcf.ServiceModelStartup))]

namespace Wheatech.ServiceModel.Wcf
{
    internal class ServiceModelStartup
    {
        public void Configure(IHostingEnvironment environment, IServiceContainer container)
        {
            var types = new List<TypeInfo>();
            foreach (Assembly assembly in environment.GetAssemblies())
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
            ConfigureServices(container, (ServicesSection)ConfigurationManager.GetSection("system.serviceModel/services"), types.ToArray());
            ConfigureClients(container, (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client"), types.ToArray());
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

        private void ConfigureServices(IServiceContainer container, ServicesSection section, TypeInfo[] types)
        {
            var contractTypes = new Dictionary<string, Type>();
            var serviceTypes = new Dictionary<string, Type>();
            foreach (var type in types)
            {
                var contractAttribute = type.GetCustomAttribute<ServiceContractAttribute>(false);
                if (contractAttribute != null)
                {
                    contractTypes.Add(contractAttribute.Name, type);
                }
                var behaviorAttribute = type.GetCustomAttribute<ServiceBehaviorAttribute>(false);
                if (behaviorAttribute != null)
                {
                    serviceTypes.Add(behaviorAttribute.Name, type);
                }
            }
            for (int i = 0; i < section.Services.Count; i++)
            {
                var element = section.Services[i];
                Type serviceType;
                if (!serviceTypes.TryGetValue(element.Name, out serviceType))
                {
                    serviceType = GetTypeFromName(element.Name, types);
                }
                if (serviceType != null)
                {
                    var attribute = serviceType.GetCustomAttribute<ServiceBehaviorAttribute>();
                    var lifetime = ServiceLifetime.PerThread;
                    if (attribute != null)
                    {
                        switch (attribute.InstanceContextMode)
                        {
                            case InstanceContextMode.PerCall:
                                lifetime = ServiceLifetime.Transient;
                                break;
                            case InstanceContextMode.Single:
                                lifetime = ServiceLifetime.Singleton;
                                break;
                            case InstanceContextMode.PerSession:
                                lifetime = ServiceLifetime.PerThread;
                                break;
                        }
                    }
                    for (int j = 0; j < element.Endpoints.Count; j++)
                    {
                        var endpoint = element.Endpoints[j];
                        Type contractType;
                        if (!contractTypes.TryGetValue(endpoint.Contract, out contractType))
                        {
                            contractType = GetTypeFromName(endpoint.Contract, types);
                        }
                        if (contractType != null)
                        {
                            container.Register(contractType, serviceType, endpoint.Address?.ToString(), lifetime);
                        }
                    }
                }
            }
        }

        private void ConfigureClients(IServiceContainer container, ClientSection section, TypeInfo[] types)
        {
            var contractTypes = new Dictionary<string, Type>();
            foreach (var type in types)
            {
                var contractAttribute = type.GetCustomAttribute<ServiceContractAttribute>(false);
                if (contractAttribute != null)
                {
                    contractTypes.Add(contractAttribute.Name, type);
                }
            }
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
                    container.Register(contractType, ClientServiceFactory.GetServiceType(contractType), endpoint.Name);
                }
            }
        }
    }
}
