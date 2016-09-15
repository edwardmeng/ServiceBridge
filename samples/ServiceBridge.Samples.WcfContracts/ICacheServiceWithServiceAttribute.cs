using System.ServiceModel;

namespace ServiceBridge.Samples.WcfContracts
{
    [ServiceContract(ConfigurationName = "ICacheServiceWithServiceAttribute")]
    public interface ICacheServiceWithServiceAttribute
    {
        [OperationContract]
        string GetVale(string key);

        [OperationContract]
        void SetVale(string key, string value);
    }
}
