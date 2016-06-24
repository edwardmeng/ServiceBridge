using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Wheatech.Activation;

[assembly: AssemblyActivator(typeof(Wheatech.ServiceModel.Mvc.ServiceModelActivator))]

namespace Wheatech.ServiceModel.Mvc
{
    internal class ServiceModelActivator
    {
        public void Configuration(IActivatingEnvironment environment, IServiceContainer container)
        {
            // We have to register the controllers at the application configuration stage.
            // Since there are some IoC implementations cannot register types after resolve instances.
            foreach (var assembly in environment.GetAssemblies())
            {
                if (!assembly.IsDynamic)
                {
                    IEnumerable<TypeInfo> types;
                    try
                    {
                        types = assembly.DefinedTypes;
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        types = ex.Types.TakeWhile(type => type != null).Select(type => type.GetTypeInfo());
                    }
                    foreach (var type in types)
                    {
                        if (!type.IsInterface && !type.IsAbstract && type.IsClass && type != typeof(Controller) && typeof(Controller).IsAssignableFrom(type))
                        {
                            container.Register(type, null, ServiceLifetime.PerRequest);
                        }
                    }
                }
            }
            DependencyResolver.SetResolver(new ServiceModelDependencyResolver());
        }
    }
}
