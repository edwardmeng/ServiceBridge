using System;
using System.Linq;
using System.Reflection;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace ServiceBridge.StructureMap
{
    internal class InjectionConstructorSelector : IConstructorSelector
    {
        public ConstructorInfo Find(Type pluggedType, DependencyCollection dependencies, PluginGraph graph)
        {
            var constructors = InjectionAttribute.GetConstructors(pluggedType).ToArray();
            if (constructors.Length == 0) return null;
            return (from constructor in constructors
                    orderby constructor.GetParameters().Length descending
                    select constructor).FirstOrDefault();
        }
    }
}
