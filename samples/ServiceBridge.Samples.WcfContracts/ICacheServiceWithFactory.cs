using System.ServiceModel;

namespace ServiceBridge.Samples.WcfContracts
{
    [ServiceContract(ConfigurationName = "ICacheServiceWithFactory")]
    public interface ICacheServiceWithFactory
    {
        [OperationContract]
        string GetVale(string key);

        [OperationContract]
        void SetVale(string key, string value);
    }
}
