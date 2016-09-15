using System.Collections;

namespace ServiceBridge.Sample.Components
{
    public class CacheRepository : ICacheRepository
    {
        private readonly Hashtable _cache = new Hashtable();

        public virtual object GetVale(string key)
        {
            return _cache[key];
        }

        public virtual void SetVale(string key, object value)
        {
            _cache[key] = value;
        }
    }
}