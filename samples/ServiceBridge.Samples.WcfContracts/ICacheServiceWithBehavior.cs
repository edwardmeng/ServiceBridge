using System.ServiceModel;

namespace ServiceBridge.Samples.WcfContracts
{
    [ServiceContract(ConfigurationName = "ICacheServiceWithBehavior")]
    public interface ICacheServiceWithBehavior
    {
        [OperationContract]
        string GetVale(string key);

        [OperationContract]
        void SetVale(string key, string value);
    }
}
