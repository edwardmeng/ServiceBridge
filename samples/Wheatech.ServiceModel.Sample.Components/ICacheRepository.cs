using Wheatech.ServiceModel.Interception;

namespace Wheatech.ServiceModel.Sample.Components
{
    public interface ICacheRepository
    {
        object GetVale(string key);

        [TransactionScope]
        void SetVale(string key, object value);
    }
}
