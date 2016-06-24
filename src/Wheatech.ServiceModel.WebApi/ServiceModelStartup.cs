using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using Wheatech.Hosting;

namespace Wheatech.ServiceModel.WebApi
{
    internal class ServiceModelStartup
    {
        public void Configure(IHostingEnvironment environment, IServiceContainer container)
        {
            foreach (var assembly in environment.GetAssemblies())
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
                    if (!type.IsInterface && !type.IsAbstract && type.IsClass && typeof(IHttpController).IsAssignableFrom(type))
                    {
                        container.Register(type, null, ServiceLifetime.PerRequest);
                    }
                }
            }
        }
    }
}
