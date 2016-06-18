using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheatech.ServiceModel.UnitTests
{
    public class ObjectWithInjection
    {
        [Injection]
        public ObjectWithInjection(ILogger logger)
        {
            InjectionFromConstructor = logger;
        }

        public ObjectWithInjection(ICanChangeParameters obj)
        {
            NotInjectionFromConstructor = obj;
        }

        public ILogger InjectionFromConstructor { get; set; }

        public ICanChangeParameters NotInjectionFromConstructor { get; set; }

        [Injection]
        public ILogger InjectionFromProperty { get; set; }

        public ILogger NotInjectionFromProperty { get; set; }
    }
}
