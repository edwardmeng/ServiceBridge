using System;
using StructureMap;
using StructureMap.Pipeline;

namespace Wheatech.ServiceModel.StructureMap
{
    public class InjectionMethodPolicy:IInstancePolicy
    {
        public void Apply(Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }
    }
}
