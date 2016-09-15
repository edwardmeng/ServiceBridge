using System.Web.Http;
using ServiceBridge.Sample.Components;

namespace ServiceBridge.Samples.WebApi
{
    public class CacheController : ApiController
    {
        [Injection]
        public ICacheRepository Repository { get; set; }

        public string Get()
        {
            return (string)Repository.GetVale("Sample");
        }
    }
}