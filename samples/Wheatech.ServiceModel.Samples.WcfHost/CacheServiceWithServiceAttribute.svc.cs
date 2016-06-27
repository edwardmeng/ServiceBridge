using System.ServiceModel;
using Wheatech.ServiceModel.Sample.Components;
using Wheatech.ServiceModel.Samples.WcfContracts;
using Wheatech.ServiceModel.Wcf;

namespace Wheatech.ServiceModel.Samples.WcfHost
{
    [ContainerService]
    [ServiceBehavior(ConfigurationName = "CacheServiceWithServiceAttribute")]
    public class CacheServiceWithServiceAttribute : ICacheServiceWithServiceAttribute
    {
        [Injection]
        public ICacheRepository Repository { get; set; }

        public string GetVale(string key)
        {
            return (string)Repository.GetVale(key);
        }

        public void SetVale(string key, object value)
        {
            Repository.SetVale(key, value);
        }
    }
}
