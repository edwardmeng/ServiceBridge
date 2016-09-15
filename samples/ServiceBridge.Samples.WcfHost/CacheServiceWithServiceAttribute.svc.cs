using System.ServiceModel;
using ServiceBridge.Sample.Components;
using ServiceBridge.Samples.WcfContracts;
using ServiceBridge.ServiceModel;

namespace ServiceBridge.Samples.WcfHost
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

        public void SetVale(string key, string value)
        {
            Repository.SetVale(key, value);
        }
    }
}
