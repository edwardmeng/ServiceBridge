using System.ServiceModel;
using ServiceBridge.Sample.Components;
using ServiceBridge.Samples.WcfContracts;

namespace ServiceBridge.Samples.WcfHost
{
    [ServiceBehavior(ConfigurationName = "CacheServiceWithBehavior")]
    public class CacheServiceWithBehavior : ICacheServiceWithBehavior
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
