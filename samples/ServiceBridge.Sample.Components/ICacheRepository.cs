using ServiceBridge.Interceptors;

namespace ServiceBridge.Sample.Components
{
    public interface ICacheRepository
    {
        object GetVale(string key);

        [TransactionScope]
        void SetVale(string key, object value);
    }
}
