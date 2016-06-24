using System.Web.Http;
using Wheatech.ServiceModel.Sample.Components;

namespace Wheatech.ServiceModel.Samples.WebApi
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