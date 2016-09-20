namespace ServiceBridge.Samples.AspNetCore
{
    public interface ICacheRepository
    {
        object GetVale(string key);

        void SetVale(string key, object value);
    }
}
